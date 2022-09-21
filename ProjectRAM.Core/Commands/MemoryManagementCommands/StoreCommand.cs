using ProjectRAM.Core.Models;
using System;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

[CommandName("store")]
internal class StoreCommand : MemoryManagementCommandBase
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

    public override ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
    {
        var target = ArgumentType switch
        {
            ArgumentType.DirectAddress => FormattedArgument,
            ArgumentType.IndirectAddress => getMemory(FormattedArgument, Line),
            _ => throw new ArgumentIsNotValidException(Line),
        };

        var accumulatorValue = getMemory(Interpreter.AccumulatorAddress, Line);

        setMemory(target, accumulatorValue);

        return ArgumentType switch
        {
            ArgumentType.DirectAddress => getMemory(Interpreter.AccumulatorAddress, Line).LCost() +
                                          FormattedArgument.LCost(),
            ArgumentType.IndirectAddress => getMemory(Interpreter.AccumulatorAddress, Line).LCost() +
                                            FormattedArgument.LCost() + getMemory(FormattedArgument, Line).LCost(),
            _ => throw new ArgumentIsNotValidException(Line),
        };
    }
}