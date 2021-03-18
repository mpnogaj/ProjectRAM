using RAMEditorMultiplatform.Views;
using RAMEditorMultiplatform.ViewModels;
using RAMEditorMultiplatform.Models;
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

namespace RAMEditorMultiplatform.Logic
{
    public static class Logic
    {
        public static MainWindow? MainWindow { get; set; }

        public static void CreateNewPage(string header = "NEW RAMCode")
        {
            //var window = MainWindow;
            //var tabControl = window.Files;
            //var cm = new ContextMenu();
            //var mu = new MenuItem{ Header = "Close" };
            //var menu = cm.Items.Cast<object>().ToList();
            //menu.Add(mu);
            //cm.Items = menu;

            //var tabItem = new TabItem { Header = new ContentControl
            //{
            //    Content = header,
            //    ContextMenu = cm
            //}, Content = new Host()};
            //var tabs = tabControl.Items.Cast<object>().ToList();
            //tabs.Add(tabItem);
            //tabControl.Items = tabs;

            MainWindowViewModel.Instance.Pages.Add(new TabPageModel
            {
                Header = header,
                Host = new Host()
            });
        }

        public static void Exit()
        {
            ((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).Shutdown();
        }
    }
}
