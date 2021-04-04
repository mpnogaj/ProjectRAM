using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Common;
using RAMEditorMultiplatform.Models;
using RAMEditorMultiplatform.ViewModels;
using RAMEditorMultiplatform.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

namespace RAMEditorMultiplatform.Logic
{
    public static class Logic
    {
        public static readonly List<FileDialogFilter> RAMCODE_FILTER = new List<FileDialogFilter>
        {
            new FileDialogFilter
            {
                Name = "RAMCode file",
                Extensions = new List<string>()
                {
                    "RAMCode"
                }
            }
        };


        public static MainWindow? MainWindow { get; set; }

        public static void CreateNewPage(string header = "NEW")
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
            foreach (string s in _output)
            {
                finalOutput += s + " ";
            }
            finalOutput = finalOutput.Trim();
            host.OutputTapeString = finalOutput;
            host.Memory.Clear();
            var newMemory = new ObservableCollection<MemoryCell>();
            foreach (var memoryRow in Interpreter.Memory)
            {
                newMemory.Add(new MemoryCell
                {
                    Address = memoryRow.Key,
                    Value = memoryRow.Value
                });
            }
            host.Memory = newMemory;
        }

        public static async void SaveFileAs(HostViewModel file)
        {
            var sfd = new SaveFileDialog
            {
                InitialFileName = file.Header,
                Title = "Save file",
                Filters = RAMCODE_FILTER
            };

            var res = await sfd.ShowAsync(GetAppInstance().MainWindow);
            if (!string.IsNullOrEmpty(res))
            {
                file.Path = res;
                SaveToFile(res, file.ProgramString);
            }
        }

        public static void SaveToFile(string file, string content)
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(content);
            }
        }

        public static async void OpenFile()
        {
            var ofd = new OpenFileDialog
            {
                Title = "Open file",
                AllowMultiple = true,
                Filters = RAMCODE_FILTER
            };
            var files = await ofd.ShowAsync(GetAppInstance().MainWindow);
            if (files == null)
            {
                return;
            }
            LoadFiles(files);
        }

        public static void LoadFiles(string[] files)
        {
            foreach (string file in files)
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    MainWindowViewModel.Instance.Pages.Add(new HostViewModel
                    {
                        Header = Path.GetFileNameWithoutExtension(file),
                        Path = file,
                        ProgramString = ReadFromFile(file)
                    });
                }
            }
        }

        public static string ReadFromFile(string file)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
        }

        public static void ChangeFontSize(HostViewModel page, int offset)
        {
            page.FontSize += offset;
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
