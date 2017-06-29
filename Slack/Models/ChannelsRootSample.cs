namespace Slack.Models
{
    public class ChannelsRootSample : SlackResponse
    {
        public Channel[] channels { get; set; }
        public Member[] members { get; set; }
    }
}