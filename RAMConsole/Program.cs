using CommandLine;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RAMConsole
{
    class Options
    {
        [Value(0, HelpText = "Path to code source file.", Required = true)]
        public string CodePath { get; set; }
        [Option('i', "inputTape", Required = false, HelpText = "Path to input tape file.")]
        public string InputTapePath { get; set; }
    }

    static class Program
    {
        private static int Main(string[] args)
        {
            Queue<string> inputTape = null;
            int result = 0;
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                if (o.InputTapePath != null)
                {
                    inputTape = Creator.CreateInputTapeFromFile(o.InputTapePath);
                }
                var cts = new CancellationTokenSource();
                try
                {
                    Interpreter.RunCommands(Creator.CreateCommandList(args[0]), inputTape, cts.Token);
                    Queue<string> outputTape = Interpreter.OutputTape;
                    while (outputTape.Count > 0)
                    {
                        Console.Write($"{outputTape.Dequeue()} ");
                    }
                }
                catch (RamInterpreterException rie)
                {
                    result = -1;
                    Console.WriteLine(rie.Message);
                    Console.WriteLine($"Exit code {result}");
                }

            }).WithNotParsed(errs =>
            {
                Console.WriteLine($"errors {errs.Count()}", errs.Count());
                if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                {
                    result = -1;
                }

                Console.WriteLine($"Exit code {result}");
            });
            return result;
        }
    }
}
