using Avalonia;
using Avalonia.Media;
using AFontFamily = Avalonia.Media.FontFamily;
using AFontWeight = Avalonia.Media.FontWeight;


namespace ProjectRAM.Editor.Models
{
	public class FontStyle
	{
		public string FontFamily { get; set; } = string.Empty;
		public AFontWeight FontWeight { get; set; } = AFontWeight.Normal;
		public double FontSize { get; set; } = 12;
		public string Foreground { get; set; } = "Black";

		public void ApplyFontStyle(string target)
		{
			var res = Application.Current.Resources;
			res[$"{target}FontFamily"] = FontFamily == string.Empty ? AFontFamily.Default : new AFontFamily(FontFamily);
			res[$"{target}FontSize"] = FontSize;
			res[$"{target}FontWeight"] = FontWeight;
			res[$"{target}Foreground"] = new SolidColorBrush(Color.Parse(Foreground));
		}
	}
}