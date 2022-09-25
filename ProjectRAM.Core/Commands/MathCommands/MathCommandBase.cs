using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectRAM.Core.Commands.MathCommands;

internal abstract class MathCommandBase : CommandBase
{
    private protected string? Accumulator, SecondValue;

    protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
    {
        ArgumentType.Const,
        ArgumentType.DirectAddress,
        ArgumentType.IndirectAddress
    };

    protected MathCommandBase(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        Accumulator = interpreter.GetMemory(interpreter.AccumulatorAddress);
        SecondValue = GetValue(interpreter);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }
    
    
    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return interpreter.GetMemory(interpreter.AccumulatorAddress).LCost() + LCostHelper(interpreter);
    }
}