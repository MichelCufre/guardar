using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaRegistroEntradaPersonasQuery : QueryObject<V_PORTERIA_REGISTRO_PERSONA, WISDB>
    {
        protected readonly List<int> _personas;

        public PorteriaRegistroEntradaPersonasQuery(List<int> personas)
        {
            this._personas = personas;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_REGISTRO_PERSONA.Where(w => _personas.Contains(w.NU_POTERIA_PERSONA));
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
