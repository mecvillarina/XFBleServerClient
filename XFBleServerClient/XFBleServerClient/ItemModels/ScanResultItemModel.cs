using Plugin.BluetoothLE;
using Prism.Mvvm;
using System;
using System.Diagnostics;

namespace XFBleServerClient.Core.ItemModels
{
	public class ScanResultItemModel : BindableBase
	{
		public IDevice Device { get; private set; }

		private string _name;
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		private bool _isConnected;
		public bool IsConnected
		{
			get => _isConnected;
			set => SetProperty(ref _isConnected, value);
		}

		private Guid _uuid;
		public Guid Uuid
		{
			get => _uuid;
			set => SetProperty(ref _uuid, value);
		}

		private string _deviceUuid;
		public string DeviceUuid
		{
			get => _deviceUuid;
			set => SetProperty(ref _deviceUuid, value);
		}

		public int Rssi { get; private set; }
		public bool IsConnectable { get; private set; }
		public int ServiceCount { get; private set; }
		public string ManufacturerData { get; private set; }
		public string LocalName { get; private set; }
		public int TxPower { get; private set; }

		public bool TrySet(IScanResult result)
		{
			var response = false;

			if (this.Uuid == Guid.Empty)
			{
				this.Device = result.Device;
				this.Uuid = this.Device.Uuid;

				this.Uuid = this.Device.Uuid;
				switch (Xamarin.Forms.Device.RuntimePlatform)
				{
					case Xamarin.Forms.Device.Android:
						this.DeviceUuid = this.Device.NativeDevice.GetType().GetProperty("Address").GetValue(Device.NativeDevice).ToString();
						break;
					case Xamarin.Forms.Device.iOS:
						this.DeviceUuid = this.Uuid.ToString();
						break;
				}

				response = true;
			}

			try
			{
				if (this.Uuid == result.Device.Uuid)
				{
					response = true;

					this.Name = result.Device.Name;
					this.Name = this.Name ?? "";
					this.Rssi = result.Rssi;

					var ad = result.AdvertisementData;
					this.ServiceCount = ad.ServiceUuids?.Length ?? 0;
					this.IsConnectable = ad.IsConnectable;
					this.LocalName = ad.LocalName;
					this.TxPower = ad.TxPower;
					this.ManufacturerData = ad.ManufacturerData == null
						? null
						: BitConverter.ToString(ad.ManufacturerData);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return response;
		}
	}
}
