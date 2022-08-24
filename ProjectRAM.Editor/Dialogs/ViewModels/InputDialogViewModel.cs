using ProjectRAM.Editor.Dialogs.Params;
using ProjectRAM.Editor.Dialogs.ViewModels.Base;

namespace ProjectRAM.Editor.Dialogs.ViewModels
{
	internal sealed class InputDialogViewModel : DialogViewModelBase
	{
		private string _inputText;

		public InputDialogViewModel(): this("Dialog", "Message", new InputDialogParams())
		{
			
		}

		public InputDialogViewModel(string title, string message, InputDialogParams @params)
		{
			Title = title;
			Message = message;
			_inputText = @params.InitialInput;
			DefaultButtonText = @params.DefaultButtonText;
			CancelButtonText = @params.CancelButtonText;
			SetProperties(@params);
		}

		public string InputText
		{
			get => _inputText;
			set => SetProperty(ref _inputText, value);
		}

		public string DefaultButtonText { get; }
		public string CancelButtonText { get; }

	}
}
