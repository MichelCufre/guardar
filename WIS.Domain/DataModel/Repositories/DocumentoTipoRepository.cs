using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class DocumentoTipoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected DocumentoTipoMapper mapper;

        public DocumentoTipoRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this.mapper = new DocumentoTipoMapper();
        }

        public virtual List<DocumentoTipo> GetDocumentosIngresoManualHabilitados()
        {
            List<DocumentoTipo> tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO
                .Where(t => t.TP_OPERACION == TipoDocumentoOperacion.INGRESO 
                    && t.FL_HABILITADO == "S" && t.FL_INGRESO_MANUAL == "S")
                .OrderBy(t => t.DS_TP_DOCUMENTO);

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual List<DocumentoTipo> GetDocumentosEgresoHabilitados()
        {
            List<DocumentoTipo> tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO
                .Where(t => t.TP_OPERACION == TipoDocumentoOperacion.EGRESO 
                    && t.FL_HABILITADO == "S" && t.FL_INGRESO_MANUAL == "S")
                .OrderBy(t => t.DS_TP_DOCUMENTO);

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual List<DocumentoTipo> GetDocumentosActaModificacionHabilitados()
        {
            List<DocumentoTipo> tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO
                .Where(t => t.TP_OPERACION == TipoDocumentoOperacion.MODIFICACION 
                    && t.FL_HABILITADO == "S")
                .OrderBy(t => t.DS_TP_DOCUMENTO);

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual Dictionary<string, HashSet<string>> GetDocumentosHabilitadosParaEdicion()
        {
            var tiposDocumento = new Dictionary<string, HashSet<string>>();

            var tipos = this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .Where(t => t.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && t.T_DOCUMENTO_TIPO.FL_PERMITE_EDICION == "S"
                    && t.FL_PERMITE_EDICION == "S")
                .AsNoTracking();

            foreach (var tipo in tipos)
            {
                if (!tiposDocumento.ContainsKey(tipo.TP_DOCUMENTO))
                    tiposDocumento.Add(tipo.TP_DOCUMENTO, new HashSet<string>());

                if (!tiposDocumento[tipo.TP_DOCUMENTO].Contains(tipo.ID_ESTADO))
                    tiposDocumento[tipo.TP_DOCUMENTO].Add(tipo.ID_ESTADO);
            }

            return tiposDocumento;
        }

        public virtual Dictionary<string, HashSet<string>> GetDocumentosHabilitadosParaSimularCC()
        {
            var tiposDocumento = new Dictionary<string, HashSet<string>>();

            var tipos = this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .Where(t => t.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && t.FL_PERMITE_SIMULAR_CC == "S")
                .AsNoTracking();

            foreach (var tipo in tipos)
            {
                if (!tiposDocumento.ContainsKey(tipo.TP_DOCUMENTO))
                    tiposDocumento.Add(tipo.TP_DOCUMENTO, new HashSet<string>());

                if (!tiposDocumento[tipo.TP_DOCUMENTO].Contains(tipo.ID_ESTADO))
                    tiposDocumento[tipo.TP_DOCUMENTO].Add(tipo.ID_ESTADO);
            }

            return tiposDocumento;
        }

        public virtual Dictionary<string, HashSet<string>> GetDocumentosHabilitadosParaBalanceo()
        {
            var tiposDocumento = new Dictionary<string, HashSet<string>>();

            var tipos = this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .Where(t => t.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && t.FL_PERMITE_BALANCEO == "S")
                .AsNoTracking();

            foreach (var tipo in tipos)
            {
                if (!tiposDocumento.ContainsKey(tipo.TP_DOCUMENTO))
                    tiposDocumento.Add(tipo.TP_DOCUMENTO, new HashSet<string>());

                if (!tiposDocumento[tipo.TP_DOCUMENTO].Contains(tipo.ID_ESTADO))
                    tiposDocumento[tipo.TP_DOCUMENTO].Add(tipo.ID_ESTADO);
            }

            return tiposDocumento;
        }

        public virtual bool RequiereValidacion(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(t => t.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && t.TP_DOCUMENTO == tipoDocumento
                    && t.ID_ESTADO == estado
                    && t.FL_REQUIERE_VALIDACION == "S");
        }

        public virtual bool RequiereAgenda(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(t => t.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && t.TP_DOCUMENTO == tipoDocumento
                    && t.ID_ESTADO == estado
                    && t.FL_REQUIERE_AGENDA == "S");
        }

        public virtual bool IsAutoAgendable(string tipoDocumento)
        {
            return this._context.T_DOCUMENTO_TIPO
                .AsNoTracking()
                .Any(t => t.TP_DOCUMENTO == tipoDocumento
                    && t.FL_HABILITADO == "S"
                    && t.FL_AUTOAGENDABLE == "S");
        }

        public virtual bool RequiereLineas(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(t => t.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && t.TP_DOCUMENTO == tipoDocumento
                    && t.ID_ESTADO == estado
                    && t.FL_REQUIERE_LINEAS == "S");
        }

        public virtual bool RequiereDUA(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(e => e.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && e.T_DOCUMENTO_TIPO.FL_REQUIERE_DUA == "S"
                    && e.TP_DOCUMENTO == tipoDocumento
                    && e.ID_ESTADO == estado
                    && e.FL_REQUIERE_DUA == "S");
        }

        public virtual bool RequiereDTI(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(e => e.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && e.T_DOCUMENTO_TIPO.FL_REQUIERE_DTI == "S"
                    && e.TP_DOCUMENTO == tipoDocumento
                    && e.ID_ESTADO == estado
                    && e.FL_REQUIERE_DTI == "S");
        }

        public virtual bool RequiereReferenciaExterna(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(e => e.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && e.T_DOCUMENTO_TIPO.FL_REQUIERE_REFERENCIA_EXTERNA == "S"
                    && e.TP_DOCUMENTO == tipoDocumento
                    && e.ID_ESTADO == estado
                    && e.FL_REQUIERE_REFERENCIA_EXTERNA == "S");
        }

        public virtual bool RequiereFactura(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(e => e.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && e.T_DOCUMENTO_TIPO.FL_REQUIERE_FACTURA == "S"
                    && e.TP_DOCUMENTO == tipoDocumento
                    && e.ID_ESTADO == estado
                    && e.FL_REQUIERE_FACTURA == "S");
        }

        public virtual bool PermiteEditarCamion(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(e => e.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && e.T_DOCUMENTO_TIPO.FL_PERMITE_EDITAR_CAMION == "S"
                    && e.TP_DOCUMENTO == tipoDocumento
                    && e.ID_ESTADO == estado
                    && e.FL_PERMITE_EDITAR_CAMION == "S");
        }

        public virtual bool PermiteEdicion(string tipoDocumento, string estado)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Include("T_DOCUMENTO_TIPO")
                .AsNoTracking()
                .Any(e => e.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && e.T_DOCUMENTO_TIPO.FL_PERMITE_EDICION == "S"
                    && e.TP_DOCUMENTO == tipoDocumento
                    && e.ID_ESTADO == estado
                    && e.FL_PERMITE_EDICION == "S");
        }

        public virtual string GetEstadoInicial(string tipoDocumento)
        {
            return this._context.T_DOCUMENTO_TIPO_ESTADO
                .Where(e => e.TP_DOCUMENTO == tipoDocumento && e.FL_INICIAL == "S")
                .Select(e => e.ID_ESTADO)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public virtual List<DocumentoTipo> GetDocumentosIngresoHabilitados()
        {
            List<DocumentoTipo> tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO
                .Where(t => t.TP_OPERACION == TipoDocumentoOperacion.INGRESO && t.FL_HABILITADO == "S");

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual bool DocumentoNumeracionAutogenerada(string tipoDocumento)
        {
            return this._context.T_DOCUMENTO_TIPO
                .AsNoTracking()
                .Any(t => t.TP_DOCUMENTO == tipoDocumento && t.FL_NUMERO_AUTOGENERADO == "S");
        }

        public virtual List<string> GetDetallesEditables(string tipoDocumento)
        {
            return this._context.T_DOCUMENTO_TIPO_EDITABLE_DET
                .AsNoTracking()
                .Where(t => t.TP_DOCUMENTO == tipoDocumento)
                .Select(t => t.NM_DATAFIELD)
                .ToList();
        }

        public virtual List<string> GetTiposDocumentoIngresoModificacion()
        {
            return this._context.T_DOCUMENTO_TIPO
                .Where(t => t.TP_OPERACION == "I" || t.TP_OPERACION == "M")
                .Select(d => d.TP_DOCUMENTO)
                .ToList();
        }

        public virtual List<DocumentoTipo> GetDocumentosHabilitados()
        {
            List<DocumentoTipo> tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO.Where(t => t.FL_HABILITADO == "S");

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual DocumentoTipo GetTipoDocumento(string tipoDocumento)
        {
            var tipo = this._context.T_DOCUMENTO_TIPO.FirstOrDefault(t => t.TP_DOCUMENTO == tipoDocumento);

            return this.mapper.MapToDocumentoTipo(tipo);
        }

        public virtual List<DocumentoTipo> GetTiposDocumentoIngresoPermitenTraspaso()
        {
            var tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO
                .Where(x => x.FL_PERMITE_TRASPASO == "S" && x.TP_OPERACION == TipoDocumentoOperacion.INGRESO && x.FL_HABILITADO == "S");

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual List<DocumentoTipo> GetTiposDocumentoEgresoPermitenTraspaso()
        {
            var tiposDocumento = new List<DocumentoTipo>();

            var tipos = this._context.T_DOCUMENTO_TIPO
                .Where(x => x.FL_PERMITE_TRASPASO == "S" && x.TP_OPERACION == TipoDocumentoOperacion.EGRESO && x.FL_HABILITADO == "S");

            foreach (var tipo in tipos)
            {
                tiposDocumento.Add(this.mapper.MapToDocumentoTipo(tipo));
            }

            return tiposDocumento;
        }

        public virtual bool AnyTipoDocumento(string tpDocumento)
        {
            return this._context.T_DOCUMENTO_TIPO.Any(x => x.TP_DOCUMENTO == tpDocumento);
        }
    }
}
