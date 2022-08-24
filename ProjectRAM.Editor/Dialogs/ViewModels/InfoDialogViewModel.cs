using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectRAM.Editor.Dialogs.Params;
using ProjectRAM.Editor.Dialogs.ViewModels.Base;

namespace ProjectRAM.Editor.Dialogs.ViewModels
{
	internal sealed class InfoDialogViewModel : DialogViewModelBase
	{
		public InfoDialogViewModel() : this("Dialog", "Message", new InfoDialogParams())
		{
			
		}

		public InfoDialogViewModel(string title, string message, InfoDialogParams @params)
		{
			Title = title;
			Message = message;
			OkButtonText = @params.OkButtonText;
			SetProperties(@params);
		}

		public string OkButtonText { get; }
	}
}
