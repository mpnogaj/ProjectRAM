using System;
using System.Numerics;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Machine.Abstraction;

namespace ProjectRAM.Core.Commands.JumpCommands;

[CommandName("jgtz")]
internal class JGTZCommand : JumpCommandBase
{
    public JGTZCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
    {
    }
    
    public override void Execute(IInterpreter interpreter)
    {
        UpdateComplexity(interpreter);
        var accumulator = interpreter.Memory.GetAccumulator(Line);
        if (accumulator > BigInteger.Zero)
        {
            interpreter.MakeJump(FormattedArgument);
        }
        else
        {
            interpreter.IncreaseExecutionCounter();
        }
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return interpreter.Memory.GetAccumulator(Line).LCost();
    }
}