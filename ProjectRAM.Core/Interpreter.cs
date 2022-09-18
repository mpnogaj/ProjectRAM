using ProjectRAM.Core.Commands.Abstractions;
using ProjectRAM.Core.Commands.Models;
using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace ProjectRAM.Core;

public class Interpreter
{
	private readonly Dictionary<string, int> _jumpMap;
	private readonly List<CommandBase> _program;
	private int _currentPosition;
	public const string UninitializedValue = "?";
	public const string AccumulatorAddress = "0";


	public event EventHandler<WriteToTapeEventArgs>? WriteToOutputTape;
	public event EventHandler<ReadFromTapeEventArgs>? ReadFromInputTape;
	public event EventHandler? ProgramFinished;

	public Interpreter(List<CommandBase> program)
	{
		_program = program;
		_jumpMap = MapLabels();
		Memory = new Dictionary<string, string>()
		{
			{AccumulatorAddress, UninitializedValue}
		};
		MaxMemory = new Dictionary<string, string>()
		{
			{ AccumulatorAddress, UninitializedValue }
		};
	}

	private Dictionary<string, string> MaxMemory { get; }
	private Dictionary<string, string> Memory { get; }

	public InterpreterResult RunCommands()
		=> RunCommands(CancellationToken.None);

	public InterpreterResult RunCommands(CancellationToken cancellationToken)
	{
		ulong uniformTimeCost = 0, logTimeCost = 0;

		_currentPosition = 0;
		while (_currentPosition < _program.Count && _currentPosition >= 0)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				ProgramFinished?.Invoke(this, EventArgs.Empty);
				cancellationToken.ThrowIfCancellationRequested();
			}

			uniformTimeCost++;
			logTimeCost += RunCommand();
		}

		ProgramFinished?.Invoke(this, EventArgs.Empty);
		var readOnlyMemory = Memory.Select(keyVal => new Cell(keyVal.Key, keyVal.Value))
			.ToList()
			.AsReadOnly();
		return new InterpreterResult(readOnlyMemory, new ComplexityReport
		{
			LogTimeCost = logTimeCost,
			LogSpaceCost = MaxMemory.Select(x => x.Value.LCost()).Aggregate((cSum, curr) => cSum + curr),
			UniformSpaceCost = (ulong)Memory.Select(x => x.Value != UninitializedValue).LongCount(),
			UniformTimeCost = uniformTimeCost
		});
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

	private ulong RunCommand()
	{
		var command = _program[_currentPosition];
		var line = command.Line;
		ulong cost = 0;
		switch (command)
		{
			case JumpCommandBase jumpCommand:
				return jumpCommand.Execute(
					jumpCommand is JumpCommand ? string.Empty : GetMemory(AccumulatorAddress, line),
					(label) =>
					{
						MakeJump(label, out _currentPosition);
					});
			case MathCommandBase mathCommand:
				cost = mathCommand.Execute(GetMemory, SetMemory);
				break;

			case MemoryManagementCommand memoryManagementCommand:
				cost = memoryManagementCommand.Execute(GetMemory, SetMemory);
				break;

			case ReadCommand readCommand:
				cost = readCommand.Execute(GetMemory, SetMemory, ReadFromInputTape);
				break;

			case WriteCommand writeCommand:
				cost = writeCommand.Execute(GetMemory, WriteToOutputTape);
				break;

			case HaltCommand haltCommand:
				_currentPosition = -1;
				return haltCommand.Execute();
		}

		_currentPosition++;
		return cost;
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

	/// <summary>
	/// Compares two big integers.
	/// </summary>
	/// <param name="v1">First value</param>
	/// <param name="v2">Second value</param>
	/// <returns>Bigger integer value</returns>
	private static string Max(string v1, string v2)
	{
		if (string.Empty == v1)
		{
			return v2;
		}
		if (string.Empty == v2)
		{
			return v1;
		}
		return BigInteger.Parse(v1) >= BigInteger.Parse(v2) ? v1 : v2;
	}
}