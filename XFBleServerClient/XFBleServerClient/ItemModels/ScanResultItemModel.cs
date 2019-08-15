using Plugin.BluetoothLE;
using Prism.Mvvm;
using System;
using System.Diagnostics;

namespace XFBleServerClient.Core.ItemModels
{
	public class ScanResultItemModel : BindableBase
	{
		public IDevice Device { get; private set; }

		public string Name { get; private set; }
		public bool IsConnected { get; private set; }
		public Guid Uuid { get; private set; }
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
