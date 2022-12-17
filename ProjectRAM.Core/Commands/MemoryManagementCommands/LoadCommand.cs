using ProjectRAM.Core.Models;
using System.Collections.Generic;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Machine.Abstraction;

namespace ProjectRAM.Core.Commands.MemoryManagementCommands;

[CommandName("load")]
internal class LoadCommand : NumberArgumentCommandBase
{
    public LoadCommand(long line, string? label, string argument, bool breakpoint) : base(line, label, argument, breakpoint)
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
        var value = GetValue(interpreter);
        interpreter.Memory.SetAccumulator(value);
        UpdateComplexity(interpreter);
        interpreter.IncreaseExecutionCounter();
    }

    protected override ulong CalculateLogarithmicTimeComplexity(IInterpreter interpreter)
    {
        return LCostHelper(interpreter);
    }
}