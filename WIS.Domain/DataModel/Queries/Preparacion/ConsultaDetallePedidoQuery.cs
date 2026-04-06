using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class ConsultaDetallePedidoQuery : QueryObject<V_PRE150_DETALLE_PEDIDO, WISDB>
    {
        protected readonly string _idPedido;
        protected readonly string _idCliente;
        protected readonly int? _idEmpresa;

        public ConsultaDetallePedidoQuery()
        {
        }

        public ConsultaDetallePedidoQuery(int idEmpresa, string idPedido, string idCliente)
        {
            this._idCliente = idCliente;
            this._idEmpresa = idEmpresa;
            this._idPedido = idPedido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE150_DETALLE_PEDIDO.AsNoTracking();

            if (!string.IsNullOrEmpty(_idPedido) && _idEmpresa != null && !string.IsNullOrEmpty(_idCliente))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.CD_CLIENTE == _idCliente && x.NU_PEDIDO == _idPedido);
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
