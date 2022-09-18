using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands;

[CommandName("write")]
internal class WriteCommand : CommandBase
{
	public WriteCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public override void ValidateArgument()
	{
		if (ArgumentType is not ArgumentType.DirectAddress
			and not ArgumentType.IndirectAddress
			and not ArgumentType.Const)
		{
			throw new ArgumentIsNotValidException(Line);
		}
	}

	public ulong Execute(Func<string, long, string> getMemory,
		EventHandler<WriteToTapeEventArgs>? readEventHandler)
	{
		if (readEventHandler == null)
		{
			throw new InputTapeEmptyException(Line);
		}

		var value = ArgumentType switch
		{
			ArgumentType.DirectAddress => getMemory(FormattedArgument, Line),
			ArgumentType.IndirectAddress => getMemory(getMemory(FormattedArgument, Line), Line),
			ArgumentType.Const => FormattedArgument,
			_ => throw new ArgumentIsNotValidException(Line)
		};

		var eventArgs = new WriteToTapeEventArgs(value);
		readEventHandler.Invoke(this, eventArgs);
		return LCostHelper(getMemory);
	}
}