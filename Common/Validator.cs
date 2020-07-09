using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace Common
{
    public static class Validator
    {
        private static bool FindLabel(List<Command> c, string lbl)
        {
            foreach(var z in c)
            {
                if (z.Label == lbl)
                    return true;
            }
            return false;
        }

        private static void ValidateCommand(Command c, int i)
        {
            if (c.Argument == string.Empty || c.CommandType == CommandType.Unknown)
                throw new LineIsEmptyException(i);
            CommandType t = c.CommandType;
            if (t == CommandType.Halt)
            {
                if (c.ArgumentType == ArgumentType.None)
                    throw new ArgumentIsNotValidException(i);
            }
            else if (t == CommandType.Jump || t == CommandType.Jzero || t == CommandType.Jgtz)
            {
                if (c.ArgumentType != ArgumentType.Label)
                    throw new ArgumentIsNotValidException(i);
            }
            else if (t == CommandType.Read || t == CommandType.Store)
            {
                if (c.ArgumentType != ArgumentType.DirectAddress && c.ArgumentType != ArgumentType.IndirectAddress)
                    throw new ArgumentIsNotValidException(i);
            }
            else
            {
                if (c.ArgumentType == ArgumentType.Label || c.ArgumentType == ArgumentType.None)
                    throw new ArgumentIsNotValidException(i);
            }
        }

        public static List<RamInterpreterException> ValidateProgram(List<Command> commands)
        {
            List<RamInterpreterException> exceptions = new List<RamInterpreterException>();
            List<string> requiredLabels = new List<string>();
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].ArgumentType == ArgumentType.Label)
                    requiredLabels.Add(commands[i].Argument);
                try
                {
                    ValidateCommand(commands[i], i);
                }
                catch(RamInterpreterException ex)
                {
                    exceptions.Add(ex);
                }
            }
            for(int i = 0; i < requiredLabels.Count; i++)
            {
                if (FindLabel(commands, requiredLabels[i]))
                {
                    requiredLabels.RemoveAt(i);
                    i--;
                }
            }
            if (requiredLabels.Count > 0)
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].ArgumentType != ArgumentType.Label)
                        continue;
                    if (requiredLabels.Contains(commands[i].Argument))
                        exceptions.Add(new LabelDoesntExistExcpetion(i, commands[i].Argument));
                }
            }
            return exceptions;
        }

    }
}
