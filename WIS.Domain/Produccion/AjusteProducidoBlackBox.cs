using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.ManejoStock;
using WIS.Domain.ManejoStock.Enums;
using WIS.Domain.StockEntities;
using WIS.Exceptions;

namespace WIS.Domain.Produccion
{
    public class AjusteProducidoBlackBox
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IngresoBlackBox _ingreso;
        protected readonly int _usuario;
        protected readonly AjusteMapper _mapper;

        public AjusteProducidoBlackBox(IUnitOfWork uow, IngresoBlackBox ingreso, int usuario)
        {
            this._uow = uow;
            this._ingreso = ingreso;
            this._usuario = usuario;
            this._mapper = new AjusteMapper();
        }

        public virtual void AddLineaProducto(string producto, string identificador, decimal faixa, DateTime? vencimiento, decimal cantidad, int empresa, out Stock stock, bool agregaromod = true, string semiacabado = "N")
        {
            LineaProducida linea = this.AddNuevaLineaProducida(producto, identificador, faixa, vencimiento, cantidad, semiacabado);
            this.AddStockProducido(linea, out stock, agregaromod);
            this.RegistrarMovimientoBB(empresa, faixa, producto, identificador, TipoMovimientosBlackBox.Producir, cantidad);
        }

        public virtual void RemoveLineaProducto(string producto, string identificador, decimal faixa, int empresa)
        {
            //LineaProducida linea = this._ingreso.Producidos.Where(d => d.Empresa == this._ingreso.Empresa && d.Producto == producto && d.Identificador == identificador && d.Faixa == faixa).FirstOrDefault();

            //this.RegistrarMovimientoBB(empresa, faixa, producto, identificador, TipoMovimientosBlackBox.Producir, -linea.Cantidad);
            //this.RemoveLineaProducida(linea);
            //this.RemoveStockProducido(linea);
        }

        public virtual void UpdateLineaProducto(string producto, string identificador, decimal faixa, decimal cantidad, int empresa)
        {
            if (cantidad > 0)
            {
                //LineaProducida linea = this._ingreso.Producidos.Where(d => d.Empresa == this._ingreso.Empresa && d.Producto == producto && d.Identificador == identificador && d.Faixa == faixa).FirstOrDefault();

                //this.RegistrarMovimientoBB(empresa, faixa, producto, identificador, TipoMovimientosBlackBox.Producir, cantidad - linea.Cantidad);
                //this.UpdateStockProducido(linea, cantidad);
                //this.UpdateLineaProducida(linea, cantidad);
            }
            else
            {
                //this.RemoveLineaProducto(producto, identificador, faixa, empresa);
            }
        }

        public virtual void CrearHistoricoProducido(List<LineaProducida> productosProducidos)
        {
            foreach (var lineaProducido in productosProducidos)
            {
                this.AddNuevaLineaHistoricaProducido(lineaProducido);
                this._uow.ProduccionRepository.RemoveProducido(this._ingreso, lineaProducido);
            }
        }

        public virtual LineaProducida AddNuevaLineaProducida(string producto, string identificador, decimal faixa, DateTime? vencimiento, decimal cantidad, string semi = "N")
        {
            var lineaNueva = new LineaProducida
            {
                Producto = producto,
                Empresa = this._ingreso.Empresa,
                Faixa = faixa,
                Identificador = identificador,
                Iteracion = 1,
                Pasada = 1,
                Vencimiento = vencimiento,
                Cantidad = cantidad,
                FechaAlta = DateTime.Now,
                Semiacabado = semi
            };

            //this._ingreso.Producidos.Add(lineaNueva);
            this._uow.ProduccionRepository.AddProducido(this._ingreso, lineaNueva, _uow.GetTransactionNumber());

            return lineaNueva;
        }

        public virtual void RemoveLineaProducida(LineaProducida linea)
        {
            this._uow.ProduccionRepository.RemoveProducido(this._ingreso, linea);
            //this._ingreso.Producidos.Remove(linea);
        }

        public virtual void UpdateLineaProducida(LineaProducida linea, decimal cantidad)
        {
            linea.Cantidad = cantidad;

            this._uow.ProduccionRepository.UpdateProducido(this._ingreso, linea, _uow.GetTransactionNumber());
        }

        public virtual void AddStockProducido(LineaProducida linea, out Stock stock, bool agregaroMod = true)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

			int empresaInt = (int)this._ingreso.Empresa;

			stock = this._uow.StockRepository.GetStock(empresaInt, linea.Producto, linea.Faixa, ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox, linea.Identificador);

            if (stock == null)
            {

				stock = new Stock
                {
                    Producto = linea.Producto,
                    Identificador = linea.Identificador,
                    Averia = "N",
                    Empresa = empresaInt,
                    Faixa = linea.Faixa,
                    Ubicacion = ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox,
                    Inventario = "R",
                    NumeroTransaccion = nuTransaccion,
                    Cantidad = linea.Cantidad,
                    Vencimiento = linea.Vencimiento,
                    ReservaSalida = 0,
                    ControlCalidad = EstadoControlCalidad.Controlado,
                    CantidadTransitoEntrada = 0,
                    FechaModificacion = DateTime.Now,
                };

                if (agregaroMod)
                {
                    this._uow.StockRepository.AddStock(stock);
                }
            }
            else
            {
                if (agregaroMod)
                {
                    stock.Cantidad = (stock.Cantidad ?? 0) + linea.Cantidad;
                    stock.NumeroTransaccion = nuTransaccion;
                    this._uow.StockRepository.UpdateStock(stock);
                }
            }

            this.AddAjusteStockProducido(stock, linea.Producto, linea.Identificador, linea.Faixa, linea.Cantidad);
        }

        public virtual void RemoveStockProducido(LineaProducida linea)
        {
			int empresaInt = (int)this._ingreso.Empresa;

			Stock stock = this._uow.StockRepository.GetStock(empresaInt, linea.Producto, linea.Faixa, ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox, linea.Identificador);

            if (stock == null)
                return;

            stock.Cantidad = (stock.Cantidad ?? 0) - linea.Cantidad;
            stock.NumeroTransaccion = _uow.GetTransactionNumber();

            if (stock.Cantidad < 0)
                stock.Cantidad = 0;

            this._uow.StockRepository.UpdateStock(stock);
            this.AddAjusteStockProducido(stock, linea.Producto, linea.Identificador, linea.Faixa, -linea.Cantidad);
        }

        public virtual void UpdateStockProducido(LineaProducida linea, decimal cantidad)
        {
			int empresaInt = (int)this._ingreso.Empresa;

			Stock stock = this._uow.StockRepository.GetStock(empresaInt, linea.Producto, linea.Faixa, ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox, linea.Identificador);

            if (stock == null)
                throw new ValidationFailedException("Stock para el producto: {1}, identificador: {2} no encontrado", new string[] { Convert.ToString(this._ingreso.Empresa), linea.Producto, linea.Identificador });

            decimal cantidadStock = cantidad - linea.Cantidad;

            stock.Cantidad = (stock.Cantidad ?? 0) + cantidadStock;
            stock.NumeroTransaccion = _uow.GetTransactionNumber();

            this._uow.StockRepository.UpdateStock(stock);
            this.AddAjusteStockProducido(stock, linea.Producto, linea.Identificador, linea.Faixa, cantidadStock);
        }

        public virtual void AddAjusteStockProducido(Stock stock, string producto, string identificador, decimal faixa, decimal cantidad)
        {
            var vlSerializado = JsonConvert.SerializeObject(new
            {
                NU_PRDC_INGRESO = this._ingreso.Id,
                CD_PRDC_FORMULA = this._ingreso.Formula.Id,
                DT_FABRICACAO = stock.Vencimiento,
                TP_PRODUCTO = "PRODUCIDO"
            });
            string predio = _uow.UbicacionRepository.GetPredio(stock.Ubicacion);

			int empresaInt = (int)this._ingreso.Empresa;

            var ajuste = new AjusteStock
            {

				Producto = producto,
                Faixa = faixa,
                Identificador = identificador,
                Empresa = empresaInt,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Produccion,
                QtMovimiento = (-1) * cantidad,
                DescMotivo = $"Consumido BB Fmla: {this._ingreso.Formula.Id} Ingr: {this._ingreso.Id}",
                CdMotivoAjuste = MotivoAjusteDb.SinRegistrar,
                Ubicacion = stock.Ubicacion,
                Serializado = vlSerializado,
                FechaVencimiento = stock.Vencimiento,
                Predio = predio,
            };

            this._uow.AjusteRepository.Add(ajuste);
        }

        public virtual void RegistrarMovimientoBB(int empresa, decimal? faixa, string producto, string identificador, TipoMovimientosBlackBox tipo, decimal? cantidadMovimiento)
        {
            var movimiento = new MovimientoStockBlackBox()
            {
                CantidadMovimiento = cantidadMovimiento,
                CodigoEmpresa = empresa,
                CodigoFaixa = faixa,
                CodigoProducto = producto,
                FechaMovimiento = DateTime.Now,
                NumeroIdentificador = identificador,
                UbicacionDestino = ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox,
                UbicacionOrigen = ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox,
                TipoAccionMovimiento = tipo,
                Usuario = this._usuario,
                NumeroMovimientoBB = this._uow.MovimientoStockBBRepository.GetNumeroMovimiento(),
                Ingreso = this._ingreso.Id
            };

            this._uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimiento);
        }

        public virtual void AddNuevaLineaHistoricaProducido(LineaProducida lineaOriginal)
        {
            var nuevaLineaHistorica = new LineaProducidaHistorica()
            {
                Cantidad = lineaOriginal.Cantidad,
                Faixa = lineaOriginal.Faixa,
                FechaAlta = DateTime.Now,
                FechaProducido = lineaOriginal.FechaAlta,
                Identificador = lineaOriginal.Identificador,
                Iteracion = lineaOriginal.Iteracion,
                Pasada = lineaOriginal.Pasada,
                Producto = lineaOriginal.Producto,
                NumeroHistorico = this._uow.ProduccionRepository.ObtenerNumeroLineaProducidoHistorica(),
                FechaVencimiento = lineaOriginal.Vencimiento
            };

            this._uow.ProduccionRepository.AddProducidoHistorico(this._ingreso, nuevaLineaHistorica, _uow.GetTransactionNumber());
        }
    }
}
