using Plugin.BluetoothLE;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        private CompositeDisposable _deactivateWith;
        protected CompositeDisposable DeactivateWith
        {
            get
            {
                if (_deactivateWith == null)
                {
                    _deactivateWith = new CompositeDisposable();
                }

                return _deactivateWith;
            }
        }

        protected CompositeDisposable DeactivateCharacteristicWith { get; set; }

        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand ConnectCommand { get; private set; }
        private async Task OnBackCommandAsync()
        {
            _selectedDevice?.CancelConnection();
            DeactivateCharacteristicWith?.Dispose();
            DeactivateWith?.Dispose();
            await this.NavigationService.GoBackAsync();
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

        private async Task PerformTask()
        {
            switch (GattCharacteristic.Characteristic.Uuid.ToString().ToUpper())
            {
                case AppConstants.GattCharDefaultServiceReadDevice: await ReadDevice(); break;
                case AppConstants.GattCharDefaultServiceSayExactWord: break;
                case AppConstants.GattCharLocationTrackingAskMyLocation: break;
                case AppConstants.GattCharLocationTrackingReverseGeocoding: break;
            }
        }

        private async Task ReadDevice()
        {
            GattCharacteristic.Characteristic.Read().Subscribe((result) =>
            {

            }).DisposeWith(DeactivateCharacteristicWith);
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
                               this.ConnectMessage = "Connecting...";
                               break;

                           case ConnectionStatus.Connected:
                               {
                                   this.ConnectMessage = "Stop";
                                   DeactivateCharacteristicWith?.Dispose();
                                   DeactivateCharacteristicWith = new CompositeDisposable();
                                   await PerformTask();
                               }
                               break;

                           case ConnectionStatus.Disconnected:
                               this.ConnectMessage = "Start";
                               break;
                       }
                   })
                   .DisposeWith(this.DeactivateWith);
        }

        public override void Destroy()
        {
            _selectedDevice?.CancelConnection();
            DeactivateCharacteristicWith?.Dispose();
            DeactivateWith?.Dispose();
            base.Destroy();
        }
    }
}
