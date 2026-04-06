using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class AvancePreparacionesPorEmpresa : QueryObject<V_PRE161_PICKING_PENDIENTE, WISDB>
    {
        protected readonly int? _empresa;
        public AvancePreparacionesPorEmpresa()
        {

        }
        public AvancePreparacionesPorEmpresa(int empresa)
        {
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE161_PICKING_PENDIENTE;

            if (this._empresa != null)
            {
                this.Query = context.V_PRE161_PICKING_PENDIENTE.Where(s => s.CD_EMPRESA == _empresa);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
