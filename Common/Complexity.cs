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
        public static int Lcost(string x)
        {
            if (x == "?") return 0;
            if (x == "0") return 1;
            BigInteger bX = BigInteger.Parse(x);
            return (int)Math.Floor(BigInteger.Log10(BigInteger.Abs(bX))) + 1;
        }

        public static string M(string x)
        {
            return Interpreter.Memory[x];
        }

        public static int T(ArgumentType a, string x)
        {
            switch(a)
            {
                case ArgumentType.Const:
                    return Lcost(x);
                case ArgumentType.DirectAddress:
                    return Lcost(x) + Lcost(M(x));
                case ArgumentType.IndirectAddress:
                    return Lcost(x) + Lcost(M(x)) + Lcost(M(M(x)));
                default:
                    return 0;
            }
        }

        public static int CommandComplexity(Command x, ref int i)
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
                    return T(x.ArgumentType, x.FormatedArg());

                case CommandType.Sub:
                case CommandType.Add:
                case CommandType.Mult:
                case CommandType.Div:
                    return T(x.ArgumentType, x.FormatedArg()) + Lcost(M("0"));

                case CommandType.Store:
                    if (x.ArgumentType == ArgumentType.DirectAddress) return Lcost(M("0")) + Lcost(x.FormatedArg());
                    return Lcost(M("0")) + Lcost(x.FormatedArg()) + Lcost(M(x.FormatedArg()));

                case CommandType.Read:
                    string input = Interpreter.ReadableInputTape[i];
                    i++;
                    if (x.ArgumentType == ArgumentType.DirectAddress) return Lcost(input) + Lcost(x.FormatedArg());
                    return Lcost(input) + Lcost(x.FormatedArg()) + Lcost(M(x.FormatedArg()));

                default:
                    return 0;
            }
        }

        public static int CountLogarithmicTimeComplexity()
        {
            int complexity = 0;
            int i = 0;
            foreach(Command command in Interpreter.ExecutedCommands)
            {
                complexity += CommandComplexity(command, ref i);
            }
            return complexity;
        }

        public static int CountUniformTimeComplexity() => Interpreter.ExecutedCommands.Count;

        public static int CountLogarithmicMemoryCoplexity()
        {
            int complexity = 0;
            foreach(var memory in Interpreter.MaxMemory.Select(k => k.Value))
            {
                complexity += Lcost(memory);
            }
            return complexity;
        }

        public static int CountUniformMemoryComplexity() => Interpreter.Memory.Count;
    }
}
