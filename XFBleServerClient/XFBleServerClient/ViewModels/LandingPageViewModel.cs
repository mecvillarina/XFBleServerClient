using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;
using XFBleServerClient.Core.Common;

namespace XFBleServerClient.Core.ViewModels
{
	public class LandingPageViewModel : ViewModelBase
	{
		public LandingPageViewModel(INavigationService navigationService)
			: base(navigationService)
		{
			this.NavigateCommand = new DelegateCommand<string>(async (selection) => await ExecuteNavigateCommand(selection));
		}

		public DelegateCommand<string> NavigateCommand { get; private set; }

		private async Task ExecuteNavigateCommand(string selection)
		{
			string page = "ViewNames.ServerSetupPage";

			if (selection == AppConstants.Server)
			{
				page = ViewNames.ServerSetupPage;
			}
			else if (selection == AppConstants.Client)
			{

			}

			var s = await this.NavigationService.NavigateAsync(page);

			if (!s.Success)
			{

			}
		}

	}
}
