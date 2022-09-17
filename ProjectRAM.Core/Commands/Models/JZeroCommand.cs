using ProjectRAM.Core.Commands.Abstractions;
using System;

namespace ProjectRAM.Core.Commands.Models;

internal class JZeroCommand : JumpCommandBase
{
	public JZeroCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override ulong Execute(string accumulator, Action<string> makeJump)
	{
		if (accumulator.IsZero(Line))
		{
			makeJump(FormattedArgument);
		}

		return accumulator.LCost();
	}
}