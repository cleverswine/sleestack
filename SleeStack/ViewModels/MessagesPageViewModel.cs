using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Slack.Common;
using Slack.Interfaces;
using Slack.Models;

namespace SleeStack.ViewModels
{
    public class MessagesPageViewModel : SlackPageBaseViewModel
    {
        private readonly ISlackClient _slackClient;
        private double? _latest;
        private bool _firstLoad = true;

        public MessagesPageViewModel(ISlackClient slackClient)
        {
            _slackClient = slackClient;
            Messages = new ObservableCollection<SlackItem>();
        }

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }

        public ObservableCollection<SlackItem> Messages { get; set; }

        public async Task RefreshMessages()
        {
            if (BusyStart != null) BusyStart();

            List<Message> messages;
            var query = new MessageQuery { Oldest = _latest };

            if (ChannelId.StartsWith("D"))
                messages = await _slackClient.GetImMessages(ChannelId, query);
            else
                messages = await _slackClient.GetChannelMessages(ChannelId, query);

            // update dates on older ones if this isn't the first load
            if (!_firstLoad && Messages.Any())
            {
                foreach (var m in Messages)
                {
                    // remove NEW msg from older ones (temporary hack)...
                    if (m.Title.StartsWith("[NEW!] "))
                        m.Title = m.Title.Replace("[NEW!] ", "");

                    // re-Humanize the time stamp
                    var dt = JavascriptDateTimeConverter.JsonToDate(m.Ts).ToLocalTime();
                    m.SubTitle = dt.Humanize(false);
                }
            }

            if (messages.Any())
            {
                _latest = messages[0].ts + 1;
                messages.RemoveAll(m => m.hidden);
                messages.Reverse();

                var members = await _slackClient.GetMembers();

                foreach (var message in messages)
                {
                    var userId = message.user;
                    var member = members.FirstOrDefault(mem => mem.id == userId)
                                 ?? new Member { real_name = message.username, profile = new Profile { title = "" } };

                    var title = "{0}{1}".FormatWith(_firstLoad ? "" : "[NEW!] ", member.real_name);
                    var dt = JavascriptDateTimeConverter.JsonToDate(message.ts).ToLocalTime();

                    Messages.Insert(0, new SlackItem
                    {
                        Ts = message.ts,
                        Title = title,
                        SubTitle = dt.Humanize(false),
                        Content = message.text
                    });
                }
            }

            _firstLoad = false;

            if (BusyStop != null) BusyStop();
        }

        public async Task PostMessage(string message)
        {
            if (BusyStart != null) BusyStart();
            await _slackClient.PostChannelMessage(ChannelId, message);
            if (BusyStop != null) BusyStop();
        }

        public async Task MarkRead()
        {
            if (!Messages.Any()) return;

            var ts = (Messages[0].Ts + 1).ToString();

            if (ChannelId.StartsWith("D"))
                await _slackClient.MarkImChannel(ChannelId, ts);
            else
                await _slackClient.MarkChannel(ChannelId, ts);
        }
    }
}