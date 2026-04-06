using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class GrupoMapper : Mapper
    {
        public virtual Grupo MapToObject(T_GRUPO entity)
        {
            if (entity == null)
                return null;

            return new Grupo
            {
                Id = entity.CD_GRUPO,
                Descripcion = entity.DS_GRUPO,
                CodigoClase = entity.CD_CLASSE,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Default = MapStringToBoolean(entity.FL_DEFAULT),
            };
        }
        public virtual T_GRUPO MapToEntity(Grupo obj)
        {
            return new T_GRUPO
            {
                CD_GRUPO = obj.Id,
                DS_GRUPO = obj.Descripcion,
                CD_CLASSE = NullIfEmpty(obj.CodigoClase),
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion,
                FL_DEFAULT = MapBooleanToString(obj.Default),
            };
        }

        public virtual GrupoRegla MapToObject(T_GRUPO_REGLA entity)
        {
            if (entity == null)
                return null;

            return new GrupoRegla
            {
                Id = entity.NU_GRUPO_REGLA,
                Descripcion = entity.DS_REGLA,
                CodigoGrupo = entity.CD_GRUPO,
                Orden = entity.NU_ORDEN,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }
        public virtual T_GRUPO_REGLA MapToEntity(GrupoRegla obj)
        {
            return new T_GRUPO_REGLA
            {
                NU_GRUPO_REGLA = obj.Id,
                DS_REGLA = obj.Descripcion,
                CD_GRUPO = obj.CodigoGrupo,
                NU_ORDEN = obj.Orden,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion
            };
        }

        public virtual GrupoParametro MapToObject(T_GRUPO_PARAM entity)
        {
            if (entity == null)
                return null;

            return new GrupoParametro
            {
                Id = entity.NU_PARAM,
                Nombre = entity.NM_PARAM,
                Descripcion = entity.DS_PARAM,
                Orden = entity.NU_ORDEN,
                Tipo = entity.TP_PARAM,
                ValorDefault = entity.VL_PARAM_DEFAULT,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }
        public virtual T_GRUPO_PARAM MapToEntity(GrupoParametro obj)
        {
            return new T_GRUPO_PARAM
            {
                NU_PARAM = obj.Id,
                NM_PARAM = obj.Nombre,
                DS_PARAM = obj.Descripcion,
                NU_ORDEN = obj.Orden,
                VL_PARAM_DEFAULT = NullIfEmpty(obj.ValorDefault),
                TP_PARAM = NullIfEmpty(obj.Tipo),
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion
            };
        }

        public virtual GrupoReglaParametro MapToObject(T_GRUPO_REGLA_PARAM entity)
        {
            if (entity == null)
                return null;

            return new GrupoReglaParametro
            {
                Id = entity.NU_GRUPO_REGLA_PARAM,
                NroRegla = entity.NU_GRUPO_REGLA,
                NroParametro = entity.NU_PARAM,
                Valor = entity.VL_PARAM,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }
        public virtual T_GRUPO_REGLA_PARAM MapToEntity(GrupoReglaParametro obj)
        {
            return new T_GRUPO_REGLA_PARAM
            {
                NU_GRUPO_REGLA_PARAM = obj.Id,
                NU_GRUPO_REGLA = obj.NroRegla,
                NU_PARAM = obj.NroParametro,
                VL_PARAM = NullIfEmpty(obj.Valor),
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion
            };
        }
    }
}
