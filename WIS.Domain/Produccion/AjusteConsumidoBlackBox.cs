using Newtonsoft.Json;
using NLog;
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
    public class AjusteConsumidoBlackBox
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IngresoBlackBox _ingreso;
        protected readonly int _usuario;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly AjusteMapper _mapper;

        public AjusteConsumidoBlackBox(IUnitOfWork uow, IngresoBlackBox ingreso, int usuario)
        {
            this._uow = uow;
            this._ingreso = ingreso;
            this._usuario = usuario;
            this._mapper = new AjusteMapper();
        }
         

        public virtual void CrearHistoricoDeConsumo(List<LineaConsumida> consumos)
        {
            foreach (var lineaConsumo in consumos)
            {
                this.AddNuevaLineaHistoricaConsumo(lineaConsumo);
                this._uow.ProduccionRepository.RemoveConsumo(this._ingreso, lineaConsumo);
            }
        }

        public virtual void UpdateLineaConsumo(LineaConsumida linea, decimal cantidad)
        {
            linea.Cantidad = cantidad;

            this._uow.ProduccionRepository.UpdateConsumido(this._ingreso, linea, _uow.GetTransactionNumber());
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
                Producto = producto,
                Faixa = faixa,
                Identificador = identificador,
                Empresa = empresa,
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

        public virtual void UpdateStockConsumido(Stock stock, decimal cantidad)
        {
            if ((stock.ReservaSalida ?? 0) < cantidad || (stock.Cantidad ?? 0) < cantidad)
                throw new ValidationFailedException("PRD210_grid1_error_NotEnoughProducto");

            stock.Cantidad = (stock.Cantidad ?? 0) - cantidad;
            stock.ReservaSalida = (stock.ReservaSalida ?? 0) - cantidad;
            stock.NumeroTransaccion = _uow.GetTransactionNumber();

            this._uow.StockRepository.UpdateStock(stock);
        }

        public virtual void RegistrarMovimientoBB(Stock stock, TipoMovimientosBlackBox tipo, decimal? cantidadMovimiento, string ubicacion = "", string fl_semiacabado = "N")
        {
            var movimiento = new MovimientoStockBlackBox()
            {
                CantidadMovimiento = cantidadMovimiento,
                CodigoEmpresa = stock.Empresa,
                CodigoFaixa = stock.Faixa,
                CodigoProducto = stock.Producto,
                FechaMovimiento = DateTime.Now,
                NumeroIdentificador = stock.Identificador,
                UbicacionDestino = ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox,
                UbicacionOrigen = ((LineaBlackBox)this._ingreso.Linea).UbicacionBlackBox,
                TipoAccionMovimiento = tipo,
                Usuario = this._usuario,
                NumeroMovimientoBB = this._uow.MovimientoStockBBRepository.GetNumeroMovimiento(),
                Ingreso = this._ingreso.Id,
            };

            if (!string.IsNullOrEmpty(ubicacion))
            {
                if (fl_semiacabado == "S")
                {
                    movimiento.TipoAccionMovimiento = TipoMovimientosBlackBox.Semiacabado;
                }
                else
                {
                    movimiento.TipoAccionMovimiento = TipoMovimientosBlackBox.Consumible;
                }

                movimiento.UbicacionOrigen = ubicacion;
            }

            this._uow.MovimientoStockBBRepository.AddMovimientoBlackBox(movimiento);
        }

        public virtual void AddNuevaLineaHistoricaConsumo(LineaConsumida lineaOriginal)
        {
            var nuevaLineaHistorica = new LineaConsumidaHistorica()
            {
                Cantidad = lineaOriginal.Cantidad,
                Faixa = lineaOriginal.Faixa,
                FechaAlta = DateTime.Now,
                FechaConsumo = lineaOriginal.FechaAlta,
                Identificador = lineaOriginal.Identificador,
                Iteracion = lineaOriginal.Iteracion,
                Pasada = lineaOriginal.Pasada,
                Producto = lineaOriginal.Producto,
                NumeroHistorico = this._uow.ProduccionRepository.ObtenerNumeroLineaConsumoHistorica()
            };

            this._uow.ProduccionRepository.AddConsumoHistorico(this._ingreso, nuevaLineaHistorica, _uow.GetTransactionNumber());
        }
    }
}
