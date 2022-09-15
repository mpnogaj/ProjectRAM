using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectRAM.Core.Models;
using ProjectRAM.Core.Commands.Abstractions;

namespace ProjectRAM.Core.Commands.Models
{
	internal class WriteCommand : CommandBase
	{
		public WriteCommand(long line, string? label, string argument) : base(line, label, argument)
		{
		}

		public override void ValidateArgument()
		{
			if (ArgumentType is not ArgumentType.DirectAddress 
			    and not ArgumentType.IndirectAddress 
			    and not ArgumentType.Const)
			{
				throw new ArgumentIsNotValidException(Line);
			}
		}

		public void Execute(Func<string, long, string> getMemory,
			EventHandler<WriteToTapeEventArgs>? readEventHandler)
		{
			if (readEventHandler == null)
			{
				throw new InputTapeEmptyException(Line);
			}

			var value = ArgumentType switch
			{
				ArgumentType.DirectAddress => getMemory(FormattedArgument, Line),
				ArgumentType.IndirectAddress => getMemory(getMemory(FormattedArgument, Line), Line),
				ArgumentType.Const => FormattedArgument,
				_ => throw new ArgumentIsNotValidException(Line)
			};

			var eventArgs = new WriteToTapeEventArgs(value);
			readEventHandler.Invoke(this, eventArgs);
		}
	}
}
