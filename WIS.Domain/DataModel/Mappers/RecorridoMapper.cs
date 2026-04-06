using WIS.Domain.Recorridos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class RecorridoMapper : Mapper
    {
        #region MapToEntity
        public virtual T_RECORRIDO MapToEntity(Recorrido obj)
        {
            if (obj == null) return null;

            return new T_RECORRIDO
            {
                NU_RECORRIDO = obj.Id,
                NM_RECORRIDO = obj.Nombre.ToUpper(),
                DS_RECORRIDO = obj.Descripcion,
                FL_DEFAULT = this.MapBooleanToString(obj.EsDefault),
                NU_PREDIO = obj.Predio,
                FL_HABILITADO = MapBooleanToString(obj.EsHabilitado),
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
            };
        }

        public virtual T_RECORRIDO_DET MapToEntity(DetalleRecorrido obj)
        {
            if (obj == null) return null;

            return new T_RECORRIDO_DET
            {
                NU_RECORRIDO_DET = obj.Id,
                NU_RECORRIDO = obj.IdRecorrido,
                CD_ENDERECO = obj.Ubicacion,
                NU_ORDEN = obj.NumeroOrden ?? -1,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
                VL_ORDEN = obj.ValorOrden,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
            };
        }
        #endregion

        #region MapToObject
        public virtual Recorrido MapToObject(T_RECORRIDO entity)
        {
            if (entity == null) return null;

            return new Recorrido
            {
                Id = entity.NU_RECORRIDO,
                Nombre = entity.NM_RECORRIDO,
                Descripcion = entity.DS_RECORRIDO,
                EsDefault = MapStringToBoolean(entity.FL_DEFAULT),
                Predio = entity.NU_PREDIO,
                EsHabilitado = MapStringToBoolean(entity.FL_HABILITADO),
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE
            };
        }
        #endregion

    }
}
