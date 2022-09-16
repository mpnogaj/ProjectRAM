using ProjectRAM.Core.Commands.Abstractions;

namespace ProjectRAM.Core.Commands.Models;

internal class JumpCommand : JumpCommandBase
{
	public JumpCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override bool CanJump(string accumulatorValue)
	{
		return true;
	}
}