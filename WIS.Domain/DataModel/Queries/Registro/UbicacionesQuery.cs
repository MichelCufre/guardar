using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class UbicacionesQuery : QueryObject<V_REG040_ENDERECO_ESTOQUE, WISDB>
    {
        protected string _idUbicacion;

        public UbicacionesQuery()
        {
        }

        public UbicacionesQuery(string idUbicacion)
        {
            this._idUbicacion = idUbicacion;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG040_ENDERECO_ESTOQUE.Select(d => d);

            if(this._idUbicacion != null)
            {
                this.Query = this.Query.Where(x => x.CD_ENDERECO == this._idUbicacion);
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
