using Xunit;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.DataCollection;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Tests
{
	public class InterpreterTests
	{
		[Theory]
		[MemberData(nameof(Data))]
		public void RunCommandsTest(List<Command> program, Queue<string> inputTape, Queue<string> expectedOutputTape, Dictionary<string, string> expectedMemory)
		{
			var interpreter = new Interpreter(program);
			var actualOutputTape = new Queue<string>();
			interpreter.ReadFromInputTape += (sender, eventArgs) =>
			{
				eventArgs.Input = inputTape.Count > 0
					? inputTape.Dequeue()
					: null;
			};
			interpreter.WriteToOutputTape += (sender, eventArgs) =>
			{
				actualOutputTape.Enqueue(eventArgs.Output);
			};
			var actualMemory = interpreter.RunCommands();
			Assert.Equal(expectedOutputTape, actualOutputTape);
			Assert.Equal(expectedMemory, actualMemory);
		}

		public static IEnumerable<object[]> Data => new List<object[]>
		{
			new object[]
			{
				new List<Command>()
				{
					new("read 0"),
					new("write 0")
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
					new("read 0"),
					new("mult 0"),
					new("mult 0"),
					new("mult 0"),
					new("mult 0"),
					new("write 0")
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
					new("read 1"),
					new("load 1 #pobierzOstatniąCyfrę"),
					new("div =10"),
					new("mult =10"),
					new("store 10"),
					new("load 1"),
					new("sub 10"),
					new("store 2"),
					new("load 1 #zamieńNaZera"),
					new("div =1000"),
					new("mult =1000"),
					new("add 2 #dodajOstatnią"),
					new("store 3 #zapisz"),
					new("write 3"),
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
					new("read 0"),
					new("jzero a"),
					new("jump b"),
					new("a: halt"),
					new("b: write 0"),
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
					new("write =20"),
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