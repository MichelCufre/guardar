using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Liberacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class OndaMapper : Mapper
    {
        public virtual Onda MapToObject(T_ONDA entity)
        {
            if (entity == null)
                return null;

            return new Onda
            {
                Id = entity.CD_ONDA,
                Descripcion = entity.DS_ONDA,
                Estado = entity.CD_SITUACAO,
                FechaAlta = entity.DT_CADASTRAMENTO,
                FechaModificacion = entity.DT_ALTERACAO,
                FechaSituacion = entity.DT_SITUACAO,
                Predio = entity.NU_PREDIO
            };
        }

        public virtual T_ONDA MapToEntity(Onda onda)
        {
            return new T_ONDA
            {
                CD_ONDA = onda.Id,
                DS_ONDA = onda.Descripcion,
                CD_SITUACAO = onda.Estado,
                DT_CADASTRAMENTO = onda.FechaAlta,
                DT_ALTERACAO = onda.FechaModificacion,
                DT_SITUACAO = onda.FechaSituacion,
                NU_PREDIO = onda.Predio
            };
        }

        public virtual short MapEstadoBooleanToShort(bool value)
        {
            return value ? SituacionDb.Activo : SituacionDb.Inactivo;
        }

    }
}