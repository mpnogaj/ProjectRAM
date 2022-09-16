using System;

namespace ProjectRAM.Core;

public class ReadFromTapeEventArgs : EventArgs
{
	public string? Input { get; set; }
}

public class WriteToTapeEventArgs : EventArgs
{
	public string Output { get; }

	public WriteToTapeEventArgs(string output)
	{
		Output = output;
	}
}