using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class PickingProductoQuery : QueryObject<V_REG050_PICKING_PRODUCTO, WISDB>
    {
        protected string _idProducto;
        protected int? _idEmpresa;

        public PickingProductoQuery()
        {
        }
        public PickingProductoQuery(int? idEmpresa)
        {
            this._idEmpresa = idEmpresa;
        }
        public PickingProductoQuery(string idProducto, int? idEmpresa)
        {
            this._idProducto = idProducto;
            this._idEmpresa = idEmpresa;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG050_PICKING_PRODUCTO.AsNoTracking().Select(d => d);

            if ((!string.IsNullOrEmpty(_idProducto)) && (this._idEmpresa != null))
            {
                this.Query = this.Query.Where(d => d.CD_PRODUTO == this._idProducto && d.CD_EMPRESA == this._idEmpresa);
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

