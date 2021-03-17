using Common;
using Microsoft.Win32;
using RAMEditor.CustomControls;
using RAMEditor.Helpers;
using RAMEditor.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RAMEditor.Logic
{
    public static class ButtonLogic
    {
        #region Other
        private static CancellationTokenSource _tokenSource;

        public static RoutedEventHandler CloseTabClick => CloseTab_Click;
        public static RoutedEventHandler HideBottomTabControlClick => HideBottomTabControl_Click;

        private static void CloseTab_Click(object sender, RoutedEventArgs e) => Logic.CloseTab(Logic.GetTabItemFromMenuItem(sender as MenuItem));

        private static void HideBottomTabControl_Click(object sender, RoutedEventArgs e) => Logic.HideBottomDock();

        public static readonly CommandBase AboutClick = new(Logic.ShowAboutWindow, null);

        public static readonly CommandBase OptionsClick = new(Logic.ShowOptionsWindow, null);

        public static readonly CommandBase CloseTab = new(() => { Logic.CloseTab(); }, FileNeeded);
        #endregion

        #region File Page
        public static readonly CommandBase NewFileClick = new(() =>
        {
            Logic.CreateTabPage();
        }, null);

        public static readonly CommandBase OpenFileClick = new(() =>
        {
            App.Log("Opening file");
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog(
                App.String("openFilePrompt"),
                $"{App.String("ramCodeFile")} (*.RAMCode)|*.RAMCode");
            if (ofd.ShowDialog() == true)
            {
                Logic.CreateTabPage(
                    Path.GetFileNameWithoutExtension(ofd.FileName),
                    ofd.FileName);
            }
            App.Log("File opened");
        }, null);

        public static readonly CommandBase SaveFileClick = new(() =>
        {
            var host = Logic.GetHost();
            if (host.CodeFilePath == null)
            {
                //No need to check if any file is open
                SaveFileAsClick.Execute(null);
                return;
            }
            Save(host, host.CodeFilePath);
        }, FileNeeded);

        public static readonly CommandBase SaveFileAsClick = new(() =>
        {
            Host h = Logic.GetHost();
            TabItem ti = Logic.GetMainWindow().Files.SelectedItem as TabItem;
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog(
                App.String("saveFilePrompt"),
                $"{App.String("ramCodeFile")} (*.RAMCode)|*.RAMCode");
            if (sfd.ShowDialog() != true)
            {
                return;
            }
            Save(h, sfd.FileName);
            Logic.ChangeHeaderPage(ti, Path.GetFileNameWithoutExtension(sfd.FileName));
            h.CodeFilePath = sfd.FileName;
            App.Log($"Saved file as {sfd.FileName}, at {sfd.FileName}");
        }, FileNeeded);

        public static readonly CommandBase ExitClick = new(Logic.Exit, null);
        #endregion

        #region View Page
        public static readonly CommandBase ZoomInClick = new(() =>
        {
            Logic.ChangeZoom(1);
        }, FileNeeded);

        public static readonly CommandBase ZoomOutClick = new(() =>
        {
            Logic.ChangeZoom(-1);
        }, FileNeeded);

        public static readonly CommandBase SwitchEditorsClick = new(() =>
        {
            App.Log("Switching editors");
            var host = Logic.GetHost();
            var code = host.Code;
            var se = host.SimpleEditor;
            if (!Logic.bUsingTextEditor())
            {
                code.Text = string.Empty;
                se.Visibility = Visibility.Collapsed;
                var text = Creator.CommandListToStringCollection(new List<Command>(se.Lines));
                foreach (string line in text)
                {
                    string a = string.Empty;
                    if (line != string.Empty)
                    {
                        //remove spaces before and after
                        a = line.Trim();
                    }
                    code.Text += a + "\r\n";
                }
                //remove last \r\n
                code.Text = code.Text.Remove(code.Text.Length - 2);
                code.Visibility = Visibility.Visible;
            }
            else
            {
                var codeLines = Creator.CreateCommandList(Logic.GetStringCollectionFromTextEditor(code));
                if (codeLines.Count <= 0)
                {
                    codeLines.Add(new Command
                    {
                        Line = 1
                    });
                }
                se.Lines = new ObservableCollection<Command>(codeLines);
                se.Visibility = Visibility.Visible;
                code.Visibility = Visibility.Collapsed;
            }
            App.Log("Editors switched");
        }, FileNeeded);
        #endregion

        #region Program Page
        public static readonly CommandBase VerifyClick = new(() =>
        {
            try
            {
                List<RamInterpreterException> ex = Logic.CheckIfValid();
                if (ex.Count == 0)
                {
                    MessageBox.Show(App.String("noErrors"));
                    Logic.GetHost().BottomDock.ValidationRaport.SetExceptions(null);
                    return;
                }
                Logic.ShowBottomDock();
                var dock = Logic.GetHost().BottomDock;
                dock.TabControl.SelectedItem = dock.ProblemsPage;
                dock.ValidationRaport.SetExceptions(ex);
            }
            catch(Exception ex)
            {
                App.LogException(ex);
                Logic.ShowErrorMessage(App.String("error"), App.String("unknownError"));
            }
        }, () => FileNeeded() && !Logic.GetHost().IsProgramRunning);

        public static readonly AsyncCommandBase RunClick = new(async () =>
        {
            _tokenSource = new CancellationTokenSource();
            Host parent = Logic.GetHost();
            try
            {
                await Task.Run(() => { Logic.RunProgram(parent, _tokenSource.Token); });
            }
            catch (OperationCanceledException) { /*Ignore*/ }
            catch (RamInterpreterException ex)
            {
                Logic.ShowErrorMessage(App.String("error"), ex.LocalizedMessage(Settings.Default.Language));
                if (_tokenSource != null)
                {
                    _tokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex);
            }
            finally
            {
                parent.IsProgramRunning = false;
            }
        }, () => FileNeeded() && !Logic.GetHost().IsProgramRunning);

        public static readonly CommandBase CancelClick = new(() => _tokenSource?.Cancel(), () => FileNeeded() && Logic.GetHost().IsProgramRunning);

        public static readonly CommandBase PrintClick = new(() =>
        {
            var pd = new PrintDialog();
            App.Log("Print dialog showed");
            if (pd.ShowDialog() == true)
            {
                var doc = Logic.GetFlowDocument();
                doc.PageHeight = pd.PrintableAreaHeight;
                doc.PageWidth = pd.PrintableAreaWidth;
                doc.PagePadding = new Thickness(25);

                doc.ColumnGap = 0;

                doc.ColumnWidth = (doc.PageWidth -
                                   doc.ColumnGap -
                                   doc.PagePadding.Left -
                                   doc.PagePadding.Right);

                pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator,
                    "RAM Machine program");
            }
            App.Log("Print dialog closed");
        }, FileNeeded);
        #endregion

        #region Data Page
        #region Memory
        public static readonly CommandBase ClearMemoryClick = new(Logic.ClearMemory, FileNeeded);

        public static readonly CommandBase MemoryExportClick = new(() =>
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog(App.String("saveTextFile"), $"{App.String("textFile")}|*.*");
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            UIElementCollection memory = Logic.GetHost().Memory.Children;
            using (var sw = new StreamWriter(sfd.FileName))
            {
                foreach (MemoryGrid mg in memory)
                {
                    sw.WriteLine($"{mg.Addr};{mg.Val}");
                }
            }
            App.Log($"Memory exported to {sfd.FileName}");
        }, FileNeeded);

        //Experimental feature
        public static readonly CommandBase MemoryImportClick = new(() =>
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog(App.String("selectTextFile"), $"{App.String("textFile")}|*.*");
            if (ofd.ShowDialog() != true)
            {
                return;
            }

            UIElementCollection memory = Logic.GetHost().Memory.Children;
            memory.Clear();
            using var sr = new StreamReader(ofd.FileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line == string.Empty)
                {
                    continue;
                }

                string[] data = line.Split(';');
                memory.Add(new MemoryGrid(new Cell(data[1], data[0])));
            }
            sr.Close();
        }, FileNeeded);
        #endregion

        #region Input Tape
        public static readonly CommandBase ClearInputTapeClick = new(Logic.ClearInputTape, FileNeeded);

        public static readonly CommandBase InputTapeImportClick = new(() =>
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog(App.String("selectTextFile"), $"{App.String("textFile")}|*.*");
            if (ofd.ShowDialog() != true)
            {
                return;
            }

            string input = string.Empty;
            using (var sr = new StreamReader(ofd.FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                    {
                        continue;
                    }

                    input += $"{line} ";
                }
            }
            Logic.GetHost().InputTape.Text = input;
            App.Log($"Input tape imported from {ofd.FileName}");
        }, FileNeeded);

        public static readonly CommandBase InputTapeExportClick = new(() =>
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog(App.String("saveTextFile"), $"{App.String("textFile")}|*.*");
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            string inputTape = Logic.GetHost().InputTape.Text;
            string[] tape = inputTape.Split(' ');
            using (var sw = new StreamWriter(sfd.FileName))
            {
                foreach (string s in tape)
                {
                    sw.WriteLine(s);
                }
            }
            App.Log($"Input tape exported to {sfd.FileName}");
        }, FileNeeded);
        #endregion

        #region Output Tape
        public static readonly CommandBase ClearOutputTapeClick = new(Logic.ClearOutputTape, FileNeeded);
        public static readonly CommandBase OutputTapeExportClick = new(() =>
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog(App.String("saveTextFile"), $"{App.String("textFile")}|*.*");
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            string outputTape = Logic.GetHost().OutputTape.Text;
            string[] tape = outputTape.Split(' ');
            using (var sw = new StreamWriter(sfd.FileName))
            {
                foreach (string s in tape)
                {
                    sw.WriteLine(s);
                }
            }
            App.Log($"Output tape exported to {sfd.FileName}");
        }, FileNeeded);
        #endregion
        #endregion

        #region Helpers
        private static void Save(Host h, string path)
        {
            if (Logic.bUsingTextEditor())
            {
                File.WriteAllTextAsync(path, h.Code.Text);
            }
            else
            {

                File.WriteAllLines(path, Creator.CommandListToStringCollection
                    (Logic.GetSimpleEditorLines()).Cast<string>());
            }
            App.Log("File saved");
        }

        private static bool FileNeeded()
        {
            var files = Logic.GetMainWindow().Files;
            if (files != null)
            {
                return files.SelectedIndex != -1;
            }

            return false;
        }
        #endregion
    }
}
