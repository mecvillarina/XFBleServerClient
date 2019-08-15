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

			this.ScanCommand = new DelegateCommand(() => OnScanCommand());

			Devices = new List<ScanResultItemModel>();
		}


		IDisposable scan;
		CompositeDisposable deactivateWith;
		protected CompositeDisposable DeactivateWith
		{
			get
			{
				if (this.deactivateWith == null)
					this.deactivateWith = new CompositeDisposable();

				return this.deactivateWith;
			}
		}

		private bool _isScanning;
		public bool IsScanning
		{
			get => _isScanning;
			set => SetProperty(ref _isScanning, value);
		}
		private List<ScanResultItemModel> _devices;
		public List<ScanResultItemModel> Devices
		{
			get => _devices;
			set => SetProperty(ref _devices, value);
		}

		public DelegateCommand ScanCommand { get; private set; }

		private void OnScanCommand()
		{
			if (this.IsScanning)
			{
				scan?.Dispose();
				this.IsScanning = false;
			}
			else
			{
				this.Devices.Clear();

				this.IsScanning = true;
				scan = this
					._adapter
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

							list = list.GroupBy(x => x.Uuid).Select(x => x.First()).ToList();
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
	}
}