using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProjectRAM.Editor.Dialogs.Views
{
	public partial class InfoDialog : Window
	{
		public InfoDialog()
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
			this.Close();
		}
	}
}
