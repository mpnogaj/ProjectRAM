using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Logika interakcji dla klasy ComplexityReport.xaml
    /// </summary>
    public partial class ComplexityReport : UserControl
    {
        public ComplexityReport()
        {
            InitializeComponent();
        }

        public void ResetData()
        {
            UniformTime.Text = "0";
            UniformMemory.Text = "0";
        }

        public void UpdateData(List<Command> commands, Dictionary<string, string> memory)
        {
            UniformTime.Text = Complexity.CountUniformTimeComplexity().ToString();
            UniformMemory.Text = Complexity.CountUniformMemoryComplexity().ToString();
            LogarythmicTime.Text = Complexity.CountLogarithmicTimeComplexity().ToString();
            LogarythmicMemory.Text = Complexity.CountLogarithmicMemoryCoplexity().ToString();
        }
    }
}
