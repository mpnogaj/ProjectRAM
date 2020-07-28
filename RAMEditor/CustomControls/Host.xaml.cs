using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Common;
using RAMEditor.Properties;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Logika interakcji dla klasy Host.xaml
    /// </summary>
    public partial class Host
    {
        private string _path;
        public string CodeFilePath 
        {
            get => _path;
            set
            {
                if(_path == null)
                {
                    _path = value;
                }
            } 
        }
        public Host()
        {
            InitializeComponent();
            if (Settings.Default.TextEditor)
            {
                Code.Visibility = Visibility.Visible;
                SimpleEditor.Visibility = Visibility.Collapsed;
            }
            else
            {
                Code.Visibility = Visibility.Collapsed;
                SimpleEditor.Visibility = Visibility.Visible;
            }
        }

        public Host(string path)
        {
            InitializeComponent();
            CodeFilePath = path;
            if (Settings.Default.TextEditor)
            {
                Code.Visibility = Visibility.Visible;
                SimpleEditor.Visibility = Visibility.Collapsed;
            }
            else
            {
                Code.Visibility = Visibility.Collapsed;
                SimpleEditor.Visibility = Visibility.Visible;
            }
            FillWithCode();
        }

        private void FillWithCode()
        {
            StringCollection sc = new StringCollection();
            using (StreamReader sr = new StreamReader(_path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sc.Add(line);
                }
            }
            if (Settings.Default.TextEditor)
            {
                foreach (string s in sc)
                {
                    Code.Text += s + "\n";
                }
            }
            else
            {
                SimpleEditor.vm.Lines = SimpleEditor.ConvertToCode(sc);
                SimpleEditor.UpdateLineNumber();
            }
        }

        private void Code_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Add || e.Key == Key.OemPlus) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Logic.ChangeZoom(1);
            }
            else if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Logic.ChangeZoom(-1);
            }
        }

        public StringCollection GetText()
        {
            StringCollection lines = new StringCollection();

            // lineCount may be -1 if TextBox layout info is not up-to-date.
            int lineCount = Code.LineCount;

            for (int line = 0; line < lineCount; line++)
                lines.Add(Code.GetLineText(line));

            return lines;
        }
    }
}
