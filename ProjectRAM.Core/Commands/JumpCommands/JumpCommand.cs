using System;

namespace ProjectRAM.Core.Commands.JumpCommands;

[CommandName("jump")]
internal class JumpCommand : JumpCommandBase
{
    public JumpCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override ulong Execute(string accumulator, Action<string> makeJump)
    {
        ValidateArgument();
        makeJump(FormattedArgument);
        return 1;
    }
}