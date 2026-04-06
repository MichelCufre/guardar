using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAjusteStockFiltroDOC363 : QueryObject<V_DOC363_AJUSTE_ACTA, WISDB>
    {
        protected readonly List<string> _filtro;
        protected readonly List<int> _nroAjustes;

        public DocumentoAjusteStockFiltroDOC363(List<string> filtro, List<int> nroAjustes)
        {
            this._filtro = filtro;
            this._nroAjustes = nroAjustes;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC363_AJUSTE_ACTA
                .Where(a => this._filtro.Contains(a.VL_FILTRO) && this._nroAjustes.Contains(a.NU_AJUSTE_STOCK));
        }
    }
}
