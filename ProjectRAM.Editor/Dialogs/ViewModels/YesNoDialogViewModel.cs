using System.Linq.Expressions;
using ProjectRAM.Editor.Dialogs.Params;
using ProjectRAM.Editor.Dialogs.ViewModels.Base;

namespace ProjectRAM.Editor.Dialogs.ViewModels
{
	internal sealed class YesNoDialogViewModel : DialogViewModelBase
	{
		public YesNoDialogViewModel() : this("Dialog", "Message", new YesNoDialogParams())
		{
			
		}

		public YesNoDialogViewModel(string title, string message, YesNoDialogParams @params)
		{
			Title = title;
			Message = message;
			YesButtonText = @params.YesButtonText;
			NoButtonText = @params.NoButtonText;
			SetProperties(@params);
		}

		public string YesButtonText { get; }
		public string NoButtonText { get; }
	}
}
