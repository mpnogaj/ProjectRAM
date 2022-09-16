using System;
using System.Numerics;

namespace ProjectRAM.Core.Models;

/// <summary>
/// Klasa reprezentującą komórkę pamięci
/// </summary>
public class Cell : IComparable<Cell>
{
	public Cell(string v, string i)
	{
		Value = v;
		Index = i;
	}

	public string Index { get; }
	public string Value { get; set; }

	public int CompareTo(Cell? other) =>
		other == null
			? 1
			: BigInteger.Compare(BigInteger.Parse(Index), BigInteger.Parse(other.Index));
}