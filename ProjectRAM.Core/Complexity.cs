using ProjectRAM.Core.Models;
using System;
using System.Linq;
using System.Numerics;

namespace ProjectRAM.Core
{
	public class Complexity
	{
		private readonly Interpreter _interpreter;

		public Complexity(Interpreter interpreter)
		{
			TimeLogComplexity = 0;
			TimeUniformComplexity = 0;
			_interpreter = interpreter;
		}

		public long MemoryLogComplexity => _interpreter.MaxMemory.Select(k => k.Value).Where(x => x != "").Sum(Lcost);
		public long MemoryUniformComplexity => _interpreter.Memory.Count;
		public long TimeLogComplexity { get; private set; } = 0;
		public long TimeUniformComplexity { get; private set; } = 0;

		public void UpdateTimeComplexity(Command c, string currentPtr)
		{
			TimeLogComplexity += CommandComplexity(c, currentPtr);
			TimeUniformComplexity++;
		}

		// ReSharper disable once IdentifierTypo
		private static long Lcost(string x)
		{
			switch (x)
			{
				case "?":
					return 0;

				case "0":
					return 1;

				default:
					{
						var bX = BigInteger.Parse(x);
						return (long)Math.Floor(BigInteger.Log10(BigInteger.Abs(bX))) + 1;
					}
			}
		}

		private long CommandComplexity(Command x, string val)
		{
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (x.CommandType)
			{
				case CommandType.Halt:
				case CommandType.Jump:
					return 1;

				case CommandType.Jgtz:
				case CommandType.Jzero:
					return Lcost(M("0"));

				case CommandType.Write:
				case CommandType.Load:
					return T(x.ArgumentType, x.FormattedArg());

				case CommandType.Sub:
				case CommandType.Add:
				case CommandType.Mult:
				case CommandType.Div:
					return T(x.ArgumentType, x.FormattedArg()) + Lcost(M("0"));

				case CommandType.Store:
					if (x.ArgumentType == ArgumentType.DirectAddress) return Lcost(M("0")) + Lcost(x.FormattedArg());
					return Lcost(M("0")) + Lcost(x.FormattedArg()) + Lcost(M(x.FormattedArg()));

				case CommandType.Read:
					if (x.ArgumentType == ArgumentType.DirectAddress) return Lcost(val) + Lcost(x.FormattedArg());
					return Lcost(val) + Lcost(x.FormattedArg()) + Lcost(M(x.FormattedArg()));

				default:
					return 0;
			}
		}

		private string M(string x)
		{
			return _interpreter.Memory[x];
		}

		private long T(ArgumentType a, string x)
		{
			return a switch
			{
				ArgumentType.Const => Lcost(x),
				ArgumentType.DirectAddress => Lcost(x) + Lcost(M(x)),
				ArgumentType.IndirectAddress => Lcost(x) + Lcost(M(x)) + Lcost(M(M(x))),
				_ => 0,
			};
		}
	}
}