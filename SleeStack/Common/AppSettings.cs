using Windows.Storage;
using Slack.Interfaces;

namespace SleeStack.Common
{
    public class AppSettings : ISettings
    {
        readonly ApplicationDataContainer _localSettings = null;
        private string _slackApiAuthToken;
        private int? _messageCount;
        private const string ContainerName = "SleeStackSettings";

        private const string TokenSettingName = "SlackApiToken";
        private const string MessageCountName = "MessageCount";

        public AppSettings()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            if (!_localSettings.Containers.ContainsKey(ContainerName))
                _localSettings.CreateContainer(ContainerName, ApplicationDataCreateDisposition.Always);
        }

        public string SlackApiAuthToken
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_slackApiAuthToken)) return _slackApiAuthToken;
                _slackApiAuthToken = _localSettings.Containers[ContainerName].Values[TokenSettingName] as string;
                return _slackApiAuthToken ?? "";
            }
            set { _slackApiAuthToken = value; }
        }

        public int MessageCount
        {
            get
            {
                if (_messageCount.HasValue) return _messageCount.Value;
                _messageCount = _localSettings.Containers[ContainerName].Values[MessageCountName] as int?;
                return _messageCount.HasValue ? _messageCount.Value : 30;
            }
            set { _messageCount = value; }
        }

        public void Save()
        {
            _localSettings.Containers[ContainerName].Values[TokenSettingName] = SlackApiAuthToken;
            _localSettings.Containers[ContainerName].Values[MessageCountName] = MessageCount;
        }
    }
}