using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class ReglaClientesSeleccionadosQuery : QueryObject<V_PRE250_CLIENTES_SELECIONADOS, WISDB>
    {
        protected int _nuRegla;
        public ReglaClientesSeleccionadosQuery(int nuRegla)
        {
            _nuRegla = nuRegla;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE250_CLIENTES_SELECIONADOS.Where(s => s.NU_REGLA == _nuRegla);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { _nuRegla.ToString(), r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { _nuRegla.ToString(), r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList(); //TODO: Ver de cambiar
        }
    }
}
