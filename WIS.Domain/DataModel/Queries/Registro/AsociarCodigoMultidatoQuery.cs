using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class AsociarCodigoMultidatoQuery : QueryObject<V_CODIGO_MULTIDATO_ASOCIADOS, WISDB>
    {
        protected int _empresa;
        protected string _flAsociado;


        public AsociarCodigoMultidatoQuery(string flAsociado, int empresa)
        {
            this._flAsociado = flAsociado;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CODIGO_MULTIDATO_ASOCIADOS.Where(d => d.CD_EMPRESA == _empresa && d.FL_ASOCIADO == _flAsociado);

        }
        public virtual List<string> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return (from c in this.Query
                    where !keysToExclude.Contains(c.CD_CODIGO_MULTIDATO)
                    select c.CD_CODIGO_MULTIDATO).ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
