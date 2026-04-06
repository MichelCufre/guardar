using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Impresiones;
using WIS.Domain.Impresiones.Dtos;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking
{
    public class EmpaquetadoPickingLogic
    {
        protected readonly IPrintingService _printingService;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IIdentityService _identity;
        protected Logger Logger;

        public EmpaquetadoPickingLogic(IPrintingService printingService, ITrackingService trackingService, IBarcodeService barcodeService, IIdentityService identity, Logger logger)
        {
            _printingService = printingService;
            _trackingService = trackingService;
            _barcodeService = barcodeService;
            _identity = identity;
            Logger = logger;
        }

        public virtual ContenedorDestinoData GetContenedorDestinoData(IUnitOfWork uow, string contenedorDestino)
        {
            _barcodeService.ValidarEtiquetaContenedor(contenedorDestino, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            return uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(datosContenedor.NuContenedor, datosContenedor.NuPreparacion);
        }

        public virtual bool PedidoIniciadosEnContenedores(IUnitOfWork uow, string nuContenedor, out string idsExternosContenedores)
        {
            idsExternosContenedores = string.Empty;

            _barcodeService.ValidarEtiquetaContenedor(nuContenedor, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            if (datosContenedor != null)
                idsExternosContenedores = uow.EmpaquetadoPickingRepository.PedidoIniciadosEnContenedores(datosContenedor.NuContenedor, datosContenedor.NuPreparacion);

            return string.IsNullOrEmpty(idsExternosContenedores) ? false : true;
        }

        public virtual DatosClientePedidoOriginal GetDatosPedidoOriginal(IUnitOfWork uow, string nuContenedor, out Contenedor contenedorOrigen)
        {
            contenedorOrigen = null;
            DatosClientePedidoOriginal data = null;

            _barcodeService.ValidarEtiquetaContenedor(nuContenedor, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            if (datosContenedor != null)
            {
                contenedorOrigen = uow.ContenedorRepository.GetContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);
                data = uow.EmpaquetadoPickingRepository.GetDatosClientePedidoOriginal(datosContenedor.NuContenedor, datosContenedor.NuPreparacion);
            }

            return data;
        }

        public virtual Contenedor CreoContenedorEmpaquetado(IUnitOfWork uow, ConfiguracionInicial configuracionInicial, DatosClientePedidoOriginal dataClientePedido, string prodLeido, int nuPreparacion, int nuContenedor, int userId)
        {
            var contenedorOrigen = uow.ContenedorRepository.GetContenedor(nuPreparacion, nuContenedor);

            var qtBulto = uow.ContenedorRepository.GetQtBulto(nuPreparacion, dataClientePedido.CompartContenedorEntrega, dataClientePedido.CodigoCliente, dataClientePedido.NumeroPedido, dataClientePedido.Direccion);
            qtBulto = qtBulto == 0 ? 1 : qtBulto;

            var tipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W;
            string idExterno;
            string codigoBarras;
            do
            {
                idExterno = uow.ContenedorRepository.GetUltimaSecuenciaTipoContenedor(tipoContenedor).ToString();
                codigoBarras = _barcodeService.GenerateBarcode(idExterno, tipoContenedor);
            }
            while (uow.ContenedorRepository.ExisteContenedorActivoByCodigoBarras(codigoBarras));

            var contenedorNuevo = new Contenedor
            {
                NumeroPreparacion = nuPreparacion,
                Numero = uow.ContenedorRepository.GetNextNuContenedor(),
                TipoContenedor = tipoContenedor,
                Estado = EstadoContenedor.EnPreparacion,
                Ubicacion = configuracionInicial.Ubicacion,
                CodigoSubClase = contenedorOrigen.CodigoSubClase,
                FechaAgregado = DateTime.Now,
                IdContenedorEmpaque = "S",
                CantidadBulto = qtBulto,
                NumeroTransaccion = uow.GetTransactionNumber(),
                IdExterno = idExterno,
                CodigoBarras = codigoBarras,
            };

            uow.ContenedorRepository.AddContenedor(contenedorNuevo);

            return contenedorNuevo;
        }

        public virtual DatosClientePedidoOriginal GetDatosByPedidoSelected(IUnitOfWork uow, string rowSelectedPedProdCont)
        {
            var keys = rowSelectedPedProdCont.Split("$");
            Pedido pedido = uow.PedidoRepository.GetPedido(int.Parse(keys[1]), keys[4], keys[3]);

            return new DatosClientePedidoOriginal
            {
                CompartContenedorEntrega = pedido.ComparteContenedorEntrega,
                CodigoCliente = pedido.Cliente,
                NumeroPedido = pedido.Id,
                Direccion = pedido.DireccionEntrega,
                Anexo4 = pedido.Anexo4,
                CodigoRuta = short.Parse((pedido.Ruta ?? 1).ToString()),
                CodigoZona = pedido.Zona,
                Empresa = pedido.Empresa,
                TipoExpedicion = pedido.ConfiguracionExpedicion.Tipo,
                TipoPedido = pedido.Tipo
            };
        }

        public virtual Contenedor GetContenedorEmpaquetado(IUnitOfWork uow, string contDestinoData)
        {
            ContenedorDestinoData contDestino = JsonConvert.DeserializeObject<ContenedorDestinoData>(contDestinoData);
            return new Contenedor()
            {
                Numero = contDestino.NumeroContenedor,
                NumeroPreparacion = contDestino.NumeroPreparacion,
                Ubicacion = contDestino.Ubicacion,
            };
        }

        public virtual bool ContenedorDestinoValido(IUnitOfWork uow, int nuContenedor, int nuPreparacion, string ubicacion, out string mensajeError)
        {
            mensajeError = string.Empty;
            bool result = true;
            Contenedor contenedor = uow.ContenedorRepository.GetContenedor(nuPreparacion, nuContenedor);

            if (contenedor == null)
            {
                result = false;
                mensajeError = "EXP110_form1_Error_NoExisteContenedor";
            }

            if (contenedor.CamionFacturado != null)
            {
                result = false;
                mensajeError = "EXP110_form1_Error_ContenedorFacturado";
            }

            if (contenedor.Ubicacion != ubicacion)
            {
                result = false;
                mensajeError = "EXP110_form1_Error_ContenedorCambioUbicacion";
            }

            return result;

        }

        public virtual void EmpaquetarTodosProductosContenedor(IUnitOfWork uow,
            int nuContenedorOrigen, int nuContenedorDestino, int nuPreparacionOrigen,
            int nuPreparacionDestino, string cdCliente, int empresa, string nuPedido,
            string ubicacionOrigen, string ubicacionDestino, out bool isContenedorOrigenVacio)
        {
            isContenedorOrigenVacio = false;
            var lstProductoPedido = uow.EmpaquetadoPickingRepository.GetProductosPedidosLoteContenedor(nuContenedorOrigen, nuPreparacionOrigen, nuPedido, cdCliente, empresa);

            foreach (var prod in lstProductoPedido)
            {
                uow.EmpaquetadoPickingRepository.MoverContenedorEmpaquetado(nuContenedorOrigen, nuContenedorDestino,
                    nuPreparacionOrigen, nuPreparacionDestino, prod.CD_CLIENTE, prod.NU_PEDIDO, prod.CD_PRODUTO, prod.CD_FAIXA,
                    prod.CD_EMPRESA, prod.NU_IDENTIFICADOR, prod.QT_PRODUTO ?? 0, uow.GetTransactionNumber(), out isContenedorOrigenVacio);

                uow.EmpaquetadoPickingRepository.MoverStockEmpaquetado(ubicacionOrigen, prod.CD_PRODUTO, prod.CD_FAIXA, empresa,
                    prod.NU_IDENTIFICADOR, ubicacionDestino, prod.QT_PRODUTO ?? 0, uow.GetTransactionNumber(), out bool result);
            }
        }

        public virtual void EmpaquetarProductoContenedor(IUnitOfWork uow,
            int nuContenedorOrigen, int nuContenedorDestino, int nuPreparacionOrigen, int nuPreparacionDestino,
            string cdCliente, int empresa, string nuPedido, string ubicacionOrigen, string ubicacionDestino, string cdProducto,
            string nuIdentificador, decimal cdFaixa, decimal cantidad, out bool isContenedorOrigenVacio)
        {
            bool resultStock = true;

            uow.EmpaquetadoPickingRepository.MoverContenedorEmpaquetado(nuContenedorOrigen, nuContenedorDestino,
                nuPreparacionOrigen, nuPreparacionDestino, cdCliente, nuPedido, cdProducto, cdFaixa, empresa,
                nuIdentificador, cantidad, uow.GetTransactionNumber(), out isContenedorOrigenVacio);

            uow.EmpaquetadoPickingRepository.MoverStockEmpaquetado(ubicacionOrigen, cdProducto, cdFaixa, empresa,
                nuIdentificador, ubicacionDestino, cantidad, uow.GetTransactionNumber(), out resultStock);
        }

        public virtual void GenerarEgresoYFacturacion(IUnitOfWork uow, Form form, FormSubmitContext context, Contenedor contenedor,
            int contenedorOrigenNumero, int nuPreparacion, string cdCliente, int cdEmpresa, string nuPedido, bool isContenedorOrigenVacio,
            string metodoOrigen, Camion camionContenedorOrigen, Camion camionContenedorDestino, out (bool Notificar, int? Camion) interfazFacturacion)
        {
            interfazFacturacion = (Notificar: false, Camion: null);

            var terminePedidoContOrigen = true;

            if (!isContenedorOrigenVacio)
            {
                terminePedidoContOrigen = !uow.EmpaquetadoPickingRepository.QuedaPedidoContenedor(contenedorOrigenNumero, nuPreparacion, cdCliente, cdEmpresa, nuPedido);
                context.AddOrUpdateParameter("AUX_TERMINE_PEDIDO_CONT_ORIG", terminePedidoContOrigen ? "S" : "N");
                context.AddOrUpdateParameter("AUX_CONT_ORIG_VACIO", "N");
                context.AddOrUpdateParameter("AUX_SET_FIELD_CD_BARRAS_PROD", "S");
            }
            else
            {
                context.AddOrUpdateParameter("AUX_TERMINE_PEDIDO_CONT_ORIG", "S");
                context.AddOrUpdateParameter("AUX_CONT_ORIG_VACIO", "S");
            }

            if (terminePedidoContOrigen)
            {
                Logger.Debug($"{metodoOrigen} - Inicio Pedido contenedor origen terminado");

                var pedidoCompleto = false;

                if (IsPedidoCompleto(uow, cdEmpresa, cdCliente, nuPedido, out Error error))
                {
                    pedidoCompleto = true;
                    context.AddSuccessNotification("EXP110SelecProd_form_Msg_PedidoEmpaquetadoCompleto", new List<string> { nuPedido, cdCliente });
                }
                else if (error != null)
                    context.AddInfoNotification(error.Mensaje, error.GetArgumentos());

                if (pedidoCompleto)
                {
                    Logger.Debug($"{metodoOrigen} - Procesando pedido completo");

                    ArmadoEgresoFacturadoEmpaquetado(uow, context, contenedor, cdEmpresa, cdCliente, nuPedido, pedidoCompleto, _identity.Predio, out Error success, out error, out interfazFacturacion);

                    uow.SaveChanges();

                    if (success != null)
                        context.AddSuccessNotification(success.Mensaje, success.GetArgumentos());

                    if (error != null)
                        context.AddInfoNotification(error.Mensaje, error.GetArgumentos());

                    context.AddOrUpdateParameter("AUX_PEDIDO_COMPLETO", "S");

                    Logger.Debug($"{metodoOrigen} - Fin pedido completo");
                }

                Logger.Debug($"{metodoOrigen} - Fin pedido contenedor origen terminado");
            }

            var pedido = uow.PedidoRepository.GetPedido(cdEmpresa, cdCliente, nuPedido);
            var configuracionExpedicion = uow.PedidoRepository.GetConfiguracionExpedicion(pedido.ConfiguracionExpedicion.Tipo);

            if (!configuracionExpedicion.DebeFacturarEnEmpaquetado)
            {
                if (camionContenedorDestino != null)
                    AddCargaCamion(uow, cdEmpresa, contenedor.NumeroPreparacion, contenedor.Numero, camionContenedorDestino.Id);
                else if (camionContenedorOrigen != null)
                    AddCargaCamion(uow, cdEmpresa, contenedor.NumeroPreparacion, contenedor.Numero, camionContenedorOrigen.Id);
            }
        }

        public virtual bool ArmadoEgresoFacturadoEmpaquetado(IUnitOfWork uow, FormSubmitContext context, Contenedor contenedorDestino, int empresa, string cdCliente, 
            string nuPedido, bool pedidoCompleto, string predio, out Error success, out Error error, out (bool Notificar, int? Camion) interfazFacturacion)
        {
            Logger.Debug("ArmadoEgresoFacturadoEmpaquetado - Inicio");

            error = null;
            success = null;
            interfazFacturacion = (Notificar: false, Camion: null);

            var pedido = uow.PedidoRepository.GetPedido(empresa, cdCliente, nuPedido);
            var configuracionExpedicionPedido = uow.PedidoRepository.GetConfiguracionExpedicion(pedido.ConfiguracionExpedicion.Tipo);

            if (pedidoCompleto && configuracionExpedicionPedido.TipoArmadoEgreso == TipoArmadoEgreso.Empaque && configuracionExpedicionPedido.DebeFacturarEnEmpaquetado)
            {
                AddCargaCamionPedidoUnico(uow, empresa, cdCliente, nuPedido, predio, out int? cdCamion);

                uow.SaveChanges();

                if (!cdCamion.HasValue)
                    cdCamion = uow.EmpaquetadoPickingRepository.GetCamionPedido(empresa, cdCliente, nuPedido);

                if (cdCamion.HasValue)
                {
                    Logger.Debug($"ArmadoEgresoFacturadoEmpaquetado - Camión: {cdCamion}");

                    uow.EmpaquetadoPickingRepository.FacturarCamion(uow, cdCamion.Value, Logger, out error);

                    if (error != null)
                        return false;

                    uow.SaveChanges();

                    context.AddOrUpdateParameter("AUX_FACTURO_CONTENEDOR_EMPAQUE", "S");
                    success = new Error("EXP110SelecProd_form_Msg_SeEnvioAFacturarPedido", nuPedido, cdCliente);
                    interfazFacturacion = (Notificar: true, Camion: cdCamion);
                }
            }

            Logger.Debug("ArmadoEgresoFacturadoEmpaquetado - Fin");

            return true;
        }

        public virtual void AddCargaCamionPedidoUnico(IUnitOfWork uow, int empresa, string cdCliente, string nuPedido, string predio, out int? cdCamion)
        {
            Logger.Debug("AddCargaCamionPedidoUnico - Inicio");

            var cargas = uow.EmpaquetadoPickingRepository.GetCargas(empresa, cdCliente, nuPedido, out cdCamion);

            Logger.Debug($"AddCargaCamionPedidoUnico - Cantidad de cargas a modificar: {cargas.Count()}");

            if (cargas != null && cargas.Count > 0)
            {
                if (!cdCamion.HasValue)
                {
                    var cdPuerta = GetCodigoPuerta(uow, predio);
                    cdCamion = uow.EmpaquetadoPickingRepository.CrearCamion(cdCliente, empresa, nuPedido, predio, cdPuerta, uow.GetTransactionNumber());

                    uow.SaveChanges();
                    Logger.Debug($"AddCargaCamionPedidoUnico - Crea camión: {cdCamion}");
                }

                var detallesPicking = uow.PreparacionRepository.GetDetalleLiberadosPreparacionByCarga(cargas)
                    .Where(w => w.CantidadPreparada != null);

                cargas = detallesPicking.Select(s => s.Carga ?? 0).Distinct().ToList();

                Logger.Debug($"AddCargaCamionPedidoUnico - Cantidad de cargas a asociar: {cargas.Count()}");

                foreach (var value in cargas)
                {
                    Carga cargaAsociar = null;

                    if (uow.PreparacionRepository.ExistenMultiplesPedidosCarga(value))
                    {
                        var carga = uow.CargaRepository.GetCarga(value);
                        cargaAsociar = CopiarCarga(uow, new PedidoAsociarUnidad { Cliente = cdCliente, Empresa = empresa, Pedido = nuPedido }, carga);
                    }
                    else
                    {
                        cargaAsociar = uow.CargaRepository.GetCarga(value);
                    }

                    AgregarLinea(uow, cargaAsociar, cdCamion.Value, cdCliente, empresa);
                }
            }

            Logger.Debug("AddCargaCamionPedidoUnico - Fin");
        }

        public virtual void AddCargaCamion(IUnitOfWork uow, int empresa, int nuPreparacion, int nuContenedor, int cdCamion)
        {
            Logger.Debug("AddCargaCamion - Inicio");

            var cargas = uow.EmpaquetadoPickingRepository.GetCargas(nuPreparacion, nuContenedor);

            if (cargas != null && cargas.Count > 0)
            {
                var detallesPicking = uow.PreparacionRepository.GetDetalleLiberadosPreparacionByCarga(cargas)
                    .Where(w => w.CantidadPreparada != null);

                var cargasConDetalles = detallesPicking
                    .Where(s => s.Carga != null)
                    .Select(s => new
                    {
                        Carga = s.Carga.Value,
                        Cliente = s.Cliente,
                        Pedido = s.Pedido
                    })
                    .Distinct()
                    .ToList();

                Logger.Debug($"AddCargaCamion - Cantidad de cargas a asociar: {cargasConDetalles.Count()}");

                foreach (var value in cargasConDetalles)
                {
                    Carga cargaAsociar = null;

                    if (uow.PreparacionRepository.ExistenMultiplesPedidosCarga(value.Carga))
                    {
                        var carga = uow.CargaRepository.GetCarga(value.Carga);
                        cargaAsociar = CopiarCarga(uow, new PedidoAsociarUnidad { Cliente = value.Cliente, Empresa = empresa, Pedido = value.Pedido }, carga);
                    }
                    else
                    {
                        cargaAsociar = uow.CargaRepository.GetCarga(value.Carga);
                    }

                    AgregarLinea(uow, cargaAsociar, cdCamion, value.Cliente, empresa);
                }
            }

            Logger.Debug("AddCargaCamion - Fin");
        }

        public virtual void AgregarLinea(IUnitOfWork uow, Carga carga, int cdCamion, string cliente, int empresa)
        {
            if (!uow.CargaCamionRepository.GetsCargasCamion(cdCamion).Any(a => a.Carga == carga.Id))
            {
                var cargaCamion = new CargaCamion
                {
                    Camion = cdCamion,
                    Carga = carga.Id,
                    Cliente = cliente,
                    Empresa = empresa,
                    FechaAlta = DateTime.Now
                };

                uow.CargaCamionRepository.AddCargaCamion(cargaCamion);
            }
        }

        public virtual Carga CopiarCarga(IUnitOfWork uow, Carga carga)
        {
            var nuevaCarga = carga.Copiar();

            nuevaCarga.Id = uow.CargaRepository.GetSiguienteNumeroCarga();
            nuevaCarga.Descripcion = $"Armado de camión Mesa de empaque.";
            nuevaCarga.FechaAlta = DateTime.Now;

            uow.CargaRepository.AddCarga(nuevaCarga);

            uow.SaveChanges();
            return nuevaCarga;
        }

        public virtual Carga CopiarCarga(IUnitOfWork uow, PedidoAsociarUnidad pedido, Carga carga)
        {
            var nuevaCarga = carga.Copiar();

            var descripcion = $"Armado de camión empaquetado. Pedido: {pedido.Pedido}";

            nuevaCarga.Id = uow.CargaRepository.GetSiguienteNumeroCarga();
            nuevaCarga.Descripcion = descripcion = descripcion.Length > 50 ? descripcion.Substring(0, 49) : descripcion;
            nuevaCarga.FechaAlta = DateTime.Now;

            uow.CargaRepository.AddCarga(nuevaCarga);

            uow.SaveChanges();

            List<DetallePreparacion> lineasCarga = uow.PreparacionRepository.GetDetallePreparacionByPedidoCarga(pedido.Empresa, pedido.Cliente, pedido.Pedido, carga.Id);

            foreach (var linea in lineasCarga)
            {
                linea.Carga = nuevaCarga.Id;
                linea.Transaccion = uow.GetTransactionNumber();

                uow.PreparacionRepository.UpdateDetallePreparacion(linea);
            }

            return nuevaCarga;
        }

        public virtual bool IsPedidoCompleto(IUnitOfWork uow, int empresa, string cdCliente, string nuPedido, out Error error)
        {
            var response = true;
            error = null;

            if (!uow.EmpaquetadoPickingRepository.IsPedidoTodoLiberado(empresa, cdCliente, nuPedido))
            {
                error = new Error("EXP110SelecProd_form_Msg_PedidoNoEmpaquetadoOk");
                return false;
            }

            if (!uow.EmpaquetadoPickingRepository.PedidoTodoPickeado(empresa, cdCliente, nuPedido))
            {
                error = new Error("EXP110SelecProd_form_Msg_PedidoNoPickeadoCompleto");
                return false;
            }

            if (!uow.EmpaquetadoPickingRepository.PedidoTodoEmpaquetado(empresa, cdCliente, nuPedido))
            {
                error = new Error("EXP110SelecProd_form_Msg_PedidoNoEmpaquetadoCompleto");
                return false;
            }

            return response;
        }

        public virtual void IsPedidoCompletoImprimirResumen(IUnitOfWork uow, int userId, string confInicial, Contenedor contenedorDestino, string cdCliente, int empresa, string nuPedido)
        {
            var configuracion = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);
            IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, "WIS-FIN-PA");
            Pedido pedido = uow.PedidoRepository.GetPedido(empresa, cdCliente, nuPedido);
            Agente cliente = uow.AgenteRepository.GetAgente(empresa, cdCliente);
            string cdRuta = (pedido.Ruta ?? 1).ToString();
            Ruta ruta = uow.RutaRepository.GetRuta(short.Parse(cdRuta));
            var datos = uow.EmpaquetadoPickingRepository.GetDatosContenedorFinPicking(contenedorDestino.NumeroPreparacion, empresa, cdCliente, nuPedido);

            var contenedorEntrega = new ContenedorFinPicking
            {
                Anexo1 = pedido.Anexo,
                NumeroContenedor = contenedorDestino.Numero,
                NumeroPreparacion = contenedorDestino.NumeroPreparacion,
                Anexo4 = pedido.Anexo4,
                CodigoCliente = cdCliente,
                DescripcionCliente = cliente.Descripcion,
                DescripcionUbicacion = string.IsNullOrEmpty(pedido.DireccionEntrega) ? cliente.Direccion : pedido.DireccionEntrega,
                NumeroPedido = nuPedido,
                TipoContenedor = contenedorDestino.TipoContenedor,
                FechaEntrega = pedido.FechaEntrega ?? DateTime.Now,
                DescripcionRuta = ruta.Descripcion,
                TotalBultos = datos.CantidadTotalBultos.ToString(),
                IdExterno = contenedorDestino.IdExterno,
                CodigoBarras = contenedorDestino.CodigoBarras,
                DescripcionContenedor = contenedorDestino.DescripcionContenedor,
            };

            IImpresionDetalleBuildingStrategy strategy = new ContenedorFinPickingImpresionStrategy(estiloTemplate, uow, _printingService, datos, contenedorEntrega, _barcodeService, 1);
            ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(configuracion.Impresora, configuracion.Predio), strategy, _printingService);

            Impresion impresion = builder.GenerarCabezal(userId, configuracion.Predio).GenerarDetalle().Build();

            impresion.Referencia = string.Format("{0} / TP: {1}", contenedorDestino.IdExterno, contenedorEntrega.TipoContenedor);

            int numImpresion = uow.ImpresionRepository.Add(impresion);
            uow.SaveChanges();

            int iterador = 1;
            int cantidadRegistros = 1;
            int cantidadCopias = 1;

            DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
            {
                NumeroImpresion = numImpresion,
                FechaProcesado = DateTime.Now,
                Estado = _printingService.GetEstadoInicial()
            };

            foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
            {
                for (int i = 0; i < cantidadCopias; i++)
                {
                    detalleImpresionInsercion.Contenido += detalle.Contenido + "\n" + "\n";

                    if (detalleImpresionInsercion.Contenido.Length > 2000)
                    {
                        detalleImpresionInsercion.Registro = cantidadRegistros;

                        uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                        cantidadRegistros++;
                        detalleImpresionInsercion.Contenido = string.Empty;
                    }
                    else if (cantidadCopias == iterador)
                    {
                        detalleImpresionInsercion.Registro = cantidadRegistros;

                        uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                        cantidadRegistros++;
                        detalleImpresionInsercion.Contenido = string.Empty;
                    }

                    iterador++;
                }

                iterador = 1;
            }

            uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

            uow.SaveChanges();

            _printingService.SendToPrint(numImpresion);
        }

        public virtual void ImprimirEtiqueta(IUnitOfWork uow, int userId, string confInicial, int? auxContenedorImpresion, Contenedor contenedorDestino, string cdCliente, int empresa, string nuPedido)
        {
            if (auxContenedorImpresion == null || (contenedorDestino.Numero != auxContenedorImpresion))
            {
                var configuracion = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);
                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, configuracion.Estilo);
                Pedido pedido = uow.PedidoRepository.GetPedido(empresa, cdCliente, nuPedido);
                Agente cliente = uow.AgenteRepository.GetAgente(empresa, cdCliente);

                IImpresionDetalleBuildingStrategy strategy = new ContenedorEntregaImpresionStrategy(estiloTemplate, uow, _printingService, contenedorDestino, pedido, cliente, _barcodeService, 1);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(configuracion.Impresora, configuracion.Predio), strategy, _printingService);

                Impresion impresion = builder.GenerarCabezal(userId, configuracion.Predio).GenerarDetalle().Build();

                impresion.Referencia = string.Format("{0} / TP: {1}", contenedorDestino.IdExterno, contenedorDestino.TipoContenedor); ;

                int numImpresion = uow.ImpresionRepository.Add(impresion);

                uow.SaveChanges();

                int iterador = 1;
                int cantidadRegistros = 1;
                int cantidadCopias = 1;

                DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
                {
                    NumeroImpresion = numImpresion,
                    FechaProcesado = DateTime.Now,
                    Estado = _printingService.GetEstadoInicial(),
                };

                foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
                {
                    for (int i = 0; i < cantidadCopias; i++)
                    {

                        detalleImpresionInsercion.Contenido += detalle.Contenido + "\n" + "\n";

                        if (detalleImpresionInsercion.Contenido.Length > 2000)
                        {
                            detalleImpresionInsercion.Registro = cantidadRegistros;

                            uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                            cantidadRegistros++;
                            detalleImpresionInsercion.Contenido = string.Empty;

                        }
                        else if (cantidadCopias == iterador)
                        {
                            detalleImpresionInsercion.Registro = cantidadRegistros;

                            uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                            cantidadRegistros++;
                            detalleImpresionInsercion.Contenido = string.Empty;

                        }

                        iterador++;
                    }
                    iterador = 1;
                }

                uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

                uow.SaveChanges();

                _printingService.SendToPrint(numImpresion);
            }
        }

        public virtual void ImprimirEtiquetaBulto(IUnitOfWork uow, int userId, string confInicial, Contenedor contenedorBulto, string cdCliente, int empresa, string nuPedido, bool imprimePrimerBulto, string memo)
        {
            var configuracion = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);

            IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, "WIS-CONH");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cdCliente, nuPedido);
            var cliente = uow.AgenteRepository.GetAgente(empresa, cdCliente);

            var contenedor = new ContenedorEtiquetaHijas
            {
                Anexo1 = pedido.Anexo,
                NumeroContenedor = contenedorBulto.Numero,
                Anexo4 = pedido.Anexo4,
                CantidadBultos = contenedorBulto.CantidadBulto ?? 1,
                CodigoCliente = cdCliente,
                CodigoTransportadora = pedido.CodigoTransportadora ?? 1,
                CodigoZona = pedido.Zona,
                DescripcionCliente = cliente.Descripcion,
                DescripcionUbicacion = string.IsNullOrEmpty(pedido.DireccionEntrega) ? cliente.Direccion : pedido.DireccionEntrega,
                NumeroPedido = nuPedido,
                TipoContenedor = contenedorBulto.TipoContenedor,
                Memo = memo,
                ImprimePrimerBulto = imprimePrimerBulto,
                IdExterno = contenedorBulto.IdExterno,
                CodigoBarras = contenedorBulto.CodigoBarras,
                DescripcionContenedor = contenedorBulto.DescripcionContenedor,
            };

            IImpresionDetalleBuildingStrategy strategy = new ContenedorEtiquetasHijasStrategy(estiloTemplate, uow, _printingService, contenedor, pedido, _barcodeService, 1);
            ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(configuracion.Impresora, configuracion.Predio), strategy, _printingService);

            Impresion impresion = builder.GenerarCabezal(userId, configuracion.Predio).GenerarDetalle().Build();

            impresion.Referencia = string.Format("{0} / TP: {1}", contenedor.IdExterno, contenedor.TipoContenedor);

            int numImpresion = uow.ImpresionRepository.Add(impresion);

            uow.SaveChanges();

            int iterador = 1;
            int cantidadRegistros = 1;
            int cantidadCopias = 1;

            DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
            {
                NumeroImpresion = numImpresion,
                FechaProcesado = DateTime.Now,
                Estado = _printingService.GetEstadoInicial(),
            };

            foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
            {
                for (int i = 0; i < cantidadCopias; i++)
                {
                    detalleImpresionInsercion.Contenido += detalle.Contenido + "\n" + "\n";

                    if (detalleImpresionInsercion.Contenido.Length > 2000)
                    {
                        detalleImpresionInsercion.Registro = cantidadRegistros;

                        uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                        cantidadRegistros++;
                        detalleImpresionInsercion.Contenido = string.Empty;
                    }
                    else if (cantidadCopias == iterador)
                    {
                        detalleImpresionInsercion.Registro = cantidadRegistros;

                        uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                        cantidadRegistros++;
                        detalleImpresionInsercion.Contenido = string.Empty;
                    }

                    iterador++;
                }

                iterador = 1;
            }

            uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

            uow.SaveChanges();
            uow.Commit();

            _printingService.SendToPrint(numImpresion);
        }

        public virtual void ImprimirEtiqueta(IUnitOfWork uow, int userId, string confInicial, int? auxContenedorImpresion, Contenedor contenedorDestino, string cdCliente, int empresa, string nuPedido, out string infoCambio)
        {
            infoCambio = "";
            if (auxContenedorImpresion == null || (contenedorDestino.Numero != auxContenedorImpresion))
            {
                var configuracion = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);
                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, configuracion.Estilo);
                Pedido pedido = uow.PedidoRepository.GetPedido(empresa, cdCliente, nuPedido);
                Agente cliente = uow.AgenteRepository.GetAgente(empresa, cdCliente);

                ParametroConfiguracion parametro = uow.ParametroRepository.GetParametroConfiguracion("MESA_EMPAQUE_TIPO_PEDIDO", $"{ParamManager.PARAM_TPED}_{pedido.Tipo}");
                if (parametro != null)
                {
                    configuracion.Estilo = parametro.Valor.Split('#')[0];
                    configuracion.Impresora = parametro.Valor.Split('#')[1];
                    infoCambio = "EXP110SelecProd_form_Msg_SeEnvioEtiquetaImpresoraCambiado#" + parametro.Valor;
                }

                IImpresionDetalleBuildingStrategy strategy = new ContenedorEntregaImpresionStrategy(estiloTemplate, uow, _printingService, contenedorDestino, pedido, cliente, _barcodeService, 1);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(configuracion.Impresora, configuracion.Predio), strategy, _printingService);

                Impresion impresion = builder.GenerarCabezal(userId, configuracion.Predio).GenerarDetalle().Build();

                impresion.Referencia = string.Format("{0} / TP: {1}", contenedorDestino.IdExterno, contenedorDestino.TipoContenedor); ;

                int numImpresion = uow.ImpresionRepository.Add(impresion);

                uow.SaveChanges();

                int iterador = 1;
                int cantidadRegistros = 1;
                int cantidadCopias = 1;

                DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
                {
                    NumeroImpresion = numImpresion,
                    FechaProcesado = DateTime.Now,
                    Estado = _printingService.GetEstadoInicial(),
                };

                foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
                {
                    for (int i = 0; i < cantidadCopias; i++)
                    {
                        detalleImpresionInsercion.Contenido += detalle.Contenido + "\n" + "\n";

                        if (detalleImpresionInsercion.Contenido.Length > 2000)
                        {
                            detalleImpresionInsercion.Registro = cantidadRegistros;

                            uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                            cantidadRegistros++;
                            detalleImpresionInsercion.Contenido = string.Empty;
                        }
                        else if (cantidadCopias == iterador)
                        {
                            detalleImpresionInsercion.Registro = cantidadRegistros;

                            uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                            cantidadRegistros++;
                            detalleImpresionInsercion.Contenido = string.Empty;
                        }

                        iterador++;
                    }
                    iterador = 1;
                }

                uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

                uow.SaveChanges();
                //uow.Commit();

                _printingService.SendToPrint(numImpresion);
            }
        }

        public virtual bool ContenedorTieneUnPedido(IUnitOfWork uow, string contenedorOrigen, out int contenedorOrigenNumero, out string contenedorOrigenUbicacion, out int cdEmpresa, out string cdCliente, out int nuPreparacion, out string nuPedido)
        {
            bool tieneUno = false;
            cdEmpresa = -1;
            cdCliente = "";
            nuPedido = "";
            contenedorOrigenNumero = -1;
            contenedorOrigenUbicacion = "";
            Contenedor auxContenedor = new Contenedor();

            if (!string.IsNullOrEmpty(contenedorOrigen))
            {
                if (int.TryParse(contenedorOrigen, out int contenedorOrigenInt))
                {
                    auxContenedor = uow.ContenedorRepository.GetContenedor(contenedorOrigenInt);
                }
                else
                {
                    _barcodeService.ValidarEtiquetaContenedor(contenedorOrigen, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);
                    auxContenedor.Numero = datosContenedor.NuContenedor;
                    auxContenedor.NumeroPreparacion = datosContenedor.NuPreparacion;
                    auxContenedor.Ubicacion = datosContenedor.Ubicacion;
                }

                if (auxContenedor != null)
                {
                    if (uow.ContenedorRepository.ContenedorTieneUnPedido(auxContenedor.Numero, auxContenedor.NumeroPreparacion))
                    {
                        DetallePreparacion detalle = uow.PreparacionRepository.GetDetallePreparacion(auxContenedor.NumeroPreparacion, auxContenedor.Numero);
                        Pedido pedido = uow.PedidoRepository.GetPedido(detalle.Empresa, detalle.Cliente, detalle.Pedido);

                        if (uow.PedidoRepository.GetTipoExpedicionModalidadEmpaque(pedido))
                        {
                            List<DetallePedido> detalles = uow.PedidoRepository.GetDetallesPedido(pedido);

                            foreach (var item in detalles)
                            {
                                Contenedor contenedor = uow.ContenedorRepository.GetContenedor(auxContenedor.NumeroPreparacion, auxContenedor.Numero);
                                Producto producto = uow.ProductoRepository.GetProducto(item.Empresa, item.Producto);
                            }


                            tieneUno = true;
                            cdEmpresa = detalle.Empresa;
                            cdCliente = detalle.Cliente;
                            nuPedido = detalle.Pedido;
                            contenedorOrigenNumero = auxContenedor.Numero;
                            contenedorOrigenUbicacion = auxContenedor.Ubicacion;
                        }
                    }
                }
            }

            nuPreparacion = auxContenedor.NumeroPreparacion;

            return tieneUno;
        }

        public virtual bool ExisteCompatibilidadContenedor(IUnitOfWork uow, int nuContenedorOrigen, int nuPreparacionOrigen, string subClasseDestino, string comparteContenedorEntregaDestino, out string menssageResult)
        {
            bool valida = true;
            menssageResult = string.Empty;

            var contenedor = uow.ContenedorRepository.GetContenedor(nuPreparacionOrigen, nuContenedorOrigen);
            if (contenedor == null)
                return false;

            var compatibilidadPedidosContenedorOrigen = uow.EmpaquetadoPickingRepository.GetCompatibilidadContenedores(nuContenedorOrigen, nuPreparacionOrigen);

            if (!compatibilidadPedidosContenedorOrigen.Any(c => c.CompartContenedorEntrega == comparteContenedorEntregaDestino))
            {
                menssageResult = "EXP110_form1_Error_PedidoNoPermiteCompartirContenedor";
                valida = false;
            }

            if (valida)
            {
                if (contenedor.IdContenedorEmpaque == "S")
                {
                    menssageResult = "EXP110_form1_Error_ContenedorOrigenEsEmpaque";
                    valida = false;
                }
                else if (contenedor.CodigoSubClase != subClasseDestino)
                {
                    menssageResult = "EXP110_form1_Error_ContenedorDestinoTieneOtraSubClase";
                    valida = false;
                }
            }

            return valida;
        }

        public virtual void ValidarMultiPreparacion(IUnitOfWork uow, Contenedor contOrigen, int contDestino, int nuPrepDestino)
        {
            var prepOrigen = uow.PreparacionRepository.GetPreparacionPorNumero(contOrigen.NumeroPreparacion);
            var prepDestino = uow.PreparacionRepository.GetPreparacionPorNumero(nuPrepDestino);

            if (prepDestino.Tipo == TipoPreparacionDb.CrossDocking && !uow.PreparacionRepository.AnyDetallePicking(prepDestino.Id, contDestino))
                throw new ValidationFailedException("General_Sec0_Error_ContenedorPendienteConvertir");

            if (prepOrigen.Empresa != prepDestino.Empresa)
                throw new ValidationFailedException("General_Sec0_Error_PrepDestinoDistintaEmpresa");

            var cargaDestino = (uow.PreparacionRepository.GetCarga(prepDestino.Id, contDestino) ?? -1);
            var cargaOrigen = (uow.CargaRepository.GetCargaPicking(prepOrigen.Id) ?? -1);

            if (cargaOrigen == -1)
                cargaOrigen = (uow.CargaRepository.GetCarga(prepOrigen.Id)?.Id ?? -1);

            var cargaCamionOrigen = uow.CargaCamionRepository.GetFirstCargaCamion(cargaOrigen);
            var cargaCamionDestino = uow.CargaCamionRepository.GetFirstCargaCamion(cargaDestino);

            var msg = "General_Sec0_Error_PrepDestinoDistintoCamion";
            if (cargaCamionOrigen != null || cargaCamionDestino != null)
            {
                if ((cargaCamionOrigen == null && cargaCamionDestino != null))
                    throw new ValidationFailedException(msg);
                else if (cargaCamionDestino == null && cargaCamionOrigen != null)
                    throw new ValidationFailedException(msg);
                else if (cargaCamionOrigen != null && cargaCamionDestino != null)
                {
                    if (cargaCamionOrigen.Camion != cargaCamionDestino.Camion)
                        throw new ValidationFailedException(msg);
                }
            }
        }

        public virtual short GetCodigoPuerta(IUnitOfWork uow, string predio)
        {
            var cdUbicacionPuerta = uow.ParametroRepository.GetParameter(ParamManager.EXP040_PUERTA_CARGA_AUTO, new Dictionary<string, string>
            {
                [ParamManager.PARAM_PRED] = $"{ParamManager.PARAM_PRED}_{predio}"
            });

            var puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarqueByUbicacion(cdUbicacionPuerta);
            if (puerta == null)
                puerta = uow.PuertaEmbarqueRepository.GetPuertaDefecto(predio);

            return puerta.Id;
        }
    }
}
