using System;
using System.Collections.Generic;

namespace WIS.Translation
{
    public interface ITranslator
    {
        Dictionary<string, string> Translate(List<string> keys);
    }
}
