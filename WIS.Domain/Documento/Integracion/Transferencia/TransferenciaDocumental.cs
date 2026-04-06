using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Documento.Integracion.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Documento.Integracion.Transferencia
{
    public class TransferenciaDocumental
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly IIdentityService _identity;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TransferenciaDocumental(IUnitOfWorkFactory uowFactory,
            IFactoryService factoryService,
            IParameterService parameterService,
            IIdentityService identity)
        {
            this._uowFactory = uowFactory;
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._identity = identity;
        }

        public virtual TransferenciaDocumentalResponse DocumentarTransferencia(TransferenciaDocumentalRequest request, IUnitOfWork uow)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var result = new TransferenciaDocumentalResponse();

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

                var documentoTransferencia = GenerarDocumentoTransferencia(docIngreso, docEgreso, request.NroTransferencia);
                uow.DocumentoRepository.AddDocumentoTransferencia(documentoTransferencia);

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

        public virtual TransferenciaDocumentalResponse DocumentarTransferenciaSinPreparacion(TransferenciaDocumentalRequest request, IUnitOfWork uow)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var result = new TransferenciaDocumentalResponse();

            try
            {
                ValidarEmpresasDocumentales(uow, request);

                var egreso = new EgresoDocumental(this._factoryService);
                var tpDocEgreso = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_EGRESO_TRANSFERENCIA, request.EmpresaEgreso);
                var docEgreso = egreso.CrearCabezalEgreso(uow, request.Usuario, request.EmpresaEgreso, null, tpDocEgreso);
                var lineasEgreso = request.LineasEgreso.Select(le => MapToLineaEgresoDocumental(le)).ToList();
                var nuevasLineasEgreso = egreso.GenerarLineasEgresoSinPreparacion(uow, lineasEgreso, request.Usuario);

                docEgreso.OutDetail.AddRange(nuevasLineasEgreso);
                docEgreso.Estado = uow.DocumentoRepository.GetEstadoDestino(docEgreso.Tipo, AccionDocumento.TransferirSinPreparacion);
                
                uow.DocumentoRepository.AddEgreso(docEgreso, nuTransaccion);

                var ingreso = new IngresoDocumental(this._factoryService, this._parameterService, this._identity);
                var tpDocIngreso = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_INGRESO_TRANSFERENCIA, request.EmpresaIngreso);
                var docIngreso = ingreso.CrearCabezalIngreso(request.EmpresaIngreso, tpDocIngreso, uow, request.Usuario);
                var lineasIngreso = request.LineasIngreso.Select(li => MapToLineaIngresoDocumental(li)).ToList();
                var nuevasLineasIngreso = ingreso.GenerarLineasIngreso(uow, lineasIngreso, docIngreso);

                docIngreso.Lineas.AddRange(nuevasLineasIngreso);
                docIngreso.Estado = uow.DocumentoRepository.GetEstadoDestino(docIngreso.Tipo, AccionDocumento.TransferirSinPreparacion);
                
                uow.DocumentoRepository.AddIngreso(docIngreso, nuTransaccion);

                var documentoTransferencia = GenerarDocumentoTransferencia(docIngreso, docEgreso, request.NroTransferencia);
                
                uow.DocumentoRepository.AddDocumentoTransferencia(documentoTransferencia);

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

        public virtual void ValidarEmpresasDocumentales(IUnitOfWork uow, TransferenciaDocumentalRequest request)
        {
            if (!uow.EmpresaRepository.IsEmpresaDocumental(request.EmpresaEgreso))
                throw new ValidationFailedException("General_Sec0_Error_EmpresaNoDocumental", new string[] { request.EmpresaEgreso.ToString() });

            if (!uow.EmpresaRepository.IsEmpresaDocumental(request.EmpresaIngreso))
                throw new ValidationFailedException("General_Sec0_Error_EmpresaNoDocumental", new string[] { request.EmpresaIngreso.ToString() });
        }

        public virtual void ValidarDocumentos(IUnitOfWork uow, TransferenciaDocumentalRequest request, out IDocumentoIngreso docIngreso, out IDocumentoEgreso docEgreso)
        {
            var tpOperativa = TipoOperativaDocumental.Transferencia;
            var docPrep = uow.DocumentoRepository.GetDocumentoPreparacionEstado(request.EmpresaEgreso, tpOperativa, request.Preparacion ?? -1, true);

            if (docPrep == null)
                throw new ValidationFailedException("General_Sec0_Error_AsociacionNoEncontrada", new string[] { tpOperativa, request.Preparacion.ToString(), request.EmpresaEgreso.ToString() });

            docIngreso = uow.DocumentoRepository.GetIngreso(docPrep.NroDocumentoIngreso, docPrep.TpDocumentoIngreso);
            if (docIngreso == null)
                throw new ValidationFailedException("General_Sec0_Error_DocumentoNoExiste", new string[] { docPrep.NroDocumentoIngreso, docPrep.TpDocumentoIngreso });

            docEgreso = uow.DocumentoRepository.GetEgreso(docPrep.NroDocumentoEgreso, docPrep.TpDocumentoEgreso);
            if (docEgreso == null)
                throw new ValidationFailedException("General_Sec0_Error_DocumentoNoExiste", new string[] { docPrep.NroDocumentoEgreso, docPrep.TpDocumentoEgreso });
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

        public virtual DocumentoTransferencia GenerarDocumentoTransferencia(IDocumentoIngreso documentoIngreso, IDocumentoEgreso documentoEgreso, string transferencia)
        {
            var documentoTransferencia = new DocumentoTransferencia()
            {
                DocumentoEgreso = documentoEgreso,
                DocumentoIngreso = documentoIngreso,
                NumeroTransferencia = transferencia
            };

            return documentoTransferencia;
        }
    }
}
