using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class LenguajeImpresionMapper
    {
        public LenguajeImpresionMapper()
        {

        }

        public virtual LenguajeImpresion MapToObject(T_LENGUAJE_IMPRESION entity)
        {
            if (entity == null) return null;

            return new LenguajeImpresion
            {
                Id = entity.CD_LENGUAJE_IMPRESION,
                Descripcion = entity.DS_LENGUAJE_IMPRESION
            };
        }
        public virtual T_LENGUAJE_IMPRESION MapToEntity(LenguajeImpresion obj)
        {
            if (obj == null) return null;

            return new T_LENGUAJE_IMPRESION
            {
                CD_LENGUAJE_IMPRESION = obj.Id,
                DS_LENGUAJE_IMPRESION = obj.Descripcion
            };
        }
    }
}
