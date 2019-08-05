using Plugin.BluetoothLE;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;

namespace XFBleServerClient.Core.ViewModels
{
	public class ServerSetupPageViewModel : ViewModelBase
	{
		private readonly IAdapter _adapter;
		private readonly IDialogService _dialogService;

		public ServerSetupPageViewModel(INavigationService navigationService, IAdapter adapter, IDialogService dialogService) : base(navigationService)
		{
			_adapter = adapter;
			_dialogService = dialogService;

			this.SelectInfoCommand = new DelegateCommand(async () => await ExecuteSelectInfoCommand());
		}

		private string _subtitleMessage;
		public string SubtitleMessage
		{
			get => _subtitleMessage;
			set => SetProperty(ref _subtitleMessage, value);
		}

		public DelegateCommand SelectInfoCommand { get; private set; }

		private async Task ExecuteSelectInfoCommand()
		{
			IDialogParameters parameters = new DialogParameters();
			parameters.Add(ParameterConstants.Message, SubtitleMessage);
			_dialogService.ShowDialog(ViewNames.DialogInfoPage, parameters);
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			base.OnNavigatedTo(parameters);
			InitializeProperty();
			//var s = await _adapter.CreateGattServer();
			//_dialogService.ShowDialog(ViewNames.DialogInfoPage);
		}

		private void InitializeProperty()
		{
			SubtitleMessage = "The GATT server corresponds to the ATT server discussed in Attribute Protocol (ATT). It receives requests from a client and sends responses back. It also sends server-initiated updates when configured to do so, and it is the role responsible for storing and making the user data available to the client, organized in attributes. Every BLE device sold must include at least a basic GATT server that can respond to client requests, even if only to return an error response.";
		}
	}
}
