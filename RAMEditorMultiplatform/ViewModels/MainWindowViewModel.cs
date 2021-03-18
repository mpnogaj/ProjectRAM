using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.Models;
using RAMEditorMultiplatform.Helpers;

namespace RAMEditorMultiplatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string DEFAULT_HEADER { get => "NEW RAMCode"; }

        private static MainWindowViewModel _instance;
        public static MainWindowViewModel Instance { get => _instance; }

        private ObservableCollection<TabPageModel> _pages;
        public ObservableCollection<TabPageModel> Pages
        {
            get => _pages;
            set { SetProperty(ref _pages, value); }
        }

        private TabPageModel _page;
        public TabPageModel Page
        {
            get => _page;
            set { SetProperty(ref _page, value); }
        }

        private ParameterBaseCommand<string> _addPage = new(Logic.Logic.CreateNewPage, null);
        public ParameterBaseCommand<string> AddPageCommand { get => _addPage; }

        public MainWindowViewModel()
        {
            _instance = this;
            _pages = new ObservableCollection<TabPageModel>();
            _page = null;

        }
    }
}
