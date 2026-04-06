using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class MonedaMapper : Mapper
    {
        public virtual Moneda MapToObject(T_MONEDA entity)
        {
            if (entity == null)
                return null;

            return new Moneda
            {
                Codigo=entity.CD_MONEDA,
                Descripcion = entity.DS_MONEDA,
                Simbolo = entity.DS_SIMBOLO,
                Alta = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
            };

        }
    }
}
