using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaPreRegistroEntradaContainersQuery : QueryObject<T_CONTAINER, WISDB>
    {
        protected readonly List<int> _containers;

        public PorteriaPreRegistroEntradaContainersQuery(List<int> containers)
        {
            this._containers = containers;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_CONTAINER.Where(w => _containers.Contains(w.NU_SEQ_CONTAINER));
        }

    }
}
