namespace ProjectRAM.Core.Generators;

internal class CommandToAdd
{
	public readonly string Name;
	public readonly string FullType;

	public CommandToAdd(string name, string fullType)
	{
		Name = name;
		FullType = fullType;
	}
}