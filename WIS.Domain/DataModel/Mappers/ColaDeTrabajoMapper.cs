using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ColaDeTrabajoMapper : Mapper
    {
        public ColaDeTrabajoMapper()
        {
        }

        public virtual ColaDeTrabajo MapToObject(T_COLA_TRABAJO entity)
        {
            if (entity == null)
                return null;

            return new ColaDeTrabajo
            {
                Numero = entity.NU_COLA_TRABAJO,
                Predio = entity.NU_PREDIO,
                Descripcion = entity.DS_COLA_TRABAJO,
                dtUpdrow = entity.DT_UPDROW,
                dtAddrow = entity.DT_ADDROW,
                flOrdenCalendario = entity.FL_ORDEN_CALENDARIO
            };
        }
        public virtual T_COLA_TRABAJO MapToEntity(ColaDeTrabajo obj)
        {
            if (obj == null)
                return null;

            return new T_COLA_TRABAJO
            {
                NU_COLA_TRABAJO = obj.Numero,
                NU_PREDIO = obj.Predio,
                DS_COLA_TRABAJO = obj.Descripcion,
                DT_ADDROW = obj.dtAddrow,
                DT_UPDROW = obj.dtUpdrow,
                FL_ORDEN_CALENDARIO = obj.flOrdenCalendario
            };
        }
        public virtual ColaDeTrabajoPonderador MapToObject(T_COLA_TRABAJO_PONDERADOR entity)
        {
            if (entity == null)
                return null;

            return new ColaDeTrabajoPonderador
            {
                Numero = entity.NU_COLA_TRABAJO,
                Ponderador = entity.CD_PONDERADOR,
                Incremento = entity.NU_INCREMENTO,
                dtUpdrow = entity.DT_UPDROW,
                dtAddrow = entity.DT_ADDROW,
                Habilitado = this.MapStringToBoolean(entity.FL_HABILITADO),
            };
        }
        public virtual T_COLA_TRABAJO_PONDERADOR MapToEntity(ColaDeTrabajoPonderador obj)
        {
            if (obj == null)
                return null;

            return new T_COLA_TRABAJO_PONDERADOR
            {
                NU_COLA_TRABAJO = obj.Numero,
                CD_PONDERADOR = obj.Ponderador,
                NU_INCREMENTO = obj.Incremento,
                DT_ADDROW = obj.dtAddrow,
                DT_UPDROW = obj.dtUpdrow,
                FL_HABILITADO = this.MapBooleanToString(obj.Habilitado),
            };
        }

        public virtual ColaDeTrabajoPonderadorDetalle MapToObject(T_COLA_TRABAJO_PONDERADOR_DET entity)
        {
            if (entity == null)
                return null;

            return new ColaDeTrabajoPonderadorDetalle
            {
                Numero = entity.NU_COLA_TRABAJO,
                Ponderador = entity.CD_PONDERADOR,
                Instancia = entity.CD_INST_PONDERADOR,
                Operacion = entity.VL_OPERACION,
                NuPonderacion = entity.NU_PONDERACION,
                dtUpdrow = entity.DT_UPDROW,
                dtAddrow = entity.DT_ADDROW
            };
        }
        public virtual T_COLA_TRABAJO_PONDERADOR_DET MapToEntity(ColaDeTrabajoPonderadorDetalle obj)
        {
            if (obj == null)
                return null;

            return new T_COLA_TRABAJO_PONDERADOR_DET
            {
                NU_COLA_TRABAJO = obj.Numero,
                CD_PONDERADOR = obj.Ponderador,
                CD_INST_PONDERADOR = obj.Instancia,
                VL_OPERACION = obj.Operacion,
                NU_PONDERACION = obj.NuPonderacion,
                DT_ADDROW = obj.dtAddrow,
                DT_UPDROW = obj.dtUpdrow
            };
        }

        public virtual PonderadorInstancia MapToObject(T_COLA_TRABAJO_PONDERADOR_INST entity)
        {
            if (entity == null)
                return null;

            return new PonderadorInstancia
            {
                Codigo = entity.CD_PONDERADOR,
                Descripcion = entity.DS_PONDERADOR,
                Habilitado = this.MapStringToBoolean(entity.FL_HABILITADO_DEFAULT),
                IncrementoDefault = entity.NU_INCREMENTO_DEFAULT,
                PonderacionDefault = entity.NU_PONDERACION_DEFAULT,
                TipoDato = entity.TP_DATO
            };
        }
    }
}
