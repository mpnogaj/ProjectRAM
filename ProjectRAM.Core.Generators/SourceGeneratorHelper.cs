﻿using System.Text;

namespace ProjectRAM.Core.Generators;

internal static class SourceGeneratorHelper
{
	public const string Header = @"// <auto-generated />
#nullable enable
";

	public const string AttributeHint = $"{AttributeName}.g.cs";
	public const string HelperClassHint = $"{HelperClassName}.g.cs";

	public const string AttributeName = "CommandNameAttribute";
	public const string AttributeNamespace = "ProjectRAM.Core.Commands";
	public const string AttributeFullName = $"{AttributeNamespace}.{AttributeName}";

	public const string HelperClassName = "CommandHelper";
	public const string HelperClassNamespace = "ProjectRAM.Core.Commands";



	public const string ClassNameAttribute = $@"{Header}
namespace {AttributeNamespace};

[System.AttributeUsage(System.AttributeTargets.Class)]
internal class {AttributeName} : System.Attribute
{{
	public CommandNameAttribute(string name) : base()
	{{
		Name = name;
	}}

	public string Name {{ get; private set; }}
}}";

	public static string GenerateCommandHelperClass(List<CommandToAdd> commandToAdds)
	{
		var stringBuilder = new StringBuilder($@"{Header}
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectRAM;
namespace {HelperClassNamespace};

internal static class {HelperClassName}
{{
	private static readonly Dictionary<string, Type> _commands = new()
	{{
");
		foreach (var commandToAdd in commandToAdds)
		{
			stringBuilder.AppendLine($"\t\t{{\"{commandToAdd.Name}\", typeof({commandToAdd.FullType})}},");
		}

		stringBuilder.Append(@"
	};
	private static HashSet<string> _commandNames = _commands.Keys.ToHashSet();

	internal static Dictionary<string, Type> Commands => _commands;
	internal static HashSet<string> CommandNames => _commandNames;

	public static CommandBase CreateCommandInstance(string command, long lineNum, string? label, string argument, bool breakpoint)
	{
		return command switch
		{
");
		foreach (var commandToAdd in commandToAdds)
		{
			stringBuilder.AppendLine($"\t\t\t\"{commandToAdd.Name}\" => new {commandToAdd.FullType}(lineNum, label, argument, breakpoint),");
		}
		stringBuilder.AppendLine("\t\t\t_ => throw new UnknownCommandTypeException(lineNum)");
		stringBuilder.Append(@"
		};
	}
}
");
		return stringBuilder.ToString();
	}
}