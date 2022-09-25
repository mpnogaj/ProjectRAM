using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace ProjectRAM.Core;

public class Interpreter : IInterpreter
{
	internal const string UninitializedValue = "?";
	public string AccumulatorAddress => "0";
	string IInterpreter.UninitializedValue => UninitializedValue;

	private readonly Dictionary<string, int> _jumpMap;
	private readonly List<CommandBase> _program;
	private readonly Stack<int> _callStack;
	private int _currentPosition;
	private ulong _uniformTimeCost = 0, _logTimeCost = 0;

	
	public event EventHandler<WriteToTapeEventArgs>? WriteToOutputTape;
	public event EventHandler<ReadFromTapeEventArgs>? ReadFromInputTape;
	public event EventHandler? ProgramFinished;
	private Dictionary<string, string> MaxMemory { get; }
	private Dictionary<string, string> Memory { get; }
	
	public Interpreter(List<CommandBase> program)
	{
		_program = program;
		_jumpMap = MapLabels();
		_callStack = new Stack<int>();
		Memory = new Dictionary<string, string>()
		{
			{ AccumulatorAddress, UninitializedValue}
		};
		MaxMemory = new Dictionary<string, string>()
		{
			{ AccumulatorAddress, UninitializedValue }
		};
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

	public InterpreterResult RunCommands()
		=> RunCommands(CancellationToken.None);

	public InterpreterResult RunCommands(CancellationToken cancellationToken)
	{
		_currentPosition = 0;
		while (_currentPosition < _program.Count && _currentPosition >= 0)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				ProgramFinished?.Invoke(this, EventArgs.Empty);
				cancellationToken.ThrowIfCancellationRequested();
			}


			int p = _currentPosition;
			_program[_currentPosition].Execute(this);
			if (p == _currentPosition)
			{
				int a = 0;
			}
		}

		ProgramFinished?.Invoke(this, EventArgs.Empty);
		var readOnlyMemory = Memory.Select(keyVal => new Cell(keyVal.Key, keyVal.Value))
			.OrderBy(x => x)
			.ToList()
			.AsReadOnly();
		
		return new InterpreterResult(readOnlyMemory, new ComplexityReport
		{
			LogTimeCost = _logTimeCost,
			LogSpaceCost = MaxMemory.Select(x => x.Value.LCost()).Aggregate((cSum, curr) => cSum + curr),
			UniformSpaceCost = (ulong)Memory.Select(x => x.Value != UninitializedValue).LongCount(),
			UniformTimeCost = _uniformTimeCost
		});
	}
	public void MakeJump(string label)
	{
		_currentPosition = _jumpMap[label];
	}

	public void MakeStackJump(string label)
	{
		_callStack.Push(_currentPosition);
		_currentPosition = _jumpMap[label];
	}

	public void PopStack()
	{
		if (_callStack.Count == 0)
		{
			throw new CallStackIsEmptyException(_currentPosition);
		}

		_currentPosition = _callStack.Pop();
	}

	public string GetMemory(string address)
	{
		long currentLine = _program[_currentPosition].Line;
		if (!Memory.ContainsKey(address))
		{
			throw address == AccumulatorAddress
				? new AccumulatorEmptyException(currentLine)
				: new UninitializedCellException(currentLine);
		}

		string value = Memory[address];
		if (value == UninitializedValue)
		{
			throw address == AccumulatorAddress
				? new AccumulatorEmptyException(currentLine)
				: new UninitializedCellException(currentLine);
		}

		return value;
	}

	public void SetMemory(string address, string value)
	{
		if (!Memory.ContainsKey(address))
		{
			Memory[address] = value;
			MaxMemory[address] = value;
		}
		else
		{
			string oldValue = Memory[address];
			Memory[address] = value;
			MaxMemory[address] = oldValue == "?" ? value : Max(MaxMemory[address], value);
		}
	}

	public void StopProgram()
	{
		_callStack.Clear();
		_currentPosition = -1;
	}

	public string ReadFromTape()
	{
		var eventArgs = new ReadFromTapeEventArgs();
		ReadFromInputTape?.Invoke(this, eventArgs);
		if (eventArgs.Input == null) // will be triggered when event is null
		{
			throw new InputTapeEmptyException(_currentPosition);
		}

		return eventArgs.Input;
	}

	public void WriteToTape(string value)
	{
		var eventArgs = new WriteToTapeEventArgs(value);
		WriteToOutputTape?.Invoke(this, eventArgs);
	}

	public void IncreaseExecutionCounter()
	{
		_currentPosition++;
	}

	public void UpdateLogarithmicTimeComplexity(ulong value)
	{
		_logTimeCost += value;
	}

	public void UpdateUniformTimeComplexity(ulong value)
	{
		_uniformTimeCost += value;
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