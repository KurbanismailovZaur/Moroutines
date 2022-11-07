using UnityEngine;

namespace Redcode.Moroutines.Extensions
{
    public static class CustomYieldInstructionExtensions
	{
		/// <summary>
		/// Returns the moroutine that represents the CustomYieldInstruction object.
		/// </summary>
		/// <param name="instruction">CustomYieldInstruction object.</param>
		/// <returns>The moroutine.</returns>
		public static Moroutine AsMoroutine(this CustomYieldInstruction instruction) => Moroutine.Run(instruction);

		/// <summary>
		/// Returns the YieldInstruction object that represents the CustomYieldInstruction object.
		/// </summary>
		/// <param name="instruction">CustomYieldInstruction object.</param>
		/// <returns>The YieldInstruction object.</returns>
		public static YieldInstruction AsYieldInstruction(this CustomYieldInstruction instruction) => instruction.AsMoroutine().WaitForComplete();
	}
}