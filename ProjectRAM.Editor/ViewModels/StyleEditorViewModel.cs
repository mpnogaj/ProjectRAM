using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using AvaloniaFontPicker;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;
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
		public AsyncRelayCommand CreateNewStyleCommand { get; }

		public AsyncRelayCommand<string> SetFontCommand { get; }

		public StyleEditorViewModel()
		{
			_styles = new ObservableCollection<Style>(Essentials.GetAllStyles().ToList());
			_currentStyle = _styles.FirstOrDefault(x => x.Equals(Settings.CurrentStyle)) ?? _styles[0];
			OnClose = () =>
			{
				if (!Equals(CurrentStyle, Settings.CurrentStyle))
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

			CreateNewStyleCommand = new AsyncRelayCommand(async () =>
			{
				var dialog = MessageBoxManager.GetMessageBoxInputWindow(new MessageBoxInputParams
				{
					ButtonDefinitions = new []
					{
						new ButtonDefinition
						{
							IsCancel = false,
							IsDefault = true,
							Name = "Add"
						},
						new ButtonDefinition
						{
							IsCancel = true,
							IsDefault = false,
							Name = "Cancel"
						}
					}
				});
				var res = await dialog.ShowDialog(Essentials.GetTopWindow());
				if (res.Button == "Add")
				{
					Styles.Add(new Style
					{
						Name = res.Message,
						FileName = $"{res.Message.ToLower()}.json"
					});
				}
			}, () => true);

			SetFontCommand = new AsyncRelayCommand<string>(async (target) =>
			{
				var propertyInfo = typeof(Style).GetProperty(target);
				var fontDescriptor = (FontDescriptor)propertyInfo!.GetValue(CurrentStyle)!;
				var font = new Font
				{
					FontFamily = fontDescriptor.FontFamily,
					FontSize = fontDescriptor.FontSize,
					FontWeight = fontDescriptor.FontWeight,
					FontStyle = fontDescriptor.FontStyle,
					Foreground = new SolidColorBrush(Color.Parse(fontDescriptor.Foreground))
				};
				var dialog = new FontDialog(font);
				await dialog.Show(Essentials.GetTopWindow(), (f) =>
				{
					var newFontDescriptor = new FontDescriptor
					{
						FontFamily = f.FontFamily.Name,
						FontSize = f.FontSize,
						FontWeight = f.FontWeight,
						FontStyle = f.FontStyle,
						Foreground = f.Foreground.Color.ToString()
					};
					propertyInfo.SetValue(CurrentStyle,  newFontDescriptor);
					newFontDescriptor.ApplyFontStyle(target);
				});
			}, () => true);
		}
	}
}