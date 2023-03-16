using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectRAM.Core.Converters;

public static class FileToTapeConverter
{
	public static Queue<string> ConvertFileToInputTape(string filePath)
	{
		using var sr = new StreamReader(filePath);
		var content = sr.ReadToEnd();
		content = Regex.Replace(content, @"\s+", " ");
		return new Queue<string>(content.Split(' '));
	}

	public static string ConvertOutputTapeToFile(string targetFilePath, string targetFileName,
		Queue<string> outputTape)
	{
		var stringBuilder = new StringBuilder();
		while (outputTape.Count > 0)
		{
			stringBuilder.Append(outputTape.Dequeue());
			stringBuilder.Append(Environment.NewLine);
		}
		return stringBuilder.ToString();
	}
}