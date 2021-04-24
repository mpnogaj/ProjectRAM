using System.Text.RegularExpressions;
using RAMEditorMultiplatform.Helpers;

namespace RAMEditorMultiplatform.Models
{
    public class ProgramLine
    {
        public int Line { get; set; } = 0;
        public string Label { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string Argument { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;

        public ProgramLine(string line)
        {
            line = line.Trim();
            line = Regex.Replace(line, @"\s+", " ");
            var arr = line.Split(' ');
            int i = 0;
            try
            {
                if (arr[i].EndsWith(':'))
                {
                    string sub = arr[i].Substring(0, arr[i].Length - 1);
                    Label = string.IsNullOrEmpty(sub) ? string.Empty : sub;
                    i++;
                }

                if (Constant.AvailableCommands.Contains(arr[i].ToLower()))
                {
                    Command = arr[i].ToLower();
                    i++;
                }

                if (!arr[i].StartsWith('#'))
                {
                    Argument = arr[i];
                    i++;
                }

                if (i < arr.Length)
                {
                    while (i < arr.Length - 1 && !arr[i].StartsWith('#')) i++;

                    if (arr[i].StartsWith('#'))
                    {
                        string sub = arr[i].Substring(1);
                        Comment = string.IsNullOrEmpty(sub) ? string.Empty : sub;
                        i++;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public ProgramLine() { }

        public override string ToString()
        {
            string output = "";
            if (!string.IsNullOrWhiteSpace(this.Label))
            {
                output = $"{this.Label}: ";
            }

            if (!string.IsNullOrWhiteSpace(this.Command))
            {
                output += $"{this.Command} ";
            }

            if (!string.IsNullOrWhiteSpace(this.Argument))
            {
                output += $"{this.Argument} ";
            }

            if (!string.IsNullOrWhiteSpace(this.Comment))
            {
                output += $"#{this.Comment}";
            }

            return output.TrimEnd();
        }
    }
}
