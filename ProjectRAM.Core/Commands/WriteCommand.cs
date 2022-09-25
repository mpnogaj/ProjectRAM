using System.Collections.Generic;
using ProjectRAM.Core.Models;
namespace ProjectRAM.Core.Commands;

[CommandName("write")]
internal class WriteCommand : CommandBase
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
			ArgumentType.DirectAddress => interpreter.GetMemory(FormattedArgument),
			ArgumentType.IndirectAddress => interpreter.GetMemory(interpreter.GetMemory(FormattedArgument)),
			ArgumentType.Const => FormattedArgument,
			_ => throw new ArgumentIsNotValidException(Line)
		};
		interpreter.WriteToTape(value);
		UpdateComplexity(interpreter);
		interpreter.IncreaseExecutionCounter();
	}

	protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
	{
		return LCostHelper(interpreter);
	}
}