using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ConfiguracionVentanaAgentesQuery : QueryObject<V_CLIENTE_CONFIG_DIAS_VALIDEZ, WISDB>
    {
        protected string _idCliente;
        protected int? _idEmpresa;

        public ConfiguracionVentanaAgentesQuery()
        {
        }
        public ConfiguracionVentanaAgentesQuery(string idCliente, int? idEmpresa)
        {
            this._idCliente = idCliente;
            this._idEmpresa = idEmpresa;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CLIENTE_CONFIG_DIAS_VALIDEZ.Select(d => d);

            if ((this._idCliente != null) && (this._idEmpresa != null))
            {
                this.Query = this.Query.Where(d => d.CD_CLIENTE == this._idCliente && d.CD_EMPRESA == this._idEmpresa);
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
