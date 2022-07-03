using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.ViewModels;

namespace ProjectRAM.Editor.Views
{
	public class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			DataContext = new MainWindowViewModel();
			var files = this.FindControl<TabControl>("Files");
			files.AddHandler(DragDrop.DropEvent, File_Drop);
			files.AddHandler(DragDrop.DragOverEvent, File_Drag_Over);

			Closing += OnClose;
		}

		private void OnClose(object? sender, CancelEventArgs e)
		{
			Settings.StartupLocation = Position;
			Settings.WindowSize = new Tuple<double, double>(Width, Height);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void File_Drop(object? sender, DragEventArgs e) => ((MainWindowViewModel)DataContext!).FileDropped(sender, e);

		private void File_Drag_Over(object? sender, DragEventArgs e) => MainWindowViewModel.FileOver(sender, e);
	}
}
