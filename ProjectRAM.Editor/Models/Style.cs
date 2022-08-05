using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.IO;
using System.Linq;

namespace ProjectRAM.Editor.Models
{
	public class Style
	{
		public string FileName { get; init; } = "default.json";
		public StyleDescriptor StyleDescriptor { get; set; } = new();

		public bool Identical(Style lhs)
		{
			return StyleDescriptor.Equals(lhs.StyleDescriptor);
		}

		public void Save()
		{
			using var sr = new StreamWriter($"Styles/{FileName}");
			sr.Write(StyleDescriptor.ToJson());
			sr.Flush();
		}

		public void ApplyStyle()
		{
			var res = new ResourceDictionary();
			typeof(StyleDescriptor).GetProperties()
				.Where(pi => !pi.GetCustomAttributes(typeof(InfoAttribute), false).Any())
				.ToList()
				.ForEach(property =>
				{
					string name = property.Name;
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
		{
			var def = new Style();
			def.Save();
		}

		public override bool Equals(object? obj)
		{
			if (obj is not Style lhs)
			{
				return false;
			}

			return lhs.GetHashCode() == GetHashCode();
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(FileName);
		}

		public override string ToString()
		{
			return StyleDescriptor.Name;
		}
	}
}