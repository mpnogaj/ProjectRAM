using System.Text.RegularExpressions;

namespace ProjectRAM.Core;

public static class Parser
{
	public static (string, string, string, string) ParseCodeLine(string line)
	{
		var comment = string.Empty;
		int commentIndex = line.IndexOf('#');
		if (commentIndex >= 0)
		{
			comment = line[(commentIndex + 1)..];
			line = line[..commentIndex];
		}

		line = Regex.Replace(line, @"\s+", " ").Trim();
		
		var label = string.Empty;
		int argumentEndIndex = line.IndexOf(':');
		if (argumentEndIndex >= 0)
		{
			label = line[..argumentEndIndex];
			line = argumentEndIndex + 1 < line.Length ? line[(argumentEndIndex + 1)..] : string.Empty;
		}
		line = line.TrimStart();

		string[] words = line.Split(' ');
		string command = words.Length > 0 ? words[0] : string.Empty;
		string argument = words.Length > 1 ? words[1] : string.Empty;

		return (label, command, argument, comment);
	}
}