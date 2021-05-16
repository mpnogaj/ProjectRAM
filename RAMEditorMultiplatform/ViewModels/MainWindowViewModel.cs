using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using RAMEditorMultiplatform.Helpers;
using System.Threading;
using Avalonia.Input;
using System.IO;
using Avalonia.Controls;
using Common;
using RAMEditorMultiplatform.Converters;
using RAMEditorMultiplatform.Models;
using MessageBox.Avalonia;

namespace RAMEditorMultiplatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance { get; private set; }
        
        private ObservableCollection<HostViewModel> _pages;

        public ObservableCollection<HostViewModel> Pages
        {
            get => _pages;
            set => SetProperty(ref _pages, value);
        }

        private HostViewModel? _page;

        public HostViewModel? Page
        {
            get => _page;
            set => SetProperty(ref _page, value);
        }

        private readonly RelayCommand _addPage;

        public RelayCommand AddPageCommand
        {
            get => _addPage;
        }

        private readonly AsyncRelayCommand _openFile;

        public AsyncRelayCommand OpenFile
        {
            get => _openFile;
        }

        private readonly RelayCommand _saveFileAs;

        public RelayCommand SaveFileAs
        {
            get => _saveFileAs;
        }

        private readonly RelayCommand _saveFile;

        public RelayCommand SaveFile
        {
            get => _saveFile;
        }

        private readonly AsyncRelayCommand _runProgram;

        public AsyncRelayCommand RunProgram
        {
            get => _runProgram;
        }

        private readonly RelayCommand _stopProgram;

        public RelayCommand StopProgram
        {
            get => _stopProgram;
        }

        private readonly RelayCommand _closeProgram;

        public RelayCommand CloseProgram
        {
            get => _closeProgram;
        }

        private readonly RelayCommand _increaseFontSize;

        public RelayCommand IncreaseFontSize
        {
            get => _increaseFontSize;
        }

        private readonly RelayCommand _decreaseFontSize;

        public RelayCommand DecreaseFontSize
        {
            get => _decreaseFontSize;
        }

        private readonly RelayCommand _switchEditors;

        public RelayCommand SwitchEditors
        {
            get => _switchEditors;
        }

        private readonly RelayCommand<string> _clear;

        public RelayCommand<string> Clear
        {
            get => _clear;
        }

        private readonly AsyncRelayCommand<string> _export;

        public AsyncRelayCommand<string> Export
        {
            get => _export;
        }

        private readonly AsyncRelayCommand _import;

        public AsyncRelayCommand Import
        {
            get => _import;
        }

        private readonly RelayCommand _validate;
        
        public RelayCommand Validate
        {
            get => _validate;
        }

        /*private readonly RelayCommand<HostViewModel> _closePage;

        public RelayCommand<HostViewModel> ClosePage
        {
            get => _closePage;
        }*/

        public MainWindowViewModel()
        {
            Instance = this;
            _pages = new ObservableCollection<HostViewModel>();
            _page = null;
            _addPage = new(() => CreateEmptyPage());
            _openFile = new(async () =>
            {
                var ofd = new OpenFileDialog
                {
                    Title = "Open file",
                    AllowMultiple = true,
                    Filters = Constant.RamcodeFilter
                };
                var files = await ofd.ShowAsync(Essentials.GetAppInstance().MainWindow);
                if (files == null)
                {
                    return;
                }

                CreateCodePagesFromFiles(files);
            });
            _saveFileAs = new(() => SaveCodeFileAs(Page!), () => IsFileOpened());
            _saveFile = new(() =>
            {
                if (string.IsNullOrEmpty(Page!.Path))
                {
                    SaveCodeFileAs(Page!);
                }
                else
                {
                    Essentials.WriteToFile(Page!.Path, Page!.GetProgramString());
                }
            }, () => IsFileOpened());
            _closeProgram = new(Essentials.Exit, () => true);

            _increaseFontSize = new(() => Page!.FontSize += 1, IsFileOpened);
            _decreaseFontSize = new(() => Page!.FontSize -= 1, () => IsFileOpened() && Page!.FontSize > 1);

            _runProgram = new(async () =>
            {
                Validate.Execute(null);
                if (Page!.Errors.Count > 0 && Page!.Errors[0].Line != -1)
                    return;
                
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
                    var msgb = MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message, icon: MessageBox.Avalonia.Enums.Icon.Warning, windowStartupLocation: WindowStartupLocation.CenterOwner);
                    await msgb.ShowDialog(Essentials.GetAppInstance().MainWindow);
                }
                finally
                {
                    Page.ProgramRunning = false;
                    Essentials.SetCursor(StandardCursorType.Arrow);
                    Page!.Memory = MemoryDictionaryToMemoryRowConverter.MemoryDictionaryToMemoryRows(Interpreter.Memory);
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _stopProgram = new(() => { Page!.Token!.Cancel(); }, () => IsFileOpened() && IsProgramRunning());

            _switchEditors = new(() => { Page!.HandleEditorSwitch(); }, () => IsFileOpened() && !IsProgramRunning());

            _clear = new((target) =>
            {
                switch (target)
                {
                    case "memory":
                        Page!.Memory = new();
                        break;
                    case "inputTape":
                        Page!.InputTapeString = string.Empty;
                        break;
                    case "outputTape":
                        Page!.OutputTapeString = string.Empty;
                        break;
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _import = new(async () =>
            {
                OpenFileDialog ofd = new()
                {
                    AllowMultiple = false,
                    Title = "Open memory file",
                    Filters = Constant.TextFileFilter
                };

                var res = await ofd.ShowAsync(Essentials.GetAppInstance().MainWindow);
                if (res.Length > 0)
                {
                    string content = Essentials.ReadFromFile(res[0]);
                    Page!.InputTapeString = content.Replace(",", "");
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _export = new(async (target) =>
            {
                string content;
                switch (target)
                {
                    case "memory":
                        content = MemoryRowToStringConverter.MemoryRowsToString(new(Page!.Memory));
                        break;
                    case "inputTape":
                        content = Page!.InputTapeString.Replace(" ", ", ");
                        break;
                    case "outputTape":
                        content = Page!.OutputTapeString.Replace(" ", ", ");
                        break;
                    default:
                        return;
                }

                SaveFileDialog sfd = new()
                {
                    Title = "Save file",
                    InitialFileName = $"{Page!.Header}_{target}",
                    Filters = Constant.TextFileFilter
                };

                var res = await sfd.ShowAsync(Essentials.GetAppInstance().MainWindow);
                if (!string.IsNullOrEmpty(res))
                {
                    Essentials.WriteToFile(res, content);
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _validate = new(() =>
            {
                var exceptions = GetAllErrors(Page!);
                var errorList = new ObservableCollection<ErrorLine>();
                foreach (var exception in exceptions)
                {
                    errorList.Add(new ErrorLine{Line = exception.Line, Message = exception.LocalizedMessage("pl-PL")});
                }

                if (errorList.Count == 0)
                {
                    errorList.Add(new ErrorLine{Message = "Everything is alright"});
                }
                Page!.Errors = errorList;
            }, () => !IsProgramRunning() && IsFileOpened());

            /*_closePage = new((page) =>
            {
                Pages.Remove(page);
            }, () => true);*/
            
            CreateEmptyPage();
        }

        private bool IsFileOpened()
        {
            return Page != null;
        }

        private bool IsProgramRunning()
        {
            return Page != null && Page.ProgramRunning;
        }

        public void FileOver(object? sender, DragEventArgs e)
        {
            bool dropEnabled = false;
            if (e.Data.GetDataFormats().Any(format => format == DataFormats.FileNames))
            {
                if (e.Data.GetFileNames() != null &&
                    e.Data.GetFileNames()!.Any(name => Path.GetExtension(name) == ".RAMCode"))
                {
                    dropEnabled = true;
                }
            }

            if (dropEnabled) return;
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }

        public void FileDropped(object? sender, DragEventArgs e)
        {
            string[] files = e.Data.GetFileNames()!.Where(fileName => Path.GetExtension(fileName) == ".RAMCode")
                .ToArray();
            CreateCodePagesFromFiles(files);
        }

        private void ClosePage(HostViewModel page)
        {
            Pages!.Remove(page);
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

        private void CreateCodePagesFromFiles(string[] files)
        {
            foreach (string file in files)
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    CreatePageWithCode(Essentials.ReadFromFile(file), file, Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        private List<RamInterpreterException> GetAllErrors(HostViewModel page)
        {
            var file = page ?? Page!;
            var programString = file.GetProgramString();
            var stringCollection = Creator.CreateStringCollection(programString, "\n");
            var program = Creator.CreateCommandList(stringCollection);
            return Task.Run(() => Validator.ValidateProgram(program)).Result;
        }

        private async void SaveCodeFileAs(HostViewModel? page)
        {
            var file = page ?? Page!;
            var sfd = new SaveFileDialog
            {
                InitialFileName = file.Header,
                Title = "Save file",
                Filters = Constant.RamcodeFilter
            };

            var res = await sfd.ShowAsync(Essentials.GetAppInstance().MainWindow);
            if (!string.IsNullOrEmpty(res))
            {
                file.Path = res;
                Essentials.WriteToFile(res, file.GetProgramString());
            }

            file.Header = Path.GetFileNameWithoutExtension(res);
        }

        private void CreateAndRunProgram(CancellationToken token)
        {
            var ct = token;
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
                sc.AddRange(program.Split('\n'));
                commands = Creator.CreateCommandList(sc);
            }  
            ct.ThrowIfCancellationRequested();
            Interpreter.RunCommands(commands, Creator.CreateInputTapeFromString(input), ct);
            ct.ThrowIfCancellationRequested();
            Queue<string> output = Interpreter.OutputTape;
            string finalOutput = "";
            foreach (string s in output)
            {
                finalOutput += s + " ";
            }
            finalOutput = finalOutput.Trim();
            Page!.OutputTapeString = finalOutput;
            Page!.Memory = MemoryDictionaryToMemoryRowConverter.MemoryDictionaryToMemoryRows(Interpreter.Memory);
        }
    }
}