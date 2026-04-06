using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AplicacionMapper : Mapper
    {
        #region MapToEntity

        public virtual T_APLICACION MapToEntity(Aplicacion obj)
        {
            if (obj == null) return null;

            return new T_APLICACION
            {
                CD_APLICACION = obj.Codigo,
                DS_APLICACION = obj.Descripcion,
                FL_RECORRIDO = this.MapBooleanToString(obj.ManejaRecorrido)
            };
        }

        #endregion

        #region MapToObject

        public virtual Aplicacion MapToObject(T_APLICACION entity)
        {
            if (entity == null) return null;

            return new Aplicacion
            {
                Codigo = entity.CD_APLICACION,
                Descripcion = entity.DS_APLICACION,
                ManejaRecorrido = this.MapStringToBoolean(entity.FL_RECORRIDO)
            };
        }

        #endregion
    }
}
