using Common;
using Microsoft.Win32;
using RAMEditor.CustomControls;
using RAMEditor.Helpers;
using RAMEditor.Properties;
using RAMEditor.Windows;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RAMEditor.Logic
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

        public static FlowDocument GetFlowDocument()
        {
            FlowDocument doc = new FlowDocument();
            Table t = new Table
            {
                CellSpacing = 0
            };
            var host = GetHost();
            t.BorderThickness = new Thickness(7);
            List<Command> rows = new List<Command>();
            doc.Blocks.Add(t);
            const int columns = 5;
            var glc = new GridLengthConverter();
            t.Columns.Add(new TableColumn { Width = (GridLength)glc.ConvertFromString("50") });
            t.Columns.Add(new TableColumn { Width = (GridLength)glc.ConvertFromString("130") });
            t.Columns.Add(new TableColumn { Width = (GridLength)glc.ConvertFromString("90") });
            t.Columns.Add(new TableColumn { Width = (GridLength)glc.ConvertFromString("130") });
            t.Columns.Add(new TableColumn { Width = GridLength.Auto });
            for (int i = 1; i < columns; i++)
            {
                t.Columns.Add(new TableColumn { Width = GridLength.Auto });
            }
            t.RowGroups.Add(new TableRowGroup());
            t.RowGroups[0].Rows.Add(new TableRow());
            t.RowGroups[0].Rows.Add(new TableRow());

            ContentControl cc = GetSelectedTab(GetMainWindow().Files).Header as ContentControl;
            string programName = cc.Content.ToString();
            t.RowGroups[0].Rows[0].Cells.Add(new FlowDocCell(new Paragraph(new Run(programName)), true));
            var title = t.RowGroups[0].Rows[0].Cells[0];
            title.ColumnSpan = columns;
            title.FontSize = 22;

            var header = t.RowGroups[0].Rows[1];
            header.Cells.Add(new FlowDocCell(new Paragraph(new Run("Line")), true));
            header.Cells.Add(new FlowDocCell(new Paragraph(new Run("Label")), true));
            header.Cells.Add(new FlowDocCell(new Paragraph(new Run("Command")), true));
            header.Cells.Add(new FlowDocCell(new Paragraph(new Run("Value")), true));
            header.Cells.Add(new FlowDocCell(new Paragraph(new Run("Comment")), true));


            rows = bUsingTextEditor() ?
                Creator.CreateCommandList(
                    host.GetText())
                : GetSimpleEditorLines();

            int j = 2;
            foreach (var row in rows)
            {
                t.RowGroups[0].Rows.Add(new TableRow());
                var currRow = t.RowGroups[0].Rows[j];
                currRow.Cells.Add(new FlowDocCell(new Paragraph(new Run((j - 1).ToString())), false));
                currRow.Cells.Add(new FlowDocCell(new Paragraph(new Run(row.Label)), false));
                currRow.Cells.Add(new FlowDocCell(new Paragraph(new Run(row.CommandType.ToString())), false));
                currRow.Cells.Add(new FlowDocCell(new Paragraph(new Run(row.Argument)), false));
                currRow.Cells.Add(new FlowDocCell(new Paragraph(new Run(row.Comment)), false));
                j++;
            }
            return doc;
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
            MenuItem menuItemToAdd = new MenuItem { Header = "Close", InputGestureText = "Ctrl+W" };
            menuItemToAdd.Click += ButtonLogic.CloseTabClick;
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItemToAdd);

            TabControl tc = GetMainWindow().Files;
            tc.Items.Add(new TabItem
            {
                Header = new ContentControl
                {
                    Content = header,
                    ContextMenu = contextMenu
                },
                Content = new Host()
            });

            if (tc.SelectedItem == null)
            {
                tc.SelectedIndex = 0;
            }
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
            TabControl tc = GetMainWindow().Files;
            tc.Items.Add(new TabItem
            {
                Header = new ContentControl
                {
                    Content = header,
                    ContextMenu = contextMenu
                },
                Content = new Host(filePath)
            });
            if (tc.SelectedItem == null)
            {
                tc.SelectedIndex = 0;
            }
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
                if (Settings.Default.TBFontSize > 1 || offset >= 0)
                {
                    Settings.Default.TBFontSize += offset;
                }
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
            Settings.Default.Save();
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

        public static void CloseTab()
        {
            TabControl tc = GetMainWindow().Files;
            tc.Items.Remove(tc.SelectedItem);
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
            List<Command> commands = new List<Command>();
            Queue<string> input = new Queue<string>();
            parent.Dispatcher.Invoke(() =>
            {
                parent.FixInputTapeFormat();
                parent.OutputTape.Text = string.Empty;
                parent.Memory.Children.Clear();
                parent.IsProgramRunning = true;
                if (bUsingTextEditor())
                {
                    commands = Creator.CreateCommandList(parent.GetText());
                }
                else
                {
                    commands = new List<Command>(parent.SimpleEditor.Lines);
                }
                input = Creator.CreateInputTapeFromString(parent.InputTape.Text);
            });

            token.ThrowIfCancellationRequested();
            Interpreter.RunCommands(commands, input, token);
            token.ThrowIfCancellationRequested();
            Queue<string> output = Interpreter.OutputTape;
            Dictionary<string, string> memory = Interpreter.Memory;
            List<Cell> cells = new List<Cell>();
            foreach (var item in memory)
            {
                cells.Add(new Cell(item.Value, item.Key));
            }
            cells.Sort();
            parent.Dispatcher.Invoke(() =>
            {
                while (output.Count > 0)
                {
                    parent.OutputTape.Text += $"{output.Dequeue()} ";
                }
                foreach (var cell in cells)
                {
                    parent.Memory.Children.Add(new MemoryGrid(cell));
                }
                parent.BottomDock.ComplexityReport.UpdateData(Interpreter.ExecutedCommands, Interpreter.Memory);
            });
        }

        /// <summary>
        /// Get List of CodeLine objects
        /// </summary>
        /// <returns>List of CodeLine objects</returns>
        public static List<Command> GetSimpleEditorLines()
        {
            return new List<Command>(GetHost().SimpleEditor.Lines);
        }

        public static List<RamInterpreterException> CheckIfValid()
        {
            Host parent = GetHost();
            StringCollection sc;
            if (parent.SimpleEditor.Visibility == Visibility.Visible)
            {
                return Validator.ValidateProgram(GetSimpleEditorLines());
            }
            else
            {
                sc = parent.GetText();
            }

            return Validator.ValidateProgram(Creator.CreateCommandList(sc));
        }

        public static void ShowErrorMessage(string header, string error)
        {
            MessageBox.Show(error, header, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool bUsingTextEditor()
        {
            if (GetHost().Code.Visibility == Visibility.Visible)
            {
                return true;
            }

            return false;
        }

        public static void HideBottomDock()
        {
            var host = GetHost();
            host.BottomDockRow.Height = new GridLength(0);
        }

        public static void ShowBottomDock()
        {
            var host = GetHost();
            host.BottomDockRow.Height = new GridLength(200);
        }

        public static StringCollection GetStringCollectionFromTextEditor(TextBox txtEditor)
        {
            StringCollection sc = new StringCollection();
            string[] txt = txtEditor.Text.Split("\r\n");
            sc.AddRange(txt);
            return sc;
        }
    }
}
