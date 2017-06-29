using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slack.Exceptions;
using Slack.Models;

namespace Slack.Common
{
    public class SlackApiHelper
    {
        private const string SlackUriTemplate = "https://slack.com/api/{0}";

        public async Task<T> Get<T>(string method, string token, Dictionary<string, string> queryParams = null) where T : SlackResponse
        {
            if (string.IsNullOrWhiteSpace(token)) ExceptionFactory.Throw("No auth token specified");

            if (queryParams == null) queryParams = new Dictionary<string, string>();
            using (var client = new HttpClient())
            {
                queryParams.Add("token", token);
                var uri = string.Format(SlackUriTemplate, method) + QueryStringHelper.FromDictionary(queryParams);
                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    ExceptionFactory.Throw(response.StatusCode, response.ReasonPhrase);

                var content = await response.Content.ReadAsStringAsync();
                var slackResponse = JsonConvert.DeserializeObject<T>(content, new JavascriptDateTimeConverter());
                if (!slackResponse.ok)
                    ExceptionFactory.Throw(slackResponse.error);

                return slackResponse;
            }
        }
    }
}