using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WIS.Documento.Execution;
using WIS.Documento.Execution.Serialization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Preparaciones;
using WIS.Domain.Documento.Integracion.Produccion;
using WIS.Domain.Documento.Integracion.Transferencia;
using WIS.Domain.Produccion;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class DocumentoService : IDocumentoService
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IIdentityService _identity;
        protected readonly IParameterService _parameterService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFactoryService _factoryService;
        protected readonly IOptions<DocumentoSettings> _configuration;

        public DocumentoService(
            IIdentityService identity,
            IParameterService parameterService,
            IUnitOfWorkFactory uowFactory,
            IFactoryService factoryService,
            IOptions<DocumentoSettings> configuration)
        {
            this._identity = identity;
            this._parameterService = parameterService;
            this._uowFactory = uowFactory;
            this._factoryService = factoryService;
            this._configuration = configuration;
        }

        public virtual Agenda CrearAgenda(IUnitOfWork uow, IDocumentoIngreso documento, string descDocumentoAduanero, string nroDocumentoAduanero)
        {
			return CrearAgenda(uow, documento, descDocumentoAduanero, nroDocumentoAduanero, documento.Predio);
		}

		public virtual List<Agenda> CrearAgenda(IUnitOfWork uow, IDocumentoAgrupador documento)
        {
            var agendas = new List<Agenda>();

            foreach (var ingreso in documento.LineasIngresoAgrupadas)
            {
                agendas.Add(CrearAgenda(uow, ingreso, null, null, documento.Predio));
            }

            return agendas;
        }

        public virtual Agenda CrearAgenda(IUnitOfWork uow, IDocumentoIngreso documento, string descDocumentoAduanero, string nroDocumentoAduanero, string predio)
        {
            Agenda agenda = null;

            try
            {
                agenda = uow.AgendaRepository.GetAgenda(documento.Numero, documento.Tipo);

                if (agenda == null)
                {
                    var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipoExternoByInterno(documento.Empresa.Value, TipoRecepcionDb.DocumentosAduaneros)?.RecepcionTipoInterno;

                    if (tipoRecepcion == null)
                        throw new ValidationFailedException("General_Sec0_Error_EmpresaSinTipoRecepcion", new string[] { documento.Empresa.Value.ToString(), TipoRecepcionDb.DocumentosAduaneros });

                    agenda = new Agenda();
                    agenda.IdEmpresa = documento.Empresa.Value;
                    agenda.Predio = predio;

                    if (string.IsNullOrEmpty(predio))
                        agenda.Predio = Convert.ToString(_parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL_PREDIO_DEF, documento.Empresa.Value) ?? "1");

                    agenda.TipoDocumento = TipoDocumentoAgendaDb.DocumentoAduanero;
                    agenda.TipoRecepcion = tipoRecepcion;
                    agenda.TipoRecepcionInterno = tipoRecepcion.Tipo;
                    agenda.CodigoInternoCliente = documento.Cliente;
                    agenda.NumeroDocumento = $"{documento.Tipo}{documento.Numero}";
                    agenda.Estado = EstadoAgenda.DocumentoAsociado;
                    agenda.FechaInsercion = DateTime.Now;
                    agenda.FechaInicio = DateTime.Now;
                    agenda.FechaFin = DateTime.Now;
                    agenda.FechaModificacion = DateTime.Now;
                    agenda.CodigoOperacion = OperacionAgendaDb.RecepcionAgrupada;
                    agenda.Averiado = false;
                    agenda.EnviaDocumentacion = false;
                    agenda.Anexo1 = documento.TipoAgrupador + documento.NumeroAgrupador;

                    if (string.IsNullOrEmpty(agenda.Anexo1))
                    {
                        agenda.Anexo1 = string.Format("{0}{1}", nroDocumentoAduanero ?? "", descDocumentoAduanero ?? "");
                    }

                    agenda.DUA = nroDocumentoAduanero;
                    agenda.Detalles = new List<AgendaDetalle>();

                    documento.Lineas.ForEach(lineaDocumento =>
                    {
                        var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(documento.Empresa.Value, lineaDocumento.Producto);
                        DateTime? dt_fabricacion = null;

                        if (producto.DiasDuracion == null)
                            producto.DiasDuracion = 0;

                        if (producto.DiasDuracion > 0)
                            dt_fabricacion = DateTime.Now.Date.AddDays((double)producto.DiasDuracion);

                        var detAgenda = new AgendaDetalle()
                        {
                            IdEmpresa = agenda.IdEmpresa,
                            CodigoProducto = lineaDocumento.Producto,
                            Faixa = 1,
                            Identificador = lineaDocumento.IdentificadorIngreso,
                            CantidadAgendada = lineaDocumento.CantidadIngresada.Value,
                            CantidadAgendadaOriginal = lineaDocumento.CantidadIngresada.Value,
                            Estado = EstadoAgendaDetalle.ConferidaConDiferencias,
                            Vencimiento = dt_fabricacion,
                            NumeroTransaccion = uow.GetTransactionNumber(),
                        };

                        agenda.Detalles.Add(detAgenda);
                    });
                    agenda.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.AgendaRepository.AddAgendaConProblemas(agenda, GetProblemas(agenda));
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }

            return agenda;
        }

        public virtual List<AgendaDetalleProblema> GetProblemas(Agenda agenda)
        {
            List<AgendaDetalleProblema> problemas = new List<AgendaDetalleProblema>();

            foreach (var detalle in agenda.Detalles)
            {
                if (detalle.CantidadAgendada != 0 || detalle.CantidadRecibida != 0 || detalle.CantidadAgendadaOriginal != 0)
                {
                    problemas.Add(new AgendaDetalleProblema()
                    {
                        CodigoProducto = detalle.CodigoProducto,
                        Identificador = detalle.Identificador,
                        Embalaje = detalle.Faixa,
                        TipoProblema = TipoProblemaAgendaDetalle.Problema,
                        Problema = ProblemaAgendaDetalle.RecibidoMenorAgendado,
                        Diferencia = detalle.CantidadAgendada,
                        Funcionario = this._identity.UserId,
                        Aceptado = false
                    });
                }
            }

            return problemas;
        }

        public virtual void ValidarCancelacionIngreso(IUnitOfWork uow, IDocumentoAgrupador documento)
        {
            var agendas = documento.LineasIngresoAgrupadas.Select(s => s.Agenda);

            foreach (var nroAgenda in agendas)
            {
                var agenda = uow.AgendaRepository.GetAgenda(nroAgenda.Value);
                if (agenda != null && agenda.EstadoId != EstadoAgendaDb.Cancelada)
                {
                    throw new ValidationFailedException("General_Sec0_Error_CancelarIngresoSituacion");
                }
            }
        }

        public virtual void HabilitarCargaYCierreCamion(IUnitOfWork uow, IDocumentoEgreso documento)
        {
            var camion = uow.CamionRepository.GetCamion(documento.Camion.Value);

            if (camion == null)
                throw new ValidationFailedException("General_Sec0_Error_EgresoHabilitarCargaCamionNoExiste");

            camion.IsCargaHabilitada = true;
            camion.IsCierreHabilitado = true;

            uow.CamionRepository.UpdateCamion(camion);
        }

        public virtual ReservaDocumentalResponse DesreservarStock(IUnitOfWork uow, List<Stock> stockReservado)
        {
            if (!string.IsNullOrEmpty(this._configuration.Value.ReservaDocumentalEndpoint))
            {
                try
                {
                    string uri = this._configuration.Value.ReservaDocumentalEndpoint;
                    using (var client = new HttpClient())
                    {
                        var reservaRequest = CrearDesreservaDocumentalRequest(uow, stockReservado);
                        var reservaWrapper = new ReservaDocumentalWrapper()
                        {
                            User = this._identity.UserId,
                            Application = this._identity.Application
                        };

                        reservaWrapper.SetData(reservaRequest);

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var address = new Uri(uri);

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);
                        request.Method = HttpMethod.Post;
                        request.Content = new StringContent(JsonConvert.SerializeObject(reservaWrapper), Encoding.UTF8, "application/json");

                        request.Headers.ConnectionClose = true;

                        var response = client.SendAsync(request).GetAwaiter().GetResult();

                        if (!response.IsSuccessStatusCode)
                            throw new Exception("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + response.Content.ReadAsStringAsync());

                        string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var resultWrapper = JsonConvert.DeserializeObject<ReservaDocumentalWrapper>(result);

                        return resultWrapper.GetData<ReservaDocumentalResponse>();
                    }
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw new Exception("Manejo Documental: " + ex.Message, ex);
                }
            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_AppSettingNoDefinido", new string[] { $"{DocumentoSettings.Position}.{nameof(DocumentoSettings.ReservaDocumentalEndpoint)}" });
            }
        }

        public virtual ReservaDocumentalRequest CrearDesreservaDocumentalRequest(IUnitOfWork uow, List<Stock> stockReservado)
        {
            ReservaDocumentalRequest request = new ReservaDocumentalRequest()
            {
                Reservas = new List<LineaReservaDocumentalRequest>()
            };

            List<int> empresasDocumentales = new List<int>();

            //Verificar el stock a desreservar,ver que empresa tiene manejo documental
            foreach (var stock in stockReservado)
            {
                var manejoDocumental = this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, stock.Empresa);

                if (manejoDocumental == "S" && !empresasDocumentales.Any(a => a == stock.Empresa))
                {
                    empresasDocumentales.Add(stock.Empresa);
                }
            }

            //Filtrar stock, solo se manda stock de emrpesas con manejo documental
            var stockManejoDocumental = stockReservado
                .Where(s => empresasDocumentales.Contains(s.Empresa))
                .ToList();

            var stockAgrupado = stockManejoDocumental
                .GroupBy(s => new { s.Producto, s.Empresa, s.Identificador, s.Faixa })
                .ToList();

            foreach (var grupo in stockAgrupado)
            {
                var cantidadDesreservar = grupo.Sum(s => s.ReservaSalida ?? 0);

                request.Reservas.Add(new LineaReservaDocumentalRequest()
                {
                    CantidadAfectada = -cantidadDesreservar,
                    Empresa = grupo.FirstOrDefault().Empresa,
                    Identificador = grupo.FirstOrDefault().Identificador,
                    Producto = grupo.FirstOrDefault().Producto
                });
            }

            return request;
        }

        public virtual ProduccionDocumentalResponse CrearDocumentos(IUnitOfWork uow, IngresoWhiteBox produccion)
        {
            if (!string.IsNullOrEmpty(this._configuration.Value.CrearDocumentosEndpoint))
            {
                try
                {
                    string uri = this._configuration.Value.CrearDocumentosEndpoint;
                    using (var client = new HttpClient())
                    {
                        var documentosRequest = CrearProduccionDocumentalRequest(uow, produccion);
                        var documentosWrapper = new ProduccionDocumentalWrapper()
                        {
                            User = this._identity.UserId,
                            Application = this._identity.Application
                        };

                        documentosWrapper.SetData(documentosRequest);

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var address = new Uri(uri);

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);
                        request.Method = HttpMethod.Post;
                        request.Content = new StringContent(JsonConvert.SerializeObject(documentosWrapper), Encoding.UTF8, "application/json");

                        request.Headers.ConnectionClose = true;

                        var response = client.SendAsync(request).GetAwaiter().GetResult();

                        if (!response.IsSuccessStatusCode)
                            throw new Exception("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + response.Content.ReadAsStringAsync());

                        string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var resultWrapper = JsonConvert.DeserializeObject<ProduccionDocumentalWrapper>(result);

                        return resultWrapper.GetData<ProduccionDocumentalResponse>();
                    }
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw new Exception("Manejo Documental: " + ex.Message, ex);
                }
            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_AppSettingNoDefinido", new string[] { $"{DocumentoSettings.Position}.{nameof(DocumentoSettings.CrearDocumentosEndpoint)}" });
            }
        }

        public virtual ProduccionDocumentalRequest CrearProduccionDocumentalRequest(IUnitOfWork uow, IngresoWhiteBox produccion)
        {
			int empresaInt = (int)produccion.Empresa;

			ProduccionDocumentalRequest request = new ProduccionDocumentalRequest()
            {
                Aplicacion = this._identity.Application,
                Usuario = this._identity.UserId,
                Empresa = empresaInt,
                NroProduccion = produccion.Id,
                LineasEgreso = new List<LineaEgresoDocumentalRequest>(),
                LineasIngreso = new List<LineaIngresoDocumentalRequest>()
            };

            //Detalles de entrada, tomar linea producido confirmadas (-4 = descartada)
            var producidoAgrupado = produccion.Producidos
                .Where(p => p.NuTransaccion != -4)
                .GroupBy(p => new { p.Producto, p.Identificador, p.Empresa, p.Faixa })
                .ToList();

            foreach (var producido in producidoAgrupado)
            {
				int empresaIntProducido = (int)producido.FirstOrDefault().Empresa;

				request.LineasIngreso.Add(new LineaIngresoDocumentalRequest()
                {
                    CantidadAfectada = producido.Sum(p => p.QtProducido),
                    Empresa = empresaIntProducido,
                    Identificador = producido.FirstOrDefault().Identificador,
                    Producto = producido.FirstOrDefault().Producto,
                    Faixa = producido.FirstOrDefault().Faixa
                });
            }

            //Detalles de salida,  tomar linea consumido confirmadas (-4 = descartada)
            var consumidoAgrupado = produccion.Consumidos
                .Where(c => c.NuTransaccion != -4)
                .GroupBy(c => new { c.Producto, c.Identificador, c.Empresa, c.Faixa })
                .ToList();

            foreach (var consumido in consumidoAgrupado)
            {
				int empresaIntConsumido = (int)consumido.FirstOrDefault().Empresa;

				request.LineasEgreso.Add(new LineaEgresoDocumentalRequest()
                {
                    CantidadAfectada = consumido.Sum(c => c.QtReal),
                    Empresa = empresaIntConsumido,
                    Identificador = consumido.FirstOrDefault().Identificador,
                    Producto = consumido.FirstOrDefault().Producto,
                    Faixa = consumido.FirstOrDefault().Faixa
                });
            }

            return request;
        }

        public virtual CambioLoteResponse CambiarLote(CambioLoteRequest request)
        {
            var result = new CambioLoteResponse();

            if (request.Cantidad > 0)
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    try
                    {
                        uow.CreateTransactionNumber("DocumentoService CambiarLote");
                        uow.BeginTransaction();

                        var nuTransaccion = uow.GetTransactionNumber();
                        var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(request.Empresa, request.Producto);
                        var loteOrigen = producto.ParseIdentificador(request.LoteOrigen.Trim().ToUpper());
                        var loteDestino = producto.ParseIdentificador(request.LoteDestino.Trim().ToUpper());

                        if (loteOrigen != loteDestino)
                        {
                            var cantidadPendienteReasignar = request.Cantidad;
                            var documentos = uow.DocumentoRepository.GetDocumentosCambioLote(request.Producto, request.Empresa, loteOrigen, request.Cantidad);

                            foreach (var documento in documentos)
                            {
                                DocumentoLinea lineaDestino = null;
                                bool existeLineaDestino = uow.DocumentoRepository.AnyDetalleDocumento(documento.Numero, documento.Tipo, request.Empresa, request.Producto, loteDestino, 1);

                                if (existeLineaDestino)
                                    lineaDestino = uow.DocumentoRepository.GetDetalleDocumento(request.Producto, 1, loteDestino, request.Empresa, documento.Numero, documento.Tipo);

                                decimal cantidadDesafectadaLineaDestino = lineaDestino?.CantidadDesafectada ?? 0;
                                decimal cantidadDescargadaLineaDestino = lineaDestino?.CantidadDescargada ?? 0;
                                decimal cantidadIngresadaLineaDestino = lineaDestino?.CantidadIngresada ?? 0;
                                decimal cantidadReservadaLineaDestino = lineaDestino?.CantidadReservada ?? 0;
                                decimal cifLineaDestino = lineaDestino?.CIF ?? 0;
                                decimal fobLineaDestino = lineaDestino?.ValorMercaderia ?? 0;
                                decimal tributoLineaDestino = lineaDestino?.ValorTributo ?? 0;

                                foreach (var lineaOrigen in documento.Lineas)
                                {
                                    decimal cantidadIngresada = (lineaOrigen.CantidadIngresada ?? 0) == 0 ? 1 : lineaOrigen.CantidadIngresada.Value;
                                    decimal cifUnitario = (lineaOrigen.CIF ?? 0) / cantidadIngresada;
                                    decimal fobUnitario = (lineaOrigen.ValorMercaderia ?? 0) / cantidadIngresada;
                                    decimal tributoUnitario = (lineaOrigen.ValorTributo ?? 0) / cantidadIngresada;

                                    if (lineaDestino == null)
                                        lineaDestino = lineaOrigen.Clone();

                                    var cantidadDisponible = lineaOrigen.GetCantidadDisponible();

                                    if (cantidadPendienteReasignar >= cantidadDisponible)
                                    {
                                        cifLineaDestino += cifUnitario * cantidadDisponible;
                                        fobLineaDestino += fobUnitario * cantidadDisponible;
                                        tributoLineaDestino += tributoUnitario * cantidadDisponible;

                                        lineaOrigen.CantidadIngresada -= cantidadDisponible;
                                        lineaOrigen.CIF = cifUnitario * lineaOrigen.CantidadIngresada;
                                        lineaOrigen.ValorMercaderia = fobUnitario * lineaOrigen.CantidadIngresada;
                                        lineaOrigen.ValorTributo = tributoUnitario * lineaOrigen.CantidadIngresada;

                                        cantidadIngresadaLineaDestino += cantidadDisponible;
                                        cantidadPendienteReasignar -= cantidadDisponible;

                                        if ((lineaOrigen.CantidadIngresada ?? 0) == 0)
                                        {
                                            lineaOrigen.FechaModificacion = DateTime.Now;
                                            lineaOrigen.NumeroTransaccionDelete = nuTransaccion;

                                            uow.DocumentoRepository.UpdateDetail(documento, lineaOrigen, nuTransaccion);
                                            uow.SaveChanges();

                                            uow.DocumentoRepository.RemoveDetail(documento, lineaOrigen, nuTransaccion);
                                        }
                                        else
                                        {
                                            uow.DocumentoRepository.UpdateDetail(documento, lineaOrigen, nuTransaccion);
                                        }
                                    }
                                    else
                                    {
                                        cifLineaDestino += cifUnitario * cantidadPendienteReasignar;
                                        fobLineaDestino += fobUnitario * cantidadPendienteReasignar;
                                        tributoLineaDestino += tributoUnitario * cantidadPendienteReasignar;

                                        lineaOrigen.CantidadIngresada -= cantidadPendienteReasignar;
                                        lineaOrigen.CIF = cifUnitario * lineaOrigen.CantidadIngresada;
                                        lineaOrigen.ValorMercaderia = fobUnitario * lineaOrigen.CantidadIngresada;
                                        lineaOrigen.ValorTributo = tributoUnitario * lineaOrigen.CantidadIngresada;

                                        cantidadIngresadaLineaDestino += cantidadPendienteReasignar;
                                        cantidadPendienteReasignar = 0;

                                        uow.DocumentoRepository.UpdateDetail(documento, lineaOrigen, nuTransaccion);

                                        break;
                                    }
                                }

                                if (cantidadIngresadaLineaDestino > 0)
                                {
                                    lineaDestino.Identificador = loteDestino;
                                    lineaDestino.IdentificadorIngreso = loteDestino;
                                    lineaDestino.CantidadDesafectada = cantidadDesafectadaLineaDestino;
                                    lineaDestino.CantidadDescargada = cantidadDescargadaLineaDestino;
                                    lineaDestino.CantidadIngresada = cantidadIngresadaLineaDestino;
                                    lineaDestino.CantidadReservada = cantidadReservadaLineaDestino;
                                    lineaDestino.CIF = cifLineaDestino;
                                    lineaDestino.ValorMercaderia = fobLineaDestino;
                                    lineaDestino.ValorTributo = tributoLineaDestino;

                                    if (existeLineaDestino)
                                        uow.DocumentoRepository.UpdateDetail(documento, lineaDestino, nuTransaccion);
                                    else
                                        uow.DocumentoRepository.AddDetail(documento, lineaDestino, nuTransaccion);

                                    result.Documentos.Add(new DocumentoCambioLoteResponse(documento.Tipo, documento.Numero));
                                }

                                if (cantidadPendienteReasignar == 0)
                                    break;
                            }

                            uow.SaveChanges();
                            uow.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        uow.Rollback();
                        _logger.Error(ex, ex.Message);
                        result.Success = false;
                        result.ErrorMsg = ex.Message;
                    }
                }
            }

            return result;
        }

        public virtual ProduccionDocumentalResponse DocumentarProduccion(ProduccionDocumentalRequest request)
        {
            var produccion = new ProduccionDocumental(this._uowFactory, this._factoryService, this._parameterService, this._identity);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("DocumentoService DocumentarProduccion");
                    uow.BeginTransaction();

                    var response = produccion.DocumentarProduccion(request, uow);

                    uow.SaveChanges();
                    uow.Commit();

                    return response;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw ex;
                }
            }
        }

        public virtual ReservaDocumentalResponse AfectarReservaDocumental(ReservaDocumentalRequest request)
        {
            var preparacion = new PreparacionDocumental(this._uowFactory);
            return preparacion.ReservaDocumentalPicking(request);
        }

        public virtual TransferenciaDocumentalResponse DocumentarTransferencia(TransferenciaDocumentalRequest request)
        {
            var transferencia = new TransferenciaDocumental(this._uowFactory, this._factoryService, this._parameterService, this._identity);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("DocumentoService DocumentarTransferencia");
                    uow.BeginTransaction();

                    var response = transferencia.DocumentarTransferencia(request, uow);

                    uow.SaveChanges();
                    uow.Commit();

                    return response;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw ex;
                }
            }
        }

        public virtual TransferenciaDocumentalResponse DocumentarTransferenciaSinPreparacion(TransferenciaDocumentalRequest request)
        {
            var transferencia = new TransferenciaDocumental(this._uowFactory, this._factoryService, this._parameterService, this._identity);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("DocumentoService DocumentarTransferenciaSinPreparacion");
                    uow.BeginTransaction();

                    var response = transferencia.DocumentarTransferenciaSinPreparacion(request, uow);

                    uow.SaveChanges();
                    uow.Commit();

                    return response;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw ex;
                }
            }
        }

        public virtual ModificarReservaDocumentalResponse ModificarReservaDocumental(ModificarReservaDocumentalRequest request)
        {
            var preparacion = new PreparacionDocumental(this._uowFactory);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("DocumentoService ModificarReservaDocumental");
                    uow.BeginTransaction();

                    var response = preparacion.ModificarReservaDocumental(uow, request);

                    uow.SaveChanges();
                    uow.Commit();

                    return response;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw ex;
                }
            }
        }
    }
}
