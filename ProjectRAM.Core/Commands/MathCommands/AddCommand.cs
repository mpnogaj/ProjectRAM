using System;
using System.Diagnostics;
using System.Numerics;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Machine.Abstraction;

namespace ProjectRAM.Core.Commands.MathCommands;

[CommandName("add")]
internal class AddCommand : MathCommandBase
{
    public AddCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        try
        {
            base.Execute(interpreter);
            Debug.Assert(Accumulator != null && SecondValue != null);
            var res = Accumulator.Value + SecondValue.Value;
            interpreter.Memory.SetAccumulator(res);
        }
        catch (FormatException)
        {
            throw new ValueIsNaN(Line);
        }
    }
}