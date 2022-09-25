using System;
using System.Collections.Generic;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands.JumpCommands;

[CommandName("jump")]
internal class JumpCommand : JumpCommandBase
{
    public JumpCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }
    
    public override void Execute(IInterpreter interpreter)
    {
        UpdateComplexity(interpreter);
        interpreter.MakeJump(FormattedArgument);
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return 1;
    }
}