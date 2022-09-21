using System;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.JumpCommands;

[CommandName("jzero")]
internal class JZeroCommand : JumpCommandBase
{
    public JZeroCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override ulong Execute(string accumulator, Action<string> makeJump)
    {
        if (accumulator.IsZero())
        {
            makeJump(FormattedArgument);
        }

        return accumulator.LCost();
    }
}