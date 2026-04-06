using System;
using System.Text;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CodigoMultidatoMapper : Mapper
    {
        #region ToEntity

        public virtual T_CODIGO_MULTIDATO_EMPRESA_DET MapToEntity(CodigoMultidatoEmpresaDetalle entity)
        {
            return new T_CODIGO_MULTIDATO_EMPRESA_DET
            {
                CD_EMPRESA = entity.CodigoEmpresa,
                CD_CODIGO_MULTIDATO = entity.CodigoMultidato,
                CD_APLICACION = entity.CodigoAplicacion,
                CD_CAMPO = entity.CodigoCampo,
                CD_AI = entity.CodigoAI,
                NU_ORDEN = entity.NumeroOrden,
                DT_ADDROW = entity.FechaAlta,
                DT_UPDROW = entity.FechaModificacion,
                NU_TRANSACCION = entity.NumeroTransaccion,
                NU_TRANSACCION_DELETE = entity.NumeroTransaccionDelete,
            };
        }

        public virtual T_CODIGO_MULTIDATO_EMPRESA MapToEntity(CodigoMultidatoEmpresa entity)
        {
            return new T_CODIGO_MULTIDATO_EMPRESA
            {
                CD_EMPRESA = entity.CodigoEmpresa,
                CD_CODIGO_MULTIDATO = entity.CodigoMultidato,
                FL_HABILITADO = entity.Habilitado,
                DT_ADDROW = entity.FechaAlta,
                DT_UPDROW = entity.FechaModificacion,
                NU_TRANSACCION = entity.NumeroTransaccion,
                NU_TRANSACCION_DELETE = entity.NumeroTransaccionDelete,
            };
        }

        #endregion

        #region ToObject

        public virtual General.CodigoMultidato MapToObject(V_CODIGO_MULTIDATO entity)
        {
            return new General.CodigoMultidato
            {
                Codigo = entity.CD_CODIGO_MULTIDATO,
                Descripcion = entity.DS_CODIGO_MULTIDATO,
            };
        }

        public virtual General.CodigoMultidato MapToObject(T_CODIGO_MULTIDATO entity)
        {
            return new General.CodigoMultidato
            {
                Codigo = entity.CD_CODIGO_MULTIDATO,
                Descripcion = entity.DS_CODIGO_MULTIDATO,
                Regex = entity.VL_REGEX != null ? Encoding.UTF8.GetString(entity.VL_REGEX) : null,
            };
        }

        public virtual CodigoMultidatoEmpresa MapToObject(T_CODIGO_MULTIDATO_EMPRESA entity)
        {
            return new CodigoMultidatoEmpresa
            {
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoMultidato = entity.CD_CODIGO_MULTIDATO,
                Habilitado = entity.FL_HABILITADO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual Aplicacion MapToObject(V_APLICACION entity)
        {
            return new Aplicacion
            {
                Codigo = entity.CD_APLICACION,
                Descripcion = entity.DS_APLICACION,
            };
        }

        public virtual AplicacionCampo MapToObject(V_APLICACION_CAMPO entity)
        {
            return new AplicacionCampo
            {
                CodigoAplicacion = entity.CD_APLICACION,
                CodigoCampo = entity.CD_CAMPO,
                Descripcion = entity.DS_CAMPO,
                FlagCodigoMultidato = entity.FL_CODIGO_MULTIDATO,
            };
        }

        public virtual DetalleCodigoMultidato MapToObject(V_CODIGO_MULTIDATO_DET entity)
        {
            return new DetalleCodigoMultidato
            {
                CodigoMultidato = entity.CD_CODIGO_MULTIDATO,
                CodigoAI = entity.CD_AI,
                Descripcion = entity.DS_AI,
            };
        }

        public virtual DetalleCodigoMultidato MapToObject(T_CODIGO_MULTIDATO_DET entity)
        {
            if (entity == null)
                return null;

            return new DetalleCodigoMultidato
            {
                CodigoMultidato = entity.CD_CODIGO_MULTIDATO,
                CodigoAI = entity.CD_AI,
                Descripcion = entity.DS_AI,
                TipoAI = entity.TP_AI,
                Conversion = entity.VL_CONVERSION,
            };
        }

        public virtual CodigoMultidatoEmpresaDetalle MapToObject(T_CODIGO_MULTIDATO_EMPRESA_DET entity)
        {
            return new CodigoMultidatoEmpresaDetalle
            {
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoMultidato = entity.CD_CODIGO_MULTIDATO,
                CodigoAplicacion = entity.CD_APLICACION,
                CodigoCampo = entity.CD_CAMPO,
                CodigoAI = entity.CD_AI,
                NumeroOrden = entity.NU_ORDEN,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual CodigoMultidatoEmpresaDetalle MapToObject(int empresa, T_CODIGO_MULTIDATO_APLICACION entity)
        {
            return new CodigoMultidatoEmpresaDetalle
            { 
                CodigoAI = entity.CD_AI,
                CodigoAplicacion = entity.CD_APLICACION,
                CodigoCampo = entity.CD_CAMPO,
                CodigoEmpresa = empresa,
                CodigoMultidato = entity.CD_CODIGO_MULTIDATO,
                FechaAlta = DateTime.Now,
                FechaModificacion = DateTime.Now,
                NumeroOrden = entity.NU_ORDEN,
            };
        }


        #endregion

    }
}
