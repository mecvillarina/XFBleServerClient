using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace XFBleServerClient.Core.Views
{
	public class ContentPageBase : ContentPage
    {
		public ContentPageBase()
		{
			Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

			On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					Padding = new Thickness(0, 20, 0, 0);
					break;
				default:
					Padding = new Thickness(0);
					break;
			}

			this.Visual = VisualMarker.Material;
		}
	}
}
