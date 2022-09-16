using System;
using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Models;
using System.Numerics;

namespace ProjectRAM.Core.Commands.Models;

public class DivCommand : MathCommandBase
{
	public DivCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override void Calculate(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		try
		{
			base.Calculate(getMemory, setMemory);
			var res = (BigInteger.Parse(_accumulator) / BigInteger.Parse(_secondValue)).ToString();
			setMemory(Interpreter.AccumulatorAddress, res);
		}
		catch (DivideByZeroException)
		{
			throw new DivByZero(Line);
		}
		catch (FormatException)
		{
			throw new ValueIsNaN(Line);
		}
	}

	public override void ValidateArgument()
	{
		base.ValidateArgument();
		if (ArgumentType == ArgumentType.Const && FormattedArgument.IsZero())
		{
			throw new DivByZero(Line);
		}
	}
}