using System;
using System.Diagnostics;
using ProjectRAM.Core.Models;
using System.Numerics;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MathCommands;

[CommandName("div")]
internal class DivCommand : MathCommandBase
{
    public DivCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        try
        {
            base.Execute(interpreter);
            Debug.Assert(Accumulator != null && SecondValue != null);
            var res = (BigInteger.Parse(Accumulator) / BigInteger.Parse(SecondValue)).ToString();
            interpreter.SetMemory(interpreter.AccumulatorAddress, res);
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