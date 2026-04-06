using WIS.Persistence.Database;
using WIS.Translation;

namespace WIS.Domain.DataModel.Mappers
{
    public class LocalizacionMapper : Mapper
    {
        public readonly string Separator = "_";

        public virtual TranslatedValue MapToObject(T_LOCALIZACION entity)
        {
            if (entity == null)
                return null;

            return new TranslatedValue
            {
                Key = string.Join("_", entity.CD_APLICACION, entity.CD_BLOQUE, entity.CD_TIPO, entity.CD_CLAVE),
                Language = entity.CD_IDIOMA,
                ResourceType = entity.CD_TIPO,
                Value = entity.DS_VALOR
            };
        }

        public virtual TranslationVersion MapToObject(T_LOCALIZACION_VERSION entity)
        {
            if (entity == null)
                return null;

            return new TranslationVersion
            {
                Language = entity.CD_IDIOMA,
                LastEdited = entity.DT_UPDROW,
                Version = entity.NU_VERSION
            };
        }

        public virtual T_LOCALIZACION MapToEntity(TranslatedValue translation)
        {
            if (translation == null)
                return null;

            TranslationSplitKey splitKey = this.SplitTranslationString(translation.Key);

            return new T_LOCALIZACION
            {
                CD_APLICACION = splitKey.Application,
                CD_BLOQUE = splitKey.Area,
                CD_TIPO = splitKey.ElementType,
                CD_CLAVE = splitKey.Key,
                CD_IDIOMA = translation.Language,
                DS_VALOR = translation.Value
            };
        }

        public virtual T_LOCALIZACION_VERSION MapToEntity(TranslationVersion version)
        {
            if (version == null)
                return null;

            return new T_LOCALIZACION_VERSION
            {
                CD_IDIOMA = version.Language,
                NU_VERSION = version.Version,
                DT_UPDROW = version.LastEdited
            };
        }

        public virtual string GetTranslationKey(string key)
        {
            var thirdOccurrenceIndex = key.IndexOf(this.Separator, key.IndexOf(this.Separator, key.IndexOf(this.Separator) + 1) + 1) + 1;

            return key.Substring(thirdOccurrenceIndex);
        }
        public virtual TranslationSplitKey SplitTranslationString(string key)
        {
            string[] parts = key.Split(this.Separator.ToCharArray()[0]);

            return new TranslationSplitKey
            {
                Application = parts[0],
                Area = parts[1],
                ElementType = parts[2],
                Key = this.GetTranslationKey(key)
            };
        }
    }
}
