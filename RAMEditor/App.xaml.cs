using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RAMEditor.Properties;

namespace RAMEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            base.OnExit(e);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            ResourceDictionary rd = new ResourceDictionary();
            switch (Settings.Default.Language)
            {
                case "en-GB":
                    rd.Source = new Uri("..\\Resources\\Languages\\en-GB.xaml", UriKind.Relative);
                    break;
                case "pl-PL":
                    rd.Source = new Uri("..\\Resources\\Languages\\pl-PL.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(rd);
        }
    }
}
