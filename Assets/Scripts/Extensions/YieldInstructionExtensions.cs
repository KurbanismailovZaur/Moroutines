using UnityEngine;

namespace Redcode.Moroutines.Extensions
{
    public static class YieldInstructionExtensions
    {
        public static Moroutine AsMoroutine(this YieldInstruction instruction) => Moroutine.Run(Routines.Wait(instruction));

        public static CustomYieldInstruction AsCustomYieldInstruction(this YieldInstruction instruction)
        {
            var coroutine = instruction.AsMoroutine();
            return new WaitUntil(() => coroutine.IsCompleted);
        }
    }
}