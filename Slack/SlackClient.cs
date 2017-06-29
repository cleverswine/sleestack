using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slack.Common;
using Slack.Interfaces;
using Slack.Models;

namespace Slack
{
    public class SlackClient : ISlackClient
    {
        private readonly ISettings _settings;
        private readonly ICache _cache;
        private readonly SlackApiHelper _slackApiHelper;

        public SlackClient(ISettings settings, ICache cache)
        {
            _settings = settings;
            _cache = cache;
            _slackApiHelper = new SlackApiHelper();
        }

        public async Task<Self> SignIn(string token = null)
        {
            if (token == null) token = _settings.SlackApiAuthToken;
            var self = await _slackApiHelper.Get<Self>("auth.test", token);
            return self;
        }

        public async Task<List<Channel>> GetChannels(ChannelQuery query)
        {
            Func<Task<List<Channel>>> add = async () =>
            {
                var p = new Dictionary<string, string>();
                if (!query.IncludeArchived) p.Add("exclude_archived", "1");

                var root = await _slackApiHelper.Get<ChannelsRoot>("channels.list", _settings.SlackApiAuthToken, p);
                return query.MyChannelsOnly
                    ? root.channels.Where(c => c.is_member).ToList()
                    : root.channels.ToList();
            };

            return await _cache.Get("Channels", add, TimeSpan.FromMinutes(3));
        }

        public async Task<ChannelInfo> GetChannel(string id)
        {
            var root = await _slackApiHelper.Get<ChannelInfoRoot>("channels.info", _settings.SlackApiAuthToken,
                new Dictionary<string, string> { { "channel", id } });
            return root.channel;
        }

        public async Task<List<Message>> GetChannelMessages(string id, MessageQuery query)
        {
            return await GetMessages("channels.history", id, query);
        }

        public async Task MarkChannel(string id, string ts)
        {
            await _slackApiHelper.Get<SlackResponse>("channels.mark", _settings.SlackApiAuthToken,
                new Dictionary<string, string> { { "channel", id }, { "ts", ts } });
        }

        public async Task PostChannelMessage(string id, string text)
        {
            var token = _settings.SlackApiAuthToken;
            var self = await SignIn(token);
            var selfUser = await GetUser(self.user_id);

            var cleanedText = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            await _slackApiHelper.Get<SlackResponse>("chat.postMessage", _settings.SlackApiAuthToken,
                    new Dictionary<string, string> { { "channel", id }, { "text", cleanedText }, { "username", selfUser.real_name + " (bot)" } });
        }

        public async Task<List<Im>> GetImChannels()
        {
            Func<Task<List<Im>>> add = async () =>
            {
                var root = await _slackApiHelper.Get<ImsRoot>("im.list", _settings.SlackApiAuthToken);
                return root.ims.ToList();
            };
            return await _cache.Get("ImChannels", add, TimeSpan.FromMinutes(3));
        }

        public async Task<List<Message>> GetImMessages(string id, MessageQuery query)
        {
            return await GetMessages("im.history", id, query);
        }

        public async Task MarkImChannel(string id, string ts)
        {
            await _slackApiHelper.Get<SlackResponse>("im.mark", _settings.SlackApiAuthToken,
                new Dictionary<string, string> { { "channel", id }, { "ts", ts } });
        }

        public async Task<List<Member>> GetMembers()
        {
            Func<Task<List<Member>>> add = async () =>
            {
                var root = await _slackApiHelper.Get<MembersRoot>("users.list", _settings.SlackApiAuthToken);
                return root.members.ToList();
            };
            return await _cache.Get("Members", add, TimeSpan.FromMinutes(60));
        }

        public async Task<Member> GetUser(string id = null)
        {
            if (id == null)
            {
                var self = await SignIn(_settings.SlackApiAuthToken);
                id = self.user_id;
            }

            var users = await GetMembers();
            return users.FirstOrDefault(u => u.id == id);
        }

        private async Task<List<Message>> GetMessages(string method, string id, MessageQuery query)
        {
            var p = new Dictionary<string, string> {{"channel", id}, {"count", _settings.MessageCount.ToString()}};
            if (query.Latest.HasValue) p.Add("latest", query.Latest.ToString());
            if (query.Oldest.HasValue) p.Add("oldest", query.Oldest.ToString());

            var root = await _slackApiHelper.Get<MessagesRoot>(method, _settings.SlackApiAuthToken, p);
            var messages = root.messages.ToList();
            messages.RemoveAll(m => m.hidden);

            return messages;
        }
    }
}
