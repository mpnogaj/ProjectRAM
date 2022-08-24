using Avalonia.Controls;
using ProjectRAM.Editor.Dialogs.Params;
using ProjectRAM.Editor.Dialogs.ViewModels;
using ProjectRAM.Editor.Dialogs.Views;
using System.Threading.Tasks;

namespace ProjectRAM.Editor.Dialogs
{
	internal static class DialogManager
	{
		public static Task<string?> ShowInputDialog(string title,
			string message,
			Window parent,
			InputDialogParams? @params = null)
		{

			var inputDialog = new InputDialog
			{
				DataContext = new InputDialogViewModel(title, message, @params ?? new InputDialogParams())
			};
			return inputDialog.ShowDialog<string?>(parent);
		}

		public static Task<bool> ShowYesNoDialog(string title,
			string message,
			Window parent,
			YesNoDialogParams? @params = null)
		{
			var yesNoDialog = new YesNoDialog
			{
				DataContext = new YesNoDialogViewModel(title, message, @params ?? new YesNoDialogParams())
			};
			return yesNoDialog.ShowDialog<bool>(parent);
		}

		public static Task ShowInfoDialog(string title,
			string message,
			Window parent,
			InfoDialogParams? @params = null)
		{
			var infoDialog = new InfoDialog
			{
				DataContext = new InfoDialogViewModel(title, message, @params ?? new InfoDialogParams())
			};
			return infoDialog.ShowDialog(parent);
		}
	}
}
