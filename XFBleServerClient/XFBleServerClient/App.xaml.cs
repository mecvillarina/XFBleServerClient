using Prism;
using Prism.Ioc;
using System;
using Xamarin.Forms.Xaml;
using XFBleServerClient.Core.Common;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XFBleServerClient.Core
{
	public partial class App
	{
		public App(IPlatformInitializer initializer) : base(initializer)
		{
			SetIcons();
		}

		private void SetIcons()
		{
			Plugin.Iconize.Iconize.With(new Plugin.Iconize.Fonts.MaterialModule());

			for (int i = 0; i < Math.Min(Plugin.Iconize.Iconize.Modules.Count, 5); i++)
			{
				var module = Plugin.Iconize.Iconize.Modules[i];
			}
		}

		protected override async void OnInitialized()
		{
			InitializeComponent();

			await this.NavigationService.NavigateAsync($"{ViewNames.NavigationPage}/{ViewNames.LandingPage}");
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
		}
	}
}
