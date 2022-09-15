using System.Collections.Generic;
using System.Linq;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands.Abstractions;

public abstract class JumpCommandBase : CommandBase
{
	public abstract bool CanJump(string accumulatorValue);

	protected JumpCommandBase(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public void ValidateLabel(Dictionary<string, int> jumpMap)
	{
		if (!jumpMap.ContainsKey(Argument))
		{
			throw new UnknownLabelException(Line, Argument);
		}
	}

	public override void ValidateArgument()
	{
		if (ArgumentType != ArgumentType.Label)
		{
			throw new ArgumentIsNotValidException(Line);
		}
	}
}