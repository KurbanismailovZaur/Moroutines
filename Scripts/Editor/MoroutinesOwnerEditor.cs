using Redcode.Moroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Redcode.Moroutines.Editor
{
    [CustomEditor(typeof(MoroutinesOwner))]
    public class MoroutinesOwnerEditor : UnityEditor.Editor
    {
        private SerializedProperty _moroutinesProperty;

        private bool showDetails;

        private void OnEnable()
        {
            _moroutinesProperty = serializedObject.FindProperty("_moroutines");

            EditorApplication.update += Update;
        }

        private void OnDisable() => EditorApplication.update -= Update;

        private void Update() => EditorUtility.SetDirty(serializedObject.targetObject);
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((MoroutinesOwner)target), typeof(MoroutinesOwner), false);
            GUI.enabled = true;

            GUILayout.Space(5f);

            #region Main line
            GUILayout.BeginHorizontal();

            var showButtonText = $"{(showDetails ? "Hide" : "Show")} Moroutines";
            if (GUILayout.Button(showButtonText, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent(showButtonText)).x + 10f)))
                showDetails = !showDetails;

            GUILayout.BeginHorizontal(EditorStyles.label);
            GUILayout.Label($":  {_moroutinesProperty.arraySize}");
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(2f);

            if (!showDetails)
                return;

            var columnWidth = (Screen.width - 22f) / 4f;
            var columnOptions = GUILayout.Width(columnWidth);

            #region Headers
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Index", EditorStyles.boldLabel, columnOptions);
            GUILayout.Label("Name", EditorStyles.boldLabel, columnOptions);
            GUILayout.Label("State", EditorStyles.boldLabel, columnOptions);
            GUILayout.Label("Last Result", EditorStyles.boldLabel, columnOptions);
            GUILayout.EndHorizontal();
            #endregion

            for (int i = 0; i < _moroutinesProperty.arraySize; i++)
            {
                var moroutine = (Moroutine)_moroutinesProperty.GetArrayElementAtIndex(i).managedReferenceValue;

                GUILayout.BeginHorizontal();
                #region Index
                GUILayout.Label(i.ToString(), EditorStyles.label, columnOptions);
                #endregion

                #region Name
                var oldColor = GUI.contentColor;
                GUI.contentColor = moroutine.Name == null ? Color.gray : Color.white;

                var name = string.IsNullOrEmpty(moroutine.Name) ? "[noname]" : moroutine.Name;
                GUILayout.Label(new GUIContent(name, name), EditorStyles.label, columnOptions);

                GUI.contentColor = oldColor;
                #endregion

                #region State
                oldColor = GUI.contentColor;
                GUI.contentColor = moroutine.CurrentState switch
                {
                    Moroutine.State.Reseted => Color.gray,
                    Moroutine.State.Running => Color.white,
                    Moroutine.State.Stopped => Color.yellow,
                    Moroutine.State.Completed => Color.green,
                    _ => Color.white
                };

                var state = moroutine.CurrentState.ToString();
                GUILayout.Label(new GUIContent(state, state), EditorStyles.label, columnOptions);

                GUI.contentColor = oldColor;
                #endregion

                #region LastResult
                var lastResult = moroutine.LastResult?.ToString() ?? "null";
                GUILayout.Label(new GUIContent(lastResult, lastResult), EditorStyles.label, columnOptions);
                #endregion
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }


    }
}