using Avalonia.Controls;
using Avalonia.Input;
using RAMEditorMultiplatform.Converters;
using RAMEditorMultiplatform.Helpers;
using RAMEditorMultiplatform.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace RAMEditorMultiplatform.ViewModels
{
    public class HostViewModel : ViewModelBase
    {
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

        private string _textEditorProgram;

        public string TextEditorProgram
        {
            get => _textEditorProgram;
            set => SetProperty(ref _textEditorProgram, value);
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

        private ObservableCollection<ProgramLine> _simpleEditorProgram;

        public ObservableCollection<ProgramLine> SimpleEditorProgram
        {
            get => _simpleEditorProgram;
            set => SetProperty(ref _simpleEditorProgram, value);
        }

        private readonly ObservableCollection<string> _availableCommands =
            new(Constant.AvailableCommands);

        public ObservableCollection<string> AvailableCommands => _availableCommands;

        private ObservableCollection<string> _labels = new();

        public ObservableCollection<string> Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }

        private ObservableCollection<ErrorLine> _errors = new();

        public ObservableCollection<ErrorLine> Errors
        {
            get => _errors;
            set => SetProperty(ref _errors, value);
        }

        public HostViewModel(string header)
        {
            _header = header;
            _memory = new ObservableCollection<MemoryRow>();
            _outputTapeString = string.Empty;
            _inputTapeString = string.Empty;
            _textEditorProgram = string.Empty;
            _programRunning = false;
            _fontSize = 13;
            _simpleEditorUsage = true;
            _simpleEditorProgram = new ObservableCollection<ProgramLine>
            {
                new()
                {
                    Line = 1,
                }
            };
        }

        public HostViewModel(string header, string code) : this(header)
        {
            _textEditorProgram = code;
            foreach (var line in code.Split('\n'))
            {
                _simpleEditorProgram.Add(new ProgramLine(line));
            }
            // Delete first line
            if(_simpleEditorProgram.Count > 1) _simpleEditorProgram.RemoveAt(0);
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
                        InsertLine(SimpleEditorProgram.Count);
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
                TextEditorProgram = ProgramLineToStringConverter.ProgramLinesToString(SimpleEditorProgram.ToList());
            }
            else
            {
                var newProgram = ProgramLineToStringConverter.StringToProgramLines(TextEditorProgram);
                if (newProgram.Count < 1)
                {
                    SimpleEditorProgram = new ObservableCollection<ProgramLine>
                    {
                        new(){Line = 1}
                    };
                }
                else
                {
                    SimpleEditorProgram = new ObservableCollection<ProgramLine>(newProgram);
                }
            }
            SimpleEditorUsage = !SimpleEditorUsage;
        }
        private void FixLineNumeration(int startPosition = 0)
        {
            for (var i = startPosition; i < SimpleEditorProgram.Count; i++)
            {
                ProgramLine currLine = SimpleEditorProgram[i];
                SimpleEditorProgram[i] = new ProgramLine
                {
                    Line = i + 1,
                    Label = currLine.Label,
                    Command = currLine.Command,
                    Argument = currLine.Argument,
                    Comment = currLine.Comment
                };
            }
        }

        public string GetProgramString()
        {
            if (SimpleEditorUsage)
            {
                return ProgramLineToStringConverter.ProgramLinesToString(SimpleEditorProgram.ToList());
            }
            else
            {
                return TextEditorProgram;
            }
        }

        public void UpdateLabels()
        {
            Labels.Clear();
            foreach (var line in SimpleEditorProgram)
            {
                if (!string.IsNullOrWhiteSpace(line.Label))
                {
                    Labels.Add(line.Label);
                }
            }
        }

        private void InsertLine(int position)
        {
            SimpleEditorProgram.Insert(position, new ProgramLine());
            FixLineNumeration(position);
        }

        private void DeleteLine(ProgramLine programLine)
        {
            if (SimpleEditorProgram.Count != 1)
            {
                var index = SimpleEditorProgram.IndexOf(programLine);
                SimpleEditorProgram.RemoveAt(index);
                FixLineNumeration(index);
            }
            else
            {
                SimpleEditorProgram[0] = new ProgramLine();
                FixLineNumeration();
            }
        }
    }
}