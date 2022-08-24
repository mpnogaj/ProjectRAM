using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProjectRAM.Editor.Dialogs.Params.Base;

namespace ProjectRAM.Editor.Dialogs.ViewModels.Base
{
    internal abstract class DialogViewModelBase : INotifyPropertyChanged
    {
		public WindowStartupLocation StartupLocation { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }
		public double MaxWidth { get; set; }
		public double MaxHeight { get; set; }
		public SizeToContent SizeToContent { get; set; }
		public bool CanResize { get; set; }
		protected string Title { get; set; } = null!;
		protected string Message { get; set; } = null!;

		protected virtual void SetProperties(DialogParamsBase @params)
		{
			this.StartupLocation = @params.StartupLocation;
			this.Width = @params.Width;
			this.Height = @params.Height;
			this.MinWidth = @params.MinWidth;
			this.MinHeight = @params.MinHeight;
			this.MaxWidth = @params.MaxWidth;
			this.MaxHeight = @params.MaxHeight;
			this.SizeToContent = @params.SizeToContent;
			this.CanResize = @params.CanResize;
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
		{
			if (storage?.Equals(value) ?? value == null)
			{
				return;
			}

			storage = value;
			PropertyChanged?.Invoke(this,
				new PropertyChangedEventArgs(propertyName ?? throw new ArgumentNullException(nameof(propertyName))));
		}
    }
}
