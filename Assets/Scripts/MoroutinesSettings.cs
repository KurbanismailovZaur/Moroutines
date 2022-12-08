using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redcode.Moroutines
{
    //[CreateAssetMenu(fileName = "Settings", menuName = "Moroutines/Settings", order = 1)]
    public class MoroutinesSettings : ScriptableObject
    {
        [SerializeField]
        private bool _hideMoroutinesExecuter;

        public bool HideMoroutinesExecuter
        {
            get => _hideMoroutinesExecuter;
            set => _hideMoroutinesExecuter = value;
        }
    }
}
