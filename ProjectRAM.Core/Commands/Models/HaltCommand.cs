using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands.Models
{
	internal class HaltCommand : CommandBase
	{
		public HaltCommand(long line, string? label, string argument) : base(line, label, argument)
		{
			
		}

		public override void ValidateArgument()
		{
			if (ArgumentType != ArgumentType.Null)
			{
				throw new ArgumentIsNotValidException(Line);
			}
		}
	}
}
