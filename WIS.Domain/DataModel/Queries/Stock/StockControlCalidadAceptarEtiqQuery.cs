using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockControlCalidadAceptarEtiqQuery : QueryObject<V_STO060_CTR_CALIDAD_ETIQ, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO060_CTR_CALIDAD_ETIQ.Where(x => x.ID_ACEPTADO == "N");
        }

        public virtual void ExcludeSelection(List<int> controlesExcluir)
        {
            if (controlesExcluir.Any())
                this.Query = this.Query.Where(d => !controlesExcluir.Contains(d.NU_CTR_CALIDAD_PENDIENTE));
        }

        public virtual List<int> GetIdControles()
        {
            return this.Query.Select(d => d.NU_CTR_CALIDAD_PENDIENTE).ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
