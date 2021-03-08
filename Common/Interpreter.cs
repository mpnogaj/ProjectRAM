using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Common
{
    public static class Interpreter
    {
        /// <summary>
        /// Variables whitch can be read after executin program
        /// Cleared and initialized at the begging of the program
        /// </summary>
        public static List<Command> Program = new List<Command>();
        public static Dictionary<string, string> Memory = new Dictionary<string, string>();
        public static Dictionary<string, string> MaxMemory = new Dictionary<string, string>();
        public static List<string> ReadableInputTape = new List<string>();
        public static Queue<string> InputTape = new Queue<string>();
        public static Queue<string> OutputTape = new Queue<string>();
        public static List<Command> ExecutedCommands = new List<Command>();

        public static bool Executed = false;

        /// <summary>
        /// Funkcja do skakania. Preszukuje wszystkie komendy i jeżeli znajdzie etykiete to skacze do niej.
        /// </summary>
        /// <param name="c">Lista komend</param>
        /// <param name="lbl">Etykieta do wyszukania</param>
        /// <param name="index">Numer linii</param>
        /// <returns>Indeks komendy do której skoczyć</returns>
        private static int Jump(string lbl, int index)
        {
            int i = 0;
            foreach (Command command in Program)
            {
                if (command.Label == lbl)
                {
                    //w pętli for znowu zwięszke 'i' więc się wyrówna.
                    return i - 1;
                }
                i++;
            }
            throw new LabelDoesntExistExcpetion(Program[index].Line, lbl);
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
        public static void RunCommands(List<Command> commands, Queue<string> inputTape, CancellationToken token)
        {
            ExecutedCommands = new List<Command>();
            Memory = new Dictionary<string, string>();
            MaxMemory = new Dictionary<string, string>();
            OutputTape = new Queue<string>();
            InputTape = new Queue<string>();
            ReadableInputTape = new List<string>();
            Executed = false;

            if (commands != null)
            {
                Program = commands;
            }
            Memory.Add("0", string.Empty);
            MaxMemory.Add("0", string.Empty);
            if (inputTape != null)
            {
                InputTape = inputTape;
                ReadableInputTape = InputTape == null ? new List<string>() : new List<string>(InputTape.ToArray());
            }
            for (int i = 0; i < commands.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                if (RunCommand(ref i))
                {
                    break;
                }
            }
            Executed = true;
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
        public static bool RunCommand(ref int i)
        {
            Command command = Program[i];
            ExecutedCommands.Add(command);
            bool exists;
            BigInteger value;
            //argument
            command.ArgumentType = Command.GetArgumentType(command.Argument);
            string arg = command.FormatedArg();
            if (command.ArgumentType == ArgumentType.IndirectAddress)
            {
                arg = Memory[arg];
            }
            //if (command.ArgumentType == ArgumentType.Const || command.ArgumentType == ArgumentType.IndirectAddress)
            //{
            //    arg = arg.Substring(1);
            //    if (command.ArgumentType == ArgumentType.IndirectAddress)
            //    {
            //        arg = Memory[arg];
            //    }
            //}

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

                    i = Jump(arg, i);
                    break;
                case CommandType.Jgtz:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    if (Memory["0"] != "" &&
                        Memory["0"][0] != '-' &&
                        Memory["0"] != "0")
                    {
                        i = Jump(arg, i);
                    }

                    break;
                case CommandType.Jzero:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    if (Memory["0"] != "" &&
                        Memory["0"] == "0")
                    {
                        i = Jump(arg, i);
                    }

                    break;
                case CommandType.Read:

                    #region Exception handling

                    if (command.ArgumentType != ArgumentType.DirectAddress &&
                        command.ArgumentType != ArgumentType.IndirectAddress)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    if (InputTape == null || InputTape.Count <= 0)
                    {
                        throw new InputTapeEmptyException(command.Line);
                    }

                    #endregion
                    exists = Memory.ContainsKey(arg);
                    string val = InputTape.Dequeue();

                    if (exists == false)
                    {
                        Memory.Add(arg, val);
                        MaxMemory.Add(arg, val);
                    }
                    else
                    {
                        Memory[arg] = val;
                        MaxMemory[arg] = Max(val, MaxMemory[arg]);
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
                        OutputTape.Enqueue(arg);
                    }
                    else
                    {
                        exists = Memory.ContainsKey(arg);
                        if (exists == false)
                        {
                            throw new CellDoesntExistException(command.Line);
                        }

                        OutputTape.Enqueue(Memory[arg]);
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
                    exists = Memory.ContainsKey(arg);
                    if (!exists)
                    {
                        Memory.Add(arg, Memory["0"]);
                        MaxMemory.Add(arg, Memory["0"]);
                    }
                    else
                    {
                        Memory[arg] = Memory["0"];
                        MaxMemory[arg] = Max(Memory["0"], MaxMemory[arg]);
                    }

                    break;
                case CommandType.Load:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    Memory["0"] = GetValue(command, arg, Memory);
                    MaxMemory["0"] = Max(Memory["0"], MaxMemory["0"]);

                    break;
                case CommandType.Add:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, Memory));

                    Memory["0"] = BigInteger
                        .Add(BigInteger.Parse(Memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();

                    MaxMemory["0"] = Max(Memory["0"], MaxMemory["0"]);
                    break;
                case CommandType.Sub:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, Memory));

                    Memory["0"] = BigInteger
                        .Subtract(BigInteger.Parse(Memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();

                    MaxMemory["0"] = Max(Memory["0"], MaxMemory["0"]);
                    break;
                case CommandType.Mult:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, Memory));

                    Memory["0"] = BigInteger
                        .Multiply(BigInteger.Parse(Memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();

                    MaxMemory["0"] = Max(Memory["0"], MaxMemory["0"]);
                    break;
                case CommandType.Div:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                    {
                        throw new ArgumentIsNotValidException(command.Line);
                    }

                    #endregion

                    value = BigInteger.Parse(GetValue(command, arg, Memory));

                    Memory["0"] = BigInteger
                        .Divide(BigInteger.Parse(Memory["0"] ?? throw new AccumulatorEmptyException(command.Line)), value).ToString();

                    MaxMemory["0"] = Max(Memory["0"], MaxMemory["0"]);
                    break;
                default:
                    ExecutedCommands.RemoveAt(ExecutedCommands.Count - 1);
                    break;
            }
            return false;
        }

        private static string Max(string v1, string v2)
        {
            if (string.Empty == v1)
            {
                return v2;
            }
            else if (string.Empty == v2)
            {
                return v1;
            }

            if (BigInteger.Parse(v1) >= BigInteger.Parse(v2))
            {
                return v1;
            }
            return v2;
        }
    }
}
