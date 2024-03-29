﻿using System.Collections.Generic;
using ProjectRAM.Core.Machine.Abstraction;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

[CommandName("halt")]
internal class HaltCommand : CommandBase
{
	public HaltCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
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