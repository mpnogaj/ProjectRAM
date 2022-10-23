using System;
using ProjectRAM.Core.Models;

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

public sealed class ProgramFinishedEventArgs : EventArgs
{
	public InterpreterSnapshot InterpreterSnapshot { get; }

	public ProgramFinishedEventArgs(InterpreterSnapshot interpreterSnapshot)
	{
		InterpreterSnapshot = interpreterSnapshot;
	}
}