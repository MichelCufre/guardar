using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class UbicacionPickingProductoMapper : Mapper
    {
        public UbicacionPickingProductoMapper()
        {

        }

        public virtual UbicacionPickingProducto MapToObject(T_PICKING_PRODUTO entity)
        {
            if (entity == null)
                return null;

            return new UbicacionPickingProducto
            {
                Id = entity.NU_SEC_PICKING_PRODUTO,
                Empresa = entity.CD_EMPRESA,
                CodigoProducto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Padron = entity.QT_PADRAO_PICKING,
                UbicacionSeparacion = entity.CD_ENDERECO_SEPARACAO,
                StockMinimo = entity.QT_ESTOQUE_MINIMO,
                StockMaximo = entity.QT_ESTOQUE_MAXIMO,
                CantidadDesborde = entity.QT_DESBORDE,
                TipoPicking = entity.TP_PICKING,
                FechaModificacion = entity.DT_UPDROW,
                FechaInsercion = entity.DT_ADDROW,
                Predio = entity.NU_PREDIO,
                CantidadPadronDesborde = entity.QT_PADRON_DESBORDE,
                NuTransaccion = entity.NU_TRANSACCION,
                NuTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                CodigoUnidadCajaAutomatismo = entity.CD_UNIDAD_CAJA_AUT,
                CantidadUnidadCajaAutomatismo = entity.QT_UNIDAD_CAJA_AUT,                
                FlagConfirmarCodBarrasAutomatismo = entity.FL_CONF_CD_BARRAS_AUT,
                Prioridad = entity.NU_PRIORIDAD
            };
        }

        public virtual T_PICKING_PRODUTO MapToEntity(UbicacionPickingProducto ubicacion)
        {
            if (ubicacion == null) return null;

            return new T_PICKING_PRODUTO
            {
                NU_SEC_PICKING_PRODUTO = ubicacion.Id,
                CD_EMPRESA = ubicacion.Empresa,
                CD_PRODUTO = ubicacion.CodigoProducto,
                CD_FAIXA = ubicacion.Faixa,
                QT_PADRAO_PICKING = ubicacion.Padron,
                CD_ENDERECO_SEPARACAO = ubicacion.UbicacionSeparacion,
                QT_ESTOQUE_MINIMO = ubicacion.StockMinimo,
                QT_ESTOQUE_MAXIMO = ubicacion.StockMaximo,
                QT_DESBORDE = ubicacion.CantidadDesborde,
                TP_PICKING = ubicacion.TipoPicking,
                DT_UPDROW = ubicacion.FechaModificacion,
                DT_ADDROW = ubicacion.FechaInsercion,
                NU_PREDIO = ubicacion.Predio,
                QT_PADRON_DESBORDE = ubicacion.CantidadPadronDesborde,
                NU_TRANSACCION = ubicacion.NuTransaccion,
                NU_TRANSACCION_DELETE = ubicacion.NuTransaccionDelete,
                CD_UNIDAD_CAJA_AUT = ubicacion.CodigoUnidadCajaAutomatismo,
                QT_UNIDAD_CAJA_AUT = ubicacion.CantidadUnidadCajaAutomatismo,
                FL_CONF_CD_BARRAS_AUT = ubicacion.FlagConfirmarCodBarrasAutomatismo,
                NU_PRIORIDAD = ubicacion.Prioridad
            };
        }
    }
}
