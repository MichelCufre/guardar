using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class CamionesPedidoPre340Query : QueryObject<V_PEDIDO_FACTURADO_CONSOLIDADO, WISDB>
    {
        protected readonly int _empresa = -1;
        protected readonly string _cdCliente;
        protected readonly string _nuPedido;

        public CamionesPedidoPre340Query(string nuPedido, string cdCliente, int empresa)
        {
            _nuPedido = nuPedido;
            _cdCliente = cdCliente;
            _empresa = empresa;
        }

        public CamionesPedidoPre340Query()
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
            this.Query = context.V_PEDIDO_FACTURADO_CONSOLIDADO.AsNoTracking();

            this.Query = this.Query.Where(x => x.NU_PEDIDO == _nuPedido &&

                                               x.CD_EMPRESA == _empresa &&
                                               x.CD_CLIENTE == _cdCliente
                                              );

        }
    }
}
