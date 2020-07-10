﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Common;

using Microsoft.Win32;

using RAMEditor.CustomControls;


namespace RAMEditor
{
    public static class ButtonLogic
    {
        private static CancellationTokenSource _tokenSource = null;

        #region Handlers
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
        //Program
        public static RoutedEventHandler VerifyClick => Verify_Click;
        public static RoutedEventHandler RunClick => Run_Click;
        public static RoutedEventHandler CancelClick => Cancel_Click;
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
        //Bottom bar
        public static RoutedEventHandler HideBottomTabControlCick => HideBottomTabControl_Cick;
        #endregion

        private static void HideBottomTabControl_Cick(object sender, RoutedEventArgs e)
        {
            Logic.HideBottomDock();
        }

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

        private static void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if(_tokenSource != null)
                _tokenSource.Cancel();
        }

        private static async void Run_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource = new CancellationTokenSource();
            Host parent = Logic.GetHost();
            try
            {
                await Task.Run(() => { Logic.RunProgram(parent, _tokenSource.Token); });
            }
            catch (OperationCanceledException) { /*Ignore*/}
            catch (RamInterpreterException ex)
            {
                Logic.ShowErrorMessage("Error", ex.Message);
                if (_tokenSource != null)
                    _tokenSource.Cancel();
            }
        }

        private static void Verify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RamInterpreterException> ex = Logic.CheckIfValid();
                if (ex.Count == 0)
                {
                    MessageBox.Show("Wszystko dobrze");
                    Logic.GetHost().BottomDock.ValidationRaport.SetExceptions(null);
                    return;
                }
                Logic.ShowBottomDock();
                var dock = Logic.GetHost().BottomDock;
                dock.TabControl.SelectedItem = dock.ProblemsPage;
                dock.ValidationRaport.SetExceptions(ex);
            }
            catch
            {
                Logic.ShowErrorMessage("Error", "Unknown error.");
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
            Logic.ChangeZoom(-1);
        }

        private static void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Logic.ChangeZoom(1);
        }

        private static void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            Host h = Logic.GetHost();
            if (h.CodeFilePath == null)
            {
                SaveAs_Click(sender, e);
                return;
            }
            if (Logic.bUsingTextEditor())
            {
                File.WriteAllTextAsync(h.CodeFilePath, h.Code.Text);
            }
            else
            {
                File.WriteAllLines(h.CodeFilePath, h.SimpleEditor.ConvertToStringCollection().Cast<string>());
            }
        }

        private static void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Host h = Logic.GetHost();
            TabItem ti = Logic.GetMainWindow().Files.SelectedItem as TabItem;
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save RAM Code", "RAM Code files (*.RAMCode)|*.RAMCode");
            if (sfd.ShowDialog() != true) return;
            if (Logic.bUsingTextEditor())
            {
                File.WriteAllTextAsync(sfd.FileName, h.Code.Text);
            }
            else
            {
                File.WriteAllLines(sfd.FileName, h.SimpleEditor.ConvertToStringCollection().Cast<string>());
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
