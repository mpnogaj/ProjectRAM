using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ProjectRAM.Core.Models
{
	public static class CommandExtensions
	{
		public static bool IsNumber(this string input)
		{
			if (input == string.Empty)
			{
				return false;
			}
			if (input[0] == '-')
			{
				return Regex.IsMatch(input[1..], @"^\d+$");
			}
			return Regex.IsMatch(input, @"^\d+$");
		}
	}

	/// <summary>
	/// Class which represents single command
	/// </summary>
	public class Command
	{
		public string Argument { get; private set; }
		public ArgumentType ArgumentType { get; private set; }
		public string CommandName { get; private set; }
		public CommandType CommandType { get; private set; }
		public string Comment { get; private set; }
		public string Label { get; private set; }
		public long Line { get; private set; }

		#region Constructors

		/// <summary>
		/// Creates empty command
		/// </summary>
		public Command()
		{
			Argument = string.Empty;
			ArgumentType = ArgumentType.Null;
			CommandName = string.Empty;
			CommandType = CommandType.Null;
			Comment = string.Empty;
			Label = string.Empty;
		}

		/// <summary>
		/// Creates command from given arguments, without line
		/// </summary>
		/// <param name="command">Command</param>
		/// <param name="argument">Argument</param>
		/// <param name="label">Label</param>
		/// <param name="comment">Comment</param>
		public Command(string label, string command, string argument, string comment)
		{
			CommandName = command;
			Argument = argument;
			Label = label;
			Comment = comment;
			ArgumentType = GetArgumentType(Argument);
			CommandType = GetCommandType(CommandName);
			Line = 0;
		}

		/// <summary>
		/// Creates command from line.
		/// </summary>
		/// <param name="line">Line</param>
		public Command(string line)
		{
			Label = string.Empty;
			CommandName = string.Empty;
			Argument = string.Empty;
			Comment = string.Empty;
			CommandType = CommandType.Null;
			ArgumentType = ArgumentType.Null;
			string lineWithoutComment = line;
			int commentPos = line.IndexOf('#');
			if (commentPos >= 0)
			{
				lineWithoutComment = line[..commentPos];
				Comment = line[(commentPos + 1)..].Trim();
			}
			string lineWithoutCommentAndLabel = string.Empty;
			int labelPos = lineWithoutComment.IndexOf(':');
			if (labelPos >= 0)
			{
				Label = Regex.Replace(lineWithoutComment[..labelPos].Trim(), @"\s+", string.Empty);
				if (labelPos != lineWithoutComment.Length - 1)
				{
					lineWithoutCommentAndLabel = lineWithoutComment[(labelPos + 1)..];
				}
			}
			else
			{
				lineWithoutCommentAndLabel = lineWithoutComment;
			}

			lineWithoutCommentAndLabel = Regex.Replace(lineWithoutCommentAndLabel, @"\s+", " ");
			var arr = lineWithoutCommentAndLabel.Trim().Split(' ');
			CommandName = arr.Length > 0 ? arr[0] : string.Empty;
			Argument = arr.Length > 1 ? arr[1] : string.Empty;
			CommandType = GetCommandType(CommandName);
			ArgumentType = GetArgumentType(Argument);
		}

		/// <summary>
		/// Creates command from line. Requires line number
		/// </summary>
		/// <param name="line">Line</param>
		/// <param name="lineNumber">Line number</param>
		public Command(long lineNumber, string line) : this(line)
		{
			Line = lineNumber;
		}

		#endregion Constructors

		#region Methods

		public override bool Equals(object? obj)
		{
			if (obj == null || obj is not Command c)
			{
				return false;
			}

			return c.Label == Label
				&& c.CommandType == CommandType
				&& c.Argument == Argument
				&& c.Comment == Comment;
		}

		public string FormattedArg()
		{
			var arg = Argument;
			if (ArgumentType == ArgumentType.Const || ArgumentType == ArgumentType.IndirectAddress)
			{
				//return arg.Substring(1);
				return arg[1..];
			}
			return arg;
		}

		[ExcludeFromCodeCoverage]
		public override int GetHashCode()
		{
			return HashCode.Combine(Label, CommandType, Argument, Comment);
		}

		/// <summary>
		/// ToString() method override
		/// </summary>
		/// <returns>{label}: {command} {argument} #{comment}</returns>
		public override string ToString()
		{
			string output = "";
			if (!string.IsNullOrWhiteSpace(Label))
			{
				output = $"{Label}: ";
			}
			if (!string.IsNullOrWhiteSpace(CommandName))
			{
				output += $"{CommandName} ";
			}
			if (!string.IsNullOrWhiteSpace(Argument))
			{
				output += $"{Argument} ";
			}
			if (!string.IsNullOrWhiteSpace(Comment))
			{
				output += $"#{Comment}";
			}
			return output.TrimEnd();
		}

		/// <summary>
		/// Get type of argument
		/// </summary>
		/// <param name="argument">Argument text</param>
		/// <returns></returns>
		private static ArgumentType GetArgumentType(string argument)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				return ArgumentType.Null;
			}

			if (argument.IsNumber())
			{
				return ArgumentType.DirectAddress;
			}

			if (argument.StartsWith('^'))
			{
				return ArgumentType.IndirectAddress;
			}
			return argument.StartsWith('=') ? ArgumentType.Const : ArgumentType.Label;
		}

		/// <summary>
		/// Get type of command
		/// </summary>
		/// <param name="command">Command text</param>
		/// <returns>Command type</returns>
		private static CommandType GetCommandType(string command)
		{
			return command.ToUpper() switch
			{
				"ADD" => CommandType.Add,
				"SUB" => CommandType.Sub,
				"MULT" => CommandType.Mult,
				"DIV" => CommandType.Div,
				"LOAD" => CommandType.Load,
				"STORE" => CommandType.Store,
				"READ" => CommandType.Read,
				"WRITE" => CommandType.Write,
				"JUMP" => CommandType.Jump,
				"JGTZ" => CommandType.Jgtz,
				"JZERO" => CommandType.Jzero,
				"HALT" => CommandType.Halt,
				"" => CommandType.Null,
				_ => CommandType.Unknown
			};
		}

		#endregion Methods
	}
}