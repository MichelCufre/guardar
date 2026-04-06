using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoPre340Query : QueryObject<V_DET_PEDIDO_PRE340, WISDB>
    {

        private readonly int _empresa = -1;
        protected readonly string _cdCliente;
        protected readonly string _nuPedido;

        public DetallePedidoPre340Query(string nuPedido, string cdCliente, int empresa)
        {
            _nuPedido = nuPedido;
            _cdCliente = cdCliente;

            _empresa = empresa;
        }

        public DetallePedidoPre340Query()
        {

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_PEDIDO_PRE340.AsNoTracking();
            this.Query = this.Query
                .Where(x => x.NU_PEDIDO == _nuPedido
                    && x.CD_EMPRESA == _empresa
                    && x.CD_CLIENTE == _cdCliente);
        }
    }
}
