using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectRAM.Core;

public sealed class Interpreter : IInterpreter
{
	private readonly Dictionary<string, int> _jumpMap;
	private readonly List<CommandBase> _program;
	private readonly Stack<int> _callStack;
	private int _currentPosition;
	private ulong _uniformTimeCost = 0, _logTimeCost = 0;

	
	public event EventHandler<WriteToTapeEventArgs>? WriteToOutputTape;
	public event EventHandler<ReadFromTapeEventArgs>? ReadFromInputTape;
	public event EventHandler<ProgramFinishedEventArgs>? ProgramFinished;
	public IMemory Memory { get; init; }
	
	public Interpreter(List<CommandBase> program)
	{
		_program = program;
		_jumpMap = MapLabels();
		_callStack = new Stack<int>();
		Memory = new Memory();
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

	public Task RunCommandsAtFullSpeed()
		=> RunCommandsAtFullSpeed(CancellationToken.None);

	public Task RunCommandsAtFullSpeed(CancellationToken cancellationToken)
	{
		_currentPosition = 0;
		while (_currentPosition < _program.Count && _currentPosition >= 0)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				ProgramFinished?.Invoke(this, new ProgramFinishedEventArgs(CreateSnapshot()));
				cancellationToken.ThrowIfCancellationRequested();
			}
			
			_program[_currentPosition].Execute(this);
		}
		ProgramFinished?.Invoke(this, new ProgramFinishedEventArgs(CreateSnapshot()));
		return Task.CompletedTask;
	}

	private InterpreterSnapshot CreateSnapshot()
	{
		return new InterpreterSnapshot(Memory.GetHumanReadableData(), new ComplexityReport
		{
			LogTimeCost = _logTimeCost,
			LogSpaceCost = Memory.CalculateLogarithmicComplexity(),
			UniformSpaceCost = Memory.CalculateUniformComplexity(),
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
}