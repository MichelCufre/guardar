using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class ConsultaDePreparaciones : QueryObject<V_PRE130_DET_PICKING, WISDB>
    {
        protected readonly DetallePreparacion _detalle;
        protected readonly int? _prep;
        protected readonly int? _emp;

        public ConsultaDePreparaciones()
        {
        }

        public ConsultaDePreparaciones(DetallePreparacion detalle)
        {
            this._detalle = detalle;
        }

        public ConsultaDePreparaciones(int? prep, int? emp)
        {
            _prep = prep;
            _emp = emp;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE130_DET_PICKING.AsNoTracking();

            if (this._detalle != null)
            {
                this.Query = this.Query.Where(x => x.NU_PREPARACION == this._detalle.NumeroPreparacion && x.CD_CLIENTE == this._detalle.Cliente
                                                && x.NU_PEDIDO == this._detalle.Pedido && x.CD_EMPRESA == this._detalle.Empresa && x.NU_CARGA == this._detalle.Carga
                                                && x.CD_PRODUTO == this._detalle.Producto && x.NU_IDENTIFICADOR == this._detalle.Lote && x.CD_FAIXA == this._detalle.Faixa);
            }
            else if (_prep != null && _emp != null)
            {
                this.Query = this.Query.Where(x => x.NU_PREPARACION == _prep && x.CD_EMPRESA == _emp);
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
