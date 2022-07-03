using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ProjectRAM.CLI.Properties;
using ProjectRAM.Core;

namespace ProjectRAM.CLI
{
	internal class Options
	{
		[Value(0, HelpText = "Path to RAMCode file.", Required = true)]
		public string CodePath { get; set; } = string.Empty;

		[Value(1, HelpText = "Path to input tape file.", Required = false)]
		public string? InputTapePath { get; set; }

		[Option('t', "timeout", HelpText = "Timeout in milliseconds after which program stops. 1 minute is default value. Everything <= 0 will set the default value", Required = false)]
		public int Timeout { get; set; }
	}

	internal static class Program
	{
		private static int Main(string[] args)
		{
			int result = 0;
			Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
			{
				result = HandleSuccess(o);
			}).WithNotParsed(errors =>
			{
				result = HandleErrors(errors);
			});
			Console.WriteLine(Resources.ProcessExited, result);
			return result;
		}

		private static int HandleSuccess(Options options)
		{
			try
			{
				Queue<string>? inputTape = null;
				if (options.InputTapePath != null)
				{
					inputTape = Factory.CreateInputTapeFromFile(options.InputTapePath);
				}

				var cts = new CancellationTokenSource(options.Timeout <= 0
					? TimeSpan.FromMinutes(1)
					: TimeSpan.FromMilliseconds(options.Timeout));
				var interpreter = new Interpreter(Factory.CreateCommandList(options.CodePath), inputTape);
				interpreter.WriteToOutputTape += (sender, args) =>
				{
					Console.WriteLine(args.Output);
				};
				interpreter.ReadFromInputTape += (sender, args) =>
				{
					Console.Write(@">>> ");
					args.Input = Console.ReadLine();
				};
				var output = interpreter.RunCommands(cts.Token);
				var outputTape = output.Item1;

				return 0;
			}
			catch (RamInterpreterException exception)
			{
				Console.WriteLine(exception.Message);
				return -1;
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine(Resources.ExecutionCancelled);
				return -1;
			}
		}

		private static int HandleErrors(IEnumerable<Error> errors)
		{
			//check if can cast, if not use slower extension methods
			var enumerable = errors as Error[] ?? errors.ToArray();
			Console.WriteLine(Resources.Errors, enumerable.Length);
			if (enumerable.Any(x => x is HelpRequestedError or VersionRequestedError))
			{
				return -1;
			}

			return 0;
		}
	}
}
