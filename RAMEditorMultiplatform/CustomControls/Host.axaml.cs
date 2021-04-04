using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using System;

namespace RAMEditorMultiplatform.CustomControls
{
    public class Host : UserControl
    {
        public Host()
        {
            InitializeComponent();
        }

        private void Fixer(object sender, KeyEventArgs e)
        {
            //TextBox tb = sender as TextBox;
            //if(tb.Text.EndsWith(Environment.NewLine))
            //{
            //    tb.Text += " ";
            //}
            //if(tb.Text.Length - 1 >= 0 && tb.Text.Substring(0, tb.Text.Length - 1).EndsWith(Environment.NewLine + " "))
            //{
            //    char typedChar = tb.Text[tb.Text.Length - 1];
            //    tb.Text = tb.Text.Substring(0, tb.Text.Length - 2) + typedChar;
            //}
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
