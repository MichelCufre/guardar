using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAjusteSeleccionadosStockPositivoDOC362Query : QueryObject<V_DOC363_AJUSTE_ACTA, WISDB>
    {
        protected readonly List<int> _nroAjustes;

        public DocumentoAjusteSeleccionadosStockPositivoDOC362Query(List<int> nroAjustes)
        {
            this._nroAjustes = nroAjustes;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC363_AJUSTE_ACTA
                .Where(a => this._nroAjustes.Contains(a.NU_AJUSTE_STOCK));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
