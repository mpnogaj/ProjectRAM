using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
		private Style _currentStyle;
		public Style CurrentStyle
		{
			get => _currentStyle;
			set
			{
				// ask save changes
				value.ApplyStyle();
				SetProperty(ref _currentStyle, value);
				foreach (var colorProperty in ColorProperties)
				{
					colorProperty.CurrentStyleChanged();
				}
			}
		}

		private ObservableCollection<Style> _styles;
		public ObservableCollection<Style> Styles
		{
			get => _styles;
			set => SetProperty(ref _styles, value);
		}

		public List<StyleDescriptorProperty> ColorProperties { get; }

		public Action OnClose { get; }

		public RelayCommand SaveCommand { get; }
		public RelayCommand CloseCommand { get; }
		public RelayCommand RevertToDefaultCommand { get; }
		public AsyncRelayCommand CreateNewStyleCommand { get; }

		public AsyncRelayCommand<string> SetFontCommand { get; }

		public StyleEditorViewModel()
		{
			OnClose = Settings.CurrentStyle.ApplyStyle;
			CloseCommand = new RelayCommand(Essentials.CloseTopWindow, () => true);
			SaveCommand = new RelayCommand(() =>
			{
				StyleManager.UpdateStyle(CurrentStyle);
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

				AddStyle(res.EndsWith(".json") ? res : res + ".json");
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

			_styles = new ObservableCollection<Style>(StyleManager.GetCopyOfStyles());

			ColorProperties =
				typeof(StyleDescriptor).GetProperties()
					.Where(pi => pi.PropertyType == typeof(string) && 
					             !pi.GetCustomAttributes(typeof(InfoAttribute), false).Any())
					.Select(pi => new StyleDescriptorProperty(pi, this))
					.ToList();
			CurrentStyle = _styles[0];
			//Make sure that the list is deep cloned
			// ReSharper disable once PossibleUnintendedReferenceComparison
			Debug.Assert(_currentStyle != StyleManager.GetStyles()[0]);
		}

		private void AddStyle(string fileName)
		{
			Styles.Add(new Style
			{
				FileName = fileName,
				StyleDescriptor = new StyleDescriptor
				{
					Name = Path.GetFileNameWithoutExtension(fileName)
				}
			});
		}
	}
}