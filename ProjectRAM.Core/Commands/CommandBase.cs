using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;

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

    internal string GetValue(IInterpreter interpreter)
        => ArgumentType switch
        {
            ArgumentType.Const => FormattedArgument,
            ArgumentType.DirectAddress => interpreter.GetMemory(FormattedArgument),
            ArgumentType.IndirectAddress => interpreter.GetMemory(interpreter.GetMemory(FormattedArgument)),
            _ => throw new InvalidOperationException()
        };

    internal string GetAddress(IInterpreter interpreter) 
        => ArgumentType switch
        {
            ArgumentType.DirectAddress => FormattedArgument,
            ArgumentType.IndirectAddress => interpreter.GetMemory(FormattedArgument),
            _ => throw new InvalidOperationException()
        };

    internal ulong LCostHelper(IInterpreter interpreter)
    {
        switch (ArgumentType)
        {
            case ArgumentType.Const:
                return FormattedArgument.LCost();
            case ArgumentType.DirectAddress:
                return FormattedArgument.LCost() + interpreter.GetMemory(FormattedArgument).LCost();
            case ArgumentType.IndirectAddress:
                var res = interpreter.GetMemory(FormattedArgument);
                return FormattedArgument.LCost() + res.LCost() + interpreter.GetMemory(res).LCost();
            default:
                throw new InvalidOperationException();
        }
    }

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