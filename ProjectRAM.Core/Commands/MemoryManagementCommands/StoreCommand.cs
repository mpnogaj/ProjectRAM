using ProjectRAM.Core.Models;
using System.Collections.Generic;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

[CommandName("store")]
internal class StoreCommand : NumberArgumentCommandBase
{
    public StoreCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
    {
        ArgumentType.DirectAddress,
        ArgumentType.IndirectAddress
    };

    public override void Execute(IInterpreter interpreter)
    {
        var target = GetAddress(interpreter);
        var accumulatorValue = interpreter.Memory.GetAccumulator(Line);
        interpreter.Memory.SetMemory(target, accumulatorValue);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return ArgumentType switch
        {
            ArgumentType.DirectAddress => interpreter.Memory.GetAccumulator(Line).LCost() +
                                          NumberArgument.LCost(),
            ArgumentType.IndirectAddress => interpreter.Memory.GetAccumulator(Line).LCost() +
                                            NumberArgument.LCost() + interpreter.Memory.GetMemory(NumberArgument, Line)
                                                .LCost(),
            _ => throw new ArgumentIsNotValidException(Line),
        };
    }
}