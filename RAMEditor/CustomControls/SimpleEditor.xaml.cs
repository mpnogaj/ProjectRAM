using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Interaction logic for SimpleEditor.xaml
    /// </summary>
    public partial class SimpleEditor : UserControl
    {
        public ObservableCollection<CodeLine> Lines { get; set; }

        public SimpleEditor()
        {
            InitializeComponent();
            Lines = new ObservableCollection<CodeLine>();
            DataContext = this;
            Lines.Add(new CodeLine());
        }

        public SimpleEditor(string path)
        {

        }

        private void Editor_OnKeyUp(object sender, KeyEventArgs e)
        {
            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                e.Handled = true;
                Lines.Insert(Editor.SelectedIndex + 1, new CodeLine());
            }
        }

        private void Editor_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (Lines.Count == 0)
            {
                Lines.Add(new CodeLine());
            }
        }
    }

    public class CodeLine
    {
        public string Label { get; set; }
        public string Command { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
    }

    public class CommandList : List<string>
    {
        public CommandList()
        {
            this.Add("load");
            this.Add("store");
            this.Add("read");
            this.Add("write");
            this.Add("add");
            this.Add("sub");
            this.Add("mult");
            this.Add("div");
            this.Add("load");
            this.Add("jump");
            this.Add("jzero");
            this.Add("jgtz");
            this.Add("halt");
        }
    }
}
