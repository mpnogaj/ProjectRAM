using System.Windows;

namespace RAMEditor.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy Options.xaml
    /// </summary>
    public partial class Options
    {
        public Options()
        {
            InitializeComponent();
            EditorBox.IsChecked = !(Settings.Default.TextEditor);
            GBackColorBox.Text = Settings.Default.MainBackground;
            GForeColorBox.Text = Settings.Default.MainBackground;
            TabBackColorBox.Text = Settings.Default.TCBackground;
            TabForeColorBox.Text = Settings.Default.TCForeground;
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.TextEditor = EditorBox.IsChecked.HasValue ? !(EditorBox.IsChecked.Value) : true;
            Settings.Default.MainBackground = GBackColorBox.Text;
            Settings.Default.MainForeground = GForeColorBox.Text;
            Settings.Default.TCBackground = TabBackColorBox.Text;
            Settings.Default.TCForeground = TabForeColorBox.Text;
            Settings.Default.Save();
            this.Close();
        }
    }
}
