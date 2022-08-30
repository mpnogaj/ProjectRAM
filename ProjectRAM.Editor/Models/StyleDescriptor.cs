using Avalonia;
using Avalonia.Media;
using ProjectRAM.Editor.Properties;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Color = Avalonia.Media.Color;

namespace ProjectRAM.Editor.Models
{
	public class StyleDescriptor : ICloneable
	{
		private const string White = "#FFFFFFFF";
		private const string Black = "#FF000000";
		private const string LightGray = "#FFD3D3D3";
		private const string Gray = "#FF808080";
		private const string SuperLightGray = "#FFE6E6E6";
		private const string SelectionBlue = "#FF0077D7";
		
		private string _background = White;
		private string _tabBackground = White;
		private string _sidebarBackground = White;
		private string _bottomBarBackground = White;
		private string _memoryGridRowColor = White;
		private string _textEditorSelectionColor = SelectionBlue;
		private string _verificationReportRowColor = White;
		private string _textEditorForeground = White;
		private string _textEditorCaretColor = Black;
		private string _simpleEditorHeaderBackground = White;
		private string _simpleEditorBackground = White;
		private string _simpleEditorRowBackground = White;
		private string _menuFlyoutPresenterBackground = White;
		private string _menuFlyoutItemBackgroundPointerOver = SuperLightGray;
		private string _menuFlyoutItemBackgroundPressed = SuperLightGray;
		private string _menuFlyoutItemBackgroundDisabled = SuperLightGray;
		private string _inputTapeBackground = White;
		private string _outputTapeBackground = White;
		private string _gridSplitterBackground = Black;
		private string _gridSplitterPointerOverBackground = Black;
		private string _gridSplitterPressedBackground = Black;

		private FontDescriptor _normalText = new();
		private FontDescriptor _textBoxText = new();
		private FontDescriptor _textBoxTextSelected = new();
		private FontDescriptor _textBoxTextPointerOver = new();
		private FontDescriptor _headerText = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _headerTextSelected = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _headerTextPointerOver = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _addressHeader = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _address = new();
		private FontDescriptor _valueHeader = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _value = new();
		private FontDescriptor _lineHeader = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _line = new();
		private FontDescriptor _messageHeader = new()
		{
			FontWeight = FontWeight.Bold,
			FontSize = 15.0
		};
		private FontDescriptor _message = new();
		private FontDescriptor _textEditor = new()
		{
			FontFamily = "Courier New"
		};
		private string _name = "default";
		private FontDescriptor _simpleEditor = new()
		{
			FontFamily = "Courier New"
		};
		private FontDescriptor _simpleEditorHeader = new()
		{
			FontWeight = FontWeight.Bold
		};
		private FontDescriptor _menuFlyoutItem = new();
		private FontDescriptor _menuFlyoutItemPointerOver = new();
		private FontDescriptor _menuFlyoutItemPressed = new();
		private FontDescriptor _menuFlyoutItemDisabled = new();
		private FontDescriptor _menuFlyoutItemGestureText = new();
		private FontDescriptor _menuFlyoutItemGestureTextPointerOver = new();
		private FontDescriptor _menuFlyoutItemGestureTextPressed = new();
		private FontDescriptor _menuFlyoutItemGestureTextDisabled = new();
		private FontDescriptor _inputTape = new();
		private FontDescriptor _inputTapeLbl = new();
		private FontDescriptor _outputTape = new();
		private FontDescriptor _outputTapeLbl = new();


		[Info]
		public string Name { get; set; }

		#region Methods
		public string ToJson()
			=> JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

		public bool Equals(StyleDescriptor lhs)
			=> typeof(StyleDescriptor).GetProperties()
				.Where(pi => !pi.GetCustomAttributes(typeof(InfoAttribute), false).Any())
				.All(property => !(property.GetValue(this)?.Equals(property.GetValue(lhs)) ?? false));

		public object Clone()
			=> JsonSerializer.Deserialize<StyleDescriptor>(this.ToJson())!;

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

		private static void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? caller = null)
		{
			if (caller == null)
			{
				throw new ArgumentNullException(nameof(caller));
			}

			if (storage?.Equals(value) ?? value == null)
			{
				return;
			}

			storage = value;
			switch (value)
			{
				case string hexColor:
					Application.Current!.Resources[caller ?? throw new ArgumentNullException(nameof(caller))] =
						new SolidColorBrush(Color.Parse(hexColor));
					break;
				case FontDescriptor fontDescriptor:
					fontDescriptor.ApplyFontStyle(Application.Current!.Resources, caller);
					break;
				default:
					throw new Exception("Invalid type");
			}
		}
		#endregion

		[LocalizedDisplayName(nameof(Strings.BackgroundColor))]
		public string Background
		{
			get => _background;
			set => SetProperty(ref _background, value);
		}

		[LocalizedDisplayName(nameof(Strings.TabBackgroundColor))]
		public string TabBackground
		{
			get => _tabBackground;
			set => SetProperty(ref _tabBackground, value);
		}

		[LocalizedDisplayName(nameof(Strings.SidebarBackgroundColor))]
		public string SidebarBackground
		{
			get => _sidebarBackground;
			set => SetProperty(ref _sidebarBackground, value);
		}

		[LocalizedDisplayName(nameof(Strings.BottomBarBackgroundColor))]
		public string BottomBarBackground
		{
			get => _bottomBarBackground;
			set => SetProperty(ref _bottomBarBackground, value);
		}

		[LocalizedDisplayName(nameof(Strings.MemoryGridRowColor))]
		public string MemoryGridRowColor
		{
			get => _memoryGridRowColor;
			set => SetProperty(ref _memoryGridRowColor, value);
		}

		[LocalizedDisplayName(nameof(Strings.VerificationReportRowColor))]
		public string VerificationReportRowColor
		{
			get => _verificationReportRowColor;
			set => SetProperty(ref _verificationReportRowColor, value);
		}

		[LocalizedDisplayName(nameof(Strings.TextEditorBackgroundColor))]
		public string TextEditorBackground
		{
			get => _textEditorForeground;
			set => SetProperty(ref _textEditorForeground, value);
		}

		[LocalizedDisplayName(nameof(Strings.TextEditorCaretColor))]
		public string TextEditorCaretColor
		{
			get => _textEditorCaretColor;
			set => SetProperty(ref _textEditorCaretColor, value);
		}

		[LocalizedDisplayName(nameof(Strings.TextEditorSelectionColor))]
		public string TextEditorSelectionColor
		{
			get => _textEditorSelectionColor;
			set => SetProperty(ref _textEditorSelectionColor, value);
		}

		public string SimpleEditorBackground
		{
			get => _simpleEditorBackground;
			set => SetProperty(ref _simpleEditorBackground, value);
		}

		public string SimpleEditorRowBackground
		{
			get => _simpleEditorRowBackground;
			set => SetProperty(ref _simpleEditorRowBackground, value);
		}

		public string SimpleEditorHeaderBackground
		{
			get => _simpleEditorHeaderBackground;
			set => SetProperty(ref _simpleEditorHeaderBackground, value);
		}

		public string MenuFlyoutPresenterBackground
		{
			get => _menuFlyoutPresenterBackground;
			set => SetProperty(ref _menuFlyoutPresenterBackground, value);
		}

		public string MenuFlyoutItemBackgroundPointerOver
		{
			get => _menuFlyoutItemBackgroundPointerOver;
			set => SetProperty(ref _menuFlyoutItemBackgroundPointerOver, value);
		}

		public string MenuFlyoutItemBackgroundPressed
		{
			get => _menuFlyoutItemBackgroundPressed;
			set => SetProperty(ref _menuFlyoutItemBackgroundPressed, value);
		}

		public string MenuFlyoutItemBackgroundDisabled
		{
			get => _menuFlyoutItemBackgroundDisabled;
			set => SetProperty(ref _menuFlyoutItemBackgroundDisabled, value);
		}

		public string InputTapeBackground
		{
			get => _inputTapeBackground;
			set => SetProperty(ref _inputTapeBackground, value);
		}

		public string OutputTapeBackground
		{
			get => _outputTapeBackground;
			set => SetProperty(ref _outputTapeBackground, value);
		}

		public string GridSplitterBackground
		{
			get => _gridSplitterBackground;
			set => SetProperty(ref _gridSplitterBackground, value);
		}

		public string GridSplitterPointerOverBackground
		{
			get => _gridSplitterPointerOverBackground;
			set => SetProperty(ref _gridSplitterPointerOverBackground, value);
		}

		public string GridSplitterPressedBackground
		{
			get => _gridSplitterPressedBackground;
			set => SetProperty(ref _gridSplitterPressedBackground, value);
		}


		public FontDescriptor NormalText
		{
			get => _normalText;
			set => SetProperty(ref _normalText, value);
		}

		public FontDescriptor TextBoxText
		{
			get => _textBoxText;
			set => SetProperty(ref _textBoxText, value);
		}

		public FontDescriptor TextBoxTextSelected
		{
			get => _textBoxTextSelected;
			set => SetProperty(ref _textBoxTextSelected, value);
		}

		public FontDescriptor TextBoxTextPointerOver
		{
			get => _textBoxTextPointerOver;
			set => SetProperty(ref _textBoxTextPointerOver, value);
		}

		public FontDescriptor HeaderText
		{
			get => _headerText;
			set => SetProperty(ref _headerText, value);
		}

		public FontDescriptor HeaderTextSelected
		{
			get => _headerTextSelected;
			set => SetProperty(ref _headerTextSelected, value);
		}

		public FontDescriptor HeaderTextPointerOver
		{
			get => _headerTextPointerOver;
			set => SetProperty(ref _headerTextPointerOver, value);
		}

		public FontDescriptor AddressHeader
		{
			get => _addressHeader;
			set => SetProperty(ref _addressHeader, value);
		}

		public FontDescriptor Address
		{
			get => _address;
			set => SetProperty(ref _address, value);
		}

		public FontDescriptor ValueHeader
		{
			get => _valueHeader;
			set => SetProperty(ref _valueHeader, value);
		}

		public FontDescriptor Value
		{
			get => _value;
			set => SetProperty(ref _value, value);
		}

		public FontDescriptor LineHeader
		{
			get => _lineHeader;
			set => SetProperty(ref _lineHeader, value);
		}

		public FontDescriptor Line
		{
			get => _line;
			set => SetProperty(ref _line, value);
		}

		public FontDescriptor MessageHeader
		{
			get => _messageHeader;
			set => SetProperty(ref _messageHeader, value);
		}

		public FontDescriptor Message
		{
			get => _message;
			set => SetProperty(ref _message, value);
		}

		public FontDescriptor TextEditor
		{
			get => _textEditor;
			set => SetProperty(ref _textEditor, value);
		}

		public FontDescriptor SimpleEditor
		{
			get => _simpleEditor;
			set => SetProperty(ref _simpleEditor, value);
		}

		public FontDescriptor SimpleEditorHeader
		{
			get => _simpleEditorHeader;
			set => SetProperty(ref _simpleEditorHeader, value);
		}


		public FontDescriptor MenuFlyoutItem
		{
			get => _menuFlyoutItem;
			set => SetProperty(ref _menuFlyoutItem, value);
		}

		public FontDescriptor MenuFlyoutItemPointerOver
		{
			get => _menuFlyoutItemPointerOver;
			set => SetProperty(ref _menuFlyoutItemPointerOver, value);
		}

		public FontDescriptor MenuFlyoutItemPressed
		{
			get => _menuFlyoutItemPressed;
			set => SetProperty(ref _menuFlyoutItemPressed, value);
		}

		public FontDescriptor MenuFlyoutItemDisabled
		{
			get => _menuFlyoutItemDisabled;
			set => SetProperty(ref _menuFlyoutItemDisabled, value);
		}

		public FontDescriptor MenuFlyoutItemGestureText
		{
			get => _menuFlyoutItemGestureText;
			set => SetProperty(ref _menuFlyoutItemGestureText, value);
		}

		public FontDescriptor MenuFlyoutItemGestureTextPointerOver
		{
			get => _menuFlyoutItemGestureTextPointerOver;
			set => SetProperty(ref _menuFlyoutItemGestureTextPointerOver, value);
		}

		public FontDescriptor MenuFlyoutItemGestureTextPressed
		{
			get => _menuFlyoutItemGestureTextPressed;
			set => SetProperty(ref _menuFlyoutItemGestureTextPressed, value);
		}

		public FontDescriptor MenuFlyoutItemGestureTextDisabled
		{
			get => _menuFlyoutItemGestureTextDisabled;
			set => SetProperty(ref _menuFlyoutItemGestureTextDisabled, value);
		}

		public FontDescriptor InputTape
		{
			get => _inputTape;
			set => SetProperty(ref _inputTape, value);
		}

		public FontDescriptor InputTapeLbl
		{
			get => _inputTapeLbl;
			set => SetProperty(ref _inputTapeLbl, value);
		}

		public FontDescriptor OutputTape
		{
			get => _outputTape;
			set => SetProperty(ref _outputTape, value);
		}

		public FontDescriptor OutputTapeLbl
		{
			get => _outputTapeLbl;
			set => SetProperty(ref _outputTapeLbl, value);
		}
	}
}
