using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Interpreter
    {
        /// <summary>
        /// Funkcja do skakania. Preszukuje wszystkie komendy i jeżeli znajdzie etykiete to skacze do niej.
        /// </summary>
        /// <param name="c">Lista komend</param>
        /// <param name="lbl">Etykieta do wyszukania</param>
        /// <returns>Indeks komendy do której skoczyć</returns>
        private static int Jump(List<Command> c, string lbl)
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
            throw new LabelDoesntExistExcpetion();
        }

        /// <summary>
        /// Funkcja wyszukująca w pamięci komórkę o indeksie x
        /// </summary>
        /// <param name="mem">Lista komórek, pamięć</param>
        /// <param name="index">Indeks szukanej komórki</param>
        /// <returns>Indeks komórki w pamięci. Jeżeli nie ma takiej komórki to zwraca -1</returns>
        private static int GetMemoryIndex(List<Cell> mem, BigInteger index)
        {
            int i = 0;
            foreach (Cell memCell in mem)
            {
                if (memCell.Index == index)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private static CommandType GetCommandType(string switcher)
        {
            //Tymczasowe
            CommandType t = CommandType.Add;
            switch (switcher.ToUpper())
            {
                case "ADD":
                    t = CommandType.Add;
                    break;
                case "SUB":
                    t = CommandType.Sub;
                    break;
                case "MULT":
                    t = CommandType.Mult;
                    break;
                case "DIV":
                    t = CommandType.Div;
                    break;
                case "LOAD":
                    t = CommandType.Load;
                    break;
                case "STORE":
                    t = CommandType.Store;
                    break;
                case "READ":
                    t = CommandType.Read;
                    break;
                case "WRITE":
                    t = CommandType.Write;
                    break;
                case "JUMP":
                    t = CommandType.Jump;
                    break;
                case "JGTZ":
                    t = CommandType.Jgtz;
                    break;
                case "JZERO":
                    t = CommandType.Jzero;
                    break;
                case "HALT":
                    t = CommandType.Halt;
                    break;
            }
            return t;
        }

        public static List<Command> CreateCommandList(string pathToFile)
        {
            List<Command> commands = new List<Command>();
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if(string.IsNullOrWhiteSpace(line))
                        continue;
                    string lbl = string.Empty, command = string.Empty, arg = string.Empty, comm = string.Empty;
                    string word = string.Empty;
                    for (int i = 0; i <= line.Length; i++)
                    {
                        if (i != line.Length && line[i] != ' ')
                            word += line[i];
                        else
                        {
                            if(word == string.Empty)
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
                    if (i != line.Length && line[i] != ' ')
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
            return new Queue<string>(line.Split(' '));
        } 

        /// <summary>
        /// Funkcja symulująca wykonanie wszystkich poleceń
        /// </summary>
        /// <param name="commands">Lista komend do wykonania</param>
        /// <param name="inputTape">Taśma wejściowa</param>
        /// <returns>Taśmę wyjścia</returns>
        public static Tuple<Queue<string>, List<Cell>> RunCommands(List<Command> commands, Queue<string> inputTape)
        {
            //Syf panie, syf
            //Przeorganizować to kiedyś
            List<Cell> memory = new List<Cell>();
            //akumulator
            memory.Add(new Cell(string.Empty, 0));
            //Taśma wyjścia
            Queue<string> outputTape = new Queue<string>();
            for(int i = 0; i < commands.Count; i++)
                if (RunCommand(commands[i], commands, inputTape, outputTape, memory, ref i)) break;
            return new Tuple<Queue<string>, List<Cell>>(outputTape, memory);
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
        public static bool RunCommand(Command command, List<Command> commands, Queue<string> inputTape, Queue<string> outputTape, List<Cell> memory, ref int i)
        {
            Cell akumulator = memory[0];
            //indeks pamieci
            int index;
            //argument
            string argStr = command.Argument;
            //argument w formie liczby
            BigInteger argInt;

            BigInteger value;
            if (!BigInteger.TryParse(argStr, out argInt))
            {
                if (argStr.StartsWith('^'))
                {
                    BigInteger.TryParse(argStr.Substring(1), out argInt);
                    BigInteger.TryParse(memory[GetMemoryIndex(memory, argInt)].Value, out argInt);
                    command.ArgumentType = ArgumentType.IndirectAddress;
                }
                else if (argStr.StartsWith('='))
                {
                    BigInteger.TryParse(argStr.Substring(1), out argInt);
                    command.ArgumentType = ArgumentType.Const;
                }
                else
                {
                    command.ArgumentType = ArgumentType.Label;
                }
            }
            else
            {
                command.ArgumentType = ArgumentType.DirectAddress;
            }

            switch (command.CommandType)
            {
                case CommandType.Halt:
                {
                    return true;
                }
                case CommandType.Jump:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    i = Jump(commands, argStr);
                    break;
                case CommandType.Jgtz:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (akumulator.Value != "" &&
                        akumulator.Value[0] != '-' &&
                        akumulator.Value != "0")
                        i = Jump(commands, argStr);
                    break;
                case CommandType.Jzero:

                    #region ExceptionHandling

                    if (command.ArgumentType != ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (akumulator.Value != "" &&
                        akumulator.Value == "0")
                        i = Jump(commands, argStr);
                    break;
                case CommandType.Read:

                    #region Exception handling

                    if (command.ArgumentType != ArgumentType.DirectAddress &&
                        command.ArgumentType != ArgumentType.IndirectAddress)
                        throw new ArgumentIsNotValidException();
                    if (inputTape.Count <= 0)
                        throw new InputTapeEmptyException();

                    #endregion

                    index = GetMemoryIndex(memory, argInt);
                    string val = inputTape.Dequeue();

                    if (index == -1)
                    {
                        memory.Add(new Cell(val, argInt));
                    }
                    else
                    {
                        memory[index].Value = val;
                    }

                    break;
                case CommandType.Write:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        outputTape.Enqueue(argInt.ToString());
                    }
                    else
                    {
                        index = GetMemoryIndex(memory, argInt);
                        if (index == -1)
                            throw new CellDoesntExistException();
                        outputTape.Enqueue(memory[index].Value);
                    }

                    break;
                case CommandType.Store:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Const ||
                        command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    index = GetMemoryIndex(memory, argInt);
                    if (index == -1)
                        memory.Add(new Cell(akumulator.Value, argInt));
                    else
                        memory[index].Value = akumulator.Value;
                    break;
                case CommandType.Load:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        akumulator.Value = argInt.ToString();
                    }
                    else
                    {
                        index = GetMemoryIndex(memory, argInt);
                        if (index == -1)
                            throw new CellDoesntExistException();
                        akumulator.Value = memory[index].Value;
                    }

                    break;
                case CommandType.Add:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        value = argInt;
                    }
                    else
                    {
                        index = GetMemoryIndex(memory, argInt);
                        if (index == -1)
                            throw new CellDoesntExistException();
                        value = BigInteger.Parse(memory[index].Value);
                    }

                    akumulator.Value = BigInteger
                        .Add(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                    break;
                case CommandType.Sub:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        value = argInt;
                    }
                    else
                    {
                        index = GetMemoryIndex(memory, argInt);
                        if (index == -1)
                            throw new CellDoesntExistException();
                        value = BigInteger.Parse(memory[index].Value);
                    }

                    akumulator.Value = BigInteger
                        .Subtract(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                    break;
                case CommandType.Mult:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        value = argInt;
                    }
                    else
                    {
                        index = GetMemoryIndex(memory, argInt);
                        if (index == -1)
                            throw new CellDoesntExistException();
                        value = BigInteger.Parse(memory[index].Value);
                    }

                    akumulator.Value = BigInteger
                        .Multiply(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                    break;
                case CommandType.Div:

                    #region ExceptionHandling

                    if (command.ArgumentType == ArgumentType.Label)
                        throw new ArgumentIsNotValidException();

                    #endregion

                    if (command.ArgumentType == ArgumentType.Const)
                    {
                        value = argInt;
                    }
                    else
                    {
                        index = GetMemoryIndex(memory, argInt);
                        if (index == -1)
                            throw new CellDoesntExistException();
                        value = BigInteger.Parse(memory[index].Value);
                    }

                    akumulator.Value = BigInteger
                        .Divide(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                    break;
            }

            return false;
        }
    }
}
