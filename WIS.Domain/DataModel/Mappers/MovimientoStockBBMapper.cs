using WIS.Domain.ManejoStock;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.ManejoStock.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class MovimientoStockBBMapper : Mapper
    {
        public virtual MovimientoStockBlackBox MapToMovimientoBlackBox(T_PRDC_MOVIMIENTO_BB movimientoEntity)
        {
            if (movimientoEntity == null)
                return null;

            return this.SetPropertiesMovimiento(movimientoEntity);
        }

        public virtual T_PRDC_MOVIMIENTO_BB MapFromMovimientoBlackBox(MovimientoStockBlackBox movimiento)
        {
            return this.CrateEntityStock(movimiento);
        }

        public virtual MovimientoStockBlackBox SetPropertiesMovimiento(T_PRDC_MOVIMIENTO_BB movimientoEntity)
        {
            return new MovimientoStockBlackBox()
            {
                CantidadMovimiento = movimientoEntity.QT_MOVIMIENTO,
                CodigoEmpresa = movimientoEntity.CD_EMPRESA,
                CodigoFaixa = movimientoEntity.CD_FAIXA,
                CodigoProducto = movimientoEntity.CD_PRODUTO,
                FechaMovimiento = movimientoEntity.DT_MOVIMIENTO,
                NumeroIdentificador = movimientoEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NumeroMovimientoBB = movimientoEntity.NU_MOVIMIENTO_BB,
                Usuario = movimientoEntity.USERID,
                TipoAccionMovimiento = this.GetTipo(movimientoEntity.ND_ACCION_MOVIMIENTO),
                UbicacionDestino = movimientoEntity.CD_ENDERECO_DESTINO,
                UbicacionOrigen = movimientoEntity.CD_ENDERECO_ORIGEN,
                Ingreso = movimientoEntity.NU_PRDC_INGRESO,
                NumeroInterfazEjecucion = movimientoEntity.NU_INTERFAZ_EJECUCION
            };
        }

        public virtual T_PRDC_MOVIMIENTO_BB CrateEntityStock(MovimientoStockBlackBox movimiento)
        {
            if (movimiento == null)
                return null;

            return new T_PRDC_MOVIMIENTO_BB()
            {
                CD_EMPRESA = movimiento.CodigoEmpresa,
                CD_ENDERECO_DESTINO = movimiento.UbicacionDestino,
                CD_ENDERECO_ORIGEN = movimiento.UbicacionOrigen,
                CD_FAIXA = movimiento.CodigoFaixa,
                CD_PRODUTO = movimiento.CodigoProducto,
                DT_MOVIMIENTO = movimiento.FechaMovimiento,
                ND_ACCION_MOVIMIENTO = this.GetTipo(movimiento.TipoAccionMovimiento),
                NU_IDENTIFICADOR = movimiento.NumeroIdentificador,
                NU_MOVIMIENTO_BB = movimiento.NumeroMovimientoBB,
                USERID = movimiento.Usuario,
                QT_MOVIMIENTO = movimiento.CantidadMovimiento,
                NU_PRDC_INGRESO = movimiento.Ingreso,
                NU_INTERFAZ_EJECUCION = movimiento.NumeroInterfazEjecucion
            };
        }

        public virtual TipoMovimientosBlackBox GetTipo(string tipo)
        {
            switch (tipo)
            {
                case TiposMovimiento.INGRESOBB:
                    return TipoMovimientosBlackBox.IngresoBalckBox;
                case TiposMovimiento.RECHAZO_SANO:
                    return TipoMovimientosBlackBox.RechazoSano;
                case TiposMovimiento.RECHAZO_AVERIA:
                    return TipoMovimientosBlackBox.RechazoAveria;
                case TiposMovimiento.CONSUMO:
                    return TipoMovimientosBlackBox.Consumir;
                case TiposMovimiento.PRODUCIR:
                    return TipoMovimientosBlackBox.Producir;
                case TiposMovimiento.SALIDA_PRODUCTO:
                    return TipoMovimientosBlackBox.SalidaProduco;
                case TiposMovimiento.SALIDA_INSUMO:
                    return TipoMovimientosBlackBox.SalidaInsumo;
                case TiposMovimiento.SALIDA_PRODUCTO_AVERIADO:
                    return TipoMovimientosBlackBox.SalidaProductoAveria;
                case TiposMovimiento.SALIDA_INSUMO_AVERIADO:
                    return TipoMovimientosBlackBox.SalidaInsumoAveria;
                default:
                    return TipoMovimientosBlackBox.Unknown;
            }
        }
        public virtual string GetTipo(TipoMovimientosBlackBox tipo)
        {
            switch (tipo)
            {
                case TipoMovimientosBlackBox.IngresoBalckBox:
                    return TiposMovimiento.INGRESOBB;
                case TipoMovimientosBlackBox.RechazoSano:
                    return TiposMovimiento.RECHAZO_SANO;
                case TipoMovimientosBlackBox.RechazoAveria:
                    return TiposMovimiento.RECHAZO_AVERIA;
                case TipoMovimientosBlackBox.Consumir:
                    return TiposMovimiento.CONSUMO;
                case TipoMovimientosBlackBox.Producir:
                    return TiposMovimiento.PRODUCIR;
                case TipoMovimientosBlackBox.SalidaProduco:
                    return TiposMovimiento.SALIDA_PRODUCTO;
                case TipoMovimientosBlackBox.SalidaInsumo:
                    return TiposMovimiento.SALIDA_INSUMO;
                case TipoMovimientosBlackBox.SalidaProductoAveria:
                    return TiposMovimiento.SALIDA_PRODUCTO_AVERIADO;
                case TipoMovimientosBlackBox.SalidaInsumoAveria:
                    return TiposMovimiento.SALIDA_INSUMO_AVERIADO;
                case TipoMovimientosBlackBox.Semiacabado:
                    return TiposMovimiento.SALIDA_INSUMO_SEMIACABADO;
                case TipoMovimientosBlackBox.Consumible:
                    return TiposMovimiento.SALIDA_INSUMO_CONSUMIBLE;
                default:
                    return "";
            }
        }
    }
}
