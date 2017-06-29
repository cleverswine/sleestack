using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Slack.Interfaces;

namespace Slack.Common
{
    public class CacheItem
    {
        public CacheItem(object item, DateTime expiry, TimeSpan slidingTs)
        {
            Expiry = expiry;
            SlidingTs = slidingTs;
            Item = item;
        }

        public DateTime Expiry { get; set; }
        public TimeSpan SlidingTs { get; set; }
        public object Item { get; set; }

        public bool IsExpired
        {
            get { return (DateTime.Now.CompareTo(Expiry)) > 0; }
        }
    }

    public class MemoryCache : ICache
    {
        private readonly Dictionary<string, CacheItem> _cache;
 
        public MemoryCache()
        {
            _cache = new Dictionary<string, CacheItem>();
        }

        public async Task<T> Get<T>(string key, Func<Task<T>> add = null, TimeSpan? expiry = null) where T : class
        {
            if (_cache.ContainsKey(key))
            {
                var item = _cache[key];
                if (!item.IsExpired)
                {
                    item.Expiry = DateTime.Now.Add(item.SlidingTs);
                    _cache[key] = item;
                    return item.Item as T;
                }
                _cache.Remove(key);
            }

            if (add == null) return default(T);

            var newItem = await add();

            _cache.Add(key, new CacheItem(newItem, DateTime.Now.Add(expiry ?? TimeSpan.FromMinutes(5)),
                expiry ?? TimeSpan.FromMinutes(5)));

            return newItem;
        }

        public Task Set(string key, object value, TimeSpan? expiry = null)
        {
            if (_cache.ContainsKey(key)) _cache.Remove(key);
            _cache.Add(key, new CacheItem(value, DateTime.Now.Add(expiry ?? TimeSpan.FromMinutes(5)), 
                expiry ?? TimeSpan.FromMinutes(5)));

            return Task.FromResult(0);
        }

        public Task Delete(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }
    }
}