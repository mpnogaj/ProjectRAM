using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.Abstractions;

internal abstract class MemoryManagementCommand : CommandBase
{
	public MemoryManagementCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public abstract ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory);

	public override void ValidateArgument()
	{
		if (ArgumentType is not ArgumentType.Const and not ArgumentType.DirectAddress
		    and not ArgumentType.IndirectAddress)
		{
			throw new ArgumentIsNotValidException(Line);
		}
	}
}