using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using System;
using RAMEditorMultiplatform.ViewModels;

namespace RAMEditorMultiplatform.Views
{
    public class Host : UserControl
    {
        private void OnKeyDown(object sender, KeyEventArgs e) => ((HostViewModel)DataContext).HandleDataGridKeyEvents(sender, e);

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
