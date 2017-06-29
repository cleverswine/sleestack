using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Slack.Interfaces;
using SleeStack.Common;

namespace SleeStack.Views
{
    public enum SignInResult
    {
        Success,
        Cancel,
        Nothing
    }

    public sealed partial class SettingsPage : ContentDialog
    {
        private readonly Func<string, Task<bool>> _authTest;
        public SignInResult Result { get; private set; }

        // todo pass in ISettings
        private readonly ISettings _settings = new AppSettings();

        public SettingsPage(Func<string, Task<bool>> authTest)
        {
            InitializeComponent();

            _authTest = authTest;

            var msgCounts = new ObservableCollection<int> {10, 20, 30, 50, 100};
            MessageCountOptionsCombo.DataContext = msgCounts;
            MessageCountOptionsCombo.SelectedItem = _settings.MessageCount;
            SlackAuthTokenTextBox.Text = _settings.SlackApiAuthToken;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();   
 
            if (await _authTest(SlackAuthTokenTextBox.Text))
            {
                _settings.SlackApiAuthToken = SlackAuthTokenTextBox.Text;
                if (MessageCountOptionsCombo.SelectedItem != null)
                    _settings.MessageCount = (int) MessageCountOptionsCombo.SelectedItem;
                _settings.Save();
                Result = SignInResult.Success;
            }
            else
            {
                ErrorBody.Visibility = Visibility.Visible;
                args.Cancel = true;
                Result = SignInResult.Cancel;
            }

            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = SignInResult.Cancel;
        }
    }
}
