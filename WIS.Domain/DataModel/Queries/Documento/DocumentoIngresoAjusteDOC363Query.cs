using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoIngresoAjusteDOC363Query : QueryObject<V_DOC363_DOCUMENTO_INGRESO, WISDB>
    {
        protected readonly List<string> _documentosFiltro;

        public DocumentoIngresoAjusteDOC363Query(List<string> documentosFiltro)
        {
            this._documentosFiltro = documentosFiltro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC363_DOCUMENTO_INGRESO
                        .Where(a => this._documentosFiltro.Contains(a.VL_FILTRO));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
