using Foundation;
using Plugin.Iconize;
using Prism.Ioc;
using System.Linq;
using UIKit;
using Xamarin.Forms;
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
			FormsMaterial.Init();

			LoadFontIcons();	

			Iconize.Init();

			AiForms.Effects.iOS.Effects.Init();
			UXDivers.Effects.iOS.Assembly.Register();

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

		private void LoadFontIcons()
		{
			foreach (var familyName in UIFont.FamilyNames.OrderBy(x => x))
			{
				System.Console.WriteLine($"Family: {familyName}");
				foreach (var name in UIFont.FontNamesForFamilyName(familyName).OrderBy(y => y))
				{
					System.Console.WriteLine(name);
				}
			}
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
