using System.Collections.Generic;

namespace WIS.Session
{
    public interface ISessionAccessor
    {
        T GetValue<T>(string key);
        Dictionary<string, object> GetInnerDictionary();
        void SetValue(string key, object value);
        bool ContainsKey(string key);
    }
}
