using Xunit;
using System.Collections.Generic;
using ProjectRAM.Core.Models;

namespace ProjectRAM.Core.Tests.Models
{
	public class CommandTests
	{
		[Theory]
		[InlineData("100", true)]
		[InlineData("-129", true)]
		[InlineData("1.35", false)]
		[InlineData("100a8", false)]
		[InlineData("ala ma kota", false)]
		[InlineData("100 000", false)]
		public void IsNumberTest(string value, bool expected)
		{
			var actual = value.IsNumber();
			Assert.Equal(expected, actual);
		}

		[Theory]
		[MemberData(nameof(Data))]
		public void CommandTest(Command expected, string value)
		{
			var actual = new Command(0, value);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[MemberData(nameof(Data))]
		public void ToStringTest(Command value, string expected)
		{
			var actual = value.ToString();
			Assert.Equal(expected, actual);
		}

		[Theory]
		[MemberData(nameof(DataEquals))]
		public void EqualsTest(object a, Command b, bool expected)
		{
			if (a != null) Assert.Equal(expected, a.Equals(b));
			Assert.Equal(expected, b.Equals(a));
		}

		[Theory]
		[InlineData("JuMp", CommandType.Jump)]
		[InlineData("jgtz", CommandType.Jgtz)]
		[InlineData("jzero", CommandType.Jzero)]
		[InlineData("add", CommandType.Add)]
		[InlineData("sub", CommandType.Sub)]
		[InlineData("DIV", CommandType.Div)]
		[InlineData("mulT", CommandType.Mult)]
		[InlineData("read", CommandType.Read)]
		[InlineData("write", CommandType.Write)]
		[InlineData("store", CommandType.Store)]
		[InlineData("load", CommandType.Load)]
		[InlineData("halt", CommandType.Halt)]
		[InlineData("", CommandType.Null)]
		[InlineData("djoiadj", CommandType.Unknown)]
		public void GetCommandTypeTest(string commandName, CommandType expected)
		{
			var command = new Command(string.Empty, commandName, string.Empty, string.Empty);
			Assert.Equal(expected, command.CommandType);
		}

		[Theory]
		[InlineData("=21", ArgumentType.Const)]
		[InlineData("", ArgumentType.Null)]
		[InlineData("abc", ArgumentType.Label)]
		[InlineData("^21", ArgumentType.IndirectAddress)]
		[InlineData("15", ArgumentType.DirectAddress)]
		public void GetArgumentTypeTest(string argument, ArgumentType expected)
		{
			var command = new Command(string.Empty, "add", argument, string.Empty);
			Assert.Equal(expected, command.ArgumentType);
		}

		public static IEnumerable<object[]> Data => new List<object[]>
		{
			new object[] { new Command(string.Empty, string.Empty, string.Empty, "loop here"), "#loop here" },
			new object[] { new Command ("halt"), "halt"},
			new object[] { new Command (string.Empty, "halt", string.Empty, string.Empty), "halt"},
			new object[] { new Command("lbl1: add                5 #add 5 to val"), "lbl1: add 5 #add 5 to val" },
			new object[] { new Command("testlbl", "sub", "-5", "shuihsaiu"), "testlbl: sub -5 #shuihsaiu" },
			new object[] { new Command(string.Empty, "mult", "8", "mult"), "mult 8 #mult" },
			new object[] { new Command(string.Empty, "div", "8", string.Empty), "div 8" },
			new object[] { new Command(string.Empty, string.Empty, string.Empty, string.Empty), "" },
			new object[] { new Command(string.Empty, "mult", string.Empty, string.Empty), "mult" },
			new object[] { new Command(string.Empty), string.Empty },
			new object[] { new Command(), string.Empty }
		};
		public static IEnumerable<object[]> DataEquals => new List<object[]>()
		{
			new object[]{ new Command(0, "read 0"), new Command(15, "read 0"), true },
			new object[]{ new Command(string.Empty, "halt", string.Empty, string.Empty), new Command(string.Empty, "HaLt", string.Empty, string.Empty), true },
			new object[]{ new Command(string.Empty, string.Empty, string.Empty, "Bolek"), new Command(string.Empty, string.Empty, string.Empty, "BoLek"), false },
			new object[]{ new Command("ab: halt"), new Command("AB: halt"), false },
			new object[]{ null, new Command(string.Empty, string.Empty, string.Empty, "BoLek"), false },
			new object[]{ new(), new Command(string.Empty, string.Empty, string.Empty, "BoLek"), false },
		};
	}
}