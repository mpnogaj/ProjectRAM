using ProjectRAM.Core.Models;
using System.Collections.Generic;

namespace ProjectRAM.Core
{
	public static class Validator
	{
		public static List<RamInterpreterException> ValidateProgram(List<Command> commands)
		{
			List<RamInterpreterException> exceptions = new();
			List<string> requiredLabels = new();
			for (var i = 0; i < commands.Count; i++)
			{
				if (commands[i].ArgumentType == ArgumentType.Label)
				{
					requiredLabels.Add(commands[i].Argument);
				}
				try
				{
					ValidateCommand(commands[i]);
				}
				catch (RamInterpreterException ex)
				{
					exceptions.Add(ex);
				}
			}
			for (var i = 0; i < requiredLabels.Count; i++)
			{
				if (FindLabel(commands, requiredLabels[i]))
				{
					requiredLabels.RemoveAt(i);
					i--;
				}
			}
			if (requiredLabels.Count > 0)
			{
				for (var i = 0; i < commands.Count; i++)
				{
					if (commands[i].ArgumentType != ArgumentType.Label)
					{
						continue;
					}

					if (requiredLabels.Contains(commands[i].Argument))
					{
						exceptions.Add(new UnknownLabelException(commands[i].Line, commands[i].Argument));
					}
				}
			}
			return exceptions;
		}

		private static bool FindLabel(List<Command> c, string lbl)
		{
			foreach (var z in c)
			{
				if (z.Label == lbl)
				{
					return true;
				}
			}
			return false;
		}

		private static void ValidateCommand(Command c)
		{
			if (c.CommandType == CommandType.Null && c.ArgumentType == ArgumentType.Null && c.Label == string.Empty)
			{
				return;
			}

			if (c.CommandType == CommandType.Unknown)
			{
				throw new UnknownCommandTypeException(c.Line);
			}

			if (c.ArgumentType == ArgumentType.Null && c.CommandType != CommandType.Halt ||
				c.ArgumentType != ArgumentType.Null && c.CommandType == CommandType.Null)
			{
				throw new LineIsInvalidException(c.Line);
			}

			var t = c.CommandType;
			if (t == CommandType.Halt)
			{
				if (c.ArgumentType != ArgumentType.Null)
				{
					throw new ArgumentIsNotValidException(c.Line);
				}
			}
			else if (t == CommandType.Jump || t == CommandType.Jzero || t == CommandType.Jgtz)
			{
				if (c.ArgumentType != ArgumentType.Label)
				{
					throw new ArgumentIsNotValidException(c.Line);
				}
			}
			else if (t == CommandType.Read || t == CommandType.Store)
			{
				if (c.ArgumentType != ArgumentType.DirectAddress && c.ArgumentType != ArgumentType.IndirectAddress)
				{
					throw new ArgumentIsNotValidException(c.Line);
				}
			}
			else
			{
				if (c.ArgumentType == ArgumentType.Label || c.ArgumentType == ArgumentType.Null)
				{
					throw new ArgumentIsNotValidException(c.Line);
				}
			}
		}
	}
}