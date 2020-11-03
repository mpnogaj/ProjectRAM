using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Common
{
    public static class Interpreter
    {
        /// <summary>
        /// Funkcja do skakania. Preszukuje wszystkie komendy i jeżeli znajdzie etykiete to skacze do niej.
        /// </summary>
        /// <param name="c">Lista komend</param>
        /// <param name="lbl">Etykieta do wyszukania</param>
        /// <param name="index">Numer linii</param>
        /// <returns>Indeks komendy do której skoczyć</returns>
        private static int Jump(List<Command> c, string lbl, int index)
        {
            int i = 0;
            foreach (Command command in c)
            {
                if (command.Label == lbl)
                {
                    //w pętli for znowu zwięszke 'i' więc się wyrówna.
                    return i - 1;
                }
                i++;
            }
            throw new LabelDoesntExistExcpetion(c[index].Line, lbl);
        }

        private static string GetValue(Command c, string formatedArg, Dictionary<string, string> memory)
        {
            if (c.ArgumentType == ArgumentType.Const)
            {
                return formatedArg;
            }
            else
            {
                bool exists = memory.ContainsKey(formatedArg);
                if (!exists)
                {
                    throw new CellDoesntExistException(c.Line);
                }

                return memory[formatedArg];
            }
        }

        /// <summary>
        /// Funkcja symulująca wykonanie wszystkich poleceń
        /// </summary>
        /// <param name="commands">Lista komend do wykonania</param>
        /// <param name="inputTape">Taśma wejściowa</param>
        /// <param name="token">Token anulowania</param>
        /// <returns>Taśmę wyjścia, pamięć</returns>
        public static Tuple<Queue<string>, Dictionary<string, string>> RunCommands(List<Command> commands, Queue<string> inputTape, CancellationToken token)
        {
            Dictionary<string, string> memory = new Dictionary<string, string>();
            memory.Add("0", string.Empty);
            Queue<string> outputTape = new Queue<string>();
            for (int i = 0; i < commands.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                if (RunCommand(commands, inputTape, outputTape, memory, ref i))
                {
                    break;
                }
            }

            return new Tuple<Queue<string>, Dictionary<string, string>>(outputTape, memory);
        }

        /// <summary>
        /// Run single command
        /// </summary>
        /// <param name="commands">List of another commands (for Jump)</param>
        /// <param name="inputTape">Input tape</param>
        /// <param name="outputTape">Output tape</param>
        /// <param name="memory">Memory</param>
        /// <param name="i">Index of current command in list</param>
        /// <returns>Should end program</returns>
        public static bool RunCommand(List<Command> commands, Queue<string> inputTape, Queue<string> outputTape, Dictionary<string, string> memory, ref int i)
        {
            Command command = commands[i];
            bool exists;
            BigInteger value;
            //argument
            command.ArgumentType = Command.GetArgumentType(command.Argument);
            string arg = command.Argument;
            if (command.ArgumentType == ArgumentType.Const || command.ArgumentType == ArgumentType.IndirectAddress)
            {
                arg = arg.Substring(1);
                if (command.ArgumentType == ArgumentType.IndirectAddress)
                {
                    arg = memory[arg];
                }
            }

            switch (command.CommandType)
            {
                case CommandType.Halt:
                    return true;
                case CommandType.Jump:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    i = Jump(commands, arg, i);
                    break;
                case CommandType.Jgtz:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    if (memory["0"] != "" &&
                        memory["0"][0] != '-' &&
                        memory["0"] != "0")
                    {
                        i = Jump(commands, arg, i);
                    }

                    break;
                case CommandType.Jzero:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    if (memory["0"] != "" &&
                        memory["0"] == "0")
                    {
                        i = Jump(commands, arg, i);
                    }

                    break;
                case CommandType.Read:

                    #region Exception handling

                    if (command.ArgumentType != ArgumentType.DirectAddress &&
                        command.ArgumentType != ArgumentType.IndirectAddress)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    if (inputTape == null || inputTape.Count <= 0)
                    {
                        throw new InputTapeEmptyException(command.Line);
                    }

                    #endregion
                    //index = GetMemoryIndex(memory, argInt);
                    exists = memory.ContainsKey(arg);
                    string val = inputTape.Dequeue();

                    if (exists == false)
                    {
                        memory.Add(arg, val);
                    }
                    else
                    {
                        memory[arg] = val;
                    }

                    break;
                case CommandType.Write:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        outputTape.Enqueue(arg);
                    }
                    else
                    {
                        exists = memory.ContainsKey(arg);
                        if (exists == false)
                        {
                            throw new CellDoesntExistException(command.Line);
                        }

                        outputTape.Enqueue(memory[arg]);
                    }

                    break;
                case CommandType.Store:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Const ||
                        command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    //index = GetMemoryIndex(memory, argInt);
                    exists = memory.ContainsKey(arg);
                    if (!exists)
                    {
                        memory.Add(arg, memory["0"]);
                    }
                    else
                    {
                        memory[arg] = memory["0"];
                    }

                    break;
                case CommandType.Load:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    memory["0"] = GetValue(command, arg, memory);

                    break;
                case CommandType.Add:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, memory));

                    memory["0"] = BigInteger
                        .Add(BigInteger.Parse(memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();
                    break;
                case CommandType.Sub:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, memory));

                    memory["0"] = BigInteger
                        .Subtract(BigInteger.Parse(memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();
                    break;
                case CommandType.Mult:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, memory));

                    memory["0"] = BigInteger
                        .Multiply(BigInteger.Parse(memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();
                    break;
                case CommandType.Div:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, memory));

                    memory["0"] = BigInteger
                        .Divide(BigInteger.Parse(memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();
                    break;
            }
            return false;
        }
    }
}
