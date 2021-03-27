using RAMEditorMultiplatform.Views;
using RAMEditorMultiplatform.ViewModels;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.FreeDesktop;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using RAMEditorMultiplatform.CustomControls;
using Common;
using System.Threading;
using RAMEditorMultiplatform.Models;
using System.Collections.ObjectModel;
using Avalonia.Input;

namespace RAMEditorMultiplatform.Logic
{
    public static class Logic
    {

        public static MainWindow? MainWindow { get; set; }

        public static void CreateNewPage(string header = "NEW RAMCode")
        {
            MainWindowViewModel.Instance.Pages.Add(new HostViewModel
            {
                Header = header
            });
        }

        public static void ClosePage(HostViewModel page)
        {
            if (page != null)
            {
                MainWindowViewModel.Instance.Pages.Remove(page);
            }
            else
            {
                MainWindowViewModel.Instance.Pages.Remove(MainWindowViewModel.Instance.Page);
            }
        }

        public static void RunProgram(CancellationToken token)
        {
            var ct = token;
            HostViewModel host = MainWindowViewModel.Instance.Page;
            string _input = host.InputTapeString;
            string _program = host.ProgramString;
            var sc = new StringCollection();
            sc.AddRange(_program.Split(Environment.NewLine));
            List<Command> commands = Creator.CreateCommandList(sc);
            ct.ThrowIfCancellationRequested();
            Interpreter.RunCommands(commands, Creator.CreateInputTapeFromString(_input), ct);
            ct.ThrowIfCancellationRequested();
            Queue<string> _output = Interpreter.OutputTape;
            string finalOutput = "";
            foreach(string s in _output)
            {
                finalOutput += s + " ";
            }
            finalOutput = finalOutput.Trim();
            host.OutputTapeString = finalOutput;
            host.Memory.Clear();
            var newMemory = new ObservableCollection<MemoryCell>();
            foreach(var memoryRow in Interpreter.Memory)
            {
                newMemory.Add(new MemoryCell
                {
                    Address = memoryRow.Key,
                    Value = memoryRow.Value
                });
            }
            host.Memory = newMemory;
        }

        public static IClassicDesktopStyleApplicationLifetime GetAppInstance()
        {
            return (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        }

        public static void SetCursor(StandardCursorType cursor)
        {
            GetAppInstance().MainWindow.Cursor = new Cursor(cursor);
        }

        public static void Exit()
        {
            GetAppInstance().Shutdown();
        }
    }
}
