using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Inventario;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.Domain.StockEntities;
using WIS.Security;

namespace WIS.Domain.Produccion.Logic
{
    public class LogicaProduccionBlackBox : LogicaProduccion
    {
        public LogicaProduccionBlackBox(IUnitOfWork uow, IIdentityService identity, IngresoProduccionBlackBox ingresoProduccion)
            : base(uow, identity, ingresoProduccion)
        {

        }

        public override IngresoProduccion CrearIngresoProduccion(string tipoIngreso, int empresa, string predio, List<IngresoProduccionDetalleTeorico> detalles, string idExterno = null, string idEspacioProduccion = null)
        {
            _ingresoProduccion.Empresa = empresa;
            _ingresoProduccion.IdFormula = string.Empty;
            _ingresoProduccion.Predio = predio;
            _ingresoProduccion.Detalles = detalles;
            _ingresoProduccion.Funcionario = _identity.UserId;
            _ingresoProduccion.FechaAlta = DateTime.Now;
            _ingresoProduccion.Situacion = SituacionDb.PRODUCCION_CREADA;
            _ingresoProduccion.NumeroProduccionOriginal = _ingresoProduccion.Id;
            _ingresoProduccion.Tipo = tipoIngreso;
            _ingresoProduccion.PermitirAutoasignarLinea = "N";
            _ingresoProduccion.IdModalidadLote = null;
            _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();
            _ingresoProduccion.IdManual = "S";

            _ingresoProduccion.IdProduccionExterno = idExterno;
            _ingresoProduccion.IdEspacioProducion = idEspacioProduccion;

            return _ingresoProduccion;
        }

        public override void ConsumirInsumoCompleto(long idInsumo, string cdEndereco, decimal cantidadConsumir, out DateTime? vencimiento, bool isConsumible = false)
        {
            IngresoProduccionDetalleReal insumo = this._ingresoProduccion.GetDetalleInsumo(idInsumo);

            if (insumo == null)
                throw new Exception("PRD113_grid1_Error_InsumoSinRegistro");

            Stock stockInsumo = this._uow.StockRepository.GetStock(insumo.Empresa ?? 1, insumo.Producto, insumo.Faixa ?? 1, cdEndereco, insumo.Identificador);

            if (stockInsumo == null || stockInsumo.Cantidad < cantidadConsumir)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");

            if (insumo.QtReal == 0)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");

            decimal saldoConsumir = (cantidadConsumir);

            stockInsumo.Cantidad -= saldoConsumir;

            if (!isConsumible)
            {
                if (stockInsumo.ReservaSalida > saldoConsumir)
                    stockInsumo.ReservaSalida = (stockInsumo.ReservaSalida ?? 0) - saldoConsumir;
                else
                    stockInsumo.ReservaSalida = 0;
            }
            vencimiento = stockInsumo.Vencimiento;
            stockInsumo.NumeroTransaccion = _uow.GetTransactionNumber();
            stockInsumo.FechaModificacion = DateTime.Now;

            insumo.QtReal -= saldoConsumir;
            insumo.QtDesafectado = (insumo.QtDesafectado ?? 0) + saldoConsumir;
            insumo.NuTransaccion = _uow.GetTransactionNumber();

            _uow.StockRepository.UpdateStock(stockInsumo);
            _uow.IngresoProduccionRepository.UpdateDetalleRealProduccion(insumo);
            _uow.SaveChanges();
        }

        public override void GenerarProductoNoEsperado(Producto producto, decimal faixa, int empresa, string lote, decimal producido, DateTime? vencimiento, string codMotivo, string dsMotivo, out string keyAjuste)
        {
            keyAjuste = string.Empty;

            var stockProductoFinal = this._uow.StockRepository.GetStock(empresa, producto.Codigo, faixa, _ingresoProduccion.EspacioProduccion.IdUbicacionProduccion, lote);

            if (stockProductoFinal == null)
            {
                stockProductoFinal = new Stock();
                stockProductoFinal.NumeroTransaccion = _uow.GetTransactionNumber();
                stockProductoFinal.FechaInventario = DateTime.Now;
                stockProductoFinal.Cantidad = producido;
                stockProductoFinal.CantidadTransitoEntrada = 0;
                stockProductoFinal.ReservaSalida = 0;
                stockProductoFinal.Ubicacion = _ingresoProduccion.EspacioProduccion.IdUbicacionProduccion;
                stockProductoFinal.Producto = producto.Codigo;
                stockProductoFinal.Empresa = empresa;
                stockProductoFinal.Identificador = lote;
                stockProductoFinal.ControlCalidad = EstadoControlCalidad.Controlado;
                stockProductoFinal.Averia = "N";
                stockProductoFinal.Faixa = faixa;
                stockProductoFinal.Inventario = "R";
                stockProductoFinal.Vencimiento = vencimiento;

                _uow.StockRepository.AddStock(stockProductoFinal);
            }
            else
            {
                stockProductoFinal.Cantidad += producido;
                stockProductoFinal.NumeroTransaccion = _uow.GetTransactionNumber();
                stockProductoFinal.FechaModificacion = DateTime.Now;
                stockProductoFinal.Vencimiento = InventarioLogic.ResolverVencimiento(stockProductoFinal.Vencimiento, vencimiento);

                _uow.StockRepository.UpdateStock(stockProductoFinal);
            }

            if (codMotivo == TipoIngresoProduccion.MOT_PROD_ADS)
            {
                dsMotivo = !string.IsNullOrEmpty(dsMotivo) ? dsMotivo : $"Producción para el Ingreso Nro: {_ingresoProduccion.Id}";
                var ajuste = new AjusteStock
                {
                    NuAjusteStock = _uow.AjusteRepository.GetNextNuAjuste(),
                    Ubicacion = stockProductoFinal.Ubicacion,
                    Empresa = stockProductoFinal.Empresa,
                    Producto = stockProductoFinal.Producto,
                    Faixa = stockProductoFinal.Faixa,
                    Identificador = stockProductoFinal.Identificador,
                    QtMovimiento = producido,
                    FechaVencimiento = stockProductoFinal.Vencimiento,
                    FechaRealizado = DateTime.Now,
                    TipoAjuste = TipoAjusteDb.Stock,
                    CdMotivoAjuste = MotivoAjusteDb.Produccion,
                    DescMotivo = dsMotivo,
                    NuTransaccion = _uow.GetTransactionNumber(),
                    Predio = GetPredio(),
                    IdAreaAveria = "N",
                    FechaMotivo = DateTime.Now,
                    Funcionario = _identity.UserId,
                    Aplicacion = _identity.Application,
                    Metadata = _ingresoProduccion.Id
                };

                _uow.AjusteRepository.Add(ajuste);

                keyAjuste = ajuste.NuAjusteStock.ToString();
            }
            else
                AddDetalleProductoNoEsperado(_ingresoProduccion.EspacioProduccion.IdUbicacionProduccion, producto.Codigo, faixa, empresa, lote, producido, codMotivo, dsMotivo, vencimiento);
        }

        public override void ConsumirInsumoParcial(long idInsumo, string ubicacion, decimal qtConsumir, out DateTime? vencimiento, bool isConsumible = false)
        {
            IngresoProduccionDetalleReal insumo = this._ingresoProduccion.GetDetalleInsumo(idInsumo);

            if (insumo == null)
                throw new Exception("PRD113_grid1_Error_InsumoSinRegistro");

            Stock stockInsumo = this._uow.StockRepository.GetStock(insumo.Empresa ?? 1, insumo.Producto, insumo.Faixa ?? 1, ubicacion, insumo.Identificador);

            if (stockInsumo == null || stockInsumo.Cantidad < qtConsumir)
                throw new Exception("PRD113_grid1_Error_InsumoSinStock");

            if (insumo.QtReal < qtConsumir)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");

            insumo.QtReal -= qtConsumir;
            insumo.QtDesafectado = (insumo.QtDesafectado ?? 0) + qtConsumir;
            insumo.NuTransaccion = _uow.GetTransactionNumber();

            stockInsumo.Cantidad -= qtConsumir;

            if (!isConsumible)
            {
                if (stockInsumo.ReservaSalida > qtConsumir)
                    stockInsumo.ReservaSalida = (stockInsumo.ReservaSalida ?? 0) - qtConsumir;
                else
                    stockInsumo.ReservaSalida = 0;
            }
            vencimiento = stockInsumo.Vencimiento;

            stockInsumo.NumeroTransaccion = _uow.GetTransactionNumber();
            stockInsumo.FechaModificacion = DateTime.Now;

            _uow.StockRepository.UpdateStock(stockInsumo);
            _uow.IngresoProduccionRepository.UpdateDetalleRealProduccion(insumo);

            _uow.SaveChanges();
        }

        public override void DefinirLotesPedido(Pedido pedido, List<DetallePedido> detalleDefinido, IFormatProvider format)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePedido(Pedido pedido)
        {
            _uow.PedidoRepository.UpdatePedido(pedido);
        }

        public virtual bool HayProduccionActivaEnEspacio(string idEspacio, out string nuIngresoProduccion)
        {
            nuIngresoProduccion = string.Empty;
            IngresoProduccion ingresoActivoEspacio = _uow.IngresoProduccionRepository.GetIngresoActivoEnEspacio(idEspacio);
            if (ingresoActivoEspacio == null)
                return false;

            nuIngresoProduccion = ingresoActivoEspacio.Id;
            return true;
        }

        public override bool PuedeIniciarProduccion(out string mensaje, out List<string> errorArg)
        {
            mensaje = null;
            errorArg = new List<string>();

            if (_ingresoProduccion.EspacioProduccion == null)
            {
                mensaje = "PRD110_grid1_Error_ProduccionSinEspacio";
                return false;
            }

            if (_ingresoProduccion.Situacion == SituacionDb.PRODUCCION_INICIADA
                    || _ingresoProduccion.Situacion == SituacionDb.PRODUCCION_PARCIALMENTE_NOTIF
                    || _ingresoProduccion.Situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL
                    || _ingresoProduccion.Situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL
                    || _ingresoProduccion.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
            {
                mensaje = "PRD110_grid1_Error_ProduccionSituacionIcorrectaParaInicio";
                return false;
            }

            return true;
        }

        public override IngresoProduccionDetalleReal ExisteIngresoReal(string codigoProducto, string identificador)
        {
            return _ingresoProduccion.Consumidos.FirstOrDefault(w => w.Producto == codigoProducto && w.Identificador == identificador);
        }
    }
}
