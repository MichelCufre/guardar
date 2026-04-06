using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Session
{
    public interface ISessionAccessorManager
    {
        void SetInnerDictionary(Dictionary<string, object> dictionary);
    }
}
