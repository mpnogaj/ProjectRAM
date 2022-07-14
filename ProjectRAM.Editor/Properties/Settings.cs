using Avalonia;
using Avalonia.Controls;
using ProjectRAM.Editor.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Tommy;

namespace ProjectRAM.Editor.Properties
{
	public class Settings
	{
		private const string PreferencesFile = "preferences.toml";

		public static CultureInfo Language { get; set; } = Essentials.GetDefaultLanguage();
		public static bool UseDiscordRpc { get; set; }
		public static bool UseTextEditor { get; set; }

		public static PixelPoint StartupLocation { get; set; } = new(0, 0);
		public static Tuple<double, double> WindowSize { get; set; } = new(800, 600);

		public static GridLength LeftPanelSize { get; set; } = new(250);
		public static GridLength BottomPanelSize { get; set; } = new(150);


		private static Style _currentStyle = new();

		public static Style CurrentStyle
		{
			get => _currentStyle;
			set => (_currentStyle = value).ApplyStyle();
		}

		public static string StyleFile { get; set; } = "default.json";
		public static HashSet<Style> AvailableStyles { get; set; } = Essentials.GetAllStyles().ToHashSet();

		public static void RestoreDefault()
		{
			Language = Essentials.GetDefaultLanguage();
			UseDiscordRpc = false;
			UseTextEditor = false;
			StartupLocation = new PixelPoint(0, 0);
			WindowSize = new Tuple<double, double>(800, 600);
			LeftPanelSize = new GridLength(250);
			BottomPanelSize = new GridLength(150);
			StyleFile = "default.json";
			CurrentStyle = new Style();
		}

		public static void Init()
		{
			try
			{
				if (!File.Exists(PreferencesFile)) return;
				using (var sr = new StreamReader(PreferencesFile))
				{
					var table = TOML.Parse(sr);
					if (table[nameof(Language)] is TomlString lang)
					{
						var cultureInfo = new CultureInfo(lang.Value);

						if (Essentials.GetAvailableCultures().Contains(cultureInfo))
						{
							Language = cultureInfo;
						}
					}

					if (table[nameof(UseDiscordRpc)] is TomlBoolean discord)
					{
						UseDiscordRpc = discord.Value;
					}

					if (table[nameof(UseTextEditor)] is TomlBoolean text)
					{
						UseTextEditor = text.Value;
					}

					if (table[nameof(StartupLocation)]["X"] is TomlInteger x &&
						table[nameof(StartupLocation)]["Y"] is TomlInteger y)
					{
						StartupLocation = new PixelPoint((int)x.Value, (int)y.Value);
					}

					var width = table[nameof(WindowSize)]["Width"] switch
					{
						TomlInteger widthInt => widthInt.Value,
						TomlFloat widthFloat => widthFloat.Value,
						_ => 800
					};

					var height = table[nameof(WindowSize)]["Height"] switch
					{
						TomlInteger widthInt => widthInt.Value,
						TomlFloat widthFloat => widthFloat.Value,
						_ => 600
					};

					WindowSize = new Tuple<double, double>(width, height);


					if (table[nameof(LeftPanelSize)] is TomlInteger leftPanelSize)
					{
						LeftPanelSize = new GridLength(leftPanelSize.Value);
					}

					if (table[nameof(BottomPanelSize)] is TomlInteger bottomPanelSize)
					{
						BottomPanelSize = new GridLength(bottomPanelSize.Value);
					}

					if (table[nameof(StyleFile)] is TomlString style)
					{
						StyleFile = style.Value;
					}
				}
				
				var currentStyle = AvailableStyles.FirstOrDefault((s) => s.FileName.Equals(StyleFile));
				if (currentStyle == null)
				{
					Style.CreateDefaultAndSave();
					currentStyle = AvailableStyles.First((x) => x.Equals(new Style()));
				}
				CurrentStyle = currentStyle;
			}
			catch
			{
				// Override with default settings 
				RestoreDefault();
			}
		}

		public static void Save()
		{
			TomlTable table = new()
			{
				[nameof(Language)] = Language.TwoLetterISOLanguageName,
				[nameof(UseDiscordRpc)] = UseDiscordRpc,
				[nameof(UseTextEditor)] = UseTextEditor,
				[nameof(StartupLocation)] =
				{
					["X"] = StartupLocation.X,
					["Y"] = StartupLocation.Y
				},
				[nameof(WindowSize)] =
				{
					["Width"] = WindowSize.Item1,
					["Height"] = WindowSize.Item2
				},
				[nameof(LeftPanelSize)] = LeftPanelSize.Value,
				[nameof(BottomPanelSize)] = BottomPanelSize.Value,

				[nameof(StyleFile)] = CurrentStyle.FileName
			};

			CurrentStyle.Save();

			using var sw = new StreamWriter(PreferencesFile);
			table.WriteTo(sw);
			sw.Flush();
		}
	}
}