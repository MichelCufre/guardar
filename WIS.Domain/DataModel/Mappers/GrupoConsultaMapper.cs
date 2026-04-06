using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class GrupoConsultaMapper : Mapper
    {
        public GrupoConsultaMapper()
        {

        }

        public virtual T_GRUPO_CONSULTA MapToEntity(GrupoConsulta grupo)
        {
            return new T_GRUPO_CONSULTA
            {
                DS_GRUPO_CONSULTA = grupo.Descripcion,
                CD_GRUPO_CONSULTA = grupo.Id
            };
        }

        public virtual GrupoConsulta MapToObject(T_GRUPO_CONSULTA grupo)
        {
            return new GrupoConsulta
            {
                Id = grupo.CD_GRUPO_CONSULTA,
                Descripcion = grupo.DS_GRUPO_CONSULTA
            };
        }

    }
}
