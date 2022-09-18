using System;
using System.Numerics;

namespace ProjectRAM.Core.Commands.MathCommands;

[CommandName("mult")]
internal class MultCommand : MathCommandBase
{
    public MultCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    public override ulong Execute(Func<string, long, string> getMemory, Action<string, string> setMemory)
    {
        try
        {
            var complexity = base.Execute(getMemory, setMemory);
            var res = (BigInteger.Parse(_accumulator) * BigInteger.Parse(_secondValue)).ToString();
            setMemory(Constants.AccumulatorAddress, res);
            return complexity;
        }
        catch (FormatException)
        {
            throw new ValueIsNaN(Line);
        }
    }
}