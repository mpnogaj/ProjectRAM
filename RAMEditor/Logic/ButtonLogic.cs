using Common;
using Microsoft.Win32;
using RAMEditor.CustomControls;
using RAMEditor.Helpers;
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
        private static void CloseTab_Click(object sender, RoutedEventArgs e) =>
            Logic.CloseTab(Logic.GetTabItemFromMenuItem(sender as MenuItem));

        public static RoutedEventHandler HideBottomTabControlClick => HideBottomTabControl_Click;
        private static void HideBottomTabControl_Click(object sender, RoutedEventArgs e) =>
            Logic.HideBottomDock();

        public static readonly CommandBase CloseTab = new CommandBase(Logic.CloseTab, FileNeeded);
        public static readonly CommandBase AboutClick = new CommandBase(Logic.ShowAboutWindow, null);
        public static readonly CommandBase OptionsClick = new CommandBase(Logic.ShowOptionsWindow, null);
        #endregion

        #region File Page
        public static readonly CommandBase NewFileClick = new CommandBase(() =>
        {
            Logic.CreateTabPage();
        }, null);

        public static readonly CommandBase OpenFileClick = new CommandBase(() =>
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog(
                "Select RAM Code to open",
                "RAM Code files (*.RAMCode)|*.RAMCode");
            if (ofd.ShowDialog() == true)
            {
                Logic.CreateTabPage(
                    Path.GetFileNameWithoutExtension(ofd.FileName),
                    ofd.FileName);
            }
        }, null);

        public static readonly CommandBase SaveFileClick = new CommandBase(() =>
        {
            var host = Logic.GetHost();
            if (host.CodeFilePath == null)
            {
                //Nie trzeba sprawdzać bo już zostało to sprawdone
                //przy wywołaniu tej metody
                SaveFileAsClick.Execute(null);
                return;
            }
            Save(host, host.CodeFilePath);
        }, FileNeeded);

        public static readonly CommandBase SaveFileAsClick = new CommandBase(() =>
        {
            Host h = Logic.GetHost();
            TabItem ti = Logic.GetMainWindow().Files.SelectedItem as TabItem;
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog(
                "Save RAM Code",
                "RAM Code files (*.RAMCode)|*.RAMCode");
            if (sfd.ShowDialog() != true)
            {
                return;
            }
            Save(h, sfd.FileName);
            Logic.ChangeHeaderPage(ti, Path.GetFileNameWithoutExtension(sfd.FileName));
            h.CodeFilePath = sfd.FileName;
        }, FileNeeded);

        public static readonly CommandBase ExitClick = new CommandBase(Logic.Exit, null);
        #endregion

        #region View Page
        public static readonly CommandBase ZoomInClick = new CommandBase(() =>
        {
            Logic.ChangeZoom(1);
        }, FileNeeded);

        public static readonly CommandBase ZoomOutClick = new CommandBase(() =>
        {
            Logic.ChangeZoom(-1);
        }, FileNeeded);

        public static readonly CommandBase SwitchEditorsClick = new CommandBase(() =>
        {
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
                //var codeLines = se.ConvertToCode(Logic.GetStringCollectionFromTextEditor(code));
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
        }, FileNeeded);
        #endregion

        #region Program Page
        public static readonly CommandBase VerifyClick = new CommandBase(() =>
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
        },
            () => FileNeeded() && !Logic.GetHost().IsProgramRunning);
        public static readonly AsyncCommandBase RunClick = new AsyncCommandBase(async () =>
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
                {
                    _tokenSource.Cancel();
                }
            }
            finally
            {
                parent.IsProgramRunning = false;
            }
        },
            () => FileNeeded() && !Logic.GetHost().IsProgramRunning);
        public static readonly CommandBase CancelClick = new CommandBase(() => _tokenSource?.Cancel(),
            () => FileNeeded() && Logic.GetHost().IsProgramRunning);
        public static readonly CommandBase PrintClick = new CommandBase(() =>
        {
            PrintDialog pd = new PrintDialog();
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
        }, FileNeeded);
        #endregion

        #region Data Page
        //Memory
        public static readonly CommandBase ClearMemoryClick = new CommandBase(Logic.ClearMemory, FileNeeded);
        public static readonly CommandBase MemoryExportClick = new CommandBase(() =>
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save text file", "Text file|*.*");
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            UIElementCollection memory = Logic.GetHost().Memory.Children;
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                foreach (MemoryGrid mg in memory)
                {
                    sw.WriteLine($"{mg.Addr};{mg.Val}");
                }
            }
        }, FileNeeded);
        public static readonly CommandBase MemoryImportClick = new CommandBase(() =>
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog("Select text file", "Text file|*.*");
            if (ofd.ShowDialog() != true)
            {
                return;
            }

            UIElementCollection memory = Logic.GetHost().Memory.Children;
            memory.Clear();
            using (StreamReader sr = new StreamReader(ofd.FileName))
            {
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
            }
        }, FileNeeded);
        //Input tape
        public static readonly CommandBase ClearInputTapeClick = new CommandBase(Logic.ClearInputTape, FileNeeded);
        public static readonly CommandBase InputTapeImportClick = new CommandBase(() =>
        {
            OpenFileDialog ofd = Logic.PrepareOpenFileDialog("Select text file", "Text file|*.*");
            if (ofd.ShowDialog() != true)
            {
                return;
            }

            string input = string.Empty;
            using (StreamReader sr = new StreamReader(ofd.FileName))
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
        }, FileNeeded);
        public static readonly CommandBase InputTapeExportClick = new CommandBase(() =>
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save text file", "Text file|*.*");
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            string inputTape = Logic.GetHost().InputTape.Text;
            string[] tape = inputTape.Split(' ');
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                foreach (string s in tape)
                {
                    sw.WriteLine(s);
                }
            }
        }, FileNeeded);
        //Output tape
        public static readonly CommandBase ClearOutputTapeClick = new CommandBase(Logic.ClearOutputTape, FileNeeded);
        public static readonly CommandBase OutputTapeExportClick = new CommandBase(() =>
        {
            SaveFileDialog sfd = Logic.PrepareSaveFileDialog("Save text file", "Text file|*.*");
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            string outputTape = Logic.GetHost().OutputTape.Text;
            string[] tape = outputTape.Split(' ');
            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                foreach (string s in tape)
                {
                    sw.WriteLine(s);
                }
            }
        }, FileNeeded);
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
