namespace Slack.Models
{
    public class ImsRoot : SlackResponse
    {
        public Im[] ims { get; set; }
    }

    public class Im
    {
        public string id { get; set; }
        public string user { get; set; }
        public int created { get; set; }
        public bool is_user_deleted { get; set; }
    }
}
