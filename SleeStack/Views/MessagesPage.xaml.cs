using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SleeStack.Common;
using SleeStack.ViewModels;

namespace SleeStack.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessagesPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private MessagesPageViewModel _viewModel;

        public MessagesPage()
        {
            InitializeComponent();

            _viewModel = DataContext as MessagesPageViewModel;

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
            if (e.PageState != null && e.PageState.ContainsKey("MessagesPageViewModel"))
            {
                _viewModel = e.PageState["MessagesPageViewModel"] as MessagesPageViewModel;
                return;
            }

            var item = e.NavigationParameter as SlackItem;
            if (item == null) return;
           
            _viewModel.ChannelId = item.UniqueId;
            _viewModel.ChannelName = item.Description;

            await _viewModel.RefreshMessages();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (e.PageState != null)
                e.PageState.Add("MessagesPageViewModel", _viewModel);
        }

        private async void Refresh_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await _viewModel.RefreshMessages();
        }

        private async void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            await _viewModel.RefreshMessages();
        }

        private async void PostMessage_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;

            var tb = sender as TextBox;
            if (tb != null)
            {
                await _viewModel.PostMessage(tb.Text);
                tb.Text = "";
                await _viewModel.RefreshMessages();
            }
            e.Handled = true;
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
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await _viewModel.MarkRead();
            this._navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
