using ProjectRAM.Core.Commands.Abstractions;

namespace ProjectRAM.Core.Commands.Models
{
	internal class JZeroCommand : JumpCommandBase
	{
		public JZeroCommand(long line, string? label, string argument) : base(line, label, argument)
		{
		}

		public override bool CanJump(string accumulatorValue)
		{
			if (accumulatorValue == Interpreter.UninitializedValue)
			{
				throw new AccumulatorEmptyException(Line);
			}

			return accumulatorValue.IsZero();
		}
	}
}
