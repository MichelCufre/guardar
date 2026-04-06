using WIS.Domain.Facturacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ListaPrecioMapper
    {
        public ListaPrecioMapper()
        {

        }

        public virtual ListaPrecio MapToObject(T_FACTURACION_LISTA_PRECIO entity)
        {
            return new ListaPrecio
            {
                Id = entity.CD_LISTA_PRECIO,
                Descripcion = entity.DS_LISTA_PRECIO,
                FechaIngresado = entity.DT_ADDROW,
                FechaActualizacion = entity.DT_UPDROW,
                IdMoneda = entity.CD_MONEDA,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual T_FACTURACION_LISTA_PRECIO MapToEntity(ListaPrecio listaPrecio)
        {
            return new T_FACTURACION_LISTA_PRECIO
            {
                CD_LISTA_PRECIO = listaPrecio.Id,
                DS_LISTA_PRECIO = listaPrecio.Descripcion,
                DT_ADDROW = listaPrecio.FechaIngresado,
                DT_UPDROW = listaPrecio.FechaActualizacion,
                CD_MONEDA = listaPrecio.IdMoneda,
                NU_TRANSACCION = listaPrecio.NumeroTransaccion,
                NU_TRANSACCION_DELETE = listaPrecio.NumeroTransaccionDelete,
            };
        }
    }
}
