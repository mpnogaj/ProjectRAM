using System;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.JumpCommands;

[CommandName("jgtz")]
internal class JGTZCommand : JumpCommandBase
{
    public JGTZCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }
    
    public override void Execute(IInterpreter interpreter)
    {
        UpdateComplexity(interpreter);
        string accumulator = interpreter.GetMemory(interpreter.AccumulatorAddress);
        if (accumulator.IsPositive())
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
        return interpreter.GetMemory(interpreter.AccumulatorAddress).LCost();
    }
}