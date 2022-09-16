using System;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core;

internal static class Extensions
{
	public static bool IsNumber(this string s)
		=> BigInteger.TryParse(s, out _);

	public static bool IsZero(this string s)
		=> BigInteger.TryParse(s, out var res) && res == BigInteger.Zero;

	public static bool IsPositive(this string s)
		=> BigInteger.TryParse(s, out var res) && res > BigInteger.Zero;

	public static long Lcost(this string s)
	{
		var bi = BigInteger.Abs(BigInteger.Parse(s));
		return bi == BigInteger.Zero ? 1 : (long)Math.Floor(BigInteger.Log10(bi)) + 1;
	}

	public static ArgumentType GetArgumentType(this string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return ArgumentType.Null;
		}
		if (s.StartsWith('=') && s.IsNumber())
		{
			return ArgumentType.Const;
		}
		if (s.IsNumber())
		{
			return ArgumentType.DirectAddress;
		}
		if (s.StartsWith('^') && s.IsNumber())
		{
			return ArgumentType.IndirectAddress;
		}
		if (s.All(char.IsLetterOrDigit))
		{
			return ArgumentType.Label;
		}
		return ArgumentType.Invalid;
	}
}