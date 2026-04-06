using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class GruposParametrosReglaQuery : QueryObject<V_REG300_PARAMETROS_REGLA, WISDB>
    {
        protected readonly long _nuRegla;

        public GruposParametrosReglaQuery(long nuRegla)
        {
            _nuRegla = nuRegla;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG300_PARAMETROS_REGLA.Where(d => d.NU_GRUPO_REGLA == _nuRegla);

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
