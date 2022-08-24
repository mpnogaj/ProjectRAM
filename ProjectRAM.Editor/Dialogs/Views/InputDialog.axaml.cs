using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProjectRAM.Editor.Dialogs.ViewModels;

namespace ProjectRAM.Editor.Dialogs.Views
{
	internal partial class InputDialog : Window
	{
		public InputDialog()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		public InputDialog(InputDialogViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void DefaultBtn_OnClick(object? sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.Close(((InputDialogViewModel)this.DataContext!).InputText);
		}

		private void CancelBtn_OnClick(object? sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.Close();
		}
	}
}
