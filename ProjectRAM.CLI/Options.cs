using CommandLine;
using CommandLine.Text;

namespace ProjectRAM.CLI;

internal class Options
{
	[Value(0, HelpText = "Path to RAMCode file.", Required = true)]
	public string CodePath { get; set; } = string.Empty;

	[Value(1, HelpText = "Path to input tape file.", Required = false)]
	public string? InputTapePath { get; set; }

	[Option('t', "timeout",
		HelpText = "Timeout in seconds after which program stops. Everything <= 0 will set timeout to infinity.",
		Required = false)]
	public int Timeout { get; set; }

	[Option('m', "memory", HelpText = "Show memory after successful execution.")]
	public bool ShowMemoryReport { get; set; }
	
	[Option('c', "complexity", HelpText="Show complexity report after successful execution.")]
	public bool ShowComplexityReport { get; set; }
	
	[Option("use-crlf", HelpText = "Use CRLF line endings instead of LF.")]
	public bool UseCRLF { get; set; }
}