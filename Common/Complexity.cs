using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Complexity
    {
        public static long Lcost(string x)
        {
            if (x == "?") return 0;
            if (x == "0") return 1;
            BigInteger bX = BigInteger.Parse(x);
            return (long)Math.Floor(BigInteger.Log10(BigInteger.Abs(bX))) + 1;
        }

        public static string M(string x)
        {
            return Interpreter.Memory[x];
        }

        public static long T(ArgumentType a, string x)
        {
            return a switch
            {
                ArgumentType.Const => Lcost(x),
                ArgumentType.DirectAddress => Lcost(x) + Lcost(M(x)),
                ArgumentType.IndirectAddress => Lcost(x) + Lcost(M(x)) + Lcost(M(M(x))),
                _ => 0,
            };
        }

        public static long CommandComplexity(Command x, ref int i)
        {
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
                    string input = Interpreter.ReadableInputTape[i];
                    i++;
                    if (x.ArgumentType == ArgumentType.DirectAddress) return Lcost(input) + Lcost(x.FormattedArg());
                    return Lcost(input) + Lcost(x.FormattedArg()) + Lcost(M(x.FormattedArg()));

                default:
                    return 0;
            }
        }

        public static long CountLogarithmicTimeComplexity()
        {
            long complexity = 0;
            int i = 0;
            foreach(Command command in Interpreter.ExecutedCommands)
            {
                complexity += CommandComplexity(command, ref i);
            }
            return complexity;
        }

        public static int CountUniformTimeComplexity() => Interpreter.ExecutedCommands.Count;

        public static long CountLogarithmicMemoryCoplexity()
        {
            long complexity = 0;
            foreach(var memory in Interpreter.MaxMemory.Select(k => k.Value).Where(x => x != ""))
            {
                complexity += Lcost(memory);
            }
            return complexity;
        }

        public static int CountUniformMemoryComplexity() => Interpreter.Memory.Count;
    }
}
