using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class RegimenAduaneroMapper : Mapper
    {
        public virtual RegimenAduanero MapToObject(T_REGIMEN_ADUANA entity)
        {
            return new RegimenAduanero
            {
                Descripcion = entity.DS_REGIMEN_ADUANA,
                CodigoRegimen = entity.CD_REGIMEN_ADUANA
            };
        }
    }

}
