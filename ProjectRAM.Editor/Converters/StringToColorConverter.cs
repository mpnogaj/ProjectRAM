using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ProjectRAM.Editor.Converters
{
	internal class StringToColorConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> Color.Parse((string)(value ?? throw new ArgumentNullException(nameof(value))));

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> $"#{((Color)(value ?? throw new ArgumentNullException(nameof(value)))).ToUint32():X8}";
	}
}
