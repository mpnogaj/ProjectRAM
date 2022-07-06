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
			set => SetProperty(ref _currentStyle, value);
		}
		#endregion
		#endregion

		public RelayCommand SaveCommand { get; }
		public RelayCommand CloseCommand { get; }
		public AsyncRelayCommand CreateNewStyleCommand { get; }

		public AsyncRelayCommand SetFontCommand { get; }

		public StyleEditorViewModel()
		{
			_styles = new ObservableCollection<Style>(Essentials.GetAllStyles().ToList());
			_currentStyle = _styles[0];
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

			SetFontCommand = new AsyncRelayCommand(async () =>
			{
				var fontDescriptor = CurrentStyle.NormalText;
				var font = new Font
				{
					FontFamily = fontDescriptor.FontFamily,
					FontSize = fontDescriptor.FontSize,
					FontWeight = fontDescriptor.FontWeight,
					FontStyle = fontDescriptor.FontStyle,
					Foreground = new SolidColorBrush(Color.Parse(fontDescriptor.Foreground))
				};
				var dialog = new FontDialog(font);
				await dialog.Show(Essentials.GetTopWindow(), (font) =>
				{
			 		CurrentStyle.NormalText = new FontDescriptor
					{
						FontFamily = font.FontFamily.Name,
						FontSize = font.FontSize,
						FontWeight = font.FontWeight,
						FontStyle = font.FontStyle,
						Foreground = font.Foreground.Color.ToString()
					};
					CurrentStyle.NormalText.ApplyFontStyle(nameof(CurrentStyle.NormalText));
				});
			}, () => true);
		}
	}
}