using System;
using System.Numerics
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

        public static int CommandComplexity(Command x, int i, int j)
        {
            switch(x.CommandType)
            {
                case CommandType.Read:
            }
        }
    }
}
