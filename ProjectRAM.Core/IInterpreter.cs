using System.Numerics;

namespace ProjectRAM.Core;

public interface IInterpreter
{
	public IMemory Memory { get; }
	public void MakeJump(string label);
	public void MakeStackJump(string label);
	public void PopStack();
	public void StopProgram();
	public string ReadFromTape();
	public void WriteToTape(string value);
	public void IncreaseExecutionCounter();
	public void UpdateLogarithmicTimeComplexity(ulong value);
	public void UpdateUniformTimeComplexity(ulong value);
}