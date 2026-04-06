using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class PedidosExpedidosQuery : QueryObject<V_EXP041_PEDIDOS_EXPEDIDOS, WISDB>
    {
        protected readonly int? _idCamion;
        protected readonly int? _idEmpresa;
        protected readonly string _idPedido;
        protected readonly string _idProducto;

        public PedidosExpedidosQuery()
        {
        }
        public PedidosExpedidosQuery(int idCamion)
        {
            this._idCamion = idCamion;
        }
        public PedidosExpedidosQuery(int idEmpresa, string idPedido, string idProducto)
        {
            this._idEmpresa = idEmpresa;
            this._idPedido = idPedido;
            this._idProducto = idProducto;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP041_PEDIDOS_EXPEDIDOS.AsNoTracking();
            if (_idCamion != null)
            {
                this.Query = this.Query.Where(x => x.CD_CAMION == _idCamion);
            }
            else if ((_idEmpresa != null) && (!string.IsNullOrEmpty(_idPedido)) && (!string.IsNullOrEmpty(_idProducto)))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.CD_PRODUTO == _idProducto && x.NU_PEDIDO == _idPedido);
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
