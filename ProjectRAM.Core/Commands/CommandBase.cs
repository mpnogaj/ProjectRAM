using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ProjectRAM.Core.Commands;

public abstract class CommandBase
{
    public long Line { get; }
    public string? Label { get; }
    public string Argument { get; }
    public ArgumentType ArgumentType { get; }
    public string FormattedArgument { get; }
    
    
    protected abstract HashSet<ArgumentType> AllowedArgumentTypes { get; }

    protected internal CommandBase(long line, string? label, string argument)
    {
        Line = line;
        Label = label;
        Argument = argument;
        ArgumentType = GetArgumentType(argument);
        FormattedArgument = GetFormattedArgument();
    }
    
    public abstract void Execute(IInterpreter interpreter);

    protected void UpdateComplexity(IInterpreter interpreter)
    {
        interpreter.UpdateLogarithmicTimeComplexity(CalculateLogarithmicTimeComplexity(interpreter));
        interpreter.UpdateUniformTimeComplexity(1);
    }
    
    protected abstract ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter);
    
    public void ValidateLabel()
    {
        if (Label != null && !Label.IsValidLabel())
        {
            throw new LabelIsNotValidException(Line);
        }
    }

    public virtual void ValidateArgument()
    {
        if (!AllowedArgumentTypes.Contains(ArgumentType))
        {
            throw new ArgumentIsNotValidException(Line);
        }
    }

    private string GetFormattedArgument()
    {
        return ArgumentType switch
        {
            ArgumentType.Null => string.Empty,
            ArgumentType.Label or ArgumentType.DirectAddress => Argument,
            ArgumentType.IndirectAddress or ArgumentType.Const => Argument[1..],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ArgumentType GetArgumentType(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            return ArgumentType.Null;
        }
        if (argument.StartsWith('=') && argument[1..].IsNumber())
        {
            return ArgumentType.Const;
        }
        if (argument.IsNumber())
        {
            return ArgumentType.DirectAddress;
        }
        if (argument.StartsWith('^') && argument[1..].IsNumber())
        {
            return ArgumentType.IndirectAddress;
        }
        if (argument.IsValidLabel())
        {
            return ArgumentType.Label;
        }
        return ArgumentType.Invalid;
    }
}