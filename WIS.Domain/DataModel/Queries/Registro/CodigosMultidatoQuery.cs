using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class CodigosMultidatoQuery : QueryObject<V_CODIGO_MULTIDATO_EMPRESA_DET, WISDB>
    {
        protected int _codigoEmpresa;

        public CodigosMultidatoQuery(int codigoEmpresa)
        {
            this._codigoEmpresa = codigoEmpresa;
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_CODIGO_MULTIDATO_EMPRESA_DET.Where(w => w.CD_EMPRESA == _codigoEmpresa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
