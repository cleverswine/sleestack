using System;
using Newtonsoft.Json;
using Slack.Common;

namespace Slack.Models
{
    public class MessagesRoot : SlackResponse
    {
        public Message[] messages { get; set; }
        public bool has_more { get; set; }
    }

    public class Message
    {
        private double _ts;
        public string type { get; set; }
        public string user { get; set; }
        public string text { get; set; }

        public double ts
        {
            get { return _ts; }
            set
            {
                _ts = value;
                timeStamp = JavascriptDateTimeConverter.JsonToDate(_ts);
            }
        }

        [JsonIgnore]
        public DateTime timeStamp { get; private set; }
        public string username { get; set; }
        public Icons icons { get; set; }
        public string bot_id { get; set; }
        public string subtype { get; set; }
        public bool hidden { get; set; }
    }

    public class Icons
    {
        public string image_48 { get; set; }
    }
}