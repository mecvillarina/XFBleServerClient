using Plugin.BluetoothLE;
using Plugin.DeviceInfo.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public ClientDeviceCharacteristicsDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {

            this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
            this.ConnectCommand = new DelegateCommand(() => OnConnectCommand(), () => CanExecuteConnectCommand).ObservesProperty(() => CanExecuteConnectCommand);

            DeactivateWith = new CompositeDisposable();
            DeactivateCharacteristicWith = new CompositeDisposable();
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
            DeactivateCharacteristicWith?.Dispose();
            DeactivateWith?.Dispose();
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
                ResultStr = string.Empty;

                switch (gattCharacteristic.Uuid.ToString().ToUpper())
                {
                    case AppConstants.GattCharDefaultServiceReadDevice: await ReadDevice(gattCharacteristic); break;
                    case AppConstants.GattCharDefaultServiceSayExactWord: await SayExactWord(gattCharacteristic); break;
                    case AppConstants.GattCharLocationTrackingAskMyLocation: break;
                    case AppConstants.GattCharLocationTrackingReverseGeocoding: break;
                }
            }
        }

        private async Task ReadDevice(IGattCharacteristic gattCharacteristic)
        {
            var gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.DeviceVersion));
            var gattReadResult = await gattCharacteristic.Read();
            ResultStr = string.Concat(ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.VersionNumber));
            gattReadResult = await gattCharacteristic.Read();
            ResultStr = string.Concat(ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.Manufacturer));
            gattReadResult = await gattCharacteristic.Read();
            ResultStr = string.Concat(ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(AppConstants.DeviceName));
            gattReadResult = await gattCharacteristic.Read();
            ResultStr = string.Concat(ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);

            _selectedDevice?.CancelConnection();
        }

        private async Task SayExactWord(IGattCharacteristic gattCharacteristic)
        {
            var gattWriteResult = await gattCharacteristic.Write(Encoding.UTF8.GetBytes(WordEntry));
            var gattReadResult = await gattCharacteristic.Read();
            ResultStr = string.Concat(ResultStr, System.Text.Encoding.UTF8.GetString(gattReadResult.Data), Environment.NewLine);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _selectedDevice = parameters.GetValue<IDevice>(ParameterConstants.SelectedDevice);
            GattCharacteristic = parameters.GetValue<GattCharacteristicViewModel>(ParameterConstants.SelectedGattCharacteristic);

            _serviceGuid = GattCharacteristic.ServiceUuid;
            _characteristicGuid = GattCharacteristic.Characteristic.Uuid;

            InitializeUI();

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
                                   CanExecuteConnectCommand = false;
                                   //this.ConnectMessage = "Stop";
                                   DeactivateCharacteristicWith?.Dispose();
                                   DeactivateCharacteristicWith = new CompositeDisposable();
                                   _characteristic = null;
                                   await _selectedDevice.DiscoverServices();
                               }
                               break;

                           case ConnectionStatus.Disconnected:
                               this.ConnectMessage = "Execute";
                               CanExecuteConnectCommand = true;
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

        private void InitializeUI()
        {
            ShowWordEntry = false;
            if (_characteristicGuid.ToString().ToUpper().Equals(AppConstants.GattCharDefaultServiceSayExactWord.ToUpper()))
            {
                ShowWordEntry = true;
            }
        }

        public override void Destroy()
        {
            Dispose();
            base.Destroy();
        }
    }
}
