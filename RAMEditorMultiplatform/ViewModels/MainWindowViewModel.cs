using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.ViewModels;
using RAMEditorMultiplatform.Helpers;

namespace RAMEditorMultiplatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string DEFAULT_HEADER { get => "NEW RAMCode"; }

        private static MainWindowViewModel _instance;
        public static MainWindowViewModel Instance { get => _instance; }

        private ObservableCollection<HostViewModel> _pages;
        public ObservableCollection<HostViewModel> Pages
        {
            get => _pages;
            set { SetProperty(ref _pages, value); }
        }

        private HostViewModel _page;
        public HostViewModel Page
        {
            get => _page;
            set { SetProperty(ref _page, value); }
        }

        private readonly ParameterBaseCommand<string> _addPage = new(Logic.Logic.CreateNewPage, () => true);
        public ParameterBaseCommand<string> AddPageCommand { get => _addPage; }

        private readonly ParameterBaseCommand<HostViewModel> _closePage = new(Logic.Logic.ClosePage, () => true);
        public ParameterBaseCommand<HostViewModel> ClosePage { get => _closePage; }

        public MainWindowViewModel()
        {
            _instance = this;
            _pages = new ObservableCollection<HostViewModel>();
            _page = null;
        }
    }
}
