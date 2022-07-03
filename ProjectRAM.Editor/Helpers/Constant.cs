using System.Collections.Generic;
using Avalonia.Controls;

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

		public static readonly List<FileDialogFilter> RamcodeFilter = new List<FileDialogFilter>
		{
			new()
			{
				Name = "RAMCode file",
				Extensions = new List<string>
				{
					"RAMCode"
				}
			}
		};

		public static readonly List<FileDialogFilter> TextFileFilter = new List<FileDialogFilter>
		{
			new()
			{
				Name = "Text file",
				Extensions = new List<string>
				{
					"txt"
				}
			}
		};

		internal const string DefaultHeader = "NEW";
		internal const string RamExtension = ".RAMCode";
	}
}
