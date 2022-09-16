using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands.Models;

internal class StoreCommand : MemoryManagementCommand
{
	public StoreCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override void ValidateArgument()
	{
		base.ValidateArgument();
		if (ArgumentType is ArgumentType.Const)
		{
			throw new ArgumentIsNotValidException(Line);
		}
	}

	public override void Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		var target = ArgumentType switch
		{
			ArgumentType.DirectAddress => FormattedArgument,
			ArgumentType.IndirectAddress => getMemory(FormattedArgument, Line),
			_ => throw new ArgumentIsNotValidException(Line),
		};

		var accumulatorValue = getMemory(Interpreter.AccumulatorAddress, Line);

		setMemory(target, accumulatorValue);
	}
}