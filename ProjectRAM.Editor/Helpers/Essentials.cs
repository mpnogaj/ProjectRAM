using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ProjectRAM.Editor.Views;
using ProjectRAM.Editor.Properties;
using Style = ProjectRAM.Editor.Properties.Style;

namespace ProjectRAM.Editor.Helpers
{
	public static class Essentials
	{
		public static string TapeToString(Queue<string> tape)
		{
			return string.Join(", ", tape.ToArray());
		}

		public static void WriteToFile(string file, string content)
		{
			using StreamWriter sw = new(file);
			sw.Write(content);
		}

		public static void AppendFile(string file, string content)
		{
			using StreamWriter sw = new(file, append: true);
			sw.Write(content);
		}

		public static string ReadFromFile(string file)
		{
			using StreamReader sr = new(file);
			return sr.ReadToEnd();
		}

		public static IClassicDesktopStyleApplicationLifetime GetAppDesktopLifetime()
		{
			return (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
		}

		public static App GetAppObject()
		{
			return (App)Application.Current;
		}

		public static void SetCursor(StandardCursorType cursor)
		{
			GetAppDesktopLifetime().MainWindow.Cursor = new Cursor(cursor);
		}

		public static void Exit()
		{
			GetAppDesktopLifetime().Shutdown();
		}

		public static IEnumerable<CultureInfo> GetAvailableCultures()
		{
			List<CultureInfo> result = new List<CultureInfo>();

			ResourceManager rm = new ResourceManager(typeof(Strings));

			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			result.Add(new CultureInfo("en"));
			foreach (CultureInfo culture in cultures)
			{
				try
				{
					if (culture.Equals(CultureInfo.InvariantCulture))
					{
						continue;
					}
					var rs = rm.GetResourceSet(culture, true, false);
					if (rs != null)
					{
						result.Add(culture);
					}
				}
				catch (CultureNotFoundException) { }
			}
			return result;
		}

		public static CultureInfo GetDefaultLanguage()
		{
			var userCulture = new CultureInfo(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
			return GetAvailableCultures().Contains(userCulture) ? userCulture : new CultureInfo("en");
		}

		public static IEnumerable<Style> GetAllStyles()
		{
			List<Style> styles = new List<Style>();
			var files = Directory.EnumerateFiles("Styles/");
			foreach (var file in files)
			{
				if (Path.GetExtension(file) != ".json") continue;
				var content = File.ReadAllText(file);
				if (string.IsNullOrEmpty(content)) continue;
				try
				{
					var style = JsonSerializer.Deserialize<Style>(content);
					if (style == null) continue;
					style.FileName = Path.GetFileName(file);
					styles.Add(style);
				}
				catch
				{
					// ignored
				}
			}
			return styles;
		}

		public static void CloseTopWindow()
		{
			var window = GetTopWindow();
			// Don't want to close main window
			if (window is not MainWindow) window.Close();
		}

		public static Window GetTopWindow()
		{
			var lifetime = GetAppDesktopLifetime();
			Window window = lifetime.Windows[^1];
			return window;
		}

		public static async Task ShowMessageBox(string title, string message, Icon icon)
		{
			var messageBox = MessageBoxManager.GetMessageBoxStandardWindow(title, message,
				icon: icon,
				windowStartupLocation: WindowStartupLocation.CenterOwner);
			await messageBox.ShowDialog(GetTopWindow());
		}
	}
}
