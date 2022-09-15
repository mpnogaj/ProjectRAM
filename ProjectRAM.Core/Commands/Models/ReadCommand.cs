using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Models;
using System;

namespace ProjectRAM.Core.Commands.Models
{
	internal class ReadCommand : CommandBase
	{
		public ReadCommand(long line, string? label, string argument) : base(line, label, argument)
		{
		}

		public override void ValidateArgument()
		{
			if (ArgumentType != ArgumentType.DirectAddress && ArgumentType != ArgumentType.IndirectAddress)
			{
				throw new ArgumentIsNotValidException(Line);
			}
		}

		public void Execute(Func<string, long, string> getMemory, Action<string, string> setMemory,
			EventHandler<ReadFromTapeEventArgs>? readEventHandler)
		{
			var eventArgs = new ReadFromTapeEventArgs();

			if (readEventHandler == null)
			{
				throw new InputTapeEmptyException(Line);
			}
			readEventHandler.Invoke(this, eventArgs);

			var target = ArgumentType switch
			{
				ArgumentType.DirectAddress => FormattedArgument,
				ArgumentType.IndirectAddress => getMemory(FormattedArgument, Line),
				_ => throw new ArgumentIsNotValidException(Line)
			};

			setMemory(target, eventArgs.Input ?? throw new InputTapeEmptyException(Line));
		}
	}
}
