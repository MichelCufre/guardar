using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ControlesDeCalidadEnproductoQuery : QueryObject<V_REG602_PRODUCTO_CONTROL_CALIDAD, WISDB>
    {
        protected string _idProducto;
        protected int? _idEmpresa;
        public ControlesDeCalidadEnproductoQuery()
        {
        }
        public ControlesDeCalidadEnproductoQuery(int? idEmpresa, string idProducto)
        {
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
        }
        public ControlesDeCalidadEnproductoQuery(int? idEmpresa)
        {
            this._idEmpresa = idEmpresa;
        }
        public ControlesDeCalidadEnproductoQuery(string idProducto)
        {
            this._idProducto = idProducto;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG602_PRODUCTO_CONTROL_CALIDAD.AsNoTracking().Select(d => d);

            if((_idEmpresa != null) && (!string.IsNullOrEmpty(_idProducto)))
            {
                this.Query = this.Query.Where(d => d.CD_PRODUTO == _idProducto && d.CD_EMPRESA == _idEmpresa);
            }else if(_idEmpresa != null)
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == _idEmpresa);
            }else if(_idProducto != null)
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