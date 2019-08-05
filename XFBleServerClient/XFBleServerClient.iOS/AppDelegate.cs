using Foundation;
using Prism.Ioc;
using UIKit;
using XFBleServerClient.Core;
using XFBleServerClient.Shared;

namespace XFBleServerClient.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		private App _coreApp;
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			ConfigureContainer();
			LoadApplication(_coreApp);

			return base.FinishedLaunching(app, options);
		}

		private void ConfigureContainer()
		{
			var platformInitializer = new AppPlatformInitializer();
			var container = platformInitializer.Container;
			RegisterContainer(container);
			_coreApp = new App(platformInitializer);
		}

		private void RegisterContainer(IContainerRegistry containerRegistry)
		{

		}

		[Export("application:supportedInterfaceOrientationsForWindow:")]
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
		{
			return UIInterfaceOrientationMask.Portrait;
		}
	}
}
