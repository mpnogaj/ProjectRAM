using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProjectRAM.Editor.Models
{
	public class Style : ICloneable
	{
		public string FileName { get; init; } = "default.json";
		public StyleDescriptor StyleDescriptor { get; set; } = new();

		public bool Identical(Style lhs) 
			=> StyleDescriptor.Equals(lhs.StyleDescriptor);

		public void Save()
		{
			using var sr = new StreamWriter($"Styles/{FileName}");
			sr.Write(StyleDescriptor.ToJson());
		}

		public void ApplyStyle()
		{
			var res = new ResourceDictionary();
			typeof(StyleDescriptor).GetProperties()
				.Where(pi => !pi.GetCustomAttributes(typeof(InfoAttribute), false).Any())
				.ToList()
				.ForEach(property =>
				{
					var name = property.Name;
					var value = property.GetValue(this.StyleDescriptor);
					switch (value)
					{
						case null:
							break;
						case FontDescriptor fontDescriptor:
							fontDescriptor.ApplyFontStyle(res, name);
							break;
						case string color:
							res[property.Name] = new SolidColorBrush(Color.Parse(color));
							break;
					}
				});
			Application.Current!.Resources = res;
		}

		public void ChangeFontSizes(double val)
		{
			StyleDescriptor.ChangeFontSizes(val);
			var res = Application.Current!.Resources;
			StyleDescriptor.TextEditor.ApplyFontStyle(res);
			StyleDescriptor.SimpleEditor.ApplyFontStyle(res);
		}

		public static void CreateDefaultAndSave() 
			=> new Style().Save();

		public override bool Equals(object? obj) 
			=> obj is Style lhs && lhs.GetHashCode() == GetHashCode();

		public override int GetHashCode()
			=> HashCode.Combine(FileName);

		public override string ToString()
			=> StyleDescriptor.Name;

		/// <summary>
		/// Create deep clone of the object. Results in new address
		/// </summary>
		/// <returns></returns>
		public object Clone()
			=> JsonSerializer.Deserialize<Style>(JsonSerializer.Serialize(this))!;

		/// <summary>
		/// Update current object. Preserves address
		/// </summary>
		/// <param name="rhs"></param>
		public void CopyValues(Style rhs)
			=> this.StyleDescriptor = (StyleDescriptor)(rhs.StyleDescriptor.Clone());
	}
}