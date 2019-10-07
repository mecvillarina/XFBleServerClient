using Plugin.BluetoothLE;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;
using XFBleServerClient.Core.ItemModels;

namespace XFBleServerClient.Core.ViewModels
{
    public class ClientDeviceCharacteristicsDetailPageViewModel : ViewModelBase
    {
        private readonly IGeolocator _geolocatorUtil;
        public ClientDeviceCharacteristicsDetailPageViewModel(INavigationService navigationService, IGeolocator geolocatorUtil) : base(navigationService)
        {
            _geolocatorUtil = geolocatorUtil;

            this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
            this.ConnectCommand = new DelegateCommand(() => OnConnectCommand(), () => this.CanExecuteConnectCommand).ObservesProperty(() => this.CanExecuteConnectCommand);

            this.DeactivateWith = new CompositeDisposable();
            this.DeactivateCharacteristicWith = new CompositeDisposable();
        }

        private IDevice _selectedDevice;

        private Guid _serviceGuid;
        private Guid _characteristicGuid;
        private IGattCharacteristic _characteristic;

        protected CompositeDisposable DeactivateWith { get; set; }
        protected CompositeDisposable DeactivateCharacteristicWith { get; set; }

        private GattCharacteristicViewModel _gattCharacteristic;
        public GattCharacteristicViewModel GattCharacteristic
        {
            get => _gattCharacteristic;
            set => SetProperty(ref _gattCharacteristic, value);
        }

        private string _connectMessage = "Start";
        public string ConnectMessage
        {
            get => _connectMessage;
            set => SetProperty(ref _connectMessage, value);
        }

        private string _resultStr;
        public string ResultStr
        {
            get => _resultStr;
            set => SetProperty(ref _resultStr, value);
        }

        private bool _canExecuteConnectCommand = true;
        public bool CanExecuteConnectCommand
        {
            get => _canExecuteConnectCommand;
            set => SetProperty(ref _canExecuteConnectCommand, value);
        }

        private bool _showWordEntry;
        public bool ShowWordEntry
        {
            get => _showWordEntry;
            set => SetProperty(ref _showWordEntry, value);
        }

        private string _wordEntry = string.Empty;
        public string WordEntry
        {
            get => _wordEntry;
            set => SetProperty(ref _wordEntry, value);
        }

        private bool _showLocationEntry;
        public bool ShowLocationEntry
        {
            get => _showLocationEntry;
            set => SetProperty(ref _showLocationEntry, value);
        }

        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand ConnectCommand { get; private set; }
        private async Task OnBackCommandAsync()
        {
            Dispose();
            await this.NavigationService.GoBackAsync();
        }

        private void Dispose()
        {
            _selectedDevice?.CancelConnection();
            this.DeactivateCharacteristicWith?.Dispose();
            this.DeactivateWith?.Dispose();
        }

        private void OnConnectCommand()
        {
            _selectedDevice.CancelConnection();
            _selectedDevice.Connect(new ConnectionConfig()
            {
                AndroidConnectionPriority = ConnectionPriority.Normal,
                AutoConnect = false
            });
        }

        private async Task PerformTask(IGattCharacteristic gattCharacteristic)
        {
            if (gattCharacteristic != null)
            {
                this.ResultStr = string.Empty;

                switch (gattCharacteristic.Uuid.ToString().ToUpper())
                {
                    case AppConstants.GattCharDefaultServiceReadDevice: await ReadDevice(gattCharacteristic); break;
                    case AppConstants.GattCharDefaultServiceSayExactWord: await SayExactWord(gattCharacteristic); break;
                    case AppConstants.GattCharLocationTrackingAskMyLocation: await AskLocation(gattCharacteristic); break;
                    case AppConstants.GattCharLocationTrackingReverseGeocoding: await GetLocationName(gattCharacteristic); break;
                }
            }
        }

        private async Task ReadDevice(IGattCharacteristic gattCharacteristic)
        {
            var gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.DeviceVersion));
            var gattReadResult = await gattCharacteristic.Read();
            this.ResultStr = string.Concat(this.ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.VersionNumber));
            gattReadResult = await gattCharacteristic.Read();
            this.ResultStr = string.Concat(this.ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.Manufacturer));
            gattReadResult = await gattCharacteristic.Read();
            this.ResultStr = string.Concat(this.ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.DeviceName));
            gattReadResult = await gattCharacteristic.Read();
            this.ResultStr = string.Concat(this.ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            _selectedDevice?.CancelConnection();
        }

        private async Task SayExactWord(IGattCharacteristic gattCharacteristic)
        {
            var gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(this.WordEntry));
            var gattReadResult = await gattCharacteristic.Read();
            this.ResultStr = string.Concat(this.ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            _selectedDevice?.CancelConnection();
        }

        private async Task AskLocation(IGattCharacteristic gattCharacteristic)
        {
            var gattReadResult = await gattCharacteristic.Read();
            string gattResult = Encoding.UTF8.GetString(gattReadResult.Data);
            this.ResultStr = $"Latitude: {gattResult.Split('|')[0]}{Environment.NewLine}Longitude: {gattResult.Split('|')[1]}";

            _selectedDevice?.CancelConnection();
        }

        private async Task GetLocationName(IGattCharacteristic gattCharacteristic)
        {
            string message = $"{this.Latitude.ToString("N4")}|{this.Longitude.ToString("N4")}";
            var gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(message));
            var gattReadResult = await gattCharacteristic.Read();
            this.ResultStr = string.Concat(this.ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            _selectedDevice?.CancelConnection();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _selectedDevice = parameters.GetValue<IDevice>(ParameterConstants.SelectedDevice);
            this.GattCharacteristic = parameters.GetValue<GattCharacteristicViewModel>(ParameterConstants.SelectedGattCharacteristic);

            _serviceGuid = this.GattCharacteristic.ServiceUuid;
            _characteristicGuid = this.GattCharacteristic.Characteristic.Uuid;

            await InitializeUI();

            _selectedDevice
                   .WhenStatusChanged()
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Subscribe(async status =>
                   {
                       switch (status)
                       {
                           case ConnectionStatus.Connecting:
                               this.ConnectMessage = "Executing...";
                               break;

                           case ConnectionStatus.Connected:
                               {
                                   this.CanExecuteConnectCommand = false;
                                   //this.ConnectMessage = "Stop";
                                   this.DeactivateCharacteristicWith?.Dispose();
                                   this.DeactivateCharacteristicWith = new CompositeDisposable();
                                   _characteristic = null;
                                   await _selectedDevice.DiscoverServices();
                               }
                               break;

                           case ConnectionStatus.Disconnected:
                               this.ConnectMessage = "Execute";
                               this.CanExecuteConnectCommand = true;
                               break;
                       }
                   })
                   .DisposeWith(this.DeactivateWith);

            _selectedDevice
                .WhenAnyCharacteristicDiscovered()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async chs =>
                {
                    var b = chs.Service.Uuid.ToString().ToUpper().Equals(_serviceGuid.ToString().ToUpper()) && chs.Uuid.ToString().ToUpper().Equals(_characteristicGuid.ToString().ToUpper());
                    Debug.WriteLine("Status:" + b);
                    Debug.WriteLine("Charac Guid:" + chs.Uuid.ToString().ToUpper());
                    Debug.WriteLine("Service Guid:" + chs.Service.Uuid.ToString().ToUpper());

                    Debug.WriteLine("My Charac Guid" + _characteristicGuid.ToString().ToUpper());
                    Debug.WriteLine("My Service Guid" + _serviceGuid.ToString().ToUpper());
                    if (chs.Uuid.Equals(_characteristicGuid) && chs.Service.Uuid.Equals(_serviceGuid))
                    {
                        _characteristic = chs;
                        await PerformTask(_characteristic);
                    }
                })
                .DisposeWith(this.DeactivateWith);
        }

        private async Task InitializeUI()
        {
            this.ShowWordEntry = false;
            this.ShowLocationEntry = false;
            if (_characteristicGuid.ToString().ToUpper().Equals(AppConstants.GattCharDefaultServiceSayExactWord.ToUpper()))
            {
                this.ShowWordEntry = true;
            }
            else if (_characteristicGuid.ToString().ToUpper().Equals(AppConstants.GattCharLocationTrackingReverseGeocoding.ToUpper()))
            {
                var location = await _geolocatorUtil.GetPositionAsync();
                this.Latitude = location.Latitude;
                this.Longitude = location.Longitude;

                this.ShowLocationEntry = true;
            }
        }

        public override void Destroy()
        {
            Dispose();
            base.Destroy();
        }
    }
}
