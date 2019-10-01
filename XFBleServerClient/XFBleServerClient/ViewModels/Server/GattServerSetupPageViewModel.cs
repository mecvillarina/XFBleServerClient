using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Plugin.BluetoothLE.Server;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;
using XFBleServerClient.Core.ItemModels;
using XFBleServerClient.Core.Providers;

namespace XFBleServerClient.Core.ViewModels
{
	public class GattServerSetupPageViewModel : ViewModelBase
	{
		private readonly IAdapter _adapter;
		private readonly IDialogService _dialogService;
		private readonly IUserDialogs _userDialogs;

		public GattServerSetupPageViewModel(INavigationService navigationService, IAdapter adapter, IDialogService dialogService, IUserDialogs userDialogs) : base(navigationService)
		{
			_adapter = adapter;
			_dialogService = dialogService;
			_userDialogs = userDialogs;

			this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
			this.SelectInfoCommand = new DelegateCommand(async () => await ExecuteSelectInfoCommand());
			this.BroadcastCommand = new DelegateCommand(async () => await OnBroadcastCommandAsync(), () => !string.IsNullOrWhiteSpace(this.ServerName)).ObservesProperty(() => this.ServerName);
			this.ServiceSelectionCommand = new DelegateCommand<GattServiceItemModel>(async (item) => await OnServiceSelectionCommand(item));
		}


		IGattServer _gattServer;

		private string _subtitleMessage;
		public string SubtitleMessage
		{
			get => _subtitleMessage;
			set => SetProperty(ref _subtitleMessage, value);
		}

		private string _btnText = "Start Broadcast";
		public string BtnText
		{
			get => _btnText;
			set => SetProperty(ref _btnText, value);
		}

		private string _serverName = "My Server";
		public string ServerName
		{
			get => _serverName;
			set => SetProperty(ref _serverName, value);
		}

		private List<GattServiceItemModel> _gattServices;
		public List<GattServiceItemModel> GattServices
		{
			get => _gattServices;
			set => SetProperty(ref _gattServices, value);
		}

		public DelegateCommand BackCommand { get; private set; }
		public DelegateCommand SelectInfoCommand { get; private set; }
		public DelegateCommand BroadcastCommand { get; private set; }
		public DelegateCommand<GattServiceItemModel> ServiceSelectionCommand { get; private set; }

		private async Task ExecuteSelectInfoCommand()
		{
			IDialogParameters parameters = new DialogParameters();
			parameters.Add(ParameterConstants.Message, this.SubtitleMessage);
			_dialogService.ShowDialog(ViewNames.DialogInfoPage, parameters);
		}

		private async Task OnBackCommandAsync()
		{
			await this.NavigationService.GoBackAsync();
		}

		private async Task OnBroadcastCommandAsync()
		{
			if (_adapter.Status != AdapterStatus.PoweredOn)
			{
				await _userDialogs.AlertAsync("Could not start GATT Server.  Adapter Status: " + _adapter.Status);

                if (_adapter.CanOpenSettings())
                {
                    _adapter.OpenSettings();
                }

				return;
			}

			if (!_adapter.Features.HasFlag(AdapterFeatures.ServerGatt))
			{
				await _userDialogs.AlertAsync("GATT Server is not supported on this platform configuration");
				return;
			}

			if (_gattServer == null)
			{
				var isSuccess = await BuildServer();

				if (isSuccess)
				{
					_adapter.Advertiser.Start(new AdvertisementData
					{
						AndroidIsConnectable = true,
						LocalName = ServerName
					});
				}
			}
			else
			{
				this.BtnText = "Start Broadcast";
				_adapter.Advertiser.Stop();
				//this.OnEvent("GATT Server Stopped");
				_gattServer.Dispose();
				_gattServer = null;
				//this.timer?.Dispose();
			}

			//var gattServer = await _adapter.CreateGattServer();
			//gattServer.ClearServices();
			//var serviceUuid = Guid.NewGuid();
			//var service = gattServer.CreateService(serviceUuid, true);
			//_adapter.Advertiser.Start(new Plugin.BluetoothLE.Server.AdvertisementData()
			//{
			//	LocalName = "Test Server",
			//	AndroidUseDeviceName = true,
			//	ServiceUuids = new List<Guid>() { serviceUuid }
			//});
		}

		private async Task OnServiceSelectionCommand(GattServiceItemModel item)
		{
			NavigationParameters parameters = new NavigationParameters();
			parameters.Add(ParameterConstants.SelectedService, item);

			await this.NavigationService.NavigateAsync(ViewNames.GattServerServiceDetailPage, parameters);
		}

		async Task<bool> BuildServer()
		{
			try
			{
				_gattServer = await _adapter.CreateGattServer();
				
				foreach(var gattServiceItemModel in GattServices)
				{
					var service = _gattServer.CreateService(gattServiceItemModel.ServiceUuid, true);
						
					foreach (var gattCharacteristicItemModel in gattServiceItemModel.Characteristics)
					{
						BuildCharacteristics(service, gattCharacteristicItemModel);
					}

					_gattServer.AddService(service);
				}


				this.BtnText = "Stop Broadcast";
				return true;
			}
			catch (Exception ex)
			{
				_gattServer.Dispose();
				_gattServer = null;
				await _userDialogs.AlertAsync("Error building gatt server - " + ex);
			}

			return false;
		}

		void BuildCharacteristics(Plugin.BluetoothLE.Server.IGattService service, GattServiceCharacteristicItemModel model)
		{
			var characteristic = service.AddCharacteristic(model.CharacteristicUuid, model.Properties, model.Permissions);
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			base.OnNavigatedTo(parameters);
			InitializeProperty();
		}

		private void InitializeProperty()
		{
			this.SubtitleMessage = "The GATT server corresponds to the ATT server discussed in Attribute Protocol (ATT). It receives requests from a client and sends responses back. It also sends server-initiated updates when configured to do so, and it is the role responsible for storing and making the user data available to the client, organized in attributes. Every BLE device sold must include at least a basic GATT server that can respond to client requests, even if only to return an error response.";

			if (this.GattServices == null)
			{
				this.GattServices = new List<GattServiceItemModel>();
				this.GattServices = GattServiceProvider.GetServices();
			}
		}
	}
}
