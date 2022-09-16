using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.Models;

internal class LoadCommand : MemoryManagementCommand
{
	public LoadCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override void Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
	{
		var value = ArgumentType switch
		{
			ArgumentType.DirectAddress => getMemory(FormattedArgument, Line),
			ArgumentType.IndirectAddress => getMemory(getMemory(FormattedArgument, Line), Line),
			ArgumentType.Const => FormattedArgument,
			_ => throw new ArgumentIsNotValidException(Line)
		};

		setMemory(Interpreter.AccumulatorAddress, value);
	}
}