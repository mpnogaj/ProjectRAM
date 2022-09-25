using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

[CommandName("halt")]
internal class HaltCommand : CommandBase
{
	public HaltCommand(long line, string? label, string argument) : base(line, label, argument)
	{
		
	}

	protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
	{
		ArgumentType.Null
	};

	public override void Execute(IInterpreter interpreter)
	{
		UpdateComplexity(interpreter);
		interpreter.StopProgram();
	}

	protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
	{
		return 1;
	}
}