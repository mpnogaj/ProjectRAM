using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectRAM.Core.Commands.JumpCommands;

public abstract class JumpCommandBase : CommandBase
{
    protected JumpCommandBase(long line, string? label, string argument) : base(line, label, argument)
    {
    }
    
    
    protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
    {
        ArgumentType.Label
    };
}