using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
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
        /// <param name="line">Numer linii</param>
        /// <returns>Indeks komendy do której skoczyć</returns>
        private static int Jump(List<Command> c, string lbl, int line)
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
            throw new LabelDoesntExistExcpetion(line, lbl);
        }

        private static CommandType GetCommandType(string switcher)
        {
            //Tymczasowe
            CommandType t;
            switch (switcher.ToUpper())
            {
                case "ADD":
                    return CommandType.Add;
                case "SUB":
                    return CommandType.Sub;
                case "MULT":
                    return CommandType.Mult;
                case "DIV":
                    return CommandType.Div;
                case "LOAD":
                    return CommandType.Load;
                case "STORE":
                    return CommandType.Store;
                case "READ":
                    t = CommandType.Read;
                    break;
                case "WRITE":
                    return CommandType.Write;
                case "JUMP":
                    t = CommandType.Jump;
                    break;
                case "JGTZ":
                    return CommandType.Jgtz;
                case "JZERO":
                    return CommandType.Jzero;
                case "HALT":
                    return CommandType.Halt;
                default:
                    return CommandType.Unknown;
            }
            return t;
        }

        private static string GetValue(ArgumentType at, string arg, Dictionary<string, string> memory, int i)
        {
            if (at == ArgumentType.Const)
            {
                return arg;
            }
            else
            {
                bool exists = memory.ContainsKey(arg);
                if (!exists)
                    throw new CellDoesntExistException(i);
                return memory[arg];
            }
        }

        #region Functions to create command list
        public static List<Command> CreateCommandList(string pathToFile)
        {
            List<Command> commands = new List<Command>();
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    string lbl = string.Empty, command = string.Empty, arg = string.Empty, comm = string.Empty;
                    string word = string.Empty;
                    for (int i = 0; i <= line.Length; i++)
                    {
                        if (i != line.Length && line[i] != ' ' && line[i] != '\r' && line[i] != '\n')
                            word += line[i];
                        else
                        {
                            if (word == string.Empty)
                                continue;
                            if (word.EndsWith(':'))
                                lbl = word;
                            else if (word.StartsWith('#'))
                                comm = word;
                            else
                            {
                                if (command == string.Empty)
                                    command = word;
                                else
                                    arg = word;
                            }

                            word = string.Empty;
                        }
                    }
                    commands.Add(new Command(GetCommandType(command),
                    Regex.Replace(arg, @"\t|\n|\r", ""), lbl,
                    Regex.Replace(comm, @"\t|\n|\r", "")));
                }
            }
            return commands;
        }

        public static List<Command> CreateCommandList(StringCollection lines)
        {
            List<Command> commands = new List<Command>();
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string lbl = string.Empty, command = string.Empty, arg = string.Empty, comm = string.Empty;
                string word = string.Empty;
                for (int i = 0; i <= line.Length; i++)
                {
                    if (i != line.Length && line[i] != ' ' && line[i] != '\r' && line[i] != '\n')
                        word += line[i];
                    else
                    {
                        if (word == string.Empty)
                            continue;
                        if (word.EndsWith(':'))
                            lbl = word.Substring(0, word.Length - 1);
                        else if (word.StartsWith('#'))
                            comm = word.Substring(1);
                        else
                        {
                            if (command == string.Empty)
                                command = word;
                            else if (arg == string.Empty)
                                arg = word;
                            else
                                comm += ' ' + word;
                        }
                        word = string.Empty;
                    }
                }
                commands.Add(new Command(GetCommandType(command),
                    Regex.Replace(arg, @"\t|\n|\r", ""), lbl,
                    Regex.Replace(comm, @"\t|\n|\r", "")));
            }
            return commands;
        }
        #endregion

        #region Functions to create input tape
        public static Queue<string> CreateInputTapeFromFile(string pathToFile)
        {
            Queue<string> inputTape = new Queue<string>();
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    inputTape.Enqueue(line);
                }
            }

            return inputTape;
        }

        public static Queue<string> CreateInputTapeFromString(string line)
        {
            if (line == string.Empty)
                return null;
            return new Queue<string>(line.Split(' '));
        }
        #endregion

        /// <summary>
        /// Funkcja symulująca wykonanie wszystkich poleceń
        /// </summary>
        /// <param name="commands">Lista komend do wykonania</param>
        /// <param name="inputTape">Taśma wejściowa</param>
        /// <param name="token">Token anulowania</param>
        /// <returns>Taśmę wyjścia, pamięć</returns>
        public static Tuple<Queue<string>, Dictionary<string, string>> RunCommands(List<Command> commands, Queue<string> inputTape, CancellationToken token)
        {
            //Syf panie, syf
            //Przeorganizować to kiedyś

            Dictionary<string, string> memory = new Dictionary<string, string>();
            //akumulator
            memory.Add("0", string.Empty);
            //Taśma wyjścia
            Queue<string> outputTape = new Queue<string>();
            for (int i = 0; i < commands.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                if (RunCommand(commands[i], commands, inputTape, outputTape, memory, ref i)) break;
            }

            return new Tuple<Queue<string>, Dictionary<string, string>>(outputTape, memory);
        }

        /// <summary>
        /// Run single command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="commands">List of another commands (for Jump)</param>
        /// <param name="inputTape">Input tape</param>
        /// <param name="outputTape">Output tape</param>
        /// <param name="memory">Memory</param>
        /// <param name="i">Index of current command in list (for Jump)</param>
        /// <returns>Should end program</returns>
        public static bool RunCommand(Command command, List<Command> commands, Queue<string> inputTape, Queue<string> outputTape, Dictionary<string, string> memory, ref int i)
        {
            bool exists;
            BigInteger value;
            //argument
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
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    i = Jump(commands, arg, i);
                    break;
                case CommandType.Jgtz:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    if (memory["0"] != "" &&
                        memory["0"][0] != '-' &&
                        memory["0"] != "0")
                        i = Jump(commands, arg, i);
                    break;
                case CommandType.Jzero:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    if (memory["0"] != "" &&
                        memory["0"] == "0")
                        i = Jump(commands, arg, i);
                    break;
                case CommandType.Read:

                    #region Exception handling

                    if (command.ArgumentType != ArgumentType.DirectAddress &&
                        command.ArgumentType != ArgumentType.IndirectAddress)
                        throw new ArgumentIsNotValidException(i);
                    if (inputTape == null || inputTape.Count <= 0)
                        throw new InputTapeEmptyException(i);

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
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        outputTape.Enqueue(arg);
                    }
                    else
                    {
                        //index = GetMemoryIndex(memory, argInt);
                        exists = memory.ContainsKey(arg);
                        if (exists == false)
                            throw new CellDoesntExistException(i);
                        outputTape.Enqueue(memory[arg]);
                    }

                    break;
                case CommandType.Store:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Const ||
                        command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    //index = GetMemoryIndex(memory, argInt);
                    exists = memory.ContainsKey(arg);
                    if (!exists)
                        memory.Add(arg, memory["0"]);
                    else
                        memory[arg] = memory["0"];
                    break;
                case CommandType.Load:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    memory["0"] = GetValue(command.ArgumentType, arg, memory, i);

                    break;
                case CommandType.Add:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    value = BigInteger.Parse(GetValue(command.ArgumentType, arg, memory, i));

                    memory["0"] = BigInteger
                        .Add(BigInteger.Parse(memory["0"] ?? throw new AcumulatorEmptyException(i)), value).ToString();
                    break;
                case CommandType.Sub:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    value = BigInteger.Parse(GetValue(command.ArgumentType, arg, memory, i));

                    memory["0"] = BigInteger
                        .Subtract(BigInteger.Parse(memory["0"] ?? throw new AcumulatorEmptyException(i)), value).ToString();
                    break;
                case CommandType.Mult:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    value = BigInteger.Parse(GetValue(command.ArgumentType, arg, memory, i));

                    memory["0"] = BigInteger
                        .Multiply(BigInteger.Parse(memory["0"] ?? throw new AcumulatorEmptyException(i)), value).ToString();
                    break;
                case CommandType.Div:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException(i);

                    #endregion

                    value = BigInteger.Parse(GetValue(command.ArgumentType, arg, memory, i));

                    memory["0"] = BigInteger
                        .Divide(BigInteger.Parse(memory["0"] ?? throw new AcumulatorEmptyException(i)), value).ToString();
                    break;
            }

            //memory["0"] = akumulator.Value;
            return false;
        }
    }
}
