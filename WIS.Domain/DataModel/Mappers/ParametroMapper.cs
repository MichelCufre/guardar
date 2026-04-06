using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ParametroMapper : Mapper
    {
        public ParametroMapper()
        {

        }

        public virtual ParametroConfiguracion MapToObject(T_LPARAMETRO_CONFIGURACION entity)
        {
            if (entity == null)
                return null;

            return new ParametroConfiguracion()
            {
                NuParametroConfiguracion = entity.NU_PARAMETRO_CONFIGURACION,
                CodigoParametro = entity.CD_PARAMETRO,
                TipoParametro = entity.DO_ENTIDAD_PARAMETRIZABLE,
                Clave = entity.ND_ENTIDAD,
                Valor = entity.VL_PARAMETRO,
            };
        }
        public virtual T_LPARAMETRO_CONFIGURACION MapToEntity(ParametroConfiguracion obj)
        {
            if (obj == null)
                return null;

            return new T_LPARAMETRO_CONFIGURACION()
            {
                NU_PARAMETRO_CONFIGURACION = obj.NuParametroConfiguracion,
                CD_PARAMETRO = obj.CodigoParametro,
                DO_ENTIDAD_PARAMETRIZABLE = obj.TipoParametro,
                ND_ENTIDAD = obj.Clave,
                VL_PARAMETRO = obj.Valor,
            };
        }
    }
}
