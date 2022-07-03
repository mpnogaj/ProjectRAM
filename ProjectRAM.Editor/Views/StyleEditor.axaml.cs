using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectRAM.Editor.ViewModels;

namespace ProjectRAM.Editor.Views
{
	public class StyleEditor : Window
	{
		public StyleEditor()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			DataContext = new StyleEditorViewModel();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}