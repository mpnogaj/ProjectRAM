using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ProjectRAM.Editor.ViewModels;

namespace ProjectRAM.Editor.Models
{
	public sealed class StyleDescriptorProperty : INotifyPropertyChanged
	{
		private readonly StyleEditorViewModel _viewModel;
		private readonly PropertyInfo _propertyInfo;

		public string PropertyName => _propertyInfo.Name;

		public string PropertyValue
		{
			get => (string)_propertyInfo.GetValue(_viewModel.CurrentStyle.StyleDescriptor)!;
			set => _propertyInfo.SetValue(_viewModel.CurrentStyle.StyleDescriptor, value);
		}

		public StyleDescriptorProperty(PropertyInfo pi, StyleEditorViewModel viewModel)
		{
			if (pi.PropertyType != typeof(string))
			{
				throw new ArgumentException();
			}

			_propertyInfo = pi;
			_viewModel = viewModel;
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
