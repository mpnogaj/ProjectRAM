namespace ProjectRAM.Core;

public interface IInterpreter
{
	public string GetMemory(string address, long line);
	public void SetMemory(string address, string value);
	public void MakeJump(string label);
	public void MakeStackJump(string label);
	public void PopStack();
}