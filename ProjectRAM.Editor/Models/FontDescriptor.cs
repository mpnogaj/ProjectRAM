using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;
using AFontFamily = Avalonia.Media.FontFamily;
using AFontWeight = Avalonia.Media.FontWeight;
using AFontStyle = Avalonia.Media.FontStyle;


namespace ProjectRAM.Editor.Models
{
	public class FontDescriptor
	{
		public string FontFamily { get; init; } = AFontFamily.DefaultFontFamilyName;
		public AFontWeight FontWeight { get; init; } = AFontWeight.Normal;
		public AFontStyle FontStyle { get; init; } = AFontStyle.Normal;
		public double FontSize { get; init; } = 12;
		public string Foreground { get; init; } = "Black";

		public void ApplyFontStyle(IResourceDictionary res, [CallerMemberName]string target = "")
		{
			res[$"{target}FontFamily"] = FontFamily == string.Empty ? AFontFamily.Default : new AFontFamily(FontFamily);
			res[$"{target}FontStyle"] = FontStyle;
			res[$"{target}FontSize"] = FontSize;
			res[$"{target}FontWeight"] = FontWeight;
			res[$"{target}Foreground"] = new SolidColorBrush(Color.Parse(Foreground));
		}

		public override bool Equals(object? obj)
		{
			if (obj is not FontDescriptor lhs)
			{
				return false;
			}
			return obj.GetHashCode() == this.GetHashCode();
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(FontFamily, (int)FontWeight, (int)FontStyle, FontSize, Foreground);
		}
	}
}