using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Translation;

namespace WIS.Domain.Validation
{
    public class Translator
    {
        public static string Translate(IUnitOfWork uow, Error error, int userId)
        {
            return Traducir(uow, error , userId)?.Mensaje;
        }

        public static List<string> Translate(IUnitOfWork uow, List<Error> errores, int userId)
        {
            var result = new List<string>();
            var keys = errores.Select(e => e.Mensaje).Distinct().ToList();

            string language = uow.SecurityRepository.GetUserLanguage(userId);
            List<TranslatedValue> translations = uow.LocalizationRepository.GetTranslation(keys, language);

            foreach (var e in errores)
            {
                var translation = translations.FirstOrDefault(d => d.Key == e.Mensaje);

                string msg = (translation != null) ? translation.Value : e.Mensaje;
                result.Add(string.Format(msg ?? "", e.Argumentos));
            }

            return result;
        }

        public static Error Traducir(IUnitOfWork uow, Error error, int userId)
        {
            var key = error.Mensaje;
            string language = uow.SecurityRepository.GetUserLanguage(userId);

            var translation = uow.LocalizationRepository.GetTranslation(key, language);

            string msg = (translation != null) ? translation.Value : key;
            return new Error(string.Format(msg ?? "", error.Argumentos));
        }

        public static List<Error> Traducir(IUnitOfWork uow, List<Error> errores, int userId)
        {
            var result = new List<Error>();
            var keys = errores.Select(e => e.Mensaje).Distinct().ToList();

            string language = uow.SecurityRepository.GetUserLanguage(userId);
            List<TranslatedValue> translations = uow.LocalizationRepository.GetTranslation(keys, language);

            foreach (var key in keys)
            {
                var error = errores.FirstOrDefault(d => d.Mensaje == key);
                var translation = translations.FirstOrDefault(d => d.Key == key);

                string msg = (translation != null) ? translation.Value : key;
                result.Add(new Error(string.Format(msg ?? "", error.Argumentos)));
            }

            return result;
        }
    }
}
