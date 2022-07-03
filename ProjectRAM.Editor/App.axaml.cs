using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProjectRAM.Editor.Views;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.Helpers;

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
			AvaloniaXamlLoader.Load(this);
			Settings.Init();
			Strings.Culture = Settings.Language;
			ProjectRAM.Core.Properties.Settings.CurrentCulture = Settings.Language;
			Settings.CurrentStyle.ApplyStyle();
		}

		#region Event handlers

		private void App_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
		{
			Settings.Save();
		}

		private void App_Startup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
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
