using System;
using ProjectRAM.Core.Models;
using System.Numerics;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MathCommands;

[CommandName("div")]
public class DivCommand : MathCommandBase
{
    public DivCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
    {
        try
        {
            var complexity = base.Execute(getMemory, setMemory);
            var res = (BigInteger.Parse(_accumulator) / BigInteger.Parse(_secondValue)).ToString();
            setMemory(Interpreter.AccumulatorAddress, res);
            return complexity;
        }
        catch (DivideByZeroException)
        {
            throw new DivByZero(Line);
        }
        catch (FormatException)
        {
            throw new ValueIsNaN(Line);
        }
    }

    public override void ValidateArgument()
    {
        base.ValidateArgument();
        if (ArgumentType == ArgumentType.Const && FormattedArgument.IsZero())
        {
            throw new DivByZero(Line);
        }
    }
}