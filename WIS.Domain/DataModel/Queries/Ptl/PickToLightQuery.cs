using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Ptl
{
    public class PickToLightQuery : QueryObject<V_PTL010_PICK_TO_LIGHT, WISDB>
    {
        public PickToLightQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PTL010_PICK_TO_LIGHT.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
