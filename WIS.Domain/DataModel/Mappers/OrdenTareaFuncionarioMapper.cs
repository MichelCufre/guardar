using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class OrdenTareaFuncionarioMapper
    {
        public OrdenTareaFuncionarioMapper()
        {

        }

        public virtual OrdenTareaFuncionario MapToObject(T_ORT_ORDEN_TAREA_FUNCIONARIO entity)
        {
            return new OrdenTareaFuncionario
            {
                NuOrtOrdenTareaFuncionario = entity.NU_ORT_ORDEN_TAREA_FUNC,
                CodigoFuncionario = entity.CD_FUNCIONARIO,
                DescripcionMemo = entity.DS_MEMO,
                NumeroOrdenTarea = entity.NU_ORDEN_TAREA,
                FechaDesde = entity.DT_DESDE,
                FechaHasta = entity.DT_HASTA,
            };
        }

        public virtual T_ORT_ORDEN_TAREA_FUNCIONARIO MapToEntity(OrdenTareaFuncionario OrdenTareaFuncionario)
        {
            return new T_ORT_ORDEN_TAREA_FUNCIONARIO
            {
                NU_ORT_ORDEN_TAREA_FUNC = OrdenTareaFuncionario.NuOrtOrdenTareaFuncionario,
                CD_FUNCIONARIO = OrdenTareaFuncionario.CodigoFuncionario,
                DS_MEMO = OrdenTareaFuncionario.DescripcionMemo,
                NU_ORDEN_TAREA = OrdenTareaFuncionario.NumeroOrdenTarea,
                DT_DESDE = OrdenTareaFuncionario.FechaDesde,
                DT_HASTA = OrdenTareaFuncionario.FechaHasta,
            };
        }
    }
}
