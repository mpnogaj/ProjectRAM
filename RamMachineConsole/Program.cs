using System;
using System.Collections.Generic;
using System.Threading;
using Common;

namespace RAMConsole
{
    static class Program
    {
        private static ConsoleColor _defaultColor;

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

        private static int Main(string[] args)
        {
            _defaultColor = Console.ForegroundColor;
            Queue<string> inputTape = null;
            switch (args.Length)
            {
                case 0:
                    PrintFatalError("No file specified");
                    return -1;
                case 1:
                    if (args[0] == "help")
                    {
                        PrintHelp();
                        return -1;
                    }
                    break;
                case 2:
                    inputTape = Interpreter.CreateInputTapeFromFile(args[1]);
                    break;
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            Queue<string> outputTape = 
                Interpreter.RunCommands(Interpreter.CreateCommandList(args[0]), inputTape, cts.Token).Item1;
            while(outputTape.Count > 0)
            {
                Console.Write($"{outputTape.Dequeue()} ");
            }
            return 0;
        }
    }
}
