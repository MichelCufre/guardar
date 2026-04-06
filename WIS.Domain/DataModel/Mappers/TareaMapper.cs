using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TareaMapper
    {
        public TareaMapper()
        {

        }

        public virtual Tarea MapToObject(T_ORT_TAREA entity)
        {
            return new Tarea
            {
                Id = entity.CD_TAREA,
                Descripcion = entity.DS_TAREA,
                CodigoSituacion = entity.CD_SITUACAO,
                NumeroComponente = entity.NU_COMPONENTE,
                TipoTarea = entity.TP_TAREA,
                RegistroHoras = entity.FL_REGISTRO_HORAS,
                RegistroManipuleo = entity.FL_REGISTRO_MANIPULEO,
                RegistroInsumos = entity.FL_REGISTRO_INSUMOS,
                RegistroHorasEquipo = entity.FL_REGISTRO_HORAS_EQUIPO,
            };
        }

        public virtual T_ORT_TAREA MapToEntity(Tarea tarea)
        {
            return new T_ORT_TAREA
            {
                CD_TAREA = tarea.Id,
                DS_TAREA =  tarea.Descripcion,
                CD_SITUACAO = tarea.CodigoSituacion,
                NU_COMPONENTE = tarea.NumeroComponente,
                TP_TAREA = tarea.TipoTarea,
                FL_REGISTRO_HORAS = tarea.RegistroHoras,
                FL_REGISTRO_MANIPULEO = tarea.RegistroManipuleo,
                FL_REGISTRO_INSUMOS = tarea.RegistroInsumos,
                FL_REGISTRO_HORAS_EQUIPO = tarea.RegistroHorasEquipo,
            };
        }

        public virtual OrdenTareaObjeto MapToObject(T_ORT_ORDEN_TAREA entity)
        {
            return new OrdenTareaObjeto
            {
                NuTarea = entity.NU_ORDEN_TAREA,
                CdTarea = entity.CD_TAREA,
                Resuelta = entity.FL_RESUELTA,
                NuOrden = entity.NU_ORT_ORDEN,
                Empresa = entity.CD_EMPRESA,
                CdFuncionarioAddrow = entity.CD_FUNCIONARIO_ADDROW,
                DtAddrow = entity.DT_ADDROW,
                DtUpdrow = entity.DT_UPDROW,
            };
        }
        public virtual T_ORT_ORDEN_TAREA MapToEntity(OrdenTareaObjeto entity)
        {
            return new T_ORT_ORDEN_TAREA
            {
                NU_ORDEN_TAREA = entity.NuTarea,
                CD_TAREA = entity.CdTarea,
                FL_RESUELTA = entity.Resuelta,
                NU_ORT_ORDEN = entity.NuOrden,
                CD_EMPRESA = entity.Empresa,
                CD_FUNCIONARIO_ADDROW = entity.CdFuncionarioAddrow,
                DT_ADDROW = entity.DtAddrow,
                DT_UPDROW = entity.DtUpdrow,
            };
        }
    }
}
