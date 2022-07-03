using System.Collections.Generic;
using ProjectRAM.Editor.Models;
using Xunit;

namespace ProjectRAM.Editor.Tests.Models
{
    public class ProgramLineTests
    {
        [Theory]
        [MemberData(nameof(CompleteObjectData))]
        public void ProgramLines_ConstructorCreateObjectFromLine(ProgramLine expected, string line)
        {
            var createdLine = new ProgramLine(line);

            Assert.Equal(expected, createdLine);
            Assert.True(createdLine.Equals(expected));
        }

        public static IEnumerable<object[]> CompleteObjectData => new List<object[]>
        {
            new object[] { new ProgramLine { Label = "lbl1", Command = "add", Argument = "5", Comment = "add 5 to val"}, "lbl1: add 5 #add 5 to val" },
            new object[] { new ProgramLine { Label = "testlbl", Command = "sub", Argument = "-5", Comment = "shuihsaiu"}, "testlbl: sub -5 #shuihsaiu" },
            new object[] { new ProgramLine { Label = string.Empty, Command = "mult", Argument = "8", Comment = "mult" }, "mult 8 #mult" },
            new object[] { new ProgramLine { Label = string.Empty, Command = "jump", Argument = string.Empty, Comment = string.Empty }, "jump" },
            new object[] { new ProgramLine { Label = string.Empty, Command = "jump", Argument = "a", Comment = string.Empty }, "jump a b" },
            new object[] { new ProgramLine { Label = string.Empty, Command = string.Empty, Argument = string.Empty, Comment = string.Empty}, string.Empty },
            new object[] { new ProgramLine { Label = "ab", Command = "jump", Argument = "a", Comment = "xd"}, "a b: jump a b #xd"}
        };
    }
}
