﻿using ProjectRAM.Core.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace ProjectRAM.Core.Tests
{
	public class FactoryTests
	{
		const string RAMCode = "Files/RAMCode/";
		const string InputTape = "Files/InputTape/";

		[Theory()]
		[MemberData(nameof(DataStringCollectionToString))]
		public void CreateStringCollectionTest(string text, string separator, StringCollection expected)
		{
			var actual = Factory.CreateStringCollection(text, separator);
			Assert.Equal(expected, actual);
		}

		[Theory()]
		[MemberData(nameof(DataStringCollectionToString))]
		public void StringCollectionToStringTest(string expected, string join, StringCollection collection)
		{
			var actual = Factory.StringCollectionToString(collection, join);
			Assert.Equal(expected, actual);
		}


		[Theory()]
		[MemberData(nameof(DataStringCollectionToCommandList))]
		public void StringCollectionToCommandListTest(StringCollection collection, List<Command> expected)
		{
			var actual = Factory.StringCollectionToCommandList(collection);
			Assert.Equal(expected, actual);
		}

		[Theory()]
		[MemberData(nameof(DataStringCollectionToCommandList))]
		public void CommandListToStringCollectionTest(StringCollection expected, List<Command> commands)
		{
			var actual = Factory.CommandListToStringCollection(commands);
			Assert.Equal(expected, actual);
		}

		[Theory()]
		[MemberData(nameof(DataFileToCommandList))]
		public void CreateCommandListTest(string filePath, List<Command> expected)
		{
			List<Command> actual = Factory.CreateCommandList(filePath);
			Assert.Equal(expected, actual);
		}

		[Theory()]
		[MemberData(nameof(DataFileToInputTape))]
		public void CreateInputTapeFromFileTest(string filePath, Queue<string> expected)
		{
			var actual = Factory.CreateInputTapeFromFile(filePath);
			Assert.Equal(expected, actual);
		}

		[Theory()]
		[MemberData(nameof(DataStringToInputTape))]
		public void CreateInputTapeFromStringTest(string input, Queue<string> expected)
		{
			var actual = Factory.CreateInputTapeFromString(input);
			Assert.Equal(expected, actual);
		}

		[Theory()]
		[MemberData(nameof(DataStringToOutputTape))]
		public void CreateOutputTapeFromQueueTest(string expected, Queue<string> input)
		{
			var actual = Factory.CreateOutputTapeFromQueue(input);
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> DataStringCollectionToCommandList => new List<object[]>
		{
			new object[] {new StringCollection()
			{
				"read 0",
				"write 0"
			}, new List<Command>()
			{
				new Command("read 0"),
				new Command("write 0")
			} },
			new object[] {new StringCollection()
			{
				"read 0",
				"mult 0",
				"mult 0",
				"mult 0",
				"mult 0",
				"write 0"
			}, new List<Command>()
			{
				new Command("read 0"),
				new Command("mult 0"),
				new Command("mult 0"),
				new Command("mult 0"),
				new Command("mult 0"),
				new Command("write 0")
			} },
			new object[] {new StringCollection()
			{
				"read 1",
				"load 1 #pobierzOstatniąCyfrę",
				"div =10",
				"mult =10",
				"store 10",
				"load 1",
				"sub 10",
				"store 2",
				"load 1 #zamieńNaZera",
				"div =1000",
				"mult =1000",
				"add 2 #dodajOstatnią",
				"store 3 #zapisz",
				"write 3"
			}, new List<Command>()
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
				new Command("write 3")
			} },
		};
		public static IEnumerable<object[]> DataStringCollectionToString => new List<object[]>
		{
			new object[] { "Ala\nma\nkota", "\n", new StringCollection()
			{
				"Ala",
				"ma",
				"kota"
			} },

			new object[] { string.Empty, "\n", new StringCollection()},

			new object[] { "Bolek i Lolek na dzikim zachodzie", string.Empty, new StringCollection()
			{
				"Bolek i Lolek na dzikim zachodzie"
			} },

			new object[] { string.Empty, string.Empty, new StringCollection() },

			new object[] { "The quick brown fox jumps over the lazy dog", " ", new StringCollection()
			{
				"The",
				"quick",
				"brown",
				"fox",
				"jumps",
				"over",
				"the",
				"lazy",
				"dog"
			} }
		};
		public static IEnumerable<object[]> DataFileToCommandList => new List<object[]>
		{
			new object[] {$"{RAMCode}1.RAMCode", new List<Command>()
			{
				new Command("read 0"),
				new Command("write 0")
			} },
			new object[] {$"{RAMCode}2.RAMCode", new List<Command>()
			{
				new Command("read 0"),
				new Command("mult 0"),
				new Command("mult 0"),
				new Command("mult 0"),
				new Command("mult 0"),
				new Command("write 0")
			} },
			new object[] {$"{RAMCode}3.RAMCode", new List<Command>()
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
				new Command("write 3")
			} },
		};
		public static IEnumerable<object[]> DataFileToInputTape => new List<object[]>()
		{
			new object[] {$"{InputTape}1.txt", new Queue<string>(new string[]
			{
				"32342",
				"132",
				"765",
				"10",
				"-1",
				"0"
			})},
			new object[] {$"{InputTape}2.txt", new Queue<string>(new string[]
			{
				"1",
				"1",
				"1",
				"1",
				"1",
				"1",
				"10"
			})},
			new object[] {$"{InputTape}3.txt", new Queue<string>(new string[]
			{
				"25",
				"10",
				"25",
				"10"
			})},
			new object[] {$"{InputTape}4.txt", new Queue<string>(new string[]
			{
				"-1",
				"-50",
				"-70",
				"88"
			})}
		};
		public static IEnumerable<object[]> DataStringToInputTape => new List<object[]>()
		{
			new object[] {"32342 132 765 10 -1 0", new Queue<string>(new string[]
			{
				"32342",
				"132",
				"765",
				"10",
				"-1",
				"0"
			})},
			new object[] {"1 1 1 1 1 1 10", new Queue<string>(new string[]
			{
				"1",
				"1",
				"1",
				"1",
				"1",
				"1",
				"10"
			})},
			new object[] {"25 10 25 10", new Queue<string>(new string[]
			{
				"25",
				"10",
				"25",
				"10"
			})},
			new object[] {"-1 -50 -70 abc 8a8", new Queue<string>(new string[]
			{
				"-1",
				"-50",
				"-70",
				"88"
			})},
			new object[] {string.Empty, new Queue<string>()}
		};
		public static IEnumerable<object[]> DataStringToOutputTape => new List<object[]>()
		{
			new object[] {"32342 132 765 10 -1 0", new Queue<string>(new string[]
			{
				"32342",
				"132",
				"765",
				"10",
				"-1",
				"0"
			})},
			new object[] {"1 1 1 1 1 1 10", new Queue<string>(new string[]
			{
				"1",
				"1",
				"1",
				"1",
				"1",
				"1",
				"10"
			})},
			new object[] {"25 10 25 10", new Queue<string>(new string[]
			{
				"25",
				"10",
				"25",
				"10"
			})},
			new object[] {"-1 -50 -70 88", new Queue<string>(new string[]
			{
				"-1",
				"-50",
				"-70",
				"fjfosjfos",
				"8adadfsff8"
			})},
			new object[] {string.Empty, new Queue<string>()}
		};
	}
}