using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FontPicker;
using ProjectRAM.Editor.Dialogs;
using ProjectRAM.Editor.Dialogs.Params;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Models;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.ViewModels.Commands;

namespace ProjectRAM.Editor.ViewModels
{
	public class StyleEditorViewModel : ViewModelBase
	{
		#region Properties
		#region Styles
		private ObservableCollection<Style> _styles;

		public ObservableCollection<Style> Styles
		{
			get => _styles;
			set => SetProperty(ref _styles, value);
		}
		#endregion

		#region CurrentStyle
		private Style _currentStyle;

		public Style CurrentStyle
		{
			get => _currentStyle;
			set
			{
				value.ApplyStyle();
				SetProperty(ref _currentStyle, value);
			}
		}
		#endregion
		#endregion

		public Action OnClose { get; }

		public RelayCommand SaveCommand { get; }
		public RelayCommand CloseCommand { get; }
		public RelayCommand RevertToDefaultCommand { get; }
		public AsyncRelayCommand CreateNewStyleCommand { get; }

		public AsyncRelayCommand<string> SetFontCommand { get; }

		public StyleEditorViewModel()
		{
			_styles = new ObservableCollection<Style>(Essentials.GetAllStyles().ToList());
			_currentStyle = _styles.FirstOrDefault(x => x.Equals(Settings.CurrentStyle)) ?? _styles[0];
			OnClose = () =>
			{
				if (!CurrentStyle.Identical(Settings.CurrentStyle) && CurrentStyle.FileName != Settings.CurrentStyle.FileName)
				{
					Settings.CurrentStyle.ApplyStyle();
				}
			};

			CloseCommand = new RelayCommand(Essentials.CloseTopWindow, () => true);
			SaveCommand = new RelayCommand(() =>
			{
				foreach (var style in _styles)
				{
					style.Save();
				}
			}, () => true);
			RevertToDefaultCommand = new RelayCommand(() =>
			{
				CurrentStyle.StyleDescriptor = new StyleDescriptor
				{
					Name = CurrentStyle.StyleDescriptor.Name
				};
				CurrentStyle.ApplyStyle();
			}, () => true);
			
			CreateNewStyleCommand = new AsyncRelayCommand(async () =>
			{
				var res = await DialogManager.ShowInputDialog("Create new style",
					"Style name:",
					Essentials.GetTopWindow(),
					new InputDialogParams
					{
						DefaultButtonText = "Add",
						MinHeight = 75
					});

				if (string.IsNullOrWhiteSpace(res))
				{
					return;
				}

				Styles.Add(new Style
				{
					StyleDescriptor =
					{
						Name = "TEMP"
					},
					FileName = $"temp.json"
				});
			}, () => true);

			SetFontCommand = new AsyncRelayCommand<string>(async (target) =>
			{
				var propertyInfo = typeof(StyleDescriptor).GetProperty(target);
				var fontDescriptor = (FontDescriptor)propertyInfo!.GetValue(CurrentStyle.StyleDescriptor)!;
				var font = new Font
				{
					FontFamily = fontDescriptor.FontFamily,
					FontSize = fontDescriptor.FontSize,
					FontWeight = fontDescriptor.FontWeight,
					FontStyle = fontDescriptor.FontStyle,
					Foreground = new SolidColorBrush(Color.Parse(fontDescriptor.Foreground))
				};
				var dialog = new FontPickerDialog(Strings.Culture)
				{
					StartupLocation = WindowStartupLocation.CenterOwner
				};
				var newFont = await dialog.OpenDialog(Essentials.GetTopWindow(), font);
				// Check if user closed the dialog or the cancel button was clicked
				if (newFont == null)
				{
					return;
				}
				var newFontDescriptor = new FontDescriptor
				{
					FontFamily = newFont.FontFamily.Name,
					FontSize = newFont.FontSize,
					FontWeight = newFont.FontWeight,
					FontStyle = newFont.FontStyle,
					Foreground = newFont.Foreground.Color.ToString()
				};
				propertyInfo.SetValue(CurrentStyle.StyleDescriptor, newFontDescriptor);
				newFontDescriptor.ApplyFontStyle(Application.Current!.Resources, target);
			}, () => true);
		}
	}
}