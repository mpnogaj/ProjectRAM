using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using MessageBox.Avalonia.Enums;
using ProjectRAM.Core;
using ProjectRAM.Core.Models;
using ProjectRAM.Editor.Converters;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Models;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.ViewModels.Commands;
using ProjectRAM.Editor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectRAM.Core.Properties;
using Essentials = ProjectRAM.Editor.Helpers.Essentials;
using Settings = ProjectRAM.Editor.Properties.Settings;

namespace ProjectRAM.Editor.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private const string TargetMemory = "memory";
		private const string TargetInputTape = "inputTape";
		private const string TargetOutputTape = "outputTape";
		private const int StringBuilderMaxCapacity = 1 << 21;
		private HostViewModel? _page;
		private ObservableCollection<HostViewModel> _pages;

		public MainWindowViewModel()
		{
			CloseCurrent = new RelayCommand(() => ClosePage(Page!), IsFileOpened);

			_pages = new ObservableCollection<HostViewModel>();
			_page = null;
			AddPageCommand = new RelayCommand(() => CreateEmptyPage());
			OpenFile = new AsyncRelayCommand(async () =>
			{
				var ofd = new OpenFileDialog
				{
					Title = Strings.openFile,
					AllowMultiple = true,
					Filters = Constant.RamcodeFilter
				};
				var files = await ofd.ShowAsync(Essentials.GetAppDesktopLifetime().MainWindow);
				if (files == null) return;

				CreateCodePagesFromFiles(files);
			});
			SaveFileAs = new AsyncRelayCommand(async () => await SaveCodeFileAs(Page!), IsFileOpened);
			SaveFile = new AsyncRelayCommand(async () =>
			{
				if (string.IsNullOrEmpty(Page!.Path))
				{
					await SaveCodeFileAs(Page!);
				}
				else
				{
					Essentials.WriteToFile(Page!.Path, Page!.GetProgramString());
				}
			}, IsFileOpened);
			CloseProgram = new RelayCommand(Essentials.Exit, () => true);

			IncreaseFontSize = new RelayCommand(() => Settings.CurrentStyle.ChangeFontSizes(1.0), () => true);
			DecreaseFontSize = new RelayCommand(() => Settings.CurrentStyle.ChangeFontSizes(-1.0), () => true);

			Verify = new AsyncRelayCommand<bool>(async shouldShowOnSuccess =>
			{
				var exceptions = await GetAllErrors(Page!);
				var errorList = new ObservableCollection<ErrorLine>();
				foreach (var exception in exceptions)
				{
					errorList.Add(new ErrorLine
					{
						Line = exception.Line,
						Message = exception.Message
					});
				}

				if (errorList.Count == 0)
				{
					if (shouldShowOnSuccess)
						await Essentials.ShowMessageBox(Strings.verification, Strings.everythingAlright, Icon.Info);
					return;
				}

				Page!.Errors = errorList;

				await Essentials.ShowMessageBox(Strings.verification,
					errorList.Count == 1 ? Strings.foundError : string.Format(Strings.foundErrors, errorList.Count),
					Icon.Warning);
			}, () => !IsProgramRunning() && IsFileOpened());

			RunProgram = new AsyncRelayCommand(async () =>
			{
				await Verify.ExecuteAsync(false);
				if (Page!.Errors.Count > 0 && Page!.Errors[0].Line != -1)
					return;
				TogglePageStatus();
				Page!.Token = new CancellationTokenSource();
				try
				{
					Essentials.SetCursor(StandardCursorType.Wait);
					Page.ProgramRunning = true;
					await Task.Run(() => { CreateAndRunProgram(Page.Token.Token); });
				}
				catch (OperationCanceledException)
				{
					/*Ignore*/
				}
				catch (RamInterpreterException ex)
				{
					await Essentials.ShowMessageBox(Strings.error,
						ex.LocalizedMessage(), Icon.Error);
				}
				catch (Exception ex)
				{
					await Essentials.ShowMessageBox(Strings.error, ex.Message, Icon.Error);
					Page!.Memory =
						MemoryDictionaryToMemoryRowConverter.MemoryDictionaryToMemoryRows(new Dictionary<string, string>()
						{
							{ "0", "?" }
						});
				}
				finally
				{
					Page.ProgramRunning = false;
					Essentials.SetCursor(StandardCursorType.Arrow);

					TogglePageStatus();
				}
			}, () => IsFileOpened() && !IsProgramRunning());

			StopProgram = new RelayCommand(() =>
			{
				Page!.Token!.Cancel();
				TogglePageStatus();
			}, () => IsFileOpened() && IsProgramRunning());

			SwitchEditors = new RelayCommand(() => { Page!.HandleEditorSwitch(); },
				() => IsFileOpened() && !IsProgramRunning());

			Clear = new RelayCommand<string>(target =>
			{
				switch (target)
				{
					case TargetMemory:
						Page!.Memory = new ObservableCollection<MemoryRow>();
						break;
					case TargetInputTape:
						Page!.InputTapeString = string.Empty;
						break;
					case TargetOutputTape:
						Page!.OutputTapeString = string.Empty;
						break;
				}
			}, () => IsFileOpened() && !IsProgramRunning());

			Import = new AsyncRelayCommand(async () =>
			{
				OpenFileDialog ofd = new()
				{
					AllowMultiple = false,
					Title = Strings.importInputTape,
					Filters = Constant.TextFileFilter
				};

				var res = await ofd.ShowAsync(Essentials.GetAppDesktopLifetime().MainWindow);
				if (res == null) return;
				if (res.Length > 0)
				{
					string content = Essentials.ReadFromFile(res[0]);
					Page!.InputTapeString = content.Replace(",", string.Empty);
				}
			}, () => IsFileOpened() && !IsProgramRunning());

			Export = new AsyncRelayCommand<string>(async target =>
			{
				string content;
				switch (target)
				{
					case TargetMemory:
						content = MemoryRowToStringConverter.MemoryRowsToString(new List<MemoryRow>(Page!.Memory));
						break;
					case TargetInputTape:
						content = Page!.InputTapeString.Replace(" ", ", ");
						break;
					case TargetOutputTape:
						content = Page!.OutputTapeString.Replace(" ", ", ");
						break;
					default:
						return;
				}

				SaveFileDialog sfd = new()
				{
					Title = Strings.exportToFile,
					InitialFileName = $"{Page!.Header}_{target}",
					Filters = Constant.TextFileFilter
				};

				var res = await sfd.ShowAsync(Essentials.GetAppDesktopLifetime().MainWindow);
				if (!string.IsNullOrEmpty(res)) Essentials.WriteToFile(res, content);
			}, () => IsFileOpened() && !IsProgramRunning());

			OpenSettings = new RelayCommand(() =>
			{
				Options optionsWindow = new();
				optionsWindow.ShowDialog(Essentials.GetAppDesktopLifetime().MainWindow);
			}, () => true);

			OpenStyleEditor = new RelayCommand(() =>
			{
				StyleEditor styleEditor = new();
				styleEditor.ShowDialog(Essentials.GetAppDesktopLifetime().MainWindow);
			}, () => true);

			CreateEmptyPage();
		}

		public ObservableCollection<HostViewModel> Pages
		{
			get => _pages;
			set => SetProperty(ref _pages, value);
		}

		public HostViewModel? Page
		{
			get => _page;
			set => SetProperty(ref _page, value);
		}

		public RelayCommand AddPageCommand { get; }

		public AsyncRelayCommand OpenFile { get; }

		public AsyncRelayCommand SaveFileAs { get; }

		public AsyncRelayCommand SaveFile { get; }

		public AsyncRelayCommand RunProgram { get; }

		public RelayCommand StopProgram { get; }

		public RelayCommand CloseProgram { get; }

		public RelayCommand IncreaseFontSize { get; }

		public RelayCommand DecreaseFontSize { get; }

		public RelayCommand SwitchEditors { get; }

		public RelayCommand<string> Clear { get; }

		public AsyncRelayCommand<string> Export { get; }

		public AsyncRelayCommand Import { get; }

		public AsyncRelayCommand<bool> Verify { get; }

		public RelayCommand OpenSettings { get; }

		public RelayCommand CloseCurrent { get; }

		public RelayCommand OpenStyleEditor { get; }

		private bool IsFileOpened()
		{
			return Page != null;
		}

		private bool IsProgramRunning()
		{
			return Page is { ProgramRunning: true };
		}

		public static void FileOver(object? sender, DragEventArgs e)
		{
			var dropEnabled = false;
			if (e.Data.GetDataFormats().Any(format => format == DataFormats.FileNames))
				if (e.Data.GetFileNames() != null &&
					e.Data.GetFileNames()!.Any(name => Path.GetExtension(name) == Constant.RamExtension))
					dropEnabled = true;

			if (dropEnabled) return;
			e.DragEffects = DragDropEffects.None;
			e.Handled = true;
		}

		public void FileDropped(object? sender, DragEventArgs e)
		{
			string[] files = e.Data.GetFileNames()!
				.Where(fileName => Path.GetExtension(fileName) == Constant.RamExtension)
				.ToArray();
			CreateCodePagesFromFiles(files);
		}

		private void ClosePage(HostViewModel page)
		{
			// Thread safe
			Dispatcher.UIThread.InvokeAsync(() => Pages.Remove(page));
		}

		private void CreateEmptyPage(string header = Constant.DefaultHeader)
		{
			Pages.Add(new HostViewModel(header, ClosePage));
		}

		private void CreatePageWithCode(string code, string path, string header = Constant.DefaultHeader)
		{
			Pages.Add(new HostViewModel(header, code, ClosePage)
			{
				Path = path
			});
		}

		private void CreateCodePagesFromFiles(IEnumerable<string> files)
		{
			foreach (string file in files)
				if (!string.IsNullOrWhiteSpace(file))
					CreatePageWithCode(Essentials.ReadFromFile(file), file, Path.GetFileNameWithoutExtension(file));
		}

		private static async Task<IEnumerable<RamInterpreterException>> GetAllErrors(HostViewModel page)
		{
			var programString = page.GetProgramString();
			var stringCollection = Factory.CreateStringCollection(programString, Environment.NewLine);
			var program = Factory.StringCollectionToCommandList(stringCollection);
			return await Task.Run(() => Validator.ValidateProgram(program));
		}

		private async Task SaveCodeFileAs(HostViewModel? page)
		{
			var file = page ?? Page!;
			var sfd = new SaveFileDialog
			{
				InitialFileName = file.Header,
				Title = Strings.saveFile,
				Filters = Constant.RamcodeFilter
			};

			var res = await sfd.ShowAsync(Essentials.GetAppDesktopLifetime().MainWindow);
			if (!string.IsNullOrEmpty(res))
			{
				file.Path = res;
				Essentials.WriteToFile(res, file.GetProgramString());
			}

			file.Header = Path.GetFileNameWithoutExtension(res)!;
		}

		private void CreateAndRunProgram(CancellationToken token)
		{
			string input = Page!.InputTapeString;
			string program = Page!.TextEditorProgram;
			List<Command> commands;
			if (Page!.SimpleEditorUsage)
			{
				commands = ProgramLineToCommandConverter.ProgramLinesToCommands(Page!.SimpleEditorProgram.ToList());
			}
			else
			{
				var sc = new StringCollection();
				sc.AddRange(program.Split(Environment.NewLine));
				commands = Factory.StringCollectionToCommandList(sc);
			}

			var inputTape = Factory.CreateInputTapeFromString(input);
			var interpreter = new Interpreter(commands);
			var sb = new StringBuilder();
			interpreter.ReadFromInputTape += (_, eventArgs) =>
			{
				eventArgs.Input = inputTape.Count > 0
					? inputTape.Dequeue()
					: null;
			};
			interpreter.WriteToOutputTape += (_, eventArgs) =>
			{
				sb.Append($" {eventArgs.Output}");
				if(sb.Length >= StringBuilderMaxCapacity)
				{
					throw new OutOfMemoryException(Strings.outputTapeOverflow);
				}
			};
			interpreter.ProgramFinished += (_, _) =>
			{
				Page!.ProgramRunning = false;
			};
			token.ThrowIfCancellationRequested();
			var memory = interpreter.RunCommands(token);
			token.ThrowIfCancellationRequested();
			Page!.OutputTapeString = sb.ToString().TrimStart();
			Page!.Memory = MemoryDictionaryToMemoryRowConverter.MemoryDictionaryToMemoryRows(memory);
		}

		private void TogglePageStatus()
		{
			foreach (var page in Pages)
				if (page != Page)
					page.IsBlocked = !page.IsBlocked;
		}
	}
}