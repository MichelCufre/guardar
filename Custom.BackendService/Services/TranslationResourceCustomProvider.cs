using Custom.Application.Localization.Resources;
using System.Collections.Generic;
using WIS.Domain.Validation;
using WIS.Translation;

namespace Custom.BackendService.Services
{
    public class TranslationResourceCustomProvider : ITranslationResourceProvider
    {
        private readonly ITranslationResourceMerger _merger;

        public TranslationResourceCustomProvider(ITranslationResourceMerger merger)
        {
            this._merger = merger;
        }

        public List<TranslatedValue> GetResources()
        {
            var resources = this._merger.MergeResources<InternalResourcesCustom, LocalResourcesCustom>();
            resources.AddRange(this._merger.MergeResources<ValidationMessage, LocalResourcesCustom>());
            return resources;
        }
    }
}
