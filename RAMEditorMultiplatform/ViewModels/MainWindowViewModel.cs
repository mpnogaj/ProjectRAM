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

        private HostViewModel _page;
        public HostViewModel Page
        {
            get => _page;
            set { SetProperty(ref _page, value); }
        }

        private readonly ParameterBaseCommand<string> _addPage;
        public ParameterBaseCommand<string> AddPageCommand { get => _addPage; }

        private readonly CommandBase _openFile;
        public CommandBase OpenFile { get => _openFile; }

        private readonly ParameterBaseCommand<HostViewModel> _saveFileAs;
        public ParameterBaseCommand<HostViewModel> SaveFileAs { get => _saveFileAs; }

        private readonly ParameterBaseCommand<HostViewModel> _saveFile;
        public ParameterBaseCommand<HostViewModel> SaveFile { get => _saveFile; }

        private readonly AsyncCommandBase _runProgram;
        public AsyncCommandBase RunProgram { get => _runProgram; }

        private readonly CommandBase _stopProgram;
        public CommandBase StopProgram { get => _stopProgram; }

        private readonly CommandBase _closeProgram;
        public CommandBase CloseProgram { get => _closeProgram; }

        private readonly CommandBase _increaseFontSize;
        public CommandBase IncreaseFontSize { get => _increaseFontSize; }

        private readonly CommandBase _decreaseFontSize;
        public CommandBase DecreaseFontSize { get => _decreaseFontSize; }

        private readonly CommandBase _switchEditors;
        public CommandBase SwitchEditors { get => _switchEditors; }

        private readonly ParameterBaseCommand<string> _clear;
        public ParameterBaseCommand<string> Clear { get => _clear; }

        private readonly ParameterBaseCommand<string> _export;
        public ParameterBaseCommand<string> Export { get => _export; }

        private readonly CommandBase _import;
        public CommandBase Import { get => _import; }

        public MainWindowViewModel()
        {
            _instance = this;
            _pages = new ObservableCollection<HostViewModel>();
            _page = null;

            _addPage = new(L.CreateNewPage, () => true);
            _openFile = new(L.OpenFile, () => true);
            _saveFileAs = new(L.SaveFileAs, () => IsFileOpened());
            _saveFile = new((page) =>
            {
                if (string.IsNullOrEmpty(page.Path))
                {
                    L.SaveFileAs(page);
                }
                else
                {
                    L.SaveToFile(page.Path, page.ProgramString);
                }
            }, () => IsFileOpened());
            _closeProgram = new(L.Exit, () => true);

            _increaseFontSize = new(() => L.ChangeFontSize(Page, 1), IsFileOpened);
            _decreaseFontSize = new(() => L.ChangeFontSize(Page, -1), () => IsFileOpened() && Page.FontSize > 1);

            _runProgram = new(async () =>
            {
                Page.Token = new CancellationTokenSource();
                try
                {
                    L.SetCursor(StandardCursorType.Wait);
                    Page.ProgrammRunning = true;
                    await Task.Run(() => { L.RunProgram(Page.Token.Token); });
                }
                catch (OperationCanceledException) { /*Ignore*/ }
                finally
                {
                    Page.ProgrammRunning = false;
                    L.SetCursor(StandardCursorType.Arrow);
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _stopProgram = new(() =>
            {
                //Program won't be stopped when it's not running. Token is created when user runs the program.
#pragma warning disable CS8602 // Dereference of a possibly null reference. 
                Page.Token.Cancel();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }, () => IsFileOpened() && IsProgramRunning());

            _switchEditors = new(() =>
            {
                Page.HandleEditorSwitch();
            }, () => IsFileOpened() && !IsProgramRunning());

            _clear = new((target) =>
            {
                switch (target)
                {
                    case "memory":
                        this.Page.Memory = new();
                        break;
                    case "inputTape":
                        this.Page.InputTapeString = string.Empty;
                        break;
                    case "outputTape":
                        this.Page.OutputTapeString = string.Empty;
                        break;
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _import = new(() => 
            { 

            }, () => IsFileOpened() && !IsProgramRunning());

            _export = new((traget) =>
            {
                
            }, () => IsFileOpened() && !IsProgramRunning());
        }

        private bool IsFileOpened()
        {
            return Page != null;
        }

        private bool IsProgramRunning()
        {
            return Page.ProgrammRunning;
        }

        public void FileOver(object sender, DragEventArgs e)
        {
            bool dropEnabled = false;
            if(e.Data != null && e.Data.GetDataFormats().Where(format => format == DataFormats.FileNames).Any())
            {
                if (e.Data.GetFileNames() != null && e.Data.GetFileNames().Where(name => Path.GetExtension(name) == ".RAMCode").Any())
                {
                    dropEnabled = true;
                }
                System.Diagnostics.Debug.WriteLine(e.Data.GetFileNames().ToList().ToString());
            }
            if (dropEnabled) return;
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }

        public void FileDropped(object sender, DragEventArgs e)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            string[] files = e.Data.GetFileNames().Where(fileName => Path.GetExtension(fileName) == ".RAMCode").ToArray();
#pragma warning restore CS8604 // Possible null reference argument.
            L.LoadFiles(files);
        }
    }
}
