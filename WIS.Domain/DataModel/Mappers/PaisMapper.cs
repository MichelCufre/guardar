using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PaisMapper
    {
        public PaisMapper()
        {
        }

        public virtual Pais MapToObject(T_PAIS entity)
        {
            if (entity == null)
                return null;

            var pais = new Pais
            {
                Id = entity.CD_PAIS,
                Nombre = entity.DS_PAIS,
                CodigoAlfa3 = entity.CD_PAIS_ALFA3,
                Alta = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
            };

            return pais;
        }

        public virtual T_PAIS MapToEntity(Pais pais)
        {
            return new T_PAIS
            {
                CD_PAIS = pais.Id,
                DS_PAIS = pais.Nombre,
                CD_PAIS_ALFA3 = pais.CodigoAlfa3,
                DT_ADDROW = pais.Alta,
                DT_UPDROW = pais.Modificacion,
            };
        }

    }
}
