using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaContainersQuery : QueryObject<T_CONTAINER, WISDB>
    {
        protected readonly List<int> _containers;

        public PorteriaContainersQuery(List<int> containers)
        {
            this._containers = containers;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_CONTAINER.Where(w => _containers.Contains(w.NU_SEQ_CONTAINER));
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
