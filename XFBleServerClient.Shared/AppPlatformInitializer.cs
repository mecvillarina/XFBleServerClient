using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism;
using Prism.Ioc;
using Prism.Logging;
using Xamarin.Forms;
using XFBleServerClient.Core.ViewModels;
using XFBleServerClient.Core.Views;

namespace XFBleServerClient.Shared
{
	public class AppPlatformInitializer : IPlatformInitializer, IPlatformInitializerExtension
	{
		public IContainerRegistry Container { get; private set; }
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			Container = containerRegistry;

			RegisterPlugins(containerRegistry);
			RegisterHelper(containerRegistry);
			RegisterUtilites(containerRegistry);
			RegisterFactories(containerRegistry);
			RegisterRepositories(containerRegistry);
			RegisterServices(containerRegistry);
			RegisterManagers(containerRegistry);
			RegisterUI(containerRegistry);
		}

		private void RegisterUI(IContainerRegistry container)
		{
			container.RegisterForNavigation<NavigationPage>();
			container.RegisterForNavigation<LandingPage, LandingPageViewModel>();
			container.RegisterForNavigation<GattServerSetupPage, GattServerSetupPageViewModel>();
			container.RegisterForNavigation<GattServerServiceDetailPage, GattServerServiceDetailPageViewModel>();
			container.RegisterForNavigation<ClientDeviceListPage, ClientDeviceListPageViewModel>();
			container.RegisterForNavigation<ClientDeviceDetailPage, ClientDeviceDetailPageViewModel>();
            container.RegisterForNavigation<ClientDeviceCharacteristicsDetailPage, ClientDeviceCharacteristicsDetailPageViewModel>();

            container.RegisterDialog<DialogInfoPage, DialogInfoPageViewModel>();

		}

		private void RegisterFactories(IContainerRegistry container)
		{
			container.Register<ILoggerFacade, DebugLogger>();
		}

		private void RegisterManagers(IContainerRegistry container)
		{
		}

		private void RegisterRepositories(IContainerRegistry container)
		{
		}

		private void RegisterUtilites(IContainerRegistry container)
		{
		}

		private void RegisterHelper(IContainerRegistry container)
		{
		}

		private void RegisterServices(IContainerRegistry container)
		{
		}

		private void RegisterPlugins(IContainerRegistry container)
		{
			container.RegisterInstance<IPermissions>(CrossPermissions.Current);
			container.RegisterInstance<IAdapter>(CrossBleAdapter.Current);
			container.RegisterInstance<IUserDialogs>(UserDialogs.Instance);
		}
	}
}
