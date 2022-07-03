using System.Collections.ObjectModel;
using System.Linq;
using ProjectRAM.Editor.Helpers;
using ProjectRAM.Editor.Properties;
using ProjectRAM.Editor.ViewModels.Commands;

namespace ProjectRAM.Editor.ViewModels
{
	public class StyleEditorViewModel : ViewModelBase
	{
		#region Properties
		#region Styles
		private ObservableCollection<Style> _styles;

		public ObservableCollection<Style> Styles
		{
			get => _styles;
			set => SetProperty(ref _styles, value);
		}
		#endregion

		#region CurrentStyle
		private Style _currentStyle;

		public Style CurrentStyle
		{
			get => _currentStyle;
			set => SetProperty(ref _currentStyle, value);
		}
		#endregion
		#endregion

		private RelayCommand _save;

		public RelayCommand Save
		{
			get => _save;
		}

		private RelayCommand _close;

		public RelayCommand Close
		{
			get => _close;
		}

		public StyleEditorViewModel()
		{
			_styles = new(Essentials.GetAllStyles().ToList());
			_currentStyle = _styles[0];
			_close = new(Essentials.CloseTopWindow, () => true);
			_save = new(() =>
			{
				foreach (var style in _styles)
				{
					style.Save();
				}
			}, () => true);
		}
	}
}