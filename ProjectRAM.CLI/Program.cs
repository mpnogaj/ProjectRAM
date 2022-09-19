using CommandLine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using CommandLine.Text;
using ProjectRAM.CLI.Properties;
using ProjectRAM.Core;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Converters;
using ProjectRAM.Core.Models;

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

		[Option('m', "memory", HelpText = "Show memory after successful execution")]
		public bool ShowMemoryReport { get; set; }

		[Option('c', "complexity", HelpText="Show complexity report after successful execution")]
		public bool ShowComplexityReport { get; set; }
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
				string[] fileContent;
				using (var sr = new StreamReader(options.CodePath))
				{
					fileContent = sr.ReadToEnd().Split(Environment.NewLine);
				}

				var exceptions = Validator.ValidateProgram(fileContent);
				if (exceptions.Count > 0)
				{
					PrintValidatorExceptions(exceptions);
					return -1;
				}

				Queue<string>? inputTape = null;
				if (options.InputTapePath != null)
				{
					inputTape = FileToTapeConverter.ConvertFileToInputTape(options.InputTapePath);
				}

				var cts = options.Timeout <= 0
					? new CancellationTokenSource()
					: new CancellationTokenSource(TimeSpan.FromSeconds(options.Timeout));

				var interpreter = new Interpreter(CommandFactory.CreateCommandList(fileContent));

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
				var result = interpreter.RunCommands(cts.Token);
				if (options.ShowMemoryReport)
				{
					PrintMemory(result.Memory);
				}

				if (options.ShowComplexityReport)
				{
					Console.WriteLine(@"Complexity report: ");
					Console.WriteLine(@"Time complexity:");
					Console.WriteLine($@"Logarithmic: {result.ComplexityReport.LogTimeCost}, Uniform: {result.ComplexityReport.UniformTimeCost}");
					Console.WriteLine(@"Space complexity:");
					Console.WriteLine($@"Logarithmic: {result.ComplexityReport.LogSpaceCost}, Uniform: {result.ComplexityReport.UniformSpaceCost}");
				}
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

		private static void PrintValidatorExceptions(List<RamInterpreterException> exceptions)
		{
			foreach (var exception in exceptions)
			{
				Console.WriteLine($@"{exception.Line}: {exception.Message}");
			}
		}

		private static void PrintMemory(ReadOnlyCollection<Cell> memory)
		{
			Console.WriteLine(@"MEMORY: ");
			foreach (var memoryCell in memory)
			{
				Console.WriteLine($@"[{memoryCell.Index}]: {memoryCell.Value}");
			}
			Console.WriteLine();
		}

		private static int HandleErrors(IEnumerable<Error> errors)
		{
			var enumerable = errors as Error[] ?? errors.ToArray();
			Console.WriteLine(Resources.Errors, enumerable.Count());
			if (enumerable.Any(x => x is HelpRequestedError or VersionRequestedError))
			{
				return -1;
			}
			return 0;
		}
	}
}
