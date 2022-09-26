using System.Collections.Generic;
using System.Numerics;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

[CommandName("read")]
internal class ReadCommand : NumberArgumentCommandBase
{
    public ReadCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }

    protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
    {
        ArgumentType.DirectAddress,
        ArgumentType.IndirectAddress
    };
    
    public override void Execute(IInterpreter interpreter)
    {
        string input = interpreter.ReadFromTape();
        var numberInput = input.IsNumber() ? BigInteger.Parse(input) : throw new ValueIsNaN(Line);
        var address = GetAddress(interpreter);
        interpreter.Memory.SetMemory(address, numberInput);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return interpreter.Memory.GetMemory(GetAddress(interpreter), Line).LCost() + NumberArgument.LCost() +
               (ArgumentType == ArgumentType.IndirectAddress ? interpreter.Memory.GetMemory(NumberArgument, Line).LCost() : 0);
    }
}