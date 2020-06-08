using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

using Common;

using RAMEditor.CustomControls;
using System.Security.Policy;

namespace RAMEditor
{
    public static class ButtonLogic
    {
        public static RoutedEventHandler CloseClick => Close_Click;
        public static RoutedEventHandler CloseTabClick => CloseTab_Click;
        public static RoutedEventHandler NewFileClick => NewFile_Click;
        public static RoutedEventHandler OpenFileClick => OpenFile_Click;
        public static RoutedEventHandler SaveFileClick => SaveFile_Click;
        public static RoutedEventHandler SaveAsClick => SaveAs_Click;
        public static RoutedEventHandler ZoomInClick => ZoomIn_Click;
        public static RoutedEventHandler ZoomOutClick => ZoomOut_Click;
        public static RoutedEventHandler AboutClick => About_Click;
        public static RoutedEventHandler UndoClick => Undo_Click;
        public static RoutedEventHandler RedoClick => Redo_Click;
        public static RoutedEventHandler VerifyClick => Verify_Click;
        public static RoutedEventHandler RunClick => Run_Click;
        public static RoutedEventHandler ClearMemoryClick => ClearMemory_Click;
        public static RoutedEventHandler ClearInputTapeClick => ClearInputTape_Click;
        public static RoutedEventHandler ClearOutputTapeClick => ClearOutputTape_Click;

        private static void ClearMemory_Click(object sender, RoutedEventArgs e)
        {
            Logic.ClearMemory();
        }

        private static void ClearInputTape_Click(object sender, RoutedEventArgs e)
        {
            Logic.ClearInputTape();
        }

        private static void ClearOutputTape_Click(object sender, RoutedEventArgs e)
        {
            Logic.ClearOutputTape();
        }

        private static void Run_Click(object sender, RoutedEventArgs e)
        {
            Verify_Click(sender, e);
            Logic.RunProgram(Logic.GetHost());
        }

        private static void Verify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Interpreter.CreateCommandList(Logic.GetHost().GetText());
            }
            catch
            {
                return;
            }
            if (Logic.GetHost().InputTape.Text != string.Empty)
            {
                try
                {
                    Interpreter.CreateInputTapeFromString(Logic.GetHost().InputTape.Text);
                }
                catch
                {
                    return;
                }
            }
            MessageBox.Show("Program cały");
        }

        private static void About_Click(object sender, RoutedEventArgs e)
        {
            Logic.ShowAboutWindow();
        }

        private static void Undo_Click(object sender, RoutedEventArgs e)
        {
            Logic.GetHost().Code.Undo();
        }

        private static void Redo_Click(object sender, RoutedEventArgs e)
        {
            Logic.GetHost().Code.Redo();
        }

        private static void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Logic.ChangeZoom(Logic.GetHost().Code, -1);
        }

        private static void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Logic.ChangeZoom(Logic.GetHost().Code, 1);
        }

        private static void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            Host h = Logic.GetHost();
            if (h.CodeFilePath == null)
            {
                SaveAs_Click(sender, e);
                return;
            }
            using (StreamWriter sw = new StreamWriter(h.CodeFilePath, append: false))
            {
                sw.WriteAsync(h.Code.Text);
            }
        }

        private static void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Host h = Logic.GetHost();
            TabItem ti = Logic.GetMainWindow().Files.SelectedItem as TabItem;
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Save RAM Code",
                Filter = "RAM Code files (*.RAMCode)|*.RAMCode"
            };
            if (sfd.ShowDialog() != true) return;
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                sw.WriteAsync(h.Code.Text);
            }
            Logic.ChangeHeaderPage(ti, Path.GetFileNameWithoutExtension(sfd.FileName));
            h.CodeFilePath = sfd.FileName;
        }

        private static void Close_Click(object sender, RoutedEventArgs e)
        {
            Logic.Exit();
        }
        
        private static void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            Logic.CloseTab(Logic.GetTabItemFromMenuItem(sender as MenuItem));
        }

        private static void NewFile_Click(object sender, RoutedEventArgs e)
        {
            Logic.CreateTabPage("NEW RAMCode");
        }

        private static void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Choose RAM Code to open",
                Multiselect = false,
                Filter = "RAM Code files (*.RAMCode)|*.RAMCode"
            };
            if(ofd.ShowDialog() == true)
            {
                Logic.CreateTabPage(Path.GetFileNameWithoutExtension(ofd.FileName), ofd.FileName);
            }
        }
    }
}
