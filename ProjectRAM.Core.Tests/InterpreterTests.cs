using Xunit;
using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Tests
{
	public class InterpreterTests
	{
		[Theory]
		[MemberData(nameof(Data))]
		public void RunCommandsTest(List<Command> program, Queue<string> inputTape, Queue<string> expectedOutputTape, Dictionary<string, string> expectedMemory)
		{
			var interpreter = new Interpreter(program, inputTape);
			var result = interpreter.RunCommands();
			var actualOutputTape = result.Item1;
			var actualMemory = result.Item2;
			Assert.Equal(expectedOutputTape, actualOutputTape);
			Assert.Equal(expectedMemory, actualMemory);
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
				new Queue<string>(new List<string>()
				{
					"15"
				}),
				new Queue<string>(new List<string>()
				{
					"15"
				}),
				new Dictionary<string, string>()
				{
					{"0", "15"}
				}
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
					new Command("write 0")
				},
				new Queue<string>(new List<string>()
				{
					"2"
				}),
				new Queue<string>(new List<string>()
				{
					"65536"
				}),
				new Dictionary<string, string>()
				{
					{"0", "65536"}
				}
			},

			new object[]
			{
				new List<Command>()
				{
					new Command("read 1"),
					new Command("load 1 #pobierzOstatniąCyfrę"),
					new Command("div =10"),
					new Command("mult =10"),
					new Command("store 10"),
					new Command("load 1"),
					new Command("sub 10"),
					new Command("store 2"),
					new Command("load 1 #zamieńNaZera"),
					new Command("div =1000"),
					new Command("mult =1000"),
					new Command("add 2 #dodajOstatnią"),
					new Command("store 3 #zapisz"),
					new Command("write 3"),
				},
				new Queue<string>(new List<string>()
				{
					"5694"
				}),
				new Queue<string>(new List<string>()
				{
					"5004"
				}),
				new Dictionary<string, string>()
				{
					{"0", "5004"},
					{"1", "5694"},
					{"2", "4"},
					{"3", "5004"},
					{"10", "5690"},
				}
			},

			new object[]
			{
				new List<Command>()
				{
					new Command("read 0"),
					new Command("jzero a"),
					new Command("jump b"),
					new Command("a: halt"),
					new Command("b: write 0"),
				},
				new Queue<string>(new List<string>()
				{
					"0"
				}),
				new Queue<string>(),
				new Dictionary<string, string>()
				{
					{"0", "0"},
				}
			},

			new object[]
			{
				new List<Command>()
				{
					new Command("write =20"),
				},
				new Queue<string>(),
				new Queue<string>(new List<string>()
				{
					"20"
				}),
				new Dictionary<string, string>()
				{
					{"0", "?"},
				}
			},
		};
	}
}