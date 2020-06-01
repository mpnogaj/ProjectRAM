using System;
using System.Collections.Generic;
using System.Numerics;

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
                if (command.Label == lbl + ':')
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

        /// <summary>
        /// Funkcja symulująca wykonanie wszystkich poleceń
        /// </summary>
        /// <param name="commands">Lista komend do wykonania</param>
        /// <param name="inputTape">Taśma wejściowa</param>
        /// <returns>Taśmę wyjścia</returns>
        public static Queue<string> RunCommands(List<Command> commands, Queue<string> inputTape)
        {
            //Syf panie, syf
            //Przeorganizować to kiedyś
            List<Cell> memory = new List<Cell>();

            //akumulator
            memory.Add(new Cell(String.Empty, 0));
            Cell akumulator = memory[0];

            //Taśma wyjścia
            Queue<string> output = new Queue<string>();
            for(int i = 0; i < commands.Count; i++)
            {
                //aktualna komenda
                Command cmd = commands[i];

                //indeks aktualnie wykonywanej instrukcji
                int index;
                //argument
                string argStr = cmd.Argument;
                //argument w formie liczby
                BigInteger argInt;

                BigInteger value;
                if (!BigInteger.TryParse(argStr, out argInt))
                {
                    if (argStr.StartsWith('^'))
                    {
                        BigInteger.TryParse(argStr.Substring(1), out argInt);
                        BigInteger.TryParse(memory[GetMemoryIndex(memory, argInt)].Value, out argInt);
                        cmd.ArgumentType = ArgumentType.IndirectAddress;
                    }
                    else if (argStr.StartsWith('='))
                    {
                        BigInteger.TryParse(argStr.Substring(1), out argInt);
                        cmd.ArgumentType = ArgumentType.Const;
                    }
                    else
                    {
                        cmd.ArgumentType = ArgumentType.Label;
                    }
                }
                else
                {
                    cmd.ArgumentType = ArgumentType.DirectAddress;
                }
                switch (cmd.CommandType)
                {
                    case CommandType.Halt:
                        return output;
                    case CommandType.Jump:
                        #region ExceptionHandling
                        if(cmd.ArgumentType != ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        i = Jump(commands, argStr);
                        break;
                    case CommandType.Jgtz:
                        #region ExceptionHandling
                        if (cmd.ArgumentType != ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (akumulator.Value != "" && 
                            akumulator.Value[0] != '-' &&
                            akumulator.Value != "0")
                            i = Jump(commands, argStr);
                        break;
                    case CommandType.Jzero:
                        #region ExceptionHandling
                        if (cmd.ArgumentType != ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (akumulator.Value != "" &&
                            akumulator.Value == "0")
                            i = Jump(commands, argStr);
                        break;
                    case CommandType.Read:
                        #region Exception handling
                        if (cmd.ArgumentType != ArgumentType.DirectAddress && 
                            cmd.ArgumentType != ArgumentType.IndirectAddress)
                            throw new ArgumentIsNotValidException();
                        if(inputTape.Count <= 0)
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
                        if(cmd.ArgumentType == ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (cmd.ArgumentType == ArgumentType.Const)
                        {
                            output.Enqueue(argInt.ToString());
                        }
                        else
                        {
                            index = GetMemoryIndex(memory, argInt);
                            if (index == -1)
                                throw new CellDoesntExistException();
                            output.Enqueue(memory[index].Value);
                        }
                        break;
                    case CommandType.Store:
                        #region ExceptionHandling
                        if (cmd.ArgumentType == ArgumentType.Const || 
                            cmd.ArgumentType == ArgumentType.Label)
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
                        if(cmd.ArgumentType == ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion

                        if (cmd.ArgumentType == ArgumentType.Const)
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
                        if(cmd.ArgumentType == ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (cmd.ArgumentType == ArgumentType.Const)
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
                        akumulator.Value = BigInteger.Add(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                        break;
                    case CommandType.Sub:
                        #region ExceptionHandling
                        if (cmd.ArgumentType == ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (cmd.ArgumentType == ArgumentType.Const)
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
                        akumulator.Value = BigInteger.Subtract(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                        break;
                    case CommandType.Mult:
                        #region ExceptionHandling
                        if (cmd.ArgumentType == ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (cmd.ArgumentType == ArgumentType.Const)
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
                        akumulator.Value = BigInteger.Multiply(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                        break;
                    case CommandType.Div:
                        #region ExceptionHandling
                        if (cmd.ArgumentType == ArgumentType.Label)
                            throw new ArgumentIsNotValidException();
                        #endregion
                        if (cmd.ArgumentType == ArgumentType.Const)
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
                        akumulator.Value = BigInteger.Divide(BigInteger.Parse(akumulator.Value ?? throw new AcumulatorEmptyException()), value).ToString();
                        break;
                }
            }
            return output;
        }
    }
}
