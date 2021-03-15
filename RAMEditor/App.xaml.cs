using RAMEditor.Properties;
using System;
using System.Windows;
using System.IO;

namespace RAMEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ResourceDictionary lang;
        private static StreamWriter logger;

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            DiscordPresence.DiscordRPC.Shutdown();
            logger.Close();
            base.OnExit(e);
        }

        public static void Log(string msg) => logger.WriteLine($"{DateTime.Now}: {msg}");

        public static string String(string key) => lang[key] as string;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            logger = new StreamWriter(@"logs/RAMEditor.txt");
            Log("Application started");
            //Set language
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
            lang = rd;
            var source = rd.Source.ToString();
            Log($"Selected language. Language: {source.Substring(source.Length - 10, 5)}");
            this.Resources.MergedDictionaries.Add(rd);
        }
    }
}
