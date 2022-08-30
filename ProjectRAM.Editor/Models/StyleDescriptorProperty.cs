using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ProjectRAM.Editor.ViewModels;

namespace ProjectRAM.Editor.Models
{
	public sealed class StyleDescriptorProperty<T> : INotifyPropertyChanged
	{
		private readonly StyleEditorViewModel _viewModel;
		private readonly PropertyInfo _propertyInfo;
		
		public string PropertyName { get; }
		public string LocalizedName { get; }

		public T PropertyValue
		{
			get => (T)_propertyInfo.GetValue(_viewModel.CurrentStyle.StyleDescriptor)!;
			set => _propertyInfo.SetValue(_viewModel.CurrentStyle.StyleDescriptor, value);
		}

		public StyleDescriptorProperty(PropertyInfo pi, StyleEditorViewModel viewModel)
		{
			if (pi.PropertyType != typeof(string) && pi.PropertyType != typeof(FontDescriptor))
			{
				throw new ArgumentException();
			}

			_propertyInfo = pi;
			_viewModel = viewModel;

			PropertyName = pi.Name;

			var attr = pi.GetCustomAttribute(typeof(LocalizedDisplayNameAttribute), false);
			LocalizedName = attr != null ? ((LocalizedDisplayNameAttribute)attr).DisplayName : PropertyName;
		}

		public void CurrentStyleChanged()
		{
			OnPropertyChanged(nameof(PropertyValue));
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
