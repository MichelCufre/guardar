using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Translation
{
    public class TranslationResourceMerger : ITranslationResourceMerger
    {
        private readonly IEqualityComparer<TranslatedValue> _comparer;

        public TranslationResourceMerger(IEqualityComparer<TranslatedValue> comparer)
        {
            this._comparer = comparer;
        }

        public List<TranslatedValue> MergeResources<Internal, Local>()
        {
            List<TranslatedValue> internalResources = this.GetResources<Internal>();
            List<TranslatedValue> localResources = this.GetResources<Local>();

            return localResources.Union(internalResources, this._comparer).ToList();
        }

        private List<TranslatedValue> GetResources<T>()
        {
            var instance = Activator.CreateInstance(typeof(T));

            return instance.GetType().GetFields().Select(f =>
                new TranslatedValue
                {
                    Key = f.Name,
                    Language = "base",
                    Value = (string)f.GetValue(instance)
                }
            ).ToList();
        }
    }
}
