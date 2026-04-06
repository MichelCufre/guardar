using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Options;
using NLog;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.General;
using WIS.Domain.Logic;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;
using WIS.Translation;

namespace WIS.Domain.Services
{
    public class ResourceValidationService : IResourceValidationService
    {
        protected List<TranslatedValue> Recursos { get; set; }
        protected Dictionary<string, string> UsuarioIdioma { get; set; }

        protected readonly IUnitOfWorkFactory _uowFactory;

        public ResourceValidationService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;

            Recursos = new List<TranslatedValue>();
            UsuarioIdioma = new Dictionary<string, string>();
        }

        public virtual string Translate(string loginName, Error error)
        {
            var idioma = UsuarioIdioma.GetValueOrDefault(loginName, null);

            if (string.IsNullOrEmpty(idioma))
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    idioma = uow.SecurityRepository.GetUserLanguage(loginName);
                    UsuarioIdioma.Add(loginName, idioma);
                }
            }

            var translation = Recursos.FirstOrDefault(t => t.Language == idioma && t.Key == error.Mensaje);

            if (translation == null)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    translation = uow.LocalizationRepository.GetTranslation(error.Mensaje, idioma);

                    if (translation.Language == "base")
                    {
                        translation.Language = idioma;
                    }

                    Recursos.Add(translation);
                }
            }

            string msg = (translation != null) ? translation.Value : error.Mensaje;
            return string.Format(msg ?? "", error.Argumentos);

        }
    }
}
