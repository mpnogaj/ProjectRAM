using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRAM.Editor.Models
{
	public class MemoryRow
	{
		public string Address { get; set; }
		public string Value { get; set; }

		public MemoryRow()
		{
			Address = string.Empty;
			Value = string.Empty;
		}

		public override string ToString()
		{
			return $"{Address}: {Value}";
		}
	}
}
