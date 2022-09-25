using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

[CommandName("read")]
internal class ReadCommand : CommandBase
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
        string address = GetAddress(interpreter);
        interpreter.SetMemory(address, input);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return interpreter.GetMemory(GetAddress(interpreter)).LCost() + FormattedArgument.LCost() +
               (ArgumentType == ArgumentType.IndirectAddress ? interpreter.GetMemory(FormattedArgument).LCost() : 0);
    }
}