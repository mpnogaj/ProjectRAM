using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProjectRAM.Editor.Views;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Models;

namespace ProjectRAM.Editor
{
    public class App : Application
	{
		public override void Initialize()
		{
			if (!Directory.Exists("Styles"))
			{
				Directory.CreateDirectory("Styles");
			}

			if (!File.Exists("Styles/default.json"))
			{
				Style.CreateDefaultAndSave();
			}

			StyleManager.Init();
			Settings.Init();
			Strings.Culture = Settings.Language;
			Core.Properties.Settings.CurrentCulture = Settings.Language;
			Settings.CurrentStyle.ApplyStyle();

			AvaloniaXamlLoader.Load(this);
		}

		#region Event handlers

		private static void App_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
		{
			Settings.Save();
		}

		private static void App_Startup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
		{
			var mainWindow = Essentials.GetAppDesktopLifetime().MainWindow;
			mainWindow.Position = Settings.StartupLocation;
			mainWindow.Width = Settings.WindowSize.Item1;
			mainWindow.Height = Settings.WindowSize.Item2;
		}

		#endregion

		#region Overrides

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow();
				desktop.Startup += App_Startup;
				desktop.Exit += App_Exit;
			}
			base.OnFrameworkInitializationCompleted();
		}

		#endregion
	}
}
