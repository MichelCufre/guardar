using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PaisSubdivisionLocalidadMapper
    {
        protected readonly PaisSubdivisionMapper _paisSubdivisionMapper;

        public PaisSubdivisionLocalidadMapper()
        {
        }

        public PaisSubdivisionLocalidadMapper(PaisSubdivisionMapper paisSubdivisionMapper)
        {
            _paisSubdivisionMapper = paisSubdivisionMapper;
        }
        public virtual PaisSubdivisionLocalidad MapToObject(T_PAIS_SUBDIVISION_LOCALIDAD entity)
        {
            if (entity == null)
                return null;

            return new PaisSubdivisionLocalidad
            {
                Id = entity.ID_LOCALIDAD,
                Codigo = entity.CD_LOCALIDAD,
                CodigoSubDivicion = entity.CD_SUBDIVISION,
                Nombre = entity.NM_LOCALIDAD,
                CodigoIATA = entity.CD_IATA,
                CodigoPostal = entity.CD_POSTAL,
                Subdivision = _paisSubdivisionMapper.MapToObject(entity.T_PAIS_SUBDIVISION)
            };
        }

        public virtual T_PAIS_SUBDIVISION_LOCALIDAD MapToEntity(PaisSubdivisionLocalidad localidad)
        {
            return new T_PAIS_SUBDIVISION_LOCALIDAD
            {
                ID_LOCALIDAD = localidad.Id,
                CD_LOCALIDAD = localidad.Codigo,
                CD_SUBDIVISION = localidad.CodigoSubDivicion,
                NM_LOCALIDAD = localidad.Nombre,
                CD_IATA = localidad.CodigoIATA,
                CD_POSTAL = localidad.CodigoPostal
            };
        }

    }
}
