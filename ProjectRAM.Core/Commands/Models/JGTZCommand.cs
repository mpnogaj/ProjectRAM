using System;
using ProjectRAM.Core.Commands.Abstractions;

namespace ProjectRAM.Core.Commands.Models;

public class JGTZCommand : JumpCommandBase
{
	public JGTZCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override ulong Execute(string accumulator, Action<string> makeJump)
	{
		if (accumulator.IsPositive(Line))
		{
			makeJump(FormattedArgument);
		}

		return accumulator.LCost();
	}
}