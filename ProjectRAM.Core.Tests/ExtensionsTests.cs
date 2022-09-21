using System;
using System.Runtime.InteropServices;
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

	[Theory]
	[InlineData(Interpreter.UninitializedValue, 0)]
	[InlineData("0", 1)]
	[InlineData("100", 3)]
	[InlineData("-100", 3)]
	public void LCostTestValidFormat(string value, ulong expected)
	{
		Assert.Equal(expected, value.LCost());
	}

	[Theory]
	[InlineData("b01")]
	[InlineData("-0")]
	[InlineData("!")]
	[InlineData("1 000")]
	[InlineData("001")]
	public void LCostTestInvalidFormat(string invalidNumber)
	{
		Assert.Throws<FormatException>(() => invalidNumber.LCost());
	}
}