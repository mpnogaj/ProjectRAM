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
        public CancellationTokenSource? Token { get; set; }

        private ObservableCollection<MemoryRow> _memory;
        public ObservableCollection<MemoryRow> Memory { get => _memory; set => SetProperty(ref _memory, value); }

        private string _header;
        public string Header { get => _header; set => SetProperty(ref _header, value); }

        private string _outputTapeString;
        public string OutputTapeString { get => _outputTapeString; set => SetProperty(ref _outputTapeString, value); }

        private string _inputTapeString;
        public string InputTapeString { get => _inputTapeString; set => SetProperty(ref _inputTapeString, value); }

        private string _programString;
        public string ProgramString { get => _programString; set => SetProperty(ref _programString, value); }

        private bool _programmRunning;
        public bool ProgrammRunning { get => _programmRunning; set => SetProperty(ref _programmRunning, value); }

        private string? _path;
        public string? Path { get => _path; set => SetProperty(ref _path, value); }

        private int _fontSize;
        public int FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

        private bool _simpleEditorUsage = false;
        public bool SimpleEditorUsage { get => _simpleEditorUsage; set => SetProperty(ref _simpleEditorUsage, value); }

        private readonly RelayCommand<HostViewModel> _closePage;
        public RelayCommand<HostViewModel> ClosePage => _closePage;

        private ObservableCollection<ProgramLine> _program = new();
        public ObservableCollection<ProgramLine> Program { get => _program; set => SetProperty(ref _program, value); }

        private readonly ObservableCollection<string> _availableCommands = new ObservableCollection<string>(Constant.AvailableCommands);
        public ObservableCollection<string> AvailableCommands => _availableCommands;

        private ObservableCollection<string> _labels = new ObservableCollection<string>();
        public ObservableCollection<string> Labels { get => _labels; set => SetProperty(ref _labels, value); }

        public HostViewModel(string header)
        {
            _header = header;
            _memory = new ObservableCollection<MemoryRow>();
            _outputTapeString = string.Empty;
            _inputTapeString = string.Empty;
            _programString = string.Empty;
            _programmRunning = false;
            _fontSize = 13;
            _closePage = new(Logic.Logic.ClosePage);
            _program = new ObservableCollection<ProgramLine>
            {
                new ProgramLine
                {
                    Line = 1,
                }
            };
        }

        public void HandleDataGridKeyEvents(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
            {
                int index = ((DataGrid)sender).SelectedIndex;
                if (index != -1)
                {
                    InsertLine(index + 1);
                    ((DataGrid)sender).SelectedIndex++;
                }
                else
                {
                    InsertLine(Program.Count);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                if (!(FocusManager.Instance.Current is TextBox))
                {
                    ProgramLine? programLine = ((DataGrid)sender).SelectedItem as ProgramLine;
                    int selectedIndex = ((DataGrid)sender).SelectedIndex;
                    if (programLine != null)
                    {
                        DeleteLine(programLine);
                        ((DataGrid)sender).SelectedIndex = selectedIndex == 0 ? 0 : selectedIndex - 1;
                    }
                }
            }
        }

        public void HandleEditorSwitch()
        {
            if (SimpleEditorUsage == true)
            {
                ProgramString = ProgramLineToStringConverter.ProgramLinesToString(Program.ToList());
            }
            else
            {
                Task.Run(() => Program = new(ProgramLineToStringConverter.StringToProgramLines(ProgramString))).Wait();
            }
            SimpleEditorUsage = !SimpleEditorUsage;
        }


        private void FixLineNumeration(int startPosition = 0)
        {
            for (int i = startPosition; i < Program.Count; i++)
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

        public void InsertLine(int position)
        {
            Program.Insert(position, new ProgramLine());
            FixLineNumeration(position);
        }

        public void DeleteLine(ProgramLine programLine)
        {
            if (Program.Count != 1)
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
