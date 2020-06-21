using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

using Common;

using RAMEditor.CustomControls;
using System.Security.Policy;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Numerics;
using System.Net;

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
        public static RoutedEventHandler OptionsClick => Options_Click;
        public static RoutedEventHandler UndoClick => Undo_Click;
        public static RoutedEventHandler RedoClick => Redo_Click;
        public static RoutedEventHandler VerifyClick => Verify_Click;
        public static RoutedEventHandler RunClick => Run_Click;
        //Memory
        public static RoutedEventHandler ClearMemoryClick => ClearMemory_Click;
        public static RoutedEventHandler MemoryExportClick => MemoryExport_Click;
        public static RoutedEventHandler MemoryImportClick => MemoryImport_Click;
        //Input tape
        public static RoutedEventHandler ClearInputTapeClick => ClearInputTape_Click;
        public static RoutedEventHandler InputTapeImportClick => InputTapeImport_Click;
        public static RoutedEventHandler InputTapeExportClick => InputTapeExport_Click;
        //Output tape
        public static RoutedEventHandler ClearOutputTapeClick => ClearOutputTape_Click;
        public static RoutedEventHandler OutputTapeExportClick => OutputTapeExport_Click;

        private static void Options_Click(object sender, RoutedEventArgs e)
        {
            Logic.ShowOptionsWindow();
        }

        private static void InputTapeImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog("Select text file", "Text file|*.*");
            if (ofd.ShowDialog() != true) return;
            string input = string.Empty;
            using (StreamReader sr = new StreamReader(ofd.FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    input += $"{line} ";
                }
            }
            Logic.GetHost().InputTape.Text = input;
        }

        private static void InputTapeExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save text file", "Text file|*.*");
            if (sfd.ShowDialog() != true) return;
            string inputTape = Logic.GetHost().InputTape.Text;
            string[] tape = inputTape.Split(' ');
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                foreach (string s in tape)
                {
                    sw.WriteLine(s);
                }
            }
        }

        private static void OutputTapeExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save text file", "Text file|*.*");
            if (sfd.ShowDialog() != true) return;
            string outputTape = Logic.GetHost().OutputTape.Text;
            string[] tape = outputTape.Split(' ');
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                foreach (string s in tape)
                {
                    sw.WriteLine(s);
                }
            }
        }

        private static void MemoryImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog("Select text file", "Text file|*.*");
            if (ofd.ShowDialog() != true) return;
            UIElementCollection memory = Logic.GetHost().Memory.Children;
            memory.Clear();
            using (StreamReader sr = new StreamReader(ofd.FileName))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    string[] data = line.Split(';');
                    memory.Add(new MemoryGrid(new Cell(data[1], BigInteger.Parse(data[0]))));
                }
            }
        }

        private static void MemoryExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save text file", "Text file|*.*");
            if (sfd.ShowDialog() != true) return;
            UIElementCollection memory = Logic.GetHost().Memory.Children;
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                foreach(MemoryGrid mg in memory)
                {
                    sw.WriteLine($"{mg.Addr};{mg.Val}");
                }
            }
        }

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
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save RAM Code", "RAM Code files (*.RAMCode)|*.RAMCode");
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
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog("Select RAM Code to open", "RAM Code files (*.RAMCode)|*.RAMCode");
            if(ofd.ShowDialog() == true)
            {
                Logic.CreateTabPage(Path.GetFileNameWithoutExtension(ofd.FileName), ofd.FileName);
            }
        }
    }
}
