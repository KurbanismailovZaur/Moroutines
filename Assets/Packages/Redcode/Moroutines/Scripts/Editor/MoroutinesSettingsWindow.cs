using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Redcode.Moroutines.Editor
{
    internal class MoroutinesSettingsWindow : EditorWindow
    {
        private static MoroutinesSettingsWindow _window;
        private static MoroutinesSettings _settings;

        [MenuItem("Window/Moroutines/Settings")]
        private static void ShowWindow()
        {
            if (_window != null)
                return;

            _window = (MoroutinesSettingsWindow)GetWindow(typeof(MoroutinesSettingsWindow));
            _window.titleContent = new GUIContent("Moroutines Settings");
            _window.Show();
        }

        private void OnEnable() => _settings = Resources.Load<MoroutinesSettings>("Redcode/Moroutines/Settings");

        private void OnDisable() => _window = null;

        private void OnGUI()
        {
            _settings.HideMoroutinesExecuter = EditorGUILayout.Toggle("Hide Moroutines Executer", _settings.HideMoroutinesExecuter);
        }
    }
}