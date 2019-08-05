using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XFBleServerClient.Core.ViewModels
{
	public class DialogInfoPageViewModel : ViewModelBase,IDialogAware, IAutoInitialize
	{
		public DialogInfoPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			RequestClose = new Action<IDialogParameters>((parameters) => { });
			CloseCommand = new DelegateCommand(() => RequestClose(null));
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

		public void OnDialogClosed()
		{
		}

		public void OnDialogOpened(IDialogParameters parameters)
		{

		}
	}
}
