using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ProjectRAM.Editor.ViewModels;


namespace ProjectRAM.Editor.Views
{
	public class Host : UserControl
	{
		private void OnKeyDown(object sender, KeyEventArgs e) => ((HostViewModel)DataContext!)!.HandleDataGridKeyEvents(sender, e);
		public Host()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
