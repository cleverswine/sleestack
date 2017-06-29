using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using Slack.Interfaces;
using Slack.Models;

namespace SleeStack.Fakes
{
    public class SlackClientMock : ISlackClient
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
 
        private async Task<T> GetJson<T>(string fileName) where T : class
        {
            if (_cache.ContainsKey(fileName)) return _cache[fileName] as T;

            var dataUri = new Uri(string.Format("ms-appx:///Fakes/Json/{0}.json", fileName));
            var file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            var jsonText = await FileIO.ReadTextAsync(file);
            var obj = JsonConvert.DeserializeObject<T>(jsonText);

            _cache.Add(fileName, obj);

            return obj;
        }

        public async Task<List<Member>> GetMembers()
        {
            var users = await GetJson<MembersRoot>("users");
            return users.members.ToList();
        }

        public Task<Self> SignIn(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Channel>> GetChannels(ChannelQuery query)
        {
            var channels = await GetJson<ChannelsRoot>("channels");
            return channels.channels.ToList();
        }

        public async Task<ChannelInfo> GetChannel(string id)
        {
            var channels = await GetJson<ChannelInfoRoot>("channels-info");
            return channels.channel;
        }

        public async Task<List<Message>> GetChannelMessages(string id, MessageQuery query)
        {
            var channels = await GetJson<MessagesRoot>("channels-history");
            return channels.messages.ToList();
        }

        public Task MarkChannel(string id, string ts)
        {
            return Task.FromResult(0);
        }

        public Task PostChannelMessage(string id, string text)
        {
            return Task.FromResult(0);
        }

        public async Task<List<Im>> GetImChannels()
        {
            var channels = await GetJson<ImsRoot>("im");
            return channels.ims.ToList();
        }

        public Task<List<Message>> GetImMessages(string id, MessageQuery query)
        {
            throw new NotImplementedException();
        }

        public Task MarkImChannel(string id, string ts)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetUser(string id)
        {
            throw new NotImplementedException();
        }
    }
}