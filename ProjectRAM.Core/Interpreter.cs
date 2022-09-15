using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Commands.Models;
using static ProjectRAM.Core.Utilities;

namespace ProjectRAM.Core
{
	public class Interpreter
	{
		private readonly Dictionary<string, int> _jumpMap;
		private readonly List<CommandBase> _program;
		private int _currentPosition = 0;
		public const string UninitializedValue = "?";
		public const string AccumulatorAddress = "0";


		public event EventHandler<WriteToTapeEventArgs>? WriteToOutputTape;
		public event EventHandler<ReadFromTapeEventArgs>? ReadFromInputTape;
		public event EventHandler? ProgramFinished;

		public Interpreter(string[] program) : this(CommandFactory.CreateCommandList(program))
		{

		}

		public Interpreter(List<CommandBase> program)
		{
			_program = program;

			//Validator.ValidateProgram()

			_jumpMap = MapLabels();
			Memory = new Dictionary<string, string>()
			{
				{AccumulatorAddress, UninitializedValue}
			};
			MaxMemory = new Dictionary<string, string>()
			{
				{AccumulatorAddress, UninitializedValue}
			};
			Complexity = new Complexity(this);
		}

		public Complexity Complexity { get; }
		internal Dictionary<string, string> MaxMemory { get; }
		internal Dictionary<string, string> Memory { get; }

		public Dictionary<string, string> RunCommands() 
			=> RunCommands(CancellationToken.None);

		public Dictionary<string, string> RunCommands(CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			// Halt command sets i to -1 when executed
			_currentPosition = 0;
			while(_currentPosition < _program.Count && _currentPosition != -1)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					ProgramFinished?.Invoke(this, EventArgs.Empty);
					cancellationToken.ThrowIfCancellationRequested();
				}

				RunCommand();

				ProgramFinished?.Invoke(this, EventArgs.Empty);
				return Memory;
			}
			ProgramFinished?.Invoke(this, EventArgs.Empty);
			return Memory;
		}
		
		private Dictionary<string, int> MapLabels()
		{
			Dictionary<string, int> labels = new();
			for (var i = 0; i < _program.Count; i++)
			{
				var label = _program[i].Label;
				if (label != null)
				{
					labels.Add(label, i);
				}
			}
			return labels;
		}

		private void RunCommand()
		{
			var command = _program[_currentPosition];
			var line = command.Line;
			switch (command)
			{
				case JumpCommandBase jumpCommand:
					if (jumpCommand.CanJump(jumpCommand is JumpCommand ? string.Empty : GetMemory(AccumulatorAddress, line)))
					{
						MakeJump(jumpCommand.FormattedArgument, out _currentPosition);
						return;
					}
					break;
				case MathCommandBase mathCommand:
					mathCommand.Calculate(GetMemory, SetMemory);
					break;

				case MemoryManagementCommand memoryManagementCommand:
					memoryManagementCommand.Execute(GetMemory, SetMemory);
					break;

				case ReadCommand readCommand:
					readCommand.Execute(GetMemory, SetMemory, ReadFromInputTape);
					break;

				case WriteCommand writeCommand:
					writeCommand.Execute(GetMemory, WriteToOutputTape);
					break;

				case HaltCommand:
					_currentPosition = -1;
					return;
			}

			_currentPosition++;
		}

		private void MakeJump(string label, out int i)
		{
			i = _jumpMap[label];
		}

		private string GetMemory(string address, long line)
		{
			if (!Memory.ContainsKey(address))
			{
				throw address == AccumulatorAddress
					? new AccumulatorEmptyException(line)
					: new UninitializedCellException(line);
			}

			var value = Memory[address];
			if (value == UninitializedValue)
			{
				throw address == AccumulatorAddress
					? new AccumulatorEmptyException(line)
					: new UninitializedCellException(line);
			}

			return value;
		}

		private void SetMemory(string address, string value)
		{
			if (!Memory.ContainsKey(address))
			{
				Memory[address] = value;
				MaxMemory[address] = value;
			}
			else
			{
				var oldValue = Memory[address];
				Memory[address] = value;
				MaxMemory[address] = oldValue == "?" ? value : Max(MaxMemory[address], value);
			}
		}
	}
}