using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class OrdenMapper : Mapper
    {
        public OrdenMapper()
        {

        }

        public virtual Orden MapToObject(T_ORT_ORDEN entity)
        {
            return new Orden
            {
                Id = entity.NU_ORT_ORDEN,
                Descripcion = entity.DS_ORT_ORDEN,
                Funcionario = entity.CD_FUNCIONARIO_ADDROW,
                DsReferencia = entity.DS_REFERENCIA,
                Estado = entity.ID_ESTADO,
                FechaAgregado = entity.DT_ADDROW,
                FechaInicio = entity.DT_INICIO,
                FechaFin = entity.DT_FIN,
                FechaUltimaOperacion = entity.DT_ULTIMA_OPERACION
            };
        }

        public virtual OrdenSesion MapToObject(T_ORT_ORDEN_SESION entity)
        {
            return new OrdenSesion
            {
                NuOrtOrdenSesion = entity.NU_ORT_ORDEN_SESION,
                NuOrtOrden = entity.NU_ORT_ORDEN,
                CdFuncionario = entity.CD_FUNCIONARIO,
                DtInicio = entity.DT_INICIO,
                DtFin = entity.DT_FIN,
            };
        }

        public virtual OrdenSesionEquipo MapToObject(T_ORT_ORDEN_SESION_EQUIPO entity)
        {
            return new OrdenSesionEquipo
            {
                NuOrtOrdenSesionEquipo = entity.NU_ORT_ORDEN_SESION_EQUIPO,
                NuOrtOrdenSesion = entity.NU_ORT_ORDEN_SESION,
                CdEquipo = entity.CD_EQUIPO,
                DtInicio = entity.DT_INICIO,
                DtFin = entity.DT_FIN,
            };
        }

        public virtual T_ORT_ORDEN MapToEntity(Orden orden)
        {
            return new T_ORT_ORDEN
            {
                NU_ORT_ORDEN = orden.Id,
                DS_ORT_ORDEN = orden.Descripcion,
                CD_FUNCIONARIO_ADDROW = orden.Funcionario,
                DS_REFERENCIA = orden.DsReferencia,
                ID_ESTADO = orden.Estado,
                DT_ADDROW = orden.FechaAgregado,
                DT_INICIO = orden.FechaInicio,
                DT_FIN = orden.FechaFin,
                DT_ULTIMA_OPERACION = orden.FechaUltimaOperacion,
            }; 
        }

      

        public virtual T_ORT_ORDEN_SESION_EQUIPO MapToEntity(OrdenSesionEquipo ordenSesionEquipo)
        {
            return new T_ORT_ORDEN_SESION_EQUIPO
            {
                NU_ORT_ORDEN_SESION_EQUIPO = ordenSesionEquipo.NuOrtOrdenSesionEquipo,
                NU_ORT_ORDEN_SESION = ordenSesionEquipo.NuOrtOrdenSesion,
                CD_EQUIPO = ordenSesionEquipo.CdEquipo,
                DT_INICIO = ordenSesionEquipo.DtInicio,
                DT_FIN = ordenSesionEquipo.DtFin,
            };
        }

        public virtual T_ORT_ORDEN_SESION MapToEntity(OrdenSesion ordenSesion)
        {
            return new T_ORT_ORDEN_SESION
            {
                NU_ORT_ORDEN_SESION = ordenSesion.NuOrtOrdenSesion,
                NU_ORT_ORDEN = ordenSesion.NuOrtOrden,
                CD_FUNCIONARIO = ordenSesion.CdFuncionario,
                DT_INICIO = ordenSesion.DtInicio,
                DT_FIN = ordenSesion.DtFin,
            };
        }
    }
}
