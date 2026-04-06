using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAnularCambioDOC410Query : QueryObject<V_ROLLBACK_CAMBIO_DOC, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ROLLBACK_CAMBIO_DOC;
        }

        public virtual List<string[]> GetKeysRowsSelected(bool allSelected, List<string> keys, IFormatProvider formatProvider)
        {
            if (allSelected)
            {
                return this.GetResult()
                    .Select(r => string.Join("$", new string[] { r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.CD_EMPRESA.ToString(), r.NU_DOCUMENTO, r.TP_DOCUMENTO, r.NU_DOCUMENTO_CAMBIO, r.TP_DOCUMENTO_CAMBIO }))
                    .Except(keys)
                    .Select(w => w.Split('$')).ToList();
            }
            else
            {
                return this.GetResult()
                    .Select(r => string.Join("$", new string[] { r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.CD_EMPRESA.ToString(), r.NU_DOCUMENTO, r.TP_DOCUMENTO, r.NU_DOCUMENTO_CAMBIO, r.TP_DOCUMENTO_CAMBIO }))
                    .Intersect(keys)
                    .Select(w => w.Split('$')).ToList();
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
