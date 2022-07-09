using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.ViewModels.Commands;

namespace ProjectRAM.Editor.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private List<CultureInfo> _languages;

		public List<CultureInfo> Languages
		{
			get => _languages;
			set => SetProperty(ref _languages, value);
		}

		private List<Style> _styles;

		public List<Style> Styles
		{
			get => _styles;
			set => SetProperty(ref _styles, value);
		}

		private Style _style;

		public Style Style
		{
			get => _style;
			set => SetProperty(ref _style, value);
		}

		private CultureInfo _language;

		public CultureInfo Language
		{
			get => _language;
			set => SetProperty(ref _language, value);
		}

		private bool _discordRpc;

		public bool DiscordRpc
		{
			get => _discordRpc;
			set => SetProperty(ref _discordRpc, value);
		}

		private bool _textEditor;
		public bool TextEditor
		{
			get => _textEditor;
			set => SetProperty(ref _textEditor, value);
		}

		public RelayCommand Close { get; }

		public RelayCommand Save { get; }

		public RelayCommand RevertToDefault { get; }

		public SettingsViewModel()
		{
			Close = new RelayCommand(Essentials.CloseTopWindow, () => true);

			Save = new RelayCommand(() =>
			{
				if (Settings.Language.TwoLetterISOLanguageName != Language.TwoLetterISOLanguageName)
				{
					// Warning message
				}
				Settings.Language = Language;
				Settings.UseDiscordRpc = DiscordRpc;
				Settings.UseTextEditor = TextEditor;
				if (!Settings.CurrentStyle.Equals(Style))
				{
					Settings.CurrentStyle = Style;
				}
				Essentials.CloseTopWindow();
			}, () => true);

			RevertToDefault = new RelayCommand(Settings.RestoreDefault, () => true);


			_languages = Essentials.GetAvailableCultures().ToList();
			_language = Settings.Language;
			_discordRpc = Settings.UseDiscordRpc;
			_textEditor = Settings.UseTextEditor;

			_styles = Essentials.GetAllStyles().ToList();
			_style = _styles.Find(x => x.FileName == Settings.CurrentStyle.FileName) ?? _styles[0];
		}
	}
}