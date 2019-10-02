using Plugin.BluetoothLE;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;
using XFBleServerClient.Core.Infrastructure;
using XFBleServerClient.Core.ItemModels;
using XFBleServerClient.Core.Providers;

namespace XFBleServerClient.Core.ViewModels
{
    public class ClientDeviceDetailPageViewModel : ViewModelBase
    {
        public ClientDeviceDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
            this.ConnectCommand = new DelegateCommand(() => OnConnectCommand());
            SelectGattCharacteristicCommand = new DelegateCommand<GattCharacteristicViewModel>(async (characteristic) => await OnSelectGattCharacteristicCommand(characteristic));

        }

        private IDevice _selectedDevice;
        private List<GattServiceCharacteristicItemModel> _gattServiceCharacterItemModels;

        private CompositeDisposable deactivateWith;
        protected CompositeDisposable DeactivateWith
        {
            get
            {
                if (deactivateWith == null)
                {
                    deactivateWith = new CompositeDisposable();
                }

                return deactivateWith;
            }
        }

        private string _connectMessage = "Connect";
        public string ConnectMessage
        {
            get => _connectMessage;
            set => SetProperty(ref _connectMessage, value);
        }

        private ObservableCollection<Group<GattCharacteristicViewModel>> _gattCharacteristics;
        public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics
        {
            get => _gattCharacteristics;
            set => SetProperty(ref _gattCharacteristics, value);
        }

        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand<GattCharacteristicViewModel> SelectGattCharacteristicCommand { get; private set; }

        private async Task OnBackCommandAsync()
        {
            _selectedDevice?.CancelConnection();
            DeactivateWith?.Dispose();
            await this.NavigationService.GoBackAsync();
        }

        private void OnConnectCommand()
        {
            if (_selectedDevice.Status == ConnectionStatus.Disconnected)
            {
                _selectedDevice.CancelConnection();
                _selectedDevice.Connect(new ConnectionConfig()
                {
                    AndroidConnectionPriority = ConnectionPriority.Normal,
                    AutoConnect = false
                });
            }
            else
            {
                _selectedDevice.CancelConnection();
            }
        }

        private async Task OnSelectGattCharacteristicCommand(GattCharacteristicViewModel characteristic)
        {
            var parameters = new NavigationParameters();
            parameters.Add(ParameterConstants.SelectedGattCharacteristic, characteristic);
            parameters.Add(ParameterConstants.SelectedDevice, _selectedDevice);
            await this.NavigationService.NavigateAsync(ViewNames.ClientDeviceCharacteristicsDetailPage, parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey(ParameterConstants.SelectedDevice))
                {
                    _selectedDevice = parameters.GetValue<IDevice>(ParameterConstants.SelectedDevice);
                }

                var gattServiceItemModel = GattServiceProvider.GetServices().Select(x =>
                {
                    foreach (var characteristic in x.Characteristics)
                    {
                        characteristic.ServiceName = x.Name;
                    }

                    return x.Characteristics;
                });

                _gattServiceCharacterItemModels = new List<GattServiceCharacteristicItemModel>();

                foreach (var model in gattServiceItemModel)
                {
                    _gattServiceCharacterItemModels.AddRange(model);
                }


                if (GattCharacteristics == null)
                {
                    GattCharacteristics = new ObservableCollection<Group<GattCharacteristicViewModel>>();
                }

                _selectedDevice
                    .WhenStatusChanged()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(status =>
                    {
                        switch (status)
                        {
                            case ConnectionStatus.Connecting:
                                this.ConnectMessage = "Cancel Connection";
                                break;

                            case ConnectionStatus.Connected:
                                this.ConnectMessage = "Disconnect";
                                break;

                            case ConnectionStatus.Disconnected:
                                this.ConnectMessage = "Connect";
                                try
                                {
                                //this.GattCharacteristics.Clear();
                            }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }

                                break;
                        }
                    })
                    .DisposeWith(this.DeactivateWith);

                _selectedDevice
                    .WhenAnyCharacteristicDiscovered()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(chs =>
                    {
                        try
                        {
                            Debug.WriteLine("Services:" + chs.Service.Uuid);
                            var b = chs.Service.Uuid.ToString().ToUpper().StartsWith(AppConstants.GuidStartPad);
                            Debug.WriteLine("Status:" + b);

                            if (chs.Service.Uuid.ToString().ToUpper().StartsWith(AppConstants.GuidStartPad))
                            {
                                var chsItemModel = _gattServiceCharacterItemModels.FirstOrDefault(x => x.CharacteristicUuid == chs.Uuid);

                                var service = this.GattCharacteristics.FirstOrDefault(x => x.ShortName.Equals(chs.Service.Uuid.ToString()));
                                if (service == null)
                                {
                                    service = new Group<GattCharacteristicViewModel>(
                                        $"{chsItemModel.ServiceName}{Environment.NewLine}({chs.Service.Uuid})",
                                        chs.Service.Uuid.ToString()
                                    );
                                    this.GattCharacteristics.Add(service);
                                }

                                if (!service.Any(x => x.Uuid == chs.Uuid))
                                {
                                    service.Add(new GattCharacteristicViewModel(chs, chsItemModel));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        // eat it
                        Console.WriteLine(ex);
                        }
                    })
                    .DisposeWith(this.DeactivateWith);
            }
        }

        public override void Destroy()
        {
            _selectedDevice?.CancelConnection();
            DeactivateWith?.Dispose();
            base.Destroy();
        }
    }
}
