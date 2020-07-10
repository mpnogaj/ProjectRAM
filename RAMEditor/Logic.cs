using Common;
using Microsoft.Win32;
using RAMEditor.CustomControls;
using RAMEditor.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static Common.Interpreter;

namespace RAMEditor
{
    public static class Logic
    {
        /// <summary>
        /// Show Options window
        /// </summary>
        public static void ShowOptionsWindow()
        {
            new Options
            {
                Owner = Application.Current.MainWindow
            }.ShowDialog();
        }

        /// <summary>
        /// Show About window
        /// </summary>
        public static void ShowAboutWindow()
        {
            new About
            {
                Owner = Application.Current.MainWindow
            }.ShowDialog();
        }

        /// <summary>
        /// Gets a MainWindow reference 
        /// </summary>
        /// <returns>MainWindow reference</returns>
        public static MainWindow GetMainWindow()
        {
            return Application.Current.MainWindow as MainWindow;
        }

        /// <summary>
        /// Gets current host control reference
        /// </summary>
        /// <returns>Current host control reference</returns>
        public static Host GetHost()
        {
            return (Host)((TabItem)GetMainWindow().Files.SelectedItem).Content;
        }

        /// <summary>
        /// Creates new tab page
        /// </summary>
        /// <param name="header">Tab's header</param>
        public static void CreateTabPage(string header)
        {
            MenuItem menuItemToAdd = new MenuItem { Header = "Close" };
            menuItemToAdd.Click += ButtonLogic.CloseTabClick;
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItemToAdd);

            GetMainWindow().Files.Items.Add(new TabItem
            {
                Header = new ContentControl
                {
                    Content = header,
                    ContextMenu = contextMenu
                },
                Content = new Host()
            });
        }

        /// <summary>
        /// Creates new tab page and loads a file as text
        /// </summary>
        /// <param name="header">Tab's header</param>
        /// <param name="filePath">Path to file</param>
        public static void CreateTabPage(string header, string filePath)
        {
            MenuItem menuItemToAdd = new MenuItem { Header = "Close" };
            menuItemToAdd.Click += ButtonLogic.CloseTabClick;
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItemToAdd);
            GetMainWindow().Files.Items.Add(new TabItem
            {
                Header = new ContentControl
                {
                    Content = header,
                    ContextMenu = contextMenu
                },
                Content = new Host(filePath)
            });
        }

        /// <summary>
        /// Change page's header 
        /// </summary>
        /// <param name="tab">Reference to TabItem</param>
        /// <param name="name">New header</param>
        public static void ChangeHeaderPage(TabItem tab, string name)
        {
            MenuItem menuItemToAdd = new MenuItem { Header = "Close" };
            menuItemToAdd.Click += ButtonLogic.CloseTabClick;
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItemToAdd);
            tab.Header = new ContentControl
            {
                Content = name,
                ContextMenu = contextMenu
            };
        }

        /// <summary>
        /// Change font size in text box
        /// </summary>
        /// <param name="tb">Reference to TextBox control</param>
        /// <param name="offset">Value to add to current font size</param>
        public static void ChangeZoom(int offset)
        {
            if (bUsingTextEditor())
            {
                TextBox tb = GetHost().Code;
                if (tb.FontSize <= 1 && offset < 0) return;
                tb.FontSize += offset;
            }
            else
            {
                if (Settings.Default.SE1FontSize > 1 || offset >= 0)
                {
                    Settings.Default.SE1FontSize += offset;
                }
                if (Settings.Default.SE2FontSize > 1 || offset >= 0)
                {
                    Settings.Default.SE2FontSize += offset;
                }
                if (Settings.Default.SE3FontSize > 1 || offset >= 0)
                {
                    Settings.Default.SE3FontSize += offset;
                }
                if (Settings.Default.SE4FontSize > 1 || offset >= 0)
                {
                    Settings.Default.SE4FontSize += offset;
                }
                if (Settings.Default.SE5FontSize > 1 || offset >= 0)
                {
                    Settings.Default.SE5FontSize += offset;
                }
            }
        }

        /// <summary>
        /// Exit program
        /// </summary>
        public static void Exit()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Close tab
        /// </summary>
        /// <param name="tb">Reference to tab to be closed</param>
        public static void CloseTab(TabItem tb)
        {
            TabControl tc = tb.Parent as TabControl;
            ((TabControl)tb.Parent).Items.Remove(tb);
        }

        /// <summary>
        /// Gets Tab page from ContextMenu
        /// </summary>
        /// <param name="mi">Menu Item</param>
        /// <returns>Tab page</returns>
        public static TabItem GetTabItemFromMenuItem(MenuItem mi)
        {
            return ((ContentControl)((ContextMenu)mi.Parent).PlacementTarget).Parent as TabItem;
        }

        /// <summary>
        /// Gets selected tab
        /// </summary>
        /// <param name="parent">Parent tab control</param>
        /// <returns>Selected tab item object</returns>
        public static TabItem GetSelectedTab(TabControl parent) { return parent.SelectedItem as TabItem; }

        /// <summary>
        /// Clears memory
        /// </summary>
        public static void ClearMemory() { GetHost().Memory.Children.Clear(); }

        /// <summary>
        /// Clears input tape
        /// </summary>
        public static void ClearInputTape() { GetHost().InputTape.Text = string.Empty; }

        /// <summary>
        /// Clears output tape
        /// </summary>
        public static void ClearOutputTape() { GetHost().OutputTape.Text = string.Empty; }

        /// <summary>
        /// Creates SaveFileDialog object
        /// </summary>
        /// <param name="t">Window title</param>
        /// <param name="f">Filter</param>
        /// <returns>SaveFileDialog object</returns>
        public static SaveFileDialog PrepareSaveFileDialog(string t, string f)
        {
            return new SaveFileDialog
            {
                Title = t,
                Filter = f
            };
        }

        /// <summary>
        /// Creates OpenFileDialog object
        /// </summary>
        /// <param name="t">Window title</param>
        /// <param name="f">Filter</param>
        /// <returns>OpenFileDialog object</returns>
        public static OpenFileDialog PrepareOpenFileDialog(string t, string f)
        {
            return new OpenFileDialog
            {
                Multiselect = false,
                Title = t,
                Filter = f
            };
        }

        /// <summary>
        /// Runs a program
        /// </summary>
        /// <param name="parent">Host control where the program is written in</param>
        public static void RunProgram(Host parent, CancellationToken token)
        {
            StringCollection sc = new StringCollection();
            List<Command> cl = new List<Command>();
            Queue<string> input = new Queue<string>();
            parent.Dispatcher.Invoke(() =>
            {
                parent.OutputTape.Text = string.Empty;
                parent.Memory.Children.Clear();
                sc = Settings.Default.TextEditor ? parent.GetText() : parent.SimpleEditor.ConvertToStringCollection();
                cl = CreateCommandList(sc);
                input = CreateInputTapeFromString(parent.InputTape.Text);
            });

            token.ThrowIfCancellationRequested();
            Tuple<Queue<string>, List<Cell>> result = RunCommands(cl, input, token);
            token.ThrowIfCancellationRequested();
            Queue<string> output = result.Item1;
            List<Cell> memory = result.Item2;
            memory.Sort();
            parent.Dispatcher.Invoke(() =>
            {
                while (output.Count > 0)
                {
                    parent.OutputTape.Text += $"{output.Dequeue()} ";
                }
                foreach (Cell mem in memory)
                {
                    parent.Memory.Children.Add(new MemoryGrid(mem));
                }
            });
        }

        /// <summary>
        /// Get List of CodeLine objects
        /// </summary>
        /// <returns>List of CodeLine objects</returns>
        public static ObservableCollection<CodeLine> GetSimpleEditorLines()
        {
            return GetHost().SimpleEditor.vm.Lines;
        }

        public static List<RamInterpreterException> CheckIfValid()
        {
            Host parent = GetHost();
            StringCollection sc;
            if (parent.SimpleEditor.Visibility == Visibility.Visible)
                sc = parent.SimpleEditor.ConvertToStringCollection();
            else
                sc = parent.GetText();
            return Validator.ValidateProgram(Interpreter.CreateCommandList(sc));
        }

        public static void ShowErrorMessage(string header, string error)
        {
            MessageBox.Show(error, header, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool bUsingTextEditor()
        {
            if (GetHost().Code.Visibility == Visibility.Visible)
                return true;
            return false;
        }

        public static void HideBottomDock()
        {
            var host = GetHost();
            host.BottomDock.Visibility = Visibility.Collapsed;
            GetHost().LeftColumn.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Auto);
        }

        public static void ShowBottomDock()
        {
            var host = GetHost();
            host.BottomDock.Visibility = Visibility.Visible;
            host.LeftColumn.RowDefinitions[3].Height = new GridLength(5, GridUnitType.Star);
        }
    }
}
