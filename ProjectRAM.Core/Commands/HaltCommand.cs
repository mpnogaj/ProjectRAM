using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands;

[CommandName("halt")]
internal class HaltCommand : CommandBase
{
	public HaltCommand(long line, string? label, string argument) : base(line, label, argument)
	{

	}

	public ulong Execute(out int position)
	{
		position = -1;
		return 1;
	}

	public override void ValidateArgument()
	{
		if (ArgumentType != ArgumentType.Null)
		{
			throw new ArgumentIsNotValidException(Line);
		}
	}
}