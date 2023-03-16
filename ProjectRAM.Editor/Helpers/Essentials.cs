using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using ProjectRAM.Editor.Dialogs;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.Views;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace ProjectRAM.Editor.Helpers
{
	public static class Essentials
	{
		public static async Task WriteToFile(IStorageFile? file, string content)
		{
			if (file is not { CanOpenWrite: true })
			{
				return;
			}
			
			var sw = new StreamWriter(await file.OpenWriteAsync());
			await sw.WriteAsync(content);
			sw.Close();
			file.Dispose();
		}

		[Obsolete]
		public static async Task WriteToFile(string file, string content)
		{
			if (!OperatingSystem.IsLinux() && !OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS())
			{
				throw new PlatformNotSupportedException();
			}
			
			var sw = new StreamWriter(file);
			await sw.WriteAsync(content);
			sw.Close();
		}

		public static async Task<string?> ReadFromFile(IStorageFile? file)
		{
			if (file is not { CanOpenRead: true })
			{
				return null;
			}

			var sr = new StreamReader(await file.OpenReadAsync());
			string res = await sr.ReadToEndAsync();
			sr.Close();
			file.Dispose();
			return res;
		}

		[Obsolete]
		public static async Task<string?> ReadFromFile(string file)
		{
			if (!OperatingSystem.IsLinux() && !OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS())
			{
				throw new PlatformNotSupportedException();
			}
			
			try
			{
				var sr = new StreamReader(file);
				string res = await sr.ReadToEndAsync();
				sr.Close();
				return res;
			}
			catch
			{
				return null;
			}
		}

		public static IClassicDesktopStyleApplicationLifetime GetAppDesktopLifetime()
		{
			return (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
		}

		public static IStorageProvider GetStorageProvider()
		{
			return GetMainWindow().StorageProvider;
		}

		public static App GetAppObject()
		{
			return (App)Application.Current!;
		}

		public static void SetCursor(StandardCursorType cursor)
		{
			GetAppDesktopLifetime().MainWindow!.Cursor = new Cursor(cursor);
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

		public static void CloseTopWindow()
		{
			var window = GetTopWindow();
			// Don't want to close main window
			if (window is not MainWindow) window.Close();
		}

		public static MainWindow GetMainWindow()
		{
			var lifetime = GetAppDesktopLifetime();
			return (MainWindow)lifetime.MainWindow!;
		}

		public static Window GetTopWindow()
		{
			var lifetime = GetAppDesktopLifetime();
			var window = lifetime.Windows[^1];
			return window;
		}

		public static async Task ShowMessageBox(string title, string message)
		{
			await DialogManager.ShowInfoDialog(title, message, GetTopWindow());
			/*var messageBox = MessageBoxManager.GetMessageBoxStandardWindow(title, message,
				icon: icon,
				windowStartupLocation: WindowStartupLocation.CenterOwner);
			await messageBox.ShowDialog(GetTopWindow());*/
		}


		public static DataGridCell? GetSelectedCell(this DataGrid dataGrid)
		{
			return dataGrid.FindDescendantOfType<DataGridRowsPresenter>()
				.Children.OfType<DataGridRow>()
				.SelectMany(row => row.FindDescendantOfType<DataGridCellsPresenter>()
					.Children.OfType<DataGridCell>())
				.FirstOrDefault(cell => cell.Classes.Contains(":current"));
		}

		public static bool IsAlphaNumeric(this Key key, bool numLockOn)
		{
			int code = (int)key;
			return code is >= 34 and <= 69 || (numLockOn && code is >= 74 and <= 83);
		}
	}
}
