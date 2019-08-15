using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;
using XFBleServerClient.Core.ItemModels;

namespace XFBleServerClient.Core.ViewModels
{
	public class GattServerServiceDetailPageViewModel : ViewModelBase
	{
		public GattServerServiceDetailPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
		}


		private GattServiceItemModel _selectedService;
		public GattServiceItemModel SelectedService
		{
			get => _selectedService;
			set => SetProperty(ref _selectedService, value);
		}

		public DelegateCommand BackCommand { get; private set; }

		private async Task OnBackCommandAsync()
		{
			await this.NavigationService.GoBackAsync();
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			base.OnNavigatedTo(parameters);

			if (parameters.ContainsKey(ParameterConstants.SelectedService))
			{
				SelectedService = parameters.GetValue<GattServiceItemModel>(ParameterConstants.SelectedService);
			}
		}
	}
}
