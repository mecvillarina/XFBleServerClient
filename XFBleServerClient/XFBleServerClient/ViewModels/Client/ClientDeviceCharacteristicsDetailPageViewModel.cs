using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using XFBleServerClient.Core.Common;
using XFBleServerClient.Core.ItemModels;

namespace XFBleServerClient.Core.ViewModels
{
    public class ClientDeviceCharacteristicsDetailPageViewModel : ViewModelBase
    {
        public ClientDeviceCharacteristicsDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        private GattCharacteristicViewModel _gattCharacteristic;
        public GattCharacteristicViewModel GattCharacteristic
        {
            get => _gattCharacteristic;
            set => SetProperty(ref _gattCharacteristic, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            GattCharacteristic = parameters.GetValue<GattCharacteristicViewModel>(ParameterConstants.SelectedGattCharacteristic);
        }
    }
}
