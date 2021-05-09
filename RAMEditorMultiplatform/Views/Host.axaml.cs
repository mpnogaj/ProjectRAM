using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using RAMEditorMultiplatform.ViewModels;
using RAMEditorMultiplatform.Models;
using Avalonia.Controls.Templates;
using System.Collections.Generic;

namespace RAMEditorMultiplatform.Views
{
    public class Host : UserControl
    {
        private void OnKeyDown(object sender, KeyEventArgs e) => (((HostViewModel)DataContext!)!).HandleDataGridKeyEvents(sender, e);
        public Host()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
