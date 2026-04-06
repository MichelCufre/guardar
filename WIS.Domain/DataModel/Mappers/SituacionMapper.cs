using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class SituacionMapper : Mapper
    {
        public virtual Situacion MapToObject(T_SITUACAO situacion)
        {
            if (situacion == null)
                return null;

            return new Situacion
            {
                Id = situacion.CD_SITUACAO,
                Descripcion = situacion.DS_SITUACAO,
                Interno = this.MapStringToBoolean(situacion.ID_CODIGO_INTERNO_CET),
                FechaInsercion = situacion.DT_ADDROW,
                FechaActualizacion = situacion.DT_UPDROW
            };
        }

        public virtual T_SITUACAO MapToEntity(Situacion situacion)
        {
            if (situacion == null)
                return null;

            return new T_SITUACAO
            {
                CD_SITUACAO = situacion.Id,
                DS_SITUACAO = situacion.Descripcion,
                ID_CODIGO_INTERNO_CET = this.MapBooleanToString(situacion.Interno),
                DT_ADDROW = situacion.FechaInsercion,
                DT_UPDROW = situacion.FechaActualizacion
            };
        }
    }

}
