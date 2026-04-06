using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class DetallesPedidosMostrador : QueryObject<V_DET_CONTENEDORES_WEXP330, WISDB>
    {

        protected string _NuPedido;
        protected string _CdCliente;
        protected int _CdEmpresa;

        public DetallesPedidosMostrador(string NuPedido, string CdCliente, int CdEmpresa)
        {
            this._NuPedido = NuPedido;
            this._CdCliente = CdCliente;
            this._CdEmpresa = CdEmpresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_CONTENEDORES_WEXP330.Where(w => w.CD_CLIENTE == _CdCliente && w.CD_EMPRESA == _CdEmpresa && w.NU_PEDIDO == _NuPedido); ;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
