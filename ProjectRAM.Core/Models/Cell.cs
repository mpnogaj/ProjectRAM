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
		if (!index.IsNumber() || !value.IsNumber())
		{
			throw new FormatException();
		}

		Index = index;
		Value = value;
	}

	public string Index { get; }
	public string Value { get; }

	public int CompareTo(Cell? other)
	{
		if (other == null)
		{
			return 1;
		}

		var v1 = BigInteger.Parse(this.Index);
		var v2 = BigInteger.Parse(other.Index);
		return v1.CompareTo(v2);
	}

	public override bool Equals(object? obj)
	{
		return obj is Cell lhs && Equals(lhs);
	}

	private bool Equals(Cell other)
	{
		return Index == other.Index && Value == other.Value;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Index, Value);
	}
}