using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Build
{
    public interface IColumnTranslationProvider
    {
        Dictionary<string, string> Translate(List<string> keys);
    }
}
