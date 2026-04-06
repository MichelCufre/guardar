using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Security;
using WIS.Translation;

namespace WIS.BackendService.Services
{
    public class GridTranslator : ITranslator
    {
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWorkFactory _uowFactory;

        public GridTranslator(IUnitOfWorkFactory uowFactory, IIdentityService identityService)
        {
            this._uowFactory = uowFactory;
            this._identityService = identityService;
        }

        public Dictionary<string, string> Translate(List<string> keys)
        {
            var result = new Dictionary<string, string>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var language = uow.SecurityRepository.GetUserLanguage(this._identityService.UserId);
            var translations = uow.LocalizationRepository.GetTranslation(keys, language);

            foreach (var key in keys)
            {
                var translation = translations.Where(d => d.Key == key).FirstOrDefault();

                if (translation != null)
                    result[translation.Key] = translation.Value;
                else
                    result[key] = key;
            }

            return result;
        }
    }
}
