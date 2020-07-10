using Common;
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
    /// Logika interakcji dla klasy ValidationRaport.xaml
    /// </summary>
    public partial class ValidationRaport : UserControl
    {
        private ValidationRaportViewModel vm;
        public ValidationRaport()
        {
            InitializeComponent();
            vm = new ValidationRaportViewModel();
            DataContext = vm;
        }

        public void SetExceptions(List<RamInterpreterException> list)
        {
            vm.Exceptions.Clear();
            if (list == null)
                return;
            foreach(var ex in list)
            {
                vm.Exceptions.Add(ex.Message);
            }
        }
    }

    public class ValidationRaportViewModel : ViewModelBase
    {
        private ObservableCollection<string> _exceptions;
        public ObservableCollection<string> Exceptions
        {
            get { return _exceptions; }
            set
            {
                _exceptions = value;
                RaisePropertyChangedEvent("Lines");
            }
        }

        public ValidationRaportViewModel()
        {
            Exceptions = new ObservableCollection<string>();
        }
    }
}
