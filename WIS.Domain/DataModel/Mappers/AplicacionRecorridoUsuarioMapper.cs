using WIS.Domain.Recorridos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AplicacionRecorridoUsuarioMapper : Mapper
    {
        #region MapToEntity

        public virtual T_APLICACION_RECORRIDO_USUARIO MapToEntity(AplicacionRecorridoUsuario obj)
        {
            if (obj == null) return null;

            return new T_APLICACION_RECORRIDO_USUARIO
            {
                CD_APLICACION = obj.IdAplicacion,
                NU_RECORRIDO = obj.IdRecorrido,
                FL_PREDETERMINADO = this.MapBooleanToString(obj.EsPredeterminado),
                NU_TRANSACCION = obj.NuTransaccion,
                USERID = obj.UserId,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION_DELETE = obj.NuTransaccionDelete
            };
        }

        #endregion

        #region MapToObject

        public virtual AplicacionRecorridoUsuario MapToObject(T_APLICACION_RECORRIDO_USUARIO entity)
        {
            if (entity == null) return null;

            return new AplicacionRecorridoUsuario
            {
                IdAplicacion = entity.CD_APLICACION,
                IdRecorrido = entity.NU_RECORRIDO,
                EsPredeterminado = this.MapStringToBoolean(entity.FL_PREDETERMINADO),
                NuTransaccion = entity.NU_TRANSACCION,
                UserId = entity.USERID,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NuTransaccionDelete = entity.NU_TRANSACCION_DELETE
            };
        }

        #endregion
    }
}
