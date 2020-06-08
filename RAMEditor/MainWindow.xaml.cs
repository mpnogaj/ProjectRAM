using System.Windows;

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
