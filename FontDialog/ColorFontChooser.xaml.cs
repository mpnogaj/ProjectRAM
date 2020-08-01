using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using MColor = System.Windows.Media.Color;
using OColor = System.Drawing.Color;

namespace FontDialog
{
    public partial class ColorFontChooser : System.Windows.Controls.UserControl
    {
        private int[] _colors = new int[16];

        private List<int> _defaultFontSizes = new List<int>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72, 96 };
        private List<int> _fontSizes = null;

        public List<int> FontSizes
        {
            get
            {
                return _fontSizes ?? _defaultFontSizes;
            }
            set
            {
                _fontSizes = value;
            }
        }

        public ColorFontChooser()
        {
            InitializeComponent();
            this.txtSampleText.IsReadOnly = true;
            this.lstSizes.ItemsSource = FontSizes;
        }

        public FontInfo SelectedFont
        {
            get
            {
                return new FontInfo(this.txtSampleText.FontFamily,
                                    this.txtSampleText.FontSize,
                                    this.txtSampleText.FontStyle,
                                    this.txtSampleText.FontStretch,
                                    this.txtSampleText.FontWeight,
                                    (SolidColorBrush)this.PickedColor.Background);
            }

        }

        private void colorPicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            this.txtSampleText.Foreground = this.PickedColor.Background;
        }

        private MColor ColorToMColor(OColor c) => MColor.FromRgb(c.R, c.G, c.B);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog
            {
                CustomColors = _colors,
                AllowFullOpen = true
            };
            if(cd.ShowDialog() == DialogResult.OK)
            {
                this.PickedColor.Background = new SolidColorBrush(ColorToMColor(cd.Color));
                this.txtSampleText.Foreground = this.PickedColor.Background;
            }
        }

        private void lstSizes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListBox lb = sender as System.Windows.Controls.ListBox;
            fontSizeBox.Text = lb.SelectedItem.ToString();
        }

        private void fontSizeBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if(fontSizeBox.Text.Length >= 1 && fontSizeBox.Text[0] != '0')
                    this.txtSampleText.FontSize = Convert.ToDouble(fontSizeBox.Text);
            }
            catch
            {
                this.txtSampleText.FontSize = 1.0;
            }
        }

        private void fontSizeBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
