using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PedidosCompatiblesLiberacionQuery : QueryObject<V_PRE051_PEDIDOS_COMPATIBLES, WISDB>
    {
        protected readonly short? _cdOnda;
        protected readonly int? _idEmpresa;
        public PedidosCompatiblesLiberacionQuery()
        {

        }
        public PedidosCompatiblesLiberacionQuery(int idEmpresa, short cdOnda)
        {
            this._idEmpresa = idEmpresa;
            this._cdOnda = cdOnda;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE051_PEDIDOS_COMPATIBLES;
            if(_idEmpresa != null && _cdOnda!= null)
            {
                this.Query = this.Query.Where(x => x.CD_ONDA == _cdOnda && x.CD_EMPRESA == _idEmpresa);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> SeleccionPedidoCompatible(out bool valido)
        {
            valido = false;
            List<string> mensajeArgumento = new List<string>();            
            var pedidoCompatible = this.Query.FirstOrDefault(x => x.CD_EMPRESA_ESPECIFICADO == this._idEmpresa && x.CD_ONDA_ESPECIFICADO == this._cdOnda);

            if (pedidoCompatible != null)
            {
                valido = false;
                mensajeArgumento = new List<string> { pedidoCompatible.CD_PRODUTO, pedidoCompatible.NU_PEDIDO_AUTO, pedidoCompatible.CD_CLIENTE_AUTO, pedidoCompatible.NU_PEDIDO_ESPE, pedidoCompatible.CD_CLIENTE_ESPE };
            }
            else
            {
                valido = true;

            }

            return mensajeArgumento;
        }

    }
}
