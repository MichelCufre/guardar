using Newtonsoft.Json;
using System;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
using WIS.Exceptions;

namespace WIS.Domain.Produccion
{
    public class ConfirmacionProduccion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIngreso _ingreso;
        protected readonly long _transaccion;
        protected readonly int _usuario;

        public ConfirmacionProduccion(IUnitOfWork uow, IIngreso ingreso, long transaccion, int usuario)
        {
            this._uow = uow;
            this._ingreso = ingreso;
            this._transaccion = transaccion;
            this._usuario = usuario;
        }

        public virtual void ConfirmarConsumoProduccion(string producto, decimal faixa, int empresa, string identificador, string ubicacion, decimal cantidad)
        {
            var stock = this._uow.StockRepository.GetStock(empresa, producto, faixa, ubicacion, identificador);

            this.ConfirmarLineasDeConsumo(producto, faixa, empresa, identificador);

            stock.NumeroTransaccion = _uow.GetTransactionNumber();

            this.UpdateStockConsumido(stock, cantidad);
            this.AddAjusteStockConsumo(stock, empresa, producto, identificador, faixa, cantidad);
        }

        public virtual void ConfirmarProducidoProduccion(string producto, decimal faixa, int empresa, string identificador, DateTime? vencimiento, string ubicacion, decimal cantidad)
        {
            var stock = this._uow.StockRepository.GetStock(empresa, producto, faixa, ubicacion, identificador);

            this.ConfirmarLineasDeProducido(producto, faixa, empresa, identificador);

            if (stock == null)
            {
                stock = this.CrearStockProducido(producto, faixa, empresa, identificador, vencimiento, ubicacion, cantidad);
            }
            else
            {
                stock.NumeroTransaccion = this._transaccion;
                this.UpdateStockProducido(stock, cantidad, vencimiento);
            }

            this.AddAjusteStockProducido(stock, empresa, producto, identificador, faixa, cantidad, vencimiento);
        }

        public virtual void ConfirmarLineasDeConsumo(string producto, decimal faixa, int empresa, string identificador)
        {
            var lineasConfirmar = this._ingreso.Consumidos
                .Where(c => c.Producto == producto
                    && c.Faixa == faixa
                    && c.Empresa == empresa
                    && c.Identificador == identificador)
                .ToList();

            //foreach (LineaConsumida consumo in lineasConfirmar)
            //{
            //    consumo.ConfirmarLinea((int)this._transaccion);
            //    this._uow.ProduccionRepository.UpdateConsumido(this._ingreso, consumo, this._transaccion);
            //}
        }

        public virtual void ConfirmarLineasDeProducido(string producto, decimal faixa, int empresa, string identificador)
        {
            var lineasConfirmar = this._ingreso.Producidos
                .Where(c => c.Producto == producto
                    && c.Faixa == faixa
                    && c.Empresa == empresa
                    && c.Identificador == identificador)
                .ToList();

            //foreach (LineaProducida consumo in lineasConfirmar)
            //{
            //    consumo.ConfirmarLinea((int)this._transaccion);
            //    this._uow.ProduccionRepository.UpdateProducido(this._ingreso, consumo, this._transaccion);
            //}
        }

        public virtual void UpdateStockConsumido(Stock stock, decimal cantidad)
        {
            if (stock.ReservaSalida < cantidad || stock.Cantidad < cantidad)
                throw new ValidationFailedException("General_Sec0_Error_NotEnoughProducto");

            stock.Cantidad -= cantidad;
            stock.ReservaSalida -= cantidad;

            this._uow.StockRepository.UpdateStock(stock);
        }

        public virtual void AddAjusteStockConsumo(Stock stock, int empresa, string producto, string identificador, decimal faixa, decimal cantidad)
        {
            var vlSerializado = JsonConvert.SerializeObject(new
            {
                NU_PRDC_INGRESO = this._ingreso.Id,
                CD_PRDC_FORMULA = this._ingreso.Formula.Id,
                DT_FABRICACAO = stock.Vencimiento,
                TP_PRODUCTO = "CONSUMIDO"
            });
            string predio = _uow.UbicacionRepository.GetPredio(stock.Ubicacion);

            var ajuste = new AjusteStock
            {
                NuAjusteStock = this._uow.AjusteRepository.GetNextNuAjuste(),
                Producto = producto,
                Faixa = faixa,
                Identificador = identificador,
                Empresa = empresa,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Produccion,
                QtMovimiento = (-1) * cantidad,
                DescMotivo = $"Consumido WB Fmla: {this._ingreso.Formula.Id} Ingr: {this._ingreso.Id}",
                CdMotivoAjuste = MotivoAjusteDb.SinRegistrar,
                Ubicacion = stock.Ubicacion,
                Serializado = vlSerializado,
                FechaVencimiento = stock.Vencimiento,
                Predio = predio,
            };

            this._uow.AjusteRepository.Add(ajuste);
        }

        public virtual void UpdateStockProducido(Stock stock, decimal cantidad, DateTime? vencimiento)
        {
            stock.Cantidad = stock.Cantidad + cantidad;
            stock.FechaModificacion = DateTime.Now;
            stock.Vencimiento = (vencimiento != null ? vencimiento : stock.Vencimiento);

            this._uow.StockRepository.UpdateStock(stock);
        }

        public virtual Stock CrearStockProducido(string producto, decimal faixa, int empresa, string identificador, DateTime? vencimiento, string ubicacion, decimal cantidad)
        {
            Stock nuevoStock = new Stock()
            {
                Vencimiento = vencimiento,
                Cantidad = cantidad,
                Empresa = empresa,
                Faixa = faixa,
                Identificador = identificador,
                Producto = producto,
                Ubicacion = ubicacion,
                NumeroTransaccion = this._transaccion,
                Averia = "N",
                Inventario = "R",
                ControlCalidad = EstadoControlCalidad.Controlado,
                FechaInventario = DateTime.Now,
                CantidadTransitoEntrada = 0,
                ReservaSalida = 0,
                FechaModificacion = DateTime.Now,
            };

            this._uow.StockRepository.AddStock(nuevoStock);

            return nuevoStock;
        }

        public virtual void AddAjusteStockProducido(Stock stock, int empresa, string producto, string identificador, decimal faixa, decimal cantidad, DateTime? vencimiento)
        {
            var vlSerializado = JsonConvert.SerializeObject(new
            {
                NU_PRDC_INGRESO = this._ingreso.Id,
                CD_PRDC_FORMULA = this._ingreso.Formula.Id,
                DT_FABRICACAO = (vencimiento != null ? vencimiento : stock.Vencimiento),
                TP_PRODUCTO = "PRODUCIDO"
            });
            string predio = _uow.UbicacionRepository.GetPredio(stock.Ubicacion);

            var ajuste = new AjusteStock
            {
                NuAjusteStock = this._uow.AjusteRepository.GetNextNuAjuste(),
                Producto = producto,
                Faixa = faixa,
                Identificador = identificador,
                Empresa = empresa,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Produccion,
                QtMovimiento = cantidad,
                DescMotivo = $"Producido WB Fmla: {this._ingreso.Formula.Id} Ingr: {this._ingreso.Id}",
                CdMotivoAjuste = MotivoAjusteDb.SinRegistrar,
                Ubicacion = stock.Ubicacion,
                Serializado = vlSerializado,
                FechaVencimiento = stock.Vencimiento,
                Predio = predio
            };

            this._uow.AjusteRepository.Add(ajuste);
        }
    }
}
