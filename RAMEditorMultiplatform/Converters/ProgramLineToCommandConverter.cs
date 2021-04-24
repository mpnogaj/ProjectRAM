using Common;
using RAMEditorMultiplatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEditorMultiplatform.Converters
{
    public class ProgramLineToCommandConverter
    {
        public static List<Command> ProgramLinesToCommands(List<ProgramLine> input)
        {
            List<Command> output = new();
            foreach(var line in input)
            {
                output.Add(new Command(command: line.Command, argument: line.Argument, label: line.Label, comment: line.Comment));
            }
            return output;
        }

        public static List<ProgramLine> CommandsToProgramLines(List<Command> input)
        {
            List<ProgramLine> output = new();
            int i = 1;
            foreach(var line in input)
            {
                output.Add(new ProgramLine
                {
                    Command = line.CommandName,
                    Argument = line.Argument,
                    Comment = line.Comment,
                    Label = line.Label,
                    Line = i++
                });
            }
            return output;
        }
    }
}
