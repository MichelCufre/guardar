using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel
{
    public class TipoAlmacenajeSeguroMapper : Mapper
    {
        public virtual TipoDeAlmacenajeYSeguro MapToObject(T_TIPO_ALMACENAJE_SEGURO entity)
        {
            if (entity == null)
                return null;

            return new TipoDeAlmacenajeYSeguro
            {
                Tipo = entity.TP_ALMACENAJE_Y_SEGURO,
                Descripcion = entity.DS_ALMACENAJE_Y_SEGURO,
            };
        }
    }
}
