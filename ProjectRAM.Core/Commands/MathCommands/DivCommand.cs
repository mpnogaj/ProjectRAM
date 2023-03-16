using System;
using System.Diagnostics;
using ProjectRAM.Core.Models;
using System.Numerics;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Machine.Abstraction;

namespace ProjectRAM.Core.Commands.MathCommands;

[CommandName("div")]
internal class DivCommand : MathCommandBase
{
    public DivCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        try
        {
            base.Execute(interpreter);
            Debug.Assert(Accumulator != null && SecondValue != null);
            var res = Accumulator.Value / SecondValue.Value;
            interpreter.Memory.SetAccumulator(res);
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