using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PredioMapper : Mapper
    {
        public PredioMapper()
        {

        }

        public virtual Predio MapPredioToObject(T_PREDIO entity)
        {
            if (entity == null) return null;

            return new Predio
            {
                Numero = entity.NU_PREDIO,
                Descripcion = entity.DS_PREDIO,
                Direccion = entity.DS_ENDERECO,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                Alta = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
                SincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA),
                IdExterno = entity.ID_EXTERNO
            };
        }

        public virtual T_PREDIO MapPredioToEntity(Predio predio)
        {
            if (predio == null) return null;

            return new T_PREDIO
            {
                NU_PREDIO = predio.Numero,
                DS_PREDIO = predio.Descripcion,
                DS_ENDERECO = predio.Direccion,
                CD_PUNTO_ENTREGA = predio.PuntoEntrega,
                DT_ADDROW = predio.Alta,
                DT_UPDROW = predio.Modificacion,
                FL_SYNC_REALIZADA = this.MapBooleanToString(predio.SincronizacionRealizada),
                ID_EXTERNO= predio.IdExterno
            };
        }

    }
}
