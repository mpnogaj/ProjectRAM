namespace ProjectRAM.Core.Models
{
	/// <summary>
	/// Typ komendy
	/// </summary>
	public enum CommandType
	{
		Null,
		Unknown,
		Add,
		Sub,
		Mult,
		Div,
		Load,
		Store,
		Read,
		Write,
		Jump,
		Jgtz,
		Jzero,
		Halt
	}
}