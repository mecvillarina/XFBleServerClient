using System;
using System.Collections.Generic;
using System.Text;

namespace XFBleServerClient.Core.Common
{
    public static class AppConstants
    {
		public const string Server = "Server";
		public const string Client = "Client";

		public const string GuidStartPad = "B78F4D10-7438-4CF5-A576";

		public const string GattCharDefaultServiceReadDevice = "B78F4D11-7438-4CF5-A576-1370F02D74DE";
		public const string GattCharDefaultServiceSayExactWord = "B78F4D12-7438-4CF5-A576-1370F02D74DE";

		public const string GattCharMathematicalOperationsAddition = "633C1281-A2D9-4A73-A5F3-F12CCFB0725C";
		public const string GattCharMathematicalOperationsSubtraction = "633C1282-A2D9-4A73-A5F3-F12CCFB0725C";
		public const string GattCharMathematicalOperationsMultiplication = "633C1283-A2D9-4A73-A5F3-F12CCFB0725C";
		public const string GattCharMathematicalOperationsDivision = "633C1284-A2D9-4A73-A5F3-F12CCFB0725C";

		public const string GattCharLocationTrackingAskMyLocation = "064B5B51-6ED8-403E-B158-B77111B5B49F";
		public const string GattCharLocationTrackingReverseGeocoding = "064B5B52-6ED8-403E-B158-B77111B5B49F";



	}
}
