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
        public static string DEFAULT_HEADER { get => "NEW RAMCode"; }

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

        private readonly ParameterBaseCommand<string> _addPage = new(Logic.Logic.CreateNewPage, () => true);
        public ParameterBaseCommand<string> AddPageCommand { get => _addPage; }

        private readonly AsyncCommandBase _runProgram;
        public AsyncCommandBase RunProgram { get => _runProgram; }

        private readonly CommandBase _stopProgram;
        public CommandBase StopProgram { get => _stopProgram; }

        public MainWindowViewModel()
        {
            _instance = this;
            _pages = new ObservableCollection<HostViewModel>();
            _page = null;

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
            }, () => { try { return !Page.ProgrammRunning; } catch (NullReferenceException) { return false; } });

            _stopProgram = new(() =>
            {
                //Program won't be stopped when it's not running. Token is created when user runs the program.
#pragma warning disable CS8602 // Dereference of a possibly null reference. 
                Page.Token.Cancel();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }, () => { try { return Page.ProgrammRunning; } catch (NullReferenceException) { return false; } });
        }
    }
}
