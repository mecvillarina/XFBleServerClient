using Plugin.BluetoothLE;
using Plugin.DeviceInfo.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
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
            this.ConnectCommand = new DelegateCommand(async () => await OnConnectCommandAsync());

            DeactivateWith = new CompositeDisposable();
            DeactivateCharacteristicWith = new CompositeDisposable();
        }

        private IDevice _selectedDevice;

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

        protected CompositeDisposable DeactivateWith { get; set; }
        protected CompositeDisposable DeactivateCharacteristicWith { get; set; }

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

        private async Task OnConnectCommandAsync()
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
            switch (gattCharacteristic.Uuid.ToString().ToUpper())
            {
                case AppConstants.GattCharDefaultServiceReadDevice: await ReadDevice(gattCharacteristic); break;
                case AppConstants.GattCharDefaultServiceSayExactWord: break;
                case AppConstants.GattCharLocationTrackingAskMyLocation: break;
                case AppConstants.GattCharLocationTrackingReverseGeocoding: break;
            }
        }

        private async Task ReadDevice(IGattCharacteristic gattCharacteristic)
        {
            ResultStr = string.Empty;

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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _selectedDevice = parameters.GetValue<IDevice>(ParameterConstants.SelectedDevice);
            GattCharacteristic = parameters.GetValue<GattCharacteristicViewModel>(ParameterConstants.SelectedGattCharacteristic);

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
                                   this.ConnectMessage = "Stop";
                                   DeactivateCharacteristicWith?.Dispose();
                                   DeactivateCharacteristicWith = new CompositeDisposable();

                                   Guid serviceUuid = GattCharacteristic.Uuid;
                                   Guid characteristicUuid = GattCharacteristic.Characteristic.Uuid;

                                   var gattCharacteristics =  await _selectedDevice.WhenKnownCharacteristicsDiscovered(serviceUuid, characteristicUuid);

                                   await PerformTask(gattCharacteristics);
                               }
                               break;

                           case ConnectionStatus.Disconnected:
                               this.ConnectMessage = "Execute";
                               break;
                       }
                   })
                   .DisposeWith(this.DeactivateWith);
        }

        public override void Destroy()
        {
            Dispose();
            base.Destroy();
        }
    }
}
