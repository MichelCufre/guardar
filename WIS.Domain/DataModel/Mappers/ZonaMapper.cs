using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ZonaMapper : Mapper
    {
        public ZonaMapper()
        {
        }

        public virtual Zona MapToObject(T_ZONA entity)
        {
            if (entity == null)
                return null;

            return new Zona
            {
                CdZona = entity.CD_ZONA,
                NmZona = entity.NM_ZONA,
                DsZona = entity.DS_ZONA,
                CdDepartamento = entity.CD_DEPARTAMENTO,
                CdLocalidad = entity.CD_LOCALIDAD
            };
        }

        public virtual T_ZONA MapToEntity(Zona zona)
        {
            if (zona == null)
                return null;

            return new T_ZONA
            {
                CD_ZONA = zona.CdZona,
                NM_ZONA = zona.NmZona,
                DS_ZONA = zona.DsZona,
                CD_DEPARTAMENTO = zona.CdDepartamento,
                CD_LOCALIDAD = zona.CdLocalidad
            };
        }
    }
}
