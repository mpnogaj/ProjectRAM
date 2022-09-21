using System;
using System.Collections.Generic;
using System.Linq;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Commands.MathCommands;

namespace ProjectRAM.Core;

internal static class Constants
{
	private static readonly string CommandsNamespace = typeof(CommandBase).Namespace!;

	private static Dictionary<string, Type> _commands = new()
	{
		{"add", typeof(AddCommand)},
	};
	private static HashSet<string> _commandNames = _commands.Keys.ToHashSet();

	internal static Dictionary<string, Type> Commands => _commands;
	internal static HashSet<string> CommandNames => _commandNames;
	internal const string UninitializedValue = "?";
	internal const string AccumulatorAddress = "0";
}