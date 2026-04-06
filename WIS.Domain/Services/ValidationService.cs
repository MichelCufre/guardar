using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Helpers;
using WIS.Domain.Interfaces;
using WIS.Domain.Logic;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Recepcion;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Validation;
using WIS.Extension;
using WIS.Security;
using Error = WIS.Domain.Validation.Error;

namespace WIS.Domain.Services
{
    public class ValidationService : IValidationService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormatProvider _culture;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;

        protected string _separador;

        public ValidationService(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IBarcodeService barcodeService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _culture = _identity.GetFormatProvider();
            _barcodeService = barcodeService;
        }

        public virtual Task<List<Error>> ValidateAgente(Agente agente, IAgenteServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values

            if (agente.RutaId == null)
            {
                var rutaParam = context.GetParametro(ParamManager.IE_507_CD_ROTA);

                if (short.TryParse(rutaParam, out short parsed))
                    agente.RutaId = parsed;
            }

            if (agente.EstadoId == -1)
            {
                var situacionParam = context.GetParametro(ParamManager.IE_507_CD_SITUACAO) ?? SituacionDb.Activo.ToString();

                if (short.TryParse(situacionParam, out short parsed))
                    agente.EstadoId = parsed;
            }

            agente.Codigo = agente.Codigo.Trim();

            #endregion

            AgenteValidacionCarga(agente, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            AgenteValidacionProcedimiento(agente, context, errors);
            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateAgenda(Agenda agenda, IAgendaServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            AgendaValidacionCarga(agenda, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            AgendaValidacionProcedimiento(agenda, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidatePedido(Pedido pedido, IPedidoServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values

            if (pedido.FechaEmision == null)
                pedido.FechaEmision = DateTime.Now;

            if (pedido.FechaLiberarDesde == null)
                pedido.FechaLiberarDesde = DateTime.Now;

            if (string.IsNullOrEmpty(pedido.CondicionLiberacion))
                pedido.CondicionLiberacion = context.GetParametro(ParamManager.IE_503_CD_CONDICION_LIBERACION);

            if (pedido.Ruta == null)
            {
                var param = context.GetParametro(ParamManager.IE_503_CD_ROTA);

                if (short.TryParse(param, out short parsed))
                    pedido.Ruta = parsed;
            }

            if (pedido.CodigoTransportadora == null)
            {
                var param = context.GetParametro(ParamManager.IE_503_CD_TRANSPORTADORA);

                if (int.TryParse(param, out int parsed))
                    pedido.CodigoTransportadora = parsed;
            }

            if (string.IsNullOrEmpty(pedido.TipoExpedicionId))
                pedido.TipoExpedicionId = context.GetParametro(ParamManager.IE_503_TP_EXPEDICION);

            if (string.IsNullOrEmpty(pedido.Tipo))
                pedido.Tipo = context.GetParametro(ParamManager.IE_503_TP_PEDIDO);

            #endregion

            PedidoValidacionCarga(pedido, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            PedidoValidacionProcedimiento(pedido, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateEgreso(Camion egreso, IEgresoServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            EgresoValidacionCarga(egreso, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            EgresoValidacionProcedimiento(egreso, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateEmpresa(Empresa empresa, IEmpresaServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values
            if (empresa.EstadoId == -1)
            {
                var situacionParam = context.GetParametro(ParamManager.IE_522_CD_SITUACAO) ?? SituacionDb.Activo.ToString();

                if (short.TryParse(situacionParam, out short parsed))
                    empresa.EstadoId = parsed;
            }
            #endregion

            EmpresaValidacionCarga(empresa, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            EmpresaValidacionProcedimiento(empresa, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateLpn(IUnitOfWork uow, Lpn lpn, ILpnServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            LpnValidacion(uow, lpn, context, out List<Error> errors, out errorProcedimiento);
            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateProducto(Producto producto, IProductoServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            List<Error> errors = new List<Error>();
            errorProcedimiento = false;

            SetDefaultValuesProducto(producto, context);

            ProductoValidacionCarga(producto, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            ProductoValidacionProcedimiento(producto, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateModificarPedido(Pedido pedido, IModificarPedidoServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values

            if (string.IsNullOrEmpty(pedido.CondicionLiberacion))
                pedido.CondicionLiberacion = context.GetParametro(ParamManager.IE_503_CD_CONDICION_LIBERACION);

            if (pedido.Ruta == null)
            {
                var param = context.GetParametro(ParamManager.IE_503_CD_ROTA);

                if (short.TryParse(param, out short ruta))
                    pedido.Ruta = ruta;
            }

            if (pedido.CodigoTransportadora == null)
            {
                var param = context.GetParametro(ParamManager.IE_503_CD_TRANSPORTADORA);

                if (int.TryParse(param, out int transportadora))
                    pedido.CodigoTransportadora = transportadora;
            }

            if (string.IsNullOrEmpty(pedido.TipoExpedicionId))
                pedido.TipoExpedicionId = context.GetParametro(ParamManager.IE_503_TP_EXPEDICION);

            if (string.IsNullOrEmpty(pedido.Tipo))
                pedido.Tipo = context.GetParametro(ParamManager.IE_503_TP_PEDIDO);

            #endregion

            ModificarPedidoValidacionCarga(pedido, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            ModificarPedidoValidacionProcedimiento(pedido, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual List<Error> ValidateFiltrosStock(IUnitOfWork uow, FiltrosStock filtros, IStockServiceContext context, out bool errorProcedimiento)
        {
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            FiltrosStockValidacionCarga(filtros, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return errors;

            FiltrosStockValidacionProcedimiento(uow, filtros, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        public virtual Task<List<Error>> ValidateAjusteStock(AjusteStock ajuste, IAjustesDeStockServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            AjusteStockValidacionCarga(ajuste, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            AjusteStockValidacionProcedimiento(ajuste, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);

        }

        public virtual Task<List<Error>> ValidateTransferencia(TransferenciaStock transferencia, ITransferenciaStockServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            var errors = new List<Error>();

            TransferenciaStockValidacionCarga(transferencia, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            TransferenciaStockValidacionProcedimiento(transferencia, context, errors);

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateTransferenciaSaldos(List<TransferenciaStock> transferencias, ITransferenciaStockServiceContext context)
        {
            var errors = new List<Error>();

            var stocks = transferencias.GroupBy(x => new { x.Ubicacion, x.Empresa, x.Producto, x.Identificador, x.Faixa })
            .Select(x => new Stock
            {
                Ubicacion = x.Key.Ubicacion,
                Empresa = x.Key.Empresa,
                Producto = x.Key.Producto,
                Identificador = x.Key.Identificador,
                Faixa = x.Key.Faixa,
                Cantidad = x.Sum(d => d.Cantidad)
            });

            foreach (var s in stocks)
            {
                var stock = context.GetStock(s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa);
                if (stock == null)
                    errors.Add(new Error("WMSAPI_msg_Error_SinStock", new object[] { s.Ubicacion, s.Empresa, s.Producto, s.Identificador, s.Faixa }));
                else if (s.Cantidad > context.GetCantidadDisponible(stock))
                    errors.Add(new Error("WMSAPI_msg_Error_NoExisteSaldoSuficienteParaEfectuarelAjuste", new object[] { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa }));
                else if (stock.Inventario == "D")
                    errors.Add(new Error("WMSAPI_msg_Error_StockPendienteInventario", s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa));
            }

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateCrossDocking(CrossDockingUnaFase detalle, ICrossDockingServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            var errors = new List<Error>();

            CrossDockingValidacionCarga(detalle, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            CrossDockingValidacionProcedimiento(detalle, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateAnularPickingPedidoPendiente(AnularPickingPedidoPendiente detalle, IAnularPickingPedidoPendienteContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            var errors = new List<Error>();

            AnularPickingPedidoPendienteCarga(detalle, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            AnularPickingPedidoPendienteProcedimiento(detalle, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateAnularPickingPedidoPendienteAutomatismo(AnularPickingPedidoPendienteAutomatismo detalle, AnularPickingPedidoPendienteAutomatismoContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            var errors = new List<Error>();

            AnularPickingPedidoPendienteAutomatismoCarga(detalle, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            AnularPickingPedidoPendienteAutomatismoProcedimiento(detalle, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateControlCalidad(ControlCalidadAPI control, IControlCalidadServiceContext context, out bool errorProcedimiento)
        {
            errorProcedimiento = false;
            var errors = new List<Error>();

            this.ValidateControlCalidadCarga(control, errors);

            if (errors.Any())
                return Task.FromResult(errors);

            this.ValidateControlCalidadProcedimiento(control, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateCodigoBarras(CodigoBarras codigoDeBarra, ICodigoBarrasServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values

            if (codigoDeBarra.TipoCodigo == null)
            {
                var tpParam = context.GetParametro(ParamManager.IE_505_TP_CODIGO_BARRAS);

                if (int.TryParse(tpParam, out int parsed))
                    codigoDeBarra.TipoCodigo = parsed;
            }

            #endregion

            CodigoBarrasValidacionCarga(codigoDeBarra, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            CodigoBarrasValidacionProcedimiento(codigoDeBarra, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateProductoProveedor(ProductoProveedor producto, IProductoProveedorServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            List<Error> errors = new List<Error>();
            errorProcedimiento = false;

            ProductoProveedorValidacionCarga(producto, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            ProductoProveedorValidacionProcedimiento(producto, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateAnularReferencia(ReferenciaRecepcion referencia, IAnularReferenciaServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            AnularReferenciaValidacionCarga(referencia, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            AnularReferenciaValidacionProcedimiento(referencia, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateReferenciaRecepcion(ReferenciaRecepcion referencia, IReferenciaRecepcionServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values
            if (referencia.FechaEmitida == null)
                referencia.FechaEmitida = DateTime.Now;
            #endregion

            ReferenciaValidacionCarga(referencia, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            ReferenciaValidacionProcedimiento(referencia, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateModificarDetalleReferencia(ReferenciaRecepcion referencia, IModificarDetalleReferenciaServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            ModificarReferenciaValidacionCarga(referencia, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            ModificarReferenciaValidacionProcedimiento(referencia, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidatePicking(DetallePreparacion pickeo, IPickingServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            var errors = new List<Error>();

            PickingValidacionCarga(pickeo, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            PickingValidacionProcedimiento(pickeo, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidatePickingSaldos(List<DetallePreparacion> pickeos, IPickingServiceContext context)
        {
            var errors = new List<Error>();

            #region Cantidades de stock

            var stocks = pickeos.GroupBy(x => new { x.Ubicacion, x.Empresa, x.Producto, x.Lote, x.Faixa })
            .Select(x => new Stock
            {
                Ubicacion = x.Key.Ubicacion,
                Empresa = x.Key.Empresa,
                Producto = x.Key.Producto,
                Identificador = x.Key.Lote,
                Faixa = x.Key.Faixa,
                Cantidad = x.Sum(d => d.Cantidad)
            });

            foreach (var s in stocks)
            {
                var stock = context.GetStock(s.Ubicacion, s.Empresa, s.Producto, s.Identificador, s.Faixa);
                if (stock == null)
                    errors.Add(new Error("WMSAPI_msg_Error_SinStock", new object[] { s.Ubicacion, s.Empresa, s.Producto, s.Identificador, s.Faixa }));
                else if (s.Cantidad > stock.ReservaSalida)
                    errors.Add(new Error("WMSAPI_msg_Error_ReservaInsuficiente", new object[] { s.Ubicacion, s.Empresa, s.Producto, s.Identificador, s.Faixa }));
            }

            #endregion

            #region Cantidades picking

            var preparaciones = pickeos.GroupBy(x => new
            {
                x.NumeroPreparacion,
                x.Agrupacion
            }).Select(x => x.Key).OrderBy(x => x.NumeroPreparacion).ToList();

            foreach (var prep in preparaciones)
            {
                IEnumerable<DetallePreparacion> detalles = new List<DetallePreparacion>();
                var detallesPrep = pickeos.Where(x => x.NumeroPreparacion == prep.NumeroPreparacion);

                switch (prep.Agrupacion)
                {
                    case Agrupacion.Pedido:
                        detalles = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Pedido, x.Empresa, x.Cliente, x.Producto, x.Lote, x.Faixa })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Pedido = x.Key.Pedido,
                            Empresa = x.Key.Empresa,
                            Cliente = x.Key.Cliente,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    case Agrupacion.Cliente:
                        detalles = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Empresa, x.Cliente, x.Producto, x.Lote, x.Faixa, x.ComparteContenedorPicking })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Empresa = x.Key.Empresa,
                            Cliente = x.Key.Cliente,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            ComparteContenedorPicking = x.Key.ComparteContenedorPicking,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    case Agrupacion.Ruta:
                        detalles = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Empresa, x.Producto, x.Lote, x.Faixa, x.Carga, x.ComparteContenedorPicking })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Empresa = x.Key.Empresa,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            Carga = x.Key.Carga,
                            ComparteContenedorPicking = x.Key.ComparteContenedorPicking,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    case Agrupacion.Onda:
                        detalles = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Empresa, x.Producto, x.Lote, x.Faixa, x.ComparteContenedorPicking })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Empresa = x.Key.Empresa,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            ComparteContenedorPicking = x.Key.ComparteContenedorPicking,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    default:
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionSinAgrupacion", prep.NumeroPreparacion));
                        break;
                }

                foreach (var d in detalles)
                {
                    var cantDetallePendiente = context.GetCantidadPendiente(d, prep.Agrupacion);
                    if (d.Cantidad > cantDetallePendiente)
                        errors.Add(new Error("WMSAPI_msg_Error_CantPrepararMayorPendiente", new object[] { d.Producto, d.Lote, d.NumeroPreparacion, d.Pedido, d.Empresa, d.Cliente, d.Ubicacion, d.ComparteContenedorPicking, cantDetallePendiente }));
                }
            }
            #endregion

            #region Cantidades documentales

            if (context.ManejaDocumental())
            {
                var detReservas = pickeos.GroupBy(x => new { x.NumeroPreparacion, x.Empresa, x.Producto, x.Lote, x.Faixa })
                .Select(x => new DocumentoPreparacionReserva
                {
                    Preparacion = x.Key.NumeroPreparacion,
                    Empresa = x.Key.Empresa,
                    Producto = x.Key.Producto,
                    Faixa = x.Key.Faixa,
                    Identificador = x.Key.Lote,
                    CantidadProducto = x.Sum(d => d.Cantidad)
                });

                foreach (var d in detReservas)
                {
                    var reservas = context.GetReservasDetalles(d.Preparacion, d.Empresa, d.Producto, d.Identificador, d.Faixa);
                    if (reservas == null || reservas.Count() == 0)
                        errors.Add(new Error("WMSAPI_msg_Error_SinReservaDocumentalSuficiente", new object[] { d.Preparacion, d.Empresa, d.Producto, d.Identificador }));
                    else if (d.CantidadProducto > reservas.Sum(r => (r.CantidadProducto ?? 0) - (r.CantidadPreparada ?? 0)))
                        errors.Add(new Error("WMSAPI_msg_Error_SinReservaDocumentalSuficiente", new object[] { d.Preparacion, d.Empresa, d.Producto, d.Identificador }));
                }
            }
            #endregion

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateIngreso(IngresoProduccion ingreso, IProduccionServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            ProduccionValidacionCarga(ingreso, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            ProduccionValidacionProcedimiento(ingreso, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateProducirProduccion(ProducirProduccion produccion, IProducirProduccionServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            ProducirProduccionValidacionCarga(produccion, context, errors);

            if (errors.Any())
                return Task.FromResult(errors);

            ProducirProduccionValidacionProcedimiento(produccion, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateSaldosProduccion(ProducirProduccion produccion, IProducirProduccionServiceContext context)
        {
            var errors = new List<Error>();

            var insumosAgrupados = produccion.Productos.GroupBy(x => new { x.Empresa, x.Producto, x.Faixa, x.Identificador })
            .Select(x => new IngresoProduccionDetalleReal
            {
                Empresa = x.Key.Empresa,
                Producto = x.Key.Producto,
                Faixa = x.Key.Faixa,
                Identificador = x.Key.Identificador,
                QtReal = x.Sum(d => d.Cantidad)
            });

            foreach (var s in insumosAgrupados)
            {
                var producto = context.GetProducto(s.Empresa.Value, s.Producto);

                if (producto.ManejoIdentificador == ManejoIdentificador.Serie && s.QtReal != 1)
                    errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
            }

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateConsumirProduccion(ConsumirProduccion consumo, IConsumirProduccionServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            ConsumirProduccionValidacionCarga(consumo, context, errors);

            if (errors.Any())
                return Task.FromResult(errors);

            ConsumirProduccionValidacionProcedimiento(consumo, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateSaldosConsumo(ConsumirProduccion consumo, IConsumirProduccionServiceContext context)
        {
            var errors = new List<Error>();

            var insumosAgrupados = consumo.Insumos.GroupBy(x => new { x.Empresa, x.Producto, x.Faixa, x.Identificador })
            .Select(x => new IngresoProduccionDetalleReal
            {
                Empresa = x.Key.Empresa,
                Producto = x.Key.Producto,
                Faixa = x.Key.Faixa,
                Identificador = x.Key.Identificador,
                QtReal = x.Sum(d => d.Cantidad)
            });

            foreach (var s in insumosAgrupados)
            {
                var insumosDisponibles = context.DetallesInsumos
                    .Where(d => d.Empresa == s.Empresa
                        && d.Producto == s.Producto
                        && d.Faixa == s.Faixa
                        && d.Identificador == s.Identificador)
                    .ToList();

                if (insumosDisponibles.Count == 0)
                    errors.Add(new Error("WMSAPI_msg_Error_IngresoRealNoExiste", s.Producto, s.Identificador, consumo.IdProduccionExterno));
                else if (s.QtReal > insumosDisponibles.Sum(d => d.QtReal))
                    errors.Add(new Error("WMSAPI_msg_Error_CantidadDisponibleInsuficiente", s.Producto, s.Identificador));
            }

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateUbicacionImportada(UbicacionExterna ubicacion, IUbicacionServiceContext context, out bool errorProcedimiento)
        {
            errorProcedimiento = false;
            List<Error> errors = [];

            UbicacionValidacionCarga(ubicacion, context, errors);

            if (errors.Count != 0)
                return Task.FromResult(errors);

            UbicacionValidacionProcedimiento(ubicacion, context, errors);

            errorProcedimiento = errors.Count != 0;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateDetalleRecorrido(DetalleRecorrido detalle, IRecorridoServiceContext context, out bool errorProcedimiento)
        {
            errorProcedimiento = false;
            List<Error> errors = [];

            DetalleRecorridoValidacionCarga(detalle, context, errors);

            if (errors.Count != 0)
                return Task.FromResult(errors);

            DetalleRecorridoValidacionProcedimiento(detalle, context, errors);

            errorProcedimiento = errors.Count != 0;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateUbicacionesPicking(UbicacionPickingProducto ubicacionPicking, PickingProductoServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            SetDefaultValuesUbicacionPicking(ubicacionPicking, context);

            PickingProductoValidacionCarga(ubicacionPicking, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            PickingProductoValidacionProcedimiento(ubicacionPicking, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateDetalleCrossDocking(CrossDockingUnaFase detalle, ICrossDockingServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            var errors = new List<Error>();

            SaldoPendienteCrossDockingValidacionProcedimiento(detalle, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidateFactura(Factura factura, IFacturaServiceContext context, out bool errorProcedimiento)
        {
            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            errorProcedimiento = false;
            List<Error> errors = new List<Error>();

            #region Default values
            if (factura.FechaEmision == null)
                factura.FechaEmision = DateTime.Now;

            if (factura.TipoAgente == null)
                factura.TipoAgente = TipoAgenteDb.Proveedor;
            #endregion

            FacturaValidacionCarga(factura, context, errors); // Largo-Formatos-Nulos

            if (errors.Any())
                return Task.FromResult(errors);

            FacturaValidacionProcedimiento(factura, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return Task.FromResult(errors);
        }

        #region Agenda
        public virtual bool AgendaValidacionCarga(Agenda agenda, IAgendaServiceContext context, List<Error> errors)
        {
            ValidarCampo("CodigoAgente", agenda.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", agenda.TipoAgente, true, typeof(string), 3, errors);
            ValidarCampo("TipoRecepcion", agenda.TipoRecepcionInterno, true, typeof(string), 6, errors);
            ValidarCampo("Predio", agenda.Predio, true, typeof(string), 10, errors);

            ValidarCampo("Referencia", agenda.NumeroDocumento, false, typeof(string), 30, errors);
            ValidarCampo("TipoReferencia", agenda.TipoReferenciaId, false, typeof(string), 6, errors);

            ValidarCampo("Anexo1", agenda.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo("Anexo2", agenda.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo("Anexo3", agenda.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo("Anexo4", agenda.Anexo4, false, typeof(string), 200, errors);

            ValidarCampo("PlacaVehiculo", agenda.PlacaVehiculo, true, typeof(string), 30, errors);
            ValidarCampo("FechaEntrega", agenda.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            return true;
        }

        public virtual bool AgendaValidacionProcedimiento(Agenda agenda, IAgendaServiceContext context, List<Error> errors)
        {
            if (!ValidarTipoAgente(agenda.TipoAgente, context.ExisteTipoAgente, errors))
                return false;

            if (!ValidarAgente(agenda.CodigoAgente, agenda.IdEmpresa, agenda.TipoAgente,
                t => context.GetAgente(t.Item1, t.Item2, t.Item3), errors, out string cdCliente))
                return false;

            if (!ValidarTipoRecepcion(agenda, context, errors, out var tpRecEmpresa))
                return false;


            ValidarPredio(agenda.Predio, context.ExistePredio, errors);
            ValidarPuerta(agenda.CodigoPuerta, agenda.Predio, context.ExistePuertaIn, errors);
            ValidarFechaMenorA("FechaEntrega", agenda.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

            if (tpRecEmpresa.RecepcionTipoInterno?.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Lpn)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoRecepcionNoPermitido", "TipoRecepcion"));
                return false;
            }
            else if (tpRecEmpresa.RecepcionTipoInterno?.TipoSeleccionReferencia != TipoSeleccionReferenciaDb.Libre)
            {
                if (string.IsNullOrEmpty(agenda.NumeroDocumento))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_Requerido", "Referencia"));
                    return false;
                }
                else if (string.IsNullOrEmpty(agenda.TipoReferenciaId))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_Requerido", "TipoReferencia"));
                    return false;
                }

                if (!ValidarTipoReferencia(agenda.TipoReferenciaId, agenda.TipoAgente, agenda.TipoRecepcionInterno, context.ExisteTipoReferencia, context.ExisteTpRefTpRecepcion,
                    t => context.ExisteTpRefTpAgente(t.Item1, t.Item2), t => context.TipoRecCompatibleTpReferencia(t.Item1, t.Item2), errors))
                    return false;

                var refAux = context.GetReferencia(agenda.NumeroDocumento, agenda.IdEmpresa, agenda.TipoReferenciaId, cdCliente);
                if (refAux == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ReferenciaNOExiste", agenda.NumeroDocumento, agenda.TipoReferenciaId, agenda.TipoAgente, agenda.CodigoAgente, agenda.IdEmpresa));
                    return false;
                }
                else if (refAux.IdPredio != agenda.Predio)
                {
                    AddError(errors, "WMSAPI_msg_Error_PredioReferenciaDistinto");
                    return false;
                }
                else if (refAux.Estado != EstadoReferenciaRecepcionDb.Abierta)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_EsadoReferenciaNoValido", refAux.Estado));
                    return false;
                }
                else if (!context.ReferenciaSaldoDisponible(refAux))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ReferenciaSinSaldoDisponible", refAux.Numero));
                    return false;
                }
                agenda.ReferenciaId = refAux.Id;
            }

            return true;
        }
        #endregion

        #region Agente
        public virtual bool AgenteValidacionCarga(Agente agente, IAgenteServiceContext context, List<Error> errors)
        {
            var oldModel = context.GetAgente(agente.Tipo, agente.Codigo);
            var camposInmutables = context.GetCamposInmutables();

            ValidarCampo(oldModel, agente, camposInmutables, "Tipo", agente.Tipo, true, typeof(string), 3, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "CodigoAgente", agente.Codigo, true, typeof(string), 40, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Descripcion", agente.Descripcion, true, typeof(string), 100, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Ruta", agente.RutaId?.ToString(), true, typeof(short), 3, errors, distintoCero: false, campoInterno: nameof(agente.RutaId));
            ValidarCampo(oldModel, agente, camposInmutables, "Estado", agente.EstadoId?.ToString(), true, typeof(short), 3, errors, campoInterno: nameof(agente.EstadoId));

            ValidarCampo(oldModel, agente, camposInmutables, "Categoria", agente.Categoria, false, typeof(string), 10, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "CodigoPostal", agente.CodigoPostal, false, typeof(string), 15, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "NumeroFiscal", agente.NumeroFiscal, false, typeof(string), 30, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "ClienteConsolidado", agente.ClienteConsolidado, false, typeof(string), 10, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "EmpresaConsolidada", agente.EmpresaConsolidada?.ToString(), false, typeof(int), 10, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "NumeroLocalizacionGlobal", agente.NumeroLocalizacionGlobal?.ToString(), false, typeof(long), 16, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "GrupoConsulta", agente.GrupoConsulta, false, typeof(string), 20, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "PuntoDeEntrega", agente.PuntoDeEntrega, false, typeof(string), 20, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Anexo1", agente.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Anexo2", agente.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Anexo3", agente.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Anexo4", agente.Anexo4, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Email", agente.Email, false, typeof(string), 100, errors, formatoEmail: true);
            ValidarCampo(oldModel, agente, camposInmutables, "Barrio", agente.Barrio, false, typeof(string), 50, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "Direccion", agente.Direccion, false, typeof(string), 100, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "AceptaDevolucion", agente.AceptaDevolucionId, false, typeof(string), 1, errors, campoInterno: nameof(agente.AceptaDevolucionId));
            ValidarCampo(oldModel, agente, camposInmutables, "IdClienteFilial", agente.IdClienteFilial, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "TipoFiscal", agente.TipoFiscalId, false, typeof(string), 20, errors, campoInterno: nameof(agente.TipoFiscalId));
            ValidarCampo(oldModel, agente, camposInmutables, "CaracteristicaTelefonica", agente.NuDDD, false, typeof(string), 15, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "TelefonoSecundario", agente.TelefonoSecundario, false, typeof(string), 30, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "OtroDatoFiscal", agente.OtroDatoFiscal, false, typeof(string), 30, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "OrdenDeCarga", agente.OrdenDeCarga?.ToString(), false, typeof(short), 3, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "TelefonoPrincipal", agente.TelefonoPrincipal, false, typeof(string), 30, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "TipoActividad", agente.TipoActividad, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, agente, camposInmutables, "ValorManejoVidaUtil", agente.ValorManejoVidaUtil?.ToString(), false, typeof(decimal), 15, errors, 3, false);

            ValidarCampo(oldModel, agente, camposInmutables, "Pais", agente.PaisId, false, typeof(string), 2, errors, campoInterno: nameof(agente.PaisId));
            ValidarCampo(oldModel, agente, camposInmutables, "Subdivisión", agente.SubdivisionId, false, typeof(string), 20, errors, campoInterno: nameof(agente.SubdivisionId));
            ValidarCampo(oldModel, agente, camposInmutables, "Municipio", agente.MunicipioId, false, typeof(string), 20, errors, campoInterno: nameof(agente.MunicipioId));

            return true;
        }

        public virtual bool AgenteValidacionProcedimiento(Agente agente, IAgenteServiceContext context, List<Error> errors)
        {
            ValidarSituacion(agente.EstadoId ?? -1, errors);
            ValidarRuta(agente.RutaId ?? -1, context.ExisteRuta, errors);

            if (!agente.Codigo.IsUpper())
                AddError(errors, "WMSAPI_msg_Error_AgenteCaracteresMinuscula");

            ValidarTipoAgente(agente.Tipo, context.ExisteTipoAgente, errors);

            if (agente.NumeroLocalizacionGlobal != null)
            {
                if (!Validations.IsValidGLN(agente.NumeroLocalizacionGlobal ?? -1))
                    AddError(errors, "WMSAPI_msg_Error_GLNNoValido");
            }

            if (agente.Tipo == TipoAgenteDb.Cliente)
            {
                if (agente.Codigo.Length > 10)
                    AddError(errors, "WMSAPI_msg_Error_CodigoAgenteCliLargoMax", new object[] { agente.Codigo, agente.Empresa, agente.Tipo });
            }

            ValidarGrupoConsulta(agente.GrupoConsulta, context.ExisteGrupoConsulta, errors);
            ValidarLocalidad(agente, null, context.ExistePais, (t => context.ExisteLocalidad(t.Item1, t.Item2)), context.GetSubdivision, (t => context.GetLocalidadId(t.Item1, t.Item2)), errors);

            return true;
        }
        #endregion

        #region AnulacionPickingPedidoPendiente
        public virtual void AnularPickingPedidoPendienteCarga(AnularPickingPedidoPendiente detalle, IAnularPickingPedidoPendienteContext context, List<Error> errors)
        {
            ValidarCampo("Empresa", detalle.Empresa.ToString(), true, typeof(int), 10, errors);
            ValidarCampo("Pedido", detalle.Pedido.ToString(), true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", detalle.TipoAgente.ToString(), true, typeof(string), 3, errors);
            ValidarCampo("CodigoAgente", detalle.CodigoAgente.ToString(), true, typeof(string), 40, errors);
            ValidarCampo("Preparacion", detalle.Preparacion.ToString(), true, typeof(int), 6, errors);
            ValidarCampo("EstadoPicking", detalle.EstadoPicking.ToString(), false, typeof(string), 20, errors);

        }

        public virtual void AnularPickingPedidoPendienteProcedimiento(AnularPickingPedidoPendiente detalle, IAnularPickingPedidoPendienteContext context, List<Error> errors)
        {
            string cdCliente;

            Agente agente = context.GetAgente(detalle.Empresa, detalle.CodigoAgente, detalle.TipoAgente);
            if (agente != null)
            {
                detalle.Cliente = agente.CodigoInterno;
                Pedido pedido = context.GetPedido(detalle.Pedido, detalle.Empresa, detalle.Cliente);
                if (pedido == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoNoEncontrado", detalle.Pedido, detalle.CodigoAgente, detalle.TipoAgente, detalle.Empresa.ToString()));
                }
                else
                {
                    if (detalle.EstadoPicking != EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE && detalle.EstadoPicking != EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EstadoPickingValidos", EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE, EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO));
                    }
                    else if (context.AnyPreparacionPedido(detalle.Preparacion, detalle.Pedido, detalle.Empresa, detalle.Cliente))
                    {
                        List<DetallePreparacion> detalles = context.GetDetallesPreparacionPedido(detalle.Preparacion, detalle.Pedido, detalle.Empresa, detalle.Cliente, detalle.EstadoPicking);
                        if (detalles.Count == 0)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_PreparacionPedido", detalle.Preparacion, detalle.Pedido, detalle.CodigoAgente, detalle.TipoAgente, detalle.Empresa.ToString()));
                        }
                        else
                        {
                            if (context.AnyAnulacionPreparacionPendiente(detalle.Preparacion))
                            {
                                errors.Add(new Error("WMSAPI_msg_Error_AnulacionPreparacionPendiente", detalle.Preparacion));
                            }
                        }
                    }
                    else
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionPedido", detalle.Preparacion, detalle.Pedido, detalle.CodigoAgente, detalle.TipoAgente, detalle.Empresa.ToString()));
                    }
                }
            }
            else
            {
                errors.Add(new Error("WMSAPI_msg_Error_AgenteNoExite", detalle.CodigoAgente, detalle.TipoAgente, detalle.Empresa.ToString()));
            }
        }
        public virtual void AnularPickingPedidoPendienteAutomatismoCarga(AnularPickingPedidoPendienteAutomatismo detalle, AnularPickingPedidoPendienteAutomatismoContext context, List<Error> errors)
        {
            ValidarCampo("Empresa", detalle.Empresa.ToString(), true, typeof(int), 10, errors);
            ValidarCampo("Pedido", detalle.Pedido?.ToString(), false, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", detalle.TipoAgente?.ToString(), false, typeof(string), 3, errors);
            ValidarCampo("CodigoAgente", detalle.CodigoAgente?.ToString(), false, typeof(string), 40, errors);
            ValidarCampo("Preparacion", detalle.Preparacion.ToString(), true, typeof(int), 6, errors);
            ValidarCampo("EstadoPicking", detalle.EstadoPicking.ToString(), false, typeof(string), 20, errors);
        }

        public virtual void AnularPickingPedidoPendienteAutomatismoProcedimiento(AnularPickingPedidoPendienteAutomatismo detalle, AnularPickingPedidoPendienteAutomatismoContext context, List<Error> errors)
        {
            string cdCliente;

            Agente agente = context.GetAgente(detalle.Empresa, detalle.CodigoAgente, detalle.TipoAgente);
            if (agente != null)
            {
                detalle.Cliente = agente.CodigoInterno;
                Pedido pedido = context.GetPedido(detalle.Pedido, detalle.Empresa, detalle.Cliente);

                if (pedido != null)
                {
                    if (detalle.EstadoPicking != EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE && detalle.EstadoPicking != EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EstadoPickingValidos", EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE, EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO));
                    }
                    else if (context.AnyPreparacionPedido(detalle.Preparacion, detalle.Pedido, detalle.Empresa, detalle.Cliente))
                    {
                        List<DetallePreparacion> detalles = context.GetDetallesPreparacionPedido(detalle.Preparacion, detalle.Pedido, detalle.Empresa, detalle.Cliente, detalle.EstadoPicking);
                        if (detalles.Count == 0)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_PreparacionPedido", detalle.Preparacion, detalle.Pedido, detalle.CodigoAgente, detalle.TipoAgente, detalle.Empresa.ToString()));
                        }
                        else
                        {
                            if (context.AnyAnulacionPreparacionPendiente(detalle.Preparacion))
                            {
                                errors.Add(new Error("WMSAPI_msg_Error_AnulacionPreparacionPendiente", detalle.Preparacion));
                            }
                        }
                    }
                    else
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionPedido", detalle.Preparacion, detalle.Pedido, detalle.CodigoAgente, detalle.TipoAgente, detalle.Empresa.ToString()));
                    }
                }
            }
        }

        #endregion

        #region CodigoBarras

        public virtual bool CodigoBarrasValidacionCarga(CodigoBarras codigoDeBarras, ICodigoBarrasServiceContext context, List<Error> errors)
        {
            ValidarCodigoBarras(codigoDeBarras.Codigo, context, errors);

            ValidarCampo("TipoOperacion", codigoDeBarras.TipoOperacionId, true, typeof(string), 1, errors);

            switch (codigoDeBarras.TipoOperacionId)
            {
                case TipoOperacionDb.Alta:
                    ValidarCampo(codigoDeBarras, errors);
                    break;

                case TipoOperacionDb.Baja:
                    ValidarCampo(codigoDeBarras, errors);
                    break;

                case TipoOperacionDb.Sobrescritura:
                    ValidarCampoModificacion(codigoDeBarras, errors, context);
                    break;
            }

            return true;
        }

        public virtual void ValidarCampoModificacion(CodigoBarras codigoDeBarras, List<Error> errors, ICodigoBarrasServiceContext context)
        {
            var camposInmutables = context.GetCamposInmutables();
            var oldModel = context.GetCodigoBarras(codigoDeBarras.Codigo);

            ValidarCampo(oldModel, codigoDeBarras, camposInmutables, "Producto", codigoDeBarras.Producto, true, typeof(string), 40, errors);
            ValidarCampo(oldModel, codigoDeBarras, camposInmutables, "TipoCodigo", codigoDeBarras.TipoCodigo?.ToString(), false, typeof(int), 8, errors, 0, false);
            ValidarCampo(oldModel, codigoDeBarras, camposInmutables, "PrioridadUso", codigoDeBarras.PrioridadUso?.ToString(), false, typeof(short), 2, errors, 0, false);
            ValidarCampo(oldModel, codigoDeBarras, camposInmutables, "CantidadEmbalaje", codigoDeBarras.CantidadEmbalaje?.ToString(), false, typeof(decimal), 10, errors, 2, false);
        }

        public virtual void ValidarCampo(CodigoBarras codigoDeBarras, List<Error> errors)
        {
            ValidarCampo("Producto", codigoDeBarras.Producto, true, typeof(string), 40, errors);
            ValidarCampo("TipoCodigo", codigoDeBarras.TipoCodigo?.ToString(), false, typeof(int), 8, errors, 0, false);
            ValidarCampo("PrioridadUso", codigoDeBarras.PrioridadUso?.ToString(), false, typeof(short), 2, errors, 0, false);
            ValidarCampo("CantidadEmbalaje", codigoDeBarras.CantidadEmbalaje?.ToString(), false, typeof(decimal), 10, errors, 2, false);
        }

        public virtual bool CodigoBarrasValidacionProcedimiento(CodigoBarras codigoDeBarras, ICodigoBarrasServiceContext context, List<Error> errors)
        {
            ValidarProducto(codigoDeBarras.Producto, codigoDeBarras.Empresa, context.GetProducto(codigoDeBarras.Empresa, codigoDeBarras.Producto), errors, context.PermiteProductoInactivos);

            if (codigoDeBarras.TipoCodigo != null)
                ValidarTpCodigoBarras(codigoDeBarras.TipoCodigo ?? -1, context, errors);

            if (!ValidarTipoOperacion(codigoDeBarras.TipoOperacionId, true, errors))
                return false;

            CodigoBarras modelo = context.GetCodigoBarras(codigoDeBarras.Codigo);

            if (modelo != null && codigoDeBarras.TipoOperacionId == "A")
            {
                errors.Add(new Error("WMSAPI_msg_Error_BarrasAsignadoOtroProducto", modelo.Producto));
                return false;
            }
            else if (modelo == null && codigoDeBarras.TipoOperacionId == "B")
            {
                errors.Add(new Error("WMSAPI_msg_Error_DeleteCodigoNoExiste"));
                return false;
            }

            return true;
        }

        public virtual bool ValidarCodigoBarras(string cdBarras, ICodigoBarrasServiceContext context, List<Error> errors)
        {
            ValidarCampo("Codigo", cdBarras, true, typeof(string), 50, errors);

            if (!string.IsNullOrEmpty(cdBarras))
            {
                string caracteresPermitidos = context.GetParametro(ParamManager.LISTA_CARACTERES_COD_BARRA);

                if (!Validations.ValidarCaracteres("Codigo", cdBarras, caracteresPermitidos, out Error error))
                {
                    errors.Add(error);
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarTpCodigoBarras(int tp, ICodigoBarrasServiceContext context, List<Error> errors)
        {
            if (!context.ExisteTipoCodigoBarras(tp))
            {
                errors.Add(new Error("WMSAPI_msg_Error_tpCodBarrasNoExiste", tp));
                return false;
            }
            return true;
        }
        #endregion

        #region Egreso
        public virtual bool EgresoValidacionCarga(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            var vehiculoRequerido = false;
            var matriculaRequerida = false;
            var transportistaRequerido = false;
            var predioRequerido = false;

            ValidarCampo("Descripcion", egreso.Descripcion, true, typeof(string), 50, errors);

            ValidarCampo("PredioExterno", egreso.PredioExterno, false, typeof(string), 50, errors);

            if (string.IsNullOrEmpty(egreso.PredioExterno))
                predioRequerido = true;
            else if (!string.IsNullOrEmpty(egreso.Predio) && !string.IsNullOrEmpty(egreso.PredioExterno))
                AddError(errors, "WMSAPI_msg_Error_PredioYPredioExterno");

            ValidarCampo("Predio", egreso.Predio, predioRequerido, typeof(string), 10, errors);

            ValidarCampo("IdExterno", egreso.IdExterno, false, typeof(string), 50, errors);
            ValidarCampo("Documento", egreso.Documento, false, typeof(string), 50, errors);
            ValidarCampo("Tracking", egreso.TrackingHabilitadoId, false, typeof(string), 1, errors);

            if (egreso.TrackingHabilitadoId == "S")
                vehiculoRequerido = true;

            ValidarCampo("CodigoVehiculo", egreso.Vehiculo?.ToString(), vehiculoRequerido, typeof(int), 10, errors);

            if (egreso.Vehiculo == null)
            {
                matriculaRequerida = true;
                transportistaRequerido = true;
            }

            ValidarCampo("Matricula", egreso.Matricula, matriculaRequerida, typeof(string), 15, errors);

            var cdTransportista = egreso.Transportista == -1 ? "" : egreso.Transportista.ToString();
            ValidarCampo("Transportista", cdTransportista, transportistaRequerido, typeof(int), 10, errors);


            ValidarCampo("Ruta", egreso.Ruta?.ToString(), false, typeof(short), 3, errors, distintoCero: false);
            ValidarCampo("Empresa", egreso.Empresa?.ToString(), false, typeof(int), 10, errors);
            ValidarCampo("Puerta", egreso.Puerta?.ToString(), false, typeof(short), 3, errors);
            ValidarCampo("ProgramacionFecha", egreso.FechaProgramado?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("ProgramacionHoraInicio", egreso.ProgramacionHoraInicio, false, typeof(string), 5, errors, formatoHora: true);
            ValidarCampo("ProgramacionHoraFin", egreso.ProgramacionHoraFin, false, typeof(string), 5, errors, formatoHora: true);
            ValidarCampo("Necesidades", egreso.Necesidades, false, typeof(string), 200, errors);

            ValidarCampo("RespetaOrdenCarga", egreso.RespetaOrdenCargaId, false, typeof(string), 1, errors);
            ValidarCampo("Ruteo", egreso.RuteoHabilitadoId, false, typeof(string), 1, errors);
            ValidarCampo("TrackingSincronizado", egreso.SincronizacionRealizadaId, false, typeof(string), 1, errors);
            ValidarCampo("CierreHabilitado", egreso.CierreHabilitadoId, false, typeof(string), 1, errors);
            ValidarCampo("CargaHabilitada", egreso.CargaHabilitadaId, false, typeof(string), 1, errors);
            ValidarCampo("CierreParcial", egreso.CierreParcialHabilitadoId, false, typeof(string), 1, errors);
            ValidarCampo("CierreAutomatico", egreso.CierreAutomaticoHabilitadoId, false, typeof(string), 1, errors);
            ValidarCampo("CargaLibre", egreso.CargaAutomaticaHabilitadaId, false, typeof(string), 1, errors);
            ValidarCampo("ArmadoHabilitado", egreso.ArmadoHabilitadoId, false, typeof(string), 1, errors);
            ValidarCampo("HabilitarUsoCargaAsignada", egreso.HabilitarUsoCargaAsignada, false, typeof(string), 1, errors);
            ValidarCampo("RequiereControlContenedores", egreso.ControlContenedoresHabilitadoId, false, typeof(string), 1, errors);

            if (errors.Any())
                return false;

            if (egreso.DetalleArmadoEgreso != null)
            {
                foreach (var p in egreso.DetalleArmadoEgreso.Pedidos)
                {
                    ValidarCampo("NroPedido", p.NroPedido, true, typeof(string), 40, errors);
                    ValidarCampo("CodigoAgente", p.CodigoAgente, true, typeof(string), 10, errors);
                    ValidarCampo("TipoAgente", p.TipoAgente, true, typeof(string), 3, errors);
                    ValidarCampo("Empresa", p.Empresa.ToString(), true, typeof(int), 10, errors);

                    var keyPedido = $"{p.NroPedido}.{p.CodigoAgente}.{p.TipoAgente}.{p.Empresa}";

                    if (context.KeysDetallesEgreso.KeyPedidos.Contains(keyPedido))
                        errors.Add(new Error("WMSAPI_msg_Error_EgresoPeiddoDatosDuplicados", p.NroPedido, p.CodigoAgente, p.TipoAgente, p.Empresa));
                    else
                        context.KeysDetallesEgreso.KeyPedidos.Add(keyPedido);

                    if (errors.Any())
                        return false;
                }

                foreach (var ca in egreso.DetalleArmadoEgreso.Cargas)
                {
                    ValidarCampo("Carga", ca.Carga.ToString(), true, typeof(long), 15, errors);
                    ValidarCampo("CodigoAgente", ca.CodigoAgente, true, typeof(string), 10, errors);
                    ValidarCampo("TipoAgente", ca.TipoAgente, true, typeof(string), 3, errors);
                    ValidarCampo("Empresa", ca.Empresa.ToString(), true, typeof(int), 10, errors);

                    var keyCarga = $"{ca.Carga}.{ca.CodigoAgente}.{ca.TipoAgente}.{ca.Empresa}";

                    if (context.KeysDetallesEgreso.KeyCargas.Contains(keyCarga))
                        errors.Add(new Error("WMSAPI_msg_Error_EgresoCargaDuplicados", ca.Carga, ca.CodigoAgente, ca.TipoAgente, ca.Empresa));
                    else
                        context.KeysDetallesEgreso.KeyCargas.Add(keyCarga);

                    if (errors.Any())
                        return false;
                }

                foreach (var co in egreso.DetalleArmadoEgreso.Contenedores)
                {
                    ValidarCampo("IdExternoContenedor", co.IdExternoContenedor, true, typeof(string), 50, errors);
                    ValidarCampo("TipoContenedor", co.TipoContenedor, true, typeof(string), 10, errors);
                    ValidarCampo("Empresa", co.Empresa.ToString(), true, typeof(int), 10, errors);

                    var keyContenedor = $"{co.IdExternoContenedor}.{co.TipoContenedor}.{co.Empresa}";

                    if (context.KeysDetallesEgreso.KeyContenedores.Contains(keyContenedor))
                        errors.Add(new Error("WMSAPI_msg_Error_EgresoContenedorDuplicados", co.IdExternoContenedor, co.TipoContenedor, co.Empresa));
                    else
                        context.KeysDetallesEgreso.KeyContenedores.Add(keyContenedor);

                    if (errors.Any())
                        return false;
                }
            }

            return true;
        }

        public virtual bool EgresoValidacionProcedimiento(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            ValidarIdExternoEgreso(egreso, context, errors);
            ValidarPredioEgreso(egreso, context, errors);
            ValidarVehiculoEgreso(egreso, context, errors);
            ValidarTransportista(egreso.Transportista, context.ExisteTransportista, errors);
            ValidarRutaEgreso(egreso, context, errors);
            ValidarEmpresa(egreso.Empresa, context.ExisteEmpresa, errors);
            ValidarPuertaEgreso(egreso, context, errors);
            ValidarFechaMenorA("ProgramacionFecha", egreso.FechaProgramado?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

            if (errors.Any())
                return false;

            if (egreso.DetalleArmadoEgreso != null)
                ValidarDetalleEgreso(context, egreso, errors);

            if (errors.Any())
                return false;

            if (egreso.TrackingHabilitadoId == "N")
                egreso.SincronizacionRealizadaId = "N";

            if (egreso.ForzarHabilitarArmado())
                egreso.ArmadoHabilitadoId = "S";

            return true;
        }

        public virtual bool ValidarDetalleEgreso(IEgresoServiceContext context, Camion egreso, List<Error> errors)
        {
            var usoCargaAsignadaHabilitado = egreso.HabilitarUsoCargaAsignada == "S";

            if (!ValidarPedidos(context, egreso, usoCargaAsignadaHabilitado, errors))
                return false;

            if (!ValidarCargas(context, egreso, usoCargaAsignadaHabilitado, errors))
                return false;

            if (!ValidarContenedores(context, egreso, usoCargaAsignadaHabilitado, errors))
                return false;

            return true;
        }

        public virtual bool ValidarPedidos(IEgresoServiceContext context, Camion egreso, bool usoCargaAsignadaHabilitado, List<Error> errors)
        {
            if (egreso.DetalleArmadoEgreso.Pedidos != null && egreso.DetalleArmadoEgreso.Pedidos.Count > 0)
            {
                foreach (var p in egreso.DetalleArmadoEgreso.Pedidos)
                {
                    if (!context.ExisteEmpresa(p.Empresa) && !egreso.IgnorarCargasInexistentes)
                    {
                        AddError(errors, "WMSAPI_msg_Error_EmpresaNoExiste", new object[] { p.Empresa });
                        break;
                    }

                    var agente = context.GetAgente(p.CodigoAgente, p.Empresa, p.TipoAgente);
                    if (agente == null)
                    {
                        if (!egreso.IgnorarCargasInexistentes)
                        {
                            AddError(errors, "WMSAPI_msg_Error_AgenteNoEncontrado", new object[] { p.CodigoAgente, p.Empresa, p.TipoAgente });
                            break;
                        }
                    }

                    var pedido = context.GetPedidoHabilitado(p.NroPedido, p.Empresa, agente?.CodigoInterno);
                    if (pedido == null)
                    {
                        if (egreso.IgnorarCargasInexistentes)
                            p.Existe = false;
                        else
                        {
                            AddError(errors, "WMSAPI_msg_Error_PedidoNoHabilitado", new object[] { p.NroPedido, p.CodigoAgente, p.TipoAgente, p.Empresa });
                            break;
                        }
                    }

                    if (p.Existe)
                    {
                        if (egreso.Empresa != null && egreso.Empresa != p.Empresa)
                        {
                            AddError(errors, "WMSAPI_msg_Error_EmpresaDistintaAlEgreso");
                            break;
                        }
                        else if (pedido.Ruta != null && egreso.Ruta != null && egreso.Ruta != pedido.Ruta)
                        {
                            AddError(errors, "WMSAPI_msg_Error_PedidoNoPerteneceRuta");
                            break;
                        }

                        var keyPedido = $"{p.NroPedido}.{p.Empresa}.{agente.CodigoInterno}";
                        var cargasLiberadasPedido = context.PedidosCargasLiberadas.GetValueOrDefault(keyPedido, null);

                        if (cargasLiberadasPedido != null && cargasLiberadasPedido.Count > 0)
                        {
                            foreach (var dp in cargasLiberadasPedido)
                            {
                                var keyParcialCargaCamion = $"{dp.Carga}.{dp.Cliente}.{dp.Empresa}";
                                if (!usoCargaAsignadaHabilitado && context.CargaEnOtroCamion(keyParcialCargaCamion))
                                {
                                    AddError(errors, "WMSAPI_msg_Error_CargasEnOtroCamion", new object[] { dp.Carga, dp.Cliente, dp.Empresa });
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return !errors.Any();
        }

        public virtual bool ValidarCargas(IEgresoServiceContext context, Camion egreso, bool usoCargaAsignadaHabilitado, List<Error> errors)
        {
            if (egreso.DetalleArmadoEgreso.Cargas != null && egreso.DetalleArmadoEgreso.Cargas.Count > 0)
            {
                foreach (var ca in egreso.DetalleArmadoEgreso.Cargas)
                {
                    if (!context.ExisteEmpresa(ca.Empresa))
                    {
                        if (egreso.IgnorarCargasInexistentes)
                            ca.Existe = false;
                        else
                        {
                            AddError(errors, "WMSAPI_msg_Error_EmpresaNoExiste", new object[] { ca.Empresa });
                            break;
                        }
                    }

                    var agente = context.GetAgente(ca.CodigoAgente, ca.Empresa, ca.TipoAgente);
                    if (agente == null)
                    {
                        if (egreso.IgnorarCargasInexistentes)
                            ca.Existe = false;
                        else
                        {
                            AddError(errors, "WMSAPI_msg_Error_AgenteNoEncontrado", new object[] { ca.CodigoAgente, ca.Empresa, ca.TipoAgente });
                            break;
                        }
                    }

                    var carga = context.GetCargaHabilitada(ca.Carga);
                    if (carga == null)
                    {
                        if (egreso.IgnorarCargasInexistentes)
                            ca.Existe = false;
                        else
                        {
                            AddError(errors, "WMSAPI_msg_Error_CargaNoExiste", new object[] { ca.Carga });
                            break;
                        }
                    }

                    if (ca.Existe)
                    {
                        if (egreso.Empresa != null && egreso.Empresa != ca.Empresa)
                        {
                            AddError(errors, "WMSAPI_msg_Error_EmpresaDistintaAlEgreso");
                            break;
                        }
                        else if (carga.Ruta != null && egreso.Ruta != null && egreso.Ruta != carga.Ruta)
                        {
                            AddError(errors, "WMSAPI_msg_Error_CargaNoPerteneceRuta", new object[] { carga.Ruta });
                            break;
                        }

                        var keyParcialCargaCamion = $"{ca.Carga}.{agente.CodigoInterno}.{ca.Empresa}";
                        if (!usoCargaAsignadaHabilitado && context.CargaEnOtroCamion(keyParcialCargaCamion))
                        {
                            AddError(errors, "WMSAPI_msg_Error_CargasEnOtroCamion", new object[] { ca.Carga, agente.CodigoInterno, ca.Empresa });
                            break;
                        }
                    }
                }
            }

            return !errors.Any();
        }

        public virtual bool ValidarContenedores(IEgresoServiceContext context, Camion egreso, bool usoCargaAsignadaHabilitado, List<Error> errors)
        {
            if (egreso.DetalleArmadoEgreso.Contenedores != null && egreso.DetalleArmadoEgreso.Contenedores.Count > 0)
            {
                foreach (var co in egreso.DetalleArmadoEgreso.Contenedores)
                {
                    if (!context.ExisteEmpresa(co.Empresa) && !egreso.IgnorarCargasInexistentes)
                    {
                        AddError(errors, "WMSAPI_msg_Error_EmpresaNoExiste", new object[] { co.Empresa });
                        break;
                    }

                    var cont = context.GetContenedorHabilitado(co.IdExternoContenedor, co.TipoContenedor, co.Empresa);
                    if (cont == null)
                    {
                        if (egreso.IgnorarCargasInexistentes)
                            co.Existe = false;
                        else
                        {
                            AddError(errors, "WMSAPI_msg_Error_ContenedorNoHabilitado", new object[] { co.IdExternoContenedor, co.TipoContenedor, co.Empresa });
                            break;
                        }
                    }

                    if (co.Existe)
                    {
                        if (egreso.Empresa != null && egreso.Empresa != co.Empresa)
                        {
                            AddError(errors, "WMSAPI_msg_Error_EmpresaDistintaAlEgreso");
                            break;
                        }
                        else if (cont.Ruta != null && egreso.Ruta != null && egreso.Ruta != cont.Ruta)
                        {
                            AddError(errors, "WMSAPI_msg_Error_PedidoNoPerteneceRuta");
                            break;
                        }

                        var keyContenedor = $"{co.IdExternoContenedor}.{co.TipoContenedor}.{co.Empresa}";
                        var cargasContenedor = context.ContenedoresCargasLiberadas.GetValueOrDefault(keyContenedor, null);

                        if (cargasContenedor != null && cargasContenedor.Count > 0)
                        {
                            foreach (var cc in cargasContenedor)
                            {
                                var keyParcialCargaCamion = $"{cc.Carga}.{cc.Cliente}.{cc.Empresa}";
                                if (!usoCargaAsignadaHabilitado && context.CargaEnOtroCamion(keyParcialCargaCamion))
                                {
                                    AddError(errors, "WMSAPI_msg_Error_CargasEnOtroCamion", new object[] { cc.Carga, cc.Cliente, cc.Empresa });
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return !errors.Any();
        }

        public virtual bool ValidarRutaEgreso(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            if (egreso.Ruta != null)
            {
                var ruta = context.GetRuta((short)egreso.Ruta);

                if (ruta == null)
                {
                    AddError(errors, "WMSAPI_msg_Error_RutaNoExiste");
                    return false;
                }
                else if (ruta.Onda != null && !string.IsNullOrEmpty(ruta.Onda.Predio))
                {
                    var predio = !string.IsNullOrEmpty(egreso.Predio) ? egreso.Predio :
                    context.Predios.FirstOrDefault(x => x.IdExterno == egreso.PredioExterno)?.Numero;

                    if (predio != ruta.Onda.Predio)
                    {
                        AddError(errors, "WMSAPI_msg_Error_RutaNoPerteneceAlPredioEgreso");
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual bool ValidarVehiculoEgreso(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            if (egreso.Vehiculo != null)
            {
                var predioEgreso = !string.IsNullOrEmpty(egreso.Predio) ? egreso.Predio :
                context.Predios.FirstOrDefault(x => x.IdExterno == egreso.PredioExterno)?.Numero;

                var vehiculo = context.GetVehiculo((int)egreso.Vehiculo);
                if (vehiculo == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_VehiculoNoExiste", egreso.Vehiculo));
                    return false;
                }
                else if (!string.IsNullOrEmpty(vehiculo.Predio) && predioEgreso != vehiculo.Predio)
                {
                    AddError(errors, "WMSAPI_msg_Error_VehiculoNoPerteneceAlPredioEgreso");
                    return false;
                }
                else
                {
                    int? cdTransportista = vehiculo.Transportista;
                    if (cdTransportista == null && int.TryParse(context.GetParametro(ParamManager.CD_TRANSPORTADORA_DEFAULT), out int parsedValue))
                        cdTransportista = parsedValue;

                    egreso.Transportista = cdTransportista ?? 1;
                    egreso.Matricula = vehiculo.Matricula;
                }
            }
            return false;
        }

        public virtual bool ValidarIdExternoEgreso(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(egreso.IdExterno) && context.ExisteIdExterno(egreso.IdExterno))
            {
                errors.Add(new Error("WMSAPI_msg_Error_IdExternoExistente", egreso.IdExterno, context.Empresa));
                return false;
            }
            return true;
        }

        public virtual bool ValidarPredioEgreso(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(egreso.Predio))
            {
                if (!context.ExistePredio(egreso.Predio))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PredioNoExiste", egreso.Predio));
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(egreso.PredioExterno))
            {
                if (!context.ExistePredioExterno(egreso.PredioExterno))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PredioExternoNoExiste", egreso.PredioExterno));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarPuertaEgreso(Camion egreso, IEgresoServiceContext context, List<Error> errors)
        {
            var predio = !string.IsNullOrEmpty(egreso.Predio) ? egreso.Predio :
               context.Predios.FirstOrDefault(x => x.IdExterno == egreso.PredioExterno)?.Numero;
            if (egreso.Puerta != null)
            {
                if (!context.ExistePuerta((short)egreso.Puerta))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PuertaNoExiste", predio));
                    return false;
                }
                else if (context.GetPuerta(egreso.Puerta.Value).NumPredio != predio)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_LaPuertaNoPerteneceAlPeedio", new string[] { egreso.Puerta.Value.ToString(), predio })); ;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Empresa
        public virtual bool EmpresaValidacionCarga(Empresa empresa, IEmpresaServiceContext context, List<Error> errors)
        {
            var oldModel = context.GetEmpresa(empresa.Id);
            var camposInmutables = context.GetCamposInmutables();

            ValidarCampo(oldModel, empresa, camposInmutables, "Codigo", empresa.Id.ToString(), true, typeof(int), 10, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Nombre", empresa.Nombre, true, typeof(string), 55, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Estado", empresa.EstadoId.ToString(), true, typeof(short), 3, errors, campoInterno: nameof(empresa.EstadoId));

            ValidarCampo(oldModel, empresa, camposInmutables, "NumeroFiscal", empresa.NumeroFiscal, false, typeof(string), 30, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "ClienteArmadoKit", empresa.CdClienteArmadoKit, false, typeof(string), 10, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "EmpresaConsolidado", empresa.EmpresaConsolidado?.ToString(), false, typeof(int), 10, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "ProveedorDevolucion", empresa.ProveedorDevolucion?.ToString(), false, typeof(int), 7, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "ListaPrecio", empresa.ListaPrecio?.ToString(), false, typeof(int), 6, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Anexo1", empresa.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Anexo2", empresa.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Anexo3", empresa.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Anexo4", empresa.Anexo4, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "CodigoPostal", empresa.CodigoPostal, false, typeof(string), 15, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "Direccion", empresa.Direccion, false, typeof(string), 100, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "IdDAP", empresa.IdDAP, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "IdOperativo", empresa.IdOperativo, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "IdUnidadFactura", empresa.IdUnidadFactura, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "ValorMinimoStock", empresa.ValorMinimoStock?.ToString(), false, typeof(decimal), 17, errors, 3);
            ValidarCampo(oldModel, empresa, camposInmutables, "TipoFiscal", empresa.TipoFiscalId, false, typeof(string), 20, errors, campoInterno: nameof(empresa.TipoFiscalId));
            ValidarCampo(oldModel, empresa, camposInmutables, "Telefono", empresa.Telefono, false, typeof(string), 30, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "CantidadDiasPeriodo", empresa.CantidadDiasPeriodo?.ToString(), false, typeof(short), 3, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "TipoDeAlmacenajeYSeguro", empresa.cdTipoDeAlmacenajeYSeguro?.ToString(), false, typeof(short), 3, errors);
            ValidarCampo(oldModel, empresa, camposInmutables, "ValorPallet", empresa.ValorPallet?.ToString(), false, typeof(decimal), 15, errors, 2);
            ValidarCampo(oldModel, empresa, camposInmutables, "ValorPalletDia", empresa.ValorPalletDia?.ToString(), false, typeof(decimal), 15, errors, 2);
            ValidarCampo(oldModel, empresa, camposInmutables, "Pais", empresa.PaisId, false, typeof(string), 2, errors, campoInterno: nameof(empresa.PaisId));
            ValidarCampo(oldModel, empresa, camposInmutables, "Subdivisión", empresa.SubdivisionId, false, typeof(string), 20, errors, campoInterno: nameof(empresa.SubdivisionId));
            ValidarCampo(oldModel, empresa, camposInmutables, "Municipio", empresa.MunicipioId, false, typeof(string), 20, errors, campoInterno: nameof(empresa.MunicipioId));

            return true;
        }

        public virtual bool EmpresaValidacionProcedimiento(Empresa empresa, IEmpresaServiceContext context, List<Error> errors)
        {
            ValidarSituacion(empresa.EstadoId, errors);
            ValidarLocalidad(null, empresa, context.ExistePais, t => context.ExisteLocalidad(t.Item1, t.Item2), context.GetSubdivision, t => context.GetLocalidadId(t.Item1, t.Item2), errors);

            return true;
        }
        #endregion

        #region Lpn

        public virtual bool LpnValidacion(IUnitOfWork uow, Lpn lpn, ILpnServiceContext context, out List<Error> errors, out bool errorProcedimiento)
        {
            errors = new List<Error>();
            errorProcedimiento = true;
            var keysAtributo = new HashSet<string>();
            var keysBarras = new HashSet<string>();
            var keysDetalles = new HashSet<string>();
            var keysAtributoDetalles = new HashSet<string>();

            ValidarCampo("IdExterno", lpn.IdExterno, true, typeof(string), 50, errors);
            ValidarCampo("Tipo", lpn.Tipo, true, typeof(string), 10, errors);
            ValidarCampo("IdPacking", lpn.IdPacking, false, typeof(string), 50, errors);

            if (errors.Any())
                return false;

            var tipoLpn = context.GetTipoLpn(lpn.Tipo);
            if (tipoLpn == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoLpnNoExiste", lpn.Tipo));
                return false;
            }
            else if ((tipoLpn.MultiProducto == "N" || tipoLpn.MultiLote == "N") && lpn.Detalles != null && lpn.Detalles.Count > 0)
            {
                var qtProductos = lpn.Detalles.Select(d => d.CodigoProducto).Distinct().Count();
                var qtLotes = lpn.Detalles.Select(d => d.Lote).Distinct().Count();

                if (tipoLpn.MultiProducto == "N" && qtProductos > 1)
                    errors.Add(new Error("WMSAPI_msg_Error_TipoLpnNOMultiProducto", lpn.Tipo));
                else if (tipoLpn.MultiLote == "N" && qtLotes > 1)
                    errors.Add(new Error("WMSAPI_msg_Error_TipoLpnNOMultiLote", lpn.Tipo));
            }

            if (context.ExisteLpnActivo(lpn.IdExterno, lpn.Tipo))
            {
                errors.Add(new Error("WMSAPI_msg_Error_LpnExistente", new string[] { lpn.IdExterno, lpn.Tipo }));
                return false;
            }

            if (lpn.BarrasSinDefinir != null)
            {
                foreach (var b in lpn.BarrasSinDefinir)
                {
                    ValidarCampo("CodigoBarras", b.CodigoBarras, true, typeof(string), 100, errors);
                    ValidarCampo("Tipo", b.Tipo, false, typeof(string), 2, errors);

                    if (!context.ExisteTipoBarra(b.Tipo))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_TipoBarraNoExiste", b.Tipo));
                        return false;
                    }
                    else if (context.ExisteLpnBarraActivo(b.CodigoBarras))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_LpnBarraExistente", b.CodigoBarras, context.Empresa.ToString()));
                        return false;
                    }

                    if (keysBarras.Contains(b.CodigoBarras))
                        errors.Add(new Error("WMSAPI_msg_Error_LpnBarraDuplicado", b.CodigoBarras));
                    else
                        keysBarras.Add(b.CodigoBarras);

                    if (b.CodigoBarras.StartsWith("WIS", StringComparison.OrdinalIgnoreCase))
                        errors.Add(new Error("WMSAPI_msg_Error_LpnBarraSinPrefijoWIS", b.CodigoBarras));
                }
            }

            if (errors.Any())
                return false;

            if (lpn.AtributosSinDefinir == null || lpn.AtributosSinDefinir.Count == 0)
                PreCargarAtributosCabezal(lpn, context);

            if (lpn.AtributosSinDefinir != null)
            {
                foreach (var a in lpn.AtributosSinDefinir)
                {
                    ValidarAtributo(uow, context, lpn.Tipo, a, errors);

                    if (keysAtributo.Contains(a.Nombre))
                        errors.Add(new Error("WMSAPI_msg_Error_LpnAtributoDuplicado", a.Nombre));
                    else
                        keysAtributo.Add(a.Nombre);
                }

                var idsAtributos = lpn.AtributosSinDefinir.Select(a => a.Nombre).ToList();
                if (context.ExistenAtributosFaltantes(lpn.Tipo, idsAtributos))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AtributosFaltantes", lpn.Tipo));
                    return false;
                }
            }

            if (errors.Any())
                return false;

            if (lpn.Detalles != null)
            {
                var anyTpLpnAtributosDet = context.AnyTipoLpnAtributoDet(lpn.Tipo);
                foreach (var det in lpn.Detalles)
                {
                    keysAtributoDetalles = new HashSet<string>();
                    ValidarCampo("IdLineaSistemaExterno", det.IdLineaSistemaExterno, true, typeof(string), 40, errors);
                    ValidarCampo("CodigoProducto", det.CodigoProducto, true, typeof(string), 40, errors);
                    ValidarCampo("CantidadDeclarada", det.CantidadDeclarada.ToString(), true, typeof(decimal), 12, errors, 3);

                    ValidarCampo("FechaVencimiento", det.Vencimiento?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);

                    var prod = context.GetProducto(det.Empresa, det.CodigoProducto);
                    if (prod == null)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", new string[] { det.CodigoProducto, det.Empresa.ToString() }));
                        return false;
                    }
                    else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", det.CodigoProducto, det.Empresa.ToString()));
                        return false;
                    }
                    else if (!prod.AceptaDecimales)
                    {
                        if (!int.TryParse(det.CantidadDeclarada?.ToString(), out int value) && ((det.CantidadDeclarada % 1) != 0))
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", det.CodigoProducto));
                            return false;
                        }
                    }

                    if (string.IsNullOrEmpty(det.Lote))
                    {
                        if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                            det.Lote = ManejoIdentificadorDb.IdentificadorProducto;
                        else
                            ValidarCampo("Identificador", det.Lote, true, typeof(string), 40, errors);
                    }
                    else if (prod.ManejoIdentificador == ManejoIdentificador.Producto && det.Lote != ManejoIdentificadorDb.IdentificadorProducto)
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", det.CodigoProducto));
                    else if (prod.ManejoIdentificador != ManejoIdentificador.Producto && det.Lote == ManejoIdentificadorDb.IdentificadorProducto)
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", det.CodigoProducto));

                    if (prod.TipoManejoFecha == ManejoFechaProductoDb.Expirable && det.Vencimiento == null)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaVencimiento", prod.Codigo));
                        return false;
                    }
                    else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Fifo)
                        det.Vencimiento = DateTime.Now;
                    else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Duradero)
                        det.Vencimiento = null;

                    ValidarFechaMenorA("FechaVencimiento", det.Vencimiento?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

                    if (prod.ManejoIdentificador == ManejoIdentificador.Serie && det.Lote != ManejoIdentificadorDb.IdentificadorAuto && det.CantidadDeclarada != 1)
                        errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));

                    if (errors.Any())
                        return false;

                    if (!prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(det.Lote, context.GetCaracteresNoPermitidosIdentificador()))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                        return false;
                    }

                    if (errors.Any())
                        return false;

                    if (det.AtributosSinDefinir != null)
                    {
                        foreach (var ad in det.AtributosSinDefinir)
                        {
                            ValidarAtributo(uow, context, lpn.Tipo, ad, errors, true);

                            if (keysAtributoDetalles.Contains(ad.Nombre))
                                errors.Add(new Error("WMSAPI_msg_Error_LpnAtributoDetalleDuplicado", ad.Nombre));
                            else
                                keysAtributoDetalles.Add(ad.Nombre);
                        }

                        var idsAtributos = det.AtributosSinDefinir.Select(a => a.Nombre).ToList();
                        if (context.ExistenAtributosDetFaltantes(lpn.Tipo, idsAtributos))
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_AtributosDetalleFaltantes", lpn.Tipo));
                            return false;
                        }
                    }

                    string keyDetalle = $"{det.IdLineaSistemaExterno}.{det.CodigoProducto}.{det.Lote}";
                    var error = new Error("WMSAPI_msg_Error_LpnDetalleProductoLoteDuplicado", det.IdLineaSistemaExterno, det.CodigoProducto, det.Lote);
                    if (!anyTpLpnAtributosDet)
                    {
                        keyDetalle = $"{det.CodigoProducto}.{det.Lote}";
                        error = new Error("WMSAPI_msg_Error_LpnProductoLoteDuplicado", det.CodigoProducto, det.Lote);
                    }

                    if (keysDetalles.Contains(keyDetalle))
                        errors.Add(error);
                    else
                        keysDetalles.Add(keyDetalle);
                }
            }

            return true;
        }

        public virtual void PreCargarAtributosCabezal(Lpn lpn, ILpnServiceContext context)
        {
            var lpnFinalizado = context.GetUltimoLpn(lpn.IdExterno, lpn.Tipo);

            if (lpnFinalizado != null)
            {
                var atributos = context.GetAtributosCabezal(lpnFinalizado.NumeroLPN);
                lpn.AtributosSinDefinir = new List<AtributoValor>();

                foreach (var a in atributos)
                {
                    lpn.AtributosSinDefinir.Add(new AtributoValor()
                    {
                        Nombre = a.Nombre,
                        Valor = a.Valor,
                    });
                }
            }
        }

        public virtual bool ValidarAtributo(IUnitOfWork uow, ILpnServiceContext context, string tpLpn, AtributoValor atr, List<Error> errors, bool detalle = false)
        {
            if (string.IsNullOrEmpty(atr?.Nombre))
            {
                errors.Add(new Error("WMSAPI_msg_Error_NombreAtributoRequerido"));
                return false;
            }

            var atributo = context.GetAtributo(atr.Nombre);

            if (atributo == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AtributoNoExisteNoAsociado", new string[] { atr.Nombre, tpLpn }));
                return false;
            }
            else
            {
                bool atributoAsociado;
                if (detalle)
                    atributoAsociado = context.ExisteTipoLpnAtributoDet(tpLpn, atr.Nombre);
                else
                    atributoAsociado = context.ExisteTipoLpnAtributo(tpLpn, atr.Nombre);

                if (!atributoAsociado)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AtributoNoExisteNoAsociado", new string[] { atr.Nombre, tpLpn }));
                    return false;
                }

                AtributoValidation.ValidarFormatoTipo(context, atributo, atr.Valor, _culture, errors);
                AtributoValidation.ValidacionAsociadaAtributos(uow, context, atributo, atr.Valor, _culture, invocarAPICustom: true, errors);

                if (errors.Any())
                    return false;

                atr.Valor = AtributoHelper.GetValorByIdTipo(uow, atributo, atr.Valor, _culture);
            }

            return true;
        }

        #endregion

        #region Pedido

        public virtual bool PedidoValidacionCarga(Pedido pedido, IPedidoServiceContext context, List<Error> errors)
        {
            var detProductos = new HashSet<string>();

            ValidarCampo("NroPedido", pedido.Id, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", pedido.TipoAgente, true, typeof(string), 3, errors);
            ValidarCampo("CodigoAgente", pedido.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("Ruta", pedido.Ruta?.ToString(), true, typeof(short), 3, errors, distintoCero: false);
            ValidarCampo("TipoExpedicion", pedido.TipoExpedicionId, true, typeof(string), 6, errors);
            ValidarCampo("TipoPedido", pedido.Tipo, true, typeof(string), 6, errors);

            ValidarCampo("Predio", pedido.Predio, false, typeof(string), 10, errors);
            ValidarCampo("CondicionLiberacion", pedido.CondicionLiberacion, true, typeof(string), 6, errors);
            ValidarCampo("PuntoEntrega", pedido.PuntoEntrega, false, typeof(string), 20, errors);
            ValidarCampo("CodigoTransportadora", pedido.CodigoTransportadora?.ToString(), false, typeof(int), 10, errors);
            ValidarCampo("Zona", pedido.Zona, false, typeof(string), 20, errors);
            ValidarCampo("Anexo1", pedido.Anexo, false, typeof(string), 200, errors);
            ValidarCampo("Anexo2", pedido.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo("Anexo3", pedido.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo("Anexo4", pedido.Anexo4, false, typeof(string), 200, errors);
            ValidarCampo("Direccion", pedido.DireccionEntrega, false, typeof(string), 400, errors);
            ValidarCampo("Memo", pedido.Memo, false, typeof(string), 1000, errors);
            ValidarCampo("Memo1", pedido.Memo1, false, typeof(string), 1000, errors);
            ValidarCampo("FechaEmision", pedido.FechaEmision?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaEntrega", pedido.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaLiberarDesde", pedido.FechaLiberarDesde?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaLiberarHasta", pedido.FechaLiberarHasta?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaGenerica", pedido.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("Agrupacion", pedido.Agrupacion, false, typeof(string), 1, errors);
            ValidarCampo("NuGenerico", pedido.NuGenerico_1?.ToString(), false, typeof(decimal), 15, errors, 3);
            ValidarCampo("OrdenEntrega", pedido.OrdenEntrega?.ToString(), false, typeof(int), 6, errors);
            ValidarCampo("ComparteContenedorEntrega", pedido.ComparteContenedorEntrega, false, typeof(string), 200, errors);
            ValidarCampo("ComparteContenedorPicking", pedido.ComparteContenedorPicking, false, typeof(string), 200, errors);
            ValidarCampo("DsGenerico", pedido.VlGenerico_1, false, typeof(string), 400, errors);
            ValidarCampo("Serializado", pedido.VlSerealizado_1, false, typeof(string), 4000, errors);
            ValidarCampo("Telefono", pedido.Telefono, false, typeof(string), 30, errors);
            ValidarCampo("TelefonoSecundario", pedido.TelefonoSecundario, false, typeof(string), 30, errors);
            ValidarCampo("Latitud", pedido.Latitud?.ToString(), false, typeof(decimal), 9, errors, 7, false, false);
            ValidarCampo("Longitud", pedido.Longitud?.ToString(), false, typeof(decimal), 10, errors, 7, false, false);

            ValidarDecimalEntre("Latitud", pedido.Latitud, -90, 90, errors);
            ValidarDecimalEntre("Longitud", pedido.Longitud, -180, 180, errors);

            if (pedido.Lpns.Count() == 0 && pedido.Lineas.Count() == 0)
                errors.Add(new Error("WMSAPI_msg_Error_PedidoSinDetalle", pedido.Id, pedido.CodigoAgente, pedido.TipoAgente, pedido.Empresa));

            var detalleLpnHabilitado = (context.GetParametro(ParamManager.IE_503_HAB_LPN) ?? "N") == "S";
            var detalleAtributosHabilitado = (context.GetParametro(ParamManager.IE_503_HAB_ATRIBUTOS) ?? "N") == "S";

            if (pedido.Lpns.Count > 0 && !detalleLpnHabilitado)
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoUsoLpn", pedido.Empresa));
                return false;
            }
            else
            {
                foreach (var lpn in pedido.Lpns)
                {
                    ValidarCampo("IdLpnExterno", lpn.IdExterno, true, typeof(string), 50, errors);
                    ValidarCampo("TipoLpn", lpn.Tipo, true, typeof(string), 10, errors, 3);
                }
            }

            if (errors.Any())
                return false;

            foreach (var detalle in pedido.Lineas)
            {
                ValidarCampo("CodigoProducto", detalle.Producto, true, typeof(string), 40, errors);
                ValidarCampo("Cantidad", detalle.Cantidad?.ToString(), true, typeof(decimal), 12, errors, 3);

                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);
                ValidarCampo("Memo", detalle.Memo, false, typeof(string), 200, errors);
                ValidarCampo("FechaGenerica", detalle.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
                ValidarCampo("NuGenerico", detalle.NuGenerico_1?.ToString(), false, typeof(decimal), 15, errors, 3);
                ValidarCampo("DsGenerico", detalle.VlGenerico_1, false, typeof(string), 400, errors);
                ValidarCampo("PorcentajeTolerancia", detalle.PorcentajeTolerancia?.ToString(), false, typeof(decimal), 10, errors, 2, false);
                ValidarCampo("Serializado", detalle.DatosSerializados, false, typeof(string), 4000, errors);

                var identificador = string.Empty;
                if (!string.IsNullOrEmpty(detalle.Identificador))
                    identificador = detalle.Identificador;

                var keyDetalle = $"{detalle.Producto}.{identificador}";
                if (detProductos.Contains(keyDetalle))
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoDetalleDuplicados", pedido.Id, detalle.Producto, identificador));
                else
                    detProductos.Add(keyDetalle);

                if (errors.Any())
                    return false;

                if (!ValidarDuplicadosDetallePedido(pedido, detalle, identificador, context, errors))
                    return false;

                if (!ValidarDetallesLpnDetallePedido(pedido, detalle, identificador, detalleLpnHabilitado, detalleAtributosHabilitado, errors))
                    return false;

                if (!ValidarAtributosDetallePedido(pedido, detalle, identificador, detalleAtributosHabilitado, errors))
                    return false;
            }

            return true;
        }

        public virtual bool ValidarDuplicadosDetallePedido(Pedido pedido, DetallePedido detalle, string identificador, IServiceContext context, List<Error> errors)
        {
            var duplicadosHabilitado = context.GetParametro(ParamManager.IE_503_HAB_DUPLICADOS) ?? "N";

            if (detalle.Duplicados.Count > 0 && duplicadosHabilitado == "N")
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoUsoDuplicados", pedido.Empresa));
                return false;
            }
            else
            {
                var dupDetalles = new HashSet<string>();
                decimal cantidadPedida = 0;

                foreach (var duplicado in detalle.Duplicados)
                {
                    ValidarCampo("IdLineaSistemaExterno", duplicado.IdLineaSistemaExterno, false, typeof(string), 40, errors);
                    ValidarCampo("TipoLinea", duplicado.TipoLinea, false, typeof(string), 200, errors);
                    ValidarCampo("CantidadPedida", duplicado.CantidadPedida.ToString(), true, typeof(decimal), 12, errors, 3, false);

                    cantidadPedida += duplicado.CantidadPedida;

                    var keyDuplicado = $"{duplicado.IdLineaSistemaExterno}";

                    if (dupDetalles.Contains(keyDuplicado))
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoLineasDuplicadas", pedido.Id, detalle.Producto, identificador, duplicado.IdLineaSistemaExterno));
                    else
                        dupDetalles.Add(keyDuplicado);

                    if (errors.Any())
                        return false;
                }

                if (detalle.Duplicados.Count > 0 && cantidadPedida != detalle.Cantidad)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoCantidadNoCoincide", pedido.Id, detalle.Producto, identificador));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ValidarDetallesLpnDetallePedido(Pedido pedido, DetallePedido detalle, string identificador, bool detalleLpnHabilitado, bool detalleAtributosHabilitado, List<Error> errors)
        {
            if (detalle.DetallesLpn.Count > 0 && !detalleLpnHabilitado)
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoUsoLpn", pedido.Empresa));
                return false;
            }
            else
            {
                var lpnDetalles = new HashSet<string>();
                decimal cantidadPedida = 0;

                foreach (var detalleLpn in detalle.DetallesLpn)
                {
                    ValidarCampo("IdLpnExterno", detalleLpn.IdLpnExterno, true, typeof(string), 40, errors);
                    ValidarCampo("TipoLpn", detalleLpn.Tipo, true, typeof(string), 200, errors);
                    ValidarCampo("CantidadPedida", detalleLpn.CantidadPedida.ToString(), true, typeof(decimal), 12, errors, 3, false);

                    cantidadPedida += (detalleLpn.CantidadPedida ?? 0);

                    var keyDetalleLpn = $"{detalleLpn.IdLpnExterno}.{detalleLpn.Identificador}";
                    if (lpnDetalles.Contains(keyDetalleLpn))
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoLpnLineasDuplicadas", pedido.Id, detalle.Producto, identificador, detalleLpn.IdLpnExterno, detalleLpn.Tipo));
                    else
                        lpnDetalles.Add(keyDetalleLpn);

                    if (!ValidarAtributosDetalleLpnDetallePedido(pedido, detalle, detalleLpn, identificador, detalleAtributosHabilitado, errors))
                        return false;

                    if (errors.Any())
                        return false;
                }

                if (detalle.DetallesLpn.Count > 0 && cantidadPedida > detalle.Cantidad)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoCantidadMenorALaSumaDeDetalleLpn", pedido.Id, detalle.Producto, identificador));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ValidarAtributosDetallePedido(Pedido pedido, DetallePedido detalle, string identificador, bool detalleAtributosHabilitado, List<Error> errors)
        {
            if (detalle.Atributos.Count > 0 && !detalleAtributosHabilitado)
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoUsoAtributos", pedido.Empresa));
                return false;
            }
            else
            {
                var confDetalles = new HashSet<string>();
                decimal cantidadPedida = 0;

                foreach (var configuracion in detalle.Atributos)
                {
                    ValidarCampo("CantidadPedida", configuracion.CantidadPedida.ToString(), true, typeof(decimal), 12, errors, 3, false);

                    cantidadPedida += configuracion.CantidadPedida;

                    if (configuracion.Atributos.Count == 0)
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoConfiguracionAtributosVacia", pedido.Id, detalle.Producto, identificador));

                    var keyConfiguracion = "";
                    var atrDetalles = new HashSet<string>();
                    var atributos = configuracion.Atributos.OrderBy(a => a.IdCabezal).ThenBy(a => a.Nombre);

                    foreach (var atributo in atributos)
                    {
                        var keyAtributo = $"{atributo.IdCabezal}.{atributo.Nombre}";

                        ValidarCampo("Tipo", atributo.Tipo, true, typeof(string), 1, errors);
                        ValidarCampo("Nombre", atributo.Nombre, true, typeof(string), 50, errors);
                        ValidarCampo("Valor", atributo.Valor, true, typeof(string), 400, errors);

                        if (atrDetalles.Contains(keyAtributo))
                            errors.Add(new Error("WMSAPI_msg_Error_PedidoAtributosDuplicados", pedido.Id, detalle.Producto, identificador, atributo.Nombre, atributo.IdCabezal));
                        else
                            atrDetalles.Add(keyAtributo);

                        keyConfiguracion += $"{keyAtributo};{atributo.Valor}";
                    }

                    var jsonConfiguracion = JsonConvert.SerializeObject(new
                    {
                        Cabezal = atributos.Where(a => a.IdCabezal == "S").Select(a => a.Nombre),
                        Detalles = atributos.Where(a => a.IdCabezal == "N").Select(a => a.Nombre),
                    });

                    if (confDetalles.Contains(keyConfiguracion))
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoConfiguracionesAtributosDuplicadas", pedido.Id, detalle.Producto, identificador, jsonConfiguracion));
                    else
                        confDetalles.Add(keyConfiguracion);

                    if (errors.Any())
                        return false;
                }

                if (detalle.Atributos.Count > 0 && cantidadPedida > detalle.Cantidad)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoCantidadMenorALaSumaDeDetalleAtributos", pedido.Id, detalle.Producto, identificador));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ValidarAtributosDetalleLpnDetallePedido(Pedido pedido, DetallePedido detalle, DetallePedidoLpn detalleLpn, string identificador, bool detalleAtributosHabilitado, List<Error> errors)
        {
            if (detalleLpn.Atributos.Count > 0 && !detalleAtributosHabilitado)
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoUsoAtributos", pedido.Empresa));
                return false;
            }
            else
            {
                var confDetallesLpn = new HashSet<string>();
                decimal cantidadPedida = 0;

                foreach (var configuracion in detalleLpn.Atributos)
                {
                    ValidarCampo("CantidadPedida", configuracion.CantidadPedida.ToString(), true, typeof(decimal), 12, errors, 3, false);

                    cantidadPedida += configuracion.CantidadPedida;

                    if (configuracion.Atributos.Count == 0)
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoConfiguracionAtributosLpnVacia", pedido.Id, detalle.Producto, identificador, detalleLpn.IdLpnExterno, detalleLpn.Tipo));

                    var keyConfiguracion = "";
                    var atrDetalles = new HashSet<string>();
                    var atributos = configuracion.Atributos.OrderBy(a => a.Nombre);

                    foreach (var atributo in atributos)
                    {
                        var keyAtributo = $"{atributo.Nombre}";

                        ValidarCampo("Tipo", atributo.Tipo, true, typeof(string), 1, errors);
                        ValidarCampo("Nombre", atributo.Nombre, true, typeof(string), 50, errors);
                        ValidarCampo("Valor", atributo.Valor, true, typeof(string), 400, errors);

                        if (atrDetalles.Contains(keyAtributo))
                            errors.Add(new Error("WMSAPI_msg_Error_PedidoAtributosLpnDuplicados", pedido.Id, detalle.Producto, identificador, detalleLpn.IdLpnExterno, detalleLpn.Tipo, atributo.Nombre, atributo.IdCabezal));
                        else
                            atrDetalles.Add(keyAtributo);

                        keyConfiguracion += $"{keyAtributo}.{atributo.Valor};";
                    }

                    var jsonConfiguracion = JsonConvert.SerializeObject(atributos.Select(a => a.Nombre));

                    if (confDetallesLpn.Contains(keyConfiguracion))
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoConfiguracionesAtributosLpnDuplicadas", pedido.Id, detalle.Producto, identificador, detalleLpn.IdLpnExterno, detalleLpn.Tipo, jsonConfiguracion));
                    else
                        confDetallesLpn.Add(keyConfiguracion);

                    if (errors.Any())
                        return false;
                }

                if (detalleLpn.Atributos.Count > 0 && cantidadPedida > detalle.Cantidad)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoCantidadMenorALaSumaDeDetalleLpnAtributos", pedido.Id, detalle.Producto, identificador, detalleLpn.IdLpnExterno, detalleLpn.Tipo));
                    return false;
                }
            }

            return true;
        }

        public virtual bool PedidoValidacionProcedimiento(Pedido pedido, IPedidoServiceContext context, List<Error> errors)
        {
            if (!ValidarAgente(pedido.CodigoAgente, pedido.Empresa, pedido.TipoAgente,
                (t => context.GetAgente(t.Item1, t.Item2, t.Item3)), errors, out string cdCliente))
                return false;

            pedido.Cliente = cdCliente;

            if (!pedido.Id.IsUpper())
            {
                AddError(errors, "WMSAPI_msg_Error_PedidoCaracteresMinuscula");
                return false;
            }

            if (context.ExistePedido(pedido.Id, pedido.Empresa, cdCliente))
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoYaExiste", pedido.Id, pedido.TipoAgente, pedido.CodigoAgente, pedido.Empresa));
                return false;
            }

            if (!ValidarTipoExpedicionYPedido(pedido, context, errors))
                return false;

            if (!string.IsNullOrEmpty(pedido.Predio))
                ValidarPredio(pedido.Predio, context.ExistePredio, errors);

            ValidarAgrupacion(pedido.Agrupacion, errors);
            ValidarRuta((short?)pedido.Ruta ?? -1, context.ExisteRuta, errors);
            ValidarCondicionLiberacion(pedido.CondicionLiberacion, context, errors);
            ValidarTransportista(pedido.CodigoTransportadora, context.ExisteTransportista, errors);
            ValidarZona(pedido.Zona, context.ExisteZona, errors);

            var validarFechas = (context.GetParametro(ParamManager.IE_503_VALIDAR_FECHAS) ?? "N") == "S";
            if (validarFechas)
            {
                ValidarFechaMayorA("FechaEmision", pedido.FechaEmision?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
                ValidarFechaMenorA("FechaEntrega", pedido.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), pedido.FechaEmision.ToString(CDateFormats.DATE_ONLY), "Emitida", errors);
                ValidarFechaMenorA("FechaLiberarDesde", pedido.FechaLiberarDesde?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
                ValidarFechaMenorA("FechaLiberarHasta", pedido.FechaLiberarHasta?.ToString(CDateFormats.DATE_ONLY), pedido.FechaLiberarDesde.ToString(CDateFormats.DATE_ONLY), "LiberarDesde", errors);
                ValidarFechaMenorA("FechaGenerica", pedido.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
            }

            if (errors.Any())
                return false;

            foreach (var lpn in pedido.Lpns)
            {
                if (!context.ExisteLpn(lpn.IdExterno, lpn.Tipo))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_LpnNoEstaRegistrado", lpn.IdExterno, lpn.Tipo));
                    return false;
                }
            }

            foreach (var detalle in pedido.Lineas)
            {
                detalle.Cliente = pedido.Cliente;

                var prod = context.GetProducto(pedido.Empresa, detalle.Producto);
                if (prod == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", detalle.Producto, pedido.Empresa));
                    return false;
                }
                else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", detalle.Producto, detalle.Empresa));
                    return false;
                }
                else if (!prod.AceptaDecimales)
                {
                    if (detalle.Cantidad.HasValue && detalle.Cantidad.Value != Math.Truncate(detalle.Cantidad.Value))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(detalle.Identificador))
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                    {
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                        detalle.EspecificaIdentificadorId = "S";
                    }
                    else
                    {
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorAuto;
                        detalle.EspecificaIdentificadorId = "N";
                    }
                }
                else if (prod.ManejoIdentificador == ManejoIdentificador.Producto && detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.Producto));

                if (validarFechas)
                    ValidarFechaMenorA("FechaGenerica", detalle.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

                if (detalle.Identificador == ManejoIdentificadorDb.IdentificadorAuto)
                    detalle.EspecificaIdentificador = false;
                else
                    detalle.EspecificaIdentificador = true;

                if (prod.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                {
                    if (detalle.Cantidad != 1)
                    {
                        errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                        return false;
                    }
                    else if (detalle.Duplicados.Count > 0)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoSerieNoPermiteDuplicados"));
                        return false;
                    }
                }

                if (!prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                    return false;
                }

                foreach (var duplicado in detalle.Duplicados)
                {
                    duplicado.Cliente = detalle.Cliente;
                    duplicado.Identificador = detalle.Identificador;
                    duplicado.IdEspecificaIdentificador = detalle.EspecificaIdentificadorId;

                    if (!prod.AceptaDecimales && duplicado.CantidadPedida != Math.Truncate(duplicado.CantidadPedida))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                        return false;
                    }
                }

                foreach (var detalleLpn in detalle.DetallesLpn)
                {
                    if (pedido.Lpns.Any(x => x.IdExterno == detalleLpn.IdLpnExterno && x.Tipo == detalleLpn.Tipo))
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoDeLpnFueEspecificadoANivelDeCabezal", detalle.Producto, detalleLpn.IdLpnExterno, detalleLpn.Tipo));

                    if (!prod.AceptaDecimales && detalleLpn.CantidadPedida.Value != Math.Truncate(detalleLpn.CantidadPedida.Value))
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));

                    var lpn = context.GetLpn(detalleLpn.IdLpnExterno, detalleLpn.Tipo);

                    detalleLpn.NumeroLpn = lpn?.NumeroLPN ?? detalleLpn.NumeroLpn;
                    detalleLpn.Cliente = detalle.Cliente;
                    detalleLpn.Identificador = detalle.Identificador;
                    detalleLpn.IdEspecificaIdentificador = detalle.EspecificaIdentificadorId;

                    foreach (var configuracion in detalleLpn.Atributos)
                    {
                        configuracion.Cliente = cdCliente;
                        configuracion.Identificador = detalleLpn.Identificador;
                        configuracion.IdEspecificaIdentificador = detalleLpn.IdEspecificaIdentificador;

                        foreach (var atributo in configuracion.Atributos)
                        {
                            if (!context.ExisteAtributoDetalleLpn(detalleLpn.IdLpnExterno, detalleLpn.Tipo, detalleLpn.Empresa, detalleLpn.Producto, detalleLpn.Faixa, detalleLpn.Identificador, atributo.Nombre))
                                errors.Add(new Error("WMSAPI_msg_Error_AtributoLpnNoExiste", atributo.Nombre, detalleLpn.IdLpnExterno, detalleLpn.Tipo, detalleLpn.Producto, detalleLpn.Identificador));
                        }
                    }
                }

                foreach (var configuracion in detalle.Atributos)
                {
                    configuracion.Cliente = cdCliente;
                    configuracion.Identificador = detalle.Identificador;
                    configuracion.IdEspecificaIdentificador = detalle.EspecificaIdentificadorId;

                    foreach (var atributo in configuracion.Atributos)
                    {
                        ValidarCoD("Tipo", atributo.Tipo, errors);

                        if (!context.ExisteAtributo(atributo.Nombre))
                            errors.Add(new Error("WMSAPI_msg_Error_AtributoNoExiste", atributo.Nombre));
                    }
                }
            }

            return true;
        }

        public virtual bool ValidarTipoExpedicionYPedido(Pedido pedido, IPedidoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteTpExpedicion(pedido.TipoExpedicionId))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoExpedicionNoExiste", pedido.TipoExpedicionId));
                return false;
            }
            else if (!context.ExisteTpPedido(pedido.Tipo))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoPedidoNoExiste", pedido.Tipo));
                return false;
            }
            else if (!context.ExisteRelTpExpdicionPedido(pedido.TipoExpedicionId, pedido.Tipo))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TpExpIncompatibleTpPedido", pedido.TipoExpedicionId, pedido.Tipo));
                return false;
            }

            return true;
        }

        public virtual bool ValidarCondicionLiberacion(string codigo, IPedidoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteCondicionLiberacion(codigo))
            {
                errors.Add(new Error("WMSAPI_msg_Error_CondicionLiberacionNoExiste", codigo));
                return false;
            }
            return true;
        }

        public virtual bool ModificarPedidoValidacionCarga(Pedido pedido, IModificarPedidoServiceContext context, List<Error> errors)
        {
            var detProductos = new HashSet<string>();

            ValidarCampo("NroPedido", pedido.Id, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", pedido.TipoAgente, true, typeof(string), 3, errors);
            ValidarCampo("CodigoAgente", pedido.CodigoAgente, true, typeof(string), 40, errors);

            ValidarCampo("Ruta", pedido.Ruta?.ToString(), false, typeof(short), 3, errors, distintoCero: false);
            ValidarCampo("Predio", pedido.Predio, false, typeof(string), 10, errors);
            ValidarCampo("TipoExpedicion", pedido.TipoExpedicionId, false, typeof(string), 6, errors);
            ValidarCampo("TipoPedido", pedido.Tipo, false, typeof(string), 6, errors);

            ValidarCampo("CondicionLiberacion", pedido.CondicionLiberacion, true, typeof(string), 6, errors);
            ValidarCampo("PuntoEntrega", pedido.PuntoEntrega, false, typeof(string), 20, errors);
            ValidarCampo("CodigoTransportadora", pedido.CodigoTransportadora?.ToString(), false, typeof(int), 10, errors);
            ValidarCampo("Zona", pedido.Zona, false, typeof(string), 20, errors);
            ValidarCampo("Anexo1", pedido.Anexo, false, typeof(string), 200, errors);
            ValidarCampo("Anexo2", pedido.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo("Anexo3", pedido.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo("Anexo4", pedido.Anexo4, false, typeof(string), 200, errors);
            ValidarCampo("Direccion", pedido.DireccionEntrega, false, typeof(string), 400, errors);
            ValidarCampo("Memo", pedido.Memo, false, typeof(string), 1000, errors);
            ValidarCampo("Memo1", pedido.Memo1, false, typeof(string), 1000, errors);
            ValidarCampo("FechaEmision", pedido.FechaEmision?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaEntrega", pedido.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaLiberarDesde", pedido.FechaLiberarDesde?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaLiberarHasta", pedido.FechaLiberarHasta?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaGenerica", pedido.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("Agrupacion", pedido.Agrupacion, false, typeof(string), 1, errors);
            ValidarCampo("NuGenerico", pedido.NuGenerico_1?.ToString(), false, typeof(decimal), 15, errors, 3);
            ValidarCampo("OrdenEntrega", pedido.OrdenEntrega?.ToString(), false, typeof(int), 6, errors);
            ValidarCampo("ComparteContenedorEntrega", pedido.ComparteContenedorEntrega, false, typeof(string), 200, errors);
            ValidarCampo("ComparteContenedorPicking", pedido.ComparteContenedorPicking, false, typeof(string), 200, errors);
            ValidarCampo("DsGenerico", pedido.VlGenerico_1, false, typeof(string), 400, errors);
            ValidarCampo("Serializado", pedido.VlSerealizado_1, false, typeof(string), 4000, errors);
            ValidarCampo("Telefono", pedido.Telefono, false, typeof(string), 30, errors);
            ValidarCampo("TelefonoSecundario", pedido.TelefonoSecundario, false, typeof(string), 30, errors);
            ValidarCampo("Latitud", pedido.Latitud?.ToString(), false, typeof(decimal), 9, errors, 7, false, false);
            ValidarCampo("Longitud", pedido.Longitud?.ToString(), false, typeof(decimal), 10, errors, 7, false, false);

            ValidarDecimalEntre("Latitud", pedido.Latitud, -90, 90, errors);
            ValidarDecimalEntre("Longitud", pedido.Longitud, -180, 180, errors);

            if (pedido.Lpns.Count() == 0 && pedido.Lineas.Count() == 0)
                errors.Add(new Error("WMSAPI_msg_Error_PedidoSinDetalle", pedido.Id, pedido.CodigoAgente, pedido.TipoAgente, pedido.Empresa));

            var detalleLpnHabilitado = (context.GetParametro(ParamManager.IE_503_HAB_LPN) ?? "N") == "S";
            var detalleAtributosHabilitado = (context.GetParametro(ParamManager.IE_503_HAB_ATRIBUTOS) ?? "N") == "S";

            if (pedido.Lpns.Count > 0 && !detalleLpnHabilitado)
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoUsoLpn", pedido.Empresa));
                return false;
            }
            else
            {
                foreach (var detalle in pedido.Lpns)
                {
                    ValidarCampo("IdLpnExterno", detalle.IdExterno, true, typeof(string), 50, errors);
                    ValidarCampo("TipoLpn", detalle.Tipo, true, typeof(string), 10, errors, 3);
                }
            }

            if (errors.Any())
                return false;

            foreach (var detalle in pedido.Lineas)
            {
                ValidarCampo("CodigoProducto", detalle.Producto, true, typeof(string), 40, errors);
                ValidarCampo("Cantidad", detalle.Cantidad?.ToString(), true, typeof(decimal), 12, errors, 3, false);

                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);
                ValidarCampo("Memo", detalle.Memo, false, typeof(string), 200, errors);
                ValidarCampo("FechaGenerica", detalle.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
                ValidarCampo("NuGenerico", detalle.NuGenerico_1?.ToString(), false, typeof(decimal), 15, errors, 3);
                ValidarCampo("DsGenerico", detalle.VlGenerico_1, false, typeof(string), 400, errors);
                ValidarCampo("PorcentajeTolerancia", detalle.PorcentajeTolerancia?.ToString(), false, typeof(decimal), 10, errors, 2, false);
                ValidarCampo("Serializado", detalle.DatosSerializados, false, typeof(string), 4000, errors);

                var identificador = string.Empty;
                if (!string.IsNullOrEmpty(detalle.Identificador))
                    identificador = detalle.Identificador;

                var keyProducto = $"{detalle.Producto}.{identificador}";
                if (detProductos.Contains(keyProducto))
                    errors.Add(new Error("WMSAPI_msg_Error_PedidoDetalleDuplicados", pedido.Id, detalle.Producto, identificador));
                else
                    detProductos.Add(keyProducto);

                if (errors.Any())
                    return false;

                if (!ValidarDuplicadosDetallePedido(pedido, detalle, identificador, context, errors))
                    return false;

                if (!ValidarDetallesLpnDetallePedido(pedido, detalle, identificador, detalleLpnHabilitado, detalleAtributosHabilitado, errors))
                    return false;

                if (!ValidarAtributosDetallePedido(pedido, detalle, identificador, detalleAtributosHabilitado, errors))
                    return false;
            }

            return true;
        }

        public virtual bool ModificarPedidoValidacionProcedimiento(Pedido pedido, IModificarPedidoServiceContext context, List<Error> errors)
        {
            if (!ValidarAgente(pedido.CodigoAgente, pedido.Empresa, pedido.TipoAgente,
                (t => context.GetAgente(t.Item1, t.Item2, t.Item3)), errors, out string cdCliente))
                return false;

            pedido.Cliente = cdCliente;

            if (!pedido.Id.IsUpper())
            {
                AddError(errors, "WMSAPI_msg_Error_PedidoCaracteresMinuscula");
                return false;
            }

            if (!context.ExistePedido(pedido.Id, pedido.Empresa, cdCliente))
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoNoExiste", pedido.Id, pedido.TipoAgente, pedido.CodigoAgente, pedido.Empresa));
                return false;
            }

            if (!context.IsPedidoProduccion(pedido.Id, pedido.Empresa, cdCliente))
            {
                errors.Add(new Error("WMSAPI_msg_Error_PedidoDeProduccion", pedido.Id, pedido.TipoAgente, pedido.CodigoAgente, pedido.Empresa));
                return false;
            }

            if (!string.IsNullOrEmpty(pedido.Tipo) || !string.IsNullOrEmpty(pedido.TipoExpedicionId))
            {
                if (!ValidarTipoExpedicionYPedido(pedido, context, errors))
                    return false;
                else
                {
                    var ped = context.GetPedido(pedido.Id, pedido.Empresa, pedido.Cliente);
                    if (!context.PuedoEditarTipo(pedido.Id, pedido.Empresa, cdCliente) && (pedido.Tipo != ped.Tipo || pedido.TipoExpedicionId != ped.TipoExpedicionId))
                    {
                        AddError(errors, "WMSAPI_msg_Error_NoSePuedeModificarTpPedTpExp");
                        return false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(pedido.Predio))
                ValidarPredio(pedido.Predio, context.ExistePredio, errors);

            ValidarAgrupacion(pedido.Agrupacion, errors);

            if (pedido.Ruta != null)
                ValidarRuta((short?)pedido.Ruta ?? -1, context.ExisteRuta, errors);

            if (!string.IsNullOrEmpty(pedido.CondicionLiberacion))
                ValidarCondicionLiberacion(pedido.CondicionLiberacion, context, errors);

            ValidarTransportadora(pedido.CodigoTransportadora, context, errors);

            ValidarZona(pedido.Zona, context.ExisteZona, errors);

            var validarFechas = (context.GetParametro(ParamManager.IE_503_VALIDAR_FECHAS) ?? "N") == "S";
            if (validarFechas)
            {
                ValidarFechaMayorA("FechaEmision", pedido.FechaEmision?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
                ValidarFechaMenorA("FechaEntrega", pedido.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), pedido.FechaEmision.ToString(CDateFormats.DATE_ONLY), "Emitida", errors);
                ValidarFechaMenorA("FechaLiberarDesde", pedido.FechaLiberarDesde?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
                ValidarFechaMenorA("FechaLiberarHasta", pedido.FechaLiberarHasta?.ToString(CDateFormats.DATE_ONLY), pedido.FechaLiberarDesde.ToString(CDateFormats.DATE_ONLY), "LiberarDesde", errors);
                ValidarFechaMenorA("FechaGenerica", pedido.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
            }

            if (errors.Any())
                return false;

            foreach (var lpn in pedido.Lpns)
            {
                if (!context.ExisteLpn(lpn.IdExterno, lpn.Tipo))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_LpnNoEstaRegistrado", lpn.IdExterno, lpn.Tipo));
                    return false;
                }
                else
                {
                    var detPedidoLpn = context.GetDetallesLpn(pedido);

                    if (detPedidoLpn.Any(d => d.Tipo == lpn.Tipo && d.IdLpnExterno == lpn.IdExterno))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_LpnYaAsociadoPorCabezal", lpn.IdExterno, lpn.Tipo));
                        return false;
                    }
                }
            }

            if (pedido.Lineas != null && pedido.Lineas.Count > 0)
            {
                /*if (context.GetSaldoTotal(pedido.Id, pedido.Empresa, cdCliente) <= 0)
                {
                    AddError(errors, "WMSAPI_msg_Error_PedidoSinSaldo");
                    return false;
                }*/

                foreach (var detalle in pedido.Lineas)
                {
                    detalle.Cliente = pedido.Cliente;

                    var prod = context.GetProducto(pedido.Empresa, detalle.Producto);
                    if (prod == null)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", detalle.Producto, pedido.Empresa));
                        return false;
                    }
                    else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", detalle.Producto, detalle.Empresa));
                        return false;
                    }
                    else if (!prod.AceptaDecimales)
                    {
                        if (detalle.Cantidad.HasValue && detalle.Cantidad.Value != Math.Truncate(detalle.Cantidad.Value))
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                            return false;
                        }
                    }

                    if (string.IsNullOrEmpty(detalle.Identificador))
                    {
                        if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                        {
                            detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                            detalle.EspecificaIdentificadorId = "S";
                        }
                        else
                        {
                            detalle.Identificador = ManejoIdentificadorDb.IdentificadorAuto;
                            detalle.EspecificaIdentificadorId = "N";
                        }
                    }
                    else if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                    {
                        detalle.EspecificaIdentificadorId = "S";
                        if (detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                            errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.Producto));
                    }

                    if (validarFechas)
                        ValidarFechaMenorA("FechaGenerica", detalle.FechaGenerica_1?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

                    if (prod.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                    {
                        if (detalle.Cantidad != 1)
                        {
                            errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                            return false;
                        }
                        else if (detalle.Duplicados.Count > 0)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_ProductoSerieNoPermiteDuplicados"));
                            return false;
                        }
                    }

                    if (errors.Any())
                        return false;

                    var detPedido = context.GetDetallePedido(detalle);
                    if (detPedido != null)
                    {
                        if (detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                        {
                            if (detalle.Cantidad < detPedido.Cantidad)
                            {
                                var cantAux = (detPedido.CantidadLiberada + detPedido.CantidadAnulada) ?? 0;
                                if (detalle.Cantidad < cantAux)
                                {
                                    errors.Add(new Error("WMSAPI_msg_Error_SinSaldoSuficiente", detalle.Producto, detalle.Identificador, pedido.Id, pedido.Empresa, pedido.Cliente));
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            var qtPedidaTotal = detPedido.Cantidad ?? 0;
                            decimal qtPedidaSoloLote = 0;

                            var detAsociados = context.GetDetallesConLoteAsociados(detalle);
                            foreach (var d in detAsociados)
                            {
                                qtPedidaTotal += (d.Cantidad ?? 0);
                                qtPedidaSoloLote += (d.Cantidad ?? 0);
                            }

                            var diff = ((detalle.Cantidad ?? 0) - qtPedidaSoloLote);
                            if (detalle.Cantidad < qtPedidaTotal)
                            {
                                if (diff < 0)
                                {
                                    errors.Add(new Error("WMSAPI_msg_Error_SinSaldoSuficiente", detalle.Producto, detalle.Identificador, pedido.Id, pedido.Empresa, pedido.Cliente));
                                    return false;
                                }
                                else if (diff < ((detPedido.CantidadLiberada ?? 0) + (detPedido.CantidadAnulada ?? 0)))
                                {
                                    errors.Add(new Error("WMSAPI_msg_Error_SinSaldoSuficiente", detalle.Producto, detalle.Identificador, pedido.Id, pedido.Empresa, pedido.Cliente));
                                    return false;
                                }
                            }
                            //Actualizo cantidad con la nueva contemplados asigando a los lotes
                            detalle.Cantidad = diff;
                            detalle.CantidadOriginal = diff;

                        }
                    }
                    else
                    {
                        detPedido = context.GetDetallePedidoNoEspecifico(detalle);

                        if (detPedido != null)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_DetalleNoEspecificoExistente", detalle.Producto, detalle.Identificador, pedido.Id, pedido.Empresa, pedido.Cliente));
                            return false;
                        }

                        if (!prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                            return false;
                        }
                    }

                    //Valido cantidad de items
                    var keysDupAsociados = context.GetKeysDuplicadosAsociados(detalle);
                    if (keysDupAsociados != null)
                    {
                        var cantItem = keysDupAsociados.Count;
                        foreach (var dup in detalle.Duplicados)
                        {
                            var key = $"{dup.Pedido}.{dup.Cliente}.{dup.Empresa}.{dup.Producto}.{dup.Identificador}.{dup.IdEspecificaIdentificador}.{dup.IdLineaSistemaExterno}";
                            if (keysDupAsociados.Contains(key))
                                cantItem--;
                        }

                        if (cantItem > 0)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_DuplicadosFaltantes", detalle.Producto, detalle.Identificador, detalle.EspecificaIdentificadorId));
                            return false;
                        }
                    }

                    foreach (var dup in detalle.Duplicados)
                    {
                        var msg = string.Format("WMSAPI_msg_Error_SinSaldoSuficienteDup", dup.Pedido, dup.Empresa, dup.Cliente, dup.Producto, dup.Identificador, dup.IdEspecificaIdentificador, dup.IdLineaSistemaExterno);
                        var duplicado = context.GetDuplicado(dup);
                        if (duplicado != null)
                        {
                            if (!prod.AceptaDecimales && duplicado.CantidadPedida != Math.Truncate(duplicado.CantidadPedida))
                            {
                                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                                return false;
                            }

                            if (dup.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                            {
                                if (dup.CantidadPedida < duplicado.CantidadPedida)
                                {
                                    var cantExcluir = (duplicado.CantidadFacturada ?? 0) > 0 ? (duplicado.CantidadFacturada ?? 0) : (duplicado.CantidadExpedida ?? 0);
                                    var min = cantExcluir + (duplicado.CantidadAnulada ?? 0);

                                    if (dup.CantidadPedida < min)
                                    {
                                        AddError(errors, msg);
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                decimal qtPedidaSoloLote = 0;
                                var dupLoteAsocaidos = context.GetDuplicadosConLoteAsociados(dup);

                                foreach (var d in dupLoteAsocaidos)
                                {
                                    qtPedidaSoloLote += d.CantidadPedida;
                                }

                                var min = qtPedidaSoloLote + (duplicado.CantidadAnulada ?? 0);
                                if (dup.CantidadPedida < min)
                                {
                                    AddError(errors, msg);
                                    return false;
                                }
                                else
                                    dup.CantidadPedida -= qtPedidaSoloLote;

                            }
                        }
                    }

                    foreach (var detalleLpn in detalle.DetallesLpn)
                    {
                        if (pedido.Lpns.Any(x => x.IdExterno == detalleLpn.IdLpnExterno && x.Tipo == detalleLpn.Tipo))
                            errors.Add(new Error("WMSAPI_msg_Error_ProductoDeLpnFueEspecificadoANivelDeCabezal", detalle.Producto, detalleLpn.IdLpnExterno, detalleLpn.Tipo));

                        if (!prod.AceptaDecimales && detalleLpn.CantidadPedida.Value != Math.Truncate(detalleLpn.CantidadPedida.Value))
                            errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));

                        var lpn = context.GetLpn(detalleLpn.IdLpnExterno, detalleLpn.Tipo);

                        detalleLpn.NumeroLpn = lpn?.NumeroLPN ?? detalleLpn.NumeroLpn;
                        detalleLpn.Cliente = detalle.Cliente;
                        detalleLpn.Identificador = detalle.Identificador;
                        detalleLpn.IdEspecificaIdentificador = detalle.EspecificaIdentificadorId;

                        if (lpn != null)
                        {
                            var detLpn = context.GetDetalleLpn(detalleLpn);
                            if (detLpn != null && (detalleLpn.CantidadPedida.Value < ((detLpn.CantidadLiberada ?? 0) + (detLpn.CantidadLiberada ?? 0))))
                                errors.Add(new Error("WMSAPI_msg_Error_CantidadMayorSaldo"));
                        }

                        foreach (var configuracion in detalleLpn.Atributos)
                        {
                            configuracion.Cliente = cdCliente;
                            configuracion.Identificador = detalleLpn.Identificador;
                            configuracion.IdEspecificaIdentificador = detalleLpn.IdEspecificaIdentificador;

                            foreach (var atributo in configuracion.Atributos)
                            {
                                if (!context.ExisteAtributoDetalleLpn(detalleLpn.IdLpnExterno, detalleLpn.Tipo, detalleLpn.Empresa, detalleLpn.Producto, detalleLpn.Faixa, detalleLpn.Identificador, atributo.Nombre))
                                    errors.Add(new Error("WMSAPI_msg_Error_AtributoLpnNoExiste", atributo.Nombre, detalleLpn.IdLpnExterno, detalleLpn.Tipo, detalleLpn.Producto, detalleLpn.Identificador));
                            }
                        }
                    }

                    foreach (var configuracion in detalle.Atributos)
                    {
                        configuracion.Cliente = cdCliente;
                        configuracion.Identificador = detalle.Identificador;
                        configuracion.IdEspecificaIdentificador = detalle.EspecificaIdentificadorId;

                        foreach (var atributo in configuracion.Atributos)
                        {
                            ValidarCoD("Tipo", atributo.Tipo, errors);

                            if (!context.ExisteAtributo(atributo.Nombre))
                                errors.Add(new Error("WMSAPI_msg_Error_AtributoNoExiste", atributo.Nombre));
                        }
                    }
                }
            }
            else
            {
                if (context.DetallesPedidoLpn.Values.Any(d => (d.CantidadLiberada) > 0 || (d.CantidadAnulada ?? 0) > 0))
                    errors.Add(new Error("WMSAPI_msg_Error_DetallePedidoLpnTrabajado"));
                else
                {
                    var detsAtributosLpn = new List<DetallePedidoAtributosLpn>();
                    foreach (var dets in context.DetallePedidoAtributosLpn.Values)
                    {
                        detsAtributosLpn.AddRange(dets);
                    }

                    if (detsAtributosLpn.Any(d => (d.CantidadLiberada ?? 0) > 0 || (d.CantidadAnulada ?? 0) > 0))
                        errors.Add(new Error("WMSAPI_msg_Error_DetallePedidoLpnTrabajado"));

                }

            }

            return true;
        }

        public virtual bool ValidarTipoExpedicionYPedido(Pedido pedido, IModificarPedidoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteTpExpedicion(pedido.TipoExpedicionId))
            {
                AddError(errors, $"TipoExpedicion: {string.Format("WMSAPI_msg_Error_TipoExpedicionNoExiste", pedido.TipoExpedicionId)}");
                return false;
            }
            else if (!context.ExisteTpPedido(pedido.Tipo))
            {
                AddError(errors, $"TipoPedido: {string.Format("WMSAPI_msg_Error_TipoPedidoNoExiste", pedido.Tipo)}");
                return false;
            }
            else if (!context.ExisteRelTpExpdicionPedido(pedido.TipoExpedicionId, pedido.Tipo))
            {
                AddError(errors, $"TipoPedido: {string.Format("WMSAPI_msg_Error_TpExpIncompatibleTpPedido", pedido.TipoExpedicionId, pedido.Tipo)}");
                return false;
            }

            return true;
        }

        public virtual bool ValidarCondicionLiberacion(string codigo, IModificarPedidoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteCondicionLiberacion(codigo))
            {
                AddError(errors, $"CondicionLiberacion: {string.Format("WMSAPI_msg_Error_CondicionLiberacionNoExiste", codigo)}");
                return false;
            }
            return true;
        }

        public virtual bool ValidarTransportadora(int? cdTransportadora, IModificarPedidoServiceContext context, List<Error> errors)
        {
            if (cdTransportadora != null)
            {
                if (!context.ExisteTransportadora((int)cdTransportadora))
                {
                    AddError(errors, $"CodigoTransportadora: {string.Format("WMSAPI_msg_Error_TransportadoraNoExiste", cdTransportadora)}");
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Picking
        public virtual void PickingValidacionCarga(DetallePreparacion pickeo, IPickingServiceContext context, List<Error> errors)
        {
            ValidarCampo("Preparacion", pickeo.NumeroPreparacion.ToString(), true, typeof(int), 6, errors);
            ValidarCampo("Ubicacion", pickeo.Ubicacion, true, typeof(string), 40, errors);
            ValidarCampo("CodigoProducto", pickeo.Producto, true, typeof(string), 40, errors);
            ValidarCampo("Faixa", pickeo.Faixa.ToString(), true, typeof(decimal), 9, errors, 3, false);
            ValidarCampo("Identificador", pickeo.Lote, false, typeof(string), 40, errors);
            ValidarCampo("Cantidad", pickeo.Cantidad.ToString(), true, typeof(decimal), 12, errors, 3);

            ValidarCampo("IdExternoContenedor", pickeo.Contenedor.IdExterno, true, typeof(string), 50, errors);
            ValidarCampo("TipoContenedor", pickeo.Contenedor.TipoContenedor, true, typeof(string), 10, errors);
            ValidarCampo("UbicacionContenedor", pickeo.Contenedor.Ubicacion, false, typeof(string), 40, errors);

            if (string.IsNullOrEmpty(pickeo.Agrupacion))
                pickeo.Agrupacion = context.GetAgrupacion(pickeo.NumeroPreparacion);

            var pedidoRequerido = false;
            var codigoAgenteRequerido = false;
            var tipoAgenteRequerido = false;
            var cargaRequerida = false;

            switch (pickeo.Agrupacion)
            {
                case Agrupacion.Pedido:
                    pedidoRequerido = true;
                    codigoAgenteRequerido = true;
                    tipoAgenteRequerido = true;
                    break;
                case Agrupacion.Cliente:
                    codigoAgenteRequerido = true;
                    tipoAgenteRequerido = true;
                    break;
                case Agrupacion.Ruta:
                    cargaRequerida = true;
                    break;
                case Agrupacion.Onda:
                    break;
                default:
                    errors.Add(new Error("WMSAPI_msg_Error_PreparacionSinAgrupacion", pickeo.NumeroPreparacion));
                    break;
            }

            ValidarCampo("Agrupacion", pickeo.Agrupacion, false, typeof(string), 1, errors);
            ValidarCampo("Pedido", pickeo.Pedido, pedidoRequerido, typeof(string), 40, errors);
            ValidarCampo("CodigoAgente", pickeo.CodigoAgente, codigoAgenteRequerido, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", pickeo.TipoAgente, tipoAgenteRequerido, typeof(string), 3, errors);
            ValidarCampo("Carga", pickeo.Carga.ToString(), cargaRequerida, typeof(long), 15, errors);
            ValidarCampo("ComparteContenedorPicking", pickeo.ComparteContenedorPicking, false, typeof(string), 200, errors);

        }

        public virtual bool PickingValidacionProcedimiento(DetallePreparacion pickeo, IPickingServiceContext context, List<Error> errors)
        {
            var estadosPermitidos = new List<string>() { EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE, EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO };

            var preparacion = context.GetPreparacion(pickeo.NumeroPreparacion);
            if (preparacion == null)
                errors.Add(new Error("WMSAPI_msg_Error_PreparacionNoExiste", pickeo.NumeroPreparacion));
            else if (preparacion.Tipo != TipoPreparacionDb.Normal)
                errors.Add(new Error("WMSAPI_msg_Error_TipoPreparacionNoHabilitado", preparacion.Tipo));

            ValidarUbicacion(pickeo.Ubicacion, context.ExisteUbicacion, errors);

            if (errors.Any())
                return false;

            if (!string.IsNullOrEmpty(pickeo.Contenedor.Ubicacion))
            {
                var ubic = context.GetUbicacion(pickeo.Contenedor.Ubicacion);
                if (ubic == null)
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionNoExiste", pickeo.Contenedor.Ubicacion));
                else if (ubic.NumeroPredio != preparacion.Predio)
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionPredioIncorrecto", pickeo.Contenedor.Ubicacion, preparacion.Predio));
                else if (ubic.IdUbicacionArea != AreaUbicacionDb.AutomatismoSalida)
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionAreaAutomatismo", AreaUbicacionDb.AutomatismoSalida));
                else if (ubic.IdUbicacionTipo != TipoUbicacionDb.AutomatismoMultiproducto)
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionTipoAutomatismo", TipoUbicacionDb.AutomatismoMultiproducto));
            }
            else if (context.GetUbicacionEquipo(preparacion.Predio) == null)
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionEquipoInvalida"));

            var producto = context.GetProducto(pickeo.Producto, pickeo.Empresa);
            if (producto == null)
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", pickeo.Producto, pickeo.Empresa));
            else if (!context.PermiteProductoInactivos && producto.Situacion != SituacionDb.Activo)
                errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", pickeo.Producto, pickeo.Empresa));
            else if (!producto.AceptaDecimales)
            {
                if (pickeo.Cantidad != Math.Truncate(pickeo.Cantidad))
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", pickeo.Producto));
            }

            if ((producto.ManejoIdentificador == ManejoIdentificador.Lote || producto.ManejoIdentificador == ManejoIdentificador.Serie))
            {
                if (string.IsNullOrEmpty(pickeo.Lote) || pickeo.Lote == ManejoIdentificadorDb.IdentificadorProducto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", pickeo.Producto));
                else if (pickeo.Lote == ManejoIdentificadorDb.IdentificadorAuto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLoteNoAUTO", pickeo.Producto));
            }
            else if (producto.ManejoIdentificador == ManejoIdentificador.Producto && pickeo.Lote != ManejoIdentificadorDb.IdentificadorProducto)
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", pickeo.Producto));

            if (producto.ManejoIdentificador == ManejoIdentificador.Serie && pickeo.Lote != ManejoIdentificadorDb.IdentificadorAuto && pickeo.Cantidad != 1)
                errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));

            ValidarAgrupacion(pickeo.Agrupacion, errors);

            if (pickeo.Agrupacion != preparacion.Agrupacion)
                errors.Add(new Error("WMSAPI_msg_Error_PreparacionAgrupacionDistinta", preparacion.Id));

            if (errors.Any())
                return false;

            switch (pickeo.Agrupacion)
            {
                case Agrupacion.Pedido:

                    if (!ValidarTipoAgente(pickeo.TipoAgente, context.ExisteTipoAgente, errors))
                        return false;

                    if (!ValidarAgente(pickeo.CodigoAgente, pickeo.Empresa, pickeo.TipoAgente, (t => context.GetAgente(t.Item1, t.Item2, t.Item3)), errors, out string cliente))
                        return false;

                    pickeo.Cliente = cliente;

                    if (!context.ExistePedido(pickeo.Pedido, pickeo.Empresa, pickeo.Cliente))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_PedidoNoExiste", pickeo.Pedido, pickeo.TipoAgente, pickeo.CodigoAgente, pickeo.Empresa));
                        return false;
                    }

                    if (pickeo.Carga != null || !string.IsNullOrEmpty(pickeo.ComparteContenedorPicking))
                        errors.Add(new Error("WMSAPI_msg_Error_AgrupacionPedidoDatosInnecesarios"));

                    break;
                case Agrupacion.Cliente:

                    if (!ValidarTipoAgente(pickeo.TipoAgente, context.ExisteTipoAgente, errors))
                        return false;

                    if (!ValidarAgente(pickeo.CodigoAgente, pickeo.Empresa, pickeo.TipoAgente, (t => context.GetAgente(t.Item1, t.Item2, t.Item3)), errors, out string cdCliente))
                        return false;

                    pickeo.Cliente = cdCliente;

                    if (pickeo.Carga != null || !string.IsNullOrEmpty(pickeo.Pedido))
                        errors.Add(new Error("WMSAPI_msg_Error_AgrupacionClienteDatosInnecesarios"));

                    break;
                case Agrupacion.Ruta:
                    if (!context.ExisteCarga((long)pickeo.Carga))
                        errors.Add(new Error("WMSAPI_msg_Error_CargaPickeoNoExistente", pickeo.Carga));

                    if (!string.IsNullOrEmpty(pickeo.Pedido) || !string.IsNullOrEmpty(pickeo.CodigoAgente) || !string.IsNullOrEmpty(pickeo.TipoAgente))
                        errors.Add(new Error("WMSAPI_msg_Error_AgrupacionClienteDatosInnecesarios"));
                    break;
                case Agrupacion.Onda:

                    if (pickeo.Carga != null || !string.IsNullOrEmpty(pickeo.Pedido) || !string.IsNullOrEmpty(pickeo.CodigoAgente) || !string.IsNullOrEmpty(pickeo.TipoAgente))
                        errors.Add(new Error("WMSAPI_msg_Error_AgrupacionClienteDatosInnecesarios"));

                    break;
                default:
                    errors.Add(new Error("WMSAPI_msg_Error_PreparacionSinAgrupacion", pickeo.NumeroPreparacion));
                    break;
            }

            ValidarContenedor(pickeo, context, preparacion, producto, errors);

            if (!estadosPermitidos.Contains(pickeo.Estado))
                errors.Add(new Error("WMSAPI_msg_Error_EstadoDetallePickingInvalido"));

            return true;
        }

        public virtual bool ValidarContenedor(DetallePreparacion pickeo, IPickingServiceContext context, Preparacion preparacion, Producto producto, List<Error> errors)
        {
            var contenedor = context.GetContenedor(pickeo.IdExternoContenedor, pickeo.TipoContenedor);
            var subClaseProducto = context.GetSubClaseProducto(producto.CodigoClase);

            if (contenedor != null)
            {
                if ((!string.IsNullOrEmpty(contenedor.CodigoSubClase) && !string.IsNullOrEmpty(subClaseProducto?.Id)) && contenedor.CodigoSubClase != subClaseProducto.Id)
                    errors.Add(new Error("WMSAPI_msg_Error_ContProductoSubClaseDistinta"));
                else if (contenedor.NroLpn != null)
                    errors.Add(new Error("WMSAPI_msg_Error_OperacionConLpnNoPermitida"));

                var prepDestino = context.GetPreparacion(contenedor.NumeroPreparacion);

                switch (contenedor.EstadoId)
                {
                    case SituacionDb.ContenedorVacio:
                        errors.Add(new Error("General_msg_Error_ContenedorReservadoCrossDocking"));
                        break;
                    case SituacionDb.ContenedorEnPreparacion:

                        if (preparacion.Id != prepDestino.Id)
                            ValidarMultipreparacion(context, preparacion, prepDestino, errors);

                        var ubicContenedor = context.GetUbicacion(contenedor.Ubicacion);
                        if (preparacion.Predio != ubicContenedor.NumeroPredio)
                            errors.Add(new Error("WMSAPI_msg_Error_ContenedorOtroPredio"));

                        if ((contenedor.IdContenedorEmpaque ?? "N") == "S")
                            errors.Add(new Error("WMSAPI_msg_Error_ContenedorEmpaquetado"));
                        else if (contenedor.CamionFacturado != null)
                            errors.Add(new Error("WMSAPI_msg_Error_ContenedorFacturado"));
                        else if (!string.IsNullOrEmpty(contenedor.Precinto1) || !string.IsNullOrEmpty(contenedor.Precinto2))
                            errors.Add(new Error("WMSAPI_msg_Error_ContenedorPrecintado"));
                        else if (!string.IsNullOrEmpty(contenedor.ValorControl))
                            errors.Add(new Error("WMSAPI_msg_Error_ContenedorControlado"));

                        break;
                    case SituacionDb.ContenedorEnCamion:
                        errors.Add(new Error("WMSAPI_msg_Error_ContenedorCargadoAlCamion"));
                        break;
                    case SituacionDb.ContenedorEnviado:
                    case SituacionDb.ContenedorEnsambladoKit:
                        if (contenedor.NumeroPreparacion == preparacion.Id)
                            errors.Add(new Error("WMSAPI_msg_Error_ContenedorYaUsadoPreparacion"));
                        break;
                    default:
                        errors.Add(new Error("WMSAPI_msg_Error_ContenedorNoHabilitado"));
                        break;
                }

                if (errors.Any())
                    return false;

                ValidarCompatibilidad(pickeo, context, preparacion, prepDestino, contenedor, errors);

                pickeo.NroContenedor = contenedor.Numero;
                pickeo.Contenedor = contenedor;
                pickeo.ExisteContenedor = true;
            }
            else
            {
                pickeo.Contenedor.CodigoSubClase = subClaseProducto?.Id;
                pickeo.Contenedor.CodigoBarras = _barcodeService.GenerateBarcode(pickeo.IdExternoContenedor, pickeo.TipoContenedor);
                pickeo.ExisteContenedor = false;

                if (!context.ExisteTipoContenedor(pickeo.Contenedor.TipoContenedor))
                    errors.Add(new Error("WMSAPI_msg_Error_TipoContenedorNoExiste", pickeo.Contenedor.TipoContenedor));
                else if (context.EsTipoLpn(pickeo.Contenedor.TipoContenedor))
                    errors.Add(new Error("WMSAPI_msg_Error_TipoContenedorNoPermitido"));
            }

            return true;
        }

        public virtual void ValidarMultipreparacion(IPickingServiceContext context, Preparacion prepOrigen, Preparacion prepDestino, List<Error> errors)
        {
            if (prepDestino.Tipo != TipoPreparacionDb.Normal)
                errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoTipoNoHabilitado"));
            else if (prepOrigen.Empresa != prepDestino.Empresa)
                errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoDistintaEmpresa"));
            else if (prepOrigen.Agrupacion != prepDestino.Agrupacion)
                errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoDistintaAgrupacion"));
            else
            {
                var nuCargaOrigen = context.GetNroCarga(prepDestino.Id, true);
                var nuCargaDestino = context.GetNroCarga(prepDestino.Id, false);

                var cargaCamionOrigen = context.GetCargaCamion(nuCargaOrigen);
                var cargaCamionDestino = context.GetCargaCamion(nuCargaDestino);

                if (cargaCamionOrigen != null || cargaCamionDestino != null)
                {
                    if ((cargaCamionOrigen == null && cargaCamionDestino != null))
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoCarga"));
                    else if (cargaCamionDestino == null && cargaCamionOrigen != null)
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoCarga"));
                    else if (cargaCamionOrigen != null && cargaCamionDestino != null)
                    {
                        if (cargaCamionOrigen.Camion != cargaCamionDestino.Camion)
                            errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoCarga"));
                    }
                }
            }
        }

        public virtual void ValidarCompatibilidad(DetallePreparacion pickeo, IPickingServiceContext context, Preparacion prepOrigen, Preparacion prepDestino, Contenedor contenedor, List<Error> errors)
        {
            ValidarPedidoOrigen(context, prepOrigen, prepDestino, contenedor, pickeo, errors);
            ValidarPedidoDestino(context, prepOrigen, prepDestino, contenedor, pickeo, errors);

            var comparteContenedorPicking = pickeo.ComparteContenedorPicking;
            if (pickeo.Agrupacion == Agrupacion.Pedido)
            {
                var pedidoOrigen = context.GetPedido(pickeo.Pedido, pickeo.Empresa, pickeo.Cliente);
                comparteContenedorPicking = pedidoOrigen.ComparteContenedorPicking;
            }

            if (!context.PuedeCompartirContenedorPicking(pickeo.Cliente, contenedor.Numero, comparteContenedorPicking))
                errors.Add(new Error("WMSAPI_msg_Error_ContenedorIncompatibe"));
        }

        public virtual void ValidarPedidoOrigen(IPickingServiceContext context, Preparacion preparacion, Preparacion prepDestino, Contenedor contenedor, DetallePreparacion pickeo, List<Error> errors)
        {
            switch (preparacion.Agrupacion)
            {
                case Agrupacion.Pedido:
                case Agrupacion.Cliente:
                    if (context.ExisteClienteDistintoContenedor(prepDestino.Id, contenedor.Numero, pickeo.Cliente))
                        errors.Add(new Error("WMSAPI_msg_Error_ContenedorIncompatibe"));
                    break;
                case Agrupacion.Ruta:
                    var ruta = context.GetRuta((long)pickeo.Carga);
                    if (context.ExisteRutaDistintaContenedor(prepDestino.Id, contenedor.Numero, ruta))
                        errors.Add(new Error("WMSAPI_msg_Error_ContenedorDistintaRuta"));
                    break;
                case Agrupacion.Onda:
                    if (preparacion.Onda != prepDestino.Onda)
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoDistintaOnda"));
                    break;
                default:
                    break;
            }
        }

        public virtual void ValidarPedidoDestino(IPickingServiceContext context, Preparacion preparacion, Preparacion prepDestino, Contenedor contenedor, DetallePreparacion pickeo, List<Error> errors)
        {
            switch (prepDestino.Agrupacion)
            {
                case Agrupacion.Pedido:
                case Agrupacion.Cliente:
                    if (context.ExisteClienteDistintoContenedor(prepDestino.Id, contenedor.Numero, pickeo.Cliente))
                        errors.Add(new Error("WMSAPI_msg_Error_ContenedorIncompatibe"));
                    break;
                case Agrupacion.Ruta:
                    var ruta = context.GetRuta((long)pickeo.Carga);
                    if (context.ExisteRutaDistintaContenedor(prepDestino.Id, contenedor.Numero, ruta))
                        errors.Add(new Error("WMSAPI_msg_Error_ContenedorDistintaRuta"));
                    break;
                case Agrupacion.Onda:
                    if (preparacion.Onda != prepDestino.Onda)
                        errors.Add(new Error("WMSAPI_msg_Error_PreparacionDestinoDistintaOnda"));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Producto
        public virtual bool ProductoValidacionCarga(Producto producto, IProductoServiceContext context, List<Error> errors)
        {
            var oldModel = context.GetProducto(producto.Codigo);
            var camposInmutables = context.GetCamposInmutables();

            ValidarCodigoProducto(producto.Codigo, context, errors);

            ValidarCampo(oldModel, producto, camposInmutables, "Descripcion", producto.Descripcion, true, typeof(string), 65, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "CodigoMercadologico", producto.CodigoMercadologico, true, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "UnidadBulto", producto.UnidadBulto.ToString(), true, typeof(decimal), 9, errors, 3);
            ValidarCampo(oldModel, producto, camposInmutables, "CodigoClase", producto.CodigoClase, true, typeof(string), 2, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Situacion", producto.Situacion.ToString(), true, typeof(short), 3, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "ManejoIdentificador", producto.ManejoIdentificadorId, true, typeof(string), 1, errors, campoInterno: nameof(producto.ManejoIdentificadorId));
            ValidarCampo(oldModel, producto, camposInmutables, "UnidadDistribucion", producto.UnidadDistribucion.ToString(), true, typeof(decimal), 9, errors, 3);
            ValidarCampo(oldModel, producto, camposInmutables, "UnidadMedida", producto.UnidadMedida, true, typeof(string), 10, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "CodigoFamilia", producto.CodigoFamilia.ToString(), true, typeof(int), 10, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "VolumenCC", producto.VolumenCC?.ToString(), true, typeof(decimal), 14, errors, 4, false);
            ValidarCampo(oldModel, producto, camposInmutables, "TipoManejoFecha", producto.TipoManejoFecha, true, typeof(string), 1, errors);

            ValidarCampo(oldModel, producto, camposInmutables, "ModalidadIngresoLote", producto.ModalidadIngresoLote, false, typeof(string), 10, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Ramo", producto.Ramo.ToString(), false, typeof(short), 2, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "CodigoRotatividad", producto.CodigoRotatividad?.ToString(), false, typeof(short), 2, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "Exclusivo", producto.Exclusivo?.ToString(), false, typeof(short), 4, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "GrupoConsulta", producto.GrupoConsulta, false, typeof(string), 20, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "NAM", producto.NAM, false, typeof(string), 20, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Nivel", producto.Nivel, false, typeof(string), 11, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "CodigoOrigen", producto.CdOrigen, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "CodigoProductoEmpresa", producto.CodigoProductoEmpresa, false, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "ProductoUnico", producto.ProductoUnico, false, typeof(short), 2, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "UndMedidaFact", producto.UndMedidaFact, false, typeof(string), 10, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "UndEmb", producto.UndEmb, false, typeof(string), 10, errors);

            ValidarCampo(oldModel, producto, camposInmutables, "Anexo1", producto.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Anexo2", producto.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Anexo3", producto.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Anexo4", producto.Anexo4, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Anexo5", producto.Anexo5, false, typeof(string), 18, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "DescDifPeso", producto.DescDifPeso, false, typeof(string), 4, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "DescripcionDisplay", producto.DescripcionDisplay, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "AyudaColector", producto.AyudaColector, false, typeof(string), 200, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "DescripcionReducida", producto.DescripcionReducida, false, typeof(string), 20, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "AceptaDecimales", producto.AceptaDecimalesId, false, typeof(string), 1, errors, campoInterno: nameof(producto.AceptaDecimalesId));
            ValidarCampo(oldModel, producto, camposInmutables, "Conversion", producto.Conversion?.ToString(), false, typeof(decimal), 12, errors, 3);
            ValidarCampo(oldModel, producto, camposInmutables, "Agrupacion", producto.Agrupacion, false, typeof(string), 1, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Componente1", producto.Componente1, false, typeof(string), 20, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Componente2", producto.Componente2, false, typeof(string), 20, errors);

            ValidarCampo(oldModel, producto, camposInmutables, "PesoBruto", producto.PesoBruto?.ToString(), false, typeof(decimal), 12, errors, 3, false);
            ValidarCampo(oldModel, producto, camposInmutables, "PesoNeto", producto.PesoNeto?.ToString(), false, typeof(decimal), 12, errors, 3, false);
            ValidarCampo(oldModel, producto, camposInmutables, "DiasDuracion", producto.DiasDuracion?.ToString(), false, typeof(short), 4, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "DiasValidez", producto.DiasValidez?.ToString(), false, typeof(short), 4, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "DiasLiberacion", producto.DiasLiberacion?.ToString(), false, typeof(short), 4, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "StockMaximo", producto.StockMaximo?.ToString(), false, typeof(int), 9, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "StockMinimo", producto.StockMinimo?.ToString(), false, typeof(int), 9, errors, 0, false);
            ValidarCampo(oldModel, producto, camposInmutables, "CantidadGenerica", producto.CantidadGenerica?.ToString(), false, typeof(decimal), 9, errors, 3);
            ValidarCampo(oldModel, producto, camposInmutables, "CantidadPadronStock", producto.CantidadPadronStock?.ToString(), false, typeof(int), 9, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "SubBulto", producto.SubBulto?.ToString(), false, typeof(short), 3, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "SgProducto", producto.SgProducto?.ToString(), false, typeof(string), 13, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "TipoDisplay", producto.TipoDisplay?.ToString(), false, typeof(string), 1, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "TipoPeso", producto.TipoPeso?.ToString(), false, typeof(short), 1, errors);

            ValidarCampo(oldModel, producto, camposInmutables, "Altura", producto.Altura?.ToString(), false, typeof(decimal), 10, errors, 4, false);
            ValidarCampo(oldModel, producto, camposInmutables, "AvisoAjusteInventario", producto.AvisoAjusteInventario?.ToString(), false, typeof(decimal), 14, errors, 4, false);
            ValidarCampo(oldModel, producto, camposInmutables, "UltimoCosto", producto.UltimoCosto?.ToString(), false, typeof(decimal), 16, errors, 2, false);
            ValidarCampo(oldModel, producto, camposInmutables, "Ancho", producto.Ancho?.ToString(), false, typeof(decimal), 10, errors, 4, false);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioDistribucion", producto.PrecioDistribucion?.ToString(), false, typeof(decimal), 10, errors, 4);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioEgreso", producto.PrecioEgreso?.ToString(), false, typeof(decimal), 10, errors, 4);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioIngreso", producto.PrecioIngreso?.ToString(), false, typeof(decimal), 10, errors, 4);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioSegDistribucion", producto.PrecioSegDistribucion?.ToString(), false, typeof(decimal), 10, errors, 4);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioSegStock", producto.PrecioSegStock?.ToString(), false, typeof(decimal), 10, errors, 4);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioStock", producto.PrecioStock?.ToString(), false, typeof(decimal), 10, errors, 4, false);
            ValidarCampo(oldModel, producto, camposInmutables, "PrecioVenta", producto.PrecioVenta?.ToString(), false, typeof(decimal), 16, errors, 2, false);
            ValidarCampo(oldModel, producto, camposInmutables, "Profundidad", producto.Profundidad?.ToString(), false, typeof(decimal), 10, errors, 4, false);

            ValidarCampo(oldModel, producto, camposInmutables, "CodigoBase", producto.CodigoBase, false, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Talle", producto.Talle, false, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Color", producto.Color, false, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Temporada", producto.Temporada, false, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Categoria1", producto.Categoria1, false, typeof(string), 40, errors);
            ValidarCampo(oldModel, producto, camposInmutables, "Categoria2", producto.Categoria2, false, typeof(string), 40, errors);

            return true;
        }

        public virtual bool ProductoValidacionProcedimiento(Producto producto, IProductoServiceContext context, List<Error> errors)
        {
            var tpManejoFecha = ManejoFechaProductoDb.GetConstantNames();
            var tpManejoIdentificador = ManejoIdentificadorDb.GetConstantNames();
            var tpModalidadIngresoLote = ModalidadIngresoLoteDb.GetConstantNames();

            ValidarSituacion(producto.Situacion, errors);

            string ramoIngrHabilitado = context.GetParametro(ParamManager.IE_500_HAB_INGRESO_RAMO) ?? "N";
            string familiaIngrHabilitado = context.GetParametro(ParamManager.IE_500_HAB_INGRESO_FAMILIA) ?? "N";

            if (familiaIngrHabilitado != "S")
            {
                if (!context.ExisteFamilia((int)producto.CodigoFamilia))
                    errors.Add(new Error("WMSAPI_msg_Error_FamiliaNoExiste", producto.CodigoFamilia.ToString()));
            }

            if (ramoIngrHabilitado != "S")
            {
                if (!context.ExisteRamo((short)producto.Ramo))
                    errors.Add(new Error("WMSAPI_msg_Error_RamoNoExiste", producto.Ramo.ToString()));
            }

            if (!context.ExisteUnidadMedida(producto.UnidadMedida))
                errors.Add(new Error("WMSAPI_msg_Error_UnidadMedidaNoExiste"));

            if (producto.UnidadBulto <= 0)
                errors.Add(new Error("WMSAPI_msg_Error_ValorMayorACero"));

            if (producto.UnidadDistribucion <= 0)
                errors.Add(new Error("WMSAPI_msg_Error_ValorMayorACero"));

            if (producto.CodigoRotatividad != null)
                if (!context.ExisteRotatividad((short)producto.CodigoRotatividad))
                    AddError(errors, "WMSAPI_msg_Error_RotatividadNoExiste");

            if (!string.IsNullOrEmpty(producto.CodigoProductoEmpresa))
                if (context.ExisteProductoEmpresa(producto.CodigoProductoEmpresa, producto.Codigo))
                    AddError(errors, "WMSAPI_msg_Error_ProductoEmpresaExistente");

            if (!string.IsNullOrEmpty(producto.NAM))
                if (!context.ExisteNAM(producto.NAM))
                    AddError(errors, "WMSAPI_msg_Error_NAMNoExiste");

            if (producto.ManejoIdentificadorId == ManejoIdentificadorDb.Serie && producto.AceptaDecimalesId == "S")
                errors.Add(new Error("WMSAPI_msg_Error_TipoIdentificadorSerieNoAceptaDecimales", producto.Codigo));

            if (producto.ManejoIdentificadorId == ManejoIdentificadorDb.Serie && producto.UnidadBulto != 1)
                errors.Add(new Error("WMSAPI_msg_Error_TipoSerieCantidadDistintaAUno", "UnidadBulto", producto.Codigo));

            if (producto.ManejoIdentificadorId == ManejoIdentificadorDb.Serie && producto.UnidadDistribucion != 1)
                errors.Add(new Error("WMSAPI_msg_Error_TipoSerieCantidadDistintaAUno", "UnidadDistribucion", producto.Codigo));

            ValidarGrupoConsulta(producto.GrupoConsulta, context.ExisteGrupoConsulta, errors);
            ValidarAgrupacion(producto.Agrupacion, errors);
            ValidarTpPesoBruto(producto.TipoPeso, errors);
            ValidarSoN("RedondeoValidez", producto.RedondeoValidez, errors);
            ValidarSoN("IdCrossDocking", producto.IdCrossDocking, errors);
            ValidarSoN("ManejaTomaDato", producto.ManejaTomaDato, errors);

            Producto modelo = context.GetProducto(producto.Codigo);
            string claseIngrHabilitado = context.GetParametro(ParamManager.IE_500_HAB_INGRESO_CLASE) ?? "N";

            if (claseIngrHabilitado != "S")
                if (!context.ExisteClase(producto.CodigoClase))
                    errors.Add(new Error("WMSAPI_msg_Error_ClaseNoExiste", producto.CodigoClase));

            if (!tpManejoFecha.Contains(producto.TipoManejoFecha))
                AddError(errors, "WMSAPI_msg_Error_TipoManejoFechaInvalido");

            if (!tpManejoIdentificador.Contains(producto.ManejoIdentificadorId))
                AddError(errors, "WMSAPI_msg_Error_ManejoIdentificadorInvalido");
            else if (modelo != null && producto.ManejoIdentificadorId != modelo.ManejoIdentificadorId)
            {
                if ((modelo.ManejoIdentificadorId != ManejoIdentificadorDb.Producto) || (modelo.ManejoIdentificadorId == ManejoIdentificadorDb.Producto && producto.ManejoIdentificadorId == ManejoIdentificadorDb.Serie))
                {
                    if (!context.PermiteEdicion(modelo.Codigo))
                        AddError(errors, "WMSAPI_msg_Error_ManejoIdentificadorProductoStock");
                }
            }

            if (!string.IsNullOrEmpty(producto.ModalidadIngresoLote))
            {
                if (!tpModalidadIngresoLote.Contains(producto.ModalidadIngresoLote))
                    AddError(errors, "WMSAPI_msg_Error_ModLoteNoValida");
                else if (producto.ManejoIdentificadorId != ManejoIdentificadorDb.Lote && producto.ModalidadIngresoLote != ModalidadIngresoLoteDb.Normal)
                    errors.Add(new Error("WMSAPI_msg_Error_ModLoteIncompatibleConTpIdent", producto.ManejoIdentificadorId, ModalidadIngresoLoteDb.Normal));

                if (producto.IsFifo() && producto.ModalidadIngresoLote == ModalidadIngresoLoteDb.Vencimiento)
                    errors.Add(new Error("REG009_msg_Error_ManejoFechaIncompatibleModalidadLote"));
            }

            if (modelo != null && producto.AceptaDecimalesId != modelo.AceptaDecimalesId)
            {
                if (producto.AceptaDecimalesId == "N" && !context.PermiteEdicion(modelo.Codigo))
                    AddError(errors, "WMSAPI_msg_Error_AceptaDecimalesProductoStock");
            }

            ValidarSoN("AceptaDecimales", producto.AceptaDecimalesId, errors);
            ValidarCantDecimalesProducto(producto, errors);

            return true;
        }

        public virtual void SetDefaultValuesProducto(Producto producto, IProductoServiceContext context)
        {
            if (producto.CodigoFamilia == -1)
            {
                var param = context.GetParametro(ParamManager.IE_500_FAMILIA_PRODUTO) ?? FamiliaProductoDb.General.ToString();
                if (int.TryParse(param, out int parsed))
                    producto.CodigoFamilia = parsed;
                else
                    producto.CodigoFamilia = FamiliaProductoDb.General;
            }

            if (producto.CodigoRotatividad == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_ROTATIVIDADE);
                if (short.TryParse(param, out short parsed))
                    producto.CodigoRotatividad = parsed;
            }

            if (string.IsNullOrEmpty(producto.CodigoClase))
                producto.CodigoClase = context.GetParametro(ParamManager.IE_500_CLASSE);

            if (producto.StockMinimo == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_ESTOQUE_MINIMO);
                if (int.TryParse(param, out int parsed))
                    producto.StockMinimo = parsed;
            }

            if (producto.StockMaximo == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_ESTOQUE_MAXIMO);
                if (int.TryParse(param, out int parsed))
                    producto.StockMaximo = parsed;
            }

            if (producto.PesoNeto == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_PS_LIQUIDO);

                if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                    producto.PesoNeto = outValue;
            }

            if (producto.PesoBruto == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_PS_BRUTO);

                if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                    producto.PesoBruto = outValue;
            }

            if (producto.VolumenCC == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_VL_CUBAGEM);

                if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                    producto.VolumenCC = outValue;
            }

            if (producto.PrecioVenta == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_VL_PRECO_VENDA);

                if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                    producto.PrecioVenta = outValue;
            }

            if (producto.UltimoCosto == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_VL_CUSTO_ULT_ENT);

                if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                    producto.UltimoCosto = outValue;
            }

            if (producto.UnidadDistribucion < 1)
            {
                if (producto.ManejoIdentificadorId == ManejoIdentificadorDb.Serie)
                    producto.UnidadDistribucion = 1;
                else
                {
                    var param = context.GetParametro(ParamManager.IE_500_UND_DISTRIBUCION);

                    if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                        producto.UnidadDistribucion = outValue;
                }
            }

            if (producto.UnidadBulto < 1)
            {
                var param = context.GetParametro(ParamManager.IE_500_UND_BULTO);

                if (decimal.TryParse(param, NumberStyles.Any, _culture, out decimal outValue))
                    producto.UnidadBulto = outValue;
            }

            if (string.IsNullOrEmpty(producto.ManejoIdentificadorId))
                producto.ManejoIdentificadorId = context.GetParametro(ParamManager.IE_500_ID_MANEJO_IDENT);

            if (string.IsNullOrEmpty(producto.UnidadMedida))
                producto.UnidadMedida = context.GetParametro(ParamManager.IE_500_UNIDA_DE_MEDIDA);

            if (string.IsNullOrEmpty(producto.TipoManejoFecha))
                producto.TipoManejoFecha = context.GetParametro(ParamManager.IE_500_TP_MANEJO_FECHA);

            if (producto.Situacion == -1)
            {
                var param = context.GetParametro(ParamManager.IE_500_CD_SITUACAO) ?? SituacionDb.Activo.ToString();
                if (short.TryParse(param, out short parsed))
                    producto.Situacion = parsed;
                else
                    producto.Situacion = SituacionDb.Activo;
            }

            if (string.IsNullOrEmpty(producto.GrupoConsulta))
                producto.GrupoConsulta = context.GetParametro(ParamManager.IE_500_CD_GRUPO_CONSULTA);

            if (string.IsNullOrEmpty(producto.TipoDisplay))
                producto.TipoDisplay = context.GetParametro(ParamManager.IE_500_TP_DISPLAY);

            if (producto.DiasDuracion == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_QT_DIAS_DURACAO);
                if (short.TryParse(param, out short parsed))
                    producto.DiasDuracion = parsed;
            }

            if (producto.DiasValidez == null)
            {
                var param = context.GetParametro(ParamManager.IE_500_QT_DIAS_VALIDADEA);
                if (short.TryParse(param, out short parsed))
                    producto.DiasValidez = parsed;
            }

            if (string.IsNullOrEmpty(producto.Agrupacion))
                producto.Agrupacion = context.GetParametro(ParamManager.IE_500_ID_AGRUPACION);

            if (producto.Ramo == -1)
            {
                var param = context.GetParametro(ParamManager.IE_500_CD_RAMO_PRODUTO) ?? RamoProductoDb.General.ToString();
                if (short.TryParse(param, out short parsed))
                    producto.Ramo = parsed;
                else
                    producto.Ramo = RamoProductoDb.General;
            }

            if (string.IsNullOrEmpty(producto.AceptaDecimalesId))
                producto.AceptaDecimalesId = context.GetParametro(ParamManager.IE_500_FL_ACEPTA_DECIMALES);

            if (string.IsNullOrEmpty(producto.CodigoProductoEmpresa))
                producto.CodigoProductoEmpresa = producto.Codigo;

            if (string.IsNullOrEmpty(producto.CodigoMercadologico))
                producto.CodigoMercadologico = producto.Codigo.Truncate(20);
        }

        public virtual bool ValidarCodigoProducto(string cdProducto, IServiceContext context, List<Error> errors)
        {
            ValidarCampo("CodigoProducto", cdProducto, true, typeof(string), 40, errors);

            if (!string.IsNullOrEmpty(cdProducto))
            {
                string caracteresPermitidos = context.GetParametro(ParamManager.LISTA_CARACTERES_COD_PROD);

                if (!cdProducto.IsUpper())
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresMinuscula"));
                else if (!Validations.ValidarCaracteres("CodigoProducto", cdProducto, caracteresPermitidos, out Error error))
                    errors.Add(error);
            }

            return true;
        }

        public virtual bool ValidarTpPesoBruto(short? tpPesoBruto, List<Error> errors)
        {
            if (tpPesoBruto != null)
            {
                short[] valueVlid = { 1, 2, 3 };
                if (!valueVlid.Contains((short)tpPesoBruto))
                    errors.Add(new Error("WMSAPI_msg_Error_PesoBruto"));
            }

            return true;
        }

        public virtual bool ValidarCantDecimalesProducto(Producto producto, List<Error> errors)
        {
            if (producto.AceptaDecimalesId != "S")
            {
                if (!int.TryParse(producto.UnidadBulto.ToString(), out int undBulto) && ((producto.UnidadBulto % 1) != 0))
                    errors.Add(new Error("WMSAPI_msg_Error_ManejoDecimalesUnidadBulto"));

                if (!int.TryParse(producto.UnidadDistribucion.ToString(), out int undDistribucion) && ((producto.UnidadDistribucion % 1) != 0))
                    errors.Add(new Error("WMSAPI_msg_Error_ManejoDecimalesUnidadDistribucion"));
            }

            return !errors.Any();
        }
        #endregion

        #region ProductoProveedor
        public virtual bool ProductoProveedorValidacionCarga(ProductoProveedor producto, IProductoProveedorServiceContext context, List<Error> errors)
        {
            ValidarCampo("CodigoProducto", producto.CodigoProducto, true, typeof(string), 40, errors);
            ValidarCampo("CodigoExterno", producto.CodigoExterno, true, typeof(string), 30, errors);
            ValidarCampo("CodigoAgente", producto.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", producto.TipoAgente, true, typeof(string), 3, errors);
            ValidarCampo("TipoOperacion", producto.TipoOperacionId, false, typeof(string), 1, errors);

            return true;
        }

        public virtual bool ProductoProveedorValidacionProcedimiento(ProductoProveedor producto, IProductoProveedorServiceContext context, List<Error> errors)
        {
            if (!ValidarProducto(producto.CodigoProducto, producto.Empresa, context.GetProducto(producto.Empresa, producto.CodigoProducto), errors, context.PermiteProductoInactivos))
                return false;

            if (!ValidarTipoAgente(producto.TipoAgente, context.ExisteTipoAgente, errors))
                return false;

            if (!ValidarAgente(producto.CodigoAgente, producto.Empresa, producto.TipoAgente,
                (t => context.GetAgente(t.Item1, t.Item2, t.Item3)), errors, out string cdCliente))
                return false;

            if (!ValidarTipoOperacion(producto.TipoOperacionId, false, errors))
                return false;

            ProductoProveedor model = context.GetProductoProveedor(producto.CodigoProducto, producto.TipoAgente, producto.CodigoAgente, producto.Empresa);

            if (producto.TipoOperacionId == "A")
            {
                if (model != null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoProveedorYaExiste", producto.CodigoProducto, producto.Empresa, producto.TipoAgente, producto.CodigoAgente));
                    return false;
                }
                else if (context.ExisteCodExterno(producto.CodigoProducto, producto.Empresa, cdCliente, producto.CodigoExterno))
                {
                    AddError(errors, $"CodigoExterno: {"WMSAPI_msg_Error_ExternoOtroProducto"}");
                    return false;
                }
            }
            else if (producto.TipoOperacionId == "B" && model == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoProveedorNoExiste", producto.CodigoProducto, producto.Empresa, producto.TipoAgente, producto.CodigoAgente));
                return false;
            }
            return true;
        }
        #endregion

        #region Referencia
        public virtual bool ReferenciaValidacionCarga(ReferenciaRecepcion referencia, IReferenciaRecepcionServiceContext context, List<Error> errors)
        {
            HashSet<string> detProductos = new HashSet<string>();

            ValidarCampo("Referencia", referencia.Numero, true, typeof(string), 20, errors);
            ValidarCampo("TipoReferencia", referencia.TipoReferencia, true, typeof(string), 6, errors);
            ValidarCampo("CodigoAgente", referencia.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", referencia.TipoAgente, true, typeof(string), 3, errors);
            ValidarCampo("Predio", referencia.IdPredio, false, typeof(string), 10, errors);
            ValidarCampo("FechaEmitida", referencia.FechaEmitida?.ToString(CDateFormats.DATE_ONLY), true, typeof(DateTime), -1, errors);

            ValidarCampo("Anexo1", referencia.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo("Anexo2", referencia.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo("Anexo3", referencia.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo("Memo", referencia.Memo, false, typeof(string), 200, errors);
            ValidarCampo("Moneda", referencia.Moneda, false, typeof(string), 6, errors);
            ValidarCampo("Serializado", referencia.Serializado, false, typeof(string), 200, errors);

            ValidarCampo("FechaEntrega", referencia.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("FechaVencimientoOrden", referencia.FechaVencimientoOrden?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);

            if (errors.Any())
                return false;

            foreach (var detalle in referencia.Detalles)
            {
                ValidarCampo("IdLineaSistemaExterno", detalle.IdLineaSistemaExterno, true, typeof(string), 40, errors);
                ValidarCampo("CodigoProducto", detalle.CodigoProducto, true, typeof(string), 40, errors);
                ValidarCampo("CantidadReferencia", detalle.CantidadReferencia?.ToString(), true, typeof(decimal), 15, errors, 3);

                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);
                ValidarCampo("Anexo1", detalle.Anexo1, false, typeof(string), 200, errors);
                ValidarCampo("ImporteUnitario", detalle.ImporteUnitario?.ToString(), false, typeof(decimal), 15, errors, 3);
                ValidarCampo("FechaVencimiento", detalle.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);

                string identificador = string.Empty;
                if (!string.IsNullOrEmpty(detalle.Identificador))
                    identificador = detalle.Identificador;

                string keyDetalle = $"{detalle.IdLineaSistemaExterno}.{detalle.CodigoProducto}.{identificador}";
                if (detProductos.Contains(keyDetalle))
                    AddError(errors, $"La referencia {referencia.Numero} contiene detalles duplicados. IdLineaSistemaExterno: {detalle.IdLineaSistemaExterno} - Producto: {detalle.CodigoProducto} - Identificador: {identificador}.");
                else
                    detProductos.Add(keyDetalle);

                if (errors.Any())
                    return false;
            }

            return true;
        }

        public virtual bool ReferenciaValidacionProcedimiento(ReferenciaRecepcion referencia, IReferenciaRecepcionServiceContext context, List<Error> errors)
        {
            if (!ValidarTipoAgente(referencia.TipoAgente, context.ExisteTipoAgente, errors))
                return false;

            if (!ValidarAgente(referencia.CodigoAgente, referencia.IdEmpresa, referencia.TipoAgente, t => context.GetAgente(t.Item1, t.Item2, t.Item3), errors, out string cdCliente))
                return false;

            if (!ValidarTipoReferencia(referencia.TipoReferencia, referencia.TipoAgente, null, context.ExisteTipoReferencia
                , context.ExisteTpRefTpRecepcion, t => context.ExisteTpRefTpAgente(t.Item1, t.Item2), null, errors))
                return false;

            if (context.ExisteReferencia(referencia.Numero, referencia.IdEmpresa, referencia.TipoReferencia, cdCliente))
            {
                errors.Add(new Error("WMSAPI_msg_Error_ReferenciaYaExiste", referencia.Numero, referencia.TipoReferencia, referencia.TipoAgente, referencia.CodigoAgente, referencia.IdEmpresa));
                return false;
            }

            ValidarPredio(referencia.IdPredio, context.ExistePredio, errors);

            if (!string.IsNullOrEmpty(referencia.Moneda))
                if (!context.ExisteMoneda(referencia.Moneda))
                    errors.Add(new Error("WMSAPI_msg_Error_MonedaNoExiste", referencia.Moneda));

            var validarFechas = (context.GetParametro(ParamManager.IE_510_VALIDAR_FECHAS) ?? "N") == "S";
            if (validarFechas)
            {
                ValidarFechaMenorA("FechaEmitida", referencia.FechaEmitida?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
                ValidarFechaMenorA("FechaEntrega", referencia.FechaEntrega?.ToString(CDateFormats.DATE_ONLY), referencia.FechaEmitida.ToString(CDateFormats.DATE_ONLY), "Emitida", errors);
                ValidarFechaMenorA("FechaVencimientoOrden", referencia.FechaVencimientoOrden?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
            }

            if (errors.Any())
                return false;

            foreach (var detalle in referencia.Detalles)
            {
                var prod = context.GetProducto(referencia.IdEmpresa, detalle.CodigoProducto);
                if (prod == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", detalle.CodigoProducto, referencia.IdEmpresa));
                    return false;
                }
                else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", detalle.CodigoProducto, referencia.Empresa));
                    return false;
                }
                else if (!prod.AceptaDecimales)
                {
                    if (detalle.CantidadReferencia.HasValue && detalle.CantidadReferencia.Value != Math.Truncate(detalle.CantidadReferencia.Value))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.CodigoProducto));
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(detalle.Identificador))
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    else
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorAuto;
                }
                else if (prod.ManejoIdentificador == ManejoIdentificador.Producto && detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.CodigoProducto));

                if (prod.TipoManejoFecha == ManejoFechaProductoDb.Expirable && detalle.FechaVencimiento == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaVencimiento", prod.Codigo));
                    return false;
                }
                else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Fifo)
                    detalle.FechaVencimiento = DateTime.Now;
                else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Duradero)
                    detalle.FechaVencimiento = null;

                if (validarFechas)
                    ValidarFechaMenorA("FechaVencimiento", detalle.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

                if (prod.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                {
                    if (detalle.CantidadReferencia != 1)
                    {
                        errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                        return false;
                    }
                    else if (context.ExisteSerie(prod.Codigo, detalle.Identificador))
                    {
                        errors.Add(new Error("General_Sec0_Error_SerieYaExiste", detalle.Identificador, prod.Codigo, referencia.IdEmpresa));
                        return false;
                    }
                }

                if (!prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ModificarReferenciaValidacionCarga(ReferenciaRecepcion referencia, IModificarDetalleReferenciaServiceContext context, List<Error> errors)
        {
            HashSet<string> detProductos = new HashSet<string>();

            ValidarCampo("Referencia", referencia.Numero, true, typeof(string), 20, errors);
            ValidarCampo("TipoReferencia", referencia.TipoReferencia, true, typeof(string), 6, errors);
            ValidarCampo("CodigoAgente", referencia.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", referencia.TipoAgente, true, typeof(string), 3, errors);

            if (errors.Any())
                return false;

            foreach (var detalle in referencia.Detalles)
            {
                ValidarCampo("IdLineaSistemaExterno", detalle.IdLineaSistemaExterno, true, typeof(string), 40, errors);
                ValidarCampo("CodigoProducto", detalle.CodigoProducto, true, typeof(string), 40, errors);
                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);
                ValidarCampo("TipoOperacion", detalle.TipoOperacionId, true, typeof(string), 1, errors);

                if (detalle.TipoOperacionId != TipoOperacionReferencia.Anular)
                    ValidarCampo("CantidadOperacion", detalle.CantidadReferencia?.ToString(), true, typeof(decimal), 15, errors, 3);
                else if (detalle.CantidadReferencia == 0) // Cuando el valor es nulo al parsearlo antes de entrar al controller lo deja en 0 y posteriormente tiraria error
                    detalle.CantidadReferencia = null;

                string keyProducto = $"{detalle.IdLineaSistemaExterno}.{detalle.CodigoProducto}.{detalle.Identificador}";
                if (detProductos.Contains(keyProducto))
                    AddError(errors, "WMSAPI_msg_Error_DetallesReferenciaDuplicados", [referencia.Numero, detalle.IdLineaSistemaExterno, detalle.CodigoProducto, detalle.Identificador]);
                else
                    detProductos.Add(keyProducto);

                if (errors.Any())
                    return false;
            }

            return true;
        }

        public virtual bool ModificarReferenciaValidacionProcedimiento(ReferenciaRecepcion referencia, IModificarDetalleReferenciaServiceContext context, List<Error> errors)
        {
            var tiposDeOperacion = TipoOperacionReferencia.GetConstantNames();

            if (!ValidarTipoAgente(referencia.TipoAgente, context.ExisteTipoAgente, errors))
                return false;

            if (!ValidarAgente(referencia.CodigoAgente, referencia.IdEmpresa, referencia.TipoAgente,
                t => context.GetAgente(t.Item1, t.Item2, t.Item3), errors, out string cdCliente))
                return false;

            if (!ValidarTipoReferencia(referencia.TipoReferencia, referencia.TipoAgente, null, context.ExisteTipoReferencia, context.ExisteTpRefTpRecepcion,
                t => context.ExisteTpRefTpAgente(t.Item1, t.Item2), null, errors))
                return false;

            var model = context.GetReferencia(referencia.Numero, referencia.IdEmpresa, referencia.TipoReferencia, cdCliente);
            if (model == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ReferenciaNOExiste", referencia.Numero, referencia.TipoReferencia, referencia.TipoAgente, referencia.CodigoAgente, referencia.IdEmpresa));
                return false;
            }
            else if (model.Estado != EstadoReferenciaRecepcionDb.Abierta)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ReferenciaEstadoIncorrecto", referencia.Numero));
                return false;
            }

            if (context.ReferenciaEnUso(model.Id))
                AddError(errors, "WMSAPI_msg_Error_ReferenciaEnUso");

            if (errors.Any())
                return false;

            foreach (var detalle in referencia.Detalles)
            {
                if (!tiposDeOperacion.Contains(detalle.TipoOperacionId))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_TpOperacionInvalido"));
                    return false;
                }
                else if (detalle.TipoOperacionId == TipoOperacionReferencia.Anular && detalle.CantidadReferencia != null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_TpOperacionACantidadNula"));
                    return false;
                }

                var prod = context.GetProducto(referencia.IdEmpresa, detalle.CodigoProducto);
                if (prod == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", detalle.CodigoProducto, referencia.IdEmpresa));
                    return false;
                }
                else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", detalle.CodigoProducto, referencia.Empresa));
                    return false;
                }
                else if (!prod.AceptaDecimales && detalle.TipoOperacionId != TipoOperacionReferencia.Anular)
                {
                    if (detalle.CantidadReferencia.HasValue && detalle.CantidadReferencia.Value != Math.Truncate(detalle.CantidadReferencia.Value))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.CodigoProducto));
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(detalle.Identificador))
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    else
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorAuto;
                }

                if (prod.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto && detalle.CantidadReferencia != 1)
                {
                    errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                    return false;
                }

                var detalleAux = context.GetDetalleReferencia(model.Id, detalle.IdLineaSistemaExterno, referencia.IdEmpresa, detalle.CodigoProducto, detalle.Identificador);

                if (detalleAux == null && detalle.TipoOperacionId != TipoOperacionReferencia.Nuevo)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_LineaDetalleNoExiste"));
                    return false;
                }
                else if (detalleAux != null)
                {
                    if (detalle.TipoOperacionId == TipoOperacionReferencia.Nuevo)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_LineaDetalleYaExiste"));
                        return false;
                    }

                    decimal saldo = (detalleAux.CantidadReferencia ?? 0) - (detalleAux.CantidadAnulada ?? 0) - (detalleAux.CantidadAgendada ?? 0) - (detalleAux.CantidadRecibida ?? 0);

                    if (saldo < 0)
                        saldo = 0;

                    if (detalle.TipoOperacionId == TipoOperacionReferencia.Reemplazar)
                    {
                        decimal consumido = (detalleAux.CantidadAnulada ?? 0) + (detalleAux.CantidadAgendada ?? 0) + (detalleAux.CantidadRecibida ?? 0);
                        if (detalle.CantidadReferencia < consumido)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_SaldoInsuficiente", referencia.Numero, detalle.IdLineaSistemaExterno, detalle.CodigoProducto, detalle.Identificador));
                            return false;
                        }
                    }
                }
                else
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto && detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.CodigoProducto));
                    else if (prod.ManejoIdentificador != ManejoIdentificador.Producto && detalle.Identificador == ManejoIdentificadorDb.IdentificadorProducto)
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", detalle.CodigoProducto));

                    if (!prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                        return false;
                    }
                }

                detalle.IdReferencia = model.Id;
            }

            return true;
        }

        public virtual bool AnularReferenciaValidacionCarga(ReferenciaRecepcion referencia, IAnularReferenciaServiceContext context, List<Error> errors)
        {
            ValidarCampo("Referencia", referencia.Numero, true, typeof(string), 20, errors);
            ValidarCampo("TipoReferencia", referencia.TipoReferencia, true, typeof(string), 6, errors);
            ValidarCampo("CodigoAgente", referencia.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", referencia.TipoAgente, true, typeof(string), 3, errors);

            if (errors.Any())
                return false;

            return true;
        }

        public virtual bool AnularReferenciaValidacionProcedimiento(ReferenciaRecepcion referencia, IAnularReferenciaServiceContext context, List<Error> errors)
        {
            if (!ValidarTipoAgente(referencia.TipoAgente, context.ExisteTipoAgente, errors))
                return false;

            if (!ValidarAgente(referencia.CodigoAgente, referencia.IdEmpresa, referencia.TipoAgente,
                t => context.GetAgente(t.Item1, t.Item2, t.Item3), errors, out string cdCliente))
                return false;

            if (!ValidarTipoReferencia(referencia.TipoReferencia, referencia.TipoAgente, null, context.ExisteTipoReferencia, context.ExisteTpRefTpRecepcion,
                t => context.ExisteTpRefTpAgente(t.Item1, t.Item2), null, errors))
                return false;

            var refAux = context.GetReferencia(referencia.Numero, referencia.IdEmpresa, referencia.TipoReferencia, cdCliente);
            if (refAux == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ReferenciaNOExiste", referencia.Numero, referencia.TipoReferencia, referencia.TipoAgente, referencia.CodigoAgente, referencia.IdEmpresa));
                return false;
            }
            else if (refAux.Situacion == SituacionDb.AnuladoCompletamente)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ReferenciaYaAnulada", referencia.Numero));
                return false;
            }

            if (context.ReferenciaEnUso(refAux.Id))
                AddError(errors, "WMSAPI_msg_Error_ReferenciaEnUso");

            if (errors.Any())
                return false;

            referencia.Id = refAux.Id;

            return true;
        }

        public virtual bool ValidarTipoReferencia(string tpReferencia, string tpAgente, string tpRecepcion, Predicate<string> ExisteTipoReferencia, Predicate<string> ExisteTpRefTpRecepcion, Predicate<Tuple<string, string>> ExisteTpRefTpAgente, Predicate<Tuple<string, string>> TipoRecCompatibleTpReferencia, List<Error> errors)
        {
            if (!ExisteTipoReferencia(tpReferencia))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoReferenciaNoExiste", tpReferencia));
                return false;
            }
            else if (!ExisteTpRefTpRecepcion(tpReferencia))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TpRefSinTpRec"));
                return false;
            }
            else if (!ExisteTpRefTpAgente(new Tuple<string, string>(tpReferencia, tpAgente)))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TpRefSinTpAgente"));
                return false;
            }
            else if (!string.IsNullOrEmpty(tpRecepcion) && !string.IsNullOrEmpty(tpReferencia))
            {
                if (!TipoRecCompatibleTpReferencia(new Tuple<string, string>(tpRecepcion, tpReferencia)))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_TpRefSinTpAgente"));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ValidarTipoRecepcion(Agenda agenda, IAgendaServiceContext context, List<Error> errors, out EmpresaRecepcionTipo tipoRec)
        {
            tipoRec = context.GetRecepcionTipoExternoByInterno(agenda.IdEmpresa, agenda.TipoRecepcionInterno);

            if (tipoRec == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoRecepcionNoExiste", agenda.TipoRecepcionInterno, agenda.IdEmpresa));
                return false;
            }
            else if (!tipoRec.Habilitado)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TpRecNoHabilitadoEmpresa", agenda.TipoRecepcionInterno, agenda.IdEmpresa));
                return false;
            }
            else if (tipoRec.RecepcionTipoInterno?.TipoAgente != agenda.TipoAgente)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TpRecSinTpAgente", agenda.TipoRecepcionInterno, agenda.TipoAgente));
                return false;
            }

            return true;
        }
        #endregion

        #region Stock

        #region Consulta Stock
        public virtual bool FiltrosStockValidacionCarga(FiltrosStock filtros, IStockServiceContext context, List<Error> errors)
        {
            ValidarCampo("Ubicacion", filtros.Ubicacion?.ToString(), false, typeof(string), 40, errors);
            ValidarCampo("Producto", filtros.Producto?.ToString(), false, typeof(string), 40, errors);
            ValidarCampo("CodigoClase", filtros.Clase?.ToString(), false, typeof(string), 2, errors);
            ValidarCampo("CodigoFamilia", filtros.Familia?.ToString(), false, typeof(int), 10, errors, -1, false);
            ValidarCampo("Ramo", filtros.Ramo?.ToString(), false, typeof(short), 2, errors, distintoCero: false);
            ValidarCampo("TipoManejoFecha", filtros.TipoManejoFecha?.ToString(), false, typeof(string), 1, errors);
            ValidarCampo("ManejoIdentificador", filtros.ManejoIdentificador?.ToString(), false, typeof(string), 1, errors);
            ValidarCampo("Predio", filtros.Predio?.ToString(), false, typeof(string), 10, errors);
            //validarCampo("Averia", filtros.Averia.ToString(), false, typeof(bool), -1, errors);
            ValidarCampo("GrupoConsulta", filtros.GrupoConsulta?.ToString(), false, typeof(string), 20, errors);

            return true;
        }

        public virtual bool FiltrosStockValidacionProcedimiento(IUnitOfWork uow, FiltrosStock filtros, IStockServiceContext context, List<Error> errors)
        {
            var tpManejoFecha = ManejoFechaProductoDb.GetConstantNames();
            var tpManejoIdentificador = ManejoIdentificadorDb.GetConstantNames();

            if (!string.IsNullOrEmpty(filtros.GrupoConsulta))
                ValidarGrupoConsulta(filtros.GrupoConsulta, context.ExisteGrupoConsulta, errors);

            if (!string.IsNullOrEmpty(filtros.Predio))
                ValidarPredio(filtros.Predio, context.ExistePredio, errors);

            if (!string.IsNullOrEmpty(filtros.Clase) && !context.ExisteClase(filtros.Clase))
                AddError(errors, "WMSAPI_msg_Error_ClaseNoExiste");

            if (filtros.Familia != null && !context.ExisteFamilia((int)filtros.Familia))
                AddError(errors, "WMSAPI_msg_Error_FamiliaNoExiste");

            if (filtros.Ramo != null && !context.ExisteRamo((short)filtros.Ramo))
                AddError(errors, "WMSAPI_msg_Error_RamoNoExiste");

            if (!string.IsNullOrEmpty(filtros.Ubicacion))
                ValidarUbicacion(filtros.Ubicacion, context.ExisteUbicacion, errors);

            if (!string.IsNullOrEmpty(filtros.Producto))
            {
                if (!uow.ProductoRepository.AnyProducto(filtros.Producto, filtros.Empresa))
                    AddError(errors, $"Producto: {string.Format("WMSAPI_msg_Error_ProductoNoExiste", filtros.Producto, filtros.Empresa)}");
            }

            if (!string.IsNullOrEmpty(filtros.TipoManejoFecha) && !tpManejoFecha.Contains(filtros.TipoManejoFecha))
                AddError(errors, "WMSAPI_msg_Error_TipoManejoFechaInvalido");

            if (!string.IsNullOrEmpty(filtros.ManejoIdentificador) && !tpManejoIdentificador.Contains(filtros.ManejoIdentificador))
                AddError(errors, "WMSAPI_msg_Error_ManejoIdentificadorInvalido");

            return true;
        }
        #endregion

        #region Ajuste Stock

        public virtual void AjusteStockValidacionCarga(AjusteStock ajuste, IAjustesDeStockServiceContext context, List<Error> errors)
        {
            ValidarCampo("Ubicacion", ajuste.Ubicacion, true, typeof(string), 40, errors);
            ValidarCampo("CodigoProducto", ajuste.Producto, true, typeof(string), 40, errors);
            ValidarCampo("Cantidad", ajuste.QtMovimiento?.ToString(), true, typeof(decimal), 12, errors, 3, noNegativo: false);
            ValidarCampo("Identificador", ajuste.Identificador, false, typeof(string), 40, errors);
            ValidarCampo("FechaVencimiento", ajuste.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
            ValidarCampo("Faixa", ajuste.Faixa.ToString(), true, typeof(decimal), 9, errors, 3, false);
            ValidarCampo("DescripcionMotivo", ajuste.DescMotivo, false, typeof(string), 50, errors);
            ValidarCampo("MotivoAjuste", ajuste.CdMotivoAjuste, false, typeof(string), 3, errors);
            ValidarCampo("Serializado", ajuste.Serializado, false, typeof(string), 4000, errors);
        }

        public virtual void AjusteStockValidacionProcedimiento(AjusteStock ajuste, IAjustesDeStockServiceContext context, List<Error> errors)
        {
            var ubicacion = context.GetUbicacion(ajuste.Ubicacion);
            if (ubicacion == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionNoExiste", ajuste.Ubicacion));
            }
            else
            {
                ValidarPredio(ubicacion.NumeroPredio, context.ExistePredio, errors);

                var prod = ValidarProducto(ajuste, context, errors);

                if (prod != null)
                {
                    ValidarUbicacion(ajuste, context, errors, ubicacion, prod);
                    ajuste.Predio = ubicacion.NumeroPredio;

                    ValidarCantidad(ajuste, prod, context, errors);

                    if (ajuste.TipoAjuste != TipoAjusteDb.Stock && ajuste.TipoAjuste != TipoAjusteDb.Automatismo)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_TipoDeAjusteInvalido", TipoAjusteDb.Stock, TipoAjusteDb.Automatismo));
                    }

                    if (!context.AnyMotivoAjuste(ajuste.CdMotivoAjuste))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_MotivoAjusteNoExiste"));
                    }
                }
            }
        }

        public virtual void ValidarCantidad(AjusteStock ajuste, Producto producto, IAjustesDeStockServiceContext context, List<Error> errors)
        {
            if (!producto.AceptaDecimales && ajuste.QtMovimiento.HasValue && ajuste.QtMovimiento.Value != Math.Truncate(ajuste.QtMovimiento.Value))
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", ajuste.Producto));
            }
            else if (ajuste.QtMovimiento == 0)
            {
                errors.Add(new Error("WMSAPI_msg_Error_CantidadMovimientoNoPuedeSer0", ajuste.Producto));
            }
            else if (ajuste.QtMovimiento < 0)
            {
                if (!context.ExisteStock(ajuste))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_NoExisteSaldoSuficienteParaEfectuarelAjuste", ajuste.Ubicacion, ajuste.Producto, ajuste.Empresa, ajuste.Identificador, ajuste.Faixa));
                }
                else if (context.GetCantidadSuelta(ajuste) < Math.Abs((ajuste.QtMovimiento ?? 0)))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_NoExisteSaldoSuficienteParaEfectuarelAjuste", ajuste.Ubicacion, ajuste.Producto, ajuste.Empresa, ajuste.Identificador, ajuste.Faixa));
                }
            }
            else
            {
                if (producto.ManejoIdentificador == ManejoIdentificador.Serie)
                {
                    if (ajuste.QtMovimiento != 1)
                        errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                    else if (context.ExisteSerie(ajuste.Producto, ajuste.Identificador))
                        errors.Add(new Error("General_Sec0_Error_SerieYaExiste", ajuste.Identificador, ajuste.Producto, ajuste.Empresa));
                }

                if (!producto.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(ajuste.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
            }
        }

        public virtual void ValidarUbicacion(AjusteStock ajuste, IAjustesDeStockServiceContext context, List<Error> errors, Ubicacion ubicacion, Producto prod)
        {
            UbicacionArea areaubic = context.GetAreaUbic(ubicacion.IdUbicacionArea);
            if (areaubic.EsAreaPicking)
            {
                var pickingProducto = context.GetUbicacionPickingProducto(ajuste.Ubicacion, ajuste.Producto, ajuste.Empresa);
                if (pickingProducto == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionPickingNoAsignadaProducto", ajuste.Ubicacion, ajuste.Producto));
                }
            }
            else if (!areaubic.EsAreaPicking && !areaubic.EsAreaStockGeneral)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AreaNoPermiteAjustar", ubicacion.IdUbicacionArea, ajuste.Ubicacion));
            }

            UbicacionTipo ubicacionTipo = context.GetTipoUbicacion(ubicacion.IdUbicacionTipo);
            if (!ubicacionTipo.PermiteVariosProductos && context.AnyProductoEnUbicacion(ajuste.Ubicacion, ajuste.Producto, ajuste.Empresa))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionMonoProducto", ubicacion.IdUbicacionArea, ajuste.Ubicacion));
            }
            else if (!ubicacionTipo.PermiteVariosLotes && context.AnyProductoLoteEnUbicacion(ajuste.Ubicacion, ajuste.Producto, ajuste.Empresa, ajuste.Identificador))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionMonoLote, ajuste.Ubicacion"));
            }

            if (prod.CodigoClase != ubicacion.CodigoClase)
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionTieneDistintaClaseProducto", ajuste.Ubicacion, ubicacion.CodigoClase, ajuste.Producto, ajuste.Empresa, prod.CodigoClase));
            }
        }

        public virtual Producto ValidarProducto(AjusteStock ajuste, IAjustesDeStockServiceContext context, List<Error> errors)
        {
            Producto prod = context.GetProducto(ajuste.Empresa, ajuste.Producto);
            if (prod == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", ajuste.Producto, ajuste.Empresa));
            }
            else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", ajuste.Producto, ajuste.Empresa));
            }
            else
            {
                if (prod.TipoManejoFecha == ManejoFechaProductoDb.Expirable && ajuste.QtMovimiento > 0 && ajuste.FechaVencimiento == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaVencimiento", prod.Codigo));
                }
                else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Fifo)
                    ajuste.FechaVencimiento = DateTime.Now;
                else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Duradero)
                    ajuste.FechaVencimiento = null;

                if ((prod.ManejoIdentificador == ManejoIdentificador.Lote || prod.ManejoIdentificador == ManejoIdentificador.Serie)
                 && (string.IsNullOrEmpty(ajuste.Identificador) || ajuste.Identificador == ManejoIdentificadorDb.IdentificadorProducto))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", ajuste.Producto));
                }
                else if (prod.ManejoIdentificador == ManejoIdentificador.Producto && ajuste.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", ajuste.Producto));
                }
            }

            return prod;
        }

        #endregion

        #region Transferencia
        public virtual void TransferenciaStockValidacionCarga(TransferenciaStock transferencia, ITransferenciaStockServiceContext context, List<Error> errors)
        {
            ValidarCampo("Ubicacion", transferencia.Ubicacion, true, typeof(string), 40, errors);
            ValidarCampo("UbicacionDestino", transferencia.UbicacionDestino, true, typeof(string), 40, errors);
            ValidarCampo("CodigoProducto", transferencia.Producto, true, typeof(string), 40, errors);
            ValidarCampo("Cantidad", transferencia.Cantidad.ToString(), true, typeof(decimal), 12, errors, 3);
            ValidarCampo("Faixa", transferencia.Faixa.ToString(), true, typeof(decimal), 9, errors, 3, false);
            ValidarCampo("Identificador", transferencia.Identificador, false, typeof(string), 40, errors);
        }

        public virtual bool TransferenciaStockValidacionProcedimiento(TransferenciaStock transferencia, ITransferenciaStockServiceContext context, List<Error> errors)
        {
            ValidarUbicacion(transferencia.Ubicacion, context.ExisteUbicacion, errors);
            ValidarUbicacion(transferencia.UbicacionDestino, context.ExisteUbicacion, errors);

            if (!context.PredioUnico())
            {
                errors.Add(new Error("WMSAPI_msg_Error_PredioUnico", transferencia.Ubicacion, transferencia.UbicacionDestino));
                return false;
            }

            var producto = context.GetProducto(transferencia.Producto, transferencia.Empresa);
            if (producto == null)
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", transferencia.Producto, transferencia.Empresa));
            else if (!context.PermiteProductoInactivos && producto.Situacion != SituacionDb.Activo)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", transferencia.Producto, transferencia.Empresa));
            }
            else if (!producto.AceptaDecimales)
            {
                if (transferencia.Cantidad != Math.Truncate(transferencia.Cantidad))
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", transferencia.Producto));
            }

            if (errors.Any())
                return false;

            var ubicDestino = context.GetUbicacion(transferencia.UbicacionDestino);

            var area = context.GetArea(ubicDestino.IdUbicacionArea);
            if (area.EsAreaPicking)
            {
                if (!context.ExisteAsignacionPicking(ubicDestino.Id, transferencia.Producto, transferencia.Empresa))
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionPickingNoAsignadaProducto", ubicDestino.Id, transferencia.Producto));
            }
            else if (!area.EsAreaPicking && !area.EsAreaStockGeneral)
                errors.Add(new Error("WMSAPI_msg_Error_AreaNoPermiteAjustar", ubicDestino.Id, ubicDestino.Id));

            var tipoUbicacion = context.GetTipoUbicacion(ubicDestino.IdUbicacionTipo);
            if (!tipoUbicacion.PermiteVariosProductos && context.AnyProductoUbicacion(ubicDestino.Id, transferencia.Producto, transferencia.Empresa))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionMonoProducto", ubicDestino.Id));
            }
            else if (!tipoUbicacion.PermiteVariosLotes && context.AnyProductoLoteUbicacion(ubicDestino.Id, transferencia.Producto, transferencia.Empresa, transferencia.Identificador))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionMonoLote", ubicDestino.Id));
            }

            if (producto.CodigoClase != ubicDestino.CodigoClase)
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionTieneDistintaClaseProducto", ubicDestino.Id, ubicDestino.CodigoClase, transferencia.Producto, transferencia.Empresa, producto.CodigoClase));

            if (transferencia.Cantidad <= 0)
                errors.Add(new Error("WMSAPI_msg_Error_ValorMayorACero"));
            else
            {
                var stockOrigen = context.GetStock(transferencia.Ubicacion, transferencia.Producto, transferencia.Empresa, transferencia.Identificador, transferencia.Faixa);
                if ((stockOrigen == null) || (context.GetCantidadDisponible(stockOrigen) < transferencia.Cantidad))
                    errors.Add(new Error("WMSAPI_msg_Error_NoExisteSaldoSuficienteParaEfectuarelAjuste", transferencia.Ubicacion, transferencia.Producto, transferencia.Empresa, transferencia.Identificador, transferencia.Faixa));
                else if (stockOrigen.Inventario == "D")
                    errors.Add(new Error("WMSAPI_msg_Error_StockPendienteInventario", transferencia.Ubicacion, transferencia.Producto, transferencia.Empresa, transferencia.Identificador, transferencia.Faixa));
                else
                {
                    var stockDestino = context.GetStock(ubicDestino.Id, transferencia.Producto, transferencia.Empresa, transferencia.Identificador, transferencia.Faixa);
                    if (stockDestino != null)
                    {
                        if (stockOrigen.Averia != stockDestino.Averia)
                            errors.Add(new Error("WMSAPI_msg_Error_OrigenDistintaAveriaDestino"));
                        else if (stockOrigen.ControlCalidad != stockDestino.ControlCalidad)
                            errors.Add(new Error("WMSAPI_msg_Error_OrigenDistintoCtrlCalidadDestino"));
                    }
                }
            }

            if ((producto.ManejoIdentificador == ManejoIdentificador.Lote || producto.ManejoIdentificador == ManejoIdentificador.Serie)
                    && (string.IsNullOrEmpty(transferencia.Identificador) || transferencia.Identificador == ManejoIdentificadorDb.IdentificadorProducto))
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", transferencia.Producto));
            }
            else if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
            {
                transferencia.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
            }

            if (producto.ManejoIdentificador == ManejoIdentificador.Serie && transferencia.Identificador != ManejoIdentificadorDb.IdentificadorAuto && transferencia.Cantidad != 1)
                errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));

            //if (!context.UbicacionEquipoValida())
            //    errors.Add(new Error("WMSAPI_msg_Error_UbicacionEquipoInvalida"));

            return true;
        }
        #endregion

        #endregion

        #region Metodos Auxiliares

        public virtual bool ValidarCampo<T>(T oldModel, T newModel, HashSet<string> camposInmutables, string campoRequest, string value, bool requerido, Type type, int maxLength, List<Error> errors, int precision = 0, bool distintoCero = true, bool noNegativo = true, bool formatoHora = false, string campoInterno = null, bool formatoEmail = false)
        {
            if (oldModel != null && camposInmutables.Contains(campoRequest))
            {
                var property = typeof(T).GetProperty(campoInterno ?? campoRequest);
                var oldValue = property.GetValue(oldModel);
                property.SetValue(newModel, oldValue);

                return true;
            }

            return ValidarCampo(campoRequest, value, requerido, type, maxLength, errors, precision, distintoCero, noNegativo, formatoHora, formatoEmail);
        }

        public virtual bool ValidarCampo(string campo, string value, bool requerido, Type type, int maxLength, List<Error> errors, int precision = 0, bool distintoCero = true, bool noNegativo = true, bool formatoHora = false, bool formatoEmail = false)
        {
            if (requerido && string.IsNullOrEmpty(value))
                errors.Add(new Error("WMSAPI_msg_Error_Requerido", campo));
            else if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > maxLength && (type != typeof(decimal) && type != typeof(DateTime) && type != typeof(bool)) && !formatoHora)
                    errors.Add(new Error("WMSAPI_msg_Error_LargoMaximo", campo, maxLength.ToString()));
                else
                {
                    if (type == typeof(int))
                    {
                        if (!int.TryParse(value, out int parsedValue))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                        else if (noNegativo && parsedValue < 0)
                            errors.Add(new Error("WMSAPI_msg_Error_ValorNegativo", campo));
                        else if (distintoCero && parsedValue == 0)
                            errors.Add(new Error("WMSAPI_msg_Error_NoPuedeSerCeo", campo));
                    }
                    else if (type == typeof(short))
                    {
                        if (!short.TryParse(value, out short parsedValue))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                        else if (noNegativo && parsedValue < 0)
                            errors.Add(new Error("WMSAPI_msg_Error_ValorNegativo", campo));
                        else if (distintoCero && parsedValue == 0)
                            errors.Add(new Error("WMSAPI_msg_Error_NoPuedeSerCeo", campo));
                    }
                    else if (type == typeof(long))
                    {
                        if (!long.TryParse(value, out long parsedValue))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                        else if (noNegativo && parsedValue < 0)
                            errors.Add(new Error("WMSAPI_msg_Error_ValorNegativo", campo));
                        else if (distintoCero && parsedValue == 0)
                            errors.Add(new Error("WMSAPI_msg_Error_NoPuedeSerCeo", campo));
                    }
                    else if (type == typeof(double))
                    {
                        if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                        else if (noNegativo && parsedValue < 0)
                            errors.Add(new Error("WMSAPI_msg_Error_ValorNegativo", campo));
                        else if (distintoCero && parsedValue == 0)
                            errors.Add(new Error("WMSAPI_msg_Error_NoPuedeSerCeo", campo));
                    }
                    else if (type == typeof(decimal))
                    {
                        if (!Validations.TryParse_Decimal(value, maxLength, precision, _separador, CultureInfo.InvariantCulture, out decimal parsedValue, out string msg))
                        {
                            switch (msg)
                            {
                                case "Valor vacio":
                                    errors.Add(new Error("WMSAPI_msg_Error_ValorVacio", campo));
                                    break;
                                case "Formato incorrecto.":
                                    errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                                    break;
                                default:
                                    errors.Add(new Error("WMSAPI_msg_Error_ErrorConversion", campo));
                                    break;
                            }
                        }
                        else if (noNegativo && parsedValue < 0)
                            errors.Add(new Error("WMSAPI_msg_Error_ValorNegativo", campo));
                        else if (distintoCero && parsedValue == 0)
                            errors.Add(new Error("WMSAPI_msg_Error_NoPuedeSerCeo", campo));
                    }
                    else if (type == typeof(DateTime))
                    {
                        if (!DateTimeExtension.IsValid_DDMMYYYY(value, this._culture))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatodeFechaIncorrecto", campo));
                    }
                    else if (type == typeof(bool))
                    {
                        if (!bool.TryParse(value, out bool parsedValue))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                    }
                    else if (formatoHora)
                    {
                        if (!DateTime.TryParseExact(value, CDateFormats.HORA_MINUTOS, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                    }
                    else if (formatoEmail)
                    {
                        if (!Regex.IsMatch(value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                            errors.Add(new Error("WMSAPI_msg_Error_FormatoIncorrecto", campo));
                    }
                }
            }
            return true;
        }

        public virtual bool ValidarCoD(string campo, string value, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] arrVal = { "C", "D" };
                if (!arrVal.Contains(value))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ValueCoD", campo));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarSoN(string campo, string value, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] arrVal = { "S", "N" };
                if (!arrVal.Contains(value))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ValueSoN", campo));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarLocalidad(Agente agente, Empresa empresa, Predicate<string> ExistePais, Predicate<Tuple<string, string>> ExisteLocalidad, Func<string, PaisSubdivision> GetSubdivision, Func<Tuple<string, string>, PaisSubdivisionLocalidad> GetLocalidadId, List<Error> errors)
        {
            string sinEspecificar = "S/E";
            bool especificaPais = false;
            bool espSubdivision = false;
            bool cargaAutomatica = false;

            string cdPais;
            string cdSubdivision;
            string cdLocalidad;
            try
            {
                if (agente != null)
                {
                    cdPais = agente.PaisId;
                    cdSubdivision = agente.SubdivisionId;
                    cdLocalidad = agente.MunicipioId;
                }
                else
                {
                    cdPais = empresa.PaisId;
                    cdSubdivision = empresa.SubdivisionId;
                    cdLocalidad = empresa.MunicipioId;
                }

                // Pais
                if (!string.IsNullOrEmpty(cdPais))
                {
                    especificaPais = true;

                    if (!ExistePais(cdPais))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ValorIngresadoNoExiste", "País", cdPais));
                        return false;
                    }
                }

                // Subdivision
                PaisSubdivision subdivision = null;
                if (!string.IsNullOrEmpty(cdSubdivision))
                {
                    espSubdivision = true;

                    subdivision = GetSubdivision(cdSubdivision);
                    if (subdivision == null)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ValorIngresadoNoExiste", "Subdivisión", cdSubdivision));
                        return false;
                    }
                    if (especificaPais && cdPais != subdivision.IdPais)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_SubdivisionPais", cdSubdivision, cdPais));
                        return false;
                    }
                }
                else if (especificaPais)
                {
                    string value = cdPais + "-" + sinEspecificar;
                    subdivision = GetSubdivision(value);
                    if (subdivision != null)
                        cargaAutomatica = true;
                }

                cdSubdivision = subdivision?.Id; // Cargo de vuelta por si tiene nuevo valor por la parte qeu le carga sin especificar

                // Localidad
                if (!string.IsNullOrEmpty(cdLocalidad) && !cargaAutomatica)
                {
                    if (!espSubdivision)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_SubdivisionNula"));
                        return false;
                    }

                    if (!ExisteLocalidad(new Tuple<string, string>(cdLocalidad, cdSubdivision)))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_LocalidadSubdivision", cdLocalidad, cdSubdivision));
                        return false;
                    }
                }
                else if (espSubdivision || cargaAutomatica)
                {
                    var subLocalidad = GetLocalidadId(new Tuple<string, string>(sinEspecificar, cdSubdivision));
                    if (subLocalidad != null)
                        cdLocalidad = subLocalidad.Codigo;
                }

                if (agente != null)
                {
                    agente.SubdivisionId = cdSubdivision;
                    agente.MunicipioId = cdLocalidad;
                }
                else
                {
                    empresa.SubdivisionId = cdSubdivision;
                    empresa.MunicipioId = cdLocalidad;
                }

                return true;
            }
            catch (Exception ex)
            {
                AddError(errors, ex.Message);
                return false;
            }
        }

        public virtual bool ValidarProducto(string value, int empresa, Producto prod, List<Error> errors, bool permiteProductoInactivos)
        {
            if (prod == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", value, empresa));
                return false;
            }
            else if (!permiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", value, empresa));
                return false;
            }
            return true;
        }

        public virtual bool ValidarSituacion(short situacion, List<Error> errors)
        {
            var situaciones = new List<short>() { SituacionDb.Activo, SituacionDb.Inactivo };

            if (!situaciones.Contains(situacion))
            {
                errors.Add(new Error("WMSAPI_msg_Error_SituacionNoValida"));
                return false;
            }
            return true;
        }

        public virtual bool ValidarRuta(short ruta, Predicate<short> ExisteRuta, List<Error> errors)
        {
            if (!ExisteRuta(ruta))
            {
                errors.Add(new Error("WMSAPI_msg_Error_RutaNoExiste"));
                return false;
            }
            return true;
        }

        public virtual bool ValidarGrupoConsulta(string value, Predicate<string> ExisteGrupoConsulta, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!ExisteGrupoConsulta(value))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_GrupoConsultaNoExiste"));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarAgrupacion(string agrupacion, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(agrupacion))
            {
                var tpAgrupacion = Agrupacion.GetConstantNames();
                if (!tpAgrupacion.Contains(agrupacion))
                    AddError(errors, "WMSAPI_msg_Error_AgrupacionNoValida");
            }
            return true;
        }

        public virtual bool ValidarPredio(string value, Predicate<string> ExistePredio, List<Error> errors)
        {
            if (!ExistePredio(value))
            {
                errors.Add(new Error("WMSAPI_msg_Error_PredioNoExiste", value));
                return false;
            }
            return true;
        }

        public virtual bool ValidarAgente(string cdAgente, int empresa, string tipo, Func<Tuple<string, int, string>, Agente> GetAgente, List<Error> errors, out string cdCliente)
        {
            cdCliente = string.Empty;
            var agente = GetAgente(new Tuple<string, int, string>(cdAgente, empresa, tipo));
            if (agente == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AgenteNoEncontrado", cdAgente, empresa, tipo));
                return false;
            }
            else
                cdCliente = agente.CodigoInterno;

            return true;
        }

        public virtual bool ValidarTipoAgente(string tpAgente, Predicate<string> ExisteTipoAgente, List<Error> errors)
        {
            if (!ExisteTipoAgente(tpAgente))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TpAgenteNoValido", tpAgente));
                return false;
            }
            return true;
        }

        public virtual bool ValidarFechaMenorA(string campo, string fecha, string fechaAux, string campoAux, List<Error> errors, bool hoy = false)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                try
                {
                    DateTime? date = DateTimeExtension.FromString_DDMMYYYY(fecha, this._culture);
                    DateTime? fechaMin = DateTimeExtension.FromString_DDMMYYYY(fechaAux, this._culture);

                    if (date < fechaMin)
                    {
                        if (hoy)
                            errors.Add(new Error("WMSAPI_msg_Error_FechaNoPuederMenorAHoy", campo, fecha));
                        else
                            errors.Add(new Error("WMSAPI_msg_Error_FechaNoPuederMenorAX", campo, fecha, campoAux));

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ErrorNoControlado", campo, ex.Message));
                }
            }
            return true;
        }

        public virtual bool ValidarDecimalEntre(string campo, decimal? valor, decimal valorMin, decimal valorMax, List<Error> errors)
        {
            if (valor.HasValue && (valor.Value < valorMin || valor.Value > valorMax))
            {
                errors.Add(new Error("WMSAPI_msg_Error_DecimalDebeEstarComprendidoEntre", campo, valorMin, valorMax));
                return false;
            }

            return true;
        }

        public virtual bool ValidarFechaMayorA(string campo, string fecha, string fechaAux, string campoAux, List<Error> errors, bool hoy = false)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                try
                {
                    DateTime? date = DateTimeExtension.FromString_DDMMYYYY(fecha, this._culture);
                    DateTime? fechaMax = DateTimeExtension.FromString_DDMMYYYY(fechaAux, this._culture);

                    if (date > fechaMax)
                    {
                        if (hoy)
                            errors.Add(new Error("WMSAPI_msg_Error_FechaNoPuederMayorAHoy", campo, fecha));
                        else
                            errors.Add(new Error("WMSAPI_msg_Error_FechaNoPuederMayorAX", campo, fecha, campoAux));

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ErrorNoControlado", campo, ex.Message));
                }
            }
            return true;
        }

        public virtual bool ValidarTipoOperacion(string value, bool editable, List<Error> errors)
        {
            List<string> arrVal = new List<string>() { "A", "B" };

            if (editable)
            {
                arrVal.Add("S");
            }

            if (!arrVal.Contains(value))
            {
                if (editable)
                {
                    AddError(errors, "WMSAPI_msg_Error_TpOperacionASoB");
                }
                else
                {
                    AddError(errors, "WMSAPI_msg_Error_TpOperacionAoB");
                }
                return false;
            }
            return true;
        }

        public virtual bool ValidarEmpresa(int? value, Predicate<int> ExisteEmpresa, List<Error> errors)
        {
            if (value != null)
            {
                if (!ExisteEmpresa((int)value))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_EmpresaNoExiste", value));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarUbicacion(string value, Predicate<string> ExisteUbicacion, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!ExisteUbicacion(value))
                {
                    AddError(errors, "WMSAPI_msg_Error_UbicacionNoExiste");
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarTransportista(int? cdTransportadora, Predicate<int> ExisteTransportista, List<Error> errors)
        {
            if (cdTransportadora != null && cdTransportadora != -1)//Parala api de egreso Cuando mapeo  le seteo -1, debido a que el request puede ser nulo y en Camion no.
            {
                if (!ExisteTransportista((int)cdTransportadora))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_TransportadoraNoExiste", cdTransportadora));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidarZona(string zona, Predicate<string> ExisteZona, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(zona))
            {
                if (!ExisteZona(zona))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ZonaNoExiste", zona));
                    return false;
                }
            }
            return true;
        }


        public virtual bool ValidarPuerta(short? value, string predio, Func<string, short, bool> ExistePuerta, List<Error> errors)
        {
            if (value != null)
            {
                if (!ExistePuerta(predio, (short)value))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_PuertaNoExiste", predio));
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidateMaxItems(ValidationsResult result, int nroRegistro, int count, int max, bool total = false)
        {
            string msg = total ? "WMSAPI_msg_Error_MaxItemsDetallesSuperado" : "WMSAPI_msg_Error_MaxItemsSuperado";

            if (count > max)
            {
                var error = new Error(msg, max);

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var messages = Translator.Translate(uow, new List<Error>() { error }, _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }

                return false;
            }

            return true;
        }

        public virtual void AddError(List<Error> errors, string error)
        {
            errors.Add(new Error(error));
        }

        public virtual void AddError(List<Error> errors, string error, object[] argumentos)
        {
            errors.Add(new Error(error, argumentos));
        }

        public virtual async Task<ValidationResult<InterfazEjecucion>> ValidateReprocess(long nroInterfaz, int interfazExterna)
        {
            Error error = null;
            var result = new ValidationResult<InterfazEjecucion>();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                if (!await uow.EjecucionRepository.ExisteEjecucion(nroInterfaz))
                    error = new Error($"WMSAPI_msg_Error_InterfazNoExiste", nroInterfaz);

                if (error == null)
                {
                    var itfz = await uow.EjecucionRepository.GetEjecucion(nroInterfaz);

                    result.Value = itfz;

                    if (itfz.CdInterfazExterna != interfazExterna)
                        error = new Error("WMSAPI_msg_Error_InterfazExternaInvalida", nroInterfaz, interfazExterna);

                    if (error == null && itfz.Situacion != SituacionDb.ProcesadoConError)
                        error = new Error($"WMSAPI_msg_Error_InterfazSinEstado", nroInterfaz, SituacionDb.ProcesadoConError);
                }

                if (error != null)
                {
                    var messages = Translator.Translate(uow, error, _identity.UserId);
                    result.Error = messages ?? error.Mensaje;
                }
            }

            return result;
        }

        public virtual string Translate(Error error)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return Translator.Translate(uow, error, _identity.UserId) ?? error.Mensaje;
            }
        }

        #endregion

        #region CrossDocking
        public virtual void CrossDockingValidacionCarga(CrossDockingUnaFase crossDocking, ICrossDockingServiceContext context, List<Error> errors)
        {
            ValidarCampo("Empresa", crossDocking.Empresa.ToString(), true, typeof(int), 10, errors);
            ValidarCampo("Producto", crossDocking.Producto, true, typeof(string), 40, errors);
            ValidarCampo("Identificador", crossDocking.Identificador, true, typeof(string), 40, errors);
            ValidarCampo("Ubicacion", crossDocking.Ubicacion, true, typeof(string), 40, errors);
            ValidarCampo("Preparacion", crossDocking.Preparacion.ToString(), true, typeof(int), 6, errors);
            ValidarCampo("Agenda", crossDocking.Agenda.ToString(), true, typeof(int), 8, errors);
            ValidarCampo("CodigoAgente", crossDocking.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("TipoAgente", crossDocking.TipoAgente, true, typeof(string), 3, errors);
            ValidarCampo("Cantidad", crossDocking.Cantidad.ToString(), true, typeof(decimal), 12, errors, 3);
            ValidarCampo("Agenda", crossDocking.Agenda.ToString(), true, typeof(int), 3, errors);

            ValidarCampo("IdExternoContenedor", crossDocking.IdExternoContenedor.ToString(), true, typeof(string), 50, errors);
            ValidarCampo("TipoContenedor", crossDocking.TipoContenedor.ToString(), true, typeof(string), 10, errors);

        }

        public virtual void CrossDockingValidacionProcedimiento(CrossDockingUnaFase crossDocking, ICrossDockingServiceContext context, List<Error> errors)
        {
            var agenda = ValidarAgenda(crossDocking, context, errors);
            if (errors.Count == 0)
            {
                ValidarUbicacionPuerta(crossDocking, context, errors, agenda);

                ValidarDetalleProduto(crossDocking, context, errors);

                ValidarContenedor(crossDocking, context, errors);

                if (SituacionDb.ContenedorContabilizado != crossDocking.SituacionDestino && SituacionDb.ContenedorEnPreparacion != crossDocking.SituacionDestino)
                    errors.Add(new Error("WMSAPI_msg_Error_SituacionDestinoNoPermitidaEnInterfaz", SituacionDb.ContenedorEnPreparacion, SituacionDb.ContenedorContabilizado));
            }
        }

        public virtual void ValidarContenedor(CrossDockingUnaFase crossDocking, ICrossDockingServiceContext context, List<Error> errors)
        {
            var contenedor = context.GetContenedor(crossDocking.IdExternoContenedor, crossDocking.TipoContenedor);
            if (contenedor != null)
            {
                if (contenedor.NumeroPreparacion != crossDocking.Preparacion)
                    errors.Add(new Error("WMSAPI_msg_Error_ContenedorEstaEnOtraPreparacionActiva", crossDocking.IdExternoContenedor, crossDocking.TipoContenedor, contenedor.NumeroPreparacion));
                else if (contenedor.NroLpn != null)
                    errors.Add(new Error("WMSAPI_msg_Error_OperacionConLpnNoPermitida"));
            }

            var tipoEtiqueta = context.GetTipoContenedor(crossDocking.TipoContenedor);
            if (tipoEtiqueta == null)
                errors.Add(new Error("WMSAPI_msg_Error_TipoEtiquetaInvalida", crossDocking.TipoContenedor));
            else if (context.EsTipoLpn(crossDocking.TipoContenedor))
                errors.Add(new Error("WMSAPI_msg_Error_TipoContenedorNoPermitido"));

        }

        public virtual void SaldoPendienteCrossDockingValidacionProcedimiento(CrossDockingUnaFase detalle, ICrossDockingServiceContext context, List<Error> errors)
        {
            var detallePendiente = context.SaldoPendienteXd(detalle.Agenda, detalle.Preparacion, detalle.Cliente, detalle.Empresa, detalle.Producto, detalle.Identificador);
            if (detallePendiente == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_DetalleNoTieneCrossDocking", detalle.Agenda, detalle.Producto, detalle.Identificador, detalle.Cliente));
            }
            else if (detallePendiente.CantidadPendiente < detalle.Cantidad)
            {
                errors.Add(new Error("WMSAPI_msg_Error_CantidadIngrasadaSuperaLaPendiente", detalle.Agenda, detalle.Producto, detalle.Identificador, detalle.Cliente));
            }
        }

        public virtual Agenda ValidarAgenda(CrossDockingUnaFase crossDocking, ICrossDockingServiceContext context, List<Error> errors)
        {
            var agenda = context.GetAgenda(crossDocking.Agenda, crossDocking.Empresa);
            if (agenda == null)
                errors.Add(new Error("WMSAPI_msg_Error_AgendaNoExiste", crossDocking.Agenda, crossDocking.Empresa));
            else
            {
                var situacionesValidas = new List<short> { SituacionDb.AgendaAbierta, SituacionDb.AgendaCerrada, SituacionDb.AgendaCancelada };
                if (situacionesValidas.Contains(agenda.EstadoId))
                    errors.Add(new Error("WMSAPI_msg_Error_AgendaSituacionNoPermiteRealizarCrossDocking", agenda.Id));

                if (context.AnyCrossDocking(crossDocking.Agenda, crossDocking.Preparacion))
                    errors.Add(new Error("WMSAPI_msg_Error_AgendaNoTieneCrossDockingActivo", agenda.Id, crossDocking.Preparacion));

                var cliente = context.GetCliente(crossDocking.TipoAgente, crossDocking.CodigoAgente, crossDocking.Empresa);
                if (cliente == null)
                    errors.Add(new Error("WMSAPI_msg_Error_AgenteNoExite", crossDocking.CodigoAgente, crossDocking.TipoAgente, crossDocking.Empresa));
                else
                    crossDocking.Cliente = cliente.Codigo;
            }

            return agenda;
        }

        public virtual void ValidarUbicacionPuerta(CrossDockingUnaFase crossDocking, ICrossDockingServiceContext context, List<Error> errors, Agenda agenda)
        {
            if (agenda.CodigoPuerta != null)
            {
                var puertaAgenda = context.GetPuerta(agenda.CodigoPuerta.Value);
                if (crossDocking.Ubicacion != puertaAgenda.CodigoUbicacion)
                    errors.Add(new Error("WMSAPI_msg_Error_LaUbicacionNoEsLaEspecificadaParaLaAgenda", crossDocking.Ubicacion));
                else
                    crossDocking.CodigoPuerta = puertaAgenda.Id;
            }
            else
            {
                var puertaAgenda = context.GetPuerta(crossDocking.Ubicacion);
                if (puertaAgenda == null)
                    errors.Add(new Error("WMSAPI_msg_Error_LaUbicacionPerteneceAUnaPuerta", crossDocking.Ubicacion));
                else
                    crossDocking.CodigoPuerta = puertaAgenda.Id;
            }
        }

        public virtual void ValidarDetalleProduto(CrossDockingUnaFase crossDocking, ICrossDockingServiceContext context, List<Error> errors)
        {
            Producto prod = context.GetProducto(crossDocking.Empresa, crossDocking.Producto);
            if (prod == null)
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", crossDocking.Producto, crossDocking.Empresa));
            else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", crossDocking.Producto, crossDocking.Empresa));
            else if (!prod.AceptaDecimales)
            {
                if (crossDocking.Cantidad != Math.Truncate(crossDocking.Cantidad))
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", crossDocking.Producto));
            }

            if (prod.ManejoIdentificador == ManejoIdentificador.Serie && crossDocking.Identificador != ManejoIdentificadorDb.IdentificadorAuto && crossDocking.Cantidad != 1)
                errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
        }

        #endregion

        #region Tracking

        public virtual Task<List<Error>> ValidarPuntoEntregaAgente(PuntoEntregaAgentes puntoEntrega, IPuntoEntregaServiceContext context)
        {
            List<Error> errors = new List<Error>();
            HashSet<string> keysAgentes = new HashSet<string>();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                ValidarCampo("CodigoPuntoEntrega", puntoEntrega.CodigoPuntoEntrega, true, typeof(string), 20, errors);
                ValidarCampo("Zona", puntoEntrega.Zona, true, typeof(string), 10, errors);

                if (context.RutaZona == null)
                    errors.Add(new Error("TRK_msg_Error_ZonaSinRutaAsociada", puntoEntrega.Zona));

                foreach (var agente in puntoEntrega.Agentes)
                {
                    var keyAgente = $"{agente.Tipo}.{agente.Codigo}.{agente.CodigoEmpresa}";
                    if (keysAgentes.Contains(keyAgente))
                    {
                        errors.Add(new Error("TRK_msg_Error_AgentesDuplicados", agente.Codigo, agente.Tipo, agente.CodigoEmpresa));
                        break;
                    }
                    else
                        keysAgentes.Add(keyAgente);

                    ValidarTipoAgente(agente.Tipo, context.ExisteTipoAgente, errors);
                    ValidarEmpresa(agente.CodigoEmpresa, context.ExisteEmpresa, errors);
                    ValidarAgente(agente.Codigo, agente.CodigoEmpresa, agente.Tipo, t => context.GetAgente(t.Item1, t.Item2, t.Item3), errors, out string cdCliente);
                }

            }
            return Task.FromResult(errors);
        }

        public virtual Task<List<Error>> ValidarRutaZona(Ruta ruta, bool nuevaRuta)
        {
            List<Error> errors = new List<Error>();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                ValidarCampo("Zona", ruta.Zona, true, typeof(string), 10, errors);
                ValidarCampo("Descripcion", ruta.Descripcion, true, typeof(string), 30, errors);

                var existeRuta = uow.RutaRepository.AnyRutaZona(ruta.Zona);

                if (existeRuta && nuevaRuta)
                    errors.Add(new Error("TRK_msg_Error_ZonaSinRutaAsociada", ruta.Zona));
                else if (!existeRuta && !nuevaRuta)
                    errors.Add(new Error("TRK_msg_Error_ZonConRutaAsociadaExistente", ruta.Zona));

            }
            return Task.FromResult(errors);
        }

        #endregion

        #region Produccion

        public virtual bool ProduccionValidacionCarga(IngresoProduccion ingreso, IProduccionServiceContext context, List<Error> errors)
        {
            var insumos = new HashSet<string>();
            var productosFinales = new HashSet<string>();

            ValidarCampo("IdProduccionExterno", ingreso.IdProduccionExterno, false, typeof(string), 100, errors);
            ValidarCampo("Tipo", ingreso.Tipo, true, typeof(string), 20, errors);
            ValidarCampo("EspacioProduccion", ingreso.IdEspacioProducion, false, typeof(string), 10, errors);
            ValidarCampo("Predio", ingreso.Predio, true, typeof(string), 10, errors);
            ValidarCampo("CodigoFormula", ingreso.IdFormula, false, typeof(string), 10, errors);
            ValidarCampo("CantidadFormula", ingreso.CantidadIteracionesFormula.ToString(), !string.IsNullOrEmpty(ingreso.IdFormula), typeof(int), 6, errors, distintoCero: !string.IsNullOrEmpty(ingreso.IdFormula));
            ValidarCampo("IdModalidadLote", ingreso.IdModalidadLote, false, typeof(string), 20, errors);
            ValidarCampo("Anexo1", ingreso.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo("Anexo2", ingreso.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo("Anexo3", ingreso.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo("Anexo4", ingreso.Anexo4, false, typeof(string), 200, errors);
            ValidarCampo("Anexo5", ingreso.Anexo5, false, typeof(string), 200, errors);
            ValidarCampo("TipoDeFlujo", ingreso.TipoDeFlujo, false, typeof(string), 40, errors);

            if (ingreso.Tipo == TipoIngresoProduccion.Colector && !ingreso.GeneraPedido)
                errors.Add(new Error("WMSAPI_msg_Error_IngresoConTipoColectorNoPuedeNoGenerarPedido"));
            else if (!string.IsNullOrEmpty(ingreso.IdFormula))
            {
                if (ingreso.Detalles.Count() > 0)
                    errors.Add(new Error("WMSAPI_msg_Error_IngresoConFormulaNoEnviarDetalles"));
            }
            else if (ingreso.Detalles.Count() == 0)
                errors.Add(new Error("WMSAPI_msg_Error_IngresoSinDetalles", ingreso.IdProduccionExterno));
            else if (!ingreso.Detalles.Any(i => i.Tipo == CIngresoProduccionDetalleTeorico.TipoDetalleEntrada))
                errors.Add(new Error("WMSAPI_msg_Error_IngresoSinInsumos", ingreso.IdProduccionExterno));

            if (errors.Any())
                return false;

            foreach (var detalle in ingreso.Detalles)
            {
                ValidarCampo("CodigoProducto", detalle.Producto, true, typeof(string), 40, errors);
                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);
                ValidarCampo("CantidadTeorica", detalle.CantidadTeorica?.ToString(), true, typeof(decimal), 12, errors, 3);
                ValidarCampo("Anexo1", detalle.Anexo1, false, typeof(string), 200, errors);
                ValidarCampo("Anexo2", detalle.Anexo2, false, typeof(string), 200, errors);
                ValidarCampo("Anexo3", detalle.Anexo3, false, typeof(string), 200, errors);
                ValidarCampo("Anexo4", detalle.Anexo4, false, typeof(string), 200, errors);

                if (errors.Any())
                    return false;

                var prod = context.GetProducto(detalle.Producto);
                if (prod != null)
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    else if (string.IsNullOrEmpty(detalle.Identificador))
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorAuto;

                    if (detalle.Tipo == CIngresoProduccionDetalleTeorico.TipoDetalleEntrada)
                    {
                        var key = $"{detalle.Producto}.{detalle.Identificador}";
                        var keyAuto = $"{detalle.Producto}.{ManejoIdentificadorDb.IdentificadorAuto}";

                        if (insumos.Contains(key))
                            errors.Add(new Error("WMSAPI_msg_Error_InsumosDuplicados", ingreso.IdProduccionExterno, detalle.Producto, detalle.Identificador));
                        else if (insumos.Contains(keyAuto))
                            errors.Add(new Error("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido"));
                        else if (detalle.Identificador == ManejoIdentificadorDb.IdentificadorAuto && insumos.Any(i => i.Contains(detalle.Producto)))
                            errors.Add(new Error("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido"));
                        else
                            insumos.Add(key);
                    }
                    else
                    {
                        if (productosFinales.Contains(detalle.Producto))
                            errors.Add(new Error("WMSAPI_msg_Error_ProductosFinalesDuplicados", ingreso.IdProduccionExterno, detalle.Producto));
                        else
                            productosFinales.Add(detalle.Producto);
                    }
                }

                if (errors.Any())
                    return false;
            }

            return true;
        }

        public virtual bool ProduccionValidacionProcedimiento(IngresoProduccion ingreso, IProduccionServiceContext context, List<Error> errors)
        {
            var manejaDocumental = (context.GetParametro(ParamManager.MANEJO_DOCUMENTAL) ?? "N") == "S";

            if (!string.IsNullOrEmpty(ingreso.IdProduccionExterno) && context.ExisteIdProduccionExternoEmpresa(ingreso.IdProduccionExterno, ingreso.Empresa.Value))
            {
                errors.Add(new Error("WMSAPI_msg_Error_IdProduccionExternoExiste", ingreso.IdProduccionExterno, ingreso.Empresa.Value));
                return false;
            }

            ValidarPredio(ingreso.Predio, context.ExistePredio, errors);

            if (!string.IsNullOrEmpty(ingreso.IdModalidadLote) && !context.ExisteModalidadLote(ingreso.IdModalidadLote.ToUpper()))
            {
                errors.Add(new Error("WMSAPI_msg_Error_IdModalidadLoteNoExiste", ingreso.IdModalidadLote));
                return false;
            }

            if (!ingreso.GeneraPedido && (ingreso.LiberarPedido != null && ingreso.LiberarPedido == true))
            {
                errors.Add(new Error("WMSAPI_msg_Error_NoPuedeLiberarSiNoGenera", ingreso.IdProduccionExterno));
                return false;
            }

            if (!context.ExisteTipoIngreso(ingreso.Tipo.ToUpper()))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoIngresoNoExiste", ingreso.Tipo));
                return false;
            }
            else if (manejaDocumental && ingreso.Tipo == TipoIngresoProduccion.BlackBox)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoIngresoNoHabilitadoDocumental", ingreso.Tipo));
                return false;
            }

            if (!string.IsNullOrEmpty(ingreso.IdEspacioProducion))
            {
                var espacio = context.GetEspacioProduccion(ingreso.IdEspacioProducion);
                if (espacio == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_EspacioProduccionNoExiste", ingreso.IdEspacioProducion));
                    return false;
                }
                else if (espacio.Predio != ingreso.Predio)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_EspacioDistintoPredioProduccion", ingreso.IdEspacioProducion));
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(ingreso.IdFormula))
            {
                var formula = context.GetFormula(ingreso.IdFormula);
                if (formula == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_FormulaNoExiste", ingreso.IdFormula));
                    return false;
                }
                else if (formula.Empresa != ingreso.Empresa.Value)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_FormulaDistintaEmpresa", ingreso.IdFormula));
                    return false;
                }
                else if (ingreso.CantidadIteracionesFormula < 1)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_FormulaCantidadPasadasMayorCero"));
                    return false;
                }

                ingreso.Formula = formula;
            }
            else
                ingreso.CantidadIteracionesFormula = 0;

            if (errors.Any())
                return false;

            foreach (var detalle in ingreso.Detalles)
            {
                var prod = context.GetProducto(detalle.Producto);
                if (prod == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", detalle.Producto, detalle.Empresa));
                    return false;
                }
                else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", detalle.Producto, detalle.Empresa));
                    return false;
                }
                else if (!prod.AceptaDecimales)
                {
                    if (detalle.CantidadTeorica.HasValue && detalle.CantidadTeorica.Value != Math.Truncate(detalle.CantidadTeorica.Value))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                        return false;
                    }
                }

                if (prod.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto && detalle.CantidadTeorica != 1)
                    errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));

                if (detalle.Tipo == CIngresoProduccionDetalleTeorico.TipoDetalleEntrada && !prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ProducirProduccionValidacionCarga(ProducirProduccion produccion, IProducirProduccionServiceContext context, List<Error> errors)
        {
            var productos = new HashSet<string>();

            ValidarCampo("IdProduccionExterno", produccion.IdProduccionExterno, false, typeof(string), 100, errors);
            ValidarCampo("IdEspacio", produccion.IdEspacio, false, typeof(string), 10, errors);
            ValidarCampo("ConfirmarMovimiento", produccion.ConfirmarMovimiento.ToString(), true, typeof(bool), -1, errors);
            ValidarCampo("FinalizarProduccion", produccion.FinalizarProduccion.ToString(), true, typeof(bool), -1, errors);

            if (string.IsNullOrEmpty(produccion.IdProduccionExterno) && string.IsNullOrEmpty(produccion.IdEspacio))
                errors.Add(new Error("WMSAPI_msg_Error_ProducirSinProduccionEspacio"));

            if (produccion.Productos.Count() == 0)
                errors.Add(new Error("WMSAPI_msg_Error_ProduccionSinProductos", produccion.IdProduccionExterno));

            if (errors.Any())
                return false;

            foreach (var detalle in produccion.Productos)
            {
                ValidarCampo("Producto", detalle.Producto, true, typeof(string), 40, errors);
                ValidarCampo("Ubicacion", detalle.Ubicacion, false, typeof(string), 40, errors);
                ValidarCampo("Vencimiento", detalle.Vencimiento?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
                ValidarCampo("Cantidad", detalle.Cantidad.ToString(), true, typeof(decimal), 12, errors, 3);
                ValidarCampo("Motivo", detalle.Motivo, true, typeof(string), 20, errors);
                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);

                ValidarFechaMenorA("Vencimiento", detalle.Vencimiento?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

                if (productos.Contains($"{detalle.Producto}.{detalle.Identificador}.{detalle.Motivo}"))
                    errors.Add(new Error("WMSAPI_msg_Error_ProductosProducirDuplicados", detalle.Producto, detalle.Identificador, detalle.Motivo));
                else
                    productos.Add($"{detalle.Producto}.{detalle.Identificador}.{detalle.Motivo}");

                if (errors.Any())
                    return false;
            }

            return true;
        }

        public virtual bool ProducirProduccionValidacionProcedimiento(ProducirProduccion produccion, IProducirProduccionServiceContext context, List<Error> errors)
        {
            var ingreso = context.GetIngreso();
            var espacio = context.GetEspacioProduccion();
            var produccionEspacioActiva = context.GetProduccionEspacioActiva();

            if (!string.IsNullOrEmpty(produccion.IdEspacio) && produccionEspacioActiva == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_NoExisteElEspacio"));
                return false;
            }
            else if (produccionEspacioActiva != null && produccionEspacioActiva.NumeroIngreso == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_NoHayIngresoActivoEnEspacio"));
                return false;
            }

            if (ingreso == null || ingreso.Empresa != produccion.Empresa)
            {
                errors.Add(new Error("WMSAPI_msg_Error_IngresoIncorrecto", produccion.IdProduccionExterno, produccion.Empresa));
                return false;
            }

            if (ingreso.Tipo != TipoIngresoProduccion.BlackBox)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoIngresoIncorrecto", produccion.IdProduccionExterno));
                return false;
            }

            if (!SituacionDb.SITUACIONES_PRODUCCION_ACTIVA.Contains(ingreso.Situacion ?? -1))
            {
                errors.Add(new Error("WMSAPI_msg_Error_SituacionNoPermiteProducir", produccion.IdProduccionExterno));
                return false;
            }
            else if (produccion.ConfirmarMovimiento && context.LogicaProduccion.ProduccionEnProcesoDeNotificacion())
            {
                errors.Add(new Error("PRD113_grid1_Error_NotificacionEnProceso"));
                return false;
            }
            if ((espacio != null && produccionEspacioActiva != null) && espacio.Id != produccionEspacioActiva.Id)
            {
                errors.Add(new Error("WMSAPI_msg_Error_EspacioIncorrecto", produccion.IdProduccionExterno, produccionEspacioActiva.Id));
                return false;
            }
            foreach (var detalle in produccion.Productos)
            {
                detalle.NuPrdcIngreso = ingreso.Id;

                if (string.IsNullOrEmpty(detalle.Ubicacion))
                    detalle.Ubicacion = context.GetEspacioProduccion().IdUbicacionProduccion;
                else if (detalle.Ubicacion != espacio.IdUbicacionProduccion)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionProduccionNoCoincide", detalle.Ubicacion, espacio.Id, espacio.Descripcion));
                    return false;
                }

                var producto = context.GetProducto(produccion.Empresa, detalle.Producto);

                if (!ValidarProducto(detalle.Producto, produccion.Empresa, producto, errors, context.PermiteProductoInactivos))
                    return false;

                if (string.IsNullOrEmpty(detalle.Identificador))
                {
                    if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    else
                        ValidarCampo("Identificador", detalle.Identificador, true, typeof(string), 40, errors);
                }
                else if (producto.ManejoIdentificador == ManejoIdentificador.Producto && detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.Producto));
                else if (producto.ManejoIdentificador != ManejoIdentificador.Producto && (detalle.Identificador == ManejoIdentificadorDb.IdentificadorProducto || detalle.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", detalle.Producto));

                if (producto.TipoManejoFecha == ManejoFechaProductoDb.Expirable && detalle.Vencimiento == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaVencimiento", detalle.Producto));
                    return false;
                }
                else if (producto.TipoManejoFecha == ManejoFechaProductoDb.Fifo)
                    detalle.Vencimiento = DateTime.Now;
                else if (producto.TipoManejoFecha == ManejoFechaProductoDb.Duradero)
                    detalle.Vencimiento = null;

                if (detalle.Cantidad <= 0)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CantidadProducirIncorrecta", detalle.Producto));
                    return false;
                }

                if (!producto.AceptaDecimales)
                {
                    if (detalle.Cantidad != Math.Truncate(detalle.Cantidad))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                        return false;
                    }
                }

                if (producto.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                {
                    if (detalle.Cantidad != 1)
                    {
                        errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                        return false;
                    }
                    else if (context.ExisteSerie(producto.Codigo, detalle.Identificador))
                    {
                        errors.Add(new Error("General_Sec0_Error_SerieYaExiste", detalle.Identificador, producto.Codigo, produccion.Empresa));
                        return false;
                    }
                }

                if (!producto.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                    return false;
                }

                if (!context.ExisteMotivo(detalle.Motivo))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_MotivoProduccionNoExiste", detalle.Motivo));
                    return false;
                }
            }

            return true;
        }

        public virtual bool ConsumirProduccionValidacionCarga(ConsumirProduccion consumo, IConsumirProduccionServiceContext context, List<Error> errors)
        {
            var insumos = new HashSet<string>();

            ValidarCampo("IdProduccionExterno", consumo.IdProduccionExterno, false, typeof(string), 100, errors);
            ValidarCampo("IdEspacio", consumo.IdEspacio, false, typeof(string), 10, errors);
            ValidarCampo("ConfirmarMovimiento", consumo.ConfirmarMovimiento.ToString(), true, typeof(bool), -1, errors);
            ValidarCampo("FinalizarProduccion", consumo.FinalizarProduccion.ToString(), true, typeof(bool), -1, errors);
            ValidarCampo("IniciarProduccion", consumo.IniciarProduccion.ToString(), true, typeof(bool), -1, errors);

            if (string.IsNullOrEmpty(consumo.IdProduccionExterno) && string.IsNullOrEmpty(consumo.IdEspacio))
                errors.Add(new Error("WMSAPI_msg_Error_ConsumoSinProduccionEspacio"));

            if (!consumo.IniciarProduccion && consumo.Insumos.Count() == 0)
                errors.Add(new Error("WMSAPI_msg_Error_ConsumoSinInsumos", consumo.IdProduccionExterno));

            if (errors.Any())
                return false;

            foreach (var detalle in consumo.Insumos)
            {
                ValidarCampo("Producto", detalle.Producto, true, typeof(string), 40, errors);
                ValidarCampo("Ubicacion", detalle.Ubicacion, false, typeof(string), 40, errors);
                ValidarCampo("Referencia", detalle.Referencia, false, typeof(string), 200, errors);
                ValidarCampo("Cantidad", detalle.Cantidad.ToString(), true, typeof(decimal), 12, errors, 3);
                ValidarCampo("Motivo", detalle.Motivo, true, typeof(string), 20, errors);
                ValidarCampo("UsarSoloReserva", detalle.UsarSoloReserva.ToString(), true, typeof(bool), -1, errors);
                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);

                if (insumos.Contains($"{detalle.Producto}.{detalle.Identificador}.{detalle.Motivo}.{detalle.Referencia}"))
                    errors.Add(new Error("WMSAPI_msg_Error_InsumosConsumirDuplicados", detalle.Producto, detalle.Identificador, detalle.Motivo));
                else
                    insumos.Add($"{detalle.Producto}.{detalle.Identificador}.{detalle.Motivo}.{detalle.Referencia}");

                if (errors.Any())
                    return false;
            }

            return true;
        }

        public virtual bool ConsumirProduccionValidacionProcedimiento(ConsumirProduccion consumo, IConsumirProduccionServiceContext context, List<Error> errors)
        {
            var ingreso = context.GetIngreso();
            var espacio = context.GetEspacioProduccion();
            var produccionEspacioActiva = context.GetProduccionEspacioActiva();
            var cantidadOrdenesActivas = context.GetCantidadOrdenesActivas();

            if (!string.IsNullOrEmpty(consumo.IdEspacio) && produccionEspacioActiva == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_NoExisteElEspacio"));
                return false;
            }
            else if (string.IsNullOrEmpty(consumo.IdProduccionExterno) && !consumo.IniciarProduccion && produccionEspacioActiva != null && produccionEspacioActiva.NumeroIngreso == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_NoHayIngresoActivoEnEspacio"));
                return false;
            }

            if (ingreso == null || ingreso.Empresa != consumo.Empresa)
            {
                errors.Add(new Error("WMSAPI_msg_Error_IngresoIncorrecto", consumo.IdProduccionExterno, consumo.Empresa));
                return false;
            }

            if (consumo.IniciarProduccion && SituacionDb.SITUACIONES_PRODUCCION_ACTIVA.Contains(ingreso.Situacion ?? -1))
            {
                errors.Add(new Error("WMSAPI_msg_Error_IngresoYaActivo", ingreso.IdProduccionExterno));
                return false;
            }
            else if ((ingreso.Situacion ?? -1) == SituacionDb.PRODUCCION_FINALIZADA)
            {
                errors.Add(new Error("WMSAPI_msg_Error_SituacionNoPermiteConsumir", ingreso.IdProduccionExterno));
                return false;
            }
            else if (consumo.IniciarProduccion && ingreso.EspacioProduccion == null)
            {
                errors.Add(new Error("WMSAPI_msg_Error_IngresoSinEspacio", ingreso.IdProduccionExterno));
                return false;
            }
            else if (consumo.IniciarProduccion && string.IsNullOrEmpty(consumo.IdProduccionExterno) && string.IsNullOrEmpty(espacio.NumeroIngreso) && cantidadOrdenesActivas > 1)
            {
                errors.Add(new Error("WMSAPI_msg_Error_NoHayOrdenActivaEspacio"));
                return false;
            }

            if (ingreso.Tipo != TipoIngresoProduccion.BlackBox)
            {
                errors.Add(new Error("WMSAPI_msg_Error_TipoIngresoIncorrecto", consumo.IdProduccionExterno));
                return false;
            }

            if (!consumo.IniciarProduccion && !SituacionDb.SITUACIONES_PRODUCCION_ACTIVA.Contains(ingreso.Situacion ?? -1))
            {
                errors.Add(new Error("WMSAPI_msg_Error_SituacionNoPermiteConsumir", consumo.IdProduccionExterno));
                return false;
            }
            else if (consumo.ConfirmarMovimiento && context.LogicaProduccion.ProduccionEnProcesoDeNotificacion())
            {
                errors.Add(new Error("PRD113_grid1_Error_NotificacionEnProceso"));
                return false;
            }

            if ((espacio != null && produccionEspacioActiva != null) && espacio.Id != produccionEspacioActiva.Id)
            {
                errors.Add(new Error("WMSAPI_msg_Error_EspacioIncorrecto", consumo.IdProduccionExterno, produccionEspacioActiva.Id));
                return false;
            }

            foreach (var detalle in consumo.Insumos)
            {
                detalle.NuPrdcIngreso = ingreso.Id;

                if (string.IsNullOrEmpty(detalle.Ubicacion))
                    detalle.Ubicacion = espacio.IdUbicacionProduccion;
                else if (detalle.Ubicacion != espacio.IdUbicacionProduccion)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionProduccionNoCoincide", detalle.Ubicacion, espacio.Id, espacio.Descripcion));
                    return false;
                }

                var producto = context.GetProducto(consumo.Empresa, detalle.Producto);

                if (!ValidarProducto(detalle.Producto, consumo.Empresa, producto, errors, context.PermiteProductoInactivos))
                    return false;

                if (string.IsNullOrEmpty(detalle.Identificador))
                {
                    if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    else
                        ValidarCampo("Identificador", detalle.Identificador, true, typeof(string), 40, errors);
                }
                else if (producto.ManejoIdentificador == ManejoIdentificador.Producto && detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.Producto));
                else if (producto.ManejoIdentificador != ManejoIdentificador.Producto && (detalle.Identificador == ManejoIdentificadorDb.IdentificadorProducto || detalle.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", detalle.Producto));

                if (detalle.Cantidad <= 0)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CantidadConsumirIncorrecta", detalle.Producto));
                    return false;
                }

                if (!producto.AceptaDecimales)
                {
                    if (detalle.Cantidad != Math.Truncate(detalle.Cantidad))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                        return false;
                    }
                }

                if (producto.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto && detalle.Cantidad != 1)
                    errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));

                if (!context.ExisteMotivo(detalle.Motivo))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_MotivoConsumoNoExiste", detalle.Motivo));
                    return false;
                }

                var ajusteDeStock = (detalle.Motivo == TipoIngresoProduccion.MOT_CONS_ADS);

                if (ajusteDeStock && detalle.UsarSoloReserva)
                    errors.Add(new Error("WMSAPI_msg_Error_AjusteNoPermitidoSobreIngresosReales", detalle.Motivo));

                if (errors.Any())
                    return false;

                var tieneReferencia = !detalle.Referencia.IsNullOrEmpty();

                var insumosDisponibles = context.DetallesInsumos
                    .Where(d => d.Empresa == detalle.Empresa
                       && d.Producto == detalle.Producto
                       && d.Faixa == detalle.Faixa
                       && d.Identificador == detalle.Identificador
                       && (tieneReferencia ? d.Referencia == detalle.Referencia : true)
                       && (ajusteDeStock ? d.Consumible == "S" : (detalle.UsarSoloReserva ? d.Consumible == "N" : true)))
                    .ToList();

                if (insumosDisponibles.Count == 0)
                {
                    var error = new Error("WMSAPI_msg_Error_IngresoRealNoExiste", detalle.Producto, detalle.Identificador, consumo.IdProduccionExterno);
                    if (tieneReferencia)
                    {
                        if (ajusteDeStock)
                            error = new Error("WMSAPI_msg_Error_IngresoRealReferenciaConsumibleNoExiste", detalle.Producto, detalle.Identificador, detalle.Referencia, consumo.IdProduccionExterno);
                        else
                            error = new Error("WMSAPI_msg_Error_IngresoRealReferenciaNoExiste", detalle.Producto, detalle.Identificador, detalle.Referencia, consumo.IdProduccionExterno);
                    }
                    else if (ajusteDeStock)
                        error = new Error("WMSAPI_msg_Error_IngresoRealConsumibleNoExiste", detalle.Producto, detalle.Identificador, consumo.IdProduccionExterno);

                    errors.Add(error);
                    return false;
                }
                else if (detalle.Cantidad > insumosDisponibles.Sum(d => d.QtReal))
                {
                    var error = new Error("WMSAPI_msg_Error_CantidadDisponibleInsuficiente", detalle.Producto, detalle.Identificador);
                    if (tieneReferencia)
                        error = new Error("WMSAPI_msg_Error_CantidadDisponibleInsuficienteRef", detalle.Producto, detalle.Identificador, detalle.Referencia);

                    errors.Add(error);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Ubicacion

        public virtual bool UbicacionValidacionCarga(UbicacionExterna ubicacion, IUbicacionServiceContext context, List<Error> errors)
        {
            var configuracion = context.UbicacionConfiguracion;

            if (string.IsNullOrEmpty(ubicacion.IdUbicacionZona))
                ubicacion.IdUbicacionZona = configuracion.UbicacionZonaPorDefecto;

            ValidarCampo("Ubicación", ubicacion.Id, true, typeof(string), 40, errors);
            ValidarCampo("Cód. Empresa", ubicacion.CodigoEmpresa, true, typeof(int), 10, errors, distintoCero: false);
            ValidarCampo("Cód. Tipo", ubicacion.CodigoTipoUbicacion, true, typeof(short), 2, errors, distintoCero: false);
            ValidarCampo("Cód. Rotatividad", ubicacion.CodigoRotatividad, true, typeof(short), 2, errors, distintoCero: false);
            ValidarCampo("Cód. Familia", ubicacion.CodigoFamilia, true, typeof(int), 10, errors, distintoCero: false);
            ValidarCampo("Cód. Clase", ubicacion.CodigoClase, true, typeof(string), 2, errors);
            ValidarCampo("CodigoSituacion", ubicacion.CodigoSituacion.ToString(), true, typeof(short), 3, errors);
            ValidarCampo("IdEsUbicacionSeparacion", ubicacion.IdEsUbicacionSeparacion, false, typeof(string), 1, errors);
            ValidarCampo("IdNecesitaReabastecer", ubicacion.IdNecesitaReabastecer, true, typeof(string), 1, errors);
            ValidarCampo("Control", ubicacion.CodigoControl, false, typeof(string), 5, errors);
            ValidarCampo("Cód. Área", ubicacion.CodigoArea, true, typeof(short), 2, errors);
            ValidarCampo("Componente facturación", ubicacion.FacturacionComponente, false, typeof(string), 4, errors);
            ValidarCampo("Cód. Zona", ubicacion.IdUbicacionZona, true, typeof(string), 20, errors);
            ValidarCampo("Cód. Control Acceso", ubicacion.CodigoControlAcceso, false, typeof(string), 20, errors);

            var tipoBloque = configuracion.BloqueNumerico ? typeof(int) : typeof(string);

            if (!configuracion.BloqueNumerico && int.TryParse(ubicacion.Bloque, out int a))
                errors.Add(new Error("WMSAPI_msg_Error_NoAceptaNumeros", "Bloque"));

            ValidarCampo("Bloque", ubicacion.Bloque, true, tipoBloque, configuracion.BloqueLargo, errors);

            var tipoCalle = configuracion.CalleNumerico ? typeof(int) : typeof(string);

            if (!configuracion.CalleNumerico && int.TryParse(ubicacion.Calle, out int b))
                errors.Add(new Error("WMSAPI_msg_Error_NoAceptaNumeros", "Calle"));

            ValidarCampo("Calle", ubicacion.Calle, true, tipoCalle, configuracion.CalleLargo, errors);
            ValidarCampo("Columna", ubicacion.NuColumna, true, typeof(int), configuracion.ColumnaLargo, errors, distintoCero: false);
            ValidarCampo("Altura", ubicacion.NuAltura, true, typeof(int), configuracion.AlturaLargo, errors, distintoCero: false);

            ValidarCampo("DominioSector", ubicacion.DominioSector, false, typeof(string), 20, errors);
            ValidarCampo("Profundidad", ubicacion.NuProfundidad, true, typeof(int), 10, errors);
            ValidarCampo("Orden por defecto", ubicacion.OrdenDefecto.ToString(), true, typeof(long), 15, errors);

            ValidarCampo("Código barras", ubicacion.CodigoBarras, true, typeof(string), 100, errors);

            ValidarCodigoUbicacionExterna(ubicacion.Id, context, errors);
            ValidarCodigoBarrasUbicacionExterna(ubicacion.CodigoBarras, context, errors);

            ValidarCampo("Ub.Baja", ubicacion.IdUbicacionBaja, true, typeof(string), 1, errors);
            ValidarSoN("Ub.Baja", ubicacion.IdUbicacionBaja, errors);

            if (!errors.Any())
            {
                ubicacion.IdEmpresa = int.Parse(ubicacion.CodigoEmpresa);
                ubicacion.IdUbicacionTipo = short.Parse(ubicacion.CodigoTipoUbicacion);
                ubicacion.IdProductoRotatividad = short.Parse(ubicacion.CodigoRotatividad);
                ubicacion.IdProductoFamilia = int.Parse(ubicacion.CodigoFamilia);
                ubicacion.IdUbicacionArea = short.Parse(ubicacion.CodigoArea);
                ubicacion.Columna = int.Parse(ubicacion.NuColumna);
                ubicacion.Altura = int.Parse(ubicacion.NuAltura);
                ubicacion.Profundidad = int.Parse(ubicacion.NuProfundidad);
                ubicacion.IdControlAcceso = ubicacion.CodigoControlAcceso;
            }
            return true;
        }

        public virtual bool ValidarCodigoUbicacionExterna(string id, IServiceContext context, List<Error> errors)
        {
            return true;
        }

        public virtual bool ValidarCodigoBarrasUbicacionExterna(string cdBarras, IServiceContext context, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(cdBarras))
            {
                string caracteresPermitidos = context.GetParametro(ParamManager.LISTA_CARACTERES_COD_BARRA);

                if (!Validations.ValidarCaracteres("Código barras", cdBarras, caracteresPermitidos, out Error error))
                {
                    errors.Add(error);
                    return false;
                }
            }
            return true;
        }

        public virtual bool UbicacionValidacionProcedimiento(UbicacionExterna registro, IUbicacionServiceContext context, List<Error> errors)
        {
            if (context.ExisteUbicacion(registro.Id))
            {
                errors.Add(new Error("REG040_msg_Error_UbicacionExistente", registro.Id));
                return false;
            }

            if (!context.ExisteEmpresa(registro.IdEmpresa))
            {
                errors.Add(new Error("REG040_msg_Error_EmpresaInexistenteONoAsignada", registro.IdEmpresa));
                return false;
            }

            if (!context.ExisteZona(registro.IdUbicacionZona))
            {
                errors.Add(new Error("REG040_msg_Error_ZonaInexistente", registro.IdUbicacionZona));
                return false;
            }

            if (!context.ExisteAreaUbicacion(registro.IdUbicacionArea))
            {
                errors.Add(new Error("REG040_msg_Error_AreaInexistente", registro.IdUbicacionArea));
                return false;
            }

            if (!context.AreaUbicacionEsMantenible(registro.IdUbicacionArea))
            {
                errors.Add(new Error("REG040_msg_Error_AreaNoMantenible", registro.IdUbicacionArea));
                return false;
            }

            registro.IsRecorrible = context.AreaUbicacionEsRecorrible(registro.IdUbicacionArea);

            if (!context.ExisteTipoUbicacion(registro.IdUbicacionTipo))
            {
                errors.Add(new Error("REG040_msg_Error_TipoUbicacionInexistente", registro.IdUbicacionTipo));
                return false;
            }

            if (!context.ExisteClase(registro.CodigoClase))
            {
                errors.Add(new Error("REG040_msg_Error_ClaseInexistente", registro.CodigoClase));
                return false;
            }

            if (!context.ExisteRotatividad(registro.IdProductoRotatividad))
            {
                errors.Add(new Error("REG040_msg_Error_RotatividadInexistente", registro.IdProductoRotatividad));
                return false;
            }

            if (!context.ExisteFamilia(registro.IdProductoFamilia))
            {
                errors.Add(new Error("REG040_msg_Error_FamiliaProductoInexistente", registro.IdProductoFamilia));
                return false;
            }

            if (!context.ExistePredio(registro.NumeroPredio))
            {
                errors.Add(new Error("REG040_msg_Error_PredioInexistente", registro.NumeroPredio));
                return false;
            }

            if (context.ExisteNumeroOrden((long)registro.OrdenDefecto))
            {
                errors.Add(new Error("REG040_msg_Error_OrdenRepetido", registro.OrdenDefecto));
                return false;
            }

            if (!context.CodigoDeBarrasEsUnicoParaPredio(registro.CodigoBarras, registro.NumeroPredio))
            {
                errors.Add(new Error("REG040_msg_Error_CodigoBarrasRepetido", registro.CodigoBarras, registro.NumeroPredio));
                return false;
            }

            if (context.IdContienePrefijosWis(registro.Id, registro.NumeroPredio))
            {
                errors.Add(new Error("REG040_msg_Error_IDTienePrefijosWis", registro.Id));
                return false;
            }

            if (context.CodigoBarrasContienePrefijosWis(registro.CodigoBarras))
            {
                errors.Add(new Error("REG040_msg_Error_CodigoBarrasTienePrefijoWIS", registro.CodigoBarras));
                return false;
            }

            if (context.ExisteOrdenPorDefecto(registro.OrdenDefecto ?? -1))
            {
                errors.Add(new Error("REG040_msg_Error_OrdenExistenteEnRecorrido", registro.OrdenDefecto, context.RecorridoPorDefecto.Id));
                return false;
            }

            return true;
        }

        #endregion

        #region Recorridos
        public virtual bool DetalleRecorridoValidacionCarga(DetalleRecorrido detalleRecorrido, IRecorridoServiceContext context, List<Error> errors)
        {
            var ordenRequerido = detalleRecorrido.TipoOperacion == RecorridosConstants.TP_OPERACION_ALTA;

            ValidarCampo("Ubicación", detalleRecorrido.Ubicacion, true, typeof(string), 40, errors);
            ValidarCampo("Orden", detalleRecorrido.NuOrden, ordenRequerido, typeof(long), 15, errors, distintoCero: false);
            ValidarCampo("Tipo Operación", detalleRecorrido.TipoOperacion, true, typeof(string), 1, errors, distintoCero: false);

            ValidarTipoOperacion(detalleRecorrido.TipoOperacion, false, errors);

            if (errors.Count == 0 && long.TryParse(detalleRecorrido.NuOrden, out long nroOrden))
                detalleRecorrido.NumeroOrden = nroOrden;

            return true;
        }

        public virtual bool DetalleRecorridoValidacionProcedimiento(DetalleRecorrido registro, IRecorridoServiceContext context, List<Error> errors)
        {
            if (context.UbicacionOperacionSeRepite(registro.Ubicacion, registro.TipoOperacion))
            {
                errors.Add(new Error("REG700_msg_Error_UbicacionDuplicada", registro.Ubicacion));
                return false;
            }

            if (registro.TipoOperacion == RecorridosConstants.TP_OPERACION_ALTA)
                return ValidarDetalleRecorridoPorIngresar(registro, context, errors);

            return ValidarDetalleRecorridoPorEliminar(registro, context, errors);
        }

        public virtual bool ValidarDetalleRecorridoPorIngresar(DetalleRecorrido registro, IRecorridoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteUbicacion(registro.Ubicacion))
            {
                errors.Add(new Error("REG700_msg_Error_UbicacionNoExistente", registro.Ubicacion));
                return false;
            }

            if (context.NumeroOrdenSeRepite((long)registro.NumeroOrden))
            {
                errors.Add(new Error("REG040_msg_Error_OrdenRepetido", registro.NumeroOrden));
                return false;
            }

            if (!context.UbicacionTienePredioCompatibleConRecorrido(registro.Ubicacion))
            {
                errors.Add(new Error("REG700_msg_Error_PredioUbicacionNoCompatibleConRecorrido", registro.Ubicacion));
                return false;
            }

            if (context.ExisteUbicacionEnRecorrido(registro.Ubicacion, RecorridosConstants.TP_OPERACION_ALTA))
            {
                errors.Add(new Error("REG700_msg_Error_UbicacionExisteEnRecorrido", registro.Ubicacion));
                return false;
            }

            if (context.ExisteNumeroOrden(registro.ValorOrden))
            {
                errors.Add(new Error("REG700_msg_Error_OrdenExisteEnRecorrido", registro.NumeroOrden));
                return false;
            }

            if (!context.AreaUbicacionEsRecorrible(registro.Ubicacion))
            {
                errors.Add(new Error("REG700_msg_Error_UbicacionNoRecorrible", registro.Ubicacion));
                return false;
            }

            return true;
        }

        public virtual bool ValidarDetalleRecorridoPorEliminar(DetalleRecorrido registro, IRecorridoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteUbicacionEnRecorrido(registro.Ubicacion, RecorridosConstants.TP_OPERACION_BAJA))
            {
                errors.Add(new Error("REG700_msg_Error_UbicacionNoExisteEnRecorrido", registro.Ubicacion));
                return false;
            }

            return true;
        }

        #endregion

        #region Control Calidad
        public virtual bool ValidateControlCalidadCarga(ControlCalidadAPI control, List<Error> errors)
        {
            this.ValidarCampo(
                "CodigoControlCalidad",
                control.CodigoControlCalidad.ToString(),
                true,
                typeof(int),
                10,
                errors,
                distintoCero: false);

            if (control.Estado == ControlCalidadOperacion.Indefinido)
            {
                errors.Add(new Error("WMSAPI_msg_Error_EstadoNoIdentificado"));
                return false;
            }

            this.ValidarCampo("Descripcion", control.Descripcion, false, typeof(string), 200, errors);

            int criterioIndex = 0;
            while (criterioIndex < control.Criterios.Count && !errors.Any())
            {
                CriterioControlCalidadAPI criterio = control.Criterios[criterioIndex];
                this.ValidateCriterioCarga(criterio, errors);
                criterioIndex++;
            }

            return !errors.Any();
        }

        public virtual void ValidateCriterioCarga(CriterioControlCalidadAPI criterio, List<Error> errors)
        {
            this.ValidarCampo("Predio", criterio.Predio, true, typeof(string), 10, errors);
            this.ValidarCampo("Producto", criterio.Producto, true, typeof(string), 40, errors);
            this.ValidarCampo("Lote", criterio.Lote, true, typeof(string), 10, errors);
            this.ValidarCampo("Ubicacion", criterio.Ubicacion, false, typeof(string), 40, errors);
            this.ValidarCampo("TipoEtiqueta", criterio.TipoEtiquetaExterna, false, typeof(string), 10, errors);
            this.ValidarCampo("Faixa", criterio.Faixa.ToString(), false, typeof(decimal?), 9, errors, distintoCero: false);

            this.ValidarCampo(
                "EtiquetaExterna",
                criterio.EtiquetaExterna,
                !string.IsNullOrEmpty(criterio.TipoEtiquetaExterna),
                typeof(string),
                50,
                errors);
        }

        public virtual bool ValidateControlCalidadProcedimiento(ControlCalidadAPI control, IControlCalidadServiceContext context, List<Error> errors)
        {
            if (!context.ExisteCodigoControl(control.CodigoControlCalidad))
            {
                errors.Add(new Error("WMSAPI_msg_Error_CodigoControlCalidadNoExiste", control.CodigoControlCalidad));
                return false;
            }

            int criterioIndex = 0;
            while (criterioIndex < control.Criterios.Count && !errors.Any())
            {
                CriterioControlCalidadAPI criterio = control.Criterios[criterioIndex];

                this.ValidateCriterioProcedimiento(
                    control.Criterios[criterioIndex],
                    control.Estado,
                    context,
                    control.Empresa,
                    control.CodigoControlCalidad,
                    errors);

                criterioIndex++;
            }

            return !errors.Any();
        }

        public virtual bool ValidateCriterioProcedimiento(CriterioControlCalidadAPI criterio, ControlCalidadOperacion operacion, IControlCalidadServiceContext context, int empresa, int codigoControl, List<Error> errors)
        {
            Lpn lpnEtiqueta = null;
            EtiquetaLote etiqueta = null;

            if (!context.ProductoExiste(criterio.Producto, criterio.Empresa))
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", criterio.Producto, empresa));
                return false;
            }

            if (!context.PredioExiste(criterio.Predio))
            {
                errors.Add(new Error("WMSAPI_msg_Error_PredioNoExiste", criterio.Predio));
                return false;
            }

            Producto producto = context.GetProducto(criterio.Producto, criterio.Empresa);

            bool manejaIdentificador =
                producto.ManejoIdentificador == ManejoIdentificador.Lote ||
                producto.ManejoIdentificador == ManejoIdentificador.Serie;

            bool tieneAsignadoLote =
                criterio.Lote != null &&
                criterio.Lote != ManejoIdentificadorDb.IdentificadorProducto &&
                criterio.Lote != ManejoIdentificadorDb.IdentificadorAuto;

            if (manejaIdentificador && !tieneAsignadoLote)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaLote", criterio.Producto));
                return false;
            }

            if (!manejaIdentificador && tieneAsignadoLote)
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", criterio.Producto));
                return false;
            }

            if (!string.IsNullOrEmpty(criterio.Ubicacion) && !context.UbicacionExiste(criterio.Ubicacion))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionNoExiste", criterio.Ubicacion));
                return false;
            }

            if (!string.IsNullOrEmpty(criterio.EtiquetaExterna))
            {
                bool esEtiquetaRecepcion = false;
                bool esEtiquetaLpn = false;

                if (string.IsNullOrEmpty(criterio.TipoEtiquetaExterna))
                {
                    esEtiquetaRecepcion = context.EtiquetaExternaExiste(
                        criterio.Predio,
                        criterio.EtiquetaExterna,
                        out var cantidadEtiquetas);

                    esEtiquetaLpn = context.LpnExternoExiste(
                        criterio.Predio,
                        criterio.EtiquetaExterna,
                        out cantidadEtiquetas);

                    if (!esEtiquetaRecepcion && !esEtiquetaLpn)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoExiste", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                        return false;
                    }

                    if (cantidadEtiquetas > 1)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EtiquetaRepetidaParaNumeroExterno", criterio.EtiquetaExterna));
                        return false;
                    }

                    if (esEtiquetaRecepcion && esEtiquetaLpn)
                    {
                        lpnEtiqueta = context.GetEtiquetaLpnExterno(criterio.Predio, criterio.EtiquetaExterna);
                        etiqueta = context.GetEtiquetaRecepcionExterno(criterio.Predio, criterio.EtiquetaExterna);
                        if (etiqueta.NroLpn != lpnEtiqueta.NumeroLPN)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_EtiquetaRepetidaParaNumeroExterno", criterio.EtiquetaExterna));
                            return false;
                        }
                    }
                    else
                    {
                        if (esEtiquetaLpn)
                            lpnEtiqueta = context.GetEtiquetaLpnExterno(criterio.Predio, criterio.EtiquetaExterna);
                        else
                            etiqueta = context.GetEtiquetaRecepcionExterno(criterio.Predio, criterio.EtiquetaExterna);
                    }
                }
                else if (!string.IsNullOrEmpty(criterio.TipoEtiquetaExterna))
                {
                    esEtiquetaRecepcion = context.EtiquetaExternaExiste(
                        criterio.Predio,
                        criterio.EtiquetaExterna,
                        criterio.TipoEtiquetaExterna);

                    esEtiquetaLpn = context.LpnExternoExiste(
                        criterio.Predio,
                        criterio.EtiquetaExterna,
                        criterio.TipoEtiquetaExterna);

                    if (!esEtiquetaRecepcion && !esEtiquetaLpn)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoExiste", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                        return false;
                    }

                    if (esEtiquetaRecepcion && esEtiquetaLpn)
                    {
                        lpnEtiqueta = context.GetEtiquetaLpnExterno(criterio.Predio, criterio.EtiquetaExterna);
                        etiqueta = context.GetEtiquetaRecepcionExterno(criterio.Predio, criterio.EtiquetaExterna);
                        if (etiqueta.NroLpn != lpnEtiqueta.NumeroLPN)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoExiste", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                            return false;
                        }
                    }
                    else
                    {
                        if (esEtiquetaLpn)
                            lpnEtiqueta = context.GetEtiquetaLpnExterno(
                                criterio.Predio,
                                criterio.EtiquetaExterna,
                                criterio.TipoEtiquetaExterna);
                        else
                            etiqueta = context.GetEtiquetaRecepcionExterno(
                                criterio.Predio,
                                criterio.EtiquetaExterna,
                                criterio.TipoEtiquetaExterna);
                    }
                }

                if (!string.IsNullOrEmpty(criterio.Ubicacion))
                {
                    if (esEtiquetaRecepcion)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoRequiereUbicacion", criterio.EtiquetaExterna));
                        return false;
                    }
                    else
                    {
                        if (lpnEtiqueta != null && lpnEtiqueta.Ubicacion != criterio.Ubicacion)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoExisteEnUbicacion", criterio.EtiquetaExterna, criterio.Ubicacion));
                            return false;
                        }

                        if (etiqueta != null && etiqueta.IdUbicacion != criterio.Ubicacion)
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoExisteEnUbicacion", criterio.EtiquetaExterna, criterio.Ubicacion));
                            return false;
                        }
                    }
                }
            }

            switch (criterio.Operacion)
            {
                case ControlCalidadCriterio.Ubicacion:
                    this.ValidateCriterioProcedimientoUbicacion(criterio, operacion, context, codigoControl, errors);
                    break;
                case ControlCalidadCriterio.LPN:
                    this.ValidateCriterioProcedimientoLpn(criterio, operacion, context, lpnEtiqueta, codigoControl, errors);
                    break;
                case ControlCalidadCriterio.Etiqueta:
                    this.ValidateCriterioProcedimientoEtiqueta(criterio, operacion, context, etiqueta, codigoControl, errors);
                    break;
                case ControlCalidadCriterio.Producto:
                    this.ValidateCriterioProcedimientoProducto(criterio, operacion, context, codigoControl, errors);
                    break;
            }

            return !errors.Any();
        }

        public virtual bool ValidateCriterioProcedimientoProducto(CriterioControlCalidadAPI criterio, ControlCalidadOperacion operacion, IControlCalidadServiceContext context, int codigoControl, List<Error> errors)
        {
            if (operacion == ControlCalidadOperacion.Aprobar)
            {
                if (!context.ExisteControlPendiente(criterio.Predio, codigoControl, criterio.Producto, criterio.Lote, criterio.Faixa, criterio.Empresa))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoLoteNoTieneControlPendiente", criterio.Producto, criterio.Lote, codigoControl));
                    return false;
                }
            }
            else
            {
                List<Lpn> lpns =
                    context.GetEtiquetaLpnExterno(
                        criterio.Predio,
                        criterio.Producto,
                        criterio.Empresa,
                        criterio.Lote,
                        criterio.Faixa);

                List<EtiquetaLote> etiquetas =
                    context.GetEtiquetaRecepcionExterno(
                        criterio.Predio,
                        criterio.Producto,
                        criterio.Empresa,
                        criterio.Lote,
                        criterio.Faixa);

                List<Stock> stocks =
                    context.GetStock(
                        criterio.Predio,
                        criterio.Producto,
                        criterio.Lote,
                        criterio.Empresa,
                        criterio.Faixa);

                if (!lpns.Any() && !etiquetas.Any() && !stocks.Any())
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoLoteNoTieneControlesParaAsignar", criterio.Producto, criterio.Lote, codigoControl));
                    return false;
                }

                foreach (EtiquetaLote etiqueta in etiquetas)
                    this.ValidateCriterioProcedimientoEtiqueta(criterio, operacion, context, etiqueta, codigoControl, errors);

                foreach (Lpn lpn in lpns)
                {
                    if (!etiquetas.Any(a => a.NroLpn == lpn.NumeroLPN))
                        this.ValidateCriterioProcedimientoLpn(criterio, operacion, context, lpn, codigoControl, errors);
                }

                foreach (Stock stock in stocks)
                {
                    List<EtiquetaLote> etiquetasUbicacionStock = etiquetas.Where(w => w.IdUbicacion == stock.Ubicacion).ToList();

                    if (context.TieneStockLibre(stock, etiquetasUbicacionStock))
                        this.ValidateCriterioProcedimientoUbicacion(criterio, operacion, context, codigoControl, errors, stock.Ubicacion);

                }
            }
            return !errors.Any();
        }

        public virtual bool ValidateCriterioProcedimientoEtiqueta(CriterioControlCalidadAPI criterio, ControlCalidadOperacion operacion, IControlCalidadServiceContext context, EtiquetaLote etiqueta, int codigoControl, List<Error> errors)
        {
            if (etiqueta.Estado != SituacionDb.PalletConferido)
            {
                errors.Add(new Error("WMSAPI_msg_Error_EtiquetaAlmacenada", criterio.EtiquetaExterna));
                return false;
            }

            List<EtiquetaLoteDetalle> detallesEtiqueta = etiqueta.Detalles.Where(
                    a => a.CodigoProducto == criterio.Producto &&
                        a.Faixa == criterio.Faixa &&
                        a.IdEmpresa == criterio.Empresa &&
                        a.Identificador == criterio.Lote &&
                        a.Cantidad > 0)
                .ToList();

            if (!detallesEtiqueta.Any())
            {
                errors.Add(new Error("WMSAPI_msg_Error_EtiquetaSinStock", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                return false;
            }

            foreach (var item in detallesEtiqueta)
            {
                if (operacion == ControlCalidadOperacion.Aprobar)
                {
                    if (!context.ExisteControlPendiente(
                        criterio.Predio,
                        codigoControl,
                        criterio.Producto,
                        criterio.Lote,
                        criterio.Faixa,
                        criterio.Empresa,
                        item.IdEtiquetaLote))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EtiquetaNoTieneControlPendiente", criterio.EtiquetaExterna, codigoControl, criterio.Producto, criterio.Lote));
                        return false;
                    }
                }
                else
                {
                    if (context.ExisteControlPendiente(
                        criterio.Predio,
                        codigoControl,
                        criterio.Producto,
                        criterio.Lote,
                        criterio.Faixa,
                        criterio.Empresa,
                        item.IdEtiquetaLote))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_EtiquetaCtrlCalidadPendienteYaAsignado", criterio.EtiquetaExterna, criterio.Empresa, criterio.Producto, criterio.Lote, codigoControl));
                        return false;
                    }
                }
            }

            return !errors.Any();
        }

        public virtual bool ValidateCriterioProcedimientoLpn(CriterioControlCalidadAPI criterio, ControlCalidadOperacion operacion, IControlCalidadServiceContext context, Lpn lpnEtiqueta, int codigoControl, List<Error> errors)
        {
            if (lpnEtiqueta.Estado != EstadosLPN.Activo)
            {
                errors.Add(new Error("WMSAPI_msg_Error_LpnNoActivo", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                return false;
            }

            List<LpnDetalle> detallesLpn = lpnEtiqueta.Detalles.Where(
                    a => a.CodigoProducto == criterio.Producto &&
                        a.Empresa == criterio.Empresa &&
                        a.Faixa == criterio.Faixa &&
                        a.Lote == criterio.Lote &&
                        a.Cantidad > 0)
                .ToList();

            if (!detallesLpn.Any())
            {
                errors.Add(new Error("WMSAPI_msg_Error_LpnSinStock", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                return false;
            }

            if (detallesLpn.Any(a => a.CantidadReserva > 0))
            {
                errors.Add(new Error("WMSAPI_msg_Error_LpnConStockReservado", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                return false;
            }

            if (!context.UbicacionLpnAreaDisponible(lpnEtiqueta.Ubicacion))
            {
                errors.Add(new Error("WMSAPI_msg_Error_LpnConAreaUbicacionIncorrecta", criterio.EtiquetaExterna));
                return false;
            }

            foreach (var item in detallesLpn)
            {
                if (operacion == ControlCalidadOperacion.Aprobar)
                {
                    if (!context.ExisteControlPendiente(
                        criterio.Predio,
                        codigoControl,
                        criterio.Producto,
                        criterio.Lote,
                        criterio.Faixa,
                        criterio.Empresa,
                        item.NumeroLPN,
                        item.Id))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_LpnNoTieneControlCalidadPendiente", criterio.EtiquetaExterna, codigoControl, criterio.Producto, criterio.Lote));
                        return false;
                    }
                }
                else
                {
                    if (context.StockLpnNoDisponible(item.Cantidad, item.NumeroLPN, lpnEtiqueta.Ubicacion, item.Empresa, item.CodigoProducto, item.Lote, item.Faixa, item.Id))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_LpnStockNoDisponible", criterio.EtiquetaExterna, criterio.Producto, criterio.Lote));
                        return false;
                    }

                    if (context.ExisteControlPendiente(
                        criterio.Predio,
                        codigoControl,
                        criterio.Producto,
                        criterio.Lote,
                        criterio.Faixa,
                        criterio.Empresa,
                        item.NumeroLPN,
                        item.Id))
                    {
                        errors.Add(
                            new Error(
                                "WMSAPI_msg_Error_LpnCtrlCalidadPendienteYaAsignado",
                                criterio.EtiquetaExterna,
                                criterio.Empresa,
                                criterio.Producto,
                                criterio.Lote,
                                codigoControl));
                        return false;
                    }
                }
            }

            return !errors.Any();
        }

        public virtual bool ValidateCriterioProcedimientoUbicacion(CriterioControlCalidadAPI criterio, ControlCalidadOperacion operacion, IControlCalidadServiceContext context, int codigoControl, List<Error> errors, string ubicacion = null)
        {
            ubicacion ??= criterio.Ubicacion;

            if (operacion == ControlCalidadOperacion.Aprobar)
            {
                if (!context.ExisteControlPendiente(
                    criterio.Predio,
                    codigoControl,
                    criterio.Producto,
                    criterio.Lote,
                    criterio.Faixa,
                    criterio.Empresa,
                    ubicacion))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionSinCtrlCalidadPendiente", ubicacion, codigoControl, criterio.Producto, criterio.Lote));
                    return false;
                }
            }
            else
            {
                if (context.ExisteControlPendiente(
                    criterio.Predio,
                    codigoControl,
                    criterio.Producto,
                    criterio.Lote,
                    criterio.Faixa,
                    criterio.Empresa,
                    ubicacion))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionCtrlCalidadPendienteYaAsignado", ubicacion, criterio.Empresa, criterio.Producto, criterio.Lote, codigoControl));
                    return false;
                }

                if (!context.UbicacionAreaDisponible(ubicacion))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionAreaIncorrecta", ubicacion));
                    return false;
                }

                if (!context.ProductoEnUbicacion(
                    ubicacion,
                    criterio.Empresa,
                    criterio.Producto,
                    criterio.Lote,
                    criterio.Faixa))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionNoContieneProducto", criterio.Producto, criterio.Lote, ubicacion));
                    return false;
                }

                if (context.StockLibreNoDisponible(
                    ubicacion,
                    criterio.Empresa,
                    criterio.Producto,
                    criterio.Lote,
                    criterio.Faixa))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_UbicacionProductoStockNoDisponible", criterio.Producto, criterio.Lote, ubicacion));
                    return false;
                }
            }

            return !errors.Any();
        }

        #endregion

        #region Facturas

        public virtual bool FacturaValidacionCarga(Factura factura, IFacturaServiceContext context, List<Error> errors)
        {
            HashSet<string> detProductos = new HashSet<string>();

            ValidarCampo("NumeroFactura", factura.NumeroFactura, true, typeof(string), 12, errors);
            ValidarCampo("TipoFactura", factura.TipoFactura, true, typeof(string), 12, errors);
            ValidarCampo("Serie", factura.Serie, true, typeof(string), 3, errors);
            ValidarCampo("CodigoAgente", factura.CodigoAgente, true, typeof(string), 40, errors);
            ValidarCampo("Predio", factura.Predio, false, typeof(string), 10, errors);
            ValidarCampo("FechaEmision", factura.FechaEmision?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);

            ValidarCampo("Anexo1", factura.Anexo1, false, typeof(string), 200, errors);
            ValidarCampo("Anexo2", factura.Anexo2, false, typeof(string), 200, errors);
            ValidarCampo("Anexo3", factura.Anexo3, false, typeof(string), 200, errors);
            ValidarCampo("Observacion", factura.Observacion, false, typeof(string), 200, errors);
            ValidarCampo("Moneda", factura.CodigoMoneda, false, typeof(string), 6, errors);
            ValidarCampo("TotalDigitado", factura.TotalDigitado?.ToString(), false, typeof(decimal), 15, errors, 4);
            ValidarCampo("IvaBase", factura.IvaBase?.ToString(), false, typeof(decimal), 15, errors, 4);
            ValidarCampo("IvaMinimo", factura.IvaMinimo?.ToString(), false, typeof(decimal), 15, errors, 4);

            ValidarCampo("FechaVencimiento", factura.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);

            if (errors.Any())
                return false;

            foreach (var detalle in factura.Detalles)
            {
                ValidarCampo("CodigoProducto", detalle.Producto, true, typeof(string), 40, errors);
                ValidarCampo("CantidadFacturada", detalle.CantidadFacturada?.ToString(), true, typeof(decimal), 12, errors, 3);

                ValidarCampo("Identificador", detalle.Identificador, false, typeof(string), 40, errors);
                ValidarCampo("ImporteUnitario", detalle.ImporteUnitario?.ToString(), false, typeof(decimal), 15, errors, 4);
                ValidarCampo("FechaVencimiento", detalle.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), false, typeof(DateTime), -1, errors);
                ValidarCampo("Anexo1", detalle.Anexo1, false, typeof(string), 200, errors);
                ValidarCampo("Anexo2", detalle.Anexo2, false, typeof(string), 200, errors);
                ValidarCampo("Anexo3", detalle.Anexo3, false, typeof(string), 200, errors);
                ValidarCampo("Anexo4", detalle.Anexo4, false, typeof(string), 200, errors);

                string identificador = string.Empty;
                if (!string.IsNullOrEmpty(detalle.Identificador))
                    identificador = detalle.Identificador;

                string keyDetalle = $"{detalle.Producto}.{identificador}";
                if (detProductos.Contains(keyDetalle))
                    errors.Add(new Error("WMSAPI_msg_Error_FacturaDetallesDuplicadas", factura.NumeroFactura, detalle.Producto, identificador));
                else
                    detProductos.Add(keyDetalle);

                if (errors.Any())
                    return false;
            }

            return true;
        }

        public virtual bool FacturaValidacionProcedimiento(Factura factura, IFacturaServiceContext context, List<Error> errors)
        {
            if (!ValidarAgente(factura.CodigoAgente, factura.IdEmpresa, factura.TipoAgente, t => context.GetAgente(t.Item1, t.Item2, t.Item3), errors, out string cdCliente))
                return false;

            if (!context.ExisteTipoFactura(factura.TipoFactura))
            {
                errors.Add(new Error("WMSAPI_msg_Error_TioiFacturaNoExiste", factura.TipoFactura));
                return false;
            }

            if (context.ExisteFactura(factura.NumeroFactura, factura.Serie, factura.IdEmpresa, cdCliente))
            {
                errors.Add(new Error("WMSAPI_msg_Error_FacturaYaExiste", factura.NumeroFactura, factura.Serie, factura.CodigoAgente, factura.IdEmpresa));
                return false;
            }

            ValidarPredio(factura.Predio, context.ExistePredio, errors);

            if (!string.IsNullOrEmpty(factura.CodigoMoneda))
                if (!context.ExisteMoneda(factura.CodigoMoneda))
                    errors.Add(new Error("WMSAPI_msg_Error_MonedaNoExiste", factura.CodigoMoneda));

            if (errors.Any())
                return false;

            var validarFechas = (context.GetParametro(ParamManager.IE_425_VALIDAR_FECHAS) ?? "N") == "S";
            if (validarFechas)
            {
                ValidarFechaMenorA("FechaEmision", factura.FechaEmision?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
                ValidarFechaMenorA("FechaVencimientoFactura", factura.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);
            }

            foreach (var detalle in factura.Detalles)
            {
                var prod = context.GetProducto(factura.IdEmpresa, detalle.Producto);
                if (prod == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", detalle.Producto, factura.IdEmpresa));
                    return false;
                }
                else if (!context.PermiteProductoInactivos && prod.Situacion != SituacionDb.Activo)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoInactivo", detalle.Producto, factura.IdEmpresa));
                    return false;
                }
                else if (!prod.AceptaDecimales)
                {
                    if (detalle.CantidadFacturada.HasValue && detalle.CantidadFacturada.Value != Math.Truncate(detalle.CantidadFacturada.Value))
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaDecimales", detalle.Producto));
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(detalle.Identificador))
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    else
                        detalle.Identificador = ManejoIdentificadorDb.IdentificadorAuto;
                }
                else if (prod.ManejoIdentificador == ManejoIdentificador.Producto && detalle.Identificador != ManejoIdentificadorDb.IdentificadorProducto)
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoNoManejaLote", detalle.Producto));

                if (prod.TipoManejoFecha == ManejoFechaProductoDb.Expirable && detalle.FechaVencimiento == null)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProductoManejaVencimiento", prod.Codigo));
                    return false;
                }
                else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Fifo)
                    detalle.FechaVencimiento = DateTime.Now;
                else if (prod.TipoManejoFecha == ManejoFechaProductoDb.Duradero)
                    detalle.FechaVencimiento = null;


                if (validarFechas)
                    ValidarFechaMenorA("FechaVencimiento", detalle.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY), DateTime.Now.ToString(CDateFormats.DATE_ONLY), "", errors, true);

                if (prod.ManejoIdentificador == ManejoIdentificador.Serie && detalle.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                {
                    if (detalle.CantidadFacturada != 1)
                    {
                        errors.Add(new Error("General_msg_Error_TipoSerieCantidadDistintaAUno"));
                        return false;
                    }
                    else if (context.ExisteSerie(prod.Codigo, detalle.Identificador))
                    {
                        errors.Add(new Error("General_Sec0_Error_SerieYaExiste", detalle.Identificador, prod.Codigo, factura.IdEmpresa));
                        return false;
                    }
                }

                if (!prod.IsIdentifiedByProducto() && LIdentificador.ContieneCaracteresNoPermitidos(detalle.Identificador, context.GetCaracteresNoPermitidosIdentificador()))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_CaracteresNoPermitidos"));
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Ubicaciones Picking

        public virtual bool PickingProductoValidacionCarga(UbicacionPickingProducto ubicacionPicking, PickingProductoServiceContext context, List<Error> errors)
        {
            ValidarCampo("TipoOperacion", ubicacionPicking.TipoOperacionId, true, typeof(string), 1, errors);

            switch (ubicacionPicking.TipoOperacionId)
            {
                case TipoOperacionDb.Alta:
                    ValidarCampo(ubicacionPicking, errors);
                    break;

                case TipoOperacionDb.Baja:
                    ValidarCampo(ubicacionPicking, errors);
                    break;

                case TipoOperacionDb.Sobrescritura:
                    ValidarCampoModificacion(ubicacionPicking, errors, context);
                    break;
            }

            return true;
        }

        public virtual void ValidarCampoModificacion(UbicacionPickingProducto ubicacionPicking, List<Error> errors, PickingProductoServiceContext context)
        {
            var camposInmutables = context.GetCamposInmutables();
            var oldModel = context.GetUbicacionPicking(ubicacionPicking.UbicacionSeparacion, ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa, ubicacionPicking.Padron, ubicacionPicking.Prioridad);

            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "StockMinimo", ubicacionPicking.StockMinimo?.ToString(), true, typeof(int), 9, errors, 0, true);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "StockMaximo", ubicacionPicking.StockMaximo?.ToString(), true, typeof(int), 9, errors, 0, true);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "CantidadDesborde", ubicacionPicking.CantidadDesborde?.ToString(), true, typeof(int), 9, errors, 0, true);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "Padron", ubicacionPicking.Padron.ToString(), true, typeof(decimal), 18, errors, 3, false);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "CodigoUnidadCajaAutomatismo", ubicacionPicking.CodigoUnidadCajaAutomatismo?.ToString(), false, typeof(string), 20, errors);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "CantidadUnidadCajaAutomatismo", ubicacionPicking.CantidadUnidadCajaAutomatismo?.ToString(), false, typeof(int), 9, errors, 0, true);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "FlagConfirmarCodBarrasAutomatismo", ubicacionPicking.FlagConfirmarCodBarrasAutomatismo?.ToString(), false, typeof(string), 1, errors, 0, true);
            ValidarCampo(oldModel, ubicacionPicking, camposInmutables, "Prioridad", ubicacionPicking.Prioridad.ToString(), true, typeof(int), 2, errors, 0, true);
        }

        public virtual void ValidarCampo(UbicacionPickingProducto ubicacionPicking, List<Error> errors)
        {
            ValidarCampo("Empresa", ubicacionPicking.Empresa.ToString(), true, typeof(int), 10, errors);
            ValidarCampo("CodigoProducto", ubicacionPicking.CodigoProducto?.ToString(), false, typeof(string), 40, errors);
            ValidarCampo("Ubicacion", ubicacionPicking.UbicacionSeparacion, false, typeof(string), 40, errors);
            ValidarCampo("Padron", ubicacionPicking.Padron.ToString(), true, typeof(decimal), 18, errors, 3, true);
            ValidarCampo("StockMinimo", ubicacionPicking.StockMinimo?.ToString(), true, typeof(int), 9, errors, 0, true);
            ValidarCampo("StockMaximo", ubicacionPicking.StockMaximo?.ToString(), true, typeof(int), 9, errors, 0, true);
            ValidarCampo("CantidadDesborde", ubicacionPicking.CantidadDesborde?.ToString(), true, typeof(int), 9, errors, 0, true);
            ValidarCampo("CodigoUnidadCajaAutomatismo", ubicacionPicking.CodigoUnidadCajaAutomatismo?.ToString(), false, typeof(string), 20, errors);
            ValidarCampo("CantidadUnidadCajaAutomatismo", ubicacionPicking.CantidadUnidadCajaAutomatismo?.ToString(), false, typeof(int), 9, errors, 0, true);
            ValidarCampo("FlagConfirmarCodBarrasAutomatismo", ubicacionPicking.FlagConfirmarCodBarrasAutomatismo?.ToString(), false, typeof(string), 1, errors, 0, true);
            ValidarCampo("Prioridad", ubicacionPicking.Prioridad.ToString(), true, typeof(int), 2, errors, 0, true);
        }

        public virtual bool PickingProductoValidacionProcedimiento(UbicacionPickingProducto ubicacionPicking, PickingProductoServiceContext context, List<Error> errors)
        {
            ValidarProducto(ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa, context.GetProducto(ubicacionPicking.Empresa, ubicacionPicking.CodigoProducto), errors, false);

            if (!ValidarTipoOperacion(ubicacionPicking.TipoOperacionId, true, errors))
                return false;

            var modelo = context.GetUbicacionPicking(ubicacionPicking.UbicacionSeparacion, ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa, ubicacionPicking.Padron, ubicacionPicking.Prioridad);
            var producto = context.GetProducto(ubicacionPicking.Empresa, ubicacionPicking.CodigoProducto);

            if (modelo != null && ubicacionPicking.TipoOperacionId == "A")
            {
                errors.Add(new Error("General_Sec0_Error_UbicacionPickingPadronExist", modelo.CodigoProducto, modelo.Predio, modelo.Padron, modelo.Empresa, modelo.Prioridad));
                return false;
            }
            else if (modelo == null && ubicacionPicking.TipoOperacionId == "B")
            {
                errors.Add(new Error("WMSAPI_msg_Error_DeleteUbiPickingNoExiste", ubicacionPicking.UbicacionSeparacion, ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa));
                return false;
            }

            if (ubicacionPicking.StockMaximo <= ubicacionPicking.StockMinimo)
            {
                errors.Add(new Error("WMSAPI_msg_Error_StockMaximoMenorMin", ubicacionPicking.UbicacionSeparacion, ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa));
                return false;
            }

            if (!context.ExisteEmpresa(ubicacionPicking.Empresa))
            {
                errors.Add(new Error("WMSAPI_msg_Error_EmpresaNoExiste", ubicacionPicking.Empresa));
                return false;
            }

            if (!context.ExisteUbicacion(ubicacionPicking.UbicacionSeparacion))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionNoExiste", ubicacionPicking.UbicacionSeparacion));
                return false;
            }

            if (!context.UbicacionValida(ubicacionPicking.UbicacionSeparacion))
            {
                errors.Add(new Error("WMSAPI_msg_Error_UbicacionInvalida", ubicacionPicking.UbicacionSeparacion));
                return false;
            }

            if (!context.ExisteProducto(ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa))
            {
                errors.Add(new Error("WMSAPI_msg_Error_ProductoNoExiste", ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa));
                return false;
            }

            if (producto.ManejoIdentificador == ManejoIdentificador.Serie)
            {
                if (ubicacionPicking.Padron != 1)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_ProdManejaSeriePadron", ubicacionPicking.CodigoProducto));
                    return false;
                }
            }

            var _padron = Convert.ToInt32(Convert.ToDecimal(ubicacionPicking.Padron));

            if (ubicacionPicking.TipoOperacionId == TipoOperacionDb.Alta)
            {
                if (context.ExisteUbicacionPicking(ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa, _padron, ubicacionPicking.Predio, ubicacionPicking.Prioridad))
                {
                    errors.Add(new Error("General_Sec0_Error_UbicacionPickingPadronExist", ubicacionPicking.CodigoProducto, ubicacionPicking.Predio, ubicacionPicking.Padron, ubicacionPicking.Empresa, ubicacionPicking.Prioridad));
                    return false;
                }

                if (context.EsUbicacionInvalidaMonoProducto(ubicacionPicking.UbicacionSeparacion))
                {
                    errors.Add(new Error("General_Sec0_Error_UbicacionMonoProductoAsignada", ubicacionPicking.UbicacionSeparacion));
                    return false;
                }

                if (context.EsUbicacionInvalidaMonoProductoOtroStock(ubicacionPicking.UbicacionSeparacion))
                {
                    errors.Add(new Error("General_Sec0_Error_UbicacionMonoProductoYOtroStock", ubicacionPicking.UbicacionSeparacion));
                    return false;
                }

                if (context.EsUbicacionInvalidaClaseProducto(ubicacionPicking.UbicacionSeparacion))
                {
                    errors.Add(new Error("General_Sec0_Error_ProductoDistintaClaseUbicacion", ubicacionPicking.UbicacionSeparacion));
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(ubicacionPicking.CodigoUnidadCajaAutomatismo))
            {
                if (!context.ExisteCodigoCajaAutomatismo(ubicacionPicking.CodigoUnidadCajaAutomatismo))
                {
                    errors.Add(new Error("General_Sec0_Error_CodigoCajaAutomatismoNoExist", ubicacionPicking.CodigoUnidadCajaAutomatismo));
                    return false;
                }
            }

            if (ubicacionPicking.Prioridad > 99)
            {
                errors.Add(new Error("General_Sec0_Error_PrioridadMax"));
                return false;
            }

            return true;
        }

        public virtual void SetDefaultValuesUbicacionPicking(UbicacionPickingProducto ubicacionPicking, PickingProductoServiceContext context)
        {
            var producto = context.GetProducto(ubicacionPicking.Empresa, ubicacionPicking.CodigoProducto);
            var esUbicacionAutomatismo = context.EsUbicacionAutomatismo(ubicacionPicking.UbicacionSeparacion);

            ubicacionPicking.Padron = Convert.ToInt32(Convert.ToDecimal(ubicacionPicking.Padron));
            ubicacionPicking.Faixa = 1;
            ubicacionPicking.TipoPicking = esUbicacionAutomatismo ? "A" : (producto?.UnidadBulto == ubicacionPicking.Padron ? "C" : "U");
            ubicacionPicking.Predio = context.GetPredioUbicacion(ubicacionPicking.UbicacionSeparacion);

            if (esUbicacionAutomatismo)
            {
                if (string.IsNullOrEmpty(ubicacionPicking.CodigoUnidadCajaAutomatismo))
                {
                    var param = context.GetParametro(ParamManager.IE_570_CODIGO_UND_CAJA_AUT);
                    ubicacionPicking.CodigoUnidadCajaAutomatismo = param;
                }

                if (ubicacionPicking.CantidadUnidadCajaAutomatismo == null)
                {
                    var param = context.GetParametro(ParamManager.IE_570_CANT_UND_CAJA_AUT);

                    if (int.TryParse(param, out int parsed))
                        ubicacionPicking.CantidadUnidadCajaAutomatismo = parsed;
                }

                if (string.IsNullOrEmpty(ubicacionPicking.FlagConfirmarCodBarrasAutomatismo))
                {
                    var param = context.GetParametro(ParamManager.IE_570_FL_CONFIRMAR_BARRA_AUT);
                    ubicacionPicking.FlagConfirmarCodBarrasAutomatismo = param;
                }
            }
        }

        #endregion
    }
}

