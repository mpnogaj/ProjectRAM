using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Commands.Models;

namespace ProjectRAM.Core.Commands
{
	internal static class CommandFactory
	{
		public static List<ICommand> CreateCommandList(string[] lines)
		{
			return lines.Select((line, index) => CreateCommand(index + 1, line))
				.Where(command => command != null)
				.ToList()!;
		}

		public static ICommand? CreateCommand(long lineNum, string line)
		{
			try
			{
				line = Regex.Replace(
					Regex.Replace(line, @"#.*", ""),
					@"\s+", "").Trim();
				var parts = line.Split(' ');
				if (parts.Length == 0)
				{
					return null;
				}
				string? label = null;
				var argument = string.Empty;
				var cmdPos = 0;
				if (parts[0].EndsWith(':'))
				{
					cmdPos++;
					label = parts[0][..^1];
				}
				if (cmdPos + 1 < parts.Length)
				{
					argument = parts[cmdPos + 1];
				}

				var command = parts[cmdPos].ToLower();
				
				return command switch
				{
					"jump" => new JumpCommand(lineNum, label, argument),
					"jzero" => new JZeroCommand(lineNum, label, argument),
					"jgtz" => new JGTZCommand(lineNum, label, argument),
					_ => throw new UnknownCommandTypeException(lineNum)
				};
			}
			catch (IndexOutOfRangeException)
			{
				throw new LineIsInvalidException(lineNum);
			}
		}
	}
}
