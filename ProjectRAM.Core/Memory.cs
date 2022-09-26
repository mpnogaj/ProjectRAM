using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core;

public class Memory : IMemory
{
	private readonly Dictionary<BigInteger, BigInteger> _memory = new();
	private readonly Dictionary<BigInteger, BigInteger> _maxMemory = new();

	private readonly BigInteger _accumulatorAddress = BigInteger.Zero;
	public ulong MemorySize { get; private set; }
	
	public BigInteger GetMemory(BigInteger address, long line)
	{
		if (_memory.ContainsKey(address))
		{
			return _memory[address];
		}
		throw address == _accumulatorAddress
			? new UninitializedCellException(line)
			: new AccumulatorEmptyException(line);
	}

	public BigInteger GetIndirectMemory(BigInteger address, long line)
		=> GetMemory(GetMemory(address, line), line);

	public void SetMemory(BigInteger address, BigInteger value)
	{
		if (!_memory.ContainsKey(address))
		{
			MemorySize++;
			_maxMemory[address] = value;
		}
		else
		{
			_maxMemory[address] = BigInteger.Max(_maxMemory[address], value);
		}
		_memory[address] = value;
	}

	public BigInteger GetAccumulator(long line)
	{
		return GetMemory(_accumulatorAddress, line);
	}

	public void SetAccumulator(BigInteger value)
	{
		SetMemory(_accumulatorAddress, value);
	}
	
	public ReadOnlyCollection<Cell> GetHumanReadableData()
	{
		var list = _memory
			.Select(keyVal => new Cell(keyVal.Key.ToString(), keyVal.Value.ToString()))
			.OrderBy(x => x)
			.ToList();
		if (!_memory.ContainsKey(_accumulatorAddress))
		{
			list.Add(new Cell(_accumulatorAddress.ToString(), IMemory.UninitializedValue));
		}

		return list.AsReadOnly();
	}

	public ulong CalculateUniformComplexity() 
		=> MemorySize;

	public ulong CalculateLogarithmicComplexity() 
		=> _maxMemory
			.Select(x => x.Value.LCost())
			.Aggregate((sum, currentItem) => sum + currentItem);
}