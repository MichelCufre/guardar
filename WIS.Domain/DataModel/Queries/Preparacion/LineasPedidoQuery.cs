using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class LineasPedidoQuery : QueryObject<V_PRE101_DET_PEDIDO_SAIDA, WISDB>
    {
        protected readonly int? _idEmpresa;
        protected readonly string _pedido;
        protected readonly string _cliente;
        public LineasPedidoQuery()
        {

        }
        public LineasPedidoQuery(int empresa, string cliente, string pedido)
        {
            this._idEmpresa = empresa;
            this._cliente = cliente;
            this._pedido = pedido;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE101_DET_PEDIDO_SAIDA;
            if (!string.IsNullOrEmpty(_cliente) && _idEmpresa != null && !string.IsNullOrEmpty(_pedido))
            {
                this.Query = this.Query.Where(x => x.NU_PEDIDO == _pedido && x.CD_EMPRESA == _idEmpresa && x.CD_CLIENTE==_cliente);
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
