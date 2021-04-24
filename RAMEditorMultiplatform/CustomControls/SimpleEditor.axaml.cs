using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RAMEditorMultiplatform.Models;
using RAMEditorMultiplatform.ViewModels;
using System;
using System.Linq;

namespace RAMEditorMultiplatform.CustomControls
{
    public class SimpleEditor : UserControl
    {
        public static readonly AttachedProperty<SimpleEditorViewModel> ViewModelProperty = 
            AvaloniaProperty.RegisterAttached<SimpleEditor, Control, SimpleEditorViewModel>("ViewModel", null, false, BindingMode.OneTime);

        public static void SetViewModel(AvaloniaObject element, SimpleEditorViewModel value) 
        {
            element.SetValue(ViewModelProperty, value);
        }

        public static SimpleEditorViewModel GetViewModel(AvaloniaObject element)
        {
            return element.GetValue(ViewModelProperty);
        }

        private void Loaded(object sender, EventArgs e)
        {
            DataContext = GetValue(ViewModelProperty);
        }


        //Move to ViewModel
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Insert)
            {
                GetValue(ViewModelProperty).InsertLine(((DataGrid)sender).SelectedIndex + 1);
                ((DataGrid)sender).SelectedIndex++;
                e.Handled = true;
            }
            else if(e.Key == Key.Delete)
            {
                if(!(FocusManager.Instance.Current is TextBox))
                {
                    ProgramLine? programLine = ((DataGrid)sender).SelectedItem as ProgramLine;
                    int selectedIndex = ((DataGrid)sender).SelectedIndex;
                    if (programLine != null) 
                    {
                        GetValue(ViewModelProperty).DeleteLine(programLine);
                        ((DataGrid)sender).SelectedIndex = selectedIndex == 0 ? 0 : selectedIndex - 1;
                    }
                }
            }
        }

        public SimpleEditor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
