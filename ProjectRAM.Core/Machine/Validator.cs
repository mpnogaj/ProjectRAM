using System.Collections.Generic;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Commands.JumpCommands;

namespace ProjectRAM.Core.Machine;

public static class Validator
{
	public static List<RamInterpreterException> ValidateProgram(string[] program)
	{
		var exceptions = new List<RamInterpreterException>();
		var validJumpCommands = new List<JumpCommandBase>();
		var usedLabels = new HashSet<string>();
		for (var i = 0; i < program.Length; i++)
		{
			try
			{
				var command = CommandFactory.CreateCommand(i + 1, program[i], false);
				if (command == null)
				{
					continue;
				}
				command.ValidateArgument();
				if (command.Label != null)
				{
					command.ValidateLabel();
					if (usedLabels.Contains(command.Label))
					{
						throw new LabelAlreadyExists(command.Line);
					}

					usedLabels.Add(command.Label);
				}

				if (command is JumpCommandBase jumpCommandBase)
				{
					validJumpCommands.Add(jumpCommandBase);
				}
			}
			catch (RamInterpreterException ex)
			{
				exceptions.Add(ex);
			}
		}

		foreach (var jumpCommand in validJumpCommands)
		{
			try
			{
				if (!usedLabels.Contains(jumpCommand.FormattedArgument))
				{
					throw new UnknownLabelException(jumpCommand.Line, jumpCommand.FormattedArgument);
				}
			}
			catch (UnknownLabelException ex)
			{
				exceptions.Add(ex);
			}
		}

		return exceptions;
	}
}