using RAMEditor.Properties;
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
        public const string DEFAULT_FILE = "NEW";
        public static readonly ResourceDictionary LANGUAGE = Application.Current.Resources.MergedDictionaries[0];


        public MainWindow()
        {
            InitializeComponent();
            App.Log("Initialized main window");
            var args = Environment.GetCommandLineArgs();
            var userArgs = args.Skip(1);
            App.Log($"Arguments: {userArgs}");
            if (Settings.Default.UseDiscordRPC)
            {
                InitializeDiscord();
            }
            if (!userArgs.Any())
            {
                Logic.Logic.CreateTabPage(DEFAULT_FILE);
                this.Files.SelectedIndex = 0;
                UpdateStatus();
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

        public void InitializeDiscord()
        {
            App.Log("Initializing Discord");
            DiscordPresence.DiscordRPC.Initialize("817494043662614569", "default", "Test");
            this.Files.SelectionChanged += (sender, e) => UpdateStatus();
        }

        public void DeinitializeDiscord()
        {
            App.Log("Deinitializing Discord");
            DiscordPresence.DiscordRPC.Shutdown();
            this.Files.SelectionChanged -= (sender, e) => UpdateStatus();
        }

        private void UpdateStatus()
        {
            App.Log("Updating status");
            if (!Settings.Default.UseDiscordRPC)
            {
                return;
            }
            if (Files.SelectedIndex == -1)
            {
                DiscordPresence.DiscordRPC.Update("", "Śpi");
            }
            else
            {
                var selectedTab = Logic.Logic.GetSelectedTab(this.Files);
                string fileName = Logic.Logic.GetOpenedProgramName(selectedTab);
                DiscordPresence.DiscordRPC.Update(fileName, "Pisze");
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

        private void Files_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                App.Log($"User dropped {files.Length} files");
                foreach (string file in files)
                {
                    Logic.Logic.CreateTabPage(Path.GetFileNameWithoutExtension(file), file);
                }
            }
        }

        private void Files_OnDragOver(object sender, DragEventArgs e)
        {
            App.Log("User tries to drop files");
            bool dropEnabled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) != ".RAMCode")
                    {
                        dropEnabled = false;
                        break;
                    }
                }
            }
            else
            {
                dropEnabled = false;
            }

            if (dropEnabled) return;
            App.Log("User tried to drop invalid item");
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}
