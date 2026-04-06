using System;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Integracion.Preparaciones;
using WIS.Domain.ManejoStock.Enums;
using WIS.Domain.StockEntities;
using WIS.Exceptions;

namespace WIS.Domain.ManejoStock.EntradaBlackBox
{
    public class EntradaBlackBox
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public EntradaBlackBox(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual void RechazarStockEntradaBlackBox(IUnitOfWork uow, Stock stock, decimal? cantidadRechazoSano, decimal? cantidadRechazoAveria, string nroIngreso, int usuario)
        {
            //Verificar saldo por si se modifico la reserva del stock
            if ((stock.ReservaSalida ?? 0) < ((cantidadRechazoSano ?? 0) + (cantidadRechazoAveria ?? 0)))
                throw new ValidationFailedException("General_Sec0_Error_Error73");

            //Desafectar reserva de stock
            stock.ReservaSalida = ((stock.ReservaSalida ?? 0) - ((cantidadRechazoSano ?? 0) + (cantidadRechazoAveria ?? 0)));
            stock.FechaModificacion = DateTime.Now;

            //Desreserva documental
            PreparacionDocumental preparacion = new PreparacionDocumental(this._uowFactory);
            preparacion.DesreservarEntradaAnularPreparacion(uow, stock.Empresa, stock.Producto, stock.Faixa, stock.Identificador, ((cantidadRechazoSano ?? 0) + (cantidadRechazoAveria ?? 0)));

            //Generar movimiento de stock black box
            if (cantidadRechazoAveria > 0)
            {
                MovimientoStockBlackBox movimientoRechazoAveria = this.GenerarMovimientoEntradaBlackBox(stock, TipoMovimientosBlackBox.RechazoAveria, cantidadRechazoAveria, stock.Ubicacion, stock.Ubicacion, nroIngreso, uow, usuario);

                uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimientoRechazoAveria);
            }

            if (cantidadRechazoSano > 0)
            {
                MovimientoStockBlackBox movimientoRechazoSano = this.GenerarMovimientoEntradaBlackBox(stock, TipoMovimientosBlackBox.RechazoSano, cantidadRechazoSano, stock.Ubicacion, stock.Ubicacion, nroIngreso, uow, usuario);

                uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimientoRechazoSano);
            }
        }

        public virtual void IngresarStockBlackBox(IUnitOfWork uow, Stock stock, decimal? cantidadMovimiento, string nroIngreso, int usuario)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            if ((stock.ReservaSalida ?? 0) < (cantidadMovimiento ?? 0))
                throw new ValidationFailedException("General_Sec0_Error_Error73");

            //Desafectar reserva y cantidad stock
            stock.ReservaSalida = ((stock.ReservaSalida ?? 0) - (cantidadMovimiento ?? 0));
            stock.Cantidad = ((stock.Cantidad ?? 0) - (cantidadMovimiento ?? 0));
            stock.FechaModificacion = DateTime.Now;

            //Obtener ubicacion de BlackBox
            var lineaBlackBox = uow.LineaRepository.GetLineaBlackBoxPorUbicacionEntrada(stock.Ubicacion);
            var EDetallePedido = uow.LineaRepository.GetDetallePedido(stock.Empresa, stock.Producto, stock.Identificador, stock.Faixa, nroIngreso);
            string endereco = lineaBlackBox.UbicacionBlackBox;

            if (EDetallePedido != null && EDetallePedido.Semiacabado == "S" || EDetallePedido.Consumible == "S")
            {
                if (EDetallePedido.Semiacabado == "S")
                {
                    endereco = uow.ProduccionRepository.GetUbicacionSemiacabado(EDetallePedido.Predio);
                }
                else
                {
                    endereco = uow.ProduccionRepository.GetUbicacionConsumible(EDetallePedido.Predio);
                }

                if (endereco == null)
                {
                    throw new ValidationFailedException("General_Sec0_Error_Error79");
                }
            }

            if (lineaBlackBox == null)
                throw new ValidationFailedException("General_Sec0_Error_Error75");

            //Verificar si existe stock en BlackBox
            var stockExistente = uow.StockRepository.GetStock(stock.Empresa, stock.Producto, stock.Faixa, endereco, stock.Identificador);

            if (stockExistente != null)
            {
                //Si ya existe stock en ubicaicon de BlackBox, actualizar stock existente
                stockExistente.Cantidad = (stockExistente.Cantidad ?? 0) + (cantidadMovimiento ?? 0);
                stockExistente.ReservaSalida = (stockExistente.ReservaSalida ?? 0) + (cantidadMovimiento ?? 0);
                stockExistente.FechaModificacion = DateTime.Now;
                stockExistente.NumeroTransaccion = nuTransaccion;

                uow.StockRepository.UpdateStock(stockExistente);
            }
            else
            {
                //Si no existe stock en ubicacion de BlackBox, crear
                var stockBlackBox = this.CrearStockParaBlackBox(stock, endereco, cantidadMovimiento);
                stockBlackBox.NumeroTransaccion = nuTransaccion;

                uow.StockRepository.AddStock(stockBlackBox);
            }

            MovimientoStockBlackBox movimientoIngreso = this.GenerarMovimientoEntradaBlackBox(stock, TipoMovimientosBlackBox.IngresoBalckBox, cantidadMovimiento, stock.Ubicacion, endereco, nroIngreso, uow, usuario);

            uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimientoIngreso);
        }

        public virtual MovimientoStockBlackBox GenerarMovimientoEntradaBlackBox(Stock stock, TipoMovimientosBlackBox tipo, decimal? cantidadMovimiento, string ubicacionOrigen, string ubicacionDestino, string ingreso, IUnitOfWork uow, int usuario)
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
                Ingreso = ingreso,
                NumeroInterfazEjecucion = -1
            };
        }

        public virtual Stock CrearStockParaBlackBox(Stock stockEntrada, string ubicacionBlackBox, decimal? cantidadMovimientoEntrada)
        {
            return new Stock()
            {
                Averia = stockEntrada.Averia,
                ReservaSalida = cantidadMovimientoEntrada,
                Cantidad = cantidadMovimientoEntrada,
                ControlCalidad = stockEntrada.ControlCalidad,
                Empresa = stockEntrada.Empresa,
                Faixa = stockEntrada.Faixa,
                FechaInventario = stockEntrada.FechaInventario,
                Identificador = stockEntrada.Identificador,
                Producto = stockEntrada.Producto,
                Ubicacion = ubicacionBlackBox,
                Vencimiento = stockEntrada.Vencimiento
            };
        }
    }
}
