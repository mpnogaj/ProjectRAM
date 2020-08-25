using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace RAMEditor.Helpers
{
    public class FlowDocCell : TableCell
    {
        public FlowDocCell(Block blockItem, bool isHeader) : 
            base(blockItem)
        {
            this.BorderBrush = new SolidColorBrush(Colors.Black);
            this.FontFamily = new FontFamily("Times New Roman");
            this.BorderThickness = new Thickness(1);
            this.Padding = new Thickness(4, 2, 4, 2);
            if(isHeader)
            {
                this.TextAlignment = TextAlignment.Center;
                this.FontSize = 16;
                this.FontWeight = FontWeights.Bold;
            }
        }
    }
}
