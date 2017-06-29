namespace Slack.Models
{
    public class Self : SlackResponse
    {
        public string url { get; set; }
        public string team { get; set; }
        public string user { get; set; }
        public string team_id { get; set; }
        public string user_id { get; set; }
    }
}