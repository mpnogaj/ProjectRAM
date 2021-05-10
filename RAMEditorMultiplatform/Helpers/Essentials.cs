using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Common;
using RAMEditorMultiplatform.Converters;
using RAMEditorMultiplatform.Models;
using RAMEditorMultiplatform.ViewModels;
using RAMEditorMultiplatform.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;

namespace RAMEditorMultiplatform.Helpers
{
    public static class Essentials
    {
        //public static MainWindow? MainWindow { get; set; }

        public static string TapeToString(Queue<string> tape)
        {
            return string.Join(", ", tape.ToArray());
        }
        
        public static void WriteToFile(string file, string content)
        {
            using StreamWriter sw = new(file);
            sw.Write(content);
        }
        
        public static string ReadFromFile(string file)
        {
            using StreamReader sr = new(file);
            return sr.ReadToEnd();
        }

        public static IClassicDesktopStyleApplicationLifetime GetAppInstance()
        {
            return (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        }

        public static void SetCursor(StandardCursorType cursor)
        {
            GetAppInstance().MainWindow.Cursor = new Cursor(cursor);
        }

        public static void Exit()
        {
            GetAppInstance().Shutdown();
        }
    }
}
