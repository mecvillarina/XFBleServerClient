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
                        new GattServiceCharacteristicItemModel() { Name = "Read Device Information", CharacteristicUuid = new Guid(AppConstants.GattCharDefaultServiceReadDevice), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write},
                        new GattServiceCharacteristicItemModel() { Name = "Say Exact Word" , CharacteristicUuid = new Guid(AppConstants.GattCharDefaultServiceSayExactWord), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write}
                    },
            });

            //gattServices.Add(new GattServiceItemModel()
            //{
            //	IsDeletable = false,
            //	IsEditable = false,
            //	Name = "Mathematical Operations",
            //	ServiceUuid = new Guid($"{AppConstants.GuidStartPad}-F12CCFB0725C"),
            //	Characteristics = new List<GattServiceCharacteristicItemModel>()
            //		{
            //			new GattServiceCharacteristicItemModel() { Name = "Addition",CharacteristicUuid = new Guid(AppConstants.GattCharMathematicalOperationsAddition), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
            //			new GattServiceCharacteristicItemModel() { Name = "Subtraction",CharacteristicUuid = new Guid(AppConstants.GattCharMathematicalOperationsSubtraction), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
            //			new GattServiceCharacteristicItemModel() { Name = "Multiplication",CharacteristicUuid = new Guid(AppConstants.GattCharMathematicalOperationsMultiplication), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
            //			new GattServiceCharacteristicItemModel() { Name = "Division",CharacteristicUuid = new Guid(AppConstants.GattCharMathematicalOperationsDivision), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
            //		}
            //});

            gattServices.Add(new GattServiceItemModel()
            {
                IsDeletable = false,
                IsEditable = false,
                Name = "Location Tracking",
                ServiceUuid = new Guid($"{AppConstants.GuidStartPad}-B77111B5B49F"),
                Characteristics = new List<GattServiceCharacteristicItemModel>()
                    {
                        new GattServiceCharacteristicItemModel() { Name = "Ask my location", CharacteristicUuid = new Guid(AppConstants.GattCharLocationTrackingAskMyLocation), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read| CharacteristicProperties.Write, Permissions = GattPermissions.Read| GattPermissions.Write },
                        new GattServiceCharacteristicItemModel() { Name = "Reverse Geocoding",CharacteristicUuid = new Guid(AppConstants.GattCharLocationTrackingReverseGeocoding), Properties = CharacteristicProperties.Notify | CharacteristicProperties.Read  | CharacteristicProperties.Write, Permissions = GattPermissions.Read | GattPermissions.Write },
                    }
            });

            return gattServices;
        }
    }
}
