using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
    public class ViaMapper : Mapper
    {
        public virtual Via MapToObject(T_VIA entity)
        {
            return new Via
            {
                Id = entity.CD_VIA,
                Descripcion = entity.DS_VIA
            };
        }
    }
}
