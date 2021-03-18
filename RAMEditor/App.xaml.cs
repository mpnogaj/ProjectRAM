using RAMEditor.Properties;
using System;
using System.Windows;
using System.IO;
using System.Threading.Tasks;

namespace RAMEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ResourceDictionary lang;
        private static StreamWriter logger;
        const string DATE_FORMAT = "dd.MM.yy_HH.mm.ss";

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            DiscordPresence.DiscordRPC.Shutdown();
            Log("Shutting down");
            logger.Close();
            base.OnExit(e);
        }

        public static void Log(string msg) => logger.WriteLine($"{DateTime.Now}: {msg}");

        public static string String(string key) => lang[key] as string;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }
            logger = new StreamWriter(@$"logs/RAMEditor_{DateTime.Now.ToString(DATE_FORMAT)}.txt");
            Log("Application started");
            //Set language
            var rd = new ResourceDictionary();
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
            Log($"Selected language: {source.Substring(source.Length - 10, 5)}");
            this.Resources.MergedDictionaries.Add(rd);

            Settings.Default.SettingsLoaded += (sender, e) => Log("Settings loaded");
            Settings.Default.SettingsSaving += (sender, e) => Log("Settings saved");

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => LogExceptionAndExit(e.ExceptionObject as Exception);
            this.DispatcherUnhandledException += (sender, e) => LogExceptionAndExit(e.Exception);
            TaskScheduler.UnobservedTaskException += (sender, e) => LogExceptionAndExit(e.Exception);
        }

        public static void LogExceptionAndExit(Exception ex)
        {
            LogException(ex);
            App.Log("Fatal error!");
            Current.Shutdown();
        }

        public static void LogException(Exception ex)
        {
            string day = DateTime.Now.ToString(DATE_FORMAT);
            Log($"An unhandled exception occured. Message: {ex.Message}. Stack trace saved to file: RAMEditor_StackTrace_{day}.txt");
            using var sw = new StreamWriter(@$"logs/RAMEditor_StackTrace_{day}.txt");
            sw.Write(ex.StackTrace);
            sw.Close();
        }
    }
}
