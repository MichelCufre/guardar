using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Eventos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ArchivoMapper : Mapper
    {
        public DominioMapper _dominioMapper = new DominioMapper();

        #region >> ArchivoAdjunto

        public virtual ArchivoAdjunto Map(T_ARCHIVO_ADJUNTO entity)
        {
            if (entity == null) return null;

            return new ArchivoAdjunto
            {
                NU_ARCHIVO_ADJUNTO = entity.NU_ARCHIVO_ADJUNTO,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_MANEJO = entity.CD_MANEJO,
                DS_REFERENCIA = entity.DS_REFERENCIA,
                DS_REFERENCIA2 = entity.DS_REFERENCIA2,
                TipoDocumento = entity.TP_DOCUMENTO,
                Observacion = entity.DS_OBSERVACION,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Anexo4 = entity.DS_ANEXO4,
                Anexo5 = entity.DS_ANEXO5,
                Anexo6 = entity.DS_ANEXO6,
                CD_SITUACAO = entity.CD_SITUACAO,
                NU_VERSION_ACTIVA = entity.NU_VERSION_ACTIVA,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UPDROW = entity.DT_UPDROW,

            };
        }

        public virtual T_ARCHIVO_ADJUNTO Map(ArchivoAdjunto obj)
        {
            if (obj == null) return null;

            return new T_ARCHIVO_ADJUNTO
            {
                NU_ARCHIVO_ADJUNTO = obj.NU_ARCHIVO_ADJUNTO,
                CD_EMPRESA = obj.CD_EMPRESA,
                CD_MANEJO = obj.CD_MANEJO,
                DS_REFERENCIA = obj.DS_REFERENCIA,
                DS_REFERENCIA2 = obj.DS_REFERENCIA2,
                TP_DOCUMENTO = obj.TipoDocumento,
                DS_OBSERVACION = obj.Observacion,
                DS_ANEXO1 = obj.Anexo1,
                DS_ANEXO2 = obj.Anexo2,
                DS_ANEXO3 = obj.Anexo3,
                DS_ANEXO4 = obj.Anexo4,
                DS_ANEXO5 = obj.Anexo5,
                DS_ANEXO6 = obj.Anexo6,
                CD_SITUACAO = obj.CD_SITUACAO,
                NU_VERSION_ACTIVA = obj.NU_VERSION_ACTIVA,
                DT_ADDROW = obj.DT_ADDROW,
                DT_UPDROW = obj.DT_UPDROW,

            };
        }


        #endregion << ArchivoAdjunto

        #region >> ArchivoVersion

        public virtual ArchivoVersion Map(T_ARCHIVO_ADJUNTO_VERSION entity)
        {
            if (entity == null) return null;

            return new ArchivoVersion
            {
                LK_RUTA = entity.LK_RUTA,
                TP_ARCHIVO = entity.TP_ARCHIVO,
                NU_VERSION = entity.NU_VERSION,
                DT_ADDROW = entity.DT_ADDROW,
                CD_FUNCIONARIO = entity.CD_FUNCIONARIO,
            };
        }

        public virtual T_ARCHIVO_ADJUNTO_VERSION Map(ArchivoAdjunto archivo, ArchivoVersion version)
        {
            if (archivo == null || version == null) return null;

            return new T_ARCHIVO_ADJUNTO_VERSION
            {
                NU_ARCHIVO_ADJUNTO = archivo.NU_ARCHIVO_ADJUNTO,
                CD_EMPRESA = archivo.CD_EMPRESA,
                CD_MANEJO = archivo.CD_MANEJO,
                DS_REFERENCIA = archivo.DS_REFERENCIA,
                LK_RUTA = version.LK_RUTA,
                TP_ARCHIVO = version.TP_ARCHIVO,
                NU_VERSION = version.NU_VERSION,
                DT_ADDROW = version.DT_ADDROW,
                CD_FUNCIONARIO = version.CD_FUNCIONARIO,

            };
        }


        #endregion << ArchivoVersion

        #region >> TipoDocumento

        public virtual ArchivoDocumento Map(T_ARCHIVO_DOCUMENTO entity)
        {
            if (entity == null) return null;

            return new ArchivoDocumento
            {
                Codigo = entity.CD_DOCUMENTO,
                Descripcion = entity.DS_DOCUMENTO,
            };
        }


        #endregion << TipoDocumento

        #region >> TipoManejo

        public virtual ArchivoManejo Map(T_ARCHIVO_MANEJO entity)
        {
            if (entity == null) return null;

            List<ArchivoDocumento> lineas = new List<ArchivoDocumento>();

            if (entity.T_ARCHIVO_MANEJO_DOCUMENTO != null)
            {
                foreach (var item in entity.T_ARCHIVO_MANEJO_DOCUMENTO)
                {
                    lineas.Add(this.Map(item.T_ARCHIVO_DOCUMENTO));
                }
            }

            return new ArchivoManejo
            {
                Codigo = entity.CD_MANEJO,
                Descripcion = entity.DS_MANEJO,
                Ruta = entity.SUB_LINK,
                CodigosAnexos = entity.CD_ANEXOS?.Split('-')?.ToList() ?? new List<string>(),
                DescripcionAnexos = entity.DS_ANEXOS?.Split('-')?.ToList() ?? new List<string>(),
                CodigosCampos = entity.CD_CAMPOS?.Split('-')?.ToList() ?? new List<string>(),
                DescripcionCampos = entity.DS_CAMPOS?.Split('-')?.ToList() ?? new List<string>(),
                TiposDocumentos = lineas,
            };
        }

        #endregion << TipoManejo
    }
}
