namespace Slack.Models
{
    public class ChannelQuery
    {
        public ChannelQuery()
        {
            MyChannelsOnly = true;
            IncludeArchived = false;
        }

        public bool MyChannelsOnly { get; set; }
        public bool IncludeArchived { get; set; }
    }
}