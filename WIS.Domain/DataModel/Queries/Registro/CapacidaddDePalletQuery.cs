using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class CapacidaddDePalletQuery : QueryObject<V_PRODUCTO_PALLET_WREG605, WISDB>
    {

        protected string _idProducto;
        protected int? _idEmpresa;
        public CapacidaddDePalletQuery()
        {
        }

        public CapacidaddDePalletQuery(int idEmpresa, string idProducto)
        {
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRODUCTO_PALLET_WREG605.AsNoTracking().Select(d => d);

            if ((_idEmpresa != null) && (!string.IsNullOrEmpty(_idProducto)))
            {
                this.Query = this.Query.Where(d => d.CD_PRODUTO == _idProducto && d.CD_EMPRESA == _idEmpresa);

            }
            else
            {
                this.Query = context.V_PRODUCTO_PALLET_WREG605;

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



