using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class DespachanteMapper : Mapper
    {
        public virtual Despachante MapToObject(T_DESPACHANTE entity)
        {
            return new Despachante
            {
                Id = entity.CD_DESPACHANTE,
                Nombre = entity.NM_DESPACHANTE,
                Telefono = entity.NU_TELEFONE
            };
        }
    }
}
