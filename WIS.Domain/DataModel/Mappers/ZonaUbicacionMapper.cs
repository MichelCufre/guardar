using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ZonaUbicacionMapper : Mapper
    {
        public ZonaUbicacionMapper()
        {
        }

        public virtual ZonaUbicacion MapToObject(T_ZONA_UBICACION entity)
        {
            if (entity == null)
                return null;

            return new ZonaUbicacion
            {
                Id = entity.CD_ZONA_UBICACION,
                Descripcion = entity.DS_ZONA_UBICACION,
                TipoZonaUbicacion = entity.TP_ZONA_UBICACION,
                ZonaUbicacionPicking = entity.CD_ZONA_UBICACION_PICKING,
                Estacion = entity.CD_ESTACION,
                EstacionAlmacenado = entity.CD_ESTACION_ALMACENAJE,
                Habilitada = this.MapStringToBoolean(entity.FL_HABILITADA),
                Alta = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
                IdInterno = entity.ID_ZONA_UBICACION,
            };
        }

        public virtual ControlAcceso MapToObject(T_CONTROL_ACCESO entity)
        {
            if (entity == null)
                return null;

            return new ControlAcceso
            {
                Id = entity.CD_CONTROL_ACCESO,
                Descripcion = entity.DS_CONTROL_ACCESO
            };
        }

        public virtual T_ZONA_UBICACION MapToEntity(ZonaUbicacion ubicacion)
        {
            if (ubicacion == null) return null;

            return new T_ZONA_UBICACION
            {
                CD_ZONA_UBICACION = ubicacion.Id,
                DS_ZONA_UBICACION = ubicacion.Descripcion,
                TP_ZONA_UBICACION = ubicacion.TipoZonaUbicacion,
                CD_ZONA_UBICACION_PICKING = NullIfEmpty(ubicacion.ZonaUbicacionPicking),
                CD_ESTACION = ubicacion.Estacion,
                CD_ESTACION_ALMACENAJE = ubicacion.EstacionAlmacenado,
                FL_HABILITADA = this.MapBooleanToString(ubicacion.Habilitada),
                DT_ADDROW = ubicacion.Alta,
                DT_UPDROW = ubicacion.Modificacion,
                ID_ZONA_UBICACION = ubicacion.IdInterno,
            };
        }
    }
}
