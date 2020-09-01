using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Logika interakcji dla klasy BottomDock.xaml
    /// </summary>
    public partial class BottomDock : UserControl
    {
        public BottomDock()
        {
            InitializeComponent();
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("test");
        }
    }
}
