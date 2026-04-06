using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Produccion;
using WIS.Persistence.Database;
using WIS.Security;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class StockEspacioProduccionQuery : QueryObject<V_PRD113_PRODUCTOS_EXPULSABLE, WISDB>
    {
        protected readonly string _ubicacionProduccion;

        public StockEspacioProduccionQuery(string ubicacionProduccion)
        {
            _ubicacionProduccion = ubicacionProduccion;
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_PRD113_PRODUCTOS_EXPULSABLE.Where(x => x.CD_ENDERECO == _ubicacionProduccion);
        }

        public virtual int GetCount()
        {
            if (Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return Query.Count();
        }

        public virtual List<ProductosExpulsable> GetProductosExpulsable(IIdentityService identity)
        {
            return this.Query.Select(d => new ProductosExpulsable
            {
                Ubicacion = d.CD_ENDERECO,
                Empresa = d.CD_EMPRESA,
                Producto = d.CD_PRODUTO,
                Faixa = d.CD_FAIXA,
                Identificador = d.NU_IDENTIFICADOR,
                Vencimiento = d.DT_FABRICACAO
            }).ToList();
        }
    }
}
