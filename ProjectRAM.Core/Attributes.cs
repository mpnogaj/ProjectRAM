using System;

namespace ProjectRAM.Core;

internal class CommandNameAttribute : Attribute
{
	public CommandNameAttribute(string name) : base()
	{
		Name = name;
	}

	public string Name { get; private set; }
}