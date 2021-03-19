using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.Helpers;

namespace RAMEditorMultiplatform.ViewModels
{
    public class HostViewModel : ViewModelBase
    {
        private static HostViewModel _instance;
        public static HostViewModel Instance { get => _instance; }

        private string _header;
        public string Header { get => _header; set { SetProperty(ref _header, value); } }

        private string _content;
        public string Content { get => _content; set { SetProperty(ref _content, value); } }

        private string _inputTapeString;
        public string InputTapeString { get => _inputTapeString; set { SetProperty(ref _inputTapeString, value); } }

        public HostViewModel()
        {
            _instance = this;
        }
    }
}
