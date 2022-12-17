using ProjectRAM.Core.Commands;
using Xunit;

namespace ProjectRAM.Core.Tests.Commands;

public class CommandFactoryTests
{
	[Theory]
	[InlineData("1")]
	[InlineData("test: mul =4 #test")]
	[InlineData("lbl1: 4 #lol")]
	public void CreateCommandThrowsUnknownCommandTypeException(string line)
	{
		Assert.Throws<UnknownCommandTypeException>(() => CommandFactory.CreateCommand(1, line, false));
	}
}