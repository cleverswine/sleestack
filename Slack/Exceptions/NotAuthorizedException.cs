using System;

namespace Slack.Exceptions
{
    public class SlackApiServerException : Exception
    {
        public SlackApiServerException(string message)
            : base(message)
        {
            
        }
    }

    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message) : base(message)
        {
            
        }
    }
}
