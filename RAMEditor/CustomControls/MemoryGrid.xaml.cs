using Common;
using System;
using System.Windows.Controls;

namespace RAMEditor.CustomControls
{
    /// <summary>
    /// Interaction logic for MemoryGrid.xaml
    /// </summary>
    public partial class MemoryGrid : UserControl
    {
        public string Addr
        {
            get => Address.Text;
            set => Address.Text = value;
        }

        public string Val
        {
            get => Value.Text;
            set
            {
                if (value == String.Empty)
                {
                    Value.Text = "?";
                }
                else
                {
                    Value.Text = value;
                }
            }
        }

        public MemoryGrid(Cell c)
        {
            InitializeComponent();
            Addr = c.Index.ToString();
            Val = c.Value;
        }
    }
}
