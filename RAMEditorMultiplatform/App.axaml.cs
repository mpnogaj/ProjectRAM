using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RAMEditorMultiplatform.Views;
using System;

namespace RAMEditorMultiplatform
{
    public class App : Application, IControlledApplicationLifetime
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public event EventHandler<ControlledApplicationLifetimeStartupEventArgs> Startup;
        public event EventHandler<ControlledApplicationLifetimeExitEventArgs> Exit;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        { 

        }

        private void App_Startup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
        {
            
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public void Shutdown(int exitCode = 0)
        {
            throw new NotImplementedException();
        }
    }
}
