using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Essentials = ProjectRAM.Editor.Helpers.Essentials;
using Settings = ProjectRAM.Editor.Properties.Settings;

namespace ProjectRAM.Editor.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private const string ObsoletePath = @"!@#";
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
				var files = await Essentials.GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title = Strings.openFile,
					AllowMultiple = true,
					FileTypeFilter = Constant.RamCodeFilter
				});
				await CreateCodePagesFromFiles(files);
			});
			SaveFileAs = new AsyncRelayCommand(async () =>
			{
				await SaveCodeFileAs(Page);
			}, IsFileOpened);
			SaveFile = new AsyncRelayCommand(async () =>
			{
				Debug.Assert(Page != null);
				if (Page.File == null)
				{
					await SaveCodeFileAs(Page);
				}
				else
				{
					if (Page.File.StartsWith(ObsoletePath))
					{
						await Essentials.WriteToFile(Page.File.Substring(ObsoletePath.Length), Page.GetProgramString());
					}
					else
					{
						var file = await Essentials.GetStorageProvider().OpenFileBookmarkAsync(Page.File);
						await Essentials.WriteToFile(file, Page.GetProgramString());
					}
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

				Page!.Errors = errorList;

				switch (errorList.Count)
				{
					case 0 when shouldShowOnSuccess:
						await Essentials.ShowMessageBox(Strings.verification, Strings.everythingAlright);
						break;
					case > 0:
						await Essentials.ShowMessageBox(Strings.verification,
							errorList.Count == 1
								? Strings.foundError
								: string.Format(Strings.foundErrors, errorList.Count));
						break;
				}
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
						ex.LocalizedMessage());
				}
				catch (Exception ex)
				{
					await Essentials.ShowMessageBox(Strings.error, ex.Message);
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
				Debug.Assert(Page != null);
				var res = await Essentials.GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions
				{
					AllowMultiple = false,
					FileTypeFilter = Constant.TextFileFilter,
					Title = Strings.importInputTape
				});

				if (res.Count > 0)
				{
					string? content = await Essentials.ReadFromFile(res[0]);
					if (content != null)
					{
						Page.InputTapeString = content.Replace(",", string.Empty);	
					}
				}
			}, () => IsFileOpened() && !IsProgramRunning());

			Export = new AsyncRelayCommand<string>(async target =>
			{
				Debug.Assert(Page != null);
				string content = target switch
				{
					TargetMemory => MemoryRowToStringConverter.MemoryRowsToString(new List<MemoryRow>(Page.Memory)),
					TargetInputTape => Page.InputTapeString.Replace(" ", ", "),
					TargetOutputTape => Page.OutputTapeString.Replace(" ", ", "),
					_ => throw new InvalidOperationException()
				};

				var res = await Essentials.GetStorageProvider().SaveFilePickerAsync(new FilePickerSaveOptions
				{
					DefaultExtension = ".txt",
					SuggestedFileName = $"{Page.Header}_{target}",
					FileTypeChoices = Constant.TextFileFilter,
					ShowOverwritePrompt = true
				});
				
				await Essentials.WriteToFile(res, content);
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
			if (!OperatingSystem.IsLinux() && !OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS())
			{
				throw new PlatformNotSupportedException();
			}
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

		private async Task CreatePageWithCode(string code, IStorageItem file)
		{
			string header = file.Name;
			var ind = header.LastIndexOf('.');
			if (ind != -1)
			{
				header = header[..ind];
			} 
			Pages.Add(new HostViewModel(header, code, ClosePage)
			{
				File = await file.SaveBookmarkAsync()	
			});
		}

		private void CreatePageWithCode(string code, string file)
		{
			Pages.Add(new HostViewModel(
				Path.GetFileNameWithoutExtension(file), code, ClosePage)
			{
				File = @$"{ObsoletePath}{file}"
			});
		}

		private async Task CreateCodePagesFromFiles(IEnumerable<IStorageFile> files)
		{
			foreach (var file in files)
			{
				var fileContent = await Essentials.ReadFromFile(file);
				if (string.IsNullOrEmpty(fileContent))
				{
					continue;
				}

				await CreatePageWithCode(fileContent, file);
			}
		}
		
		[Obsolete]
		private async Task CreateCodePagesFromFiles(string[] files)
		{
			foreach (var file in files)
			{
				var fileContent = await Essentials.ReadFromFile(file);
				if (string.IsNullOrEmpty(fileContent))
				{
					continue;
				}

				CreatePageWithCode(fileContent, file);
			}
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
			var targetFile = page ?? Page!;
			var file = await Essentials.GetStorageProvider().SaveFilePickerAsync(new FilePickerSaveOptions
			{
				Title = Strings.saveFileAs,
				DefaultExtension = ".RAMCode",
				FileTypeChoices = Constant.RamCodeFilter,
				ShowOverwritePrompt = true,
				SuggestedFileName = targetFile.Header
			});
			
			if (file == null)
			{
				return;
			}

			await Essentials.WriteToFile(file, targetFile.GetProgramString());
			targetFile.File = await file.SaveBookmarkAsync();
			targetFile.Header = file.Name;
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