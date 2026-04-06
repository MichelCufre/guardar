using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PRE052AsociarPedidosQuery : QueryObject<V_PRE052_PEDIDOS_DISPONIBLES, WISDB>
    {
        protected readonly int _prep;
        protected readonly int _emp;

        public PRE052AsociarPedidosQuery(int idPrep, int emp)
        {
            this._prep = idPrep;
            _emp = emp;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE052_PEDIDOS_DISPONIBLES.Where(d => (d.NU_PREPARACION_MANUAL != this._prep) && d.CD_EMPRESA == _emp); ;
        }

        public virtual List<Pedido> GetPedidos()
        {
            return this.Query.Select(d => new Pedido
            {
                Id = d.NU_PEDIDO,
                Cliente = d.CD_CLIENTE,
                Empresa = d.CD_EMPRESA
            }).ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
