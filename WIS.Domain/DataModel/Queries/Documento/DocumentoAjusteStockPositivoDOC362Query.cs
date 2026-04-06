using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAjusteStockPositivoDOC362Query : QueryObject<V_DOC362_DOCUMENTO_INGRESO, WISDB>
    {
        protected int _empresa { get; }

        public DocumentoAjusteStockPositivoDOC362Query(int empresa)
        {
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC362_DOCUMENTO_INGRESO
                .Where(a => a.CD_EMPRESA == this._empresa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
