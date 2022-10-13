using System;
using System.Numerics;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

public abstract class NumberArgumentCommandBase : CommandBase
{
	//temporary value
	//if argument is NaN it will be detected in ValidateArgument method during validation process
	public BigInteger NumberArgument { get; } = BigInteger.Zero;
	
	protected NumberArgumentCommandBase(long line, string? label, string argument) : base(line, label, argument)
	{
		if (BigInteger.TryParse(FormattedArgument, out var res))
		{
			NumberArgument = res;
		}
	}
	
	internal BigInteger GetValue(IInterpreter interpreter)
		=> ArgumentType switch
		{
			ArgumentType.Const => NumberArgument,
			ArgumentType.DirectAddress => interpreter.Memory.GetMemory(NumberArgument, Line),
			ArgumentType.IndirectAddress => interpreter.Memory.GetIndirectMemory(NumberArgument, Line),
			_ => throw new InvalidOperationException()
		};

	internal BigInteger GetAddress(IInterpreter interpreter) 
		=> ArgumentType switch
		{
			ArgumentType.DirectAddress => NumberArgument,
			ArgumentType.IndirectAddress => interpreter.Memory.GetMemory(NumberArgument, Line),
			_ => throw new InvalidOperationException()
		};
	
	internal ulong LCostHelper(IInterpreter interpreter)
	{
		switch (ArgumentType)
		{
			case ArgumentType.Const:
				return NumberArgument.LCost();
			case ArgumentType.DirectAddress:
				return NumberArgument.LCost() + interpreter.Memory.GetMemory(NumberArgument, Line).LCost();
			case ArgumentType.IndirectAddress:
				var res = interpreter.Memory.GetMemory(NumberArgument, Line);
				return NumberArgument.LCost() + res.LCost() + interpreter.Memory.GetMemory(res, Line).LCost();
			default:
				throw new InvalidOperationException();
		}
	}
}