using System;
using System.IO;
using System.Collections.Generic;

//Moje usingi
using Common;

namespace RamMachineConsole
{
    class Program
    {
        private static ConsoleColor _defaultColor;

        static List<Command> CreateCommandList(string pathToFile)
        {
            List<Command> commands = new List<Command>();
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if(line == String.Empty)
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
                    commands.Add(new Command(GetCommandType(command), arg, lbl, comm));
                }
            }
            return commands;
        }

        static Queue<string> GetInputTape(string pathToFile)
        {
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string line = sr.ReadLine();
                if(line == null)
                    throw new Exception();
                return new Queue<string>(line.Split(' '));
            }
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

        static void PrintFatalError(string error)
        {
            Console.Write("Ram Machine: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("fatal error: ");
            Console.ForegroundColor = _defaultColor;
            Console.Write(error);
        }

        static void PrintHelp()
        {

        }

        static int Main(string[] args)
        {
            _defaultColor = Console.ForegroundColor;
            List<Command> cmds = new List<Command>();
            Queue<string> inputTape = null;
            switch (args.Length)
            {
                case 0:
                    PrintFatalError("No file specified");
                    return -1;
                case 1:
                    if (args[0] == "help")
                        PrintHelp();
                    else
                        cmds = CreateCommandList(args[0]);
                    break;
                case 2:
                    cmds = CreateCommandList(args[0]);
                    inputTape = GetInputTape(args[1]);
                    break;
            }
            Queue<string> outputTape = Interpreter.RunCommands(cmds, inputTape);
            while(outputTape.Count > 0)
            {
                Console.Write($"{outputTape.Dequeue()} ");
            }

            return 0;
        }
    }
}
