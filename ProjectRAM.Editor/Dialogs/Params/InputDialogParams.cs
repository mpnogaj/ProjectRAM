using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectRAM.Editor.Dialogs.Params.Base;

namespace ProjectRAM.Editor.Dialogs.Params
{
	internal class InputDialogParams : DialogParamsBase
	{
		public string InitialInput { get; set; } = "";
		public string DefaultButtonText { get; set; } = "Ok";
		public string CancelButtonText { get; set; } = "Cancel";
	}
}
