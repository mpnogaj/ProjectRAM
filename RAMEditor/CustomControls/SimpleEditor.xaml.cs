using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


//Źle przekształca polecenia na kod
//NAPRAWIĆ

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Interaction logic for SimpleEditor.xaml
    /// </summary>
    public partial class SimpleEditor : UserControl
    {
        /// <summary>
        /// Reference to ViewModel
        /// </summary>
        public SimpleEditorViewModel vm;

        public SimpleEditor()
        {
            InitializeComponent();
            this.DataContext = new SimpleEditorViewModel();
            vm = this.DataContext as SimpleEditorViewModel;
            vm.Lines.Add(new CodeLine
            {
                Line = 1
            });
        }

        /// <summary>
        /// Gets the "normal" code from list of CodeLine
        /// </summary>
        /// <returns>Lines of "normal" code</returns>
        public StringCollection ConvertToStringCollection()
        {
            vm.Lines.Add(new CodeLine());
            vm.Lines.RemoveAt(vm.Lines.Count - 1);
            StringCollection outCollection = new StringCollection();
            string line;
            foreach (CodeLine codeLine in vm.Lines)
            {
                line = string.Empty;
                if (!String.IsNullOrEmpty(codeLine.Label))
                    line += codeLine.Label + ": ";
                line += codeLine.Command + ' ';
                line += codeLine.Value + ' ';
                if (!String.IsNullOrEmpty(codeLine.Comment))
                    line += '#' + codeLine.Comment;

                outCollection.Add(line);
            }

            return outCollection;
        }

        /// <summary>
        /// Get list of CodeLine from "normal" code
        /// </summary>
        /// <param name="code">"normal" code</param>
        /// <returns>List of CodeLine</returns>
        public ObservableCollection<CodeLine> ConvertToCode(StringCollection code)
        {
            ObservableCollection<CodeLine> lines = new ObservableCollection<CodeLine>();
            foreach (string line in code)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string lbl = string.Empty, command = string.Empty, arg = string.Empty, comm = string.Empty;
                string word = string.Empty;
                for (int i = 0; i <= line.Length; i++)
                {
                    if (i != line.Length && line[i] != ' ')
                        word += line[i];
                    else
                    {
                        if (word == string.Empty)
                            continue;
                        if (word.EndsWith(':'))
                            lbl = word.Substring(0, word.Length - 1);
                        else if (word.StartsWith('#'))
                            comm = word.Substring(1);
                        else
                        {
                            if (command == string.Empty)
                                command = word;
                            else if (arg == string.Empty)
                                arg = word;
                            else
                                comm += ' ' + word;
                        }
                        word = string.Empty;
                    }
                }
                lines.Add(new CodeLine
                {
                    Label = lbl,
                    Command = command,
                    Value = arg,
                    Comment = comm
                });
            }

            return lines;
        }

        
        private void Editor_OnKeyUp(object sender, KeyEventArgs e)
        {
            var uiElement = e.OriginalSource as UIElement;
            //adds new line after pressing enter key
            if (e.Key == Key.Enter && uiElement != null)
            {
                e.Handled = true;
                vm.Lines.Insert(Editor.Items.IndexOf(Editor.CurrentCell.Item) + 1, new CodeLine());
                UpdateLineNumber();
            }
        }

        //prevent deleting the las row
        private void Editor_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (vm.Lines.Count == 0)
            {
                vm.Lines.Add(new CodeLine
                {
                    Line = 1
                });
            }
            UpdateLineNumber();
        }

        public void UpdateLineNumber()
        {
            for (int i = 0; i < vm.Lines.Count; i++)
            {
                vm.Lines[i].Line = i + 1;
            }
        }

        private void Editor_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                Editor.BeginEdit(e);
            }
        }
    }

    /// <summary>
    /// SimpleEditor ViewModel class
    /// </summary>
    public class SimpleEditorViewModel : ViewModelBase
    {
        private ObservableCollection<CodeLine> _lines;
        public ObservableCollection<CodeLine> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                RaisePropertyChangedEvent("Lines");
            }
        }

        public SimpleEditorViewModel()
        {
            Lines = new ObservableCollection<CodeLine>();
        }
    }

    /// <summary>
    /// Class whitch represents a CodeLine in SimpleEditor
    /// </summary>
    public class CodeLine : ViewModelBase
    {
        #region Local variables
        private int _line;
        private string _label;
        private string _command;
        private string _value;
        private string _comment;
        #endregion

        #region Properties

        public int Line
        {
            get { return _line; }
            set
            {
                _line = value;
                RaisePropertyChangedEvent("Line");
            }
        }

        public string Label 
        { 
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChangedEvent("Label");
            } 
        }

        public string Command 
        {
            get { return _command; } 
            set
            {
                _command = value;
                RaisePropertyChangedEvent("Command");
            }
        }
        public string Value 
        { 
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChangedEvent("Value");
            }
        }
        public string Comment 
        { 
            get { return _comment; } 
            set
            {
                _comment = value;
                RaisePropertyChangedEvent("Comment");
            }
        }
        #endregion

        public CodeLine()
        {
            Label = string.Empty;
            Command = string.Empty;
            Value = string.Empty;
            Comment = string.Empty;
        }
    }

    

    /// <summary>
    /// Just list of available commands
    /// </summary>
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
