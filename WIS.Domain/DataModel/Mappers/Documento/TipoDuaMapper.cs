using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
    public class TipoDuaMapper : Mapper
    {
        public virtual TipoDua MapToObject(T_TIPO_DUA entity)
        {
            return new TipoDua
            {
                Id = entity.TP_DUA,
                Descripcion = entity.DS_DUA
            };
        }
    }
}
