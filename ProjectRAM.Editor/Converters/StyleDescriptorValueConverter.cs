using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ProjectRAM.Editor.Models;

namespace ProjectRAM.Editor.Converters
{
	internal class StyleDescriptorValueConverter : IMultiValueConverter
	{
		public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
		{
			var propertyName = (string)(values[0] ?? throw new ArgumentNullException(nameof(values)));
			return (string?)(typeof(StyleDescriptor).GetProperty(propertyName)!.GetValue(values[1]));
		}
	}
}
