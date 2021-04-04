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

        public MainWindowViewModel()
        {
            _instance = this;
            _pages = new ObservableCollection<HostViewModel>();
            _page = null;
            
            _addPage = new(Logic.Logic.CreateNewPage, () => true);
            _openFile = new(Logic.Logic.OpenFile, () => true);
            _saveFileAs = new(Logic.Logic.SaveFileAs, () => IsFileOpened());
            _saveFile = new((page) =>
            {
                if (string.IsNullOrEmpty(page.Path))
                {
                    Logic.Logic.SaveFileAs(page);
                }
                else
                {
                    Logic.Logic.SaveToFile(page.Path, page.ProgramString);
                }
            }, () => IsFileOpened());
            _closeProgram = new(Logic.Logic.Exit, () => true);

            _increaseFontSize = new(() => Logic.Logic.ChangeFontSize(Page, 1), IsFileOpened);
            _decreaseFontSize = new(() => Logic.Logic.ChangeFontSize(Page, -1), () => IsFileOpened() && Page.FontSize > 1);

            _runProgram = new(async () =>
            {
                Page.Token = new CancellationTokenSource();
                try
                {
                    Logic.Logic.SetCursor(StandardCursorType.Wait);
                    Page.ProgrammRunning = true;
                    await Task.Run(() => { Logic.Logic.RunProgram(Page.Token.Token); });
                }
                catch (OperationCanceledException) { /*Ignore*/ }
                finally
                {
                    Page.ProgrammRunning = false;
                    Logic.Logic.SetCursor(StandardCursorType.Arrow);
                }
            }, () => IsFileOpened() && !IsProgramRunning());

            _stopProgram = new(() =>
            {
                //Program won't be stopped when it's not running. Token is created when user runs the program.
#pragma warning disable CS8602 // Dereference of a possibly null reference. 
                Page.Token.Cancel();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }, () => IsFileOpened() && IsProgramRunning());
        }

        private bool IsFileOpened()
        {
            return Page != null;
        }

        private bool IsProgramRunning()
        {
            return Page.ProgrammRunning;
        }
    }
}
