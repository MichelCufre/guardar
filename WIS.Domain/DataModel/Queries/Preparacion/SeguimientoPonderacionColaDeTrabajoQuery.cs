using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class SeguimientoPonderacionColaDeTrabajoQuery : QueryObject<V_CALCULO_PONDERACION_PRE812, WISDB>
    {
        public int _cdEmpresa;
        public string _nuPedido;
        public string _cdCliente;


        public SeguimientoPonderacionColaDeTrabajoQuery(int cdEmpresa, string cdCliente, string nuPedido)
        {
            this._cdCliente = cdCliente;
            this._cdEmpresa = cdEmpresa;
            this._nuPedido = nuPedido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CALCULO_PONDERACION_PRE812.Where(w => w.CD_CLIENTE == this._cdCliente && w.CD_EMPRESA == this._cdEmpresa && w.NU_PEDIDO == this._nuPedido);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
