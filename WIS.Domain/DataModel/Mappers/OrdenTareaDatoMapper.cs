using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class OrdenTareaDatoMapper
    {
        public OrdenTareaDatoMapper()
        {

        }

        public virtual OrdenTareaManipuleoInsumo MapToObject(T_ORT_ORDEN_TAREA_DATO entity)
        {
            return new OrdenTareaManipuleoInsumo
            {
                NumeroOrdenTareaDato = entity.NU_ORT_ORDEN_TAREA_DATO,
                NumeroOrdenTarea = entity.NU_ORDEN_TAREA,
                Referencia = entity.DS_REFERENCIA,
                CodigoInsumoManipuleo = entity.CD_INSUMO_MANIPULEO,
                Cantidad = entity.QT_INSUMO_MANIPULEO,
            };
        }

        public virtual T_ORT_ORDEN_TAREA_DATO MapToEntity(OrdenTareaManipuleoInsumo ordenManipuleoInsumo)
        {
            return new T_ORT_ORDEN_TAREA_DATO
            {
                NU_ORT_ORDEN_TAREA_DATO = ordenManipuleoInsumo.NumeroOrdenTareaDato,
                NU_ORDEN_TAREA = ordenManipuleoInsumo.NumeroOrdenTarea,
                DS_REFERENCIA = ordenManipuleoInsumo.Referencia,
                CD_INSUMO_MANIPULEO = ordenManipuleoInsumo.CodigoInsumoManipuleo,
                QT_INSUMO_MANIPULEO = ordenManipuleoInsumo.Cantidad
            };
        }
    }
}
