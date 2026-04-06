using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class DetalleDominioQuery : QueryObject<V_REG911_DETALLE_DOMINIO, WISDB>
    {
        protected string _codigoDominio;

        public DetalleDominioQuery(string codigoDominio = "")
        {
            this._codigoDominio = codigoDominio;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG911_DETALLE_DOMINIO.AsNoTracking();

            if (!string.IsNullOrEmpty(this._codigoDominio))
                this.Query = this.Query.Where(d => d.CD_DOMINIO == this._codigoDominio);
            
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
