using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Plugin.BluetoothLE.Server;
using Plugin.DeviceInfo.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
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
        private readonly IDeviceInfo _deviceInfoUtil;
        public GattServerSetupPageViewModel(INavigationService navigationService, IAdapter adapter, IDialogService dialogService, IUserDialogs userDialogs, IDeviceInfo deviceInfoUtil) : base(navigationService)
        {
            _adapter = adapter;
            _dialogService = dialogService;
            _userDialogs = userDialogs;
            _deviceInfoUtil = deviceInfoUtil;

            this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
            this.SelectInfoCommand = new DelegateCommand(async () => await ExecuteSelectInfoCommand());
            this.BroadcastCommand = new DelegateCommand(async () => await OnBroadcastCommandAsync(), () => !string.IsNullOrWhiteSpace(this.ServerName)).ObservesProperty(() => this.ServerName);
            this.ServiceSelectionCommand = new DelegateCommand<GattServiceItemModel>(async (item) => await OnServiceSelectionCommand(item));

            DeactivateCharacteristicWith = new CompositeDisposable();
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

        private string _currentWriteCommand;
        protected CompositeDisposable DeactivateCharacteristicWith { get; set; }

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
            DisposeBLE();
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
                DeactivateCharacteristicWith = new CompositeDisposable();

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
                DisposeBLE();
            }
        }

        private void DisposeBLE()
        {
            try
            {
                DeactivateCharacteristicWith?.Dispose();
                _adapter.Advertiser.Stop();
                _gattServer.Dispose();
                _gattServer = null;
            }
            catch
            {

            }
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

                foreach (var gattServiceItemModel in GattServices)
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
            characteristic.AddDescriptor(model.CharacteristicUuid, Encoding.UTF8.GetBytes(model.CharacteristicUuid.ToString()));

            switch (model.CharacteristicUuid.ToString().ToUpper())
            {
                case AppConstants.GattCharDefaultServiceReadDevice: GattCharReadDevice(characteristic); break;
                case AppConstants.GattCharDefaultServiceSayExactWord: GattCharSayExactWord(characteristic); break;
                case AppConstants.GattCharLocationTrackingAskMyLocation: break;
                case AppConstants.GattCharLocationTrackingReverseGeocoding: break;
            }
        }

        void GattCharReadDevice(Plugin.BluetoothLE.Server.IGattCharacteristic characteristic)
        {
            characteristic.WhenWriteReceived().Subscribe(writeRequest =>
            {
                _currentWriteCommand = Encoding.UTF8.GetString(writeRequest.Value);
                writeRequest.Status = GattStatus.Success;

            }).DisposeWith(DeactivateCharacteristicWith);

            characteristic.WhenReadReceived().Subscribe(readRequest =>
            {
                switch (_currentWriteCommand)
                {
                    case AppConstants.DeviceVersion: readRequest.Value = Encoding.UTF8.GetBytes(_deviceInfoUtil.Version); break;
                    case AppConstants.VersionNumber: readRequest.Value = Encoding.UTF8.GetBytes(_deviceInfoUtil.VersionNumber.ToString()); break;
                    case AppConstants.Manufacturer: readRequest.Value = Encoding.UTF8.GetBytes(_deviceInfoUtil.Manufacturer); break;
                    case AppConstants.DeviceName: readRequest.Value = Encoding.UTF8.GetBytes(_deviceInfoUtil.DeviceName); break;
                }

                readRequest.Status = GattStatus.Success;

            }).DisposeWith(DeactivateCharacteristicWith);
        }

        void GattCharSayExactWord(Plugin.BluetoothLE.Server.IGattCharacteristic characteristic)
        {
            characteristic.WhenWriteReceived().Subscribe(writeRequest =>
            {
                _currentWriteCommand = Encoding.UTF8.GetString(writeRequest.Value);
                writeRequest.Status = GattStatus.Success;

            }).DisposeWith(DeactivateCharacteristicWith);

            characteristic.WhenReadReceived().Subscribe(readRequest =>
            {
                readRequest.Value = Encoding.UTF8.GetBytes(_currentWriteCommand);
                readRequest.Status = GattStatus.Success;

            }).DisposeWith(DeactivateCharacteristicWith);
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
