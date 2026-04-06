using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class FacturaDetalleQuery : QueryObject<V_REC500_FACTURAS_DET, WISDB>
    {
        protected readonly int? _facturaDetalle;
        protected readonly int? _idEmpresa;
        protected readonly string? _idProducto;
        protected readonly int? _idFactura;

        public FacturaDetalleQuery()
        {
        }
        public FacturaDetalleQuery(int? facturaDetalle)
        {
            this._facturaDetalle = facturaDetalle;
        }
        public FacturaDetalleQuery(int idFactura, int idEmpresa)
        {
            this._idFactura = idFactura;
            this._idEmpresa = idEmpresa;
        }
        public FacturaDetalleQuery(int idFactura, int idEmpresa, string idProducto)
        {
            this._idFactura = idFactura;
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC500_FACTURAS_DET.AsNoTracking();

            if ((_idFactura != null) && (_idEmpresa != null) && (_idProducto != null))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.NU_RECEPCION_FACTURA == _idFactura && x.CD_PRODUTO == _idProducto);
            }
            else if ((_idFactura != null) && (_idEmpresa != null))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.NU_RECEPCION_FACTURA == _idFactura);
            }
            else if (_idFactura != null)
            {
                this.Query = this.Query.Where(x => x.NU_RECEPCION_FACTURA == this._idFactura);
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
