using System.Collections.ObjectModel;
using System.Numerics;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Machine.Abstraction;

public interface IMemory
{
	public BigInteger GetMemory(BigInteger address, long line);
	public BigInteger GetIndirectMemory(BigInteger address, long line);
	public void SetMemory(BigInteger address, BigInteger value);
	public BigInteger GetAccumulator(long line);
	public void SetAccumulator(BigInteger value);
	public ulong CalculateUniformComplexity();
	public ulong CalculateLogarithmicComplexity();
	public ReadOnlyCollection<Cell> GetHumanReadableData();
	public ulong MemorySize { get; }

	public static string UninitializedValue { get; } = "?";
}