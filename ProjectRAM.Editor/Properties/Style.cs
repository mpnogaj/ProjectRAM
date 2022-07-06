using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Media;
using ProjectRAM.Editor.Models;
using ProjectRAM.Editor.ViewModels;

namespace ProjectRAM.Editor.Properties
{
	public class Style
	{
		#region Constants

		private const string White = "#FFFFFF";
		private const string Black = "#000000";
		private const string LightGray = "#D3D3D3";
		private const string Gray = "#808080";
		private const string SuperLightGray = "#E6E6E6";

		#endregion

		#region Info

		[JsonIgnore] public string FileName { get; set; } = "default.json";
		public string Name { get; set; } = "default";

		#endregion

		#region Methods

		public string ToJson()
		{
			return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
		}

		public void Save()
		{
			using var sr = new StreamWriter($"Styles/{FileName}");
			sr.Write(ToJson());
			sr.Flush();
		}

		public void ApplyStyle()
		{
			var res = Application.Current.Resources;
			var properties = GetType().GetProperties();
			foreach (var property in properties)
			{
				string name = property.Name;
				object value = property.GetValue(this)!;
				if (property.PropertyType == typeof(FontDescriptor))
					((FontDescriptor)value).ApplyFontStyle(name);
				else if (name != nameof(FileName) && name != nameof(Name))
					res[property.Name] = new SolidColorBrush(Color.Parse((string)value));
			}
		}

		public void ChangeFontSizes(double val)
		{
			TextEditor.FontSize += val;
			SimpleEditor.FontSize += val;
			TextEditor.ApplyFontStyle(nameof(TextEditor));
			SimpleEditor.ApplyFontStyle(nameof(SimpleEditor));
		}

		public static void CreateDefaultAndSave()
		{
			var def = new Style();
			def.Save();
		}

		#endregion

		#region General

		public string HostBackground { get; set; } = White;

		public string Background { get; set; } = White;

		public string SideBarBackground { get; set; } = White;

		public string BottomBarBackground { get; set; } = White;



		public FontDescriptor NormalText { get; set; } = new();

		public FontDescriptor HeaderText { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};


		#endregion

		#region Data grid

		public FontDescriptor DataGridHeader { get; set; } = new()
		{
			FontSize = 15.0,
			FontWeight = FontWeight.Bold
		};

		public string DataGridHeaderBackground { get; set; } = White;
		public string DataGridRowBackground { get; set; } = White;
		public string DataGridAlternativeRowBackground { get; set; } = White;
		public string DataGridBackground { get; set; } = White;

		#endregion

		#region Text editor

		public FontDescriptor TextEditor { get; set; } = new()
		{
			FontFamily = "Courier New"
		};

		public string TextEditorBackground { get; set; } = White;

		#endregion

		#region Simple editor

		public FontDescriptor SimpleEditor { get; set; } = new()
		{
			FontFamily = "Courier New"
		};

		public FontDescriptor SimpleEditorHeader { get; set; } = new()
		{
			FontWeight = FontWeight.Bold
		};

		public string SimpleEditorHeaderBackground { get; set; } = White;
		public string SimpleEditorBackground { get; set; } = White;
		public string SimpleEditorRowBackground { get; set; } = White;

		#endregion

		#region Menu

		public string MenuFlyoutItemBackgroundPointerOver { get; set; } = SuperLightGray;
		public string MenuFlyoutItemBackgroundPressed { get; set; } = SuperLightGray;
		public string MenuFlyoutPresenterBackground { get; set; } = White;
		public string MenuFlyoutItemBackgroundDisabled { get; set; } = SuperLightGray;
		public FontDescriptor MenuFlyoutItem { get; set; } = new();
		public FontDescriptor MenuFlyoutItemPointerOver { get; set; } = new();
		public FontDescriptor MenuFlyoutItemPressed { get; set; } = new();
		public FontDescriptor MenuFlyoutItemDisabled { get; set; } = new();
		public FontDescriptor MenuFlyoutItemGestureText { get; set; } = new();
		public FontDescriptor MenuFlyoutItemGestureTextPointerOver { get; set; } = new();
		public FontDescriptor MenuFlyoutItemGestureTextPressed { get; set; } = new();
		public FontDescriptor MenuFlyoutItemGestureTextDisabled { get; set; } = new();

		#endregion

		#region Tapes

		public string InputTapeBackground { get; set; } = White;
		public string OutputTapeBackground { get; set; } = White;

		public FontDescriptor InputTape { get; set; } = new();
		public FontDescriptor OutputTape { get; set; } = new();

		#endregion

		#region GridSplitters

		public string GridSplitterBackground { get; set; } = LightGray;
		public string GridSplitterPointerOverBackground { get; set; } = LightGray;
		public string GridSplitterPressedBackground { get; set; } = Gray;

		#endregion

		#region Overrides

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != typeof(Style)) return false;
			Style lhs = (Style)obj;
			return lhs.FileName == FileName;
		}

		public override int GetHashCode()
		{
			var hashCode = new HashCode();
			var properties = GetType().GetProperties();
			foreach (var property in properties)
			{
				object value = property.GetValue(this)!;
				hashCode.Add(value);
			}
			return hashCode.ToHashCode();
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion
	}
}