using FontDialog;
using RAMEditor.Properties;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;
using ColorDialog = System.Windows.Forms.ColorDialog;
using MColor = System.Windows.Media.Color;

namespace RAMEditor.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy Options.xaml
    /// </summary>
    public partial class Options
    {
        private int[] _customColors = new int[16];

        public Options()
        {
            InitializeComponent();
            Array.Fill(_customColors, ColorToInt(Color.White));
            EditorBox.IsChecked = !(Settings.Default.TextEditor);
            #region Load colors
            GBackColorBox.Background = CreateBrush(Settings.Default.TCBackground);
            ItBackColorBox.Background = CreateBrush(Settings.Default.ITBackground);
            OtBackColorBox.Background = CreateBrush(Settings.Default.OTBackground);
            TbBackColorBox.Background = CreateBrush(Settings.Default.TBBackground);
            SeBackBox.Background = CreateBrush(Settings.Default.SEBack);
            Se1BackBox.Background = CreateBrush(Settings.Default.SERow1Back);
            Se2BackBox.Background = CreateBrush(Settings.Default.SERow2Back);
            Se3BackBox.Background = CreateBrush(Settings.Default.SERow3Back);
            Se4BackBox.Background = CreateBrush(Settings.Default.SERow4Back);
            Se5BackBox.Background = CreateBrush(Settings.Default.SERow5Back);
            #endregion
            #region Load font
            SetFont(TeFontBox, Settings.Default.TBFontFamily, Settings.Default.TBFontSize,
                Settings.Default.TBFontWeight, Settings.Default.TBFontColor);
            SetFont(ItFontBox, Settings.Default.ITFontFamily, Settings.Default.ITFontSize,
                Settings.Default.ITFontWeight, Settings.Default.ITFontColor);
            SetFont(OtFontBox, Settings.Default.OTFontFamily, Settings.Default.OTFontSize,
                Settings.Default.OTFontWeight, Settings.Default.OTFontColor);
            SetFont(HdFontBox, Settings.Default.HDFontFamily, Settings.Default.HDFontSize,
                Settings.Default.HDFontWeight, Settings.Default.HDFontColor);
            SetFont(Se1FontBox, Settings.Default.SE1FontFamily, Settings.Default.SE1FontSize,
                Settings.Default.SE1FontWeight, Settings.Default.SE1FontColor);
            SetFont(Se2FontBox, Settings.Default.SE2FontFamily, Settings.Default.SE2FontSize,
                Settings.Default.SE2FontWeight, Settings.Default.SE2FontColor);
            SetFont(Se3FontBox, Settings.Default.SE3FontFamily, Settings.Default.SE3FontSize,
                Settings.Default.SE3FontWeight, Settings.Default.SE3FontColor);
            SetFont(Se4FontBox, Settings.Default.SE4FontFamily, Settings.Default.SE4FontSize,
                Settings.Default.SE4FontWeight, Settings.Default.SE4FontColor);
            SetFont(Se5FontBox, Settings.Default.SE5FontFamily, Settings.Default.SE5FontSize,
                Settings.Default.SE5FontWeight, Settings.Default.SE5FontColor);
            #endregion
            string langCode = Settings.Default.Language;
            foreach (ComboBoxItem item in SelectedLanguage.Items)
            {
                if (item.Tag.ToString() == langCode)
                {
                    SelectedLanguage.SelectedItem = item;
                    break;
                }
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            #region Save colors
            Settings.Default.TextEditor = EditorBox.IsChecked.HasValue ? !(EditorBox.IsChecked.Value) : true;
            Settings.Default.TCBackground = GetStringFromBrush(GBackColorBox.Background);
            Settings.Default.ITBackground = GetStringFromBrush(ItBackColorBox.Background);
            Settings.Default.OTBackground = GetStringFromBrush(OtBackColorBox.Background);
            Settings.Default.TBBackground = GetStringFromBrush(TbBackColorBox.Background);
            Settings.Default.SEBack = GetStringFromBrush(SeBackBox.Background);
            Settings.Default.SERow1Back = GetStringFromBrush(Se1BackBox.Background);
            Settings.Default.SERow2Back = GetStringFromBrush(Se2BackBox.Background);
            Settings.Default.SERow3Back = GetStringFromBrush(Se3BackBox.Background);
            Settings.Default.SERow4Back = GetStringFromBrush(Se4BackBox.Background);
            Settings.Default.SERow5Back = GetStringFromBrush(Se5BackBox.Background);
            #endregion
            #region Save font

            Settings.Default.TBFontFamily = TeFontBox.FontFamily.Source;
            Settings.Default.TBFontSize = Convert.ToInt32(TeFontBox.FontSize);
            Settings.Default.TBFontWeight = TeFontBox.FontWeight.ToString();
            Settings.Default.TBFontColor = GetStringFromBrush(TeFontBox.Foreground);

            Settings.Default.ITFontFamily = ItFontBox.FontFamily.Source;
            Settings.Default.ITFontSize = Convert.ToInt32(ItFontBox.FontSize);
            Settings.Default.ITFontWeight = ItFontBox.FontWeight.ToString();
            Settings.Default.ITFontColor = GetStringFromBrush(ItFontBox.Foreground);

            Settings.Default.OTFontFamily = OtFontBox.FontFamily.Source;
            Settings.Default.OTFontSize = Convert.ToInt32(OtFontBox.FontSize);
            Settings.Default.OTFontWeight = OtFontBox.FontWeight.ToString();
            Settings.Default.OTFontColor = GetStringFromBrush(OtFontBox.Foreground);

            Settings.Default.HDFontFamily = HdFontBox.FontFamily.Source;
            Settings.Default.HDFontSize = Convert.ToInt32(HdFontBox.FontSize);
            Settings.Default.HDFontWeight = HdFontBox.FontWeight.ToString();
            Settings.Default.HDFontColor = GetStringFromBrush(HdFontBox.Foreground);

            Settings.Default.SE1FontFamily = Se1FontBox.FontFamily.Source;
            Settings.Default.SE1FontSize = Convert.ToInt32(Se1FontBox.FontSize);
            Settings.Default.SE1FontWeight = Se1FontBox.FontWeight.ToString();
            Settings.Default.SE1FontColor = GetStringFromBrush(Se1FontBox.Foreground);

            Settings.Default.SE2FontFamily = Se2FontBox.FontFamily.Source;
            Settings.Default.SE2FontSize = Convert.ToInt32(Se2FontBox.FontSize);
            Settings.Default.SE2FontWeight = Se2FontBox.FontWeight.ToString();
            Settings.Default.SE2FontColor = GetStringFromBrush(Se2FontBox.Foreground);

            Settings.Default.SE3FontFamily = Se3FontBox.FontFamily.Source;
            Settings.Default.SE3FontSize = Convert.ToInt32(Se3FontBox.FontSize);
            Settings.Default.SE3FontWeight = Se3FontBox.FontWeight.ToString();
            Settings.Default.SE3FontColor = GetStringFromBrush(Se3FontBox.Foreground);

            Settings.Default.SE4FontFamily = Se4FontBox.FontFamily.Source;
            Settings.Default.SE4FontSize = Convert.ToInt32(Se4FontBox.FontSize);
            Settings.Default.SE4FontWeight = Se4FontBox.FontWeight.ToString();
            Settings.Default.SE4FontColor = GetStringFromBrush(Se4FontBox.Foreground);

            Settings.Default.SE5FontFamily = Se5FontBox.FontFamily.Source;
            Settings.Default.SE5FontSize = Convert.ToInt32(Se5FontBox.FontSize);
            Settings.Default.SE5FontWeight = Se5FontBox.FontWeight.ToString();
            Settings.Default.SE5FontColor = GetStringFromBrush(Se5FontBox.Foreground);

            #endregion
            Settings.Default.Language = ((ComboBoxItem)SelectedLanguage.SelectedItem).Tag.ToString();
            Settings.Default.Save();
            this.Close();
        }

        void SetFont(Control c, string family, int size, string weight, string color)
        {
            c.FontFamily = new FontFamily(family);
            c.FontSize = size;
            c.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(weight);
            c.Foreground = CreateBrush(color);
        }

        private SolidColorBrush CreateBrush(string color)
        {
            return new SolidColorBrush(StringToMColor(color));
        }

        private string GetStringFromBrush(Brush b)
        {
            return ToHexString(((SolidColorBrush)b).Color);
        }

        private string ToHexString(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private string ToHexString(MColor c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private int ColorToInt(Color c) => (c.R) | (c.G << 8) | (c.B << 16);

        private MColor ColorToMColor(Color c) => MColor.FromRgb(c.R, c.G, c.B);

        private Color MColorToColor(MColor c) => Color.FromArgb(c.R, c.G, c.B);

        private MColor StringToMColor(string s) => (MColor)ColorConverter.ConvertFromString(s);

        private void PickColor(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.SolidColorOnly = true;
            cd.AllowFullOpen = true;
            cd.AnyColor = true;
            cd.CustomColors = _customColors;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _customColors = cd.CustomColors;
                if (((Button)sender).Parent is StackPanel sp)
                {
                    ((Border)sp.Children[0]).Background = new SolidColorBrush(ColorToMColor(cd.Color));
                }
            }
        }

        private void PickFont(object sender, RoutedEventArgs e)
        {
            TextBox box = ((StackPanel)((Button)sender).Parent).Children[1] as TextBox;
            ColorFontDialog fd = new ColorFontDialog
            {
                Owner = this,
                Font = FontInfo.GetControlFont(box)
            };
            if (fd.ShowDialog() == true)
            {
                FontInfo.ApplyFont(box, fd.Font);
            }
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RevertToDefault_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = MessageBox.Show(
                "Are you sure you want to restore the default settings. This cannot be undone!",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (dialog == MessageBoxResult.Yes)
            {
                Settings.Default.Reset();
                Settings.Default.Save();
                this.Close();
            }
        }
    }
}
