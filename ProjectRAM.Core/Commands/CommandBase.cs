using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands;

public abstract class CommandBase
{
    public long Line { get; }
    public string? Label { get; }
    public string Argument { get; }
    public ArgumentType ArgumentType { get; }
    public string FormattedArgument { get; }

    protected CommandBase(long line, string? label, string argument)
    {
        Line = line;
        Label = label;
        Argument = argument;
        ArgumentType = GetArgumentType(argument);
        FormattedArgument = GetFormattedArgument();
    }

    public abstract void ValidateArgument();

    protected ulong LCostHelper(Func<string, long, string> getMemory)
    {
        switch (ArgumentType)
        {
            case ArgumentType.Const:
                return FormattedArgument.LCost();
            case ArgumentType.DirectAddress:
                return FormattedArgument.LCost() + getMemory(FormattedArgument, Line).LCost();
            case ArgumentType.IndirectAddress:
                var res = getMemory(FormattedArgument, Line);
                return FormattedArgument.LCost() + res.LCost() + getMemory(res, Line).LCost();
            default:
                throw new ArgumentIsNotValidException(Line);
        }
    }

    public void ValidateLabel()
    {
        if (Label != null && !Label.IsValidLabel())
        {
            throw new LabelIsNotValidException(Line);
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

    private ArgumentType GetArgumentType(string argument)
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