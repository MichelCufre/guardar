using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Documento.Integracion.Preparaciones;
using WIS.Domain.Documento.Integracion.Recepcion;
using WIS.Domain.Documento.Serializables.Reserva;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Documento.Integracion.Produccion
{
    public class ProduccionDocumental
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly IIdentityService _identity;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProduccionDocumental(IUnitOfWorkFactory uowFactory,
            IFactoryService factoryService,
            IParameterService parameterService,
            IIdentityService identity)
        {
            this._uowFactory = uowFactory;
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._identity = identity;
        }

        public virtual ProduccionDocumentalResponse DocumentarProduccion(ProduccionDocumentalRequest request, IUnitOfWork uow)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var result = new ProduccionDocumentalResponse();

            try
            {
                ValidarDocumentos(uow, request, out IDocumentoIngreso docIngreso, out IDocumentoEgreso docEgreso);

                var egreso = new EgresoDocumental(this._factoryService);
                var lineasEgreso = request.LineasEgreso.Select(le => MapToLineaEgresoDocumental(le)).ToList();
                var nuevasLineasEgreso = egreso.GenerarLineasEgreso(uow, lineasEgreso, request.Usuario, docEgreso, out DocumentoReservaDesafectada infoReservas);

                docEgreso.Estado = uow.DocumentoRepository.GetEstadoDestino(docEgreso.Tipo, AccionDocumento.IniciarOperacion);
                uow.DocumentoRepository.UpdateEgresoAndDetails(docEgreso, nuevasLineasEgreso, nuTransaccion);

                var ingreso = new IngresoDocumental(this._factoryService, this._parameterService, this._identity);
                var lineasIngreso = request.LineasIngreso.Select(li => MapToLineaIngresoDocumental(li)).ToList();
                var nuevasLineasIngreso = ingreso.GenerarLineasIngreso(uow, lineasIngreso, docIngreso);

                docIngreso.Estado = uow.DocumentoRepository.GetEstadoDestino(docIngreso.Tipo, AccionDocumento.IniciarOperacion);
                uow.DocumentoRepository.UpdateIngresoAndDetails(docIngreso, nuevasLineasIngreso, nuTransaccion);

                var documentoProduccion = GenerarDocumentoProduccion(docIngreso, docEgreso, request.NroProduccion);
                uow.DocumentoRepository.AddDocumentoProduccion(documentoProduccion);

                var reservasAfectadas = new List<DocumentoPreparacionReserva>();

                reservasAfectadas.AddRange(infoReservas.ReservasModificadas);
                reservasAfectadas.AddRange(infoReservas.ReservasEliminadas);

                foreach (var reserva in reservasAfectadas)
                {
                    if (string.IsNullOrEmpty(reserva.Semiacabado))
                    {
                        uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                    }
                }

                uow.SaveChanges();

                if (infoReservas.ReservasEliminadas.Count() > 0)
                {
                    foreach (var reserva in infoReservas.ReservasEliminadas)
                    {
                        if (string.IsNullOrEmpty(reserva.Semiacabado))
                        {
                            uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
                        }
                    }
                }

                result.Success = true;
                result.NroDocumentoIngreso = docIngreso.Numero;
                result.TipoDocumentoIngreso = docIngreso.Tipo;
                result.NroDocumentoEgreso = docEgreso.Numero;
                result.TipoDocumentoEgreso = docEgreso.Tipo;
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error(ex, ex.Message);
                result.Success = false;
                result.ErrorMsg = ex.Message;
                result.StrArguments = ex.StrArguments;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result.Success = false;
                result.ErrorMsg = ex.Message;
                throw ex;
            }

            return result;
        }

        public virtual LineaEgresoDocumental MapToLineaEgresoDocumental(LineaEgresoDocumentalRequest request)
        {
            return new LineaEgresoDocumental()
            {
                CantidadAfectada = request.CantidadAfectada,
                Consumible = request.Consumible,
                Empresa = request.Empresa,
                Identificador = request.Identificador,
                Preparacion = request.Preparacion ?? -1,
                Producto = request.Producto,
                Semiacabado = request.Semiacabado,
                Faixa = request.Faixa
            };
        }

        public virtual LineaIngresoDocumental MapToLineaIngresoDocumental(LineaIngresoDocumentalRequest request)
        {
            return new LineaIngresoDocumental()
            {
                CantidadAfectada = request.CantidadAfectada,
                Empresa = request.Empresa,
                Identificador = request.Identificador,
                Producto = request.Producto,
                Faixa = request.Faixa,
                Semiacabado = request.Semiacabado
            };
        }

        public virtual DocumentoProduccion GenerarDocumentoProduccion(IDocumentoIngreso documentoIngreso, IDocumentoEgreso documentoEgreso, string produccion)
        {
            var documentoProduccion = new DocumentoProduccion()
            {
                DocumentoEgreso = documentoEgreso,
                DocumentoIngreso = documentoIngreso,
                NumeroProduccion = produccion
            };

            return documentoProduccion;
        }

        public virtual void ValidarDocumentos(IUnitOfWork uow, ProduccionDocumentalRequest request, out IDocumentoIngreso docIngreso, out IDocumentoEgreso docEgreso)
        {
            var tpOperativa = TipoOperativaDocumental.Produccion;
            var docPrep = uow.DocumentoRepository.GetDocumentoPreparacionEstado(request.Empresa, tpOperativa, request.Preparacion, true);

            if (docPrep == null)
                throw new ValidationFailedException("General_Sec0_Error_AsociacionNoEncontrada", new string[] { tpOperativa, request.Preparacion.ToString(), request.Empresa.ToString() });

            docIngreso = uow.DocumentoRepository.GetIngreso(docPrep.NroDocumentoIngreso, docPrep.TpDocumentoIngreso);
            if (docIngreso == null)
                throw new ValidationFailedException("General_Sec0_Error_DocumentoNoExiste", new string[] { docPrep.NroDocumentoIngreso, docPrep.TpDocumentoIngreso });

            docEgreso = uow.DocumentoRepository.GetEgreso(docPrep.NroDocumentoEgreso, docPrep.TpDocumentoEgreso);
            if (docEgreso == null)
                throw new ValidationFailedException("General_Sec0_Error_DocumentoNoExiste", new string[] { docPrep.NroDocumentoEgreso, docPrep.TpDocumentoEgreso });
        }

        #region BlackBox

        public virtual ProduccionDocumentalResponse DocumentarProduccionBlackBox(ProduccionDocumentalBlackBoxRequest request, IUnitOfWork uow)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var result = new ProduccionDocumentalResponse();

            try
            {
                //Verificar que no existan los documentos de ingreso y egreso para la produccion
                if (!this.ExistenDocumentosProduccion(request.NroProduccion, request.Empresa, uow, out IDocumentoEgreso egresoExistente, out IDocumentoIngreso ingresoExistente))
                {
                    var egreso = new EgresoDocumental(this._factoryService);
                    var tpEgreProd = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_EGRESO_PRODUCCION, request.Empresa);

                    //Generar documento de Egreso por producción
                    var documentoEgreso = egreso.CrearCabezalEgreso(uow, request.Usuario, request.Empresa, null, tpEgreProd, request.NroProduccion);
                    documentoEgreso.Estado = uow.DocumentoRepository.GetEstadoDestino(tpEgreProd, AccionDocumento.Finalizar);

                    //Cargo lineas de egreso
                    var lineasReserva = new List<DesreservarStockDocumentalRequestLinea>();
                    foreach (var reserva in request.Reservas)
                    {
                        lineasReserva.Add(new DesreservarStockDocumentalRequestLinea()
                        {
                            Empresa = reserva.Empresa,
                            Producto = reserva.Producto,
                            Faixa = reserva.Faixa,
                            NumeroIdentificador = reserva.Identificador,
                            CantidadAnular = (reserva.CantidadAfectada ?? 0),
                            EspecificaIdentificador = reserva.EspecificaIdentificador,
                            Semiacabado = reserva.Semiacabado,
                            Consumible = reserva.Consumible
                        });
                    }

                    var preparacion = new PreparacionDocumental(this._uowFactory);
                    var anulacionPreparaciones = preparacion.DesreservarEntradasAnularPreparaciones(uow, lineasReserva, false);

                    foreach (var anulacion in anulacionPreparaciones)
                    {
                        request.AddLineaEgreso((int)(anulacion.NumeroPreparacion ?? -1), (int)anulacion.Empresa, anulacion.Producto, anulacion.Identificador, anulacion.Faixa, anulacion.CantidadAnular, anulacion.Semiacabado, anulacion.Consumible);
                    }

                    var ingreso = new IngresoDocumental(this._factoryService, this._parameterService, this._identity);
                    var lineasEgreso = request.LineasEgreso.Select(le => MapToLineaEgresoDocumental(le)).ToList();

                    egreso.GenerarLineasEgresoConReserva(documentoEgreso, lineasEgreso, uow, request.Usuario, out DocumentoReservaDesafectada infoReservas);

                    uow.DocumentoRepository.AddEgreso(documentoEgreso, nuTransaccion);

                    var tpIngresoProd = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_INGRESO_PRODUCCION, request.Empresa);
                    var documentoIngreso = ingreso.CrearCabezalIngreso(request.Empresa, tpIngresoProd, uow, request.Usuario, request.NroProduccion);
                    var lineasIngreso = request.LineasIngreso.Select(li => MapToLineaIngresoDocumental(li)).ToList();

                    ingreso.GenerarLineasIngresoBB(lineasIngreso, documentoIngreso, uow);

                    uow.DocumentoRepository.AddIngreso(documentoIngreso, nuTransaccion);

                    var documentoProduccion = this.GenerarDocumentoProduccion(documentoIngreso, documentoEgreso, request.NroProduccion);
                    uow.DocumentoRepository.AddDocumentoProduccion(documentoProduccion);

                    var reservasAfectadas = new List<DocumentoPreparacionReserva>();

                    reservasAfectadas.AddRange(infoReservas.ReservasModificadas);
                    reservasAfectadas.AddRange(infoReservas.ReservasEliminadas);

                    foreach (var reserva in reservasAfectadas)
                    {
                        if (string.IsNullOrEmpty(reserva.Semiacabado))
                        {
                            uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                        }
                    }

                    uow.SaveChanges();

                    if (infoReservas.ReservasEliminadas.Count() > 0)
                    {
                        foreach (var reserva in infoReservas.ReservasEliminadas)
                        {
                            if (string.IsNullOrEmpty(reserva.Semiacabado))
                            {
                                uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
                            }
                        }
                    }

                    result.Success = true;
                    result.TipoDocumentoEgreso = documentoEgreso.Tipo;
                    result.NroDocumentoEgreso = documentoEgreso.Numero;
                    result.TipoDocumentoIngreso = documentoIngreso.Tipo;
                    result.NroDocumentoIngreso = documentoIngreso.Numero;
                }
                else
                {
                    result.Success = true;
                    result.TipoDocumentoEgreso = egresoExistente.Tipo;
                    result.NroDocumentoEgreso = egresoExistente.Numero;
                    result.TipoDocumentoIngreso = ingresoExistente.Tipo;
                    result.NroDocumentoIngreso = ingresoExistente.Numero;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result.Success = false;
                result.ErrorMsg = ex.Message;
                throw ex;
            }

            return result;
        }

        public virtual bool ExistenDocumentosProduccion(string produccion, int empresa, IUnitOfWork context, out IDocumentoEgreso egresoProd, out IDocumentoIngreso ingresoProd)
        {
            bool result = false;

            //Verificar egreso producción
            string tpEgreProd = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_EGRESO_PRODUCCION, empresa);
            egresoProd = context.DocumentoRepository.GetEgreso(produccion, tpEgreProd);

            //Verificar ingreso producción
            string tpIngresoProd = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_INGRESO_PRODUCCION, empresa); ;
            ingresoProd = context.DocumentoRepository.GetIngreso(produccion, tpIngresoProd);

            if (egresoProd != null && ingresoProd != null)
            {
                result = true;
            }

            return result;
        }

        #endregion
    }
}
