using System;
using System.Numerics;
using ProjectRAM.Core.Commands.Abstractions;

namespace ProjectRAM.Core.Commands.Models;

internal class SubCommand : MathCommandBase
{
	public SubCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override void Calculate(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		base.Calculate(getMemory, setMemory);
		try
		{
			var res = (BigInteger.Parse(_accumulator) - BigInteger.Parse(_secondValue)).ToString();
			setMemory(Interpreter.AccumulatorAddress, res);
		}
		catch (FormatException)
		{
			throw new ValueIsNaN(Line);
		}
	}
}