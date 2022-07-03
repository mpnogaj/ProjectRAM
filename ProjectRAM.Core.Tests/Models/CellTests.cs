using Xunit;
using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Tests.Models
{
	public class CellTests
	{
		[Theory]
		[MemberData(nameof(Data))]
		public void CompareToTest(Cell cellA, Cell cellB, int expected)
		{
			if (expected < 0)
			{
				Assert.True(cellA.CompareTo(cellB) < 0);
			}
			else if (expected > 0)
			{
				Assert.True(cellA.CompareTo(cellB) > 0);
			}
			else
			{
				Assert.True(cellA.CompareTo(cellB) == 0);
			}
		}


		public static IEnumerable<object[]> Data => new List<object[]>()
		{
			new object[] { new Cell("11", "2013"), new Cell("1111", "2013"), 0 },
			new object[] { new Cell("0", "31231829031890"), new Cell("3802948", "809380918230918301280"), -1 },
			new object[] { new Cell("0", "-218"), new Cell("48309483", "-581"), 1 },
		};
	}
}