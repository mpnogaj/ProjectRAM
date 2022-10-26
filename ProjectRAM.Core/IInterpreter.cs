using System.Threading.Tasks;
using System.Threading;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core;

public interface IInterpreter
{
	public IMemory Memory { get; }
	public void MakeJump(string label);
	public void MakeStackJump(string label);
	public void PopStack();
	public Task<InterpreterSnapshot> RunCommandsAtFullSpeed();
	public Task<InterpreterSnapshot> RunCommandsAtFullSpeed(CancellationToken cancellationToken);
	public Task<InterpreterSnapshot> RunTillBreakpoint();
	public Task<InterpreterSnapshot> RunTillBreakpoint(CancellationToken cancellationToken);
	public InterpreterSnapshot StepForward();
	public void StopProgram();
	public string ReadFromTape();
	public void WriteToTape(string value);
	public void IncreaseExecutionCounter();
	public void UpdateLogarithmicTimeComplexity(ulong value);
	public void UpdateUniformTimeComplexity(ulong value);
}