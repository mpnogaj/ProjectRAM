using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System.IO;

namespace RAMEditorMultiplatform.Windows
{
    public class MainWindow : Window
    {
        public TabControl Files;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Files = this.FindControl<TabControl>("Files");
            Logic.Logic.MainWindow = this;
            Logic.Logic.CreateNewPage();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        
    }
}
