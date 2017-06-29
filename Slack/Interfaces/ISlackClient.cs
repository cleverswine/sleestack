using System.Collections.Generic;
using System.Threading.Tasks;
using Slack.Models;

namespace Slack.Interfaces
{
    public interface ISlackClient
    {
        Task<Self> SignIn(string token = null);

        Task<List<Channel>> GetChannels(ChannelQuery query);
        Task<ChannelInfo> GetChannel(string id);
        Task<List<Message>> GetChannelMessages(string id, MessageQuery query);
        Task MarkChannel(string id, string ts);

        Task PostChannelMessage(string id, string text);

        Task<List<Im>> GetImChannels();
        Task<List<Message>> GetImMessages(string id, MessageQuery query);
        Task MarkImChannel(string id, string ts);

        Task<List<Member>> GetMembers();
        Task<Member> GetUser(string id = null);
    }
}