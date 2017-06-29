using System.Collections.Generic;
using System.Linq;

namespace Slack.Common
{
    public class QueryStringHelper
    {
        public static string FromDictionary(Dictionary<string, string> queryParams = null)
        {
            if (queryParams == null || queryParams.Count <= 0) return "";

            var uri = "";
            var uriSep = "?";

            uri = queryParams.Keys.Aggregate(uri, (current, k) =>
            {
                current = current + string.Format("{0}{1}={2}", 
                    uriSep, System.Uri.EscapeUriString(k), System.Uri.EscapeUriString(queryParams[k]));
                uriSep = "&";
                return current;
            });

            return uri;
        }
    }
}