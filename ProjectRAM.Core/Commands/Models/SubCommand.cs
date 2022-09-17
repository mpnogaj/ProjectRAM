using System;
using System.Numerics;
using ProjectRAM.Core.Commands.Abstractions;

namespace ProjectRAM.Core.Commands.Models;

internal class SubCommand : MathCommandBase
{
	public SubCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		try
		{
			var complexity = base.Execute(getMemory, setMemory);
			var res = (BigInteger.Parse(_accumulator) - BigInteger.Parse(_secondValue)).ToString();
			setMemory(Interpreter.AccumulatorAddress, res);
			return complexity;
		}
		catch (FormatException)
		{
			throw new ValueIsNaN(Line);
		}
	}
}