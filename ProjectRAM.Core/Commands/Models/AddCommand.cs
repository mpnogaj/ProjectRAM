using System;
using System.Numerics;
using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands.Models;

internal class AddCommand : MathCommandBase
{
	public AddCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override void Calculate(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		base.Calculate(getMemory, setMemory);
		try
		{
			var res = (BigInteger.Parse(_accumulator) + BigInteger.Parse(_secondValue)).ToString();
			setMemory(Interpreter.AccumulatorAddress, res);
		}
		catch (FormatException)
		{
			throw new ValueIsNaN(Line);
		}
	}

	public override long CalculateComplexity(Func<string, long, string> getMemory)
	{
		long cost1 = Form

		return ArgumentType switch
		{
			ArgumentType.DirectAddress => expr,
			ArgumentType.IndirectAddress => expr,
			ArgumentType.Const => 0,
			_ => throw new ArgumentIsNotValidException()
		};
		return getMemory(Interpreter.AccumulatorAddress, Line).Lcost();
	}
}