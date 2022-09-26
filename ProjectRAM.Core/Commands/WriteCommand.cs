using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

[CommandName("write")]
internal class WriteCommand : NumberArgumentCommandBase
{
	public WriteCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
	{
		ArgumentType.Const,
		ArgumentType.DirectAddress,
		ArgumentType.IndirectAddress
	};

	public override void Execute(IInterpreter interpreter)
	{
		var value = ArgumentType switch
		{
			ArgumentType.DirectAddress => interpreter.Memory.GetMemory(NumberArgument, Line),
			ArgumentType.IndirectAddress => interpreter.Memory.GetMemory(
				interpreter.Memory.GetMemory(NumberArgument, Line), Line),
			ArgumentType.Const => NumberArgument,
			_ => throw new ArgumentIsNotValidException(Line)
		};
		interpreter.WriteToTape(value.ToString());
		UpdateComplexity(interpreter);
		interpreter.IncreaseExecutionCounter();
	}

	protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
	{
		return LCostHelper(interpreter);
	}
}