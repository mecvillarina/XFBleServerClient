using Acr.Collections;
using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Prism.Commands;
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
	public class ClientDeviceListPageViewModel : ViewModelBase
	{
		private readonly IAdapter _adapter;
		private readonly IUserDialogs _userDialogs;

		public ClientDeviceListPageViewModel(INavigationService navigationService, IUserDialogs userDialogs, IAdapter adapter) : base(navigationService)
		{
			_userDialogs = userDialogs;
			_adapter = adapter;

			this.BackCommand = new DelegateCommand(async () => await OnBackCommandAsync());
			this.ScanCommand = new DelegateCommand(() => OnScanCommand());
			this.ServerSelectionCommand = new DelegateCommand<ScanResultItemModel>(async (model) => await OnServerSelectionCommandAsync(model));
		}



		IDisposable _scan;
		CompositeDisposable deactivateWith;
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

		private bool _isScanning;
		public bool IsScanning
		{
			get => _isScanning;
			set => SetProperty(ref _isScanning, value);
		}

		private string _scanMessage = "Scan";
		public string ScanMessage
		{
			get => _scanMessage;
			set => SetProperty(ref _scanMessage, value);
		}

		public ObservableList<ScanResultItemModel> Devices { get; } = new ObservableList<ScanResultItemModel>();

		public DelegateCommand BackCommand { get; private set; }
		public DelegateCommand ScanCommand { get; private set; }
		public DelegateCommand<ScanResultItemModel> ServerSelectionCommand { get; private set; }

		private async Task OnBackCommandAsync()
		{
			StopScanning();
			DeactivateWith.Clear();
			await this.NavigationService.GoBackAsync();
		}

		private void OnScanCommand()
		{
			if (this.IsScanning)
			{
				StopScanning();
			}
			else
			{
				this.Devices.Clear();
				this.ScanMessage = "Stop Scan";
				this.IsScanning = true;
				_scan =
					_adapter
					.Scan()
					.Buffer(TimeSpan.FromSeconds(1))
					.ObserveOn(RxApp.MainThreadScheduler)
					.Subscribe(
						results =>
						{
							var list = new List<ScanResultItemModel>();
							foreach (var result in results)
							{
								var dev = this.Devices.FirstOrDefault(x => x.Uuid.Equals(result.Device.Uuid));

								if (dev != null)
								{
									dev.TrySet(result);
								}
								else
								{
									dev = new ScanResultItemModel();
									dev.TrySet(result);
									list.Add(dev);
								}
							}

							list = list.GroupBy(x => x.Uuid).Select(x => x.FirstOrDefault()).ToList();
							//list = list.Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
							//list = list.Where(x => x.Name.ToLower().Contains("biral_")).ToList();

							if (list.Any())
							{
								this.Devices.AddRange(list);
							}
						},
						ex => _userDialogs.Alert(ex.ToString(), "ERROR")
					)
					.DisposeWith(this.DeactivateWith);
			}

		}

		private void StopScanning()
		{
			if (this.IsScanning)
			{
				_scan?.Dispose();
				this.IsScanning = false;
				this.ScanMessage = "Scan";
			}
		}

		private async Task OnServerSelectionCommandAsync(ScanResultItemModel model)
		{
			StopScanning();

			var parameters = new NavigationParameters();
			parameters.Add(ParameterConstants.SelectedDevice, model.Device);
			await this.NavigationService.NavigateAsync(ViewNames.ClientDeviceDetailPage, parameters);
		}


	}
}