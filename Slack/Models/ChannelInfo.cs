namespace Slack.Models
{
    public class ChannelInfoRoot : SlackResponse
    {
        public ChannelInfo channel { get; set; }
    }

    public class ChannelInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public int created { get; set; }
        public string creator { get; set; }
        public bool is_archived { get; set; }
        public bool is_general { get; set; }
        public string[] members { get; set; }
        public bool is_member { get; set; }
        public string last_read { get; set; }
        public Latest latest { get; set; }
        public int unread_count { get; set; }
        public Topic topic { get; set; }
        public Purpose purpose { get; set; }
    }

    public class Latest
    {
        public string type { get; set; }
        public string user { get; set; }
        public string text { get; set; }
        public string ts { get; set; }
    }
}
