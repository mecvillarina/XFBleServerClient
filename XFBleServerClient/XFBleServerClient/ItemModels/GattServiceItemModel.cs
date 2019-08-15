using Plugin.BluetoothLE;
using Plugin.BluetoothLE.Server;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace XFBleServerClient.Core.ItemModels
{
	public class GattServiceItemModel : BindableBase
	{
		private Guid _serviceUuid;
		public Guid ServiceUuid
		{
			get => _serviceUuid;
			set => SetProperty(ref _serviceUuid, value);
		}

		private string _name;
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		private bool _isDeletable;
		public bool IsDeletable
		{
			get => _isDeletable;
			set => SetProperty(ref _isDeletable, value);
		}

		private bool _isEditable;
		public bool IsEditable
		{
			get => _isEditable;
			set => SetProperty(ref _isEditable, value);
		}

		private List<GattServiceCharacteristicItemModel> _characteristics = new List<GattServiceCharacteristicItemModel>();
		public List<GattServiceCharacteristicItemModel> Characteristics
		{
			get => _characteristics;
			set => SetProperty(ref _characteristics, value);
		}
	}

	public class GattServiceCharacteristicItemModel : BindableBase
	{
		private Guid _characteristicUuid;
		public Guid CharacteristicUuid
		{
			get => _characteristicUuid;
			set => SetProperty(ref _characteristicUuid, value);
		}

		private string _name;
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		public CharacteristicProperties Properties { get; set; }
		public GattPermissions Permissions { get; set; }

		public Action<object> ReadRequestAction { get; set; }
		public Action<object> WriteRequestAction { get; set; }

		public string PropertiesDisplay
		{
			get => $"Characteristic Properties: {Properties.ToString()}"; 
		}

		public string PermissionsDisplay
		{
			get => $"Gatt Permissions: {Permissions.ToString()}";
		}
	}
}
