using Common;
using RAMEditor.Logic;
using RAMEditor.Properties;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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
                if (_path == null)
                {
                    _path = value;
                }
            }
        }

        private bool _isProgramRunning;
        public bool IsProgramRunning
        {
            get => _isProgramRunning;
            set
            {
                _isProgramRunning = value;
                var mw = Logic.Logic.GetMainWindow();
                mw.runBtn.IsEnabled = !value;
                mw.verifyBtn.IsEnabled = !value;
                mw.stopBtn.IsEnabled = value;
                mw.runMi.IsEnabled = !value;
                mw.verifyMi.IsEnabled = !value;
                mw.stopMi.IsEnabled = value;
            }
        }

        public Host()
        {
            InitializeComponent();
            BasicSetup();
        }

        public Host(string path)
        {
            InitializeComponent();
            CodeFilePath = path;
            BasicSetup();
            FillWithCode();
        }

        private void BasicSetup()
        {
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
            IsProgramRunning = false;
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
                SimpleEditor.Lines = new ObservableCollection<Command>(Creator.CreateCommandList(sc));
                SimpleEditor.UpdateLineNumber();
            }
        }

        private void Code_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Add || e.Key == Key.OemPlus) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Logic.Logic.ChangeZoom(1);
            }
            else if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Logic.Logic.ChangeZoom(-1);
            }
        }

        public StringCollection GetText()
        {
            StringCollection lines = new StringCollection();

            // lineCount may be -1 if TextBox layout info is not up-to-date.
            int lineCount = Code.LineCount;

            for (int line = 0; line < lineCount; line++)
            {
                lines.Add(Code.GetLineText(line));
            }

            return lines;
        }

        public void FixInputTapeFormat()
        {
            InputTape.Text = new String
                (InputTape.Text
                .Where(x => (char.IsDigit(x) || char.IsWhiteSpace(x) || x == '-'))
                .ToArray());
            //Usuń nadmiarowe spacje
            InputTape.Text = Regex.Replace(InputTape.Text, @"\s+", " ");
            //Usuń spacje po minusach
            InputTape.Text = Regex.Replace(InputTape.Text, @"-\s*", "-");
            InputTape.Text = InputTape.Text.Trim();
        }

        private void InputTape_LostFocus(object sender, RoutedEventArgs e)
        {
            FixInputTapeFormat();
        }
    }
}
