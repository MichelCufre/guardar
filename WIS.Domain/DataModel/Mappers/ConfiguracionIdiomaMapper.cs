using WIS.Domain.Configuracion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ConfiguracionIdiomaMapper
    {
        public ConfiguracionIdiomaMapper()
        {

        }

        public virtual ConfiguracionIdioma MapToObject(T_LOCALIZACION entity)
        {
            return new ConfiguracionIdioma
            {
                Aplicacion = entity.CD_APLICACION,
                Bloque = entity.CD_BLOQUE,
                Tipo = entity.CD_TIPO,
                Clave = entity.CD_CLAVE,
                Idioma = entity.CD_IDIOMA,
                Valor =  entity.DS_VALOR
            };
        }
        public virtual T_LOCALIZACION MapToEntity(ConfiguracionIdioma configuracionIdioma)
        {
            return new T_LOCALIZACION
            {
                CD_APLICACION = configuracionIdioma.Aplicacion,
                CD_BLOQUE = configuracionIdioma.Bloque,
                CD_TIPO = configuracionIdioma.Tipo,
                CD_CLAVE = configuracionIdioma.Clave,
                CD_IDIOMA = configuracionIdioma.Idioma,
                DS_VALOR = configuracionIdioma.Valor
            };
        }
    }
}
