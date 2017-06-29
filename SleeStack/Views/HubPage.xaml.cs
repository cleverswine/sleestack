using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SleeStack.Common;
using SleeStack.ViewModels;

namespace SleeStack.Views
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private HubPageViewModel _viewModel;

        public HubPage()
        {
            InitializeComponent();

            _viewModel = DataContext as HubPageViewModel;

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            NavigationCacheMode = NavigationCacheMode.Required;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // restore state from previous session
            if (e.PageState != null && e.PageState.ContainsKey("HubPageViewModel"))
            {
                _viewModel = e.PageState["HubPageViewModel"] as HubPageViewModel;
                return;
            }

            await _viewModel.SignIn();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (e.PageState != null)
                e.PageState.Add("HubPageViewModel", _viewModel);
        }

        private void Im_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Frame.Navigate(typeof(MessagesPage), e.ClickedItem))
            {
                throw new Exception(this._resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private async void Channel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (((SlackItem) e.ClickedItem).UniqueId == "SS_SIGNIN")
            {
                await ShowSettingsDialog();
                return;
            }

            if (!Frame.Navigate(typeof(MessagesPage), e.ClickedItem))
            {
                throw new Exception(this._resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private async void Refresh_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await _viewModel.RefreshAll();
        }

        private async void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            await _viewModel.RefreshAll();
        }

        private async Task ShowSettingsDialog()
        {
            Func<string, Task<bool>> testAuth = async token => await _viewModel.AuthTest(token);

            var dialog = new SettingsPage(testAuth);
            await dialog.ShowAsync();

            if (dialog.Result != SignInResult.Success) return;

            await _viewModel.SignIn();
        }

        private async void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            await ShowSettingsDialog();
        }

        private async void Settings_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await ShowSettingsDialog();
        }

        private void SignOut_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _viewModel.SignOut();
        }

        private void SignOut_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.SignOut();
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
