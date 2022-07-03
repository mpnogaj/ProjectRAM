using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectRAM.Editor.ViewModels;

namespace ProjectRAM.Editor.Views
{
	public class Options : Window
	{
		public Options()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			DataContext = new SettingsViewModel();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}