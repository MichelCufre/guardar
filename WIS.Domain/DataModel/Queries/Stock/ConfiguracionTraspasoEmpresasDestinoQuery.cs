using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConfiguracionTraspasoEmpresasDestinoQuery : QueryObject<V_REG100_EMPRESAS, WISDB>
    {
        protected readonly int? _idUsuario;
        protected readonly long _idConfig;
        protected readonly bool _asociado;

        public ConfiguracionTraspasoEmpresasDestinoQuery()
        {
        }

        public ConfiguracionTraspasoEmpresasDestinoQuery(int? idUsuario, long idConfig, bool asociado)
        {
            this._idUsuario = idUsuario;
            this._idConfig = idConfig;
            this._asociado = asociado;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG100_EMPRESAS
                .Where(e => context.T_EMPRESA_FUNCIONARIO
                    .Any(f => f.CD_EMPRESA == e.CD_EMPRESA && f.USERID == this._idUsuario));

            if (this._asociado)
                this.Query = this.Query
                    .Where(x => context.T_TRASPASO_CONF_DESTINO.Any(ime => ime.NU_TRASPASO_CONFIGURACION == _idConfig && ime.CD_EMPRESA == x.CD_EMPRESA));
            else
                this.Query = this.Query
                    .Where(x => !context.T_TRASPASO_CONF_DESTINO.Any(ime => ime.NU_TRASPASO_CONFIGURACION == _idConfig && ime.CD_EMPRESA == x.CD_EMPRESA));

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<int> GetEmpresas()
        {
            return this.Query.Select(x => x.CD_EMPRESA).ToList();
        }
    }
}
