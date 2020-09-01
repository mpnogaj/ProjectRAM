using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Logika interakcji dla klasy ValidationRaport.xaml
    /// </summary>
    public partial class ValidationRaport : UserControl
    {
        private readonly ValidationRaportViewModel vm;
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
            {
                return;
            }

            foreach (var ex in list)
            {
                vm.Exceptions.Add(ex.Message);
            }
        }
    }

    public class ValidationRaportViewModel : INotifyPropertyChanged
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
