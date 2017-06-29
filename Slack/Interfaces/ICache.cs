using System;
using System.Threading.Tasks;

namespace Slack.Interfaces
{
    public interface ICache
    {
        Task<T> Get<T>(string key, Func<Task<T>> add = null, TimeSpan? expiry = null) where T : class;
        Task Set(string key, object value, TimeSpan? expiry = null);
        Task Delete(string key);
    }
}