using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

internal class EmptyCommand : CommandBase
{
	public EmptyCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
	{
	}

	protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
	{
		ArgumentType.Null
	};

	public override void Execute(IInterpreter interpreter)
	{
		interpreter.IncreaseExecutionCounter();
	}

	protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
	{
		return 0ul;
	}
}