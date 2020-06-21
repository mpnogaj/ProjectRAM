using System.Windows;
using System.Windows.Media;

namespace RAMEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Logic.CreateTabPage("NEWRamCode");
        }
    }
}
