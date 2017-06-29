using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Slack.Interfaces;
using Slack.Models;

namespace SleeStack.ViewModels
{
    public class HubPageViewModel : SlackPageBaseViewModel
    {
        private readonly ISlackClient _slackClient;
        private readonly ISettings _settings;

        public HubPageViewModel(ISlackClient slackClient, ISettings settings)
        {
            _slackClient = slackClient;
            _settings = settings;

            Channels = new ObservableCollection<SlackItem>();
            ImChannels = new ObservableCollection<SlackItem>();
        }

        public ObservableCollection<SlackItem> Channels { get; private set; }
        public ObservableCollection<SlackItem> ImChannels { get; private set; }

        public async Task RefreshAll(bool force = false)
        {
            await RefreshChannels(force);
            await RefreshImChannels(force);
            await RefreshChannelCounts();
        }

        public async Task<Self> SignIn()
        {
            if(string.IsNullOrWhiteSpace(_settings.SlackApiAuthToken))
            {
                ShowSignIn();
                return null;
            }

            try
            {
                var self = await _slackClient.SignIn();
                await RefreshAll(true);
                return self;
            }
            catch
            {
                ShowSignIn();
                return null;
            }
        }

        public void SignOut()
        {
            ShowSignIn();
            _settings.SlackApiAuthToken = "";
            _settings.Save();
        }

        /// <summary>
        /// This function is passed in to the Settings dialog so that it has a means to test the token
        ///     the settings dialog is not MVVM at the moment...
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> AuthTest(string token)
        {
            try
            {
                await _slackClient.SignIn(token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ShowSignIn()
        {
            Channels.Clear();
            Channels.Add(new SlackItem("SS_SIGNIN", "Sign In", "Sign in with your Slack Authentication token to continue."));
            ImChannels.Clear();
        }

        private async Task RefreshChannels(bool force = false)
        {
            if (!force && Channels.Any()) return;

            if (BusyStart != null) BusyStart();

            Channels.Clear();

            var channels = await _slackClient.GetChannels(new ChannelQuery { IncludeArchived = false, MyChannelsOnly = true });
            foreach (var c in channels) Channels.Add(
                new SlackItem(c.id, "#{0}".FormatWith(c.name), c.purpose.value, "#{0}".FormatWith(c.name)));

            if (BusyStop != null) BusyStop();
        }

        private async Task RefreshImChannels(bool force = false)
        {
            if (!force && ImChannels.Any()) return;

            if (BusyStart != null) BusyStart();

            ImChannels.Clear();

            var members = await _slackClient.GetMembers();
            var ims = await _slackClient.GetImChannels();
            var imsVm = new List<SlackItem>();

            // todo move isDeleted flag to slack client
            foreach (var c in ims.Where(im => !im.is_user_deleted))
            {
                var m = members.FirstOrDefault(mem => mem.id == c.user)
                    ?? new Member { real_name = c.user, profile = new Profile { image_48 = "Assets/MediumGray.png", title = "" } };
                imsVm.Add(new SlackItem(c.id, m.real_name, m.profile.title, m.real_name, null, m.profile.image_48));
            }

            foreach (var item in imsVm.OrderBy(c => c.Title))
            {
                ImChannels.Add(item);
            }

            if (BusyStop != null) BusyStop();
        }

        private async Task RefreshChannelCounts()
        {
            if (BusyStart != null) BusyStart();

            foreach (var channel in Channels)
            {
                var ci = await _slackClient.GetChannel(channel.UniqueId);

                channel.Title = ci.unread_count > 0
                    ? "#{0} ({1})".FormatWith(ci.name, ci.unread_count)
                    : "#{0}".FormatWith(ci.name);
            }

            if (BusyStop != null) BusyStop();
        }
    }
}
