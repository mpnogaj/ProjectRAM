using Avalonia;
using Avalonia.Media;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Color = Avalonia.Media.Color;

namespace ProjectRAM.Editor.Models
{
	public class StyleDescriptor
	{
		private const string White = "#FFFFFF";
		private const string Black = "#000000";
		private const string LightGray = "#D3D3D3";
		private const string Gray = "#808080";
		private const string SuperLightGray = "#E6E6E6";
		private const string SelectionBlue = "#FF0077D7";

		[Info]
		public string Name { get; set; } = "default";

		public string ToJson()
		{
			return JsonSerializer.Serialize(this);
		}

		public void ChangeFontSizes(double val)
		{
			TextEditor = new FontDescriptor
			{
				FontFamily = TextEditor.FontFamily,
				FontStyle = TextEditor.FontStyle,
				FontWeight = TextEditor.FontWeight,
				FontSize = TextEditor.FontSize + val
			};
			SimpleEditor = new FontDescriptor
			{
				FontFamily = SimpleEditor.FontFamily,
				FontStyle = SimpleEditor.FontStyle,
				FontWeight = SimpleEditor.FontWeight,
				FontSize = SimpleEditor.FontSize + val
			};
		}

		public bool Equals(StyleDescriptor lhs)
		{
			return typeof(StyleDescriptor).GetProperties()
				.Where(pi => !pi.GetCustomAttributes(typeof(InfoAttribute), false).Any())
				.All(property => !(property.GetValue(this)?.Equals(property.GetValue(lhs)) ?? false));
		}

		private static void UpdateColor(ref string storage, string value, [CallerMemberName] string? caller = null)
		{
			if (storage == value)
			{
				return;
			}
			storage = value;
			Application.Current!.Resources[caller ?? throw new ArgumentNullException(nameof(caller))] =
				new SolidColorBrush(Color.Parse(value));
		}

		#region Visuals

		#region General

		public string HostBackground { get; set; } = White;
		public string Background { get; set; } = White;

		public string SideBarBackground { get; set; } = White;

		public string BottomBarBackground { get; set; } = White;



		public FontDescriptor NormalText { get; set; } = new();

		public FontDescriptor TextBoxText { get; set; } = new();

		#endregion
		#region Header

		public FontDescriptor HeaderText { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		public FontDescriptor HeaderTextSelected { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		public FontDescriptor HeaderTextPointerOver { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		#endregion
		#region Memory

		public FontDescriptor AddressHeader { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		public FontDescriptor Address { get; set; } = new();

		public FontDescriptor ValueHeader { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		public FontDescriptor Value { get; set; } = new();

		public string MemoryGridRowColor = White;

		#endregion
		#region Verification report

		public FontDescriptor LineHeader { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		public FontDescriptor Line { get; set; } = new();

		public FontDescriptor MessageHeader { get; set; } = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};

		public FontDescriptor Message { get; set; } = new();

		public string VerificationReportRowColor = White;

		#endregion
		#region Text editor

		public FontDescriptor TextEditor { get; set; } = new()
		{
			FontFamily = "Courier New"
		};

		private string _textEditorForeground = White;
		public string TextEditorBackground
		{
			get => _textEditorForeground;
			set => UpdateColor(ref _textEditorForeground, value);
		}

		private string _textEditorCaretColor = Black;
		public string TextEditorCaretColor
		{
			get => _textEditorCaretColor;
			set => UpdateColor(ref _textEditorCaretColor, value);
		}

		private string _textEditorSelectionColor = SelectionBlue;
		public string TextEditorSelectionColor
		{
			get => _textEditorSelectionColor;
			set => UpdateColor(ref _textEditorSelectionColor, value);
		}

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

		public string MenuFlyoutPresenterBackground { get; set; } = White;
		public string MenuFlyoutItemBackgroundPointerOver { get; set; } = SuperLightGray;
		public string MenuFlyoutItemBackgroundPressed { get; set; } = SuperLightGray;
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
		public FontDescriptor InputTapeLbl { get; set; } = new();
		public FontDescriptor OutputTape { get; set; } = new();
		public FontDescriptor OutputTapeLbl { get; set; } = new();

		#endregion
		#region GridSplitters

		public string GridSplitterBackground { get; set; } = Black;
		public string GridSplitterPointerOverBackground { get; set; } = Black;
		public string GridSplitterPressedBackground { get; set; } = Black;

		#endregion

		#endregion

	}
}
