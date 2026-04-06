using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class ReglaClientesDisponiblesQuery : QueryObject<V_PRE250_CLIENTES_DISPONIBLES, WISDB>
    {
        protected int _empresa;
        protected string _tpAgente;
        protected int _nuRegla;

        public ReglaClientesDisponiblesQuery(int nuRegla, int empresa, string tpAgente = null)
        {
            _empresa = empresa;
            _nuRegla = nuRegla;
            _tpAgente = tpAgente;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE250_CLIENTES_DISPONIBLES.Where(a => a.CD_EMPRESA == _empresa);

            if (!string.IsNullOrEmpty(_tpAgente))
                this.Query = this.Query.Where(c => c.TP_AGENTE == _tpAgente);

            this.Query = this.Query.GroupJoin(
                context.V_PRE250_CLIENTES_SELECIONADOS.Where(x => x.NU_REGLA == _nuRegla),
                cd => new { cd.CD_CLIENTE, cd.CD_EMPRESA },
                cs => new { cs.CD_CLIENTE, cs.CD_EMPRESA },
                (cd, css) => new { Disponible = cd, Seleccionados = css })
                .SelectMany(cdcss => cdcss.Seleccionados.DefaultIfEmpty(),
                (cd, cs) => new { Disponible = cd.Disponible, Seleccionado = cs })
                .Where(cdcs => cdcs.Seleccionado == null)
                .Select(cdcs => cdcs.Disponible);
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
                .Select(r => string.Join("$", new string[] { r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList(); //TODO: Ver de cambiar
        }
    }
}
