using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class PickingProductoQuery : QueryObject<V_EXP110_DET_PICKING, WISDB>
    {
        protected readonly PickingProductoQueryData _queryData;

        public PickingProductoQuery(PickingProductoQueryData queryData)
        {
            _queryData = queryData;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP110_DET_PICKING
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == _queryData.Contenedor
                    && x.NU_PREPARACION == _queryData.Preparacion
                    && x.CD_PRODUTO == _queryData.Producto
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

    public class PickingProductoQueryData
    {
        public int Contenedor { get; set; }
        public int Preparacion { get; set; }
        public string Producto { get; set; }
        public string ComparteContenedorEntregaDestino { get; set; }
        public bool FiltrarComparteContenedorEntrega { get; set; }
    }
}
