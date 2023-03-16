using CommandLine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ProjectRAM.CLI.Properties;
using ProjectRAM.Core;
using ProjectRAM.Core.Commands;
using ProjectRAM.Core.Converters;
using ProjectRAM.Core.Machine;
using ProjectRAM.Core.Models;

namespace ProjectRAM.CLI
{
	internal static class Program
	{
		private const string CRLF = "\r\n";
		private const string LF = "\n";
		
		private static int Main(string[] args)
		{
			var result = 0;
			CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
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
					fileContent = sr.ReadToEnd().Split(options.UseCRLF ? CRLF : LF);
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

				var interpreter = new Interpreter(CommandFactory.CreateCommandList(fileContent, new HashSet<long>()));

				interpreter.WriteToOutputTape += (_, args) =>
				{
					Console.WriteLine($@"<<< {args.Output}");
				};

				interpreter.ReadFromInputTape += (_, args) =>
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

				var task = Task.Run(() => interpreter.RunCommandsAtFullSpeed(cts.Token), cts.Token);
				task.Wait(cts.Token);
				var result = task.Result;
				
				if (options.ShowMemoryReport)
				{
					PrintMemory(result.Memory);
				}
				if (options.ShowComplexityReport)
				{
					PrintComplexityReport(result.ComplexityReport);
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

		private static void PrintComplexityReport(ComplexityReport complexityReport)
		{
			Console.WriteLine(@"Complexity report: ");
			Console.WriteLine(@"Time complexity:");
			Console.WriteLine($@"Logarithmic: {complexityReport.LogTimeCost}, Uniform: {complexityReport.UniformTimeCost}");
			Console.WriteLine(@"Space complexity:");
			Console.WriteLine($@"Logarithmic: {complexityReport.LogSpaceCost}, Uniform: {complexityReport.UniformSpaceCost}");
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
