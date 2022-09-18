using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectRAM.Core.Commands.JumpCommands;

public abstract class JumpCommandBase : CommandBase
{
    protected JumpCommandBase(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public abstract ulong Execute(string accumulator, Action<string> makeJump);

    public void ValidateLabel(Dictionary<string, int> jumpMap)
    {
        if (!jumpMap.ContainsKey(Argument))
        {
            throw new UnknownLabelException(Line, Argument);
        }
    }

    public override void ValidateArgument()
    {
        if (ArgumentType != ArgumentType.Label)
        {
            throw new ArgumentIsNotValidException(Line);
        }
    }
}