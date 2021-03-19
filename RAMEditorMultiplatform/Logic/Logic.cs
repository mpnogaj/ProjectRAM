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
        static int i = 0;

        public static MainWindow? MainWindow { get; set; }

        public static void CreateNewPage(string header = "NEW RAMCode")
        {
            MainWindowViewModel.Instance.Pages.Add(new HostViewModel
            {
                Header = header,
                Content = i++.ToString()
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

        public static void Exit()
        {
            ((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).Shutdown();
        }
    }
}
