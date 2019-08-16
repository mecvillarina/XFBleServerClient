using Plugin.BluetoothLE;
using Plugin.BluetoothLE.Server;
using System;
using System.Collections.Generic;
using XFBleServerClient.Core.Common;
using XFBleServerClient.Core.ItemModels;

namespace XFBleServerClient.Core.Providers
{
	public static class GattServiceProvider
	{
		public static List<GattServiceItemModel> GetServices()
		{
			var gattServices = new List<GattServiceItemModel>();

			gattServices.Add(new GattServiceItemModel()
			{
				Name = "Default Service",
				ServiceUuid = new Guid($"{AppConstants.GuidStartPad}-1370F02D74DE"),
				Characteristics = new List<GattServiceCharacteristicItemModel>()
					{
						new GattServiceCharacteristicItemModel() { Name = "Read Device Information", CharacteristicUuid = new Guid("B78F4D11-7438-4CF5-A576-1370F02D74DE"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read, Permissions = GattPermissions.Read },
						new GattServiceCharacteristicItemModel() { Name = "Say Exact Word" , CharacteristicUuid = new Guid("B78F4D12-7438-4CF5-A576-1370F02D74DE"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write}
					},
			});

			gattServices.Add(new GattServiceItemModel()
			{
				IsDeletable = false,
				IsEditable = false,
				Name = "Mathematical Operations",
				ServiceUuid = new Guid($"{AppConstants.GuidStartPad}-F12CCFB0725C"),
				Characteristics = new List<GattServiceCharacteristicItemModel>()
					{
						new GattServiceCharacteristicItemModel() { Name = "Addition",CharacteristicUuid = new Guid("633C1281-A2D9-4A73-A5F3-F12CCFB0725C"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
						new GattServiceCharacteristicItemModel() { Name = "Subtraction",CharacteristicUuid = new Guid("633C1282-A2D9-4A73-A5F3-F12CCFB0725C"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
						new GattServiceCharacteristicItemModel() { Name = "Multiplication",CharacteristicUuid = new Guid("633C1283-A2D9-4A73-A5F3-F12CCFB0725C"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
						new GattServiceCharacteristicItemModel() { Name = "Division",CharacteristicUuid = new Guid("633C1284-A2D9-4A73-A5F3-F12CCFB0725C"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
					}
			});

			gattServices.Add(new GattServiceItemModel()
			{
				IsDeletable = false,
				IsEditable = false,
				Name = "Location Tracking",
				ServiceUuid = new Guid($"{AppConstants.GuidStartPad}-B77111B5B49F"),
				Characteristics = new List<GattServiceCharacteristicItemModel>()
					{
						new GattServiceCharacteristicItemModel() { Name = "Ask my location", CharacteristicUuid = new Guid("064B5B51-6ED8-403E-B158-B77111B5B49F"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read, Permissions = GattPermissions.Read },
						new GattServiceCharacteristicItemModel() { Name = "Reverse Geocoding",CharacteristicUuid = new Guid("064B5B52-6ED8-403E-B158-B77111B5B49F"), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
					}
			});

			return gattServices;
		}
	}
}
