using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

[CommandName("load")]
internal class LoadCommand : MemoryManagementCommandBase
{
    public LoadCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
    {
        var value = ArgumentType switch
        {
            ArgumentType.DirectAddress => getMemory(FormattedArgument, Line),
            ArgumentType.IndirectAddress => getMemory(getMemory(FormattedArgument, Line), Line),
            ArgumentType.Const => FormattedArgument,
            _ => throw new ArgumentIsNotValidException(Line)
        };

        setMemory(Constants.AccumulatorAddress, value);

        return LCostHelper(getMemory);
    }
}