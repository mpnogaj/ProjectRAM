using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Interaction logic for SimpleEditor.xaml
    /// </summary>
    public partial class SimpleEditor : INotifyPropertyChanged
    {
        private readonly List<string> _availableCommands = new List<string>
        {
            "load",
            "store",
            "read",
            "write",
            "add",
            "sub",
            "mult",
            "div",
            "load",
            "jump",
            "jzero",
            "jgtz",
            "halt"
        };
        public List<string> AvailableCommands
        {
            get => _availableCommands;
        }

        private ObservableCollection<Command> _lines;
        public ObservableCollection<Command> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                RaisePropertyChangedEvent("Lines");
            }
        }

        public SimpleEditor()
        {
            InitializeComponent();
            DataContext = this;
            Lines = new ObservableCollection<Command>();
            Lines.Add(new Command
            {
                Line = 1
            });
        }

        private void Editor_OnKeyUp(object sender, KeyEventArgs e)
        {
            var uiElement = e.OriginalSource as UIElement;
            //adds new line after pressing enter key
            if (e.Key == Key.Enter && uiElement != null)
            {
                e.Handled = true;
                Lines.Insert(Editor.Items.IndexOf(Editor.CurrentCell.Item) + 1, new Command());
                UpdateLineNumber();
            }
            else if (e.Key == Key.Delete)
            {
                //sprawdzic czy zaznaczony numer lini lub tekst jest pusty lub combobox jest showany i pusty
                //if (!(uiElement is DataGridCell)) return;
                try
                {
                    if (((ComboBox)uiElement).IsDropDownOpen)
                    {
                        return;
                    }

                    Lines.RemoveAt(Editor.Items.IndexOf(Editor.CurrentCell.Item));
                }
                catch
                {
                    try
                    {
                        if (((TextBox)uiElement).Text != string.Empty)
                        {
                            return;
                        }

                        Lines.RemoveAt(Editor.Items.IndexOf(Editor.CurrentCell.Item));
                    }
                    catch { Lines.RemoveAt(Editor.Items.IndexOf(Editor.CurrentCell.Item)); }
                }
            }
        }

        //prevent deleting the las row
        private void Editor_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (Lines.Count == 0)
            {
                Lines.Add(new Command
                {
                    Line = 1
                });
            }
            UpdateLineNumber();
        }

        public void UpdateLineNumber()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                Lines[i].Line = i + 1;
            }
        }

        private void Editor_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                Editor.BeginEdit(e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }
    }
}
