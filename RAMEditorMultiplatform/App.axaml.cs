using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RAMEditorMultiplatform.Windows;
using System;

namespace RAMEditorMultiplatform
{
    public class App : Application, IControlledApplicationLifetime
    {
        public event EventHandler<ControlledApplicationLifetimeStartupEventArgs> Startup;
        public event EventHandler<ControlledApplicationLifetimeExitEventArgs> Exit;

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
