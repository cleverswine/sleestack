namespace Slack.Interfaces
{
    public interface ISettings
    {
        string SlackApiAuthToken { get; set; }
        int MessageCount { get; set; }
        void Save();
    }
}