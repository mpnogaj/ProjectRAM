using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProjectRAM.Core.Commands;

internal static class CommandFactory
{
	public static List<CommandBase> CreateCommandList(IEnumerable<string> lines)
	{
		return lines
			.Select((line, index) => CreateCommand(index + 1, line))
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

			//replace with source code generator asap for better performance
			//https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview
			Constants.Commands.TryGetValue(command, out var commandType);
			if (commandType == null)
			{
				throw new UnknownCommandTypeException(lineNum);
			}

			return (CommandBase)Activator.CreateInstance(commandType, lineNum, label, argument)!;
		}
		catch (IndexOutOfRangeException)
		{
			throw new LineIsInvalidException(lineNum);
		}
	}
}