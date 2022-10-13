using ProjectRAM.Editor.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectRAM.Editor.Converters
{
	public static class ProgramLineToStringConverter
	{
		public static List<ProgramLine> StringToProgramLines(string input)
		{
			List<ProgramLine> output = new();
			var inputArr = input.Split('\n');
			int i = 1;
			foreach (var line in inputArr)
			{
				output.Add(new ProgramLine(line)
				{
					Line = i++
				});
			}
			return output;
		}

		public static string[] ProgramLinesToString(List<ProgramLine> input)
		{
			return input.Select(x => x.ToString()).ToArray();
		}
	}
}
