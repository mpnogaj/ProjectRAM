using Microsoft.Win32;
using RAMEditor.CustomControls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RAMEditor
{
    public static class ButtonLogic
    {
        #region ClickEventMethods
        public static RoutedEventHandler CloseClick => Close_Click;
        public static RoutedEventHandler CloseTabClick => CloseTab_Click;
        public static RoutedEventHandler NewFlieClick => NewFile_Click;
        public static RoutedEventHandler OpenFileClick => OpenFile_Click;

        private static void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private static void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            ContextMenu cm = mi.Parent as ContextMenu;
            TabItem tb = ((ContentControl)cm.PlacementTarget).Parent as TabItem;
            TabControl tc = tb.Parent as TabControl;
            tc.Items.Remove(tb);
        }

        private static void NewFile_Click(object sender, RoutedEventArgs e)
        {
            CreateTabPage("NEW RAMCode");
        }

        private static void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Choose RAM Code to open",
                Multiselect = false,
                Filter = "RAM Code files (*.RAMCode)|*.RAMCode"
            };
            if(ofd.ShowDialog() == true)
            {
                CreateTabPage(Path.GetFileNameWithoutExtension(ofd.FileName), ofd.FileName);
            }
        }
        #endregion

        private static MainWindow GetMainWindow() { return Application.Current.MainWindow as MainWindow; }

        public static void CreateTabPage(string fileName)
        {
            MenuItem menuItemToAdd = new MenuItem { Header = "Close" };
            menuItemToAdd.Click += ButtonLogic.CloseTabClick;
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItemToAdd);

            GetMainWindow().Files.Items.Add(new TabItem
            {
                Header = new ContentControl
                {
                    Content = fileName,
                    ContextMenu = contextMenu
                },
                Content = new Host()
            });
        }

        public static void CreateTabPage(string fileName, string filePath)
        {
            MenuItem menuItemToAdd = new MenuItem { Header = "Close" };
            menuItemToAdd.Click += ButtonLogic.CloseTabClick;
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItemToAdd);
            GetMainWindow().Files.Items.Add(new TabItem
            {
                Header = new ContentControl
                {
                    Content = fileName,
                    ContextMenu = contextMenu
                },
                Content = new Host(filePath)
            });
        }
    }
}
