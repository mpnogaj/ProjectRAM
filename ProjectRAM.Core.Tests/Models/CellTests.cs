using System;
using System.Collections.Generic;
using ProjectRAM.Core.Models;
using Xunit;

namespace ProjectRAM.Core.Tests.Models;

public class CellTests
{
	[Theory, MemberData(nameof(ConstructorTestData))]
	public void ConstructorTestValidFormat(string index, string value, Cell expected)
	{
		var actual = new Cell(index, value);
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("abc", "db")]
	[InlineData("d", "5")]
	[InlineData("12", " 32")]
	public void ConstructorTestInvalidFormat(string index, string value)
	{
		Assert.Throws<FormatException>(() => new Cell(index, value));
	}

	public static IEnumerable<object[]> ConstructorTestData =>
		new[]
		{
			new object[] { "1", "12", new Cell("1", "12") },
			new object[] { "21", "21", new Cell("21", "21") }
		};
}