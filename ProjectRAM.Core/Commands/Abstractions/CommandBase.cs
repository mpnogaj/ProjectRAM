using System;
using System.Collections.Generic;
using System.Linq;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Commands.Abstractions
{
	public abstract class CommandBase
	{
		public long Line { get; }
		public string? Label { get; }
		public string Argument { get; }
		public ArgumentType ArgumentType { get; }
		public string FormattedArgument { get; }

		protected CommandBase(long line, string? label, string argument)
		{
			Line = line;
			Label = label;
			Argument = argument;
			ArgumentType = argument.GetArgumentType();
			FormattedArgument = GetFormattedArgument();
		}

		public abstract void ValidateArgument();

		public void ValidateLabel()
		{
			if (Label != null && !Label.All(char.IsLetterOrDigit))
			{
				throw new LabelIsNotValidException(Line);
			}
		}

		private string GetFormattedArgument()
		{
			return ArgumentType switch
			{
				ArgumentType.Null => string.Empty,
				ArgumentType.Label or ArgumentType.DirectAddress => Argument,
				ArgumentType.IndirectAddress or ArgumentType.Const => Argument[1..],
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}
