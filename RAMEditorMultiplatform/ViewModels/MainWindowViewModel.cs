using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.ViewModels;
using RAMEditorMultiplatform.Helpers;
using System.Threading;
using Avalonia.Input;
using System.IO;
using L = RAMEditorMultiplatform.Logic.Logic;
using Avalonia.Controls;
using RAMEditorMultiplatform.Converters;
using System.Text.RegularExpressions;

namespace RAMEditorMultiplatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string DEFAULT_HEADER { get => "NEW"; }

        private static MainWindowViewModel _instance;
        public static MainWindowViewModel Instance { get => _instance; }

        private ObservableCollection<HostViewModel> _pages;
        public ObservableCollection<HostViewModel> Pages
        {
            get => _pages;
            set { SetProperty(ref _pages, value); }
        }

        private HostViewModel? _page;
        public HostViewModel? Page
        {
            get => _page;
            set { SetProperty(ref _page, value); }
        }

        private readonly RelayCommand _addPage;
        public RelayCommand AddPageCommand { get => _addPage; }

        private readonly RelayCommand _openFile;
        public RelayCommand OpenFile { get => _openFile; }

        private readonly RelayCommand _saveFileAs;
        public RelayCommand SaveFileAs { get => _saveFileAs; }

        private readonly RelayCommand _saveFile;
        public RelayCommand SaveFile { get => _saveFile; }

        private readonly AsyncRelayCommand _runProgram;
        public AsyncRelayCommand RunProgram { get => _runProgram; }

        private readonly RelayCommand _stopProgram;
        public RelayCommand StopProgram { get => _stopProgram; }

        private readonly RelayCommand _closeProgram;
        public RelayCommand CloseProgram { get => _closeProgram; }

        private readonly RelayCommand _increaseFontSize;
        public RelayCommand IncreaseFontSize { get => _increaseFontSize; }

        private readonly RelayCommand _decreaseFontSize;
        public RelayCommand DecreaseFontSize { get => _decreaseFontSize; }

        private readonly RelayCommand _switchEditors;
        public RelayCommand SwitchEditors { get => _switchEditors; }

        private readonly RelayCommand<string> _clear;
        public RelayCommand<string> Clear { get => _clear; }

        private readonly AsyncRelayCommand<string> _export;
        public AsyncRelayCommand<string> Export { get => _export; }

        private readonly AsyncRelayCommand _import;
        public AsyncRelayCommand Import { get => _import; }

        public MainWindowViewModel()
        {
            _instance = this;
            _pages = new ObservableCollection<HostViewModel>();
            _page = null;
            _addPage = new(() => L.CreateNewPage(DEFAULT_HEADER));
            _openFile = new(() => L.OpenFile());
            _saveFileAs = new(() => L.SaveFileAs(Page!), () => IsFileOpened());
            _saveFile = new(() =>
            {
                if (string.IsNullOrEmpty(Page!.Path))
                {
                    L.SaveFileAs(Page!);
                }
                else
                {
                    L.WriteToFile(Page!.Path, Page!.ProgramString);
                }
            }, () => IsFileOpened());
            _closeProgram = new(L.Exit, () => true);

            _increaseFontSize = new(() => Page!.FontSize += 1, IsFileOpened);
            _decreaseFontSize = new(() => Page!.FontSize -= 1, () => IsFileOpened() && Page!.FontSize > 1);

            _runProgram = new(async () =>
            {
                Page!.Token = new CancellationTokenSource();
                try
                {
                    L.SetCursor(StandardCursorType.Wait);
                    Page.ProgramRunning = true;
                    await Task.Run(() => { L.RunProgram(Page.Token.Token); });
                }
                catch (OperationCanceledException) { /*Ignore*/ }
                finally
                {
                    Page.ProgramRunning = false;
                    L.SetCursor(StandardCursorType.Arrow);
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _stopProgram = new(() =>
            {
                Page!.Token!.Cancel();
            }, () => IsFileOpened() && IsProgramRunning());

            _switchEditors = new(() =>
            {
                Page!.HandleEditorSwitch();
            }, () => IsFileOpened() && !IsProgramRunning());

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
                    Filters = L.TEXT_FILE_FILTER
                };

                var res = await ofd.ShowAsync(L.GetAppInstance().MainWindow);
                if(res.Length > 0)
                {
                    string content = L.ReadFromFile(res[0]);
                    Page!.InputTapeString = content.Replace(",", "");
                }

            }, () => IsFileOpened() && !IsProgramRunning());

            _export = new(async (target) =>
            {
                string content = string.Empty;
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
                    Filters = L.TEXT_FILE_FILTER
                };

                var res = await sfd.ShowAsync(L.GetAppInstance().MainWindow);
                if (!string.IsNullOrEmpty(res))
                {
                    L.WriteToFile(res, content);
                }
            }, () => IsFileOpened() && !IsProgramRunning());
        }

        private bool IsFileOpened()
        {
            return Page != null;
        }

        private bool IsProgramRunning()
        {
            return Page.ProgramRunning;
        }

        public void FileOver(object sender, DragEventArgs e)
        {
            bool dropEnabled = false;
            if(e.Data != null && e.Data.GetDataFormats().Where(format => format == DataFormats.FileNames).Any())
            {
                if (e.Data.GetFileNames() != null && e.Data.GetFileNames()!.Where(name => Path.GetExtension(name) == ".RAMCode").Any())
                {
                    dropEnabled = true;
                }
            }
            if (dropEnabled) return;
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }

        public void FileDropped(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetFileNames()!.Where(fileName => Path.GetExtension(fileName) == ".RAMCode").ToArray();
            L.LoadFiles(files);
        }
    }
}
