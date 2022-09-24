namespace ProjectRAM.Core.Commands;

public class EmptyCommand : CommandBase
{
	public EmptyCommand(long line, string? label, string argument) : base(line, label, argument)
	{
	}

	public void Execute()
	{
		
	}

	public override void ValidateArgument()
	{
		if (!string.IsNullOrEmpty(Argument))
		{
			throw new EmptyLineDoesNotTakeArgumentException(Line);
		}
	}
}