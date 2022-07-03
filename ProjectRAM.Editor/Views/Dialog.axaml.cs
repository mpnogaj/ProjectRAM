using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProjectRAM.Editor.Views
{
	public partial class Dialog : Window
	{
		public Dialog()
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

		public static bool ShowDialog()
		{
			return true;
		}
	}
}
