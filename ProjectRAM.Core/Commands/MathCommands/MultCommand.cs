using System;
using System.Diagnostics;
using System.Numerics;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MathCommands;

[CommandName("mult")]
internal class MultCommand : MathCommandBase
{
    public MultCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override void Execute(IInterpreter interpreter)
    {
        try
        {
            base.Execute(interpreter);
            Debug.Assert(Accumulator != null && SecondValue != null);
            var res = (BigInteger.Parse(Accumulator) * BigInteger.Parse(SecondValue)).ToString();
            interpreter.SetMemory(interpreter.AccumulatorAddress, res);
        }
        catch (FormatException)
        {
            throw new ValueIsNaN(Line);
        }
    }
}