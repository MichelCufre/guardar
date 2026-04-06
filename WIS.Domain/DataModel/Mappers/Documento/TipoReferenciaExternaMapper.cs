using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
    public class TipoReferenciaExternaMapper : Mapper
    {
        public virtual TipoReferenciaExterna MapToObject(T_TIPO_REFERENCIA_EXTERNA entity)
        {
            return new TipoReferenciaExterna
            {
                Id = entity.TP_REFERENCIA_EXTERNA,
                Descripcion = entity.DS_REFERENCIA_EXTERNA
            };
        }
    }
}
