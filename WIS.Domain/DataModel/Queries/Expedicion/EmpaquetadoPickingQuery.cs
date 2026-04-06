using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EmpaquetadoPickingQuery : QueryObject<V_PRD_PEDIDO_LOTE_CONTENEDOR, WISDB>
    {
        private readonly int _nuContenedor = -1;
        private readonly int _nuPreparacion = -1;

        public EmpaquetadoPickingQuery(int nuContenedor, int nuPreparacion)
        {
            _nuContenedor = nuContenedor;
            _nuPreparacion = nuPreparacion;
        }
        public EmpaquetadoPickingQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRD_PEDIDO_LOTE_CONTENEDOR.AsNoTracking();

            if (_nuContenedor != -1 && _nuPreparacion != -1)
                this.Query = this.Query.Where(x => x.NU_CONTENEDOR == _nuContenedor && x.NU_PREPARACION == _nuPreparacion);

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
