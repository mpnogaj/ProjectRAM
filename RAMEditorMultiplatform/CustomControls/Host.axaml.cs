using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RAMEditorMultiplatform.CustomControls
{
    public class Host : UserControl
    {
        public Host()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
