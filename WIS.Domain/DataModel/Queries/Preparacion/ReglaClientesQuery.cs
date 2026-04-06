using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class ReglaClientesQuery : QueryObject<V_PRE250_CLIENTES_DISPONIBLES, WISDB>
    {
        protected List<string> _keys;
        protected int? _empresa;
        protected string _tpAgente;

        public ReglaClientesQuery(List<string> keys = null, int? empresa = null, string tpAgente = null)
        {
            this._keys = keys ?? new List<string>();
            this._empresa = empresa;
            this._tpAgente = tpAgente;
        }
        public override void BuildQuery(WISDB context)
        {
            // if(_nuRegla != null)
            //_keys = _keys.Union(context.T_REGLA_CLIENTES.Where(s=>s.NU_REGLA == (int)_nuRegla).Select(a=>a.CD_CLIENTE + "$" + a.CD_EMPRESA)).ToList();
            this.Query = context.V_PRE250_CLIENTES_DISPONIBLES.Where(s => !_keys.Contains(s.KEY));

            if (this._empresa != null)
                this.Query = this.Query.Where(c => c.CD_EMPRESA == this._empresa);

            if (!string.IsNullOrEmpty(_tpAgente))
                this.Query = this.Query.Where(c => c.TP_AGENTE == _tpAgente);
        }

        public virtual List<string> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.Query.AsEnumerable().Select(r => r.KEY).Intersect(keysToSelect).ToList();
        }

        public virtual List<string> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.Query.AsEnumerable().Select(r => r.KEY).Except(keysToExclude).ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
