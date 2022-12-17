using System;

namespace ProjectRAM.Core;

public sealed class ReadFromTapeEventArgs : EventArgs
{
	public string? Input { get; set; }
}

public sealed class WriteToTapeEventArgs : EventArgs
{
	public string Output { get; }

	public WriteToTapeEventArgs(string output)
	{
		Output = output;
	}
}