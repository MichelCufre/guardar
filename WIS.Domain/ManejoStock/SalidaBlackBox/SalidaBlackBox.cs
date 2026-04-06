using System;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Integracion.Preparaciones;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Enums;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.StockEntities;
using WIS.Exceptions;

namespace WIS.Domain.ManejoStock.SalidaBlackBox
{
    public class SalidaBlackBox
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public SalidaBlackBox(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual void SalidaStockBlackBox(IUnitOfWork uow, Stock stock, decimal? cantidadMovimiento, decimal? cantidadAveria, TipoStockOutBB tipoStock, string nroIngreso, int usuario, string Semiacabado = "N")
        {
            var nuTransaccion = uow.GetTransactionNumber();

            if (stock.ReservaSalida < ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)) && tipoStock == TipoStockOutBB.INSUMO)
                throw new ValidationFailedException("General_Sec0_Error_Error73");

            if (stock.Cantidad < ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)) && tipoStock == TipoStockOutBB.PRODUCTO)
                throw new ValidationFailedException("General_Sec0_Error_Error73");

            //Desafectar reserva y cantidad stock
            if (tipoStock == TipoStockOutBB.INSUMO)
                stock.ReservaSalida = ((stock.ReservaSalida ?? 0) - ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)));

            stock.Cantidad = ((stock.Cantidad ?? 0) - ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)));
            stock.FechaModificacion = DateTime.Now;

            //Obtener ubicacion de salida BlackBox
            var lineaBlackBox = uow.LineaRepository.GetLineaBlackBoxPorUbicacionBB(stock.Ubicacion);

            if (lineaBlackBox == null)
                throw new ValidationFailedException("General_Sec0_Error_Error75");

            //Verificar si existe stock en BlackBox
            string endereco = lineaBlackBox.UbicacionSalida;
            if (Semiacabado == "S")
            {
                string predio = uow.ProduccionRepository.GetPedidoProduccion(nroIngreso);
                if (!string.IsNullOrEmpty(predio))
                {
                    endereco = uow.ProduccionRepository.GetUbicacionSemiacabado(predio);
                    if (endereco == null)
                    {
                        throw new ValidationFailedException("General_Sec0_Error_Error79");
                    }
                }
            }

            var stockExistente = uow.StockRepository.GetStock(stock.Empresa, stock.Producto, stock.Faixa, endereco, stock.Identificador);
            if (stockExistente != null)
            {
                //Si ya existe stock en ubicaicon de SALIDA BlackBox, actualizar stock existente
                stockExistente.Cantidad = (stockExistente.Cantidad ?? 0) + ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0));

                if (Semiacabado == "S")
                {
                    stockExistente.ReservaSalida = (stockExistente.ReservaSalida ?? 0) + ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0));
                }

                stockExistente.FechaModificacion = DateTime.Now;
                stockExistente.NumeroTransaccion = nuTransaccion;

                uow.StockRepository.UpdateStock(stockExistente);
                uow.SaveChanges();
            }
            else
            {
                Producto prod = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(stock.Empresa, stock.Producto);

                if (prod.IsFifo())
                {
                    stock.Vencimiento = DateTime.Now;
                }

                if (Semiacabado == "S")
                {
                    var stockBlackBox = this.CrearStockSemiParaBlackBox(stock, endereco, ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)));
                    stockBlackBox.NumeroTransaccion = nuTransaccion;
                    uow.StockRepository.AddStock(stockBlackBox);
                }
                else
                {
                    //Si no existe stock en ubicacion de SALIDA BlackBox, crear
                    var stockBlackBox = this.CrearStockParaBlackBox(stock, endereco, ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)));
                    stockBlackBox.NumeroTransaccion = nuTransaccion;
                    uow.StockRepository.AddStock(stockBlackBox);
                }

            }

            //Desreserva documental
            if (tipoStock == TipoStockOutBB.INSUMO)
            {
                PreparacionDocumental preparacion = new PreparacionDocumental(this._uowFactory);
                preparacion.DesreservarEntradaAnularPreparacion(uow, stock.Empresa, stock.Producto, stock.Faixa, stock.Identificador, ((cantidadMovimiento ?? 0) + (cantidadAveria ?? 0)));
            }

            //Generar movimiento de stock black box
            if ((cantidadAveria ?? 0) > 0)
            {
                MovimientoStockBlackBox movimientoSalidaAveria = this.GenerarMovimientoSalidaBlackBox(stock, tipoStock == TipoStockOutBB.INSUMO ? TipoMovimientosBlackBox.SalidaInsumoAveria : TipoMovimientosBlackBox.SalidaProductoAveria, cantidadAveria, stock.Ubicacion, lineaBlackBox.UbicacionSalida, nroIngreso, uow, usuario);
                uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimientoSalidaAveria);
            }

            if ((cantidadMovimiento ?? 0) > 0)
            {
                MovimientoStockBlackBox movimientoSalida = this.GenerarMovimientoSalidaBlackBox(stock, tipoStock == TipoStockOutBB.INSUMO ? TipoMovimientosBlackBox.SalidaInsumo : TipoMovimientosBlackBox.SalidaProduco, cantidadMovimiento, stock.Ubicacion, lineaBlackBox.UbicacionSalida, nroIngreso, uow, usuario);
                uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimientoSalida);
            }
        }

        public virtual MovimientoStockBlackBox GenerarMovimientoSalidaBlackBox(Stock stock, TipoMovimientosBlackBox tipo, decimal? cantidadMovimiento, string ubicacionOrigen, string ubicacionDestino, string nroIngreso, IUnitOfWork uow, int usuario)
        {
            return new MovimientoStockBlackBox()
            {
                CantidadMovimiento = cantidadMovimiento,
                CodigoEmpresa = stock.Empresa,
                CodigoFaixa = stock.Faixa,
                CodigoProducto = stock.Producto,
                FechaMovimiento = DateTime.Now,
                NumeroIdentificador = stock.Identificador,
                UbicacionDestino = ubicacionDestino,
                UbicacionOrigen = ubicacionOrigen,
                TipoAccionMovimiento = tipo,
                Usuario = usuario,
                NumeroMovimientoBB = uow.MovimientoStockBBRepository.GetNumeroMovimiento(),
                Ingreso = nroIngreso
            };
        }

        public virtual Stock CrearStockParaBlackBox(Stock stockEntrada, string ubicacionSalidaBlackBox, decimal? cantidadMovimientoSalida)
        {
            return new Stock()
            {
                Averia = stockEntrada.Averia,
                ReservaSalida = 0,
                Cantidad = cantidadMovimientoSalida,
                ControlCalidad = stockEntrada.ControlCalidad,
                Empresa = stockEntrada.Empresa,
                Faixa = stockEntrada.Faixa,
                FechaInventario = stockEntrada.FechaInventario,
                Identificador = stockEntrada.Identificador,
                Producto = stockEntrada.Producto,
                Ubicacion = ubicacionSalidaBlackBox,
                Vencimiento = stockEntrada.Vencimiento
            };
        }

        public virtual Stock CrearStockSemiParaBlackBox(Stock stockEntrada, string ubicacionSalidaBlackBox, decimal? cantidadMovimientoSalida)
        {
            return new Stock()
            {
                Averia = stockEntrada.Averia,
                ReservaSalida = cantidadMovimientoSalida,
                Cantidad = cantidadMovimientoSalida,
                ControlCalidad = stockEntrada.ControlCalidad,
                Empresa = stockEntrada.Empresa,
                Faixa = stockEntrada.Faixa,
                FechaInventario = stockEntrada.FechaInventario,
                Identificador = stockEntrada.Identificador,
                Producto = stockEntrada.Producto,
                Ubicacion = ubicacionSalidaBlackBox,
                Vencimiento = stockEntrada.Vencimiento
            };
        }
    }
}
