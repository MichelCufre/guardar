using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ClaseMapper : Mapper
    {
        public ClaseMapper()
        {
        }

        public virtual Clase MapToObject(T_CLASSE clase)
        {
            if (clase == null)
                return null;

            return new Clase
            {
                Id = clase.CD_CLASSE,
                Descripcion = clase.DS_CLASSE,
                IdSuperClase = clase.CD_SUB_CLASSE,
                FechaInsercion = clase.DT_ADDROW,
                FechaModificacion = clase.DT_UPDROW
            };
        }

        public virtual T_CLASSE MapToEntity(Clase clase)
        {
            return new T_CLASSE
            {
                CD_CLASSE = clase.Id,
                DS_CLASSE = clase.Descripcion,
                CD_SUB_CLASSE = NullIfEmpty(clase.IdSuperClase),
                DT_ADDROW = clase.FechaInsercion,
                DT_UPDROW = clase.FechaModificacion
            };
        }
    }
}
