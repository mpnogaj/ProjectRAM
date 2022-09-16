using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Commands.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProjectRAM.Core.Commands;

internal static class CommandFactory
{
	public static List<CommandBase> CreateCommandList(string[] lines)
	{
		return lines.Select((line, index) => CreateCommand(index + 1, line))
			.Where(command => command != null)
			.ToList()!;
	}

	public static CommandBase? CreateCommand(long lineNum, string line)
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
				"add" => new AddCommand(lineNum, label, argument),
				"sub" => new SubCommand(lineNum, label, argument),
				"mult" => new MultCommand(lineNum, label, argument),
				"div" => new DivCommand(lineNum, label, argument),
				"load" => new LoadCommand(lineNum, label, argument),
				"store" => new StoreCommand(lineNum, label, argument),
				"read" => new ReadCommand(lineNum, label, argument),
				"write" => new WriteCommand(lineNum, label, argument),
				_ => throw new UnknownCommandTypeException(lineNum)
			};
		}
		catch (IndexOutOfRangeException)
		{
			throw new LineIsInvalidException(lineNum);
		}
	}
}