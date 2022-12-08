using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redcode.Moroutines.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Create moroutines group based on moroutines enumerable.
        /// </summary>
        /// <param name="moroutines">Moroutines which need add to created group.</param>
        /// <returns>Moroutines group.</returns>
        public static MoroutinesGroup ToMoroutinesGroup(this IEnumerable<Moroutine> moroutines) => new(moroutines);
    }
}