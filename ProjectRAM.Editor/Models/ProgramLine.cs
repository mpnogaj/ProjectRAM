﻿using System;
using System.Text.RegularExpressions;

namespace ProjectRAM.Editor.Models
{
	public class ProgramLine
	{
		public int Line { get; set; } = 0;
		public string Label { get; set; } = string.Empty;
		public string Command { get; set; } = string.Empty;
		public string Argument { get; set; } = string.Empty;
		public string Comment { get; set; } = string.Empty;
		public bool Breakpoint { get; set; } = false;

		public ProgramLine(string line)
		{
			try
			{
				string lineWithoutComment = line;
				int commentPos = line.IndexOf('#');
				if (commentPos >= 0)
				{
					lineWithoutComment = line.Substring(0, commentPos - 1);
					Comment = line.Substring(commentPos + 1).Trim();
				}

				string lineWithoutCommentAndLabel = string.Empty;
				int labelPos = lineWithoutComment.IndexOf(':');
				if (labelPos >= 0)
				{
					Label = Regex.Replace(lineWithoutComment.Substring(0, labelPos).Trim(), @"\s+", string.Empty);
					if (labelPos != lineWithoutComment.Length - 1)
					{
						lineWithoutCommentAndLabel = lineWithoutComment.Substring(labelPos + 1);
					}
				}
				else
				{
					lineWithoutCommentAndLabel = lineWithoutComment;
				}

				lineWithoutCommentAndLabel = Regex.Replace(lineWithoutCommentAndLabel, @"\s+", " ");
				var arr = lineWithoutCommentAndLabel.Trim().Split(' ');
				Command = arr.Length > 0 ? arr[0] : string.Empty;
				Argument = arr.Length > 1 ? arr[1] : string.Empty;
				Breakpoint = line.Contains(";;");
			}
			catch
			{
				Label = string.Empty;
				Command = string.Empty;
				Argument = string.Empty;
				Comment = string.Empty;
				Breakpoint = false;
			}
		}

		public ProgramLine() { }

		public override string ToString()
		{
			string output = "";
			if (!string.IsNullOrWhiteSpace(Label))
			{
				output = $"{Label}: ";
			}

			if (!string.IsNullOrWhiteSpace(Command))
			{
				output += $"{Command} ";
			}

			if (!string.IsNullOrWhiteSpace(Argument))
			{
				output += $"{Argument} ";
			}

			if (!string.IsNullOrWhiteSpace(Comment))
			{
				output += $"#{Comment} ";
			}

			if (Breakpoint)
			{
				output += ";;";
			}

			return output.TrimEnd();
		}

		public override bool Equals(object? obj)
		{
			if (obj == null || obj!.GetType() != GetType()) return false;

			ProgramLine lhs = (ProgramLine)obj;
			return GetHashCode() == lhs.GetHashCode();
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Line, Label, Command, Argument, Comment);
		}
	}
}
