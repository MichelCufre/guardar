using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Translation
{
    public interface ITranslationResourceProvider
    {
        List<TranslatedValue> GetResources();
    }
}
