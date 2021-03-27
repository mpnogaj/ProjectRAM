using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using RAMEditorMultiplatform.ViewModels;
using System.IO;

namespace RAMEditorMultiplatform.Views
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
            Logic.Logic.MainWindow = this;
            this.DataContext = new MainWindowViewModel();
            Logic.Logic.CreateNewPage();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
