using WIS.Domain.Parametrizacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AtributoMapper : Mapper
    {
        public virtual AtributoSistema MapToObject(T_ATRIBUTO_SISTEMA entity)
        {
            return new AtributoSistema
            {
                Nombre = entity.NM_CAMPO,
                Descripcion = entity.DS_CAMPO
            };
        }

        public virtual Atributo MapToObject(T_ATRIBUTO entity)
        {
            return new Atributo
            {
                CodigoDominio = entity.CD_DOMINIO,
                Descripcion = entity.DS_ATRIBUTO,
                Id = entity.ID_ATRIBUTO,
                IdTipo = entity.ID_ATRIBUTO_TIPO,
                Nombre = entity.NM_ATRIBUTO,
                Campo = entity.NM_CAMPO,
                Largo = entity.NU_LARGO,
                Decimales = entity.NU_DECIMALES,
                MascaraDisplay = entity.VL_MASCARA_DISPLAY,
                MascaraIngreso = entity.VL_MASCARA_INGRESO,
                Separador = entity.VL_SEPARADOR
            };
        }

        public virtual T_ATRIBUTO MapToEntity(Atributo obj)
        {
            return new T_ATRIBUTO
            {
                CD_DOMINIO = obj.CodigoDominio,
                DS_ATRIBUTO = obj.Descripcion,
                ID_ATRIBUTO = obj.Id,
                ID_ATRIBUTO_TIPO = obj.IdTipo,
                NM_ATRIBUTO = obj.Nombre.ToUpper(),
                NM_CAMPO = obj.Campo,
                NU_LARGO = obj.Largo,
                NU_DECIMALES = obj.Decimales,
                VL_MASCARA_DISPLAY = obj.MascaraDisplay,
                VL_MASCARA_INGRESO = obj.MascaraIngreso,
                VL_SEPARADOR = obj.Separador
            };
        }

        public virtual AtributoValidacion MapToObject(T_ATRIBUTO_VALIDACION entity)
        {
            if (entity == null)
                return null;

            return new AtributoValidacion
            {
                Id = entity.ID_VALIDACION,
                NombreValidacion = entity.NM_VALIDACION,
                Descripcion = entity.DS_VALIDACION,
                AtributoTipo = entity.ID_ATRIBUTO_TIPO,
                NombreArgumento = entity.NM_ARGUMENTO,
                TipoArgumento = entity.TP_ARGUMENTO,
                Error = entity.DS_ERROR,
            };
        }

        public virtual T_ATRIBUTO_VALIDACION MapToEntity(AtributoValidacion obj)
        {
            if (obj == null)
                return null;

            return new T_ATRIBUTO_VALIDACION
            {
                ID_VALIDACION = obj.Id,
                NM_VALIDACION = obj.NombreValidacion,
                DS_VALIDACION = obj.Descripcion,
                ID_ATRIBUTO_TIPO = obj.AtributoTipo,
                NM_ARGUMENTO = obj.NombreArgumento,
                TP_ARGUMENTO = obj.TipoArgumento,
                DS_ERROR = obj.Error,
            };
        }

        public virtual AtributoValidacionAsociada MapToObject(T_ATRIBUTO_VALIDACION_ASOCIADA entity)
        {
            if (entity == null)
                return null;

            return new AtributoValidacionAsociada
            {
                IdAtributo = entity.ID_ATRIBUTO,
                IdValidacion = entity.ID_VALIDACION,
                Valor = entity.VL_ARGUMENTO,

            };
        }

        public virtual T_ATRIBUTO_VALIDACION_ASOCIADA MapToEntity(AtributoValidacionAsociada obj)
        {
            if (obj == null)
                return null;

            return new T_ATRIBUTO_VALIDACION_ASOCIADA
            {
                ID_ATRIBUTO = obj.IdAtributo,
                ID_VALIDACION = obj.IdValidacion,
                VL_ARGUMENTO = obj.Valor,

            };
        }

        public virtual AtributoTipo MapToObject(T_ATRIBUTO_TIPO entity)
        {
            if (entity == null) { return null; }

            return new AtributoTipo
            {
                Id = entity.ID_ATRIBUTO_TIPO,
                Descripcion = entity.DS_ATRIBUTO_TIPO
            };
        }

        public virtual T_ATRIBUTO_TIPO MapToEntity(AtributoTipo obj)
        {
            if (obj == null) { return null; }

            return new T_ATRIBUTO_TIPO
            {
                ID_ATRIBUTO_TIPO = obj.Id,
                DS_ATRIBUTO_TIPO = obj.Descripcion
            };
        }

        public virtual AtributoDisponible MapToObject(V_PAR401_ASOCIAR_ATRIBUTO_TIPO entity)
        {
            if (entity == null) { return null; }

            return new AtributoDisponible
            {
                Id = entity.ID_ATRIBUTO,
                Descripcion = entity.NM_ATRIBUTO,
                TipoLpn = entity.TP_LPN_TIPO
            };
        }

        public virtual AtributoDisponible MapToObject(V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET entity)
        {
            if (entity == null) { return null; }

            return new AtributoDisponible
            {
                Id = entity.ID_ATRIBUTO,
                Descripcion = entity.NM_ATRIBUTO,
                TipoLpn = entity.TP_LPN_TIPO
            };
        }

        public virtual AtributoEstado MapToObject(T_ATRIBUTO_ESTADO entity)
        {
            if (entity == null) return null;

            return new AtributoEstado
            {
                Id = entity.ID_ESTADO,
                Descripcion = entity.DS_ESTADO
            };
        }

        public virtual T_ATRIBUTO_ESTADO MapToEntity(AtributoEstado obj)
        {
            if (obj == null) return null;

            return new T_ATRIBUTO_ESTADO
            {
                ID_ESTADO = obj.Id,
                DS_ESTADO = obj.Descripcion
            };
        }

    }
}
