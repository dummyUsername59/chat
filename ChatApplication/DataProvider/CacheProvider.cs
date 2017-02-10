using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace ChatApplication.DataProvider
{
    public class CacheProvider : ICacheProvider<string, KeyValuePair<string,DateTime>>
    {
        static readonly private HashSet<string> allKeys = new HashSet<string>();
        static readonly private object locker = new object();
        private static readonly Lazy<CacheProvider> lazy =
        new Lazy<CacheProvider>(() => new CacheProvider());

        public static CacheProvider Instance { get { return lazy.Value; } }

        private CacheProvider()
        {
        }
        public KeyValuePair<string,DateTime>? Get(string key)
        {
            lock (locker)
            {
                return WebCache.Get(key);
            }
           
        }
        public bool Set(string key,KeyValuePair<string,DateTime> value)
        {
            lock(locker)
            {
                WebCache.Set(key, value, 100);
                allKeys.Add(key);
                return true;
            }
            
        }
        public void UpdateLastUsed(string key)
        {
            lock (locker)
            {
                var value = WebCache.Get(key);
                if((object)value != null)
                {
                    KeyValuePair<string, DateTime> item = ((KeyValuePair<string, DateTime>)value);
                    WebCache.Set(key, new KeyValuePair<string, DateTime>(item.Key, DateTime.Now));
                }
                
            }
        }
        public bool Remove(string key)
        {
            lock (locker)
            {
                WebCache.Remove(key);
                allKeys.Remove(key);
                return true;
            }
           
        }
        public IDictionary<string,KeyValuePair<string,DateTime>> GetAll()
        {

            IDictionary <string,KeyValuePair<string,DateTime>> result= 
                new Dictionary<string, KeyValuePair<string, DateTime>>();
            HashSet<string> itemsToRemove = new HashSet<string>();
            lock (locker)
            {
                foreach (var key in allKeys)
                {
                    var value = (KeyValuePair<string, DateTime>)WebCache.Get(key);
                    if (value.Value.AddMinutes(1) < DateTime.Now)
                    {
                        WebCache.Remove(key);
                        itemsToRemove.Add(key);
                    }
                    else
                    {
                        result.Add(key, value);
                    }
                   
                }
                foreach(var itm in itemsToRemove)
                {
                    allKeys.Remove(itm);
                }
            }
            
            return result;
        }
    }
    public interface ICacheProvider<T1,T2> where T1:class where T2: struct
    {
        T2? Get(T1 key);
        bool Set(T1 key, T2 value);
    }
}