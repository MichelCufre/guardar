using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PRE350StockPickingReabastMapper : Mapper
    {
        public PRE350StockPickingReabastMapper()
        {

        }

        public virtual PRE350StockPickingReabast MapToObject(V_PRE350_STOCK_PICKING_REABAST e)
        {
            if (e == null)
                return null;

            return new PRE350StockPickingReabast
            {
                Empresa = e.CD_EMPRESA,
                NombreEmpresa = e.NM_EMPRESA,
                Producto = e.CD_PRODUTO,
                DescProducto = e.DS_PRODUTO,
                Faixa = e.CD_FAIXA,
                NuPredioPi = e.NU_PREDIO_PI,
                EnderecoPicking = e.CD_ENDERECO_PI,
                QtMinimoPi = e.QT_MINIMO_PI,
                QtMaximoPi = e.QT_MAXIMO_PI,
                QtPadraoPi = e.QT_PADRAO_PI,
                QtDesbordePi = e.QT_DESBORDE_PI,
                IdentificadorPi = e.NU_IDENTIFICADOR_PI?.Trim()?.ToUpper(),
                QtFisicoPi = e.QT_FISICO_PI,
                QtSalidaPi = e.QT_SALIDA_PI,
                QtEntradaPi = e.QT_ENTRADA_PI,
                QtDisponiblePi = e.QT_DESBORDE_PI,
                FlNecesitaUrgente = MapStringToBoolean(e.FL_NECESITA_URGENTE),
                FlNecesitaMinimo = MapStringToBoolean(e.FL_NECESITA_MINIMO),
                FlNecesitaForzado = MapStringToBoolean(e.FL_NECESITA_FORZADO),
                QtDisponibleStock = e.QT_DISPONIBLE_STOCK,
                VlPorcentajeForzado = e.VL_PORCENTAJE_FORZADO
            };
        }

        public virtual V_PRE350_STOCK_PICKING_REABAST MapToEntity(PRE350StockPickingReabast o)
        {
            if (o == null)
                return null;

            return new V_PRE350_STOCK_PICKING_REABAST
            {
                CD_EMPRESA = o.Empresa,
                NM_EMPRESA = o.NombreEmpresa,
                CD_PRODUTO = o.Producto,
                DS_PRODUTO = o.DescProducto,
                CD_FAIXA = o.Faixa,
                NU_PREDIO_PI = o.NuPredioPi,
                CD_ENDERECO_PI = o.EnderecoPicking,
                QT_MAXIMO_PI = o.QtMaximoPi,
                QT_MINIMO_PI = o.QtMinimoPi,
                QT_PADRAO_PI = o.QtPadraoPi,
                QT_DESBORDE_PI = o.QtDesbordePi,
                NU_IDENTIFICADOR_PI = o.IdentificadorPi?.Trim()?.ToUpper(),
                QT_FISICO_PI = o.QtFisicoPi,
                QT_SALIDA_PI = o.QtSalidaPi,
                QT_DISPONIBLE_PI = o.QtDisponiblePi,
                FL_NECESITA_URGENTE = MapBooleanToString(o.FlNecesitaUrgente),
                FL_NECESITA_MINIMO = MapBooleanToString(o.FlNecesitaMinimo),
                FL_NECESITA_FORZADO = MapBooleanToString(o.FlNecesitaForzado),
                QT_DISPONIBLE_STOCK = o.QtDisponibleStock,
                VL_PORCENTAJE_FORZADO = o.VlPorcentajeForzado
            };
        }
    }
}
