using System;
using System.Collections.Generic;
using System.Numerics;
using ProjectRAM.Core.Machine.Abstraction;
using Xunit;

namespace ProjectRAM.Core.Tests;

public class ExtensionsTests
{
	[Theory]
	[InlineData("add", false)]
	[InlineData("lbl1", true)]
	[InlineData("#ala", false)]
	[InlineData("test label", false)]
	[InlineData("test#label", false)]
	[InlineData("1lbl", false)]
	public void IsValidLabelTests(string label, bool expected)
	{
		Assert.Equal(expected, label.IsValidLabel()); 
	}

	[Theory]
	[InlineData("0", true)]
	[InlineData("00", false)]
	[InlineData("01", false)]
	[InlineData(" 13", false)]
	[InlineData(" 034", false)]
	[InlineData("10ala01", false)]
	[InlineData("-100", true)]
	[InlineData("1000000000000000000000000000000000000000000000000000000", true)]
	[InlineData("3 450", false)]
	[InlineData("-0", false)]
	[InlineData("", false)]
	public void IsNumberTests(string value, bool expected)
	{
		Assert.Equal(expected, value.IsNumber());
	}

	[Theory]
	[InlineData("0", true)]
	[InlineData("00", false)]
	[InlineData("01", false)]
	[InlineData("13", false)]
	[InlineData(" 0", false)]
	[InlineData("0a", false)]
	[InlineData("-100", false)]
	public void IsZeroTest(string value, bool expected)
	{
		Assert.Equal(expected, value.IsZero());
	}
	
	[Theory]
	[InlineData("0", false)]
	[InlineData("10000000000000000000000000000000000000000000000000000000000", true)]
	[InlineData("-10000000000000000000000000000000000000000000000000000000000", false)]
	[InlineData("13", true)]
	[InlineData("-1", false)]
	public void IsPositiveTest(string value, bool expected)
	{
		Assert.Equal(expected, value.IsPositive());
	}

	[Theory, MemberData(nameof(LcostBigIntegerData))]
	public void LCostBigInteger(BigInteger bigInt, ulong expected)
	{
		Assert.Equal(expected, bigInt.LCost());
	}

	public static IEnumerable<object[]> LCostTestValidFormatData =>
		new[]
		{
			new object[] { IMemory.UninitializedValue, 0 },
			new object[] { "0", 1 },
			new object[] { "100", 3 },
			new object[] { "-100", 3 }
		};

	public static IEnumerable<object[]> LcostBigIntegerData =>
		new[]
		{
			new object[] { BigInteger.Zero, 1 },
			new object[] { BigInteger.One, 1 },
			new object[] { BigInteger.MinusOne, 1 },
			new object[] { BigInteger.Parse("125634"), 6 },
		};
}