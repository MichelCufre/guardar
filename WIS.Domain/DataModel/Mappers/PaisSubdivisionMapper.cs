using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PaisSubdivisionMapper
    {
        protected readonly PaisMapper _paisMapper;

        public PaisSubdivisionMapper(PaisMapper paisMapper)
        {
            _paisMapper = paisMapper;
        }

        public virtual PaisSubdivision MapToObject(T_PAIS_SUBDIVISION entity)
        {
            if (entity == null)
                return null;

            return new PaisSubdivision
            {
                Id = entity.CD_SUBDIVISION,
                Nombre = entity.NM_SUBDIVISION,
                IdPais = entity.CD_PAIS,
                Pais = _paisMapper.MapToObject(entity.T_PAIS)
            };

        }

        public virtual T_PAIS_SUBDIVISION MapToEntity(PaisSubdivision localidad)
        {
            return new T_PAIS_SUBDIVISION
            {
                CD_SUBDIVISION = localidad.Id,
                NM_SUBDIVISION = localidad.Nombre,
                CD_PAIS = localidad.IdPais
            };
        }

    }
}
