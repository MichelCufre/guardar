using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WIS.Documento.Execution;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Serializables;
using WIS.Domain.Documento.Serializables.Salida;
using WIS.Domain.Extensions;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class DocumentoRepository
    {
        public readonly WISDB _context;
        public string _application;
        public int _userId;
        public readonly DocumentoMapper _mapper;
        public readonly IFactoryService _service;
        public readonly IDapper _dapper;

        public DocumentoRepository(WISDB context,
            string application,
            int userId,
            IFactoryService service,
            IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._service = service;
            this._dapper = dapper;
            this._mapper = new DocumentoMapper();
        }

        public virtual IDocumento GetDocumento(string nroDocumento, string tipoDocumento)
        {
            var tpDocumento = this._context.T_DOCUMENTO_TIPO
                .AsNoTracking()
                .Where(e => e.TP_DOCUMENTO == tipoDocumento)
                .FirstOrDefault();

            switch (tpDocumento.TP_OPERACION)
            {
                case TipoDocumentoOperacion.INGRESO:
                    return this.GetIngreso(nroDocumento, tipoDocumento);

                case TipoDocumentoOperacion.EGRESO:
                    return this.GetEgreso(nroDocumento, tipoDocumento);

                case TipoDocumentoOperacion.MODIFICACION:
                    return this.GetActa(nroDocumento, tipoDocumento);
            }

            return null;
        }

        #region CAMBIO LOTE

        public virtual List<DocumentoCambioLote> GetDocumentosCambioLote(string producto, int empresa, string lote, decimal cantidad)
        {
            var documentos = new Dictionary<string, DocumentoCambioLote>();
            var detalles = this._context.T_DET_DOCUMENTO
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO_ESTADO")
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == 1
                    && d.NU_IDENTIFICADOR.Trim().ToUpper() == lote
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_DISPONIBILIZA_STOCK == "S"
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO_ESTADO.FL_DISPONIBILIZA_STOCK == "S"
                    && ((d.QT_INGRESADA ?? 0) - (d.QT_RESERVADA ?? 0) - (d.QT_DESAFECTADA ?? 0)) > 0)
                .OrderByDescending(d => ((d.QT_INGRESADA ?? 0) - (d.QT_RESERVADA ?? 0) - (d.QT_DESAFECTADA ?? 0)));

            decimal saldoTotal = 0;

            foreach (var detalle in detalles)
            {
                var key = $"{detalle.TP_DOCUMENTO}.{detalle.NU_DOCUMENTO}";
                var documento = new DocumentoCambioLote(detalle.TP_DOCUMENTO, detalle.NU_DOCUMENTO);

                if (documentos.ContainsKey(key))
                    documento = documentos[key];
                else
                    documentos[key] = documento;

                var linea = this._mapper.MapToDocumentoLinea(detalle);

                documento.Lineas.Add(linea);

                saldoTotal += ((detalle.QT_INGRESADA ?? 0) - (detalle.QT_RESERVADA ?? 0) - (detalle.QT_DESAFECTADA ?? 0));

                if (saldoTotal >= cantidad)
                    break;
            }

            if (saldoTotal < cantidad)
                throw new ValidationFailedException("General_Sec0_Error_CambioLoteDocumentoLoteSinSaldo");

            return documentos.Values.OrderByDescending(d => d.Lineas.Sum(l => l.GetCantidadDisponible())).ToList();
        }

        #endregion

        #region DOCUMENTO_INGRESO

        public virtual IDocumentoIngreso GetIngreso(string nroDocumento, string tipoDocumento)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    d => d.CD_EMPRESA.Value,
                    ef => ef.CD_EMPRESA,
                    (d, ef) => d)
                .Where(d => d.NU_DOCUMENTO == nroDocumento && d.TP_DOCUMENTO == tipoDocumento)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToIngreso(documentoEntity, this._service);
        }

        public virtual IDocumentoIngreso GetIngresoPorAgenda(int numeroAgenda)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_AGENDA == numeroAgenda);

            return documentoEntity == null ? null : this._mapper.MapToIngreso(documentoEntity, this._service);
        }

        public virtual void UpdateIngreso(IDocumentoIngreso documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromIngreso(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateIngresoAndDetails(IDocumentoIngreso documento, List<DocumentoLinea> nuevaslineas, long nuTransaccion)
        {
            var documentoEntity = _mapper.CreateEntityDocumento(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local.FirstOrDefault(x => x.NU_DOCUMENTO == documento.Numero && x.TP_DOCUMENTO == documento.Tipo);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry<T_DOCUMENTO>(documentoEntity).State = EntityState.Modified;
            }

            foreach (var lineaIngreso in documento.Lineas)
            {
                UpdateLineaDocumento(lineaIngreso, documento, nuTransaccion);
            }

            foreach (var nuevaLineaIngreso in nuevaslineas)
            {
                AddLineaDocumento(documento, nuevaLineaIngreso, nuTransaccion);
            }
        }

        public virtual List<CuentaCorriente> GetCuentaDocumentoIPNivel(string nuDocEgreso, string tpDocEgreso, int nivel)
        {
            List<T_PRDC_CUENTA_CORRIENTE_INSUMO> listaInsumosSemiacabado = this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO
                .AsNoTracking()
                .Where(x => x.NU_DOCUMENTO_EGRESO == nuDocEgreso
                    && x.TP_DOCUMENTO_EGRESO == tpDocEgreso
                    && x.NU_NIVEL == nivel
                    && x.TP_DOCUMENTO_INGRESO == "IP")
                .ToList();

            List<CuentaCorriente> lista = new List<CuentaCorriente>();
            foreach (var detalle in listaInsumosSemiacabado)
            {
                CuentaCorriente cuenta = this._mapper.MapEntityToObject(detalle);
                lista.Add(cuenta);
            }
            return lista;
        }

        public virtual void UpdateIngresoAndDetails(IDocumentoIngreso documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromIngresoWithDetail(documento);
            var attachedDocumentoEntity = this._context.T_DOCUMENTO.Local
               .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                   && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO);

            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedDocumentoEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedDocumentoEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Modified;
            }

            foreach (T_DET_DOCUMENTO linea in documentoEntity.T_DET_DOCUMENTO)
            {
                var attachedLineaEntity = this._context.T_DET_DOCUMENTO.Local
                    .FirstOrDefault(x => x.CD_PRODUTO == linea.CD_PRODUTO
                        && x.CD_EMPRESA == linea.CD_EMPRESA
                        && x.NU_IDENTIFICADOR == linea.NU_IDENTIFICADOR
                        && x.CD_FAIXA == linea.CD_FAIXA
                        && x.NU_DOCUMENTO == linea.NU_DOCUMENTO
                        && x.TP_DOCUMENTO == linea.TP_DOCUMENTO);

                linea.VL_DATO_AUDITORIA = datoAuditoria;

                if (attachedLineaEntity != null)
                {
                    var attachedEntry = this._context.Entry(attachedLineaEntity);
                    attachedEntry.CurrentValues.SetValues(linea);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    this._context.T_DET_DOCUMENTO.Attach(linea);
                    this._context.Entry(linea).State = EntityState.Modified;
                }
            }
        }

        public virtual List<DocumentoLineaEgreso> GetEgresoDocumento(string tpEgreso, string nroIngreso)
        {
            var documentoLineaEntity = this._context.T_DET_DOCUMENTO_EGRESO
                .Include("T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .AsNoTracking()
                .Where(d => d.TP_DOCUMENTO == tpEgreso && d.NU_DOCUMENTO_INGRESO == nroIngreso)
                .ToList();

            if (documentoLineaEntity.Count() == 0)
            {
                return null;
            }
            else
            {
                return this._mapper.MapToDocumentoLineaEgreso(documentoLineaEntity, this._service);
            }
        }

        public virtual List<PrdcSaldo> GetPrdcSaldo(string v_nu_documento_EP, string v_tp_documento_EP, int v_cd_empresa_EP, string v_cd_producto_EP, decimal v_cd_faixa_EP)
        {
            List<PrdcSaldo> documentoLineas = new List<PrdcSaldo>();
            List<V_PRDC_SALDO_CC> lista = this._context.V_PRDC_SALDO_CC
                .AsNoTracking()
                .Where(x => x.NU_DOCUMENTO == v_nu_documento_EP
                    && x.TP_DOCUMENTO == v_tp_documento_EP
                    && x.CD_EMPRESA == v_cd_empresa_EP
                    && x.CD_PRODUTO == v_cd_producto_EP
                    && x.CD_FAIXA == v_cd_faixa_EP
                    && x.QT_SALDO >= 0)
                .ToList();
            return this._mapper.MapEntityToObject(lista);

        }

        public virtual void GetCambioDocPre(string docNuevo, string tpdocNue, int empresa, string lote, decimal faxia, string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESSO, string cD_PRODUTO, decimal qT_MOVIMIENTO)
        {
            var detailEntity = this._context.T_CAMBIO_DOCUMENTO_DET
                .FirstOrDefault(x => x.NU_DOCUMENTO_CAMBIO == docNuevo
                    && x.TP_DOCUMENTO_CAMBIO == tpdocNue
                    && x.NU_DOCUMENTO == nU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO == tP_DOCUMENTO_INGRESSO
                    && x.CD_PRODUTO == cD_PRODUTO
                    && x.NU_IDENTIFICADOR == lote
                    && x.CD_FAIXA == faxia
                    && x.CD_EMPRESA == empresa);
            var attachedEntity = this._context.T_CAMBIO_DOCUMENTO_DET.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO_CAMBIO == docNuevo
                    && x.TP_DOCUMENTO_CAMBIO == tpdocNue
                    && x.NU_DOCUMENTO == nU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO == tP_DOCUMENTO_INGRESSO
                    && x.CD_PRODUTO == cD_PRODUTO
                    && x.NU_IDENTIFICADOR == lote
                    && x.CD_FAIXA == faxia
                    && x.CD_EMPRESA == empresa);

            detailEntity.QT_CAMBIO = qT_MOVIMIENTO;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_CAMBIO_DOCUMENTO_DET.Attach(detailEntity);
                this._context.Entry(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual List<DatoVistaDoc401> GetDatosVistaDOC401()
        {
            List<DatoVistaDoc401> list1 = new List<DatoVistaDoc401>();
            var list = this._context.V_CAMBIO_DOC_DOC401.AsNoTracking().ToList();

            foreach (var det in list)
            {
                list1.Add(this._mapper.MapEntityToObject(det));
            }

            return list1;
        }

        public virtual DocumentoLineaEgreso GetDocumentoEgresoDet(string nU_DOCUMENTO_EGRESO_PRDC, string tP_DOCUMENTO_EGRESO_PRDC, string cD_PRODUTO, string lote, int empresa, string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESSO)
        {
            var det = this._context.T_DET_DOCUMENTO_EGRESO
                .Include("T_DET_DOCU_EGRESO_RESERV")
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO == nU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO == tP_DOCUMENTO_EGRESO_PRDC
                    && x.CD_PRODUTO == cD_PRODUTO
                    && x.NU_IDENTIFICADOR == lote
                    && x.NU_DOCUMENTO_INGRESO == nU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO == tP_DOCUMENTO_INGRESSO);

            return this._mapper.MapToDocumentoLineaEgreso1(det);
        }

        public virtual void UpdateDetalleDocumentoEgreso(DocumentoLineaEgreso det, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapToDocumentoLineaEgreso1(det);
            var attachedEntity = _context.T_DET_DOCUMENTO_EGRESO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO
                    && x.NU_SECUENCIA == detailEntity.NU_SECUENCIA);

            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DET_DOCUMENTO_EGRESO.Attach(detailEntity);
                this._context.Entry<T_DET_DOCUMENTO_EGRESO>(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual curDocProduccion GetPrdcIngresoSaldo(string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESO)
        {
            V_PRDC_SALDO_INGRESADO doc = this._context.V_PRDC_SALDO_INGRESADO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_ING == nU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_ING == tP_DOCUMENTO_INGRESO);

            return this._mapper.MapEntityToObject(doc);
        }

        public virtual decimal GetQtIngresadaDocOrigen(string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESO, int v_cd_empresa_EP, string v_cd_producto_EP, decimal v_cd_faixa_EP, string v_nu_identificador_EP)
        {
            decimal cantidad = 0;
            T_DET_DOCUMENTO det = this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO == nU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO == tP_DOCUMENTO_INGRESO
                    && x.CD_EMPRESA == v_cd_empresa_EP
                    && x.CD_PRODUTO == v_cd_producto_EP
                    && x.CD_FAIXA == v_cd_faixa_EP
                    && x.NU_IDENTIFICADOR == v_nu_identificador_EP);

            if (det != null)
                cantidad = (det.QT_INGRESADA ?? 0);

            return cantidad;
        }

        public virtual DocumentoLinea GetDetalleDocumento(string cdproduto, decimal cdfaixa, string nuidentificador, int cdEmpresa, string nuDocumento, string tpDocumento)
        {
            T_DET_DOCUMENTO det = this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO == nuDocumento
                    && x.TP_DOCUMENTO == tpDocumento
                    && x.CD_EMPRESA == cdEmpresa
                    && x.CD_PRODUTO == cdproduto
                    && x.CD_FAIXA == cdfaixa
                    && x.NU_IDENTIFICADOR == nuidentificador);

            return _mapper.MapToDocumentoLinea(det);
        }

        public virtual void DeleteCambioDocDet(string cdproduto, decimal cdfaixa, string nuidentificador, int cdEmpresa, string nuDocCambio, string tPDocCambio, string nuDocOrigen, string tpDocOrigen, string NU_DOCUMENTO_NUEVO, string TP_DOCUMENTO_NUEVO)
        {
            var detailEntity = this._context.LT_CAMBIO_DOCUMENTO_DET
                    .FirstOrDefault(x => x.CD_PRODUTO == cdproduto
                        && x.CD_FAIXA == cdfaixa
                        && x.NU_IDENTIFICADOR == nuidentificador
                        && x.CD_EMPRESA == cdEmpresa
                        && x.NU_DOCUMENTO == nuDocOrigen
                        && x.TP_DOCUMENTO == tpDocOrigen
                        && x.NU_DOCUMENTO_CAMBIO == NU_DOCUMENTO_NUEVO
                        && x.TP_DOCUMENTO_CAMBIO == TP_DOCUMENTO_NUEVO);
            var attachedEntity = this._context.LT_CAMBIO_DOCUMENTO_DET.Local
                    .FirstOrDefault(x => x.CD_PRODUTO == cdproduto
                        && x.CD_FAIXA == cdfaixa
                        && x.NU_IDENTIFICADOR == nuidentificador
                        && x.CD_EMPRESA == cdEmpresa
                        && x.NU_DOCUMENTO == nuDocOrigen
                        && x.TP_DOCUMENTO == tpDocOrigen
                        && x.NU_DOCUMENTO_CAMBIO == NU_DOCUMENTO_NUEVO
                        && x.TP_DOCUMENTO_CAMBIO == TP_DOCUMENTO_NUEVO);

            if (attachedEntity != null)
            {
                this._context.LT_CAMBIO_DOCUMENTO_DET.Remove(attachedEntity);
            }
            else
            {
                this._context.LT_CAMBIO_DOCUMENTO_DET.Attach(detailEntity);
                this._context.LT_CAMBIO_DOCUMENTO_DET.Remove(detailEntity);
            }
        }

        public virtual void CancelarCambioDocPre()
        {
            var list = this._context.V_CAMBIO_DOC_DOC401.AsNoTracking().ToList();

            foreach (var det in list)
            {
                var detailEntity = this._context.T_CAMBIO_DOCUMENTO_DET
                    .FirstOrDefault(x => x.NU_DOCUMENTO_CAMBIO == det.NU_DOC
                        && x.TP_DOCUMENTO_CAMBIO == det.TP_DOCUMENTO_CAMBIO
                        && x.NU_DOCUMENTO == det.NU_DOCUMENTO_INGRESO
                        && x.TP_DOCUMENTO == det.TP_DOCUMENTO_INGRESSO
                        && x.CD_PRODUTO == det.CD_PRODUTO
                        && x.NU_IDENTIFICADOR == det.NU_IDENTIFICADOR
                        && x.CD_FAIXA == det.CD_FAIXA
                        && x.CD_EMPRESA == det.CD_EMPRESA);
                var attachedEntity = this._context.T_CAMBIO_DOCUMENTO_DET.Local
                    .FirstOrDefault(x => x.NU_DOCUMENTO_CAMBIO == det.NU_DOC
                        && x.TP_DOCUMENTO_CAMBIO == det.TP_DOCUMENTO_CAMBIO
                        && x.NU_DOCUMENTO == det.NU_DOCUMENTO_INGRESO
                        && x.TP_DOCUMENTO == det.TP_DOCUMENTO_INGRESSO
                        && x.CD_PRODUTO == det.CD_PRODUTO
                        && x.NU_IDENTIFICADOR == det.NU_IDENTIFICADOR
                        && x.CD_FAIXA == det.CD_FAIXA
                        && x.CD_EMPRESA == det.CD_EMPRESA);

                detailEntity.ID_PROCESADO = "D";

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(detailEntity);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    this._context.T_CAMBIO_DOCUMENTO_DET.Attach(detailEntity);
                    this._context.Entry(detailEntity).State = EntityState.Modified;
                }
            }
        }

        public virtual void UpdateProcesarCambioDocPre()
        {
            var list = this._context.V_CAMBIO_DOC_DOC401.AsNoTracking().ToList();

            foreach (var det in list)
            {
                var detailEntity = this._context.T_CAMBIO_DOCUMENTO_DET
                    .FirstOrDefault(x => x.NU_DOCUMENTO_CAMBIO == det.NU_DOC
                        && x.TP_DOCUMENTO_CAMBIO == det.TP_DOCUMENTO_CAMBIO
                        && x.NU_DOCUMENTO == det.NU_DOCUMENTO_INGRESO
                        && x.TP_DOCUMENTO == det.TP_DOCUMENTO_INGRESSO
                        && x.CD_PRODUTO == det.CD_PRODUTO
                        && x.NU_IDENTIFICADOR == det.NU_IDENTIFICADOR
                        && x.CD_FAIXA == det.CD_FAIXA
                        && x.CD_EMPRESA == det.CD_EMPRESA);
                var attachedEntity = this._context.T_CAMBIO_DOCUMENTO_DET.Local
                    .FirstOrDefault(x => x.NU_DOCUMENTO_CAMBIO == det.NU_DOC
                        && x.TP_DOCUMENTO_CAMBIO == det.TP_DOCUMENTO_CAMBIO
                        && x.NU_DOCUMENTO == det.NU_DOCUMENTO_INGRESO
                        && x.TP_DOCUMENTO == det.TP_DOCUMENTO_INGRESSO
                        && x.CD_PRODUTO == det.CD_PRODUTO
                        && x.NU_IDENTIFICADOR == det.NU_IDENTIFICADOR
                        && x.CD_FAIXA == det.CD_FAIXA
                        && x.CD_EMPRESA == det.CD_EMPRESA);

                detailEntity.ID_PROCESADO = "P";

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(detailEntity);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    this._context.T_CAMBIO_DOCUMENTO_DET.Attach(detailEntity);
                    this._context.Entry(detailEntity).State = EntityState.Modified;
                }
            }
        }

        public virtual void RemoveDocumento(IDocumentoIngreso documento, long nuTransaccion)
        {
            var documentoEntity = _mapper.MapFromIngreso(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documento.Numero
                    && x.TP_DOCUMENTO == documento.Tipo);

            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                this._context.T_DOCUMENTO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.T_DOCUMENTO.Remove(documentoEntity);
            }
        }

        public virtual bool GetAnyDetalleDocumento(string nuDocCambio, string tPDocCambio)
        {
            return this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .Any(d => d.NU_DOCUMENTO == nuDocCambio && d.TP_DOCUMENTO == tPDocCambio);
        }

        public virtual decimal GetCantidadActa(string var_nu_doc_orig, string var_tp_doc_orig, int v_cd_empresa_EP, string v_cd_producto_EP, decimal v_cd_faixa_EP, string v_nu_identificador_EP)
        {
            decimal cantidad = 0;
            T_DET_DOCUMENTO det = this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO == var_nu_doc_orig
                    && x.TP_DOCUMENTO == var_tp_doc_orig
                    && x.CD_EMPRESA == v_cd_empresa_EP
                    && x.CD_PRODUTO == v_cd_producto_EP
                    && x.CD_FAIXA == v_cd_faixa_EP
                    && x.NU_IDENTIFICADOR == v_nu_identificador_EP);

            if (det != null)
                cantidad = (det.QT_INGRESADA ?? 0);

            return cantidad;
        }

        public virtual decimal GetQtIngresadaDocOrigen(string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESO)
        {
            decimal cantidad = 0;
            var detalle = this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .Where(x => x.NU_DOCUMENTO == nU_DOCUMENTO_INGRESO && x.TP_DOCUMENTO == tP_DOCUMENTO_INGRESO)
                .GroupBy(x => new { x.NU_DOCUMENTO, x.TP_DOCUMENTO })
                .Select(x => new { resultado = x.Sum(d => d.QT_INGRESADA) });

            if (detalle != null)
            {
                cantidad = (detalle.FirstOrDefault().resultado ?? 0);
            }

            return cantidad;
        }

        public virtual bool TieneSaldoDocumental(CambioDocInt request)
        {
            bool rest = true;
            var det = this._context.V_CAMBIO_DOC_DOC400
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO
                    && x.TP_DOCUMENTO_INGRESSO == request.TP_DOCUMENTO
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR);

            if (det != null)
            {
                if (request.QT_CAMBIO > det.QT_SALDO)
                {
                    rest = false;
                }
            }
            else
            {
                rest = false;
            }

            return rest;
        }

        public virtual void AddPreCambioDoc(CambioDocInt request)
        {
            T_CAMBIO_DOCUMENTO_DET newreg = this._mapper.MapObjectToEntity(request, GetNumeroSecuenciaPreCambio());
            this._context.T_CAMBIO_DOCUMENTO_DET.Add(newreg);
        }

        public virtual void AddCuentaCorrienteInsumo(CuentaCorriente new_insumo)
        {
            T_PRDC_CUENTA_CORRIENTE_INSUMO newreg = this._mapper.MapObjectToEntity(new_insumo);
            this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO.Add(newreg);
        }

        public virtual void AddCuentaCorrienteInsumo(CuentaCorrienteCambioDoc new_insumo)
        {
            T_PRDC_CUENTA_CAMBIO_DOC newreg = this._mapper.MapObjectToEntity(new_insumo);
            newreg.DT_ADDROW = DateTime.Now;
            this._context.T_PRDC_CUENTA_CAMBIO_DOC.Add(newreg);
        }

        public virtual decimal GetDocOriQTIngreso(string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESO, int v_cd_empresa_EP, string v_cd_producto_EP, decimal v_cd_faixa_EP, string v_nu_identificador_EP)
        {
            decimal cantidad = 0;

            var detalle = this._context.V_ACTA_DET_DOCUMENTO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO == nU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO == tP_DOCUMENTO_INGRESO
                    && x.CD_EMPRESA == v_cd_empresa_EP
                    && x.CD_PRODUTO == v_cd_producto_EP
                    && x.CD_FAIXA == v_cd_faixa_EP
                    && x.NU_IDENTIFICADOR == v_nu_identificador_EP);

            if (detalle != null)
            {
                cantidad = (detalle.QT_INGRESADA ?? 0);
            }

            return cantidad;
        }

        public virtual void GetDocumentoActa(string nU_DOCUMENTO_INGRESO, string tP_DOCUMENTO_INGRESO, out string v_tp_documento_ingreso_orig, out string v_nu_documento_ingreso_orig)
        {
            T_DET_DOCUMENTO_ACTA acta = this._context.T_DET_DOCUMENTO_ACTA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ACTA == nU_DOCUMENTO_INGRESO && x.TP_ACTA == tP_DOCUMENTO_INGRESO);

            v_tp_documento_ingreso_orig = acta.TP_DOCUMENTO;
            v_nu_documento_ingreso_orig = acta.NU_DOCUMENTO;
        }

        public virtual void AddIngreso(IDocumentoIngreso documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromIngresoWithDetail(documento);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;
            documentoEntity.T_DET_DOCUMENTO.Select(d => { d.VL_DATO_AUDITORIA = datoAuditoria; return d; }).ToList();

            this._context.T_DOCUMENTO.Add(documentoEntity);
        }

        public virtual void AddLogCambioIngreso(CambioDocDet documento)
        {
            var documentoEntity = this._mapper.MapFromObjEnt(documento);

            this._context.LT_CAMBIO_DOCUMENTO_DET.Add(documentoEntity);
        }

        public virtual DocumentoLinea GetLineaIngreso(string nroDocumento, string tipoDocumento, string producto, string identificador)
        {
            var documentoLineaEntity = this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .Where(d => d.TP_DOCUMENTO == tipoDocumento
                    && d.NU_DOCUMENTO == nroDocumento
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador)
                .FirstOrDefault();

            return (documentoLineaEntity == null) ? null : this._mapper.MapToDocumentoLinea(documentoLineaEntity);
        }

        public virtual List<DocumentoLinea> GetLineasIngreso(string nroDocumento, string tipoDocumento)
        {
            List<DocumentoLinea> documentoLineas = new List<DocumentoLinea>();

            var documentoLineasEntity = this._context.T_DET_DOCUMENTO
                .Where(d => d.TP_DOCUMENTO == tipoDocumento && d.NU_DOCUMENTO == nroDocumento)
                .AsNoTracking()
                .ToList();

            foreach (var detEntity in documentoLineasEntity)
                documentoLineas.Add(this._mapper.MapToDocumentoLinea(detEntity));

            return documentoLineas;
        }

        public virtual bool ExisteLogCambioDoc(CambioDocumentoDetIngreso request)
        {
            return _context.LT_CAMBIO_DOCUMENTO_DET.Any(x =>
                x.CD_PRODUTO == request.CD_PRODUTO &&
                x.CD_FAIXA == request.CD_FAIXA &&
                x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR &&
                x.CD_EMPRESA == request.CD_EMPRESA &&
                x.NU_DOCUMENTO == request.NU_DOCUMENTO_INGRESO &&
                x.TP_DOCUMENTO == request.TP_DOCUMENTO_INGRESO &&
                x.NU_DOCUMENTO_CAMBIO == request.NU_DOCUMENTO_NUEVO &&
                x.TP_DOCUMENTO_CAMBIO == request.TP_DOCUMENTO_NUEVO);
        }

        public virtual List<TipoDocumentoLiberable> GetTiposDocumentosLiberables()
        {
            return this._context.T_DOCUMENTO_TIPO
                .Where(t => t.FL_HABILITADO == "S"
                    && t.FL_DISPONIBILIZA_STOCK == "S")
                .AsNoTracking()
                .Select(t => new TipoDocumentoLiberable { Tipo = t.TP_DOCUMENTO, Descripcion = t.DS_TP_DOCUMENTO })
                .OrderBy(t => t.Descripcion)
                .ToList();
        }

        public virtual List<DocumentoLiberable> GetDocumentosLiberables(int empresa, IEnumerable<string> tiposDocumentos = null)
        {
            return this._context.T_DET_DOCUMENTO
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Where(d => ((d.QT_INGRESADA ?? 0) - (d.QT_RESERVADA ?? 0) - (d.QT_DESAFECTADA ?? 0)) > 0
                    && d.CD_EMPRESA == empresa
                    && (tiposDocumentos == null || tiposDocumentos.Contains(d.TP_DOCUMENTO))
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_DISPONIBILIZA_STOCK == "S")
                .Join(this._context.T_DOCUMENTO_TIPO_ESTADO,
                    d => new { d.TP_DOCUMENTO, d.T_DOCUMENTO.ID_ESTADO },
                    e => new { e.TP_DOCUMENTO, e.ID_ESTADO },
                    (d, e) => new { Detalle = d, Estado = e })
                .Where(de => de.Estado.FL_DISPONIBILIZA_STOCK == "S")
                .Select(de => de.Detalle)
                .GroupBy(d => new { d.TP_DOCUMENTO, d.NU_DOCUMENTO, d.T_DOCUMENTO.T_DOCUMENTO_TIPO.DS_TP_DOCUMENTO })
                .Where(g => g.Count() > 0)
                .AsNoTracking()
                .Select(g => new DocumentoLiberable { Tipo = g.Key.TP_DOCUMENTO, Numero = g.Key.NU_DOCUMENTO, Descripcion = g.Key.DS_TP_DOCUMENTO + " - " + g.Key.NU_DOCUMENTO })
                .OrderBy(d => d.Descripcion)
                .ToList();
        }

        public virtual void UpdateLineaDocumento(DocumentoLinea linea, IDocumentoIngreso documento, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea);
            var attachedEntity = this._context.T_DET_DOCUMENTO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.CD_PRODUTO == detailEntity.CD_PRODUTO
                    && x.NU_IDENTIFICADOR == detailEntity.NU_IDENTIFICADOR
                    && x.CD_FAIXA == detailEntity.CD_FAIXA
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO);

            detailEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DET_DOCUMENTO.Attach(detailEntity);
                this._context.Entry<T_DET_DOCUMENTO>(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual void AddLineaDocumento(IDocumentoIngreso documento, DocumentoLinea linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea);

            detailEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            this._context.T_DET_DOCUMENTO.Add(detailEntity);
        }

        public virtual void DesvincularAgenda(Agenda agenda, long nuTransaccion)
        {
            var documento = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO_PREPARACION_RESERV")
                .FirstOrDefault(d => d.NU_AGENDA == agenda.Id);

            if (documento != null)
            {
                EliminarReservas(documento, nuTransaccion);
                RetrocederDocumento(documento, nuTransaccion);
                documento.NU_AGENDA = null;
            }

            agenda.NumeroDocumento = null;
        }

        public virtual void EliminarReservas(T_DOCUMENTO documento, long nuTransaccion)
        {
            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            foreach (var reserva in documento.T_DOCUMENTO_PREPARACION_RESERV)
            {
                reserva.VL_DATO_AUDITORIA = datoAuditoria;
                reserva.DT_UPDROW = DateTime.Now;
                reserva.NU_TRANSACCION_DELETE = nuTransaccion;
            }

            documento.T_DOCUMENTO_PREPARACION_RESERV.Clear();

            foreach (var det in documento.T_DET_DOCUMENTO)
            {
                det.QT_RESERVADA = 0;
                det.VL_DATO_AUDITORIA = datoAuditoria;
            }
        }

        public virtual void RetrocederDocumento(T_DOCUMENTO documento, long nuTransaccion)
        {
            var estadoRetroceso = "";

            var estados = this._context.T_DOCUMENTO_TIPO_ESTADO
                    .AsNoTracking()
                    .Where(e => e.TP_DOCUMENTO == documento.TP_DOCUMENTO);

            var estadoInicial = estados
                .FirstOrDefault(e => e.FL_INICIAL == "S");

            var estadoRequiereAgenda = estados
                .FirstOrDefault(e => e.FL_REQUIERE_AGENDA == "S");

            if (estadoInicial != null)
            {
                if (estadoRequiereAgenda == null)
                {
                    estadoRetroceso = estadoInicial.ID_ESTADO;
                }
                else
                {
                    var estadosVisitados = new HashSet<string>();
                    var transiciones = this._context.T_DOCUMENTO_ESTADO_ORDEN
                        .AsNoTracking()
                        .Where(e => e.TP_DOCUMENTO == documento.TP_DOCUMENTO);

                    var transicionInicial = transiciones
                        .FirstOrDefault(t => t.ID_ESTADO_ORIGEN == estadoInicial.ID_ESTADO);

                    if (transicionInicial != null)
                    {
                        var transicionesCount = transiciones.Count();
                        var transicionActual = transicionInicial;
                        var estadoAnterior = "";

                        for (int i = 0; i < transicionesCount && transicionActual != null; i++)
                        {
                            var estadoActual = transicionActual.ID_ESTADO_ORIGEN;

                            if (estadoActual == estadoRequiereAgenda.ID_ESTADO
                                && !estadosVisitados.Contains(documento.ID_ESTADO))
                            {
                                estadoRetroceso = string.IsNullOrEmpty(estadoAnterior) ? estadoInicial.ID_ESTADO : estadoAnterior;
                                break;
                            }

                            estadosVisitados.Add(estadoActual);

                            transicionActual = transiciones
                                .FirstOrDefault(t => t.ID_ESTADO_ORIGEN == transicionActual.ID_ESTADO_DESTINO
                                    && !estadosVisitados.Contains(t.ID_ESTADO_DESTINO));

                            estadoAnterior = estadoActual;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(estadoRetroceso))
            {
                documento.ID_ESTADO = estadoRetroceso;
                documento.DT_UPDROW = DateTime.Now;
                documento.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            }
        }

        public virtual bool AnyEstadoDocumentoHabilitaStock(string documentoIngreso, string tipoDocumento)
        {
            return _context.T_DOCUMENTO.Join(_context.T_DOCUMENTO_TIPO_ESTADO,
                d => new { d.TP_DOCUMENTO, d.ID_ESTADO },
                dte => new { dte.TP_DOCUMENTO, dte.ID_ESTADO },
                (d, dte) => new { Documento = d, DocumentoTipoEstado = dte })
            .Any(x => x.Documento.NU_DOCUMENTO == documentoIngreso && x.Documento.TP_DOCUMENTO == tipoDocumento
                && x.DocumentoTipoEstado.FL_DISPONIBILIZA_STOCK == "S"
            );

        }

        #endregion

        #region DOCUMENTO_EGRESO

        public virtual void AddEgreso(IDocumentoEgreso documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromEgreso(documento);
            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (documentoEntity.T_DET_DOCUMENTO_EGRESO.Count > 0)
                documentoEntity.T_DET_DOCUMENTO_EGRESO.Select(d => { d.VL_DATO_AUDITORIA = datoAuditoria; d.NU_SECUENCIA = GetNumeroSecuenciaDetalleEgreso(); return d; }).ToList();

            this._context.T_DOCUMENTO.Add(documentoEntity);
        }

        public virtual IDocumentoEgreso GetEgreso(string nroDocumento, string tipoDocumento)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Include("T_DET_DOCUMENTO_EGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    d => d.CD_EMPRESA.Value,
                    ef => ef.CD_EMPRESA,
                    (d, ef) => d)
                .Where(d => d.NU_DOCUMENTO == nroDocumento && d.TP_DOCUMENTO == tipoDocumento)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToEgreso(documentoEntity, this._service);
        }

        public virtual CuentaCorriente GetCuentaCorriente(CuentaCorriente request)
        {
            T_PRDC_CUENTA_CORRIENTE_INSUMO insumo = this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == request.NU_DOCUMENTO_EGRESO
                    && x.TP_DOCUMENTO_EGRESO == request.TP_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == request.CD_EMPRESA
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.CD_FAIXA == request.CD_FAIXA
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == request.NU_NIVEL);

            if (insumo == null)
            {
                T_PRDC_CUENTA_CORRIENTE_INSUMO insumo1 = this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO.Local
                    .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == request.NU_DOCUMENTO_EGRESO
                        && x.TP_DOCUMENTO_EGRESO == request.TP_DOCUMENTO_EGRESO
                        && x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                        && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                        && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                        && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                        && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                        && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                        && x.CD_EMPRESA == request.CD_EMPRESA
                        && x.CD_PRODUTO == request.CD_PRODUTO
                        && x.CD_FAIXA == request.CD_FAIXA
                        && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                        && x.NU_NIVEL == request.NU_NIVEL);

                if (insumo == null)
                {
                    return null;
                }
                else
                {
                    return this._mapper.MapEntityToObject(insumo);
                }
            }
            else
            {
                return this._mapper.MapEntityToObject(insumo);
            }
        }

        public virtual CuentaCorrienteCambioDoc GetCuentaCorriente(CuentaCorrienteCambioDoc request)
        {
            T_PRDC_CUENTA_CAMBIO_DOC insumo = this._context.T_PRDC_CUENTA_CAMBIO_DOC
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == request.NU_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == request.CD_EMPRESA
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.CD_FAIXA == request.CD_FAIXA
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == request.NU_NIVEL);

            if (insumo == null)
            {
                return null;
            }
            else
            {
                return this._mapper.MapEntityToObject(insumo);
            }
        }

        public virtual CuentaCorrienteCambioDoc GetCuentaCorrienteSinNumEgreso(CuentaCorrienteCambioDoc request)
        {
            T_PRDC_CUENTA_CAMBIO_DOC insumo = this._context.T_PRDC_CUENTA_CAMBIO_DOC
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == request.CD_EMPRESA
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.CD_FAIXA == request.CD_FAIXA
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == request.NU_NIVEL);

            if (insumo == null)
            {
                return null;
            }
            else
            {
                return this._mapper.MapEntityToObject(insumo);
            }
        }

        public virtual CuentaCorriente GetCuentaCorrienteDocumento(CambioDocumentoDetIngreso request)
        {
            T_PRDC_CUENTA_CORRIENTE_INSUMO insumo = this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == request.NU_DOCUMENTO_EGRESO
                    && x.TP_DOCUMENTO_EGRESO == request.TP_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == request.CD_EMPRESA
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.CD_FAIXA == request.CD_FAIXA
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == request.NU_NIVEL);

            return this._mapper.MapEntityToObject(insumo);
        }

        public virtual bool GetAnyCuentaCorrienteDocumento(string nroDocumento, string tipoDocumento)
        {
            return this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO
                .AsNoTracking()
                .Any(x => x.NU_DOCUMENTO_EGRESO == nroDocumento
                    && x.TP_DOCUMENTO_EGRESO == tipoDocumento
                    && x.TP_DOCUMENTO_INGRESO == "DA"
                    && x.NU_DOCUMENTO_CAMBIO == null);
        }

        public virtual void GenerarCuentaCorrienteInsumo(string nroDocumento, string tipoDocumento)
        {
            string sql = "PR_CUENTA_CORRIENTE_INSUMO";
            _dapper.Query<object>(_context.Database.GetDbConnection(), sql, param: new
            {
                NU_DOCUMENTO = nroDocumento,
                TP_DOCUMENTO = tipoDocumento
            }, commandType: CommandType.StoredProcedure, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual IDocumentoEgreso GetEgresoPorCamion(int codigoCamion)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Include("T_DET_DOCUMENTO_EGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(d => d.CD_CAMION == codigoCamion)
                .AsNoTracking()
                .FirstOrDefault();

            return documentoEntity == null ? null : this._mapper.MapToEgreso(documentoEntity, this._service);
        }

        public virtual void UpdateEgreso(IDocumentoEgreso documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromEgreso(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documento.Numero
                    && x.TP_DOCUMENTO == documento.Tipo);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateEgresoAndDetails(IDocumentoEgreso documento, List<DocumentoLineaEgreso> nuevaslineas, long nuTransaccion)
        {
            var documentoEntity = _mapper.CreateEntityDocumento(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local.FirstOrDefault(x => x.NU_DOCUMENTO == documento.Numero && x.TP_DOCUMENTO == documento.Tipo);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry<T_DOCUMENTO>(documentoEntity).State = EntityState.Modified;
            }

            foreach (var lineaEgreso in documento.OutDetail)
            {
                UpdateLineaEgreso(lineaEgreso, documento.Numero, documento.Tipo, nuTransaccion);
            }

            foreach (var nuevaLineaEgreso in nuevaslineas)
            {
                AddLineaEgreso(documento, nuevaLineaEgreso, nuTransaccion);
            }
        }

        public virtual void RemoveEgresoDetails(IDocumentoEgreso documento, long nuTransaccion)
        {
            foreach (var linea in documento.OutDetail)
            {
                RemoveEgresoDetail(documento, linea, nuTransaccion);
            }

            documento.OutDetail.Clear();
        }

        public virtual void RemoveEgresoDetail(IDocumentoEgreso documento, DocumentoLineaEgreso linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLineaEgreso(documento.Numero, documento.Tipo, linea);
            var attachedDetailEntity = this._context.T_DET_DOCUMENTO_EGRESO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO
                    && x.NU_SECUENCIA == detailEntity.NU_SECUENCIA);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            foreach (var reservaEntity in detailEntity.T_DET_DOCU_EGRESO_RESERV)
            {
                var attachedReservaEntity = this._context.T_DET_DOCU_EGRESO_RESERV.Local
                    .FirstOrDefault(x => x.NU_DOCUMENTO == reservaEntity.NU_DOCUMENTO
                        && x.TP_DOCUMENTO == reservaEntity.TP_DOCUMENTO
                        && x.NU_SECUENCIA == reservaEntity.NU_SECUENCIA
                        && x.NU_DOCUMENTO_INGRESO == reservaEntity.NU_DOCUMENTO_INGRESO
                        && x.TP_DOCUMENTO_INGRESO == reservaEntity.TP_DOCUMENTO_INGRESO
                        && x.NU_PREPARACION == reservaEntity.NU_PREPARACION
                        && x.CD_EMPRESA == reservaEntity.CD_EMPRESA
                        && x.CD_PRODUTO == reservaEntity.CD_PRODUTO
                        && x.CD_FAIXA == reservaEntity.CD_FAIXA
                        && x.NU_IDENTIFICADOR_PICKING_DET == reservaEntity.NU_IDENTIFICADOR_PICKING_DET
                        && x.NU_IDENTIFICADOR == reservaEntity.NU_IDENTIFICADOR);

                if (attachedReservaEntity != null)
                {
                    this._context.T_DET_DOCU_EGRESO_RESERV.Remove(attachedReservaEntity);
                }
                else
                {
                    this._context.T_DET_DOCU_EGRESO_RESERV.Attach(reservaEntity);
                    this._context.T_DET_DOCU_EGRESO_RESERV.Remove(reservaEntity);
                }
            }

            if (attachedDetailEntity != null)
            {
                this._context.T_DET_DOCUMENTO_EGRESO.Remove(attachedDetailEntity);
            }
            else
            {
                this._context.T_DET_DOCUMENTO_EGRESO.Attach(detailEntity);
                this._context.T_DET_DOCUMENTO_EGRESO.Remove(detailEntity);
            }
        }

        public virtual void AddLineaEgreso(IDocumentoEgreso documento, DocumentoLineaEgreso linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLineaEgreso(documento.Numero, documento.Tipo, linea);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            this._context.T_DET_DOCUMENTO_EGRESO.Add(detailEntity);
        }

        public virtual DocumentoLineaEgreso GetLineaEgreso(string nroDocumento, string tipoDocumento, string producto, string identificador)
        {
            var documentoLineaEntity = this._context.T_DET_DOCUMENTO_EGRESO
                .Include("T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DET_DOCU_EGRESO_RESERV")
                .Where(d => d.TP_DOCUMENTO == tipoDocumento
                    && d.NU_DOCUMENTO == nroDocumento
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador)
                .AsNoTracking()
                .FirstOrDefault();

            return (documentoLineaEntity == null) ? null : this._mapper.MapToDocumentoLineaEgreso(documentoLineaEntity, this._service);
        }

        public virtual DocumentoLineaEgreso GetLineaEgreso(IDocumentoEgreso documento, string producto, string identificador)
        {
            var documentoLineaEntity = this._context.T_DET_DOCUMENTO_EGRESO
                .Include("T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(d => d.TP_DOCUMENTO == documento.Tipo
                    && d.NU_DOCUMENTO == documento.Numero
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador)
                .AsNoTracking()
                .FirstOrDefault();

            if (documentoLineaEntity != null)
            {
                var lineaObj = this._mapper.MapToDocumentoLineaEgreso(documentoLineaEntity, this._service);

                lineaObj.DocumentoIngreso = this.GetIngreso(documentoLineaEntity.NU_DOCUMENTO_INGRESO, documentoLineaEntity.TP_DOCUMENTO_INGRESO);

                return lineaObj;
            }
            else
            {
                return null;
            }
        }

        public virtual void UpdateLineaEgreso(DocumentoLineaEgreso linea, string nroDocumento, string tipoDocumento, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLineaEgreso(nroDocumento, tipoDocumento, linea);
            var attachedEntity = this._context.T_DET_DOCUMENTO_EGRESO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO
                    && x.NU_SECUENCIA == detailEntity.NU_SECUENCIA);

            detailEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DET_DOCUMENTO_EGRESO.Attach(detailEntity);
                this._context.Entry<T_DET_DOCUMENTO_EGRESO>(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual List<LineaEgresoDocumental> GetLineasEgreso(int camion)
        {
            return this._context.V_EGRESO_GEN_DOCOCUMENTO_WEXP
                .AsNoTracking()
                .Where(s => s.CD_CAMION == camion)
                .Select(s => new LineaEgresoDocumental()
                {
                    Preparacion = s.NU_PREPARACION,
                    Empresa = s.CD_EMPRESA,
                    Producto = s.CD_PRODUTO,
                    Faixa = s.CD_FAIXA,
                    Identificador = s.NU_IDENTIFICADOR,
                    CantidadAfectada = s.QT_PRODUTO
                })
                .ToList();
        }

        public virtual bool AnyCargaSinPreparar(int camion)
        {
            return this._context.V_EGRESO_CONTROL_DOC_WEXP
                .AsNoTracking()
                .Any(s => s.CD_CAMION == camion && s.QT_PREPARADO == null);
        }

        #endregion

        #region DOCUMENTO_ACTA

        public virtual IDocumentoActa GetActa(string nroDocumento, string tipoDocumento)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Include("T_DET_DOCUMENTO_EGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DET_DOCUMENTO_ACTA")
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    d => d.CD_EMPRESA.Value,
                    ef => ef.CD_EMPRESA,
                    (d, ef) => d)
                .Where(d => d.NU_DOCUMENTO == nroDocumento && d.TP_DOCUMENTO == tipoDocumento)
                .AsNoTracking()
                .FirstOrDefault();

            var paramRepository = new ParametroRepository(this._context, this._application, this._userId, this._dapper);

            var tipoActa = paramRepository.GetParameter(ParamManager.TP_DOC_ACTA, new Dictionary<string, string>()
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{documentoEntity.CD_EMPRESA.Value}"
            });

            if (tipoDocumento == tipoActa)
            {
                return this._mapper.MapToActa(documentoEntity, this._service);
            }
            else
            {
                return this._mapper.MapToActaStock(documentoEntity, this._service);
            }
        }

        public virtual IDocumentoActa GetActaStockEnEdicion(string nroIngreso, string tipoIngreso, string tipoActaStock, bool positiva = false)
        {
            var documentoTipoRepository = new DocumentoTipoRepository(this._context, this._application, this._userId);

            IDocumentoActa acta = null;

            var query = this._context.T_DET_DOCUMENTO_ACTA
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO_ESTADO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(d => d.NU_DOCUMENTO == nroIngreso
                    && d.TP_DOCUMENTO == tipoIngreso
                    && d.TP_ACTA == tipoActaStock
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_PERMITE_EDICION == "S"
                    && d.T_DOCUMENTO.T_DOCUMENTO_TIPO_ESTADO.FL_PERMITE_EDICION == "S");

            if (positiva)
            {
                query = query.Where(s => s.T_DOCUMENTO.T_DET_DOCUMENTO.Count > 0);
            }
            else
            {
                query = query.Where(s => s.T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.Count > 0);
            }

            var documentoEntity = query.Select(s => s.T_DOCUMENTO).AsNoTracking().FirstOrDefault();

            if (documentoEntity != null)
            {
                acta = this._mapper.MapToActaStock(documentoEntity, this._service);
            }

            return acta;
        }

        public virtual void AddActa(IDocumentoActa documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromActa(documento);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (documentoEntity.T_DET_DOCUMENTO.Count > 0)
                documentoEntity.T_DET_DOCUMENTO.Select(d => { d.VL_DATO_AUDITORIA = datoAuditoria; return d; }).ToList();

            if (documentoEntity.T_DET_DOCUMENTO_EGRESO.Count > 0)
                documentoEntity.T_DET_DOCUMENTO_EGRESO.Select(d => { d.VL_DATO_AUDITORIA = datoAuditoria; d.NU_SECUENCIA = GetNumeroSecuenciaDetalleEgreso(); return d; }).ToList();

            this._context.T_DOCUMENTO.Add(documentoEntity);
        }

        public virtual void UpdateActa(IDocumentoActa documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromActa(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateActaSinDetalle(IDocumentoActa documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromActaSinDetalle(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local
               .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                   && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Modified;
            }
        }

        public virtual void AddLineaActa(IDocumentoActa documento, DocumentoLinea linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            this._context.T_DET_DOCUMENTO.Add(detailEntity);
        }

        public virtual void AddLineaEgresoActa(IDocumentoActa documento, DocumentoLineaEgreso linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLineaEgreso(documento.Numero, documento.Tipo, linea);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            this._context.T_DET_DOCUMENTO_EGRESO.Add(detailEntity);
        }

        public virtual void AddLineaEgresoActa(DocumentoLineaEgreso linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapToDocumentoLineaEgresoCambio(linea);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            this._context.T_DET_DOCU_EGRESO_CAMBIO.Add(detailEntity);
        }

        public virtual void UpdateLineaActa(DocumentoLinea linea, string nroDocumento, string tipoDocumento, long nuTransaccion)
        {
            var lineaEntity = this._mapper.MapFromDocumentoLinea(nroDocumento, tipoDocumento, linea);

            lineaEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            this._context.Entry(lineaEntity).State = EntityState.Modified;
        }

        public virtual void UpdateLineaEgresoActa(DocumentoLineaEgreso linea, string nroDocumento, string tipoDocumento, long nuTransaccion)
        {
            var lineaEntity = this._mapper.MapFromDocumentoLineaEgreso(nroDocumento, tipoDocumento, linea);

            lineaEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            this._context.Entry(lineaEntity).State = EntityState.Modified;
        }

        public virtual void UpdateActaIngresoAndDetails(IDocumentoActa documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromActa(documento);
            var attachedDocumentoEntity = this._context.T_DOCUMENTO.Local
               .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                   && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedDocumentoEntity != null)
            {
                var attachedEntry = _context.Entry(attachedDocumentoEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Modified;
            }

            foreach (T_DET_DOCUMENTO linea in documentoEntity.T_DET_DOCUMENTO)
            {
                var attachedLineaEntity = this._context.T_DET_DOCUMENTO.Local
                    .FirstOrDefault(x => x.NU_DOCUMENTO == linea.NU_DOCUMENTO
                        && x.CD_PRODUTO == linea.CD_PRODUTO
                        && x.NU_IDENTIFICADOR == linea.NU_IDENTIFICADOR
                        && x.CD_FAIXA == linea.CD_FAIXA
                        && x.TP_DOCUMENTO == linea.TP_DOCUMENTO);

                linea.VL_DATO_AUDITORIA = datoAuditoria;

                if (attachedLineaEntity != null)
                {
                    var attachedEntry = this._context.Entry(attachedLineaEntity);
                    attachedEntry.CurrentValues.SetValues(linea);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    this._context.T_DET_DOCUMENTO.Attach(linea);
                    this._context.Entry(linea).State = EntityState.Modified;
                }
            }
        }

        #endregion

        #region DOCUMENTO_PRODUCCION

        public virtual DocumentoProduccion GetDocumentoProduccion(string nroProduccion, string nroDocumentoIngreso, string tpDocumentoIngreso, string nroDocumentoEgreso, string tpDocumentoEgreso)
        {
            var documentoProduccionEntity = this._context.T_DOCUMENTO_PRODUCCION
                .Include("T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(dp => dp.NU_PRDC_INGRESO == nroProduccion
                    && dp.NU_DOCUMENTO_ING == nroDocumentoIngreso
                    && dp.NU_DOCUMENTO_EGR == nroDocumentoEgreso
                    && dp.TP_DOCUMENTO_ING == tpDocumentoIngreso
                    && dp.TP_DOCUMENTO_EGR == tpDocumentoEgreso)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToDocumentoProduccion(documentoProduccionEntity, this._service);
        }

        public virtual void AddDocumentoProduccion(DocumentoProduccion documentoProduccion)
        {
            if (!AnyDocumentoProduccion(documentoProduccion))
            {
                var documentoProduccionEntity = this._mapper.MapFromDocumentoProduccion(documentoProduccion);
                documentoProduccionEntity.DT_ADDROW = DateTime.Now;
                this._context.T_DOCUMENTO_PRODUCCION.Add(documentoProduccionEntity);
            }
        }

        public virtual bool AnyDocumentoProduccion(DocumentoProduccion documentoProduccion)
        {
            return this._context.T_DOCUMENTO_PRODUCCION
                .AsNoTracking()
                .Any(dt => dt.NU_DOCUMENTO_EGR == documentoProduccion.DocumentoEgreso.Numero
                    && dt.TP_DOCUMENTO_EGR == documentoProduccion.DocumentoEgreso.Tipo
                    && dt.NU_DOCUMENTO_ING == documentoProduccion.DocumentoIngreso.Numero
                    && dt.TP_DOCUMENTO_ING == documentoProduccion.DocumentoIngreso.Tipo
                    && dt.NU_PRDC_INGRESO == documentoProduccion.NumeroProduccion);
        }

        #endregion

        #region DOCUMENTO_TRANSFERENCIA

        public virtual DocumentoTransferencia GetDocumentoTransferencia(string nroTransferencia, string nroDocumentoIngreso, string tpDocumentoIngreso, string nroDocumentoEgreso, string tpDocumentoEgreso)
        {
            var documentoTransferenciaEntity = this._context.T_DOCUMENTO_TRANSFERENCIA
                .Include("T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO_EGRESO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(dp => dp.NU_TRANSFERENCIA == nroTransferencia
                    && dp.NU_DOCUMENTO_ING == nroDocumentoIngreso
                    && dp.NU_DOCUMENTO_EGR == nroDocumentoEgreso
                    && dp.TP_DOCUMENTO_ING == tpDocumentoIngreso
                    && dp.TP_DOCUMENTO_EGR == tpDocumentoEgreso)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToDocumentoTransferencia(documentoTransferenciaEntity, this._service);
        }

        public virtual void AddDocumentoTransferencia(DocumentoTransferencia documentoTransferencia)
        {
            if (!AnyDocumentoTransferencia(documentoTransferencia))
            {
                var documentoTransferenciaEntity = this._mapper.MapFromDocumentoTransferencia(documentoTransferencia);
                this._context.T_DOCUMENTO_TRANSFERENCIA.Add(documentoTransferenciaEntity);
            }
        }

        public virtual bool AnyDocumentoTransferencia(DocumentoTransferencia documentoTransferencia)
        {
            return this._context.T_DOCUMENTO_TRANSFERENCIA
                .AsNoTracking()
                .Any(dt => dt.NU_DOCUMENTO_EGR == documentoTransferencia.DocumentoEgreso.Numero
                    && dt.TP_DOCUMENTO_EGR == documentoTransferencia.DocumentoEgreso.Tipo
                    && dt.NU_DOCUMENTO_ING == documentoTransferencia.DocumentoIngreso.Numero
                    && dt.TP_DOCUMENTO_ING == documentoTransferencia.DocumentoIngreso.Tipo
                    && dt.NU_TRANSFERENCIA == documentoTransferencia.NumeroTransferencia);
        }

        #endregion

        #region DOCUMENTO_AGRUPADOR

        public virtual IDocumentoAgrupador GetAgrupador(string nroAgrupador, string tipoAgrupador)
        {
            var agrupadorEntity = this._context.T_DOCUMENTO_AGRUPADOR
                .Include("T_DOCUMENTO_AGRUPADOR_TIPO")
                .Include("T_TRANSPORTADORA")
                .Include("T_TIPO_VEICULO")
                .Where(a => a.NU_AGRUPADOR == nroAgrupador && a.TP_AGRUPADOR == tipoAgrupador)
                .AsNoTracking()
                .FirstOrDefault();

            if (agrupadorEntity != null)
                return this._mapper.MapToDocumentoAgrupador(agrupadorEntity, this._service);
            else
                return null;
        }

        public virtual IDocumentoAgrupador GetAgrupadorWithDetail(string nroAgrupador, string tipoAgrupador)
        {
            IDocumentoAgrupador agrupador;

            var agrupadorEntity = this._context.T_DOCUMENTO_AGRUPADOR
                .Include("T_DOCUMENTO_AGRUPADOR_TIPO")
                .Include("T_TRANSPORTADORA")
                .Include("T_TIPO_VEICULO")
                .Where(a => a.NU_AGRUPADOR == nroAgrupador && a.TP_AGRUPADOR == tipoAgrupador)
                .AsNoTracking()
                .FirstOrDefault();

            agrupador = this._mapper.MapToDocumentoAgrupador(agrupadorEntity, this._service);

            if (agrupadorEntity != null)
            {
                switch (agrupador.Tipo.TipoOperacion)
                {
                    case TipoOperacionAgrupador.EGRESO:
                        agrupador.LineasEgresoAgrupadas = this.GetEgresosByAgrupador(nroAgrupador, tipoAgrupador);
                        break;
                    case TipoOperacionAgrupador.INGRESO:
                        agrupador.LineasIngresoAgrupadas = this.GetIngresosByAgrupador(nroAgrupador, tipoAgrupador);
                        break;
                }
            }

            return agrupador;
        }

        public virtual List<IDocumentoIngreso> GetDocumentosIngresoAgrupablesByKeys(List<KeyValuePair<string, string>> keysAgrupadores)
        {
            List<IDocumentoIngreso> documentos = new List<IDocumentoIngreso>();

            foreach (var keys in keysAgrupadores)
            {
                documentos.Add(this.GetIngresoAgrupable(keys.Key, keys.Value));
            }

            return documentos;
        }

        public virtual List<IDocumentoEgreso> GetDocumentosEgresoAgrupablesByKeys(List<KeyValuePair<string, string>> keysAgrupadores)
        {
            List<IDocumentoEgreso> documentos = new List<IDocumentoEgreso>();

            foreach (var keys in keysAgrupadores)
            {
                documentos.Add(this.GetEgresoAgrupable(keys.Key, keys.Value));
            }

            return documentos;
        }

        public virtual IDocumentoIngreso GetIngresoAgrupable(string nroDocumento, string tipoDocumento)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Where(d => d.NU_DOCUMENTO == nroDocumento && d.TP_DOCUMENTO == tipoDocumento)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToIngreso(documentoEntity, this._service);
        }

        public virtual IDocumentoEgreso GetEgresoAgrupable(string nroDocumento, string tipoDocumento)
        {
            var documentoEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Include("T_DET_DOCUMENTO_EGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(d => d.NU_DOCUMENTO == nroDocumento && d.TP_DOCUMENTO == tipoDocumento)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToEgreso(documentoEntity, this._service);
        }

        public virtual List<IDocumentoIngreso> GetIngresosByAgrupador(string nroAgrupador, string tipoAgrupador)
        {
            List<IDocumentoIngreso> documentosAgrupados = new List<IDocumentoIngreso>();
            var documentosEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Where(d => d.NU_AGRUPADOR == nroAgrupador && d.TP_AGRUPADOR == tipoAgrupador)
                .AsNoTracking()
                .ToList();

            foreach (var documento in documentosEntity)
            {
                documentosAgrupados.Add(this._mapper.MapToIngreso(documento, this._service));
            }

            return documentosAgrupados;
        }

        public virtual List<IDocumentoEgreso> GetEgresosByAgrupador(string nroAgrupador, string tipoAgrupador)
        {
            List<IDocumentoEgreso> documentosAgrupados = new List<IDocumentoEgreso>();
            var documentosEntity = this._context.T_DOCUMENTO
                .Include("T_DET_DOCUMENTO")
                .Include("T_DET_DOCUMENTO_EGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Where(d => d.NU_AGRUPADOR == nroAgrupador && d.TP_AGRUPADOR == tipoAgrupador)
                .AsNoTracking()
                .ToList();

            foreach (var documento in documentosEntity)
            {
                documentosAgrupados.Add(this._mapper.MapToEgreso(documento, this._service));
            }

            return documentosAgrupados;
        }

        public virtual void AddAgrupador(IDocumentoAgrupador agrupador, long nuTransaccion)
        {
            var agrupadorEntity = this._mapper.MapFromDocumentoAgrupador(agrupador);
            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            agrupadorEntity.VL_DATO_AUDITORIA = datoAuditoria;

            this._context.T_DOCUMENTO_AGRUPADOR.Add(agrupadorEntity);
        }

        public virtual void UpdateAgrupador(IDocumentoAgrupador agrupador, long nuTransaccion)
        {
            var agrupadorEntity = this._mapper.MapFromDocumentoAgrupador(agrupador);
            var attachedEntity = this._context.T_DOCUMENTO_AGRUPADOR.Local
                .FirstOrDefault(x => x.NU_AGRUPADOR == agrupadorEntity.NU_AGRUPADOR
                    && x.TP_AGRUPADOR == agrupadorEntity.TP_AGRUPADOR);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            agrupadorEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(agrupadorEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO_AGRUPADOR.Attach(agrupadorEntity);
                this._context.Entry(agrupadorEntity).State = EntityState.Modified;
            }
        }

        public virtual List<DocumentoAgrupadorTipo> GetDocumentoAgrupadorTipos()
        {
            List<DocumentoAgrupadorTipo> tipos = new List<DocumentoAgrupadorTipo>();

            var agrupadorTiposEntity = this._context.T_DOCUMENTO_AGRUPADOR_TIPO
                .Include("T_DOCUMENTO_AGRUPADOR_GRUPO")
                .Where(t => t.FL_HABILITADO == "S")
                .AsNoTracking()
                .ToList();

            foreach (var entity in agrupadorTiposEntity)
            {
                tipos.Add(this._mapper.MapToDocumentoAgrupadorTipo(entity));
            }

            return tipos;
        }

        public virtual DocumentoAgrupadorTipo GetDocumentoAgrupadorTipo(string tipoAgrupador)
        {
            var agrupadorTipoEntity = this._context.T_DOCUMENTO_AGRUPADOR_TIPO
                .Include("T_DOCUMENTO_AGRUPADOR_GRUPO")
                .Where(t => t.TP_AGRUPADOR == tipoAgrupador)
                .AsNoTracking()
                .FirstOrDefault();

            if (agrupadorTipoEntity != null)
                return this._mapper.MapToDocumentoAgrupadorTipo(agrupadorTipoEntity);
            else
                return null;
        }

        #endregion

        #region DOCUMENTO_RESERVAS

        public virtual List<DocumentoPreparacionReserva> GetDocumentoPreparacionReservas(int preparacion, string producto, string identificador, int empresa, decimal? faixa)
        {
            var reservas = new List<DocumentoPreparacionReserva>();

            var documentoReservaEntity = this._context.T_DOCUMENTO_PREPARACION_RESERV
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
                .Where(d => d.NU_PREPARACION == preparacion
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR_PICKING_DET == identificador
                    && d.CD_EMPRESA == empresa
                    && d.CD_FAIXA == faixa)
                .AsNoTracking()
                .ToList();

            documentoReservaEntity.ForEach(entity => reservas.Add(this._mapper.MapToDocumentoReserva(entity, this._service)));

            return reservas;
        }

        public virtual List<DocumentoLineaDesafectada> DesafectarLineasSinReserva(string producto, string identificador, int empresa, decimal? faixa, decimal? qtDesafectar)
        {
            var lineasDesafectadas = new List<DocumentoLineaDesafectada>();
            var lineasConSaldo = this._context.T_DET_DOCUMENTO
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO_ESTADO")
                .AsNoTracking()
                .Where(dd => dd.CD_PRODUTO == producto
                    && dd.NU_IDENTIFICADOR == identificador
                    && dd.CD_EMPRESA == empresa
                    && dd.CD_FAIXA == faixa
                    && dd.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && dd.T_DOCUMENTO.T_DOCUMENTO_TIPO.FL_DISPONIBILIZA_STOCK == "S"
                    && dd.T_DOCUMENTO.T_DOCUMENTO_TIPO_ESTADO.FL_DISPONIBILIZA_STOCK == "S"
                    && (dd.QT_INGRESADA ?? 0) - (dd.QT_RESERVADA ?? 0) - (dd.QT_DESAFECTADA ?? 0) > 0)
                .OrderByDescending(dd => (dd.QT_INGRESADA ?? 0) - (dd.QT_RESERVADA ?? 0) - (dd.QT_DESAFECTADA ?? 0));

            var qtDisponible = lineasConSaldo
                .Sum(dd => (dd.QT_INGRESADA ?? 0) - (dd.QT_RESERVADA ?? 0) - (dd.QT_DESAFECTADA ?? 0));

            if (qtDisponible >= qtDesafectar)
            {
                var saldoDesafectar = qtDesafectar ?? 0;

                foreach (var linea in lineasConSaldo)
                {
                    if (saldoDesafectar == 0)
                        break;

                    var saldoLinea = (linea.QT_INGRESADA ?? 0) - (linea.QT_RESERVADA ?? 0) - (linea.QT_DESAFECTADA ?? 0);

                    if (saldoLinea >= saldoDesafectar)
                    {
                        linea.QT_DESAFECTADA = (linea.QT_DESAFECTADA ?? 0) + saldoDesafectar;
                        saldoDesafectar = 0;
                    }
                    else
                    {
                        linea.QT_DESAFECTADA = (linea.QT_DESAFECTADA ?? 0) + saldoLinea;
                        saldoDesafectar -= saldoLinea;
                    }

                    lineasDesafectadas.Add(new DocumentoLineaDesafectada
                    {
                        NroDocumento = linea.NU_DOCUMENTO,
                        TipoDocumento = linea.TP_DOCUMENTO,
                        LineaModificada = this._mapper.MapToDocumentoLinea(linea)
                    });
                }
            }
            else
            {
                throw new ValidationFailedException("General_Sec0_Error_TransferenciaDocumentoSinSaldo", new string[] {
                    producto,
                    empresa.ToString(),
                    identificador
                });
            }

            return lineasDesafectadas;
        }

        public virtual List<DocumentoPreparacionReserva> GetDocumentoSemiacabado(string producto, string identificador, int empresa, decimal? faixa)
        {
            List<DocumentoPreparacionReserva> reservas = new List<DocumentoPreparacionReserva>();

            var documentoReservaEntity = this._context.V_DET_DOCUMENTO_RESERVA
                .Where(d => d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador
                    && d.CD_FAIXA == faixa
                    && d.CD_EMPRESA == empresa
                    && d.QT_RESERVADA > 0)
                .AsNoTracking()
                .ToList();

            documentoReservaEntity.ForEach(entity => reservas.Add(this._mapper.MapToDocumentoReserva(entity)));

            return reservas;
        }

        public virtual DocumentoPreparacionReserva GetPreparacionReserva(string nuDocumento, string tpDocumento, int preparacion, int empresa, string producto, decimal faixa, string identificador)
        {
            var documentoReservaEntity = this._context.T_DOCUMENTO_PREPARACION_RESERV
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
                .Where(d => d.NU_DOCUMENTO == nuDocumento
                    && d.TP_DOCUMENTO == tpDocumento
                    && d.NU_PREPARACION == preparacion
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR_PICKING_DET == identificador
                    && d.CD_FAIXA == faixa
                    && d.CD_EMPRESA == empresa)
                .AsNoTracking()
                .FirstOrDefault();

            return (documentoReservaEntity == null) ? null : this._mapper.MapToDocumentoReserva(documentoReservaEntity, this._service);
        }

        public virtual DocumentoPreparacionReserva GetPreparacionReservaDesafectada(string nuDocumento, string tpDocumento, int preparacion, int empresa, string producto, decimal faixa, string identificadorPicking, string identificador)
        {
            var documentoReservaDesafectadaEntity = this._context.LT_DELETE_DOCUMENTO_PREPARACION_RESERV
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
                .Where(d => d.NU_DOCUMENTO == nuDocumento
                    && d.TP_DOCUMENTO == tpDocumento
                    && d.NU_PREPARACION == preparacion
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR_PICKING_DET == identificadorPicking
                    && d.NU_IDENTIFICADOR == identificador)
                .AsNoTracking()
                .FirstOrDefault();

            return (documentoReservaDesafectadaEntity == null) ? null : this._mapper.MapToDocumentoReservaDesafectada(documentoReservaDesafectadaEntity, this._service);
        }

        public virtual List<DocumentoPreparacionReserva> GetPreparacionReservas(int preparacion, int empresa, string producto, decimal faixa, string identificador)
        {
            var documentoReservaEntitys = this._context.T_DOCUMENTO_PREPARACION_RESERV
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
                .Where(d => d.NU_PREPARACION == preparacion
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR_PICKING_DET == identificador
                    && d.CD_EMPRESA == empresa
                    && (d.QT_PRODUTO ?? 0) - (d.QT_PREPARADO ?? 0) > 0)
                .OrderByDescending(d => d.T_DOCUMENTO.DT_ADDROW)
                .AsNoTracking()
                .ToList();

            List<DocumentoPreparacionReserva> preparacionReservas = new List<DocumentoPreparacionReserva>();

            foreach (var resevaEntity in documentoReservaEntitys)
            {
                preparacionReservas.Add(this._mapper.MapToDocumentoReserva(resevaEntity, this._service));
            }

            return preparacionReservas;
        }

        public virtual void AddPreparacionReserva(DocumentoPreparacionReserva preparacionReserva)
        {
            preparacionReserva.FechaAlta = DateTime.Now;

            var nuTransaccion = preparacionReserva.NumeroTransaccion;
            var documentoEntity = this._mapper.MapFromPreparacionReserva(preparacionReserva);
            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;
            documentoEntity.NU_TRANSACCION_DELETE = null;

            this._context.T_DOCUMENTO_PREPARACION_RESERV.Add(documentoEntity);
        }

        public virtual void UpdateDocumentoPreparacionReserva(DocumentoPreparacionReserva documentoPreparacionReserva)
        {
            documentoPreparacionReserva.FechaModificacion = DateTime.Now;

            var nuTransaccion = documentoPreparacionReserva.NumeroTransaccion;
            var documentoEntity = this._mapper.MapFromPreparacionReserva(documentoPreparacionReserva);
            var attachedEntity = this._context.T_DOCUMENTO_PREPARACION_RESERV.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO
                    && x.NU_PREPARACION == documentoEntity.NU_PREPARACION
                    && x.CD_EMPRESA == documentoEntity.CD_EMPRESA
                    && x.CD_PRODUTO == documentoEntity.CD_PRODUTO
                    && x.CD_FAIXA == documentoEntity.CD_FAIXA
                    && x.NU_IDENTIFICADOR_PICKING_DET == documentoEntity.NU_IDENTIFICADOR_PICKING_DET
                    && x.NU_IDENTIFICADOR == documentoEntity.NU_IDENTIFICADOR);

            documentoEntity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(documentoEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO_PREPARACION_RESERV.Attach(documentoEntity);
                this._context.Entry<T_DOCUMENTO_PREPARACION_RESERV>(documentoEntity).State = EntityState.Modified;
            }
        }

        public virtual void RemoveDocumentoPreparacionReserva(DocumentoPreparacionReserva documentoPreparacionReserva)
        {
            var nuTransaccion = documentoPreparacionReserva.NumeroTransaccion;
            var documentoEntity = this._mapper.MapFromPreparacionReserva(documentoPreparacionReserva);
            var attachedEntity = this._context.T_DOCUMENTO_PREPARACION_RESERV.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO
                    && x.NU_PREPARACION == documentoEntity.NU_PREPARACION
                    && x.CD_EMPRESA == documentoEntity.CD_EMPRESA
                    && x.CD_PRODUTO == documentoEntity.CD_PRODUTO
                    && x.CD_FAIXA == documentoEntity.CD_FAIXA
                    && x.NU_IDENTIFICADOR_PICKING_DET == documentoEntity.NU_IDENTIFICADOR_PICKING_DET
                    && x.NU_IDENTIFICADOR == documentoEntity.NU_IDENTIFICADOR);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                this._context.T_DOCUMENTO_PREPARACION_RESERV.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DOCUMENTO_PREPARACION_RESERV.Attach(documentoEntity);
                this._context.T_DOCUMENTO_PREPARACION_RESERV.Remove(documentoEntity);
            }
        }

        public virtual void RemoveDocumentoPreparacionReservaDesafectada(DocumentoPreparacionReserva documentoPreparacionReserva)
        {
            var nuTransaccion = documentoPreparacionReserva.NumeroTransaccion;
            var documentoEntity = this._mapper.MapFromPreparacionReservaDesafectada(documentoPreparacionReserva);
            var attachedEntity = this._context.LT_DELETE_DOCUMENTO_PREPARACION_RESERV.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO
                    && x.NU_PREPARACION == documentoEntity.NU_PREPARACION
                    && x.CD_EMPRESA == documentoEntity.CD_EMPRESA
                    && x.CD_PRODUTO == documentoEntity.CD_PRODUTO
                    && x.CD_FAIXA == documentoEntity.CD_FAIXA
                    && x.NU_IDENTIFICADOR_PICKING_DET == documentoEntity.NU_IDENTIFICADOR_PICKING_DET
                    && x.NU_IDENTIFICADOR == documentoEntity.NU_IDENTIFICADOR);

            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                this._context.LT_DELETE_DOCUMENTO_PREPARACION_RESERV.Remove(attachedEntity);
            }
            else
            {
                this._context.LT_DELETE_DOCUMENTO_PREPARACION_RESERV.Attach(documentoEntity);
                this._context.LT_DELETE_DOCUMENTO_PREPARACION_RESERV.Remove(documentoEntity);
            }
        }

        public virtual List<DocumentoPreparacionReserva> GetPreparacionReservas(int preparacion, int empresa)
        {
            return _context.T_DOCUMENTO_PREPARACION_RESERV
            .Include("T_DOCUMENTO")
            .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
            .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
            .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
            .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
            .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
            .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
            .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
            .AsNoTracking().Where(r => r.NU_PREPARACION == preparacion && r.CD_EMPRESA == empresa)
            .Select(r => _mapper.MapToDocumentoReserva(r, this._service)).ToList();
        }

        public virtual List<DocumentoPreparacionReserva> GetPreparacionReservasDocumental(int preparacion)
        {
            return _context.T_DOCUMENTO_PREPARACION_RESERV.AsNoTracking().Where(r => r.NU_PREPARACION == preparacion).Select(r => _mapper.MapEntityToObject(r)).ToList();
        }

        #endregion

        #region DOCUMENTO_PREPARACION

        public virtual DocumentoPreparacion GetDocumentoPreparacion(int nroDocPreparacion)
        {
            var entity = _context.T_DOCUMENTO_PREPARACION
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_DOCUMENTO_PREPARACION == nroDocPreparacion);

            return (entity == null) ? null : this._mapper.MapToDocumentoPreparacion(entity);
        }

        public virtual DocumentoPreparacion GetDocumentoPreparacionEstado(int empresa, string tpOperativa, int nroPrep, bool estado)
        {
            string activa = estado ? "S" : "N";
            var entity = _context.T_DOCUMENTO_PREPARACION
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA_EGRESO == empresa
                    && d.TP_OPERATIVA == tpOperativa
                    && d.NU_PREPARACION == nroPrep
                    && d.FL_ACTIVE == activa);

            return (entity == null) ? null : this._mapper.MapToDocumentoPreparacion(entity);
        }

        public virtual void UpdateDocumentoPreparacion(DocumentoPreparacion obj)
        {
            T_DOCUMENTO_PREPARACION entity = _mapper.MapFromDocumentoPreparacion(obj);
            T_DOCUMENTO_PREPARACION attachedEntity = _context.T_DOCUMENTO_PREPARACION.Local.FirstOrDefault(x => x.NU_PREPARACION == obj.NroDocumentoPreparacion);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DOCUMENTO_PREPARACION.Attach(entity);
                _context.Entry<T_DOCUMENTO_PREPARACION>(entity).State = EntityState.Modified;
            }
        }

        public virtual bool ExisteDocumentoPreparacionActivo(int preparacion, string tpOperativa = null)
        {
            var result = _context.T_DOCUMENTO_PREPARACION
                .AsNoTracking()
                .Where(d => d.NU_PREPARACION == preparacion
                    && d.FL_ACTIVE == "S");

            if (!string.IsNullOrEmpty(tpOperativa))
                result.Where(d => d.TP_OPERATIVA == tpOperativa);

            return result.Any();
        }

        public virtual bool ExistenDocumentosEnOtraAsociacion(int nuDocPrep, string nuDocingreso, string tpDocIngreso, string nuDocEgreso, string tpDocEgreso)
        {
            return _context.T_DOCUMENTO_PREPARACION
                .AsNoTracking()
                .Any(d => d.FL_ACTIVE == "S"
                    && d.NU_DOCUMENTO_INGRESO == nuDocingreso
                    && d.TP_DOCUMENTO_INGRESO == tpDocIngreso
                    && d.NU_DOCUMENTO_EGRESO == nuDocEgreso
                    && d.TP_DOCUMENTO_EGRESO == tpDocEgreso
                    && d.NU_DOCUMENTO_PREPARACION != nuDocPrep);
        }

        public virtual bool AnyContenedorEnsamblado(int prep)
        {
            return _context.T_CONTENEDOR
                .AsNoTracking()
                .Any(d => d.NU_PREPARACION == prep
                    && d.CD_SITUACAO == SituacionDb.ContenedorEnsambladoKit);
        }

        public virtual void DeleteDocumentoPreparacion(DocumentoPreparacion doc)
        {
            var entity = this._mapper.MapFromDocumentoPreparacion(doc);
            var attachedEntity = this._context.T_DOCUMENTO_PREPARACION.Local
                .FirstOrDefault(d => d.NU_DOCUMENTO_PREPARACION == doc.NroDocumentoPreparacion);

            if (attachedEntity != null)
            {
                this._context.T_DOCUMENTO_PREPARACION.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DOCUMENTO_PREPARACION.Attach(entity);
                this._context.T_DOCUMENTO_PREPARACION.Remove(entity);
            }
        }

        public virtual List<string[]> GetDocumentoById(string value, int empresa, string tpDoc)
        {
            return _context.V_DOC_DISP_ASOCIAR_PREP
                    .AsNoTracking()
                    .Where(p => p.CD_EMPRESA == empresa
                        && p.TP_DOCUMENTO == tpDoc
                        && (p.NU_DOCUMENTO == value || p.NU_DOCUMENTO.ToLower().Contains(value) || p.DS_DOCUMENTO.ToLower().Contains(value)))
                    .Select(e => new string[] { e.TP_DOCUMENTO, e.NU_DOCUMENTO, e.DS_DOCUMENTO })
                    .ToList();
        }

        public virtual bool IsDocumentoAsociable(string tpDocumento, string nuDocumento)
        {
            return _context.V_DOC_DISP_ASOCIAR_PREP
                    .AsNoTracking()
                    .Any(p => p.TP_DOCUMENTO == tpDocumento
                        && p.NU_DOCUMENTO == nuDocumento);
        }

        public virtual void AddDocumentoPreparacion(DocumentoPreparacion docPrep)
        {
            if (docPrep.NroDocumentoPreparacion == 0)
                docPrep.NroDocumentoPreparacion = this._context.GetNextSequenceValueInt(_dapper, "S_DOCUMENTO_PREPARACION");

            var entity = this._mapper.MapFromDocumentoPreparacion(docPrep);

            _context.T_DOCUMENTO_PREPARACION.Add(entity);
        }

        #endregion

        #region GENERAL

        public virtual void UpdateDetail(IDocumentoBase documento, DocumentoLinea linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea);
            var attachedEntity = this._context.T_DET_DOCUMENTO.Local
                .FirstOrDefault(x => x.CD_PRODUTO == detailEntity.CD_PRODUTO
                    && x.CD_EMPRESA == detailEntity.CD_EMPRESA
                    && x.NU_IDENTIFICADOR == detailEntity.NU_IDENTIFICADOR
                    && x.CD_FAIXA == detailEntity.CD_FAIXA
                    && x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO);

            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DET_DOCUMENTO.Attach(detailEntity);
                this._context.Entry(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateCuentaCorrienteDocumento(CuentaCorriente request)
        {
            var detailEntity = this._mapper.MapObjectToEntity(request);
            var attachedEntity = this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == request.NU_DOCUMENTO_EGRESO
                    && x.TP_DOCUMENTO_EGRESO == request.TP_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == request.CD_EMPRESA
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.CD_FAIXA == request.CD_FAIXA
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == request.NU_NIVEL);

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_CUENTA_CORRIENTE_INSUMO.Attach(detailEntity);
                this._context.Entry<T_PRDC_CUENTA_CORRIENTE_INSUMO>(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual CuentaCorrienteCambioDoc GetCuentaCorrienteDocumentoDOC500(CambioDocumentoDetIngreso request)
        {
            T_PRDC_CUENTA_CAMBIO_DOC insumo = this._context.T_PRDC_CUENTA_CAMBIO_DOC
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == request.NU_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == request.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == request.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == request.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == request.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == request.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == request.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == request.CD_EMPRESA
                    && x.CD_PRODUTO == request.CD_PRODUTO
                    && x.CD_FAIXA == request.CD_FAIXA
                    && x.NU_IDENTIFICADOR == request.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == request.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == request.NU_NIVEL);

            return this._mapper.MapEntityToObject(insumo);
        }

        public virtual void UpdateCuentaCorrienteDocumentoDOC500(CuentaCorrienteCambioDoc linea)
        {
            var detailEntity = this._mapper.MapObjectToEntity(linea);
            var attachedEntity = this._context.T_PRDC_CUENTA_CAMBIO_DOC.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == detailEntity.NU_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == detailEntity.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == detailEntity.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == detailEntity.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == detailEntity.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == detailEntity.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == detailEntity.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == detailEntity.CD_EMPRESA
                    && x.CD_PRODUTO == detailEntity.CD_PRODUTO
                    && x.CD_FAIXA == detailEntity.CD_FAIXA
                    && x.NU_IDENTIFICADOR == detailEntity.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == detailEntity.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == detailEntity.NU_NIVEL);

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_CUENTA_CAMBIO_DOC.Attach(detailEntity);
                this._context.Entry(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateCuentaCorrienteDocumento(CuentaCorrienteCambioDoc linea)
        {
            var detailEntity = this._mapper.MapObjectToEntity(linea);
            var attachedEntity = this._context.T_PRDC_CUENTA_CAMBIO_DOC.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == detailEntity.NU_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == detailEntity.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == detailEntity.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == detailEntity.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == detailEntity.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == detailEntity.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == detailEntity.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == detailEntity.CD_EMPRESA
                    && x.CD_PRODUTO == detailEntity.CD_PRODUTO
                    && x.CD_FAIXA == detailEntity.CD_FAIXA
                    && x.NU_IDENTIFICADOR == detailEntity.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == detailEntity.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == detailEntity.NU_NIVEL);

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_CUENTA_CAMBIO_DOC.Attach(detailEntity);
                this._context.Entry<T_PRDC_CUENTA_CAMBIO_DOC>(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetailWithoutDocument(string nroDocumento, string tipoDocumento, DocumentoLinea linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(nroDocumento, tipoDocumento, linea);
            var attachedEntity = this._context.T_DET_DOCUMENTO.Local
                .FirstOrDefault(x => x.CD_PRODUTO == detailEntity.CD_PRODUTO
                    && x.CD_EMPRESA == detailEntity.CD_EMPRESA
                    && x.NU_IDENTIFICADOR == detailEntity.NU_IDENTIFICADOR
                    && x.CD_FAIXA == detailEntity.CD_FAIXA
                    && x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO);

            var datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DET_DOCUMENTO.Attach(detailEntity);
                this._context.Entry(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual void Add(IDocumento documento, long nuTransaccion)
        {
            var documentoEntity = this._mapper.MapFromDocumento(documento);
            var attachedEntity = this._context.T_DOCUMENTO.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO == documentoEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == documentoEntity.TP_DOCUMENTO);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            documentoEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity == null)
            {
                this._context.T_DOCUMENTO.Attach(documentoEntity);
                this._context.Entry(documentoEntity).State = EntityState.Added;
            }
        }

        public virtual void AddDetail(IDocumentoBase documento, DocumentoLinea linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea);
            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            this._context.T_DET_DOCUMENTO.Add(detailEntity);
        }

        public virtual void RemoveDetail(IDocumentoBase documento, DocumentoLinea linea, long nuTransaccion)
        {
            var detailEntity = this._mapper.MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea);
            var attachedEntity = this._context.T_DET_DOCUMENTO.Local
                .FirstOrDefault(x => x.CD_PRODUTO == detailEntity.CD_PRODUTO
                    && x.CD_EMPRESA == detailEntity.CD_EMPRESA
                    && x.NU_IDENTIFICADOR == detailEntity.NU_IDENTIFICADOR
                    && x.CD_FAIXA == detailEntity.CD_FAIXA
                    && x.NU_DOCUMENTO == detailEntity.NU_DOCUMENTO
                    && x.TP_DOCUMENTO == detailEntity.TP_DOCUMENTO);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            detailEntity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                this._context.T_DET_DOCUMENTO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_DOCUMENTO.Attach(detailEntity);
                this._context.T_DET_DOCUMENTO.Remove(detailEntity);
            }
        }

        public virtual string GetNumeroDocumento(string tipoDocumento)
        {
            var secuencia = this._context.T_DOCUMENTO_TIPO
                .Where(e => e.TP_DOCUMENTO == tipoDocumento)
                .Select(e => e.NM_SECUENCIA)
                .AsNoTracking()
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(secuencia))
                return this._context.GetNextSequenceValueDecimal(_dapper, secuencia).ToString();

            return null;
        }

        public virtual int GetNumeroSecuenciaDetalleEgreso()
        {
            return int.Parse(this._context.GetNextSequenceValueInt(_dapper, "S_DET_DOCUMENTO_EGRESO").ToString());
        }

        public virtual int GetNumeroSecuenciaPreCambio()
        {
            return int.Parse(this._context.GetNextSequenceValueInt(_dapper, "S_CAMBIO_DOCUMENTO").ToString());
        }

        public virtual int GetNumeroSecuenciaCambioCuenta()
        {
            return int.Parse(this._context.GetNextSequenceValueInt(_dapper, "S_CAMBIO_DOCUMENTO_DOC080").ToString());
        }

        public virtual string GetNumeroAgrupador(string secuencia)
        {
            return this._context.GetNextSequenceValueInt(_dapper, secuencia).ToString();
        }

        public virtual DocumentoEstado GetEstado(string estado)
        {
            return _context.T_DOCUMENTO_ESTADO
                .AsNoTracking()
                .Where(e => e.ID_ESTADO == estado)
                .Select(e => new DocumentoEstado()
                {
                    Id = estado,
                    Descripcion = e.DS_ESTADO
                })
                .FirstOrDefault();
        }

        public virtual List<DocumentoAccion> GetEstadosDestino(string estadoOrigen, string tipoDocumento)
        {
            return _context.T_DOCUMENTO_ESTADO_ORDEN
                .Include("T_DOCUMENTO_ESTADO_ORIGEN")
                .Include("T_DOCUMENTO_ESTADO_DESTINO")
                .Where(o => o.TP_DOCUMENTO == tipoDocumento
                    && o.ID_ESTADO_ORIGEN == estadoOrigen)
                .AsNoTracking()
                .Select(o => this._mapper.MapToDocumentoAccion(o))
                .ToList();
        }

        public virtual List<DocumentoAccion> GetEstados(string accion)
        {
            return this._context.T_DOCUMENTO_ESTADO_ORDEN
                .Include("T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO_ESTADO_ORIGEN")
                .Include("T_DOCUMENTO_ESTADO_DESTINO")
                .Where(o => o.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && o.CD_ACCION == accion)
                .AsNoTracking()
                .Select(o => this._mapper.MapToDocumentoAccion(o))
                .ToList();
        }

        public virtual List<DocumentoAccion> GetEstadosAgrupacion(string accion)
        {
            return this._context.T_DOCUMENTO_ESTADO_ORDEN
                .Include("T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO_ESTADO_ORIGEN")
                .Include("T_DOCUMENTO_ESTADO_DESTINO")
                .Where(o => o.T_DOCUMENTO_TIPO.FL_HABILITADO == "S"
                    && o.T_DOCUMENTO_TIPO.FL_MANEJA_AGRUPADOR == "S"
                    && o.CD_ACCION == accion)
                .AsNoTracking()
                .Select(o => this._mapper.MapToDocumentoAccion(o))
                .ToList();
        }

        public virtual string GetEstadoActual(string tipoDocumento, string nuDocumento)
        {
            return this._context.T_DOCUMENTO
                .Where(e => e.TP_DOCUMENTO == tipoDocumento && e.NU_DOCUMENTO == nuDocumento)
                .Select(e => e.ID_ESTADO)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public virtual DocumentoAccion GetAccion(string tipoDocumento, string estadoOrigen, string estadoDestino)
        {
            return this._context.T_DOCUMENTO_ESTADO_ORDEN
                .Include("T_DOCUMENTO_ESTADO_ORIGEN")
                .Include("T_DOCUMENTO_ESTADO_DESTINO")
                .Where(e => e.TP_DOCUMENTO == tipoDocumento && e.ID_ESTADO_ORIGEN == estadoOrigen && e.ID_ESTADO_DESTINO == estadoDestino)
                .Select(e => this._mapper.MapToDocumentoAccion(e))
                .AsNoTracking()
                .FirstOrDefault();
        }

        public virtual string GetEstadoDestino(string tipoDocumento, string estadoOrigen, string accion)
        {
            return this._context.T_DOCUMENTO_ESTADO_ORDEN
                .Where(e => e.TP_DOCUMENTO == tipoDocumento && e.ID_ESTADO_ORIGEN == estadoOrigen && e.CD_ACCION == accion)
                .Select(e => e.ID_ESTADO_DESTINO)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public virtual string GetEstadoDestino(string tipoDocumento, string accion)
        {
            return this._context.T_DOCUMENTO_ESTADO_ORDEN
                .Where(e => e.TP_DOCUMENTO == tipoDocumento && e.CD_ACCION == accion)
                .Select(e => e.ID_ESTADO_DESTINO)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public virtual bool AnyDetalleDocumento(string numeroDocumento, string tipoDocumento, int empresa, string producto, string identificador, decimal faixa)
        {
            return this._context.T_DET_DOCUMENTO
                .AsNoTracking()
                .Any(d => d.NU_DOCUMENTO == numeroDocumento
                    && d.TP_DOCUMENTO == tipoDocumento
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador
                    && d.CD_FAIXA == faixa);
        }

        public virtual List<CuentaCorrienteCambioDoc> GetCuentaDocumentoIPNivel(string v_nu_documento_EG, int nivel)
        {
            List<T_PRDC_CUENTA_CAMBIO_DOC> listaInsumosSemiacabado = this._context.T_PRDC_CUENTA_CAMBIO_DOC
                .AsNoTracking()
                .Where(x => x.NU_DOCUMENTO_EGRESO == v_nu_documento_EG
                    && x.NU_NIVEL == nivel
                    && x.TP_DOCUMENTO_INGRESO == "IP")
                .ToList();

            List<CuentaCorrienteCambioDoc> lista = new List<CuentaCorrienteCambioDoc>();
            foreach (var detalle in listaInsumosSemiacabado)
            {
                CuentaCorrienteCambioDoc cuenta = this._mapper.MapEntityToObject(detalle);
                lista.Add(cuenta);
            }

            return lista;
        }

        public virtual void DeleteCuentaCorrienteDocumento(string nuDocumentoEgreso)
        {
            var entities = this._context.T_PRDC_CUENTA_CAMBIO_DOC
                .Where(cdt => cdt.NU_DOCUMENTO_EGRESO == nuDocumentoEgreso && cdt.NU_NIVEL > 1);

            foreach (var entity in entities)
            {
                var attachedEntity = this._context.T_PRDC_CUENTA_CAMBIO_DOC.Local
                .FirstOrDefault(x => x.NU_DOCUMENTO_EGRESO == entity.NU_DOCUMENTO_EGRESO
                    && x.NU_DOCUMENTO_EGRESO_PRDC == entity.NU_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_EGRESO_PRDC == entity.TP_DOCUMENTO_EGRESO_PRDC
                    && x.TP_DOCUMENTO_INGRESO == entity.TP_DOCUMENTO_INGRESO
                    && x.NU_DOCUMENTO_INGRESO == entity.NU_DOCUMENTO_INGRESO
                    && x.TP_DOCUMENTO_INGRESO_ORIGINAL == entity.TP_DOCUMENTO_INGRESO_ORIGINAL
                    && x.NU_DOCUMENTO_INGRESO_ORIGINAL == entity.NU_DOCUMENTO_INGRESO_ORIGINAL
                    && x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PRODUTO == entity.CD_PRODUTO
                    && x.CD_FAIXA == entity.CD_FAIXA
                    && x.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && x.CD_PRODUTO_PRODUCIDO == entity.CD_PRODUTO_PRODUCIDO
                    && x.NU_NIVEL == entity.NU_NIVEL);

                if (attachedEntity != null)
                {
                    this._context.T_PRDC_CUENTA_CAMBIO_DOC.Remove(attachedEntity);
                }
                else
                {
                    this._context.T_PRDC_CUENTA_CAMBIO_DOC.Attach(entity);
                    this._context.T_PRDC_CUENTA_CAMBIO_DOC.Remove(entity);
                }
            }
        }

        #endregion

        public virtual List<DocumentoLineaDesafectada> ComprobarReservaDisponible(int empresa, string producto, decimal faixa, string identificador, decimal saldoADesreservar)
        {
            var detalles = new List<DocumentoLineaDesafectada>();

            var query = _context.T_DET_DOCUMENTO
                .Join(_context.T_DOCUMENTO,
                    dd => new { dd.NU_DOCUMENTO, dd.TP_DOCUMENTO },
                    d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO },
                    (dd, d) => new { Detalle = dd, Documento = d })
                .Join(_context.T_DOCUMENTO_TIPO_ESTADO.Where(dte => dte.FL_DISPONIBILIZA_STOCK == "S"),
                    d => new { d.Documento.TP_DOCUMENTO, d.Documento.ID_ESTADO },
                    dte => new { dte.TP_DOCUMENTO, dte.ID_ESTADO },
                    (d, dte) => new { Detalle = d.Detalle, Documento = d.Documento, TipoEstado = dte })
                .Join(_context.T_DOCUMENTO_TIPO.Where(dt => dt.FL_HABILITADO == "S" && dt.FL_DISPONIBILIZA_STOCK == "S"),
                    dte => dte.TipoEstado.TP_DOCUMENTO,
                    dt => dt.TP_DOCUMENTO,
                    (dte, dt) => new { Detalle = dte.Detalle }).AsNoTracking()
                .Where(x => x.Detalle.CD_EMPRESA == empresa && x.Detalle.CD_PRODUTO == producto && x.Detalle.CD_FAIXA == faixa && x.Detalle.NU_IDENTIFICADOR == identificador);

            var cantDisponible = query.Sum(x => (x.Detalle.QT_INGRESADA ?? 0) - (x.Detalle.QT_RESERVADA ?? 0) - (x.Detalle.QT_DESAFECTADA ?? 0));

            if (saldoADesreservar > cantDisponible)
                throw new ValidationFailedException("General_Sec0_Error_NoHaySaldoDisponibleDocumentos", new string[] { producto, identificador });

            detalles = query.Where(x => (x.Detalle.QT_INGRESADA ?? 0) - (x.Detalle.QT_RESERVADA ?? 0) - (x.Detalle.QT_DESAFECTADA ?? 0) > 0)
                    .OrderByDescending(x => (x.Detalle.QT_INGRESADA ?? 0) - (x.Detalle.QT_RESERVADA ?? 0) - (x.Detalle.QT_DESAFECTADA ?? 0))
                    .Select(x => _mapper.MapToDocumentoLineaDesafectada(x.Detalle)).ToList();

            return detalles;
        }

        public virtual bool ExisteReservaEquivalente(ModificarReservaDocumentalRequest request, out List<DocumentoPreparacionReserva> reservas)
        {
            bool result = false;
            reservas = new List<DocumentoPreparacionReserva>();

            var query = _context.T_DOCUMENTO_PREPARACION_RESERV
                .AsNoTracking()
                .Where(d => d.NU_PREPARACION == request.Preparacion
                    && d.CD_EMPRESA == request.Empresa
                    && d.CD_PRODUTO == request.Producto
                    && d.CD_FAIXA == request.Faixa
                    && d.NU_IDENTIFICADOR_PICKING_DET == request.Identificador);

            var cantReservadaTotalLote = query.Sum(d => (d.QT_PRODUTO ?? 0) - (d.QT_PREPARADO ?? 0));

            if (cantReservadaTotalLote >= request.Cantidad)
                result = true;
            else
            {
                result = false;
                reservas = query.Where(d => (d.QT_PRODUTO ?? 0) - (d.QT_PREPARADO ?? 0) > 0)
                    .OrderByDescending(d => (d.QT_PRODUTO ?? 0) - (d.QT_PREPARADO ?? 0))
                    .Select(x => _mapper.MapToDocumentoReserva(x, _service)).ToList();
            }

            return result;
        }

        public virtual bool ExisteReservaEquivalente(ModificarReservaDocumentalRequest request, out decimal cantReservadaTotalLote)
        {
            cantReservadaTotalLote = _context.T_DOCUMENTO_PREPARACION_RESERV
                .AsNoTracking()
                .Where(d => d.NU_PREPARACION == request.Preparacion
                    && d.CD_EMPRESA == request.Empresa
                    && d.CD_PRODUTO == request.Producto
                    && d.CD_FAIXA == request.Faixa
                    && d.NU_IDENTIFICADOR_PICKING_DET == request.Identificador)
                .Sum(d => (d.QT_PRODUTO ?? 0) - (d.QT_PREPARADO ?? 0));

            if (cantReservadaTotalLote >= request.Cantidad)
                return true;
            else
                return false;
        }

        public virtual List<DocumentoPreparacionReserva> GetReservasDistintoLote(int preparacion, int empresa, string producto, decimal? faixa, string identificador)
        {
            return _context.T_DOCUMENTO_PREPARACION_RESERV
                .Include("T_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DOCUMENTO_TIPO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DET_DOCU_EGRESO_RESERV")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_EGRESO.T_DOCUMENTO_INGRESO.T_DET_DOCUMENTO")
                .Include("T_DOCUMENTO.T_DET_DOCUMENTO_ACTA")
                .AsNoTracking()
                .Where(d => d.NU_PREPARACION == preparacion
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR_PICKING_DET != identificador)
                .OrderByDescending(d => (d.QT_PRODUTO ?? 0) - (d.QT_PREPARADO ?? 0))
                .Select(d => _mapper.MapToDocumentoReserva(d, this._service))
                .ToList();
        }

        #region Cross-docking documental

        public virtual Dictionary<string, Dictionary<string, Dictionary<string, DocumentoLineaDesafectada>>> GetDocumentosDisponibles(int nuAgenda, bool consumirOtrosDocs)
        {
            var result = new Dictionary<string, Dictionary<string, Dictionary<string, DocumentoLineaDesafectada>>>();

            var prodLotes = _context.T_ETIQUETA_LOTE
                .Where(e => e.NU_AGENDA == nuAgenda && e.CD_SITUACAO == SituacionDb.PalletConferido)
                .Join(_context.T_DET_ETIQUETA_LOTE,
                e => e.NU_ETIQUETA_LOTE,
                de => de.NU_ETIQUETA_LOTE,
                (e, de) => de)
                .GroupBy(e => new { e.CD_PRODUTO, e.NU_IDENTIFICADOR })
                .AsNoTracking()
                .Select(g => g.Key);

            IQueryable<T_DOCUMENTO> documentos = _context.T_DOCUMENTO;
            if (!consumirOtrosDocs)
                documentos = documentos.Where(d => d.NU_AGENDA == nuAgenda);

            var query = _context.T_DET_DOCUMENTO
            .Join(documentos,
                dd => new { dd.NU_DOCUMENTO, dd.TP_DOCUMENTO },
                d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO },
                (dd, d) => new { Detalle = dd, Documento = d })
            .Join(_context.T_DOCUMENTO_TIPO_ESTADO.Where(dte => dte.FL_DISPONIBILIZA_STOCK == "S"),
                d => new { d.Documento.TP_DOCUMENTO, d.Documento.ID_ESTADO },
                dte => new { dte.TP_DOCUMENTO, dte.ID_ESTADO },
                (d, dte) => new { Detalle = d.Detalle, Documento = d.Documento, TipoEstado = dte })
            .Join(_context.T_DOCUMENTO_TIPO.Where(dt => dt.FL_HABILITADO == "S" && dt.FL_DISPONIBILIZA_STOCK == "S"),
                dte => dte.TipoEstado.TP_DOCUMENTO,
                dt => dt.TP_DOCUMENTO,
                (dte, dt) => new { Detalle = dte.Detalle })
            .Join(prodLotes,
            dte => new { dte.Detalle.CD_PRODUTO, dte.Detalle.NU_IDENTIFICADOR },
            pl => new { pl.CD_PRODUTO, pl.NU_IDENTIFICADOR },
            (dte, pl) => new { Detalle = dte.Detalle, CD_PRODUTO = pl.CD_PRODUTO, NU_IDENTIFICADOR = pl.NU_IDENTIFICADOR })
            .Where(x => (x.Detalle.QT_INGRESADA ?? 0) - (x.Detalle.QT_RESERVADA ?? 0) - (x.Detalle.QT_DESAFECTADA ?? 0) > 0)
            .OrderByDescending(x => (x.Detalle.QT_INGRESADA ?? 0) - (x.Detalle.QT_RESERVADA ?? 0) - (x.Detalle.QT_DESAFECTADA ?? 0))
            .AsNoTracking();

            var dodedf = query.ToList();

            foreach (var d in query)
            {
                if (!result.ContainsKey(d.CD_PRODUTO))
                    result[d.CD_PRODUTO] = new Dictionary<string, Dictionary<string, DocumentoLineaDesafectada>>();

                if (!result[d.CD_PRODUTO].ContainsKey(d.NU_IDENTIFICADOR))
                    result[d.CD_PRODUTO][d.NU_IDENTIFICADOR] = new Dictionary<string, DocumentoLineaDesafectada>();

                var keyDoc = $"{d.Detalle.NU_DOCUMENTO}@{d.Detalle.TP_DOCUMENTO}";
                result[d.CD_PRODUTO][d.NU_IDENTIFICADOR][keyDoc] = _mapper.MapToDocumentoLineaDesafectada(d.Detalle);
            }

            return result;
        }

        #endregion

        #region Dapper
        public virtual IEnumerable<DocumentoPreparacionReserva> GetReservasPreparacion(IEnumerable<Preparacion> preparaciones)
        {
            IEnumerable<DocumentoPreparacionReserva> resultado = new List<DocumentoPreparacionReserva>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_TEMP (NU_PREPARACION) VALUES (:Id)";
                    _dapper.Execute(connection, sql, preparaciones, transaction: tran);

                    sql = GetSqlSelectPreparacionReserva() +
                    @" INNER JOIN T_PICKING_TEMP T ON D.NU_PREPARACION = T.NU_PREPARACION ";

                    resultado = _dapper.Query<DocumentoPreparacionReserva>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectPreparacionReserva()
        {
            return @"SELECT
	                    D.NU_DOCUMENTO as NroDocumento,
	                    D.TP_DOCUMENTO as TipoDocumento,
	                    D.NU_PREPARACION as Preparacion,
	                    D.CD_EMPRESA as Empresa,
	                    D.CD_PRODUTO as Producto,
	                    D.CD_FAIXA as Faixa,
	                    D.NU_IDENTIFICADOR as Identificador,
	                    D.ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
	                    D.NU_IDENTIFICADOR_PICKING_DET as NroIdentificadorPicking,
	                    D.QT_PRODUTO as CantidadProducto,
	                    D.QT_ANULAR as CantidadAnular,
	                    D.QT_PREPARADO as CantidadPreparada,
	                    D.DT_ADDROW as FechaAlta,
	                    D.DT_UPDROW as FechaModificacion
                    FROM T_DOCUMENTO_PREPARACION_RESERV D ";
        }

        public virtual void AddPreparacionReservaTemp(DbConnection connection, DbTransaction tran, List<DocumentoPreparacionReserva> preparacionReserva)
        {
            _dapper.BulkInsert(connection, tran, preparacionReserva, "T_DOC_PREPARACION_RESERV_TEMP", new Dictionary<string, Func<DocumentoPreparacionReserva, ColumnInfo>>
            {
                { "NU_DOCUMENTO", x => new ColumnInfo(x.NroDocumento)},
                { "TP_DOCUMENTO", x => new ColumnInfo(x.TipoDocumento)},
                { "NU_PREPARACION", x => new ColumnInfo(x.Preparacion)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                { "QT_PRODUTO", x => new ColumnInfo(x.CantidadProducto)},
                { "ID_ESPECIFICA_IDENTIFICADOR", x => new ColumnInfo(x.EspecificaIdentificadorId)},
                { "NU_IDENTIFICADOR_PICKING_DET", x => new ColumnInfo(x.NroIdentificadorPicking)},
                { "VL_DATO_AUDITORIA", x => new ColumnInfo(x.Auditoria,DbType.String) },
                { "NU_SECUENCIA", x => new ColumnInfo(x.Secuencia,DbType.Int32)}
            });
        }

        public virtual void AddPreparacionReserva(DbConnection connection, DbTransaction tran)
        {
            string sql = @"INSERT INTO T_DOCUMENTO_PREPARACION_RESERV
                        (NU_DOCUMENTO,
                        TP_DOCUMENTO,
                        NU_PREPARACION,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        NU_IDENTIFICADOR_PICKING_DET,
                        QT_PRODUTO,
                        QT_ANULAR,
                        VL_DATO_AUDITORIA,
                        QT_PREPARADO,
                        DT_ADDROW) 
                        (SELECT  
                            dprt.NU_DOCUMENTO,                          
                            dprt.TP_DOCUMENTO,                     
                            dprt.NU_PREPARACION,                     
                            dprt.CD_EMPRESA,                       
                            dprt.CD_PRODUTO,                                           
                            dprt.CD_FAIXA,                             
                            dprt.NU_IDENTIFICADOR,                  
                            dprt.ID_ESPECIFICA_IDENTIFICADOR,         
                            dprt.NU_IDENTIFICADOR_PICKING_DET,               
                            dprt.QT_PRODUTO,             
                            0 QT_ANULAR,            
                            dprt.VL_DATO_AUDITORIA,                    
                            0 QT_PREPARADO,            
                            :FechaAlta
                        FROM T_DOC_PREPARACION_RESERV_TEMP dprt
                        LEFT JOIN T_DOCUMENTO_PREPARACION_RESERV dpr on   dpr.NU_DOCUMENTO = dprt.NU_DOCUMENTO AND
                        dpr.TP_DOCUMENTO = dprt.TP_DOCUMENTO AND
                        dpr.NU_PREPARACION = dprt.NU_PREPARACION AND
                        dpr.CD_EMPRESA = dprt.CD_EMPRESA AND
                        dpr.CD_PRODUTO = dprt.CD_PRODUTO AND
                        dpr.CD_FAIXA = dprt.CD_FAIXA AND
                        dpr.NU_IDENTIFICADOR = dprt.NU_IDENTIFICADOR 
                        WHERE dpr.NU_DOCUMENTO is null
                        )";

            _dapper.Execute(connection, sql, param: new { FechaAlta = DateTime.Now }, transaction: tran);
        }

        public virtual void UpdatePreparacionReserva(DbConnection connection, DbTransaction tran)
        {
            var alias = "dpr";
            var from = $@"
                T_DOCUMENTO_PREPARACION_RESERV dpr
                INNER JOIN (
                    SELECT 
                        dpr.NU_DOCUMENTO,
                        dpr.TP_DOCUMENTO,
                        dpr.NU_PREPARACION,
                        dpr.CD_EMPRESA,
                        dpr.CD_PRODUTO,
                        dpr.CD_FAIXA,
                        dpr.NU_IDENTIFICADOR,
                        SUM(dpt.QT_PRODUTO) QT_PRODUTO_TEMP
                    FROM  T_DOC_PREPARACION_RESERV_TEMP  dpt
                    INNER JOIN T_DOCUMENTO_PREPARACION_RESERV dpr on 
                        dpr.NU_DOCUMENTO = dpt.NU_DOCUMENTO AND
                        dpr.TP_DOCUMENTO = dpt.TP_DOCUMENTO AND
                        dpr.NU_PREPARACION = dpt.NU_PREPARACION AND
                        dpr.CD_EMPRESA = dpt.CD_EMPRESA AND
                        dpr.CD_PRODUTO = dpt.CD_PRODUTO AND
                        dpr.CD_FAIXA = dpt.CD_FAIXA AND
                        dpr.NU_IDENTIFICADOR = dpt.NU_IDENTIFICADOR 
                    GROUP by  dpr.NU_DOCUMENTO,
                        dpr.TP_DOCUMENTO,
                        dpr.NU_PREPARACION,
                        dpr.CD_EMPRESA,
                        dpr.CD_PRODUTO,
                        dpr.CD_FAIXA,
                        dpr.NU_IDENTIFICADOR
                ) dprt ON dpr.NU_DOCUMENTO = dprt.NU_DOCUMENTO AND
                        dpr.TP_DOCUMENTO = dprt.TP_DOCUMENTO AND
                        dpr.NU_PREPARACION = dprt.NU_PREPARACION AND
                        dpr.CD_EMPRESA = dprt.CD_EMPRESA AND
                        dpr.CD_PRODUTO = dprt.CD_PRODUTO AND
                        dpr.CD_FAIXA = dprt.CD_FAIXA AND
                        dpr.NU_IDENTIFICADOR = dprt.NU_IDENTIFICADOR";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                QT_PRODUTO = QT_PRODUTO + QT_PRODUTO_TEMP";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDocumentoReserva(DbConnection connection, DbTransaction tran)
        {
            var alias = "dpr";
            var from = $@"
                T_DET_DOCUMENTO dpr
                INNER JOIN (
                    SELECT 
                        dpt.NU_DOCUMENTO,
                        dpt.TP_DOCUMENTO,
                        dpt.CD_EMPRESA,
                        dpt.CD_PRODUTO,
                        dpt.CD_FAIXA,
                        dpt.NU_IDENTIFICADOR,
                        SUM(dpt.QT_PRODUTO) QT_PRODUTO_TEMP
                    FROM  T_DOC_PREPARACION_RESERV_TEMP  dpt
                    INNER JOIN T_DET_DOCUMENTO dpr on 
                        dpr.NU_DOCUMENTO = dpt.NU_DOCUMENTO AND
                        dpr.TP_DOCUMENTO = dpt.TP_DOCUMENTO AND
                        dpr.CD_EMPRESA = dpt.CD_EMPRESA AND
                        dpr.CD_PRODUTO = dpt.CD_PRODUTO AND
                        dpr.CD_FAIXA = dpt.CD_FAIXA AND
                        dpr.NU_IDENTIFICADOR = dpt.NU_IDENTIFICADOR 
                    GROUP BY dpt.NU_DOCUMENTO,
                        dpt.TP_DOCUMENTO,
                        dpt.CD_EMPRESA,
                        dpt.CD_PRODUTO,
                        dpt.CD_FAIXA,
                        dpt.NU_IDENTIFICADOR
                ) dprt ON dpr.NU_DOCUMENTO = dprt.NU_DOCUMENTO AND
                        dpr.TP_DOCUMENTO = dprt.TP_DOCUMENTO AND
                        dpr.CD_EMPRESA = dprt.CD_EMPRESA AND
                        dpr.CD_PRODUTO = dprt.CD_PRODUTO AND
                        dpr.CD_FAIXA = dprt.CD_FAIXA AND
                        dpr.NU_IDENTIFICADOR = dprt.NU_IDENTIFICADOR";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                QT_RESERVADA = COALESCE(QT_RESERVADA,0) + QT_PRODUTO_TEMP";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion
    }
}

