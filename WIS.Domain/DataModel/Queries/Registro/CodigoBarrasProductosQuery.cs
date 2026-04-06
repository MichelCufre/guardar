using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class CodigoBarrasProductosQuery : QueryObject<V_REG603_CODIGO_BARRAS, WISDB>
    {
        protected string _idProducto;
        protected int? _idEmpresa;
        public CodigoBarrasProductosQuery()
        {
        }

        public CodigoBarrasProductosQuery(int? idEmpresa, string idProducto)
        {
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
        }
        public CodigoBarrasProductosQuery(string idProducto)
        {
            this._idProducto = idProducto;
        }
        public CodigoBarrasProductosQuery(int? idEmpresa)
        {
            this._idEmpresa = idEmpresa;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG603_CODIGO_BARRAS.AsNoTracking().Select(d => d);

            if ((_idEmpresa != null) && (!string.IsNullOrEmpty(_idProducto)))
            {
                this.Query = this.Query.Where(d => d.CD_PRODUTO == _idProducto && d.CD_EMPRESA == _idEmpresa);

            }
            else if (_idEmpresa != null)
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == _idEmpresa);

            }
            else if (!string.IsNullOrEmpty(_idProducto))
            {
                this.Query = this.Query.Where(d => d.CD_PRODUTO == _idProducto);
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
