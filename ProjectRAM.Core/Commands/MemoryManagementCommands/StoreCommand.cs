using ProjectRAM.Core.Models;
using System.Collections.Generic;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

[CommandName("store")]
internal class StoreCommand : CommandBase
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
        string target = GetAddress(interpreter);
        string accumulatorValue = interpreter.GetMemory(interpreter.AccumulatorAddress);
        interpreter.SetMemory(target, accumulatorValue);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return ArgumentType switch
        {
            ArgumentType.DirectAddress => interpreter.GetMemory(interpreter.AccumulatorAddress).LCost() +
                                          FormattedArgument.LCost(),
            ArgumentType.IndirectAddress => interpreter.GetMemory(interpreter.AccumulatorAddress).LCost() +
                                            FormattedArgument.LCost() + interpreter.GetMemory(FormattedArgument).LCost(),
            _ => throw new ArgumentIsNotValidException(Line),
        };
    }
}