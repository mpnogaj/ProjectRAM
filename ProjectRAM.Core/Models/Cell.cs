using System;
using System.Numerics;

namespace ProjectRAM.Core.Models;

/// <summary>
/// Klasa reprezentującą komórkę pamięci
/// </summary>
public class Cell : IComparable<Cell>
{
	public Cell(string index, string value)
	{
		Index = index;
		Value = value;
	}

	public string Index { get; }
	public string Value { get; }

	public int CompareTo(Cell? other) =>
		other == null
			? 1
			: BigInteger.Compare(BigInteger.Parse(Index), BigInteger.Parse(other.Index));
}