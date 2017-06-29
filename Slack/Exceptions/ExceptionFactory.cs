using System;
using System.Net;

namespace Slack.Exceptions
{
    internal static class ExceptionFactory
    {
        public static void Throw(string slackStatus)
        {
            switch (slackStatus)
            {
                case "invalid_auth":
                case "account_inactive":
                case "not_authed": 
                    throw new NotAuthorizedException(slackStatus);
                case "channel_not_found":
                default: 
                    throw new Exception(slackStatus);
            }
        }

        public static void Throw(HttpStatusCode statusCode, string message)
        {
            throw new SlackApiServerException(statusCode + ": " + message);
        }
    }
}