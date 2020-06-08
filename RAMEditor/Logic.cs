using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Common;
using RAMEditor.CustomControls;
using RAMEditor.Windows;
using static Common.Interpreter;

namespace RAMEditor
{
    public static class Logic
    {
        /// <summary>
        /// Show About window
        /// </summary>
        public static void ShowAboutWindow()
        {
            new About().ShowDialog();
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
        public static void ChangeZoom(TextBox tb, int offset)
        {
            if (tb.FontSize <= 1 && offset < 0) return;
            tb.FontSize += offset;
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

        public static TabItem GetSelectedTab(TabControl parent)
        {
            return parent.SelectedItem as TabItem;
        }

        public static void ClearMemory()
        {
            GetHost().Memory.Children.Clear();
        }

        public static void ClearInputTape()
        {
            GetHost().InputTape.Text = string.Empty;
        }

        public static void ClearOutputTape()
        {
            GetHost().OutputTape.Text = string.Empty;
        }

        public static void RunProgram(Host parent)
        {
            parent.OutputTape.Text = string.Empty;
            Tuple<Queue<string>, List<Cell>> result = 
            RunCommands(CreateCommandList(parent.GetText()), CreateInputTapeFromString(parent.InputTape.Text));
            Queue<string> output = result.Item1;
            List<Cell> memory = result.Item2;
            memory.Sort();
            while (output.Count > 0)
            {
                parent.OutputTape.Text += $"{output.Dequeue()} ";
            }
            UIElement header = parent.Memory.Children[0];
            parent.Memory.Children.Clear();
            parent.Memory.Children.Add(header);
            foreach (Cell mem in memory)
            {
                parent.Memory.Children.Add(new MemoryGrid(mem));
            }
        }
    }
}
