using ProjectRAM.Editor.Models;
using System.Collections.Generic;

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

		public static string ProgramLinesToString(List<ProgramLine> input)
		{
			string output = "";
			foreach (var programLine in input)
			{
				output += programLine.ToString() + '\n';
			}
			//Skip last '\n'
			return output.Substring(0, output.Length - 1);
		}
	}
}
