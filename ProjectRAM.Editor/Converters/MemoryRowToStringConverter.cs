using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectRAM.Editor.Models;

namespace ProjectRAM.Editor.Converters
{
	public static class MemoryRowToStringConverter
	{
		public static string MemoryRowsToString(List<MemoryRow> input)
		{
			string output = string.Empty;
			foreach (var row in input)
			{
				output += row.ToString() + '\n';
			}
			return output;
		}
	}
}
