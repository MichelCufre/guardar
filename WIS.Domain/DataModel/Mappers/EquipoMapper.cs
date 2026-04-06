using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EquipoMapper : Mapper
    {
        public virtual T_EQUIPO MapToEntity(Equipo equipo)
        {
            return new T_EQUIPO
            {
                CD_EQUIPO = equipo.Id,
                DS_EQUIPO = equipo.Descripcion,
                CD_FERRAMENTA = equipo.CodigoHerramienta,
                CD_ENDERECO = equipo.CodigoUbicacion,
                CD_FUNCIONARIO = equipo.CodigoFuncionario,
                CD_APLICACAO = equipo.Aplicacion,
                DT_ADDROW = equipo.FechaInsercion,
                DT_UPDROW = equipo.FechaModificacion,
                CD_ENDERECO_REAL = NullIfEmpty(equipo.CodigoUbicacionReal),
                CD_ZONA = NullIfEmpty(equipo.CodigoZona),
                TP_OPERATIVA = equipo.TipoOperacion,
                NU_COMPONENTE = equipo.NuComponente,
            };
        }
        public virtual Equipo MapToObject(T_EQUIPO equipo)
        {
            if (equipo == null)
                return null;

            var obj = new Equipo
            {
                Id = equipo.CD_EQUIPO,
                Aplicacion = equipo.CD_APLICACAO,
                CodigoFuncionario = equipo.CD_FUNCIONARIO,
                CodigoHerramienta = equipo.CD_FERRAMENTA,
                CodigoUbicacion = equipo.CD_ENDERECO,
                Descripcion = equipo.DS_EQUIPO,
                FechaInsercion = equipo.DT_ADDROW,
                FechaModificacion = equipo.DT_UPDROW,
                CodigoUbicacionReal = equipo.CD_ENDERECO_REAL,
                CodigoZona = equipo.CD_ZONA,
                TipoOperacion = equipo.TP_OPERATIVA,
                NuComponente = equipo.NU_COMPONENTE,
            };

            if (equipo.T_FERRAMENTAS != null)
                obj.Herramienta = MapToObject(equipo.T_FERRAMENTAS);

            return obj;
        }

        public virtual T_FERRAMENTAS MapToEntity(Herramienta obj)
        {
            return new T_FERRAMENTAS
            {
                CD_FERRAMENTA = obj.Id,
                DS_FERRAMENTA = obj.Descripcion,
                ID_AUTOASIGNADO = this.MapBooleanToString(obj.Autoasignado),
                CD_SITUACAO = obj.Estado,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion,
            };
        }
        public virtual Herramienta MapToObject(T_FERRAMENTAS entity)
        {
            return new Herramienta
            {
                Id = entity.CD_FERRAMENTA,
                Descripcion = entity.DS_FERRAMENTA,
                Estado = entity.CD_SITUACAO,
                Autoasignado = this.MapStringToBoolean(entity.ID_AUTOASIGNADO),
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

    }
}
