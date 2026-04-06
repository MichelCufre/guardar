using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConfiguracionTraspasoTiposTraspasoQuery : QueryObject<V_STO800_TIPOS_TRASPASO, WISDB>
    {
        protected readonly long _idConfig;
        protected readonly bool _asociado;

        public ConfiguracionTraspasoTiposTraspasoQuery()
        {
        }

        public ConfiguracionTraspasoTiposTraspasoQuery( long idConfig, bool asociado)
        {
            this._idConfig = idConfig;
            this._asociado = asociado;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO800_TIPOS_TRASPASO;

            if (this._asociado)
                this.Query = this.Query
                    .Where(x => context.T_TRASPASO_CONF_TIPO_TRASPASO.Any(y => y.NU_TRASPASO_CONFIGURACION == _idConfig && y.TP_TRASPASO == x.TP_TRASPASO));
            else
                this.Query = this.Query
                    .Where(x => !context.T_TRASPASO_CONF_TIPO_TRASPASO.Any(y => y.NU_TRASPASO_CONFIGURACION == _idConfig && y.TP_TRASPASO == x.TP_TRASPASO));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetTiposTraspaso()
        {
            return this.Query.Select(x => x.TP_TRASPASO).ToList();
        }
    }
}
