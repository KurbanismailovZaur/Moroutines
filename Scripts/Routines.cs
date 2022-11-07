using System;
using System.Collections;
using UnityEngine;

namespace Redcode.Moroutines
{
    /// <summary>
    /// Auxiliary class for regular coroutine/moroutine tasks. <br/>
    /// All methods in the class create <see cref="IEnumerable"/> objects.
    /// </summary>
    public static class Routines
    {
        /// <summary>
        /// Create enumerable, which can wait <paramref name="delay"/> time, and then perform <paramref name="action"/>.
        /// </summary>
        /// <param name="delay">Delay before action.</param>
        /// <param name="action">Action to invoke.</param>
        /// <returns>The enumerable.</returns>
        public static IEnumerable Delay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        /// <summary>
        /// Create enumerable, which can wait <paramref name="delay"/> time, and then perform <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="delay">Delay before perform enumerable performing.</param>
        /// <param name="enumerable">Enumerable which will be perform after delay.</param>
        /// <returns><inheritdoc cref="Delay(float, Action)"/></returns>
        public static IEnumerable Delay(float delay, IEnumerable enumerable)
        {
            yield return new WaitForSeconds(delay);

            var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        /// <summary>
        /// Create enumerable, which can wait <paramref name="delay"/> time, and then perform <paramref name="enumerator"/>.
        /// </summary>
        /// <param name="delay">Delay before perform enumerator performing.</param>
        /// <param name="enumerator">Enumerator which will be perform after delay. Enumerator can not be reseted.</param>
        /// <returns><inheritdoc cref="Delay(float, Action)"/></returns>
        public static IEnumerable Delay(float delay, IEnumerator enumerator) => Delay(delay, new EnumerableEnumerator(enumerator));

        /// <summary>
        /// Repeat <paramref name="enumerable"/> <paramref name="count"/> times.
        /// </summary>
        /// <param name="count">How many times <paramref name="enumerable"/> should be repeated?</param>
        /// <param name="enumerable">Enumerable which will be repeated.</param>
        /// <returns><inheritdoc cref="Delay(float, Action)"/></returns>
        public static IEnumerable Repeat(int count, IEnumerable enumerable)
        {
            while (Math.Max(-1, count--) != 0)
            {
                var enumerator = enumerable.GetEnumerator();

                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        /// <summary>
        /// Create enumerable, which can wait <paramref name="count"/> frames, and then perform <paramref name="action"/>.
        /// </summary>
        /// <param name="count">Frames count which will be skiped before <paramref name="action"/> performs.</param>
        /// <param name="action">Action to invoke.</param>
        /// <returns>The enumerable.</returns>
        public static IEnumerable FrameDelay(int count, Action action)
        {
            while (count-- > 0)
                yield return null;

            action();
        }

        /// <summary>
        /// Create enumerable, which can wait <paramref name="count"/> frames, and then perform <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="count">Frames count which will be skiped before <paramref name="enumerable"/> performs.</param>
        /// <param name="enumerable">Enumerable which will be perform after <paramref name="count"/> frames.</param>
        /// <returns><inheritdoc cref="Delay(float, Action)"/></returns>
        public static IEnumerable FrameDelay(int count, IEnumerable enumerable)
        {
            while (count-- > 0)
                yield return null;

            var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        /// <summary>
        /// Create enumerable, which can wait <paramref name="count"/> frames, and then perform <paramref name="enumerator"/>.
        /// </summary>
        /// <param name="count">Frames count which will be skiped before <paramref name="enumerable"/> performs.</param>
        /// <param name="enumerator">Enumerator which will be perform after <paramref name="count"/> frames.</param>
        /// <returns><inheritdoc cref="Delay(float, Action)"/></returns>
        public static IEnumerable FrameDelay(int count, IEnumerator enumerator) => FrameDelay(count, new EnumerableEnumerator(enumerator));

        /// <summary>
        /// Create enumerable, which will wait <see cref="YieldInstruction" object/>
        /// </summary>
        /// <param name="instruction">Instruction for waiting.</param>
        /// <returns>The enumerable.</returns>
        public static IEnumerable Wait(YieldInstruction instruction)
        {
            yield return instruction;
        }

        /// <summary>
        /// Create enumerable, which will wait <see cref="CustomYieldInstruction" object/>
        /// </summary>
        /// <param name="instruction">Instruction for waiting.</param>
        /// <returns>The enumerable.</returns>
        public static IEnumerable Wait(CustomYieldInstruction instruction)
        {
            yield return instruction;
        }
    }
}