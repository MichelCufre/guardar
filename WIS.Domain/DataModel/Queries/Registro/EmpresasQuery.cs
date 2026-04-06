using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class EmpresasQuery : QueryObject<V_REG100_EMPRESAS, WISDB>
    {
        protected int? _idEmpresa;
        public EmpresasQuery()
        {
        }

        public EmpresasQuery(int? idEmpresa)
        {
            this._idEmpresa = idEmpresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG100_EMPRESAS.Select(d => d);

            if (this._idEmpresa != null)
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == this._idEmpresa);
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
