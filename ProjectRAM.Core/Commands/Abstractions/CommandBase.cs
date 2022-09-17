using ProjectRAM.Core.Models;
using System;
using System.Linq;

namespace ProjectRAM.Core.Commands.Abstractions;

public abstract class CommandBase
{
	public long Line { get; }
	public string? Label { get; }
	public string Argument { get; }
	public ArgumentType ArgumentType { get; }
	public string FormattedArgument { get; }

	protected CommandBase(long line, string? label, string argument)
	{
		Line = line;
		Label = label;
		Argument = argument;
		ArgumentType = argument.GetArgumentType();
		FormattedArgument = GetFormattedArgument();
	}

	public abstract void ValidateArgument();

	protected ulong LCostHelper(Func<string, long, string> getMemory)
	{
		switch (ArgumentType)
		{
			case ArgumentType.Const:
				return FormattedArgument.LCost();
			case ArgumentType.DirectAddress:
				return FormattedArgument.LCost() + getMemory(FormattedArgument, Line).LCost();
			case ArgumentType.IndirectAddress:
				var res = getMemory(FormattedArgument, Line);
				return FormattedArgument.LCost() + res.LCost() + getMemory(res, Line).LCost();
			default:
				throw new ArgumentIsNotValidException(Line);
		}
	}

	public void ValidateLabel()
	{
		if (Label != null && !Label.All(char.IsLetterOrDigit))
		{
			throw new LabelIsNotValidException(Line);
		}
	}

	private string GetFormattedArgument()
	{
		return ArgumentType switch
		{
			ArgumentType.Null => string.Empty,
			ArgumentType.Label or ArgumentType.DirectAddress => Argument,
			ArgumentType.IndirectAddress or ArgumentType.Const => Argument[1..],
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}