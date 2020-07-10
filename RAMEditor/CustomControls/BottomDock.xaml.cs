using System;
using System.Collections.Generic;
using System.Text;
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
