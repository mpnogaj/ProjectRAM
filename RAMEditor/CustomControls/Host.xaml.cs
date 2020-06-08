using System.Collections.Specialized;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

using Common;

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
        }

        public Host(string path)
        {
            InitializeComponent();
            CodeFilePath = path;
            FillWithCode();
        }

        private void FillWithCode()
        {
            using(StreamReader sr = new StreamReader(_path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Code.Text += line + "\n";
                }
            }
        }

        private void Code_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Add || e.Key == Key.OemPlus) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Logic.ChangeZoom(Code, 1);
            }
            else if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Logic.ChangeZoom(Code, -1);
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
