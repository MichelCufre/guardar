using System.Collections.Generic;
using WIS.Application.Localization.Resources;
using WIS.Domain.Validation;
using WIS.Translation;

namespace WIS.BackendService.Services
{
    public class TranslationResourceProvider : ITranslationResourceProvider
    {
        private readonly ITranslationResourceMerger _merger;

        public TranslationResourceProvider(ITranslationResourceMerger merger)
        {
            this._merger = merger;
        }

        public List<TranslatedValue> GetResources()
        {
            var resources = this._merger.MergeResources<InternalResources, LocalResources>();
            resources.AddRange(this._merger.MergeResources<ValidationMessage, LocalResources>());
            return resources;
        }
    }
}
