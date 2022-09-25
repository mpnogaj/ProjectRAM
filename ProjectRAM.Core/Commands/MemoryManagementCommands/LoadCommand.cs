using ProjectRAM.Core.Models;
using System.Collections.Generic;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

[CommandName("load")]
internal class LoadCommand : CommandBase
{
    public LoadCommand(long line, string? label, string argument) : base(line, label, argument)
    {
    }
    
    protected override HashSet<ArgumentType> AllowedArgumentTypes => new()
    {
        ArgumentType.Const,
        ArgumentType.DirectAddress,
        ArgumentType.IndirectAddress
    };
    
    public override void Execute(IInterpreter interpreter)
    {
        string value = GetValue(interpreter);
        interpreter.SetMemory(interpreter.AccumulatorAddress, value);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return LCostHelper(interpreter);
    }
}