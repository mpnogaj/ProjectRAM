using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core;

internal static class Constants
{
	private static readonly string CommandsNamespace = typeof(CommandBase).Namespace!;
	private static Dictionary<string, Type> _commands = Assembly.GetCallingAssembly()
		.GetTypes()
		.Where(type => type.IsClass &&
					   !type.IsAbstract &&
					   type.Namespace!.StartsWith(CommandsNamespace) &&
					   type.GetCustomAttribute<CommandNameAttribute>() != null)
		.ToDictionary(type => type.GetCustomAttribute<CommandNameAttribute>()!.Name);
	private static HashSet<string> _commandNames = _commands.Keys.ToHashSet();

	internal static Dictionary<string, Type> Commands => _commands;
	internal static HashSet<string> CommandNames => _commandNames;
	internal const string UninitializedValue = "?";
	internal const string AccumulatorAddress = "0";
}