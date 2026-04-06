using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class CamionesProductoPedidoPre340Query : QueryObject<V_PEDIDO_PRODUTO_CAMION, WISDB>
    {
        protected readonly int _empresa = -1;
        protected readonly string _cdCliente;
        protected readonly string _nuPedido;
        protected readonly string _camion = "";

        public CamionesProductoPedidoPre340Query(string nuPedido, string cdCliente, int empresa, string camion)
        {
            _nuPedido = nuPedido;
            if (!string.IsNullOrEmpty(camion))
            {
                _camion = camion;

            }

            _cdCliente = cdCliente;
            _empresa = empresa;
        }

        public CamionesProductoPedidoPre340Query()
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
            this.Query = context.V_PEDIDO_PRODUTO_CAMION.AsNoTracking();
            int.TryParse(_camion, out int cam);


            this.Query = this.Query.Where(x => x.NU_PEDIDO == _nuPedido &&

                                               x.CD_EMPRESA == _empresa &&
                                               x.CD_CLIENTE == _cdCliente
                                               && x.CD_CAMION == cam
                                              );


        }
    }
}
