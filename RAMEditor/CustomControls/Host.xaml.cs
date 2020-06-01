using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logika interakcji dla klasy Host.xaml
    /// </summary>
    public partial class Host : UserControl
    {
        private string _path;
        public string CodeFilePath 
        {
            get => _path;
            set
            {
                if(_path == string.Empty)
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
                    code.Text += line + "\n";
                }
            }
        }
    }
}
