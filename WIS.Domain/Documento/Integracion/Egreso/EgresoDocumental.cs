using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Serializables;
using WIS.Domain.Documento.Serializables.Salida;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Exceptions;

namespace WIS.Domain.Documento.Integracion.Egreso
{
    public class EgresoDocumental
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IFactoryService _factoryService;

        public EgresoDocumental(IFactoryService factoryService)
        {
            this._factoryService = factoryService;
        }

        public virtual IDocumentoEgreso GenerarEgresoDocumental(IUnitOfWork uow, int usuario, int cdCamion, string tipoDocumento, string numeroDocumento)
        {
            try
            {
                var nuTransaccion = uow.GetTransactionNumber();
                var documentoEgreso = ValidarEgresoDocumental(uow, cdCamion, out List<LineaEgresoDocumental> lineas, out bool cancelarDocumento);
                var edicion = documentoEgreso != null;
                var camion = uow.CamionRepository.GetCamion(cdCamion);
                var empresa = camion.Empresa.Value;

                if (edicion)
                    RecomponerReservaDocumental(uow, documentoEgreso);
                else
                    documentoEgreso = CrearCabezalEgreso(uow, usuario, empresa, cdCamion, tipoDocumento, numeroDocumento);

                if (cancelarDocumento)
                {
                    documentoEgreso.Estado = uow.DocumentoRepository.GetEstadoDestino(documentoEgreso.Tipo, AccionDocumento.Cancelar);
                    documentoEgreso.Cancelar();
                    uow.DocumentoRepository.UpdateEgreso(documentoEgreso, nuTransaccion);
                    camion.Documento = null;
                }
                else
                {
                    GenerarLineasEgresoConReserva(documentoEgreso, lineas, uow, usuario, out DocumentoReservaDesafectada infoReservas);

                    if (edicion)
                    {
                        foreach (var linea in documentoEgreso.OutDetail)
                        {
                            uow.DocumentoRepository.AddLineaEgreso(documentoEgreso, linea, nuTransaccion);
                        }
                    }
                    else
                    {
                        uow.DocumentoRepository.AddEgreso(documentoEgreso, nuTransaccion);
                    }

                    var reservasAfectadas = new List<DocumentoPreparacionReserva>();
                    
                    reservasAfectadas.AddRange(infoReservas.ReservasModificadas);
                    reservasAfectadas.AddRange(infoReservas.ReservasEliminadas);

                    foreach (var reserva in reservasAfectadas)
                    {
                        uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                    }

                    uow.SaveChanges();

                    foreach (var reserva in infoReservas.ReservasEliminadas)
                    {
                        uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
                    }

                    GenerarCuentaCorrienteInsumo(uow, documentoEgreso);

                    camion.Documento = string.Format("{0} {1}", documentoEgreso.Tipo, documentoEgreso.Numero);
                    GenerarCargasPredefinidas(uow, cdCamion);
                }

                uow.CamionRepository.UpdateCamion(camion);
                return documentoEgreso;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual void RecomponerReservaDocumental(IUnitOfWork uow, IDocumentoEgreso documentoEgreso)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            foreach (var lineaEgreso in documentoEgreso.OutDetail)
            {
                var lineaIngreso = uow.DocumentoRepository.GetLineaIngreso(lineaEgreso.NumeroDocumentoIngreso, lineaEgreso.TpDocumentoIngreso, lineaEgreso.Producto, lineaEgreso.Identificador);

                lineaIngreso.CantidadDesafectada -= lineaEgreso.CantidadDesafectada;
                lineaIngreso.CantidadReservada += lineaEgreso.CantidadDesafectada;

                if (lineaEgreso.Reservas.Count == 0)
                    throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalSinReserva");

                foreach (var lineaReserva in lineaEgreso.Reservas)
                {
                    var reserva = uow.DocumentoRepository.GetPreparacionReserva(lineaReserva.NumeroDocumentoIngreso, lineaReserva.TipoDocumentoIngreso, lineaReserva.Preparacion, lineaReserva.Empresa, lineaReserva.Producto, lineaReserva.Faixa, lineaReserva.IdentificadorPicking);
                    var regenerarReserva = false;

                    if (reserva == null)
                    {
                        reserva = uow.DocumentoRepository.GetPreparacionReservaDesafectada(lineaReserva.NumeroDocumentoIngreso, lineaReserva.TipoDocumentoIngreso, lineaReserva.Preparacion, lineaReserva.Empresa, lineaReserva.Producto, lineaReserva.Faixa, lineaReserva.IdentificadorPicking, lineaReserva.Identificador);
                        regenerarReserva = true;
                    }

                    reserva.NumeroTransaccion = nuTransaccion;
                    reserva.NumeroTransaccionDelete = null;
                    reserva.AfectarCantidadProducto(lineaReserva.CantidadAfectada);

                    if (regenerarReserva)
                    {
                        uow.DocumentoRepository.RemoveDocumentoPreparacionReservaDesafectada(reserva);
                        uow.DocumentoRepository.AddPreparacionReserva(reserva);
                    }
                    else
                    {
                        uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                    }
                }

                uow.DocumentoRepository.UpdateDetailWithoutDocument(lineaEgreso.NumeroDocumentoIngreso, lineaEgreso.TpDocumentoIngreso, lineaIngreso, nuTransaccion);
            }

            uow.DocumentoRepository.RemoveEgresoDetails(documentoEgreso, nuTransaccion);
            uow.SaveChanges();
        }

        public virtual IDocumentoEgreso ValidarEgresoDocumental(IUnitOfWork uow, int camion, out List<LineaEgresoDocumental> lineas, out bool cancelarDocumento)
        {
            cancelarDocumento = false;
            lineas = new List<LineaEgresoDocumental>();

            var documentoEgreso = uow.DocumentoRepository.GetEgresoPorCamion(camion);
            if (documentoEgreso != null && !uow.DocumentoTipoRepository.PermiteEditarCamion(documentoEgreso.Tipo, documentoEgreso.Estado))
                throw new ValidationFailedException("General_Sec0_Error_DocumentoEgresoNoEditable");

            if (!uow.CamionRepository.AnyCargaAsociada(camion) && documentoEgreso != null)
            {
                var estadoDestino = uow.DocumentoRepository.GetEstadoDestino(documentoEgreso.Tipo, AccionDocumento.Cancelar);
                if (estadoDestino == null)
                    throw new ValidationFailedException("General_Sec0_Error_ImposbileCancelarEgreso");

                cancelarDocumento = true;
            }
            else
            {
                if (!uow.CamionRepository.AnyCargaAsociada(camion))
                    throw new ValidationFailedException("General_Sec0_Error_Er223_CamionSinMercaderiaASociada");

                if (uow.DocumentoRepository.AnyCargaSinPreparar(camion))
                    throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalCamionSinPreparar");

                lineas = uow.DocumentoRepository.GetLineasEgreso(camion);

                if (lineas.Count == 0)
                    throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalCamionSinlineasDoc");
            }
            return documentoEgreso;
        }

        public virtual void GenerarCuentaCorrienteInsumo(IUnitOfWork uow, IDocumentoEgreso documentoEgreso)
        {
            string v_nu_documento_EG = documentoEgreso.Numero;
            int x = 1;

            uow.DocumentoRepository.GenerarCuentaCorrienteInsumo(v_nu_documento_EG, documentoEgreso.Tipo);

            while (true) // Itero en los detalles de documento para buscar lineas que desafecten de IP
            {
                List<CuentaCorriente> lista = uow.DocumentoRepository.GetCuentaDocumentoIPNivel(v_nu_documento_EG, documentoEgreso.Tipo, x);

                if (lista.Count == 0)
                {
                    break;
                }
                else
                {
                    foreach (var curDetEgreso in lista)
                    {
                        int v_cd_empresa_EG = curDetEgreso.CD_EMPRESA;
                        string v_cd_producto_EG = curDetEgreso.CD_PRODUTO;
                        decimal v_cd_faixa_EG = curDetEgreso.CD_FAIXA;
                        string v_nu_identificador_EG = curDetEgreso.NU_IDENTIFICADOR;
                        decimal v_qt_egresado = curDetEgreso.QT_MOVIMIENTO ?? 0;


                        curDocProduccion curDocProduccion = uow.DocumentoRepository.GetPrdcIngresoSaldo(curDetEgreso.NU_DOCUMENTO_INGRESO, curDetEgreso.TP_DOCUMENTO_INGRESO);
                        string v_nu_prdc_ingreso = curDocProduccion.NU_PRDC_INGRESO;
                        string v_nu_documento_EP = curDocProduccion.NU_DOCUMENTO_EGR;
                        string v_tp_documento_EP = curDocProduccion.TP_DOCUMENTO_EGR;
                        decimal v_qt_ingresado_producido = curDocProduccion.QT_INGRESADO ?? 0;

                        List<DocumentoLineaEgreso> ListacurPRDCEntrada = uow.DocumentoRepository.GetEgresoDocumento(v_tp_documento_EP, curDetEgreso.NU_DOCUMENTO_INGRESO);
                        foreach (var curPRDCEntrada in ListacurPRDCEntrada)
                        {
                            decimal v_qt_insumo = (curDetEgreso.QT_MOVIMIENTO ?? 0) * (curPRDCEntrada.CantidadDesafectada ?? 0) / (curDetEgreso.QT_DECLARADA_ORIGINAL ?? 1);

                            int v_cd_empresa_EP = curPRDCEntrada.Empresa;
                            string v_cd_producto_EP = curPRDCEntrada.Producto;
                            decimal v_cd_faixa_EP = curPRDCEntrada.Faixa;
                            List<PrdcSaldo> listssaldo = uow.DocumentoRepository.GetPrdcSaldo(v_nu_documento_EP, v_tp_documento_EP, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP);

                            foreach (var curDetEP in listssaldo)
                            {
                                decimal v_qt_movimiento;

                                if (v_qt_insumo > 0)
                                {
                                    if (curDetEP.QT_SALDO <= v_qt_insumo)
                                    {
                                        v_qt_movimiento = curDetEP.QT_SALDO ?? 0;
                                    }
                                    else
                                    {
                                        v_qt_movimiento = v_qt_insumo;
                                    }

                                    v_qt_insumo = v_qt_insumo - v_qt_movimiento;

                                    string v_tp_documento_ingreso_orig = curDetEP.TP_DOCUMENTO_INGRESO;
                                    string v_nu_documento_ingreso_orig = curDetEP.NU_DOCUMENTO_INGRESO;
                                    string v_nu_identificador_EP = curDetEP.NU_IDENTIFICADOR;
                                    decimal v_qt_ingresada_doc_Origen = uow.DocumentoRepository.GetQtIngresadaDocOrigen(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP, v_nu_identificador_EP);
                                    decimal v_qt_ingresada_doc_Origen_acta = 0;

                                    if (curDetEP.TP_DOCUMENTO_INGRESO == "A")
                                    {
                                        uow.DocumentoRepository.GetDocumentoActa(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO, out v_tp_documento_ingreso_orig, out v_nu_documento_ingreso_orig);
                                        v_qt_ingresada_doc_Origen_acta = uow.DocumentoRepository.GetCantidadActa(v_nu_documento_ingreso_orig, v_tp_documento_ingreso_orig, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP, v_nu_identificador_EP);
                                    }
                                    else
                                    {
                                        v_qt_ingresada_doc_Origen_acta = uow.DocumentoRepository.GetDocOriQTIngreso(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP, v_nu_identificador_EP);
                                    }
                                    if (curDetEP.TP_DOCUMENTO_INGRESO == "IP")
                                    {
                                        v_qt_ingresada_doc_Origen = uow.DocumentoRepository.GetQtIngresadaDocOrigen(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO);
                                    }

                                    decimal v_qt_declarada_original = v_qt_ingresada_doc_Origen + (v_qt_ingresada_doc_Origen_acta);

                                    CuentaCorriente new_insumo = new CuentaCorriente();
                                    new_insumo.NU_DOCUMENTO_EGRESO = v_nu_documento_EG;
                                    new_insumo.TP_DOCUMENTO_EGRESO = documentoEgreso.Tipo;
                                    new_insumo.NU_DOCUMENTO_EGRESO_PRDC = v_nu_documento_EP;
                                    new_insumo.TP_DOCUMENTO_EGRESO_PRDC = v_tp_documento_EP;
                                    new_insumo.TP_DOCUMENTO_INGRESO = curDetEP.TP_DOCUMENTO_INGRESO;
                                    new_insumo.NU_DOCUMENTO_INGRESO = curDetEP.NU_DOCUMENTO_INGRESO;
                                    new_insumo.TP_DOCUMENTO_INGRESO_ORIGINAL = v_tp_documento_ingreso_orig;
                                    new_insumo.NU_DOCUMENTO_INGRESO_ORIGINAL = v_nu_documento_ingreso_orig;
                                    new_insumo.QT_DECLARADA_ORIGINAL = v_qt_declarada_original;
                                    new_insumo.CD_EMPRESA = v_cd_empresa_EP;
                                    new_insumo.CD_PRODUTO = v_cd_producto_EP;
                                    new_insumo.CD_FAIXA = v_cd_faixa_EP;
                                    new_insumo.NU_IDENTIFICADOR = v_nu_identificador_EP;
                                    new_insumo.QT_MOVIMIENTO = v_qt_movimiento;
                                    new_insumo.CD_PRODUTO_PRODUCIDO = v_cd_producto_EG;
                                    new_insumo.NU_NIVEL = x + 1;

                                    CuentaCorriente new_insu = uow.DocumentoRepository.GetCuentaCorriente(new_insumo);

                                    if (new_insu != null)
                                    {
                                        new_insu.QT_MOVIMIENTO = new_insu.QT_MOVIMIENTO + v_qt_movimiento;
                                        uow.DocumentoRepository.UpdateCuentaCorrienteDocumento(new_insu);
                                    }
                                    else
                                    {
                                        uow.DocumentoRepository.AddCuentaCorrienteInsumo(new_insumo);
                                    }
                                }
                            }
                        }

                        x = x + 1;
                    }
                }
            }
        }

        public virtual CambioDocInt CambioPreRegIngreso(IUnitOfWork uow, CambioDocInt request)
        {
            if (request.QT_CAMBIO < 0)
            {
                request.success = false;
                request.errorMsg = "El número debe ser positivo";
            }
            else if (!uow.DocumentoRepository.TieneSaldoDocumental(request))
            {
                request.success = false;
                request.errorMsg = string.Format("El documento {0} Tipo {1} Producto {2} Lote {3} no tiene suficiente saldo ", request.NU_DOCUMENTO, request.TP_DOCUMENTO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);
            }
            else
            {
                uow.DocumentoRepository.AddPreCambioDoc(request);
            }

            return request;
        }

        public virtual DocumentoEgresoIngreso ValidarCambioIngreso(IUnitOfWork uow, DocumentoEgresoIngreso request, bool validar = false)
        {
            try
            {
                IDocumento doc = uow.DocumentoRepository.GetIngreso(request.NU_DOC_NUEVO, request.TP_DOCUMENTO);
                if (doc != null)
                {
                    if (validar)
                    {
                        string descripcion = request.NU_EGRESO + "#" + request.TP_DOCUMENTO_EGR;
                        if (!doc.Descripcion.Contains(descripcion))
                        {
                            throw new Exception("General_Sec0_Error_Error86");
                        }
                    }
                    request.existeDocumento = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                request.success = false;
                request.errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw ex;
            }

            return request;
        }

        public virtual DocumentoEgresoIngreso ValidarCambioIngr(IUnitOfWork uow, DocumentoEgresoIngreso request, bool validar = false)
        {
            try
            {
                IDocumento doc = uow.DocumentoRepository.GetIngreso(request.NU_DOC_NUEVO, request.TP_DOCUMENTO);
                if (doc != null)
                {
                    if (validar)
                    {
                        string descripcion = request.FechaControl + "#" + request.NU_DOC_NUEVO;
                        if (!doc.Descripcion.Contains(descripcion))
                        {
                            throw new Exception("General_Sec0_Error_Error86");
                        }
                    }
                    request.existeDocumento = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                request.success = false;
                request.errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw ex;
            }

            return request;
        }

        public virtual bool ValidarCambioIngr(IUnitOfWork uow, string FechaControl, string NU_DOC_NUEVO, string TP_DOCUMENTO, string aplicacion, int usuario, bool validar = true)
        {
            bool existe = false;
            IDocumento doc = uow.DocumentoRepository.GetIngreso(NU_DOC_NUEVO, TP_DOCUMENTO);

            if (doc != null)
            {
                if (validar)
                {
                    string descripcion = FechaControl + "#" + NU_DOC_NUEVO;
                    if (!doc.Descripcion.Contains(descripcion))
                    {
                        throw new Exception("General_Sec0_Error_Error86");
                    }
                }
                existe = true;
            }

            return existe;
        }

        public virtual void CambioIngresoDOC400(IUnitOfWork uow, CambioDocumentoDetIngreso request, string descripcion = "")
        {
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                IDocumentoIngreso documento = null;

                if (!request.existeDoc)
                {
                    documento = this.CrearDocumentoObject(request, uow, descripcion);
                    uow.DocumentoRepository.AddIngreso(documento, nuTransaccion);
                }
                else
                {
                    documento = uow.DocumentoRepository.GetIngreso(request.NU_DOCUMENTO_NUEVO, request.TP_DOCUMENTO_NUEVO);
                }

                DocumentoLinea linea = uow.DocumentoRepository.GetLineaIngreso(request.NU_DOCUMENTO_NUEVO, request.TP_DOCUMENTO_NUEVO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);

                if (linea != null)
                {
                    linea.CantidadIngresada = linea.CantidadIngresada + request.QT_MOVIMIENTO;
                    uow.DocumentoRepository.UpdateLineaDocumento(linea, documento, nuTransaccion);
                }
                else
                {
                    linea = new DocumentoLinea();
                    linea.Producto = request.CD_PRODUTO;
                    linea.Empresa = request.CD_EMPRESA;
                    linea.Identificador = request.NU_IDENTIFICADOR;
                    linea.Faixa = request.CD_FAIXA;
                    linea.CantidadIngresada = request.QT_MOVIMIENTO;
                    uow.DocumentoRepository.AddLineaDocumento(documento, linea, nuTransaccion);
                }

                if (!uow.DocumentoRepository.ExisteLogCambioDoc(request))
                {
                    CambioDocDet det = new CambioDocDet();
                    det.CD_PRODUTO = request.CD_PRODUTO;
                    det.CD_FAIXA = request.CD_FAIXA;
                    det.NU_IDENTIFICADOR = request.NU_IDENTIFICADOR;
                    det.CD_EMPRESA = request.CD_EMPRESA;
                    det.NU_DOCUMENTO = request.NU_DOCUMENTO_INGRESO;
                    det.TP_DOCUMENTO = request.TP_DOCUMENTO_INGRESO;
                    det.NU_DOCUMENTO_CAMBIO = request.NU_DOCUMENTO_NUEVO;
                    det.TP_DOCUMENTO_CAMBIO = request.TP_DOCUMENTO_NUEVO;
                    uow.DocumentoRepository.AddLogCambioIngreso(det);
                }

                DocumentoLinea detalleupdate = uow.DocumentoRepository.GetLineaIngreso(request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);
                detalleupdate.CantidadIngresada = detalleupdate.CantidadIngresada - request.QT_MOVIMIENTO;

                IDocumentoIngreso documento1 = null;
                documento1 = uow.DocumentoRepository.GetIngreso(request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO);

                uow.DocumentoRepository.UpdateDetail(documento1, detalleupdate, nuTransaccion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual void PreCambioIngreso(CambioDocumentoDetIngreso request, IUnitOfWork uow, long nuTransaccion)
        {
            try
            {
                IDocumentoIngreso documento = null;

                if (request.existeDoc == false)
                {
                    documento = this.CrearDocumentoObject(request, uow);
                    uow.DocumentoRepository.AddIngreso(documento, nuTransaccion);
                }
                else
                {
                    documento = uow.DocumentoRepository.GetIngreso(request.NU_DOCUMENTO_NUEVO, request.TP_DOCUMENTO_NUEVO);
                }

                DocumentoLinea linea = uow.DocumentoRepository.GetLineaIngreso(request.NU_DOCUMENTO_NUEVO, request.TP_DOCUMENTO_NUEVO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);

                if (linea != null)
                {
                    linea.CantidadDesafectada = linea.CantidadDesafectada + request.QT_MOVIMIENTO;
                    linea.CantidadIngresada = linea.CantidadIngresada + request.QT_MOVIMIENTO;
                    uow.DocumentoRepository.UpdateLineaDocumento(linea, documento, nuTransaccion);
                }
                else
                {
                    linea = new DocumentoLinea();
                    linea.Producto = request.CD_PRODUTO;
                    linea.Empresa = request.CD_EMPRESA;
                    linea.Identificador = request.NU_IDENTIFICADOR;
                    linea.Faixa = request.CD_FAIXA;
                    linea.CantidadDesafectada = request.QT_MOVIMIENTO;
                    linea.CantidadIngresada = request.QT_MOVIMIENTO;
                    uow.DocumentoRepository.AddLineaDocumento(documento, linea, nuTransaccion);
                }

                CuentaCorriente cuenta = uow.DocumentoRepository.GetCuentaCorrienteDocumento(request);
                cuenta.NU_DOCUMENTO_CAMBIO = request.NU_DOCUMENTO_NUEVO;
                cuenta.TP_DOCUMENTO_CAMBIO = request.TP_DOCUMENTO_NUEVO;
                cuenta.DT_UPDROW = DateTime.Now;
                cuenta.CD_FUNCIONARIO = request.userId;
                uow.DocumentoRepository.UpdateCuentaCorrienteDocumento(cuenta);

                if (!uow.DocumentoRepository.ExisteLogCambioDoc(request))
                {
                    CambioDocDet det = new CambioDocDet();
                    det.CD_PRODUTO = request.CD_PRODUTO;
                    det.CD_FAIXA = request.CD_FAIXA;
                    det.NU_IDENTIFICADOR = request.NU_IDENTIFICADOR;
                    det.CD_EMPRESA = request.CD_EMPRESA;
                    det.NU_DOCUMENTO = request.NU_DOCUMENTO_INGRESO;
                    det.TP_DOCUMENTO = request.TP_DOCUMENTO_INGRESO;
                    det.NU_DOCUMENTO_CAMBIO = request.NU_DOCUMENTO_NUEVO;
                    det.TP_DOCUMENTO_CAMBIO = request.TP_DOCUMENTO_NUEVO;
                    uow.DocumentoRepository.AddLogCambioIngreso(det);
                }

                DocumentoLinea detalleupdate = uow.DocumentoRepository.GetLineaIngreso(request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);
                detalleupdate.CantidadDesafectada = detalleupdate.CantidadDesafectada - request.QT_MOVIMIENTO;
                detalleupdate.CantidadIngresada = detalleupdate.CantidadIngresada - request.QT_MOVIMIENTO;

                IDocumentoIngreso documento1 = null;
                documento1 = uow.DocumentoRepository.GetIngreso(request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO);
                uow.DocumentoRepository.UpdateDetail(documento1, detalleupdate, nuTransaccion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual void CambioIngreso(CambioDocumentoDetIngreso request, IUnitOfWork uow, bool camino = true, string descripcion = "")
        {
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                IDocumentoIngreso documento = null;

                if (request.existeDoc == false)
                {
                    documento = this.CrearDocumentoObject(request, uow, descripcion);
                    uow.DocumentoRepository.AddIngreso(documento, nuTransaccion);
                }
                else
                {
                    documento = uow.DocumentoRepository.GetIngreso(request.NU_DOCUMENTO_NUEVO, request.TP_DOCUMENTO_NUEVO);
                }

                DocumentoLinea linea = uow.DocumentoRepository.GetLineaIngreso(request.NU_DOCUMENTO_NUEVO, request.TP_DOCUMENTO_NUEVO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);

                if (linea != null)
                {
                    linea.CantidadDesafectada = linea.CantidadDesafectada + request.QT_MOVIMIENTO;
                    linea.CantidadIngresada = linea.CantidadIngresada + request.QT_MOVIMIENTO;
                    uow.DocumentoRepository.UpdateLineaDocumento(linea, documento, nuTransaccion);
                }
                else
                {
                    linea = new DocumentoLinea();
                    linea.Producto = request.CD_PRODUTO;
                    linea.Empresa = request.CD_EMPRESA;
                    linea.Identificador = request.NU_IDENTIFICADOR;
                    linea.Faixa = request.CD_FAIXA;
                    linea.CantidadDesafectada = request.QT_MOVIMIENTO;
                    linea.CantidadIngresada = request.QT_MOVIMIENTO;
                    uow.DocumentoRepository.AddLineaDocumento(documento, linea, nuTransaccion);

                }

                if (camino)
                {
                    CuentaCorriente cuenta = uow.DocumentoRepository.GetCuentaCorrienteDocumento(request);
                    cuenta.NU_DOCUMENTO_CAMBIO = request.NU_DOCUMENTO_NUEVO;
                    cuenta.TP_DOCUMENTO_CAMBIO = request.TP_DOCUMENTO_NUEVO;
                    cuenta.DT_UPDROW = DateTime.Now;
                    cuenta.CD_FUNCIONARIO = request.userId;
                    uow.DocumentoRepository.UpdateCuentaCorrienteDocumento(cuenta);
                }
                else
                {
                    CuentaCorrienteCambioDoc cuenta = uow.DocumentoRepository.GetCuentaCorrienteDocumentoDOC500(request);
                    cuenta.NU_DOCUMENTO_CAMBIO = request.NU_DOCUMENTO_NUEVO;
                    cuenta.TP_DOCUMENTO_CAMBIO = request.TP_DOCUMENTO_NUEVO;
                    cuenta.DT_UPDROW = DateTime.Now;
                    cuenta.CD_FUNCIONARIO = request.userId;
                    uow.DocumentoRepository.UpdateCuentaCorrienteDocumentoDOC500(cuenta);
                }

                if (!uow.DocumentoRepository.ExisteLogCambioDoc(request))
                {
                    CambioDocDet det = new CambioDocDet();
                    det.CD_PRODUTO = request.CD_PRODUTO;
                    det.CD_FAIXA = request.CD_FAIXA;
                    det.NU_IDENTIFICADOR = request.NU_IDENTIFICADOR;
                    det.CD_EMPRESA = request.CD_EMPRESA;
                    det.NU_DOCUMENTO = request.NU_DOCUMENTO_INGRESO;
                    det.TP_DOCUMENTO = request.TP_DOCUMENTO_INGRESO;
                    det.NU_DOCUMENTO_CAMBIO = request.NU_DOCUMENTO_NUEVO;
                    det.TP_DOCUMENTO_CAMBIO = request.TP_DOCUMENTO_NUEVO;
                    uow.DocumentoRepository.AddLogCambioIngreso(det);
                }

                DocumentoLinea detalleupdate = uow.DocumentoRepository.GetLineaIngreso(request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO, request.CD_PRODUTO, request.NU_IDENTIFICADOR);
                detalleupdate.CantidadDesafectada = detalleupdate.CantidadDesafectada - request.QT_MOVIMIENTO;
                detalleupdate.CantidadIngresada = detalleupdate.CantidadIngresada - request.QT_MOVIMIENTO;

                IDocumentoIngreso documento1 = null;
                documento1 = uow.DocumentoRepository.GetIngreso(request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO);
                uow.DocumentoRepository.UpdateDetail(documento1, detalleupdate, nuTransaccion);

                if (!camino)
                {
                    DocumentoLineaEgreso det = uow.DocumentoRepository.GetDocumentoEgresoDet(request.NU_DOCUMENTO_EGRESO_PRDC, request.TP_DOCUMENTO_EGRESO_PRDC, request.CD_PRODUTO, request.NU_IDENTIFICADOR,
                        request.CD_EMPRESA, request.NU_DOCUMENTO_INGRESO, request.TP_DOCUMENTO_INGRESO);
                    decimal cantidad_desafctada = det.CantidadDesafectada ?? 0;

                    if (cantidad_desafctada - request.QT_MOVIMIENTO > 0)
                    {
                        DocumentoLineaEgreso egreso = new DocumentoLineaEgreso();
                        egreso.CantidadDesafectada = cantidad_desafctada - request.QT_MOVIMIENTO;
                        egreso.CantidadDescargada = det.CantidadDescargada;
                        egreso.CIF = det.CIF;
                        egreso.Empresa = det.Empresa;
                        egreso.Faixa = det.Faixa;
                        egreso.FechaAlta = det.FechaAlta;
                        egreso.FechaModificacion = det.FechaModificacion;
                        egreso.FOB = det.FOB;
                        egreso.Identificador = det.Identificador;
                        egreso.Numero = det.Numero;
                        egreso.Producto = det.Producto;
                        egreso.Tributo = det.Tributo;
                        egreso.NumeroProceso = det.NumeroProceso;
                        egreso.DocumentoIngreso = det.DocumentoIngreso;
                        egreso.TpDocumentoIngreso = det.TpDocumentoIngreso;
                        egreso.NumeroDocumento = det.NumeroDocumento;
                        egreso.TpDocumento = det.TpDocumento;
                        egreso.NumeroDocumentoIngreso = det.NumeroDocumentoIngreso;
                        uow.DocumentoRepository.AddLineaEgresoActa(egreso, nuTransaccion);

                        DocumentoLineaEgreso egreso1 = new DocumentoLineaEgreso();
                        egreso1.CantidadDesafectada = request.QT_MOVIMIENTO;
                        egreso1.CantidadDescargada = det.CantidadDescargada;
                        egreso1.CIF = det.CIF;
                        egreso1.Empresa = det.Empresa;
                        egreso1.Faixa = det.Faixa;
                        egreso1.FechaAlta = det.FechaAlta;
                        egreso1.FechaModificacion = det.FechaModificacion;
                        egreso1.FOB = det.FOB;
                        egreso1.Identificador = det.Identificador;
                        egreso1.Numero = det.Numero;
                        egreso1.Producto = det.Producto;
                        egreso1.Tributo = det.Tributo;
                        egreso1.NumeroProceso = det.NumeroProceso;
                        egreso1.DocumentoIngreso = det.DocumentoIngreso;
                        egreso1.NumeroDocumento = det.NumeroDocumento;
                        egreso1.TpDocumento = det.TpDocumento;
                        egreso1.NumeroDocumentoIngreso = request.NU_DOCUMENTO_NUEVO;
                        egreso1.TpDocumentoIngreso = request.TP_DOCUMENTO_NUEVO;
                        egreso1.Numero = -egreso1.Numero;

                        uow.DocumentoRepository.AddLineaEgresoActa(egreso1, nuTransaccion);
                    }

                    det.NumeroDocumentoIngreso = request.NU_DOCUMENTO_NUEVO;
                    det.TpDocumentoIngreso = request.TP_DOCUMENTO_NUEVO;
                    uow.DocumentoRepository.UpdateDetalleDocumentoEgreso(det, nuTransaccion);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual IDocumentoIngreso CrearDocumentoObject(CambioDocumentoDetIngreso request, IUnitOfWork uow, string descripcion = "")
        {
            IDocumentoIngreso documento = this._factoryService.CreateDocumentoIngreso(request.TP_DOCUMENTO_NUEVO);

            if (documento != null)
            {
                string descripcion1 = request.NU_DOCUMENTO_EGRESO + "#" + request.TP_DOCUMENTO_EGRESO;

                if (!string.IsNullOrEmpty(descripcion))
                {
                    descripcion1 = descripcion + "#" + request.NU_DOCUMENTO_NUEVO;
                    documento.Descripcion = descripcion1;
                }
                else
                {
                    documento.Descripcion = "Cambio DOc Egreso:" + descripcion1;
                }

                documento.Numero = request.NU_DOCUMENTO_NUEVO;
                documento.Tipo = request.TP_DOCUMENTO_NUEVO;
                documento.IdManual = "N";
                documento.Empresa = request.CD_EMPRESA;
                documento.FechaAlta = DateTime.Now;
                documento.Estado = uow.DocumentoRepository.GetEstadoDestino(request.TP_DOCUMENTO_NUEVO, AccionDocumento.Finalizar);
            }

            return documento;
        }

        public virtual void FinalizarEgresoPorCamion(IUnitOfWork uow, Camion camion)
        {
            try
            {
                IDocumentoEgreso documentoEgreso = uow.DocumentoRepository.GetEgresoPorCamion(camion.Id);

                if (documentoEgreso != null)
                {
                    documentoEgreso.Estado = uow.DocumentoRepository.GetEstadoDestino(documentoEgreso.Tipo, AccionDocumento.Finalizar);
                    documentoEgreso.Finalizar();

                    uow.DocumentoRepository.UpdateEgreso(documentoEgreso, uow.GetTransactionNumber());
                    uow.SaveChanges();
                }
                else
                {
                    throw new ValidationFailedException("General_Sec0_Error_CamionSinDocumento", new string[] { camion.Id.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual void GenerarLineasEgresoConReserva(IDocumentoEgreso documentoEgreso, List<LineaEgresoDocumental> lineasReserva, IUnitOfWork uow, int usuario, out DocumentoReservaDesafectada infoReservas)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var lineasModificadas = new List<DocumentoLineaDesafectada>();
            infoReservas = new DocumentoReservaDesafectada();

            if (documentoEgreso != null)
            {
                DocumentoLinea linea;
                decimal? saldoADesafectar;
                decimal? qtDesafectada;

                foreach (var lineaReserva in lineasReserva)
                {
                    saldoADesafectar = lineaReserva.CantidadAfectada;

                    List<DocumentoPreparacionReserva> reservas = uow.DocumentoRepository.GetDocumentoPreparacionReservas(lineaReserva.Preparacion, lineaReserva.Producto, lineaReserva.Identificador, lineaReserva.Empresa, lineaReserva.Faixa);

                    if (reservas.Count == 0 && lineaReserva.Semiacabado == "S")
                    {
                        List<DocumentoPreparacionReserva> DOC = uow.DocumentoRepository.GetDocumentoSemiacabado(lineaReserva.Producto, lineaReserva.Identificador, lineaReserva.Empresa, lineaReserva.Faixa);

                        foreach (var reserva in DOC)
                        {
                            //Consumir saldo de preparación
                            if (saldoADesafectar >= reserva.CantidadPreparada)
                            {
                                qtDesafectada = reserva.CantidadPreparada;
                                saldoADesafectar -= reserva.CantidadPreparada;
                            }
                            else
                            {
                                qtDesafectada = saldoADesafectar;
                                saldoADesafectar = 0;
                            }

                            linea = uow.DocumentoRepository.GetLineaIngreso(reserva.NroDocumento, reserva.TipoDocumento, reserva.Producto, reserva.NroIdentificadorPicking);

                            this.AgregarLineaEgreso(linea, reserva, documentoEgreso, uow, usuario, lineaReserva.Semiacabado, lineaReserva.Consumible, qtDesafectada);

                            var lineaExiste = lineasModificadas
                                .FirstOrDefault(lm => lm.NroDocumento == reserva.NroDocumento
                                    && lm.TipoDocumento == reserva.TipoDocumento
                                    && lm.LineaModificada.Producto == linea.Producto
                                    && lm.LineaModificada.Identificador == linea.Identificador);

                            if (lineaExiste != null)
                            {
                                lineasModificadas.Remove(lineaExiste);
                                linea.CantidadDesafectada = lineaExiste.LineaModificada.CantidadDesafectada;
                                linea.CantidadReservada = lineaExiste.LineaModificada.CantidadReservada;
                            }

                            //Desafectar saldo en linea de Ingreso
                            linea.UpdateReserva(qtDesafectada);

                            lineasModificadas.Add(new DocumentoLineaDesafectada() { NroDocumento = reserva.NroDocumento, TipoDocumento = reserva.TipoDocumento, LineaModificada = linea });

                            //Desafectar saldo en linea de Reserva
                            reserva.AfectarCantidadProducto(-qtDesafectada);
                            reserva.NumeroTransaccion = nuTransaccion;
                            reserva.Semiacabado = "S";

                            if (reserva.CantidadProducto == 0)
                            {
                                reserva.NumeroTransaccionDelete = nuTransaccion;                                
                                infoReservas.ReservasEliminadas.Add(reserva);
                            }
                            else
                            {
                                reserva.NumeroTransaccionDelete = null;
                                infoReservas.ReservasModificadas.Add(reserva);
                            }

                            if (saldoADesafectar == 0)
                                break;
                        }

                        if (saldoADesafectar > 0)
                        {
                            var err = string.Format("Error diferencia en saldos del Semiacabado y consumido, Linea: {0} - {1} - {2} - {3} " + lineaReserva.Empresa.ToString(), lineaReserva.Producto, lineaReserva.Identificador, lineaReserva.Faixa);
                            _logger.Error(new Exception(err), err);
                        }
                    }
                    else
                    {
                        foreach (var reserva in reservas)
                        {
                            var documentoHabilitaStock = uow.DocumentoRepository.AnyEstadoDocumentoHabilitaStock(reserva.NroDocumento, reserva.TipoDocumento);
                            if (!documentoHabilitaStock)
                            {
                                throw new ValidationFailedException("General_Sec0_Error_NumeroDocumentoNoHabilitaStock", new string[]{ reserva.NroDocumento, reserva.TipoDocumento });
                            }

                            //Consumir saldo de preparación
                            if (saldoADesafectar >= reserva.CantidadPreparada)
                            {
                                qtDesafectada = reserva.CantidadPreparada;
                                saldoADesafectar -= reserva.CantidadPreparada;
                            }
                            else
                            {
                                qtDesafectada = saldoADesafectar;
                                saldoADesafectar = 0;
                            }

                            linea = uow.DocumentoRepository.GetLineaIngreso(reserva.NroDocumento, reserva.TipoDocumento, reserva.Producto, reserva.NroIdentificadorPicking);

                            this.AgregarLineaEgreso(linea, reserva, documentoEgreso, uow, usuario, lineaReserva.Semiacabado, lineaReserva.Consumible, qtDesafectada);

                            var lineaExiste = lineasModificadas
                                .FirstOrDefault(lm => lm.NroDocumento == reserva.NroDocumento
                                    && lm.TipoDocumento == reserva.TipoDocumento
                                    && lm.LineaModificada.Producto == linea.Producto
                                    && lm.LineaModificada.Identificador == linea.Identificador);

                            if (lineaExiste != null)
                            {
                                lineasModificadas.Remove(lineaExiste);
                                linea.CantidadDesafectada = lineaExiste.LineaModificada.CantidadDesafectada;
                                linea.CantidadReservada = lineaExiste.LineaModificada.CantidadReservada;
                            }

                            //Desafectar saldo en linea de Ingreso
                            linea.UpdateReserva(qtDesafectada);

                            lineasModificadas.Add(new DocumentoLineaDesafectada()
                            {
                                NroDocumento = reserva.NroDocumento,
                                TipoDocumento = reserva.TipoDocumento,
                                LineaModificada = linea
                            });

                            //Desafectar saldo en linea de Reserva
                            reserva.AfectarCantidadProducto(-qtDesafectada);
                            reserva.NumeroTransaccion = nuTransaccion;

                            if (reserva.CantidadProducto == 0)
                            {
                                reserva.NumeroTransaccionDelete = nuTransaccion;
                                infoReservas.ReservasEliminadas.Add(reserva);
                            }
                            else
                            {
                                reserva.NumeroTransaccionDelete = null;
                                infoReservas.ReservasModificadas.Add(reserva);
                            }

                            if (saldoADesafectar == 0)
                                break;
                        }
                    }
                }

                foreach (var lineaMod in lineasModificadas)
                {
                    uow.DocumentoRepository.UpdateDetailWithoutDocument(lineaMod.NroDocumento, lineaMod.TipoDocumento, lineaMod.LineaModificada, nuTransaccion);
                }
            }
        }

        public virtual IDocumentoEgreso CrearCabezalEgreso(IUnitOfWork uow, int usuario, int empresa, int? camion, string tipoDocumento, string numeroDocumento = null)
        {
            var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(tipoDocumento);
            var documento = this._factoryService.CreateDocumentoEgreso(tipoDocumento);

            if (!string.IsNullOrEmpty(numeroDocumento))
            {
                IDocumentoEgreso documentoExistente = uow.DocumentoRepository.GetEgreso(numeroDocumento, tipoDocumento);
                if (documentoExistente != null)
                    throw new ValidationFailedException("General_Sec0_Error_Error47");
            }

            if (documento != null)
            {
                documento.Tipo = tipoDocumento;

                if (uow.DocumentoTipoRepository.DocumentoNumeracionAutogenerada(tipoDocumento))
                    documento.Numero = documento.GetNumeroDocumento(uow);
                else if (string.IsNullOrEmpty(numeroDocumento))
                    throw new ValidationFailedException("General_Sec0_Error_NumeroDocumentoNoEspecificado");
                else
                    documento.Numero = numeroDocumento;

                documento.Usuario = usuario;
                documento.ValorArbitraje = 1;
                documento.Situacion = 15;
                documento.FechaAlta = DateTime.Now;
                documento.Empresa = empresa;
                documento.Estado = estadoInicial;
                documento.Camion = camion;
                documento.AgendarAutomaticamente = false;
                documento.Validado = false;
                documento.DocumentoAduana = new DocumentoAduana();
                documento.Descripcion = uow.DocumentoTipoRepository.GetTipoDocumento(tipoDocumento).DescripcionTipoDocumento;
            }

            return documento;
        }

        public virtual void AgregarLineaEgreso(DocumentoLinea lineaIngreso, DocumentoPreparacionReserva reserva, IDocumentoEgreso documentoEgreso, IUnitOfWork uow, int usuario, string semiacabado, string consumible, decimal? qtDesafectada)
        {
            var nuevaLinea = ProcesarLineaEgreso(uow, lineaIngreso, reserva, usuario, semiacabado, consumible, qtDesafectada, documentoEgreso);
            if (nuevaLinea != null)
            {
                documentoEgreso.OutDetail.Add(nuevaLinea);
            }
        }

        public virtual InformacionReserva GetNewInformacionReserva(DocumentoPreparacionReserva reserva, decimal? qtDesafectada)
        {
            return new InformacionReserva
            {
                CantidadAfectada = qtDesafectada,
                Empresa = reserva.Empresa,
                Faixa = reserva.Faixa,
                Identificador = reserva.Identificador,
                IdentificadorPicking = reserva.NroIdentificadorPicking,
                NumeroDocumentoIngreso = reserva.DocumentoIngreso.Numero,
                Preparacion = reserva.Preparacion,
                Producto = reserva.Producto,
                TipoDocumentoIngreso = reserva.DocumentoIngreso.Tipo
            };
        }

        public virtual List<DocumentoLineaEgreso> GenerarLineasEgreso(IUnitOfWork uow, List<LineaEgresoDocumental> lineasEgreso, int usuario, IDocumentoEgreso documentoEgreso, out DocumentoReservaDesafectada infoReservas)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var lineasModificadas = new List<DocumentoLineaDesafectada>();
            var nuevasLineas = new List<DocumentoLineaEgreso>();
            infoReservas = new DocumentoReservaDesafectada();

            if (documentoEgreso != null)
            {
                DocumentoLinea lineaIngreso;
                decimal? saldoADesafectar;
                decimal? qtDesafectada;

                foreach (var lineaEgreso in lineasEgreso)
                {
                    saldoADesafectar = lineaEgreso.CantidadAfectada;

                    var reservas = uow.DocumentoRepository.GetDocumentoPreparacionReservas(lineaEgreso.Preparacion, lineaEgreso.Producto, lineaEgreso.Identificador, lineaEgreso.Empresa, lineaEgreso.Faixa);

                    if (reservas.Count == 0 && lineaEgreso.Semiacabado == "S")
                    {
                        var DOC = uow.DocumentoRepository.GetDocumentoSemiacabado(lineaEgreso.Producto, lineaEgreso.Identificador, lineaEgreso.Empresa, lineaEgreso.Faixa);

                        foreach (var reserva in DOC)
                        {
                            //Consumir saldo de preparación
                            if (saldoADesafectar >= reserva.CantidadPreparada)
                            {
                                qtDesafectada = reserva.CantidadPreparada;
                                saldoADesafectar = saldoADesafectar - reserva.CantidadPreparada;
                            }
                            else
                            {
                                qtDesafectada = saldoADesafectar;
                                saldoADesafectar = 0;
                            }

                            lineaIngreso = uow.DocumentoRepository.GetLineaIngreso(reserva.NroDocumento, reserva.TipoDocumento, reserva.Producto, reserva.NroIdentificadorPicking);

                            var nuevaLineaEgreso = this.ProcesarLineaEgreso(uow, lineaIngreso, reserva, usuario, lineaEgreso.Semiacabado, lineaEgreso.Consumible, qtDesafectada, documentoEgreso);
                            if (nuevaLineaEgreso != null)
                                nuevasLineas.Add(nuevaLineaEgreso);

                            var lineaExiste = lineasModificadas.FirstOrDefault(lm => lm.NroDocumento == reserva.NroDocumento
                                && lm.TipoDocumento == reserva.TipoDocumento
                                && lm.LineaModificada.Producto == lineaIngreso.Producto
                                && lm.LineaModificada.Identificador == lineaIngreso.Identificador);

                            if (lineaExiste != null)
                            {
                                lineasModificadas.Remove(lineaExiste);
                                lineaIngreso.CantidadDesafectada = lineaExiste.LineaModificada.CantidadDesafectada;
                                lineaIngreso.CantidadReservada = lineaExiste.LineaModificada.CantidadReservada;
                            }

                            //Desafectar saldo en linea de Ingreso
                            lineaIngreso.UpdateReserva(qtDesafectada);

                            lineasModificadas.Add(new DocumentoLineaDesafectada() { NroDocumento = reserva.NroDocumento, TipoDocumento = reserva.TipoDocumento, LineaModificada = lineaIngreso });

                            //Desafectar saldo en linea de Reserva
                            reserva.AfectarCantidadProducto(-qtDesafectada);
                            reserva.NumeroTransaccion = nuTransaccion;
                            reserva.Semiacabado = "S";

                            if (reserva.CantidadProducto == 0)
                            {
                                reserva.NumeroTransaccionDelete = nuTransaccion;
                                infoReservas.ReservasEliminadas.Add(reserva);
                            }
                            else
                            {
                                reserva.NumeroTransaccionDelete = null;
                                infoReservas.ReservasModificadas.Add(reserva);
                            }

                            if (saldoADesafectar == 0)
                                break;
                        }

                        if (saldoADesafectar > 0)
                        {
                            var err = string.Format("Error diferencia en saldos del Semiacabado y consumido, Linea: {0} - {1} - {2} - {3} " + lineaEgreso.Empresa.ToString(), lineaEgreso.Producto, lineaEgreso.Identificador, lineaEgreso.Faixa);
                            _logger.Error(new Exception(err), err);
                        }
                    }
                    else
                    {
                        foreach (var reserva in reservas)
                        {
                            //Consumir saldo de preparación
                            if (saldoADesafectar >= reserva.CantidadPreparada)
                            {
                                qtDesafectada = reserva.CantidadPreparada;
                                saldoADesafectar = saldoADesafectar - reserva.CantidadPreparada;
                            }
                            else
                            {
                                qtDesafectada = saldoADesafectar;
                                saldoADesafectar = 0;
                            }

                            lineaIngreso = uow.DocumentoRepository.GetLineaIngreso(reserva.NroDocumento, reserva.TipoDocumento, reserva.Producto, reserva.NroIdentificadorPicking);

                            var nuevaLineaEgreso = this.ProcesarLineaEgreso(uow, lineaIngreso, reserva, usuario, lineaEgreso.Semiacabado, lineaEgreso.Consumible, qtDesafectada, documentoEgreso);
                            if (nuevaLineaEgreso != null)
                                nuevasLineas.Add(nuevaLineaEgreso);

                            var lineaExiste = lineasModificadas.FirstOrDefault(lm => lm.NroDocumento == reserva.NroDocumento
                                && lm.TipoDocumento == reserva.TipoDocumento
                                && lm.LineaModificada.Producto == lineaIngreso.Producto
                                && lm.LineaModificada.Identificador == lineaIngreso.Identificador);

                            if (lineaExiste != null)
                            {
                                lineasModificadas.Remove(lineaExiste);
                                lineaIngreso.CantidadDesafectada = lineaExiste.LineaModificada.CantidadDesafectada;
                                lineaIngreso.CantidadReservada = lineaExiste.LineaModificada.CantidadReservada;
                            }

                            //Desafectar saldo en linea de Ingreso
                            lineaIngreso.UpdateReserva(qtDesafectada);

                            lineasModificadas.Add(new DocumentoLineaDesafectada()
                            {
                                NroDocumento = reserva.NroDocumento,
                                TipoDocumento = reserva.TipoDocumento,
                                LineaModificada = lineaIngreso
                            });

                            //Desafectar saldo en linea de Reserva
                            reserva.AfectarCantidadProducto(-qtDesafectada);
                            reserva.NumeroTransaccion = nuTransaccion;

                            if (reserva.CantidadProducto == 0)
                            {
                                reserva.NumeroTransaccionDelete = nuTransaccion;
                                infoReservas.ReservasEliminadas.Add(reserva);
                            }
                            else
                            {
                                reserva.NumeroTransaccionDelete = null;
                                infoReservas.ReservasModificadas.Add(reserva);
                            }

                            if (saldoADesafectar == 0)
                                break;
                        }
                    }
                }

                foreach (var lineaMod in lineasModificadas)
                {
                    uow.DocumentoRepository.UpdateDetailWithoutDocument(lineaMod.NroDocumento, lineaMod.TipoDocumento, lineaMod.LineaModificada, nuTransaccion);
                }
            }

            return nuevasLineas;
        }

        public virtual List<DocumentoLineaEgreso> GenerarLineasEgresoSinPreparacion(IUnitOfWork uow, List<LineaEgresoDocumental> lineasEgreso, int usuario)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var lineasModificadas = new List<DocumentoLineaDesafectada>();
            var nuevasLineas = new List<DocumentoLineaEgreso>();

            foreach (var lineaEgreso in lineasEgreso)
            {
                var qtDesafectar = lineaEgreso.CantidadAfectada ?? 0;
                var lineasDesafectadas = uow.DocumentoRepository.DesafectarLineasSinReserva(lineaEgreso.Producto, lineaEgreso.Identificador, lineaEgreso.Empresa, lineaEgreso.Faixa, qtDesafectar);

                foreach (var lineaDesafectada in lineasDesafectadas)
                {
                    var nuevaLineaEgreso = this.GenerarLineaEgreso(uow, lineaDesafectada, usuario, lineaEgreso.Semiacabado, lineaEgreso.Consumible, qtDesafectar);
                    nuevasLineas.Add(nuevaLineaEgreso);
                }

                lineasModificadas.AddRange(lineasDesafectadas);
            }

            foreach (var lineaMod in lineasModificadas)
            {
                uow.DocumentoRepository.UpdateDetailWithoutDocument(lineaMod.NroDocumento, lineaMod.TipoDocumento, lineaMod.LineaModificada, nuTransaccion);
            }

            return nuevasLineas;
        }

        public virtual DocumentoLineaEgreso ProcesarLineaEgreso(IUnitOfWork uow, DocumentoLinea lineaIngreso, DocumentoPreparacionReserva reserva, int usuario, string semiacabado, string consumible, decimal? qtDesafectada, IDocumentoEgreso documentoEgreso)
        {
            IDocumento documento;
            DocumentoLineaEgreso nuevaLinea = null;
            var tipoDoc = uow.DocumentoTipoRepository.GetTipoDocumento(reserva.TipoDocumento);

            if (tipoDoc.TipoOperacion == TipoDocumentoOperacion.MODIFICACION)
                documento = uow.DocumentoRepository.GetActa(reserva.NroDocumento, reserva.TipoDocumento);
            else
                documento = uow.DocumentoRepository.GetIngreso(reserva.NroDocumento, reserva.TipoDocumento);

            //Verificar si existe linea para mismos Producto/Identificador/DocumentoIngreso/TipoDocIngreso
            DocumentoLineaEgreso lineaExistente = documentoEgreso.OutDetail
                .FirstOrDefault(od => od.Producto == reserva.Producto
                    && od.Identificador == reserva.NroIdentificadorPicking
                    && od.NumeroDocumentoIngreso == reserva.NroDocumento
                    && od.TpDocumentoIngreso == reserva.TipoDocumento);

            if (lineaExistente != null)
            {
                var cantidadTotal = lineaExistente.CantidadDesafectada + qtDesafectada;

                lineaExistente.CIF = ((lineaIngreso.CIF ?? 0) / lineaIngreso.CantidadIngresada) * cantidadTotal;
                lineaExistente.FOB = ((lineaIngreso.ValorMercaderia ?? 0) / lineaIngreso.CantidadIngresada) * cantidadTotal;
                lineaExistente.Tributo = ((lineaIngreso.ValorTributo ?? 0) / lineaIngreso.CantidadIngresada) * cantidadTotal;
                lineaExistente.CantidadDesafectada = cantidadTotal;
                lineaExistente.FechaModificacion = DateTime.Now;

                var lineaReservaExistente = lineaExistente.Reservas
                    .FirstOrDefault(lr => lr.Empresa == reserva.Empresa
                        && lr.Faixa == reserva.Faixa
                        && lr.Identificador == reserva.Identificador
                        && lr.IdentificadorPicking == reserva.NroIdentificadorPicking
                        && lr.NumeroDocumentoIngreso == reserva.NroDocumento
                        && lr.Preparacion == reserva.Preparacion
                        && lr.Producto == reserva.Producto
                        && lr.TipoDocumentoIngreso == reserva.TipoDocumento);

                if (lineaReservaExistente == null)
                    lineaExistente.Reservas.Add(GetNewInformacionReserva(reserva, qtDesafectada));
                else
                    lineaReservaExistente.CantidadAfectada += qtDesafectada;
            }
            else
            {
                nuevaLinea = new DocumentoLineaEgreso()
                {
                    CIF = ((lineaIngreso.CIF ?? 0) / lineaIngreso.CantidadIngresada) * qtDesafectada,
                    Empresa = reserva.Empresa,
                    Faixa = lineaIngreso.Faixa,
                    DocumentoIngreso = documento,
                    FechaAlta = DateTime.Now,
                    FOB = ((lineaIngreso.ValorMercaderia ?? 0) / lineaIngreso.CantidadIngresada) * qtDesafectada,
                    Identificador = reserva.NroIdentificadorPicking,
                    Numero = uow.DocumentoRepository.GetNumeroSecuenciaDetalleEgreso(),
                    NumeroDocumento = reserva.NroDocumento,
                    Producto = reserva.Producto,
                    TpDocumento = reserva.TipoDocumento,
                    Usuario = usuario,
                    CantidadDesafectada = qtDesafectada,
                    Tributo = ((lineaIngreso.ValorTributo ?? 0) / lineaIngreso.CantidadIngresada) * qtDesafectada,
                    Semiacabado = semiacabado,
                    Consumible = consumible,
                    NumeroDocumentoIngreso = documento?.Numero,
                    TpDocumentoIngreso = documento?.Tipo,
                    Reservas = new List<InformacionReserva>
                    {
                        GetNewInformacionReserva(reserva, qtDesafectada)
                    }
                };
            }

            return nuevaLinea;
        }

        public virtual DocumentoLineaEgreso GenerarLineaEgreso(IUnitOfWork uow, DocumentoLineaDesafectada lineaDesafectada, int usuario, string semiacabado, string consumible, decimal qtDesafectada)
        {
            IDocumento documento;
            var tipoDoc = uow.DocumentoTipoRepository.GetTipoDocumento(lineaDesafectada.TipoDocumento);
            var lineaIngreso = lineaDesafectada.LineaModificada;

            if (tipoDoc.TipoOperacion == TipoDocumentoOperacion.MODIFICACION)
                documento = uow.DocumentoRepository.GetActa(lineaDesafectada.NroDocumento, lineaDesafectada.TipoDocumento);
            else
                documento = uow.DocumentoRepository.GetIngreso(lineaDesafectada.NroDocumento, lineaDesafectada.TipoDocumento);

            return new DocumentoLineaEgreso()
            {
                CIF = ((lineaIngreso.CIF ?? 0) / lineaIngreso.CantidadIngresada) * qtDesafectada,
                Empresa = lineaIngreso.Empresa,
                Faixa = lineaIngreso.Faixa,
                DocumentoIngreso = documento,
                FechaAlta = DateTime.Now,
                FOB = ((lineaIngreso.ValorMercaderia ?? 0) / lineaIngreso.CantidadIngresada) * qtDesafectada,
                Identificador = lineaIngreso.Identificador,
                Numero = uow.DocumentoRepository.GetNumeroSecuenciaDetalleEgreso(),
                NumeroDocumento = lineaDesafectada.NroDocumento,
                Producto = lineaIngreso.Producto,
                TpDocumento = lineaDesafectada.TipoDocumento,
                Usuario = usuario,
                CantidadDesafectada = qtDesafectada,
                Tributo = ((lineaIngreso.ValorTributo ?? 0) / lineaIngreso.CantidadIngresada) * qtDesafectada,
                Semiacabado = semiacabado,
                Consumible = consumible
            };
        }

        public virtual void GenerarCargasPredefinidas(IUnitOfWork uow, int cdCamion)
        {
            //Nota: Esta lista de pedidos se carga automaticamente con los nuevos número de cargas, no refleja el dato real existente.
            var pedidos = uow.PedidoRepository.GetPedidosNuevasCargas(cdCamion);

            foreach (var p in pedidos)
            {
                var carga = new Carga
                {
                    Id = (long)p.NuCarga,
                    Ruta = (short)p.Ruta,
                    Descripcion = $"Generada por el egreso documental para el Pedido {p.Id} - Cliente: {p.Cliente} - Empresa: {p.Empresa}",
                    Preparacion = null,
                    FechaAlta = DateTime.Now
                };

                p.Transaccion = uow.GetTransactionNumber();

                uow.CargaRepository.AddCarga(carga, false);
                uow.PedidoRepository.UpdatePedido(p);
            }
        }
    }
}