using ProjectRAM.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace ProjectRAM.Core.Tests
{
	public class ValidatorTests
	{
		[Theory()]
		[MemberData(nameof(Data))]
		public void ValidateProgramTest(List<Command> program, List<RamInterpreterException> expected)
		{
			var actual = Validator.ValidateProgram(program);
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> Data => new List<object[]>
		{
			new object[]
			{
				new List<Command>()
				{
					new Command("read 0"),
					new Command("write 0")
				},
				new List<RamInterpreterException>()
			},

			new object[]
			{
				new List<Command>()
				{
					new Command("read 0"),
					new Command("mult 0"),
					new Command("mult 0"),
					new Command("mult 0"),
					new Command("mult 0"),
					new Command("write 0"),
					new Command("a: halt")
				},
				new List<RamInterpreterException>()
			},
		};
	}
}