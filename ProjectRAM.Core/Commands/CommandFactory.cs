using System.Collections.Generic;
using System.Linq;

namespace ProjectRAM.Core.Commands;

public static class CommandFactory
{
	public static List<CommandBase> CreateCommandList(IEnumerable<string> lines, HashSet<long> breakpoints)
	{
		return lines
			.Select((line, index) => CreateCommand(index + 1, line, breakpoints.Contains(index + 1)))
			.Where(command => command != null)
			.ToList()!;
	}

	internal static CommandBase? CreateCommand(long lineNum, string line, bool breakpoint)
	{
		var parsedLine = Parser.ParseCodeLine(line);
		string command = parsedLine.Item2.ToLower();
		string? label = parsedLine.Item1 != string.Empty ? parsedLine.Item1 : null;
		string argument = parsedLine.Item3;
		if (string.IsNullOrEmpty(label) && string.IsNullOrEmpty(command) && string.IsNullOrEmpty(argument))
		{
			return null;
		}
		return CommandHelper.CreateCommandInstance(command, lineNum, label, argument, breakpoint);
	}
}