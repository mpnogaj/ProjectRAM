using Xunit;

namespace ProjectRAM.Core.Tests
{
	public class UtilitiesTests
	{
		[Theory]
		[InlineData("", "", "")]
		[InlineData("1", "", "1")]
		[InlineData("", "3", "3")]
		[InlineData("5", "7", "7")]
		[InlineData("-6", "-9", "-6")]
		[InlineData("1000000000000000000", "10000000000000000000", "10000000000000000000")]
		public void MaxTest(string number1, string number2, string expected)
		{
			string actual = Utilities.Max(number1, number2);
			Assert.Equal(expected, actual);
		}
	}
}
