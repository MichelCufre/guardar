using WIS.Domain.Facturacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CotizacionListasMapper
    {
        public CotizacionListasMapper()
        {

        }

        public virtual CotizacionListas MapToObject(T_FACTURACION_LISTA_COTIZACION entity)
        {
            return new CotizacionListas
            {
                CodigoListaPrecio = entity.CD_LISTA_PRECIO,
                CodigoFacturacion = entity.CD_FACTURACION,
                NumeroComponente = entity.NU_COMPONENTE,
                Funcionario = entity.CD_FUNCIONARIO,
                CantidadImporte = entity.QT_IMPORTE,
                CantidadImporteMinimo = entity.QT_IMPORTE_MINIMO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual T_FACTURACION_LISTA_COTIZACION MapToEntity(CotizacionListas cotizacionListas)
        {
            return new T_FACTURACION_LISTA_COTIZACION
            {
                CD_LISTA_PRECIO = cotizacionListas.CodigoListaPrecio,
                CD_FACTURACION = cotizacionListas.CodigoFacturacion,
                NU_COMPONENTE = cotizacionListas.NumeroComponente,
                CD_FUNCIONARIO = cotizacionListas.Funcionario,
                QT_IMPORTE = cotizacionListas.CantidadImporte,
                QT_IMPORTE_MINIMO = cotizacionListas.CantidadImporteMinimo,
                DT_ADDROW = cotizacionListas.FechaAlta,
                DT_UPDROW = cotizacionListas.FechaModificacion,
            };
        }
    }
}
