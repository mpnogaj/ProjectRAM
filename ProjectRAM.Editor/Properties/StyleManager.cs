using ProjectRAM.Editor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProjectRAM.Editor.Properties
{
	public static class StyleManager
	{
		public const string DefaultFileName = "default.json";

		private static readonly SortedList<string, Style> Styles = new();
		public static Style Default => Styles.Values[Styles.IndexOfKey(DefaultFileName)];
		public static int Count => Styles.Count;

		public static ReadOnlyCollection<Style> GetStyles() => new(Styles.Values);

		public static IEnumerable<Style> GetCopyOfStyles()
		{
			List<Style> styles = new(Count);
			styles.AddRange(Styles.Values.Select(style => (Style)style.Clone()));
			return styles.AsEnumerable();
		}

		public static Style? GetStyle(string fileName)
		{
			var index = Styles.IndexOfKey(fileName);
			return index == -1 ? null : Styles.Values[index];
		}

		public static void Init()
		{
			Directory.EnumerateFiles("Styles/").ToList().ForEach(file =>
			{
				if (Path.GetExtension(file) != ".json")
				{
					return;
				}
				var content = File.ReadAllText(file);
				if (string.IsNullOrEmpty(content))
				{
					return;
				}
				try
				{
					var styleDescriptor = JsonSerializer.Deserialize<StyleDescriptor>(content);
					if (styleDescriptor == null)
					{
						return;
					}
					Styles.Add(Path.GetFileName(file), new Style
					{
						FileName = Path.GetFileName(file),
						StyleDescriptor = styleDescriptor
					});
				}
				catch
				{
					// ignored
				}
			});
		}

		/// <summary>
		/// Updates style in collection. Styles are matched by theirs file names.
		/// </summary>
		/// <param name="style">New version of the style.</param>
		public static void UpdateStyle(Style style)
		{
			if (!Styles.TryGetValue(style.FileName, out var myStyle))
			{
				myStyle = (Style)style.Clone();
				Styles.Add(style.FileName, myStyle);
			}
			else
			{
				myStyle.CopyValues(style);
			}
			myStyle.Save();
		}
	}
}
