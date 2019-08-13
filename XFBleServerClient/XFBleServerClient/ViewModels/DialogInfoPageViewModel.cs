using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;

namespace XFBleServerClient.Core.ViewModels
{
	public class DialogInfoPageViewModel : BindableBase, IDialogAware
	{
		public DialogInfoPageViewModel() 
		{
			this.CloseCommand = new DelegateCommand(() => OnCloseCommand());
		}


		private string _message;
		[AutoInitialize(true)] // Makes Message parameter required
		public string Message
		{
			get => _message;
			set => SetProperty(ref _message, value);
		}

		public DelegateCommand CloseCommand { get; private set; }
		public event Action<IDialogParameters> RequestClose;

		public bool CanCloseDialog()
		{
			return true;
		}
		private void OnCloseCommand()
		{
			RequestClose?.Invoke(new DialogParameters());
		}

		public void OnDialogClosed()
		{
		}

		public void OnDialogOpened(IDialogParameters parameters)
		{

		}
	}
}
