using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Avalonia.Media;

namespace ProjectRAM.Editor.Converters
{
	internal class StringToHsvColorConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> Color.Parse((string)(value ?? throw new ArgumentNullException(nameof(value)))).ToHsv();

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> $"#{((HsvColor)(value ?? throw new ArgumentNullException(nameof(value)))).ToRgb().ToUint32():X8}";
	}
}
