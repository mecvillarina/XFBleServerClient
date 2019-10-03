using Plugin.BluetoothLE;
using Prism.Mvvm;
using System;

namespace XFBleServerClient.Core.ItemModels
{
	public class GattCharacteristicViewModel : BindableBase
	{
		IDisposable _watcher;

		public GattCharacteristicViewModel(IGattCharacteristic characteristic, GattServiceCharacteristicItemModel characteristicItemModel)
		{
			this.Characteristic = characteristic;
			this.CharacteristicItemModel = characteristicItemModel;
		}

		public IGattCharacteristic Characteristic { get; set; }
		public GattServiceCharacteristicItemModel CharacteristicItemModel { get; }

		private string _value;
		public string Value
		{
			get => _value;
			set => SetProperty(ref _value, value);
		}

		private bool _notifying;
		public bool IsNotifying
		{
			get => _notifying;
			set => SetProperty(ref _notifying, value);
		}

		private bool _valueAvailable;
		public bool IsValueAvailable
		{
			get => _valueAvailable;
			set => SetProperty(ref _valueAvailable, value);
		}

		DateTime _lastValue;
		public DateTime LastValue
		{
			get => _lastValue;
			set => SetProperty(ref _lastValue, value);
		}

		public Guid Uuid => this.Characteristic.Uuid;
		public Guid ServiceUuid => this.Characteristic.Service.Uuid;
		public string Description => this.CharacteristicItemModel.Name;
		public string Properties => this.Characteristic.Properties.ToString();
		public string ServiceName => this.CharacteristicItemModel.ServiceName;
	}
}
