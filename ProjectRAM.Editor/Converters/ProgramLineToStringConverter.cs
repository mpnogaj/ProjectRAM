using System;
using ProjectRAM.Editor.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectRAM.Editor.Converters
{
	public static class ProgramLineToStringConverter
	{
		public static List<ProgramLine> StringToProgramLines(string input)
		{
			var i = 1;
			return input.Split(Environment.NewLine).Select(x => new ProgramLine(x)
			{
				Line = i++
			}).ToList();
		}

		public static string ProgramLinesToString(List<ProgramLine> input) =>
			input.Aggregate("", (current, programLine) => current + (programLine + Environment.NewLine)).TrimEnd();
	}
}
