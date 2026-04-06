using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking.Dtos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoLpnAtributoQuery : QueryObject<V_PRE100_DET_PEDIDO_LPN_ATRIB, WISDB>
    {
        protected readonly DetallePedidoLpnEspecifico _datos;

        public DetallePedidoLpnAtributoQuery(DetallePedidoLpnEspecifico datos)
        {
            this._datos = datos;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_DET_PEDIDO_LPN_ATRIB
                .Where(x => x.NU_PEDIDO == this._datos.Pedido
                    && x.CD_CLIENTE == this._datos.Cliente
                    && x.CD_EMPRESA == this._datos.Empresa
                    && x.CD_PRODUTO == this._datos.Producto
                    && x.CD_FAIXA == this._datos.Faixa
                    && x.NU_IDENTIFICADOR == this._datos.Identificador
                    && x.ID_ESPECIFICA_IDENTIFICADOR == this._datos.IdEspecificaIdentificador
                    && x.TP_LPN_TIPO == this._datos.TipoLpn
                    && x.ID_LPN_EXTERNO == this._datos.IdExternoLpn);

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
