using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Translation
{
    public interface ITranslationResourceMerger
    {
        List<TranslatedValue> MergeResources<Internal, Local>();
    }
}
