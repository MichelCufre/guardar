using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking;
using WIS.Domain.Reportes;
using WIS.Domain.Reportes.Especificaciones;
using WIS.Domain.Reportes.Setups;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class CierreEgreso
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IDapper _dapper;
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly IReportKeyService _reporteKeyService;
        protected readonly IBarcodeService _barcodeService;

        protected readonly int _userId;
        protected readonly Camion _camion;
        protected readonly ExpedicionConfiguracionService _expedicionService;
        protected readonly ConfiguracionExpedicion _configuracion;
        protected readonly FacturacionContenedorLegacy _facturacionContenedor;

        protected readonly ITaskQueueService _taskQueue;

        public CierreEgreso(
            IUnitOfWork uow,
            Camion camion,
            IDapper dapper,
            IParameterService parameterService,
            IIdentityService identity,
            IFactoryService factoryService,
            IReportKeyService reporteKeyService,
            IBarcodeService barcodeService,
            ITaskQueueService taskQueue)
        {
            this._uow = uow;
            this._camion = camion;
            this._dapper = dapper;
            this._identity = identity;
            this._factoryService = factoryService;
            this._barcodeService = barcodeService;
            this._parameterService = parameterService;
            this._reporteKeyService = reporteKeyService;
            this._parameterService = parameterService;
            this._expedicionService = new ExpedicionConfiguracionService(uow, parameterService, new ParametroMapper());
            this._configuracion = _expedicionService.GetConfiguracion();            
            this._facturacionContenedor = new FacturacionContenedorLegacy(uow, camion);
            this._expedicionService = new ExpedicionConfiguracionService(uow, parameterService, new ParametroMapper());
            this._configuracion = _expedicionService.GetConfiguracion();
            this._taskQueue = taskQueue;
        }

        public virtual List<ValidacionCamionResultado> Cerrar()
        {
            var resultadoValidacion = new List<ValidacionCamionResultado>();

            if (!this._uow.CamionRepository.CamionVacio(this._camion.Id))
            {
                resultadoValidacion = Validate();

                if (resultadoValidacion.Any())
                    return resultadoValidacion;

                if (this._camion.IsPendienteFacturar())
                {
                    this._facturacionContenedor.Facturar();
                    this._camion.NumeroInterfazEjecucionFactura = 0;
                }

                if (this._expedicionService.IsManejoDocumentalHabilitado(this._camion.Empresa ?? -1))
                {
                    var egreso = new EgresoDocumental(this._factoryService);
                    egreso.FinalizarEgresoPorCamion(this._uow, this._camion);
                }

                this._uow.CamionRepository.UpdateCamion(this._camion);
                this._uow.SaveChanges();

                this._camion.Cerrar();
                ProcesarCerrarCamion(_camion, _uow.GetTransactionNumber());
            }
            else
            {
                this._camion.Cerrar();
                this._uow.CamionRepository.UpdateCamion(this._camion);
                this._uow.SaveChanges();
            }

            return resultadoValidacion;
        }

        public virtual List<ValidacionCamionResultado> Validate()
        {
            var resultado = new List<ValidacionCamionResultado>();

            if (!ValidarPreparaciones(out ValidacionCamionResultado result))
            {
                resultado.Add(result);
                return resultado;
            }

            if (this._camion.IsCerrado())
                throw new ValidationFailedException("General_Sec0_Error_Er196_CamionCerrado");

            if (_camion.IsControlContenedoresHabilitado && _uow.PreparacionRepository.AnyContenedorSinControl(_camion, out int cantCont))
                throw new ValidationFailedException("General_Sec0_Error_ContenedoreSinControlar", new string[] { cantCont.ToString() });
            else if (_uow.PreparacionRepository.AnyContenedorSinFinalizarControl(_camion, out cantCont))
                throw new ValidationFailedException("General_Sec0_Error_ContenedoreSinFinalizarControl", new string[] { cantCont.ToString() });

            if (this._configuracion.IsControlFacturacionRequerido && _uow.CamionRepository.RequiereFacturacion(_camion.Id) && !this._camion.IsFacturado())
                throw new ValidationFailedException("WEXP040_Sec0_Error_Er001_CamionDebeEstarFacturado");

            if (this._camion.IsFacturacionEnProceso())
                throw new ValidationFailedException("General_Sec0_Error_Er117_FacturacionEnProceso");

            if (this._camion.Puerta == null)
                throw new ValidationFailedException("General_Sec0_Error_Er197_CamionSinPuerta");

            if (this._expedicionService.IsManejoDocumentalHabilitado(this._camion.Empresa ?? -1))
            {
                if (!this._camion.IsCierreHabilitado)
                    throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalCamionSinCierreHabilitado");

                // Si es camión documental debe estar todo preparado y cargado
                this._camion.IsCierreParcialHabilitado = false;
            }

            if (!this._camion.IsCierreParcialHabilitado)
            {
                if (this._uow.PreparacionRepository.AnyProductoSinPreparar(this._camion.Id))
                    throw new ValidationFailedException("General_Sec0_Error_Er202_PickeosPendientes");

                if (!this._uow.PreparacionRepository.AnyContenedorEmbarcado(this._camion.Id))
                    throw new ValidationFailedException("General_Sec0_Error_Er200_ContNoCargadosCamion");

                if (this._uow.PreparacionRepository.AnyContenedorSinEmbarcar(this._camion.Id))
                    throw new ValidationFailedException("General_Sec0_Error_Er201_ContPendientesEnvios");
            }

            return resultado;
        }

        public virtual void UpdateContenedores(string ubicacion)
        {
            List<Contenedor> list = this._uow.ContenedorRepository.GetContenedoreEnPuerta(this._camion.Id, ubicacion);

            foreach (var contenedor in list)
            {
                contenedor.Estado = EstadoContenedor.Enviado;
                contenedor.NumeroTransaccion = this._uow.GetTransactionNumber();
                contenedor.FechaExpedido = DateTime.Now;

                this._uow.ContenedorRepository.UpdateContenedor(contenedor);

                if (contenedor.NroLpn != null)
                    this._uow.ManejoLpnRepository.ExpedirLpn(contenedor.NroLpn.Value, contenedor.Ubicacion, _uow.GetTransactionNumber());

                if (this._uow.ContenedorRepository.TipoContenedorEnvase(contenedor.TipoContenedor))
                {
                    Envase envase = this._uow.EnvaseRepository.GetEnvase(contenedor.IdExterno, contenedor.TipoContenedor);

                    Agente agente = this._uow.AgenteRepository.GetAgenteContenedor(contenedor.Numero, contenedor.NumeroPreparacion);

                    string pedidosDelContenedor = string.Join("-", this._uow.PreparacionRepository.GetNumerosPedidosDeUnContenedor(contenedor.Numero, contenedor.NumeroPreparacion));

                    if (envase == null)
                    {
                        envase = new Envase
                        {
                            Id = contenedor.IdExterno,
                            TipoEnvase = contenedor.TipoContenedor,
                            Estado = EstadoEnvase.Expedido,
                            CodigoBarras = contenedor.CodigoBarras,
                            CodigoAgente = agente.Codigo,
                            TipoAgente = agente.Tipo,
                            Empresa = agente.Empresa,
                            NumeroTransaccion = this._uow.GetTransactionNumber(),
                            FechaUltimaExpedicion = DateTime.Now,
                            UsuarioUltimaExpedicion = this._identity.UserId,
                            DescripcionUltimoMovimiento = ($"Expedido => Camión: {this._camion.Id}; Preparación: {contenedor.NumeroPreparacion}; Pedidos: {pedidosDelContenedor};").Truncate(200)
                        };

                        this._uow.EnvaseRepository.AddEnvase(envase);

                    }
                    else
                    {
                        envase.NumeroTransaccion = this._uow.GetTransactionNumber();
                        envase.Estado = EstadoEnvase.Expedido;
                        envase.FechaUltimaExpedicion = DateTime.Now;
                        envase.UsuarioUltimaExpedicion = this._identity.UserId;
                        envase.DescripcionUltimoMovimiento = ($"Expedido => Camión: {this._camion.Id}; Preparación: {contenedor.NumeroPreparacion}; Pedidos: {pedidosDelContenedor};").Truncate(200);

                        this._uow.EnvaseRepository.UpdateEnvase(envase);
                    }
                }
            }
            foreach (var ut in list.Where(x => x.NumeroUnidadTransporte != null).GroupBy(x => x.NumeroUnidadTransporte).Select(x => x.Key).ToList())
            {
                var unidadTranporte = this._uow.ContenedorRepository.GetUnidadTransporte(ut);
                unidadTranporte.Situacion = SituacionDb.ContenedorEnviado;
                this._uow.ContenedorRepository.UpdateUnidadTransporte(unidadTranporte);
            }
        }

        public virtual void UpdateStock(string ubicacion)
        {
            this._logger.Debug($"INICIO => UpdateStock");

            var nuTransaccion = this._uow.GetTransactionNumber();
            var productos = this._uow.StockRepository.GetStockContePuertaCamion(this._camion.Id, ubicacion);

            this._logger.Debug($"Cantidad productos => {productos.Count()}");

            foreach (var producto in productos)
            {
                _logger.Debug($"Datos Producto: {JsonConvert.SerializeObject(producto)}");

                this._uow.StockRepository.UpdateStock(producto.CodigoUbicacion, producto.CodigoEmpresa, producto.CodigoProducto, producto.CodigoFaixa, producto.Lote, (producto.CantidadPreparada ?? 0), alta: false, modificarReserva: true);

                this._logger.Debug($"INICIO => UpdateOrCreateDetallePedidoExpedido");
                this.UpdateOrCreateDetallePedidoExpedido(producto);

                this._logger.Debug($"INICIO => SaveChanges");
                this._uow.SaveChanges();
                this._logger.Debug($"FIN => SaveChanges");
            }

            this._uow.SaveChanges();
            this._logger.Debug($"FIN => UpdateStock");
        }

        public virtual void UpdateOrCreateDetallePedidoExpedido(ProductoContenedorPuerta producto)
        {
            DetallePedidoExpedido detallePedidoExpedido = this._uow.PedidoRepository.GetDetallePedidoExpedido(this._camion.Id, producto.NumeroPedido, producto.CodigoCliente, producto.CodigoEmpresa,
                    producto.CodigoProducto, producto.CodigoFaixa, producto.Lote, producto.EspecificaLote);

            if (detallePedidoExpedido == null)
            {
                detallePedidoExpedido = new DetallePedidoExpedido
                {
                    Camion = this._camion.Id,
                    Pedido = producto.NumeroPedido,
                    Cliente = producto.CodigoCliente,
                    Empresa = producto.CodigoEmpresa,
                    Producto = producto.CodigoProducto,
                    Faixa = producto.CodigoFaixa,
                    Identificador = producto.Lote,
                    EspecificaLote = producto.EspecificaLote,
                    Cantidad = producto.CantidadPreparada,
                    FechaExpedicion = DateTime.Now
                };

                this._uow.PedidoRepository.AddDetallePedidoExpedido(detallePedidoExpedido);
            }
            else
            {
                detallePedidoExpedido.Cantidad = (detallePedidoExpedido.Cantidad ?? 0) + producto.CantidadPreparada;
                detallePedidoExpedido.FechaExpedicion = DateTime.Now;

                this._uow.PedidoRepository.UpdateDetallePedidoExpedido(detallePedidoExpedido);
            }
        }

        protected virtual void DeleteCargas()
        {
            List<CargaCamion> ListaClienteCamion = this._uow.CargaCamionRepository.GetsCargasCamion(this._camion.Id);

            foreach (var clienteCamion in ListaClienteCamion)
            {
                this._uow.CargaCamionRepository.DeleteCargaCamiones(clienteCamion);
            }
        }

        public virtual bool ValidarPreparaciones(out ValidacionCamionResultado result)
        {
            List<long> cargas = _camion.Cargas.Select(d => d.Carga).ToList();
            List<DetallePreparacion> lineasPicking = this._uow.PreparacionRepository.GetDetallePreparacionByCarga(cargas);

            string msg = "Las siguientes preparaciones no estan finalizadas:";
            var datos = new List<string>();

            var preps = lineasPicking.GroupBy(p => p.NumeroPreparacion).Distinct().Select(p => p.Key).ToList();
            var prepNofinalizadas = _uow.PreparacionRepository.GetPrepManualSinFinalizar(preps);

            foreach (var pick in prepNofinalizadas)
            {
                datos.Add($"Nro: {pick.Id} - {pick.Descripcion}");
            }
            result = new ValidacionCamionResultado(msg, datos);

            if (datos.Any())
                return false;
            else
                return true;
        }

        public virtual void ExpedirEnvases(IUnitOfWork uow, Camion camion)
        {
            var expedicionEnvase = new ExpedicionEnvase(_identity, _barcodeService, uow, camion);

            expedicionEnvase.Expedir();
        }

        public virtual List<long> GenerarReportes(IUnitOfWork uow, Camion camion)
        {
            var reports = new List<long>();

            reports.AddRange(this.CrearReportePackingList(uow, camion));
            reports.AddRange(this.CrearReportePackingListSinLpn(uow, camion));
            reports.AddRange(this.CrearReporteContenedoresCamion(uow, camion));
            reports.AddRange(this.CrearReporteControlCambio(uow, camion));

            uow.SaveChanges();

            return reports;
        }

        public virtual List<long> CrearReportePackingList(IUnitOfWork uow, Camion camion)
        {
            var reportSetup = new PackingListReportSetup(uow, this._parameterService, this._reporteKeyService, this._identity, camion, camion.Predio);
            return reportSetup.Preparar();
        }

        public virtual List<long> CrearReportePackingListSinLpn(IUnitOfWork uow, Camion camion)
        {
            var reportSetup = new PackingListSinLpnReportSetup(uow, this._parameterService, this._reporteKeyService, this._identity, camion, camion.Predio);
            return reportSetup.Preparar();
        }

        public virtual List<long> CrearReporteContenedoresCamion(IUnitOfWork uow, Camion camion)
        {
            var reportSetup = new ContenedoresCamionReportSetup(uow, this._parameterService, this._reporteKeyService, this._identity, camion, camion.Predio);
            return reportSetup.Preparar();
        }

        public virtual List<long> CrearReporteControlCambio(IUnitOfWork uow, Camion camion)
        {
            var reportSetup = new ControlCambioReportSetup(uow, this._parameterService, this._reporteKeyService, this._identity, camion, camion.Predio);
            return reportSetup.Preparar();
        }

        public virtual void CierreCamionAuto()
        {
            _uow.CreateTransactionNumber($"Proceso de Cierre Camion Auto");
            _uow.BeginTransaction();

            var nuTransaccion = _uow.GetTransactionNumber();
            var reportes = new List<long>();

            try
            {
                _camion.NumeroTransaccion = nuTransaccion;

                ProcesarCerrarCamion(_camion, nuTransaccion);

                ExpedirEnvases(_uow, _camion);

                reportes = GenerarReportes(_uow, _camion);

                if (this._expedicionService.IsManejoDocumentalHabilitado(this._camion.Empresa ?? -1))
                {
                    var egreso = new EgresoDocumental(this._factoryService);
                    egreso.FinalizarEgresoPorCamion(this._uow, this._camion);
                }

                _uow.SaveChanges();
                _uow.Commit();
            }
            catch (Exception ex)
            {
                _uow.Rollback();
                throw ex;
            }

            if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
                _taskQueue.Enqueue(TaskQueueCategory.REPORT, reportes.Select(x => x.ToString()).ToList());
        }

        public virtual void ProcesarCerrarCamion(Camion camion, long nuTransaccion)
        {
            _uow.CamionRepository.UpdateCamionInicioCierre(camion.Id, SituacionDb.CamionIniciandoCierre, DateTime.Now, nuTransaccion);

            string ubicacionPuerta = _uow.PuertaEmbarqueRepository.GetUbicacionPuerta(camion.Puerta);

            _uow.CamionRepository.UpdateStockExpedicionCamion(camion.Id, ubicacionPuerta, nuTransaccion);

            _uow.CamionRepository.AddDetalleExpedicion(camion.Id);
            _uow.CamionRepository.RemoveClienteCamion(camion.Id);
            _uow.CamionRepository.UpdateExpedirContenedor(camion.Id, ubicacionPuerta, SituacionDb.ContenedorEnCamion, SituacionDb.ContenedorEnviado, nuTransaccion, out IEnumerable<long> nuLpns);
            _uow.CamionRepository.UpdateExpedirUt(camion.Id, ubicacionPuerta, SituacionDb.ContenedorEnviado, nuTransaccion);
            _uow.ManejoLpnRepository.ExpedirLpns(nuLpns, ubicacionPuerta, nuTransaccion);

            _uow.CamionRepository.UpdateEntregaNoCargadas(camion.Id);
            _uow.CamionRepository.UpdateEntregaExpedir(camion.Id);
            _uow.CamionRepository.UpdateCamionCierre(camion.Id, SituacionDb.CamionCerrado, DateTime.Now, nuTransaccion);
        }

    }
}
