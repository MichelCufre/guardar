using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class PedidoProductoLoteQuery : QueryObject<V_PRD_PEDIDO_LOTE_CONTENEDOR, WISDB>
    {
        protected readonly PedidoProductoLoteQueryData _queryData;

        public PedidoProductoLoteQuery(PedidoProductoLoteQueryData queryData)
        {
            _queryData = queryData;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRD_PEDIDO_LOTE_CONTENEDOR
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == _queryData.Contenedor
                    && x.NU_PREPARACION == _queryData.Preparacion
                    && x.CD_PRODUTO == _queryData.Producto
                    && (!string.IsNullOrEmpty(_queryData.Pedido) ? x.NU_PEDIDO == _queryData.Pedido : true)
                    && (!string.IsNullOrEmpty(_queryData.Cliente) ? x.CD_CLIENTE == _queryData.Cliente : true)
                    && (_queryData.Empresa.HasValue ? x.CD_EMPRESA == _queryData.Empresa.Value : true)
                    && (!_queryData.FiltrarComparteContenedorEntrega ? true
                        : (!string.IsNullOrEmpty(_queryData.ComparteContenedorEntregaDestino) ? x.VL_COMPARTE_CONTENEDOR_ENTREGA == _queryData.ComparteContenedorEntregaDestino 
                            : string.IsNullOrEmpty(x.VL_COMPARTE_CONTENEDOR_ENTREGA))));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }

    public class PedidoProductoLoteQueryData
    {
        public int Contenedor { get; set; }
        public int Preparacion { get; set; }
        public string Producto { get; set; }
        public int? Empresa { get; set; }
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public string ComparteContenedorEntregaDestino { get; set; }
        public bool FiltrarComparteContenedorEntrega { get; set; }
    }
}
