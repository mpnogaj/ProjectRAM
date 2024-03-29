﻿using System;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using ProjectRAM.Core.Commands;

namespace ProjectRAM.Core;

internal static class Extensions
{
	private const string NumberRegexPattern = @"^-?(0|[1-9][0-9]*)$";
	private const string MinusZero = "-0";
	
	public static bool IsValidLabel(this string s)
		=> s.All(char.IsLetterOrDigit) && char.IsLetter(s[0]) && !CommandHelper.CommandNames.Contains(s);

	public static bool IsNumber(this string s)
		=> !s.Any(char.IsWhiteSpace) && Regex.IsMatch(s, NumberRegexPattern) && s != MinusZero;

	public static bool IsZero(this string s)
		=> s.IsNumber() && BigInteger.Parse(s) == BigInteger.Zero;

	public static bool IsPositive(this string s)
		=> s.IsNumber() && BigInteger.Parse(s) > BigInteger.Zero;
	
	public static ulong LCost(this BigInteger bigInteger)
		=> bigInteger == BigInteger.Zero ? 1 : (ulong)Math.Floor(BigInteger.Log10(bigInteger)) + 1;
}