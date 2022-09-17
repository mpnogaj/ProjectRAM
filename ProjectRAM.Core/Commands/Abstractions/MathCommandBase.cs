using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.Abstractions;

public abstract class MathCommandBase : CommandBase
{
	private protected string _accumulator = Interpreter.UninitializedValue,
		_secondValue = Interpreter.UninitializedValue;

	protected MathCommandBase(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public virtual ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		_accumulator = getMemory(Interpreter.AccumulatorAddress, Line);
		_secondValue = ArgumentType switch
		{
			ArgumentType.DirectAddress => getMemory(FormattedArgument, Line),
			ArgumentType.IndirectAddress => getMemory(getMemory(FormattedArgument, Line), Line),
			ArgumentType.Const => FormattedArgument,
			_ => throw new ArgumentIsNotValidException(Line)
		};
		return Complexity(getMemory);
	}

	protected ulong Complexity(Func<string, long, string> getMemory) 
		=> getMemory(Interpreter.AccumulatorAddress, Line).LCost() + this.LCostHelper(getMemory);

	public override void ValidateArgument()
	{
		if (ArgumentType != ArgumentType.Const &&
			ArgumentType != ArgumentType.DirectAddress &&
			ArgumentType != ArgumentType.IndirectAddress)
		{
			throw new ArgumentIsNotValidException(Line);
		}
	}
}