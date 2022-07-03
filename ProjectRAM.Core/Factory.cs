using ProjectRAM.Core.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace ProjectRAM.Core
{
	/// <summary>
	/// Class used to create different collections (code, input tape)
	/// </summary>
	public static class Factory
	{
		/// <summary>
		/// Convert List of commands to code lines
		/// </summary>
		/// <param name="commands">List of commands</param>
		/// <returns>Code lines</returns>
		public static StringCollection CommandListToStringCollection(List<Command> commands)
		{
			StringCollection sc = new();
			foreach (var command in commands)
			{
				sc.Add(command.ToString());
			}
			return sc;
		}

		/// <summary>
		/// Create command list from file
		/// </summary>
		/// <param name="pathToFile">Path to .RAMCode file</param>
		/// <returns>List of commands</returns>
		public static List<Command> CreateCommandList(string pathToFile)
		{
			List<Command> commands = new();
			using (StreamReader sr = new(pathToFile))
			{
				long i = 0;
				string? line;
				while ((line = sr.ReadLine()) != null)
				{
					commands.Add(new Command(++i, line));
				}
			}
			return commands;
		}

		/// <summary>
		/// Create input tape from file
		/// </summary>
		/// <param name="pathToFile">Path to file</param>
		/// <returns>Queue of strings</returns>
		public static Queue<string> CreateInputTapeFromFile(string pathToFile)
		{
			Queue<string> inputTape = new();
			using (StreamReader sr = new(pathToFile))
			{
				string? line;
				while ((line = sr.ReadLine()) != null)
				{
					StringBuilder sb = new();
					foreach (var c in line)
					{
						if (char.IsDigit(c) || c == '-')
						{
							sb.Append(c);
						}
					}
					string number = sb.ToString();
					// Double check
					if (number.IsNumber())
					{
						inputTape.Enqueue(number);
					}
				}
			}
			return inputTape;
		}

		/// <summary>
		/// Create input tape from given string
		/// </summary>
		/// <param name="line">Input tape string</param>
		/// <returns>Queue of strings</returns>
		public static Queue<string> CreateInputTapeFromString(string? line)
		{
			Queue<string> inputTape = new();
			if (string.IsNullOrEmpty(line))
			{
				return inputTape;
			}
			var arr = line.Split(' ');
			foreach (var entry in arr)
			{
				StringBuilder sb = new();
				foreach (var c in entry)
				{
					if (char.IsDigit(c) || c == '-')
					{
						sb.Append(c);
					}
				}
				string finalNum = sb.ToString();
				if (finalNum.IsNumber())
				{
					inputTape.Enqueue(finalNum);
				}
			}

			return inputTape;
		}

		/// <summary>
		/// Create output tape string from given string queue
		/// </summary>
		/// <param name="tape">Output tape</param>
		/// <returns>String representation of output tape</returns>
		public static string CreateOutputTapeFromQueue(Queue<string> tape)
		{
			string outputTape = string.Empty;
			while (tape != null && tape.Count > 0)
			{
				string number = tape.Dequeue();
				StringBuilder sb = new();
				foreach (var c in number)
				{
					if (char.IsDigit(c) || c == '-')
					{
						sb.Append(c);
					}
				}
				string finalNumber = sb.ToString();
				if (finalNumber.IsNumber())
				{
					outputTape += $"{finalNumber} ";
				}
			}
			return outputTape.Trim();
		}

		public static StringCollection CreateStringCollection(string text, string separator)
		{
			var outCollection = new StringCollection();
			var arr = text.Split(separator);
			foreach (string t in arr)
			{
				if (!string.IsNullOrWhiteSpace(t))
				{
					outCollection.Add(t);
				}
			}
			return outCollection;
		}

		/// <summary>
		/// Create list of commands
		/// </summary>
		/// <param name="lines">Code lines</param>
		/// <returns>List of Command objects.</returns>
		public static List<Command> StringCollectionToCommandList(StringCollection lines)
		{
			List<Command> commands = new();
			long i = 0;
			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				commands.Add(new Command(++i, line));
			}
			return commands;
		}

		public static string StringCollectionToString(StringCollection collection, string join)
		{
			var array = new string[collection.Count];
			collection.CopyTo(array, 0);
			return string.Join(join, array);
		}
	}
}