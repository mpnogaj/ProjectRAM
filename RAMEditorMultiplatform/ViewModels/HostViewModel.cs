using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using RAMEditorMultiplatform.Converters;
using RAMEditorMultiplatform.Helpers;
using RAMEditorMultiplatform.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RAMEditorMultiplatform.ViewModels
{
    public class HostViewModel : ViewModelBase
    {
        public event EventHandler EditorShowed;

        protected virtual void OnEditorShowed(EventArgs e)
        {
            EventHandler handler = EditorShowed;
            handler?.Invoke(this, e);
        }



        public CancellationTokenSource? Token { get; set; }

        private ObservableCollection<MemoryRow> _memory;

        public ObservableCollection<MemoryRow> Memory
        {
            get => _memory;
            set => SetProperty(ref _memory, value);
        }

        private string _header;

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        private string _outputTapeString;

        public string OutputTapeString
        {
            get => _outputTapeString;
            set => SetProperty(ref _outputTapeString, value);
        }

        private string _inputTapeString;

        public string InputTapeString
        {
            get => _inputTapeString;
            set => SetProperty(ref _inputTapeString, value);
        }

        private string _programString;

        public string ProgramString
        {
            get => _programString;
            set => SetProperty(ref _programString, value);
        }

        private bool _programRunning;

        public bool ProgramRunning
        {
            get => _programRunning;
            set => SetProperty(ref _programRunning, value);
        }

        private string? _path;

        public string? Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        private int _fontSize;

        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private bool _simpleEditorUsage;

        public bool SimpleEditorUsage
        {
            get => _simpleEditorUsage;
            set => SetProperty(ref _simpleEditorUsage, value);
        }

        private ObservableCollection<ProgramLine> _program;

        public ObservableCollection<ProgramLine> Program
        {
            get => _program;
            set => SetProperty(ref _program, value);
        }

        private readonly ObservableCollection<string> _availableCommands =
            new ObservableCollection<string>(Constant.AvailableCommands);

        public ObservableCollection<string> AvailableCommands => _availableCommands;

        private ObservableCollection<string> _labels = new ObservableCollection<string>();

        public ObservableCollection<string> Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }

        private TimeSpan _delay;

        public TimeSpan Delay
        {
            get => _delay;
            set => SetProperty(ref _delay, value);
        }

        public HostViewModel(string header)
        {
            _header = header;
            _memory = new ObservableCollection<MemoryRow>();
            _outputTapeString = string.Empty;
            _inputTapeString = string.Empty;
            _programString = string.Empty;
            _programRunning = false;
            _fontSize = 13;
            _simpleEditorUsage = true;
            _program = new ObservableCollection<ProgramLine>
            {
                new()
                {
                    Line = 1,
                }
            };
            _delay = TimeSpan.FromSeconds(1);
        }

        public void HandleDataGridKeyEvents(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Insert:
                    var index = ((DataGrid) sender).SelectedIndex;
                    if (index != -1)
                    {
                        InsertLine(index + 1);
                        ((DataGrid) sender).SelectedIndex++;
                    }
                    else
                    {
                        InsertLine(Program.Count);
                    }

                    e.Handled = true;
                    break;
                case Key.Delete when FocusManager.Instance.Current is TextBox:
                    return;
                case Key.Delete:
                    var programLine = ((DataGrid) sender).SelectedItem as ProgramLine;
                    var selectedIndex = ((DataGrid) sender).SelectedIndex;
                    if (programLine == null) return;
                    DeleteLine(programLine);
                    ((DataGrid) sender).SelectedIndex = selectedIndex == 0 ? 0 : selectedIndex - 1;
                    break;
            }
        }

        public void HandleEditorSwitch()
        {
            if (SimpleEditorUsage)
            { 
                ProgramString = ProgramLineToStringConverter.ProgramLinesToString(Program.ToList());
            }
            else
            {
                var newProgram = ProgramLineToStringConverter.StringToProgramLines(ProgramString);
                if (newProgram.Count < 1)
                {
                    Program = new ObservableCollection<ProgramLine>
                    {
                        new(){Line = 1}
                    };
                }
                else
                {
                    Program = new ObservableCollection<ProgramLine>(newProgram);
                }
                OnEditorShowed(EventArgs.Empty);
            }
            SimpleEditorUsage = !SimpleEditorUsage;
        }
        private void FixLineNumeration(int startPosition = 0)
        {
            for (var i = startPosition; i < Program.Count; i++)
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
            foreach (var line in Program)
            {
                if (!string.IsNullOrWhiteSpace(line.Label))
                {
                    Labels.Add(line.Label);
                }
            }
        }

        private void InsertLine(int position)
        {
            Program.Insert(position, new ProgramLine());
            FixLineNumeration(position);
        }

        private void DeleteLine(ProgramLine programLine)
        {
            if (Program.Count != 1)
            {
                var index = Program.IndexOf(programLine);
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