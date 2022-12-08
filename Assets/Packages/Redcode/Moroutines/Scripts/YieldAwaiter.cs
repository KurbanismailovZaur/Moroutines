using System.Collections;
using UnityEngine;

namespace Redcode.Moroutines
{
    /// <summary>
    /// Base class for awaiting some events on moroutines.
    /// </summary>
    public abstract class YieldAwaiter : YieldInstruction, IEnumerator
    {
        object IEnumerator.Current => null;

        bool IEnumerator.MoveNext() => KeepWaiting;

        void IEnumerator.Reset() { }

        /// <summary>
        /// Should we continue to wait for the event? 
        /// </summary>
        public abstract bool KeepWaiting { get; }
    }
}