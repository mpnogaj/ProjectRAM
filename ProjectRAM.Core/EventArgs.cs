using System;

namespace ProjectRAM.Core
{
	public class ReadFromTapeEventArgs : EventArgs
	{
		public string? Input { get; set; }
	}

	public class WriteFromTapeEventArgs : EventArgs
	{
		public string Output { get; }

		public WriteFromTapeEventArgs(string output)
		{
			Output = output;
		}
	}
}
