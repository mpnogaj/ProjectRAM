using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AFontFamily = Avalonia.Media.FontFamily;
using AFontWeight = Avalonia.Media.FontWeight;
using AFontStyle = Avalonia.Media.FontStyle;


namespace ProjectRAM.Editor.Models
{
	public class FontDescriptor
	{
		public string FontFamily { get; set; } = AFontFamily.DefaultFontFamilyName;
		public AFontWeight FontWeight { get; set; } = AFontWeight.Normal;
		public AFontStyle FontStyle { get; set; } = AFontStyle.Normal;
		public double FontSize { get; set; } = 12;
		public string Foreground { get; set; } = "Black";

		public void ApplyFontStyle(string target, IResourceDictionary res)
		{
			res[$"{target}FontFamily"] = FontFamily == string.Empty ? AFontFamily.Default : new AFontFamily(FontFamily);
			res[$"{target}FontStyle"] = FontStyle;
			res[$"{target}FontSize"] = FontSize;
			res[$"{target}FontWeight"] = FontWeight;
			res[$"{target}Foreground"] = new SolidColorBrush(Color.Parse(Foreground));
		}
	}
}