using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProjectRAM.Editor.Dialogs.Views
{
	public partial class YesNoDialog : Window
	{
		public YesNoDialog()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OkBtn_Clicked(object? sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.Close(true);
		}

		private void CancelBtn_Clicked(object? sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.Close(false);
		}
	}
}
