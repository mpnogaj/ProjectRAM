using System.Collections.Generic;
using Avalonia.Platform.Storage;

namespace ProjectRAM.Editor.Helpers
{
	public static class Constant
	{
		public static readonly List<string> AvailableCommands = new List<string>
		{
			"load",
			"store",
			"read",
			"write",
			"add",
			"sub",
			"mult",
			"div",
			"jump",
			"jzero",
			"jgtz",
			"halt"
		};
		public static IReadOnlyList<FilePickerFileType> RamCodeFilter => new[]
		{
			new FilePickerFileType("RAM Code")
			{
				Patterns = new[] { $"*{RamExtension}" },
				AppleUniformTypeIdentifiers = new[] { "public.source-code" }
			}
		};	
		
		public static IReadOnlyList<FilePickerFileType> TextFileFilter => new[]
		{
			new FilePickerFileType("Text file")
			{
				Patterns = new[] { "*.txt" },
				MimeTypes = new [] {"text/plain"},
				AppleUniformTypeIdentifiers = new[] { "public.plain-text" }
			}
		};

		internal const string DefaultHeader = "NEW";
		internal const string RamExtension = ".RAMCode";
	}
}
