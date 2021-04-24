using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.Helpers;
using RAMEditorMultiplatform.Models;

namespace RAMEditorMultiplatform.ViewModels
{
    public class SimpleEditorViewModel : ViewModelBase
    {
        private ObservableCollection<ProgramLine> _program = new();
        public ObservableCollection<ProgramLine> Program { get => _program; set { SetProperty(ref _program, value); UpdateLabels(); } }

        private ObservableCollection<string> _availableCommands = new ObservableCollection<string>(Constant.AvailableCommands);
        public ObservableCollection<string> AvailableCommands { get => _availableCommands; }

        private ObservableCollection<string> _labels = new ObservableCollection<string>();
        public ObservableCollection<string> Labels { get => _labels; set { SetProperty(ref _labels, value); } }

        public SimpleEditorViewModel()
        {
            Program = new ObservableCollection<ProgramLine>
            {
                new ProgramLine
                {
                    Line = 1,
                    Label = "kjop",
                    Command = "add",
                    Argument = "13",
                    Comment = "das"
                },
                new ProgramLine
                {
                    Line = 2,
                    Label = "dtrdtr",
                    Command = "add",
                    Argument = "1dtrdtrd3",
                    Comment = "drtdrt"
                }
            };
        }

        private void FixLineNumeration(int startPosition = 0)
        {
            for(int i = startPosition; i < Program.Count; i++)
            {
                ProgramLine currLine = Program[i];
                Program[i] = new ProgramLine
                {
                    Line = i + 1,
                    Label = currLine.Label,
                    Command = currLine.Command,
                    Argument = currLine.Argument,
                    Comment = currLine.Comment
                };
            }
        }

        public void UpdateLabels()
        {
            Labels.Clear();
            foreach(var line in Program)
            {
                if(!string.IsNullOrWhiteSpace(line.Label))
                {
                    Labels.Add(line.Label);
                }
            }
        }

        public void InsertLine(int position)
        {
            Program.Insert(position, new ProgramLine());
            FixLineNumeration(position);
        }

        public void DeleteLine(ProgramLine programLine)
        {
            if(this.Program.Count != 1)
            {
                int index = Program.IndexOf(programLine);
                Program.RemoveAt(index);
                FixLineNumeration(index);
            }
            else
            {
                Program[0] = new ProgramLine();
                FixLineNumeration();
            }
        }
    }
}
