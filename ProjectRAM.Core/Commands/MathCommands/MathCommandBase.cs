using ProjectRAM.Core.Models;
using System.Collections.Generic;
using System.Numerics;

namespace ProjectRAM.Core.Commands.MathCommands;

internal abstract class MathCommandBase : NumberArgumentCommandBase
{
    private protected BigInteger? Accumulator, SecondValue;

    protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
    {
        ArgumentType.Const,
        ArgumentType.DirectAddress,
        ArgumentType.IndirectAddress
    };

    protected MathCommandBase(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        Accumulator = interpreter.Memory.GetAccumulator(Line);
        SecondValue = GetValue(interpreter);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }
    
    
    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return interpreter.Memory.GetAccumulator(Line).LCost() + LCostHelper(interpreter);
    }
}