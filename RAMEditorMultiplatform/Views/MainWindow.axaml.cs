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
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Logic.Logic.MainWindow = this;
            this.DataContext = new MainWindowViewModel();
            Logic.Logic.CreateNewPage();
            var Files = this.FindControl<TabControl>("Files");
            Files.AddHandler(DragDrop.DropEvent, File_Drop);
            Files.AddHandler(DragDrop.DragOverEvent, File_Drag_Over);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void File_Drop(object? sender, DragEventArgs e) => MainWindowViewModel.Instance.FileDropped(sender, e);

        private void File_Drag_Over(object? sender, DragEventArgs e) => MainWindowViewModel.Instance.FileOver(sender, e);
    }
}
