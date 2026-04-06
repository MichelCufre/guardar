using WIS.Domain.Recorridos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AplicacionRecorridoMapper : Mapper
    {
        #region MapToEntity

        public virtual T_APLICACION_RECORRIDO MapToEntity(AplicacionRecorrido obj)
        {
            if (obj == null) return null;

            return new T_APLICACION_RECORRIDO
            {
                CD_APLICACION = obj.IdAplicacion,
                NU_RECORRIDO = obj.IdRecorrido,
                FL_PREDETERMINADO = this.MapBooleanToString(obj.EsPredeterminado),
                NU_TRANSACCION = obj.NuTransaccion,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION_DELETE = obj.NuTransaccionDelete

            };
        }

        #endregion

        #region MapToObject

        public virtual AplicacionRecorrido MapToObject(T_APLICACION_RECORRIDO entity)
        {
            if (entity == null) return null;

            return new AplicacionRecorrido
            {
                IdAplicacion = entity.CD_APLICACION,
                IdRecorrido = entity.NU_RECORRIDO,
                EsPredeterminado = this.MapStringToBoolean(entity.FL_PREDETERMINADO),
                NuTransaccion = entity.NU_TRANSACCION,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NuTransaccionDelete = entity.NU_TRANSACCION_DELETE
            };
        }
        #endregion
    }
}
