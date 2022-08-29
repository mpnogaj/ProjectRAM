using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectRAM.Editor.ViewModels
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this,
				new PropertyChangedEventArgs(propertyName ?? throw new ArgumentNullException(nameof(propertyName))));
		}

		protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(storage, value)) return;
			storage = value;
			OnPropertyChanged(propertyName);
		}
	}
}
