using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class UsuarioEmpresaQuery : QueryObject<V_REG100_EMPRESAS, WISDB>
    {
        protected readonly int? _idUsuario;
        protected readonly bool _modo;
        public UsuarioEmpresaQuery()
        {

        }
        public UsuarioEmpresaQuery(int idUsuario, bool modo)
        {
            this._idUsuario = idUsuario;
            this._modo = modo;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG100_EMPRESAS;
            if (this._idUsuario != null)
            {
                var listEmpresas = context.T_EMPRESA_FUNCIONARIO.Where(x => x.USERID == this._idUsuario).Select(x => x.CD_EMPRESA).ToList();
                if (this._modo)
                    this.Query = this.Query.Where(x => listEmpresas.Contains(x.CD_EMPRESA));
                else
                    this.Query = this.Query.Where(x => !listEmpresas.Contains(x.CD_EMPRESA));
            }
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
