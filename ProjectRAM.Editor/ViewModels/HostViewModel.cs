using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using ProjectRAM.Editor.ViewModels.Commands;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.Models;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Converters;

namespace ProjectRAM.Editor.ViewModels
{
	public class HostViewModel : ViewModelBase
	{
		public IEnumerable Items => new List<string>
		{
			"read",
			"write",
			"load",
			"store",
			"add",
			"sub",
			"mult",
			"div",
			"jump",
			"jzero",
			"jgtz"
		};

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

		private bool _isBlocked;

		public bool IsBlocked
		{
			get => _isBlocked;
			set => SetProperty(ref _isBlocked, value);
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

		private readonly RelayCommand<HostViewModel> _closePage;

		public RelayCommand<HostViewModel> ClosePage
		{
			get => _closePage;
		}

		public HostViewModel(string header, Action<HostViewModel> closePageAction)
		{
			_closePage = new RelayCommand<HostViewModel>(closePageAction, () => true);
			_header = header;
			_memory = new ObservableCollection<MemoryRow>();
			_outputTapeString = string.Empty;
			_inputTapeString = string.Empty;
			_textEditorProgram = string.Empty;
			_programRunning = false;
			_fontSize = 13;
			_simpleEditorUsage = !Settings.UseTextEditor;
			_simpleEditorProgram = new ObservableCollection<ProgramLine>
			{
				new()
				{
					Line = 1,
				}
			};
		}

		public HostViewModel(string header, string code, Action<HostViewModel> closePageAction) : this(header, closePageAction)
		{
			_textEditorProgram = code;
			foreach (var line in code.Split(Environment.NewLine))
			{
				_simpleEditorProgram.Add(new ProgramLine(line));
			}
			// Delete first line
			if (_simpleEditorProgram.Count > 1) _simpleEditorProgram.RemoveAt(0);
			FixLineNumeration();
		}

		public void HandleDataGridKeyEvents(object sender, KeyEventArgs e)
		{
			var focusManager = FocusManager.Instance;
			var dataGrid = (DataGrid)sender;
			switch (e.Key)
			{
				case Key.Insert:
					var index = ((DataGrid)sender).SelectedIndex;
					if (index != -1)
					{
						InsertLine(index + 1);
						((DataGrid)sender).SelectedIndex++;
					}
					else
					{
						InsertLine(SimpleEditorProgram.Count);
					}

					e.Handled = true;
					break;
				case Key.Delete when focusManager?.Current is TextBox:
					return;
				case Key.Delete:
					var programLine = dataGrid.SelectedItem as ProgramLine;
					var selectedIndex = dataGrid.SelectedIndex;
					if (programLine == null)
					{
						return;
					}
					DeleteLine(programLine);
					//selectedIndex == 0 ? 0 : selectedIndex - 1;
					dataGrid.SelectedIndex = Math.Max(selectedIndex - 1, 0);
					break;
				case Key.Escape when focusManager?.Current is TextBox textBox:
					dataGrid.Focus();
					break;
				default:
					if (e.Key.IsAlphaNumeric(true) || e.Key is Key.OemPlus or Key.OemMinus)
					{
						if (focusManager?.Current is DataGrid)
						{
							var selectedCell = dataGrid.GetSelectedCell();

							if (selectedCell?.Content is TextBox or AutoCompleteBox)
							{
								((IControl)selectedCell.Content).Focus();
							}
						}
					}
					break;
			}
		}

		public void HandleEditorSwitch()
		{
			if (SimpleEditorUsage)
			{
				var programList = ProgramLineToStringConverter.ProgramLinesToString(SimpleEditorProgram.ToList()); 
				TextEditorProgram = string.Join(Environment.NewLine, programList);
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

		public string[] GetProgramString()
		{
			return SimpleEditorUsage
				? ProgramLineToStringConverter.ProgramLinesToString(SimpleEditorProgram.ToList())
				: TextEditorProgram.Replace("\r", string.Empty).Split('\n');
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