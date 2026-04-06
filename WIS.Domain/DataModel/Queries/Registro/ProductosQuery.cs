using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ProductosQuery : QueryObject<V_REG009_PRODUCTOS, WISDB>
    {
        protected string _idProducto;
        protected int? _idEmpresa;

        public ProductosQuery()
        {
        }
        public ProductosQuery(int? idEmpresa)
        {
            this._idEmpresa = idEmpresa;
        }
        public ProductosQuery(string idProducto, int? idEmpresa)
        {
            this._idProducto = idProducto;
            this._idEmpresa = idEmpresa;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG009_PRODUCTOS.Select(d => d);

            if ((!string.IsNullOrEmpty(_idProducto)) && (this._idEmpresa != null))
            {
                this.Query = this.Query.Where(d => d.CD_PRODUTO == this._idProducto && d.CD_EMPRESA == this._idEmpresa);
            }
            else if (this._idEmpresa != null)
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._idEmpresa);
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
