using System;
using System.Numerics;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.JumpCommands;

[CommandName("jzero")]
internal class JZeroCommand : JumpCommandBase
{
    public JZeroCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        UpdateComplexity(interpreter);
        var accumulator = interpreter.Memory.GetAccumulator(Line);
        if (accumulator == BigInteger.Zero)
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