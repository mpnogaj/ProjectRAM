using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

		[Option('t', "timeout", HelpText = "Timeout in seconds after which program stops. Everything <= 0 will set timeout to infinity", Required = false)]
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

				var cts = options.Timeout <= 0
					? new CancellationTokenSource()
					: new CancellationTokenSource(TimeSpan.FromSeconds(options.Timeout));
				var interpreter = new Interpreter(Factory.CreateCommandList(options.CodePath));
				interpreter.WriteToOutputTape += (sender, args) =>
				{
					Console.WriteLine($@"<<< {args.Output}");
				};
				interpreter.ReadFromInputTape += (sender, args) =>
				{
					if (inputTape is { Count: > 0 })
					{
						args.Input = inputTape.Dequeue();
					}
					else
					{
						string? input;
						bool res;
						do
						{
							Console.Write(@">>> ");
							input = Console.ReadLine();
							res = BigInteger.TryParse(input, out var _);
							if (!res)
							{
								Console.WriteLine(Resources.ParseError);
							}
						} while (!res);
						args.Input = input;
					}
				};
				var mem = interpreter.RunCommands(cts.Token);

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
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
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
