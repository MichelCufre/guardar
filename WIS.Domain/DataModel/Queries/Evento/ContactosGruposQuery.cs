using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ContactosGruposQuery : QueryObject<V_CONTACTO_GRUPO_WEVT030, WISDB>
    {
        protected int? _NuGrupo;

        public ContactosGruposQuery(int? nuGrupo)
        {
            this._NuGrupo = nuGrupo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CONTACTO_GRUPO_WEVT030.Where(w => w.NU_CONTACTO_GRUPO == _NuGrupo);
        }

        public virtual List<int> GetSelectedKeys(List<int> keysToSelect)
        {
            return this.GetResult()
                .Select(c => c.NU_CONTACTO)
                .Intersect(keysToSelect)
                .ToList();
        }

        public virtual List<int> GetSelectedKeysAndExclude(List<int> keysToExclude)
        {
            return this.GetResult()
                .Select(c => c.NU_CONTACTO)
                .Except(keysToExclude)
                .ToList();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
