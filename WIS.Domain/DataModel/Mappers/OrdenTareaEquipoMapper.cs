using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class OrdenTareaEquipoMapper
    {
        public OrdenTareaEquipoMapper()
        {

        }

        public virtual OrdenTareaEquipo MapToObject(T_ORT_ORDEN_TAREA_EQUIPO entity)
        {
            return new OrdenTareaEquipo
            {
                NuOrtOrdenTareaEquipo = entity.NU_ORT_ORDEN_TAREA_EQUIPO,
                CdEquipo = entity.CD_EQUIPO,
                DescripcionMemo = entity.DS_MEMO,
                NuOrdenTarea = entity.NU_ORDEN_TAREA,
                FechaDesde = entity.DT_DESDE,
                FechaHasta = entity.DT_HASTA,
            };
        }

        public virtual T_ORT_ORDEN_TAREA_EQUIPO MapToEntity(OrdenTareaEquipo OrdenTareaEquipo)
        {
            return new T_ORT_ORDEN_TAREA_EQUIPO
            {
                NU_ORT_ORDEN_TAREA_EQUIPO = OrdenTareaEquipo.NuOrtOrdenTareaEquipo,
                CD_EQUIPO = OrdenTareaEquipo.CdEquipo,
                DS_MEMO = OrdenTareaEquipo.DescripcionMemo,
                NU_ORDEN_TAREA = OrdenTareaEquipo.NuOrdenTarea,
                DT_DESDE = OrdenTareaEquipo.FechaDesde,
                DT_HASTA = OrdenTareaEquipo.FechaHasta,
            };
        }
    }
}
