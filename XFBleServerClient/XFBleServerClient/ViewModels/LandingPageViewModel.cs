using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;

namespace XFBleServerClient.Core.ViewModels
{
	public class LandingPageViewModel : ViewModelBase
	{
        private readonly IPermissions _permissionsUtil;

		public LandingPageViewModel(INavigationService navigationService, IPermissions permissionsUtil)
			: base(navigationService)
		{
            _permissionsUtil = permissionsUtil;

			this.NavigateCommand = new DelegateCommand<string>(async (selection) => await ExecuteNavigateCommand(selection));
		}

		public DelegateCommand<string> NavigateCommand { get; private set; }

		private async Task ExecuteNavigateCommand(string selection)
		{
			string page = "ViewNames.ServerSetupPage";

			if (selection == AppConstants.Server)
			{
				page = ViewNames.GattServerSetupPage;
			}
			else if (selection == AppConstants.Client)
			{
				page = ViewNames.ClientDeviceListPage;
			}

            await CheckPermissionAsync();

			var s = await this.NavigationService.NavigateAsync(page);

			if (!s.Success)
			{

			}
		}

        private async Task CheckPermissionAsync()
        {
            await _permissionsUtil.RequestPermissionsAsync(new Permission[] { Permission.Location});
        }

	}
}
