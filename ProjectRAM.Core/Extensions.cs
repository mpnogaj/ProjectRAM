using ProjectRAM.Core.Models;
using System;
using System.Linq;
using System.Numerics;

namespace ProjectRAM.Core;

internal static class Extensions
{
	public static bool IsValidLabel(this string s)
		=> s.All(char.IsLetterOrDigit) && char.IsLetter(s[0]) && !Constants.CommandNames.Contains(s);

	public static bool IsNumber(this string s)
		=> BigInteger.TryParse(s, out _);

	public static bool IsZero(this string s, long line)
		=> s.ToNumber(line) == BigInteger.Zero;

	public static bool IsPositive(this string s, long line)
		=> s.ToNumber(line) > BigInteger.Zero;

	public static BigInteger ToNumber(this string s, long line)
	{
		try
		{
			if (s == Constants.UninitializedValue)
			{
				throw new UninitializedCellException(line);
			}
			return BigInteger.Parse(s);
		}
		catch (FormatException)
		{
			throw new ValueIsNaN(line);
		}
	}

	public static ulong LCost(this string s)
	{
		if (s == Constants.UninitializedValue)
		{
			return 0;
		}
		var bi = BigInteger.Abs(BigInteger.Parse(s));
		return bi == BigInteger.Zero ? 1 : (ulong)Math.Floor(BigInteger.Log10(bi)) + 1;
	}
}