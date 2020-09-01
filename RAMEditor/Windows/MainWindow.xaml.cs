using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RAMEditor.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            var userArgs = args.Skip(1);
            if (userArgs == null || userArgs.Count() < 1)
            {
                Logic.Logic.CreateTabPage("NEW RAMCode");
            }
            else
            {
                foreach (var arg in userArgs)
                {
                    Uri pathUri;
                    bool isValidUri = Uri.TryCreate(arg, UriKind.Absolute, out pathUri);
                    if (isValidUri && pathUri != null && pathUri.IsLoopback)
                    {
                        Logic.Logic.CreateTabPage(Path.GetFileNameWithoutExtension(arg), arg);
                    }
                }
            }
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }
    }
}
