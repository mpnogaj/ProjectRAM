using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

internal abstract class MemoryManagementCommandBase : CommandBase
{
    public MemoryManagementCommandBase(long line, string? label, string argument) : base(line, label, argument)
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