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
                CancellationTokenSource cts = new CancellationTokenSource();
                try
                {
                    Queue<string> outputTape =
                                       Interpreter.RunCommands(Creator.CreateCommandList(args[0]), inputTape, cts.Token).Item1;
                    while (outputTape.Count > 0)
                    {
                        Console.Write($"{outputTape.Dequeue()} ");
                    }
                }
                catch (RamInterpreterException rie)
                {
                    result = -1;
                    Console.WriteLine(rie.Message);
                    Console.WriteLine("Exit code {0}", result);
                }

            }).WithNotParsed(errs =>
            {
                Console.WriteLine("errors {0}", errs.Count());
                if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                {
                    result = -1;
                }

                Console.WriteLine("Exit code {0}", result);
            });
            return result;
        }
    }
}
