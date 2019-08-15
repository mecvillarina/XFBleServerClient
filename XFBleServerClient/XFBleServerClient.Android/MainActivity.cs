using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism.Ioc;
using Xamarin.Forms;
using XFBleServerClient.Core;
using XFBleServerClient.Shared;

namespace XFBleServerClient.Droid
{
	[Activity(Label = "XFBleServerClient",
		Icon = "@mipmap/ic_launcher",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		WindowSoftInputMode = Android.Views.SoftInput.StateVisible | Android.Views.SoftInput.AdjustResize,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		private App _coreApp;
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);
			global::Xamarin.Forms.Forms.Init(this, bundle);
			FormsMaterial.Init(this, bundle);
			UserDialogs.Init(this);
			Plugin.Iconize.Iconize.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);

			ConfigureContainer();
			LoadApplication(_coreApp);
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

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

