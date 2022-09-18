using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.MathCommands;

public abstract class MathCommandBase : CommandBase
{
    private protected string _accumulator = Constants.UninitializedValue,
        _secondValue = Constants.UninitializedValue;

    protected MathCommandBase(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public virtual ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
    {
        _accumulator = getMemory(Constants.AccumulatorAddress, Line);
        _secondValue = ArgumentType switch
        {
            ArgumentType.DirectAddress => getMemory(FormattedArgument, Line),
            ArgumentType.IndirectAddress => getMemory(getMemory(FormattedArgument, Line), Line),
            ArgumentType.Const => FormattedArgument,
            _ => throw new ArgumentIsNotValidException(Line)
        };
        return Complexity(getMemory);
    }

    protected ulong Complexity(Func<string, long, string> getMemory)
        => getMemory(Constants.AccumulatorAddress, Line).LCost() + LCostHelper(getMemory);

    public override void ValidateArgument()
    {
        if (ArgumentType != ArgumentType.Const &&
            ArgumentType != ArgumentType.DirectAddress &&
            ArgumentType != ArgumentType.IndirectAddress)
        {
            throw new ArgumentIsNotValidException(Line);
        }
    }
}