using ProjectRAM.Editor.Models;
using System.Collections.Generic;
using System.Linq;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Editor.Converters
{
	public static class ProgramLineToCommandConverter
	{
		public static List<Command> ProgramLinesToCommands(IEnumerable<ProgramLine> input)
		{
			return input.Select(line => new Command(
				command: line.Command,
				argument: line.Argument,
				label: line.Label,
				comment: line.Comment)
			).ToList();
		}

		public static List<ProgramLine> CommandsToProgramLines(IEnumerable<Command> input)
		{
			var i = 1;
			return input.Select(line => new ProgramLine
			{
				Command = line.CommandName,
				Argument = line.Argument,
				Comment = line.Comment,
				Label = line.Label,
				Line = i++
			}).ToList();
		}
	}
}
