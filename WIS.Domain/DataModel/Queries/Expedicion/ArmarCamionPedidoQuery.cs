using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Expedicion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ArmarCamionPedidoQuery : QueryObject<V_EXP013_PEDIDO_CAMION, WISDB>
    {
        protected readonly int _camion;
        protected readonly int? _empresa;
        protected readonly short? _ruta;

        public ArmarCamionPedidoQuery(int camion, int? empresa = null, short? ruta = null)
        {
            this._camion = camion;
            this._empresa = empresa;
            this._ruta = ruta;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP013_PEDIDO_CAMION.Where(d => (d.CD_CAMION == this._camion || d.CD_CAMION == null) && d.ID_CARGAR == null);

            if (this._empresa != null)
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._empresa);
            else
                this.Query = this.Query.Join(context.V_EMPRESA_DOCUMENTAL.Where(e => e.FL_DOCUMENTAL == "N"),
                    c => c.CD_EMPRESA,
                    e => e.CD_EMPRESA,
                    (c, e) => c);

            if (this._ruta != null)
                this.Query = this.Query.Where(d => d.CD_ROTA == this._ruta);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PedidoAsociarUnidad> GetPedidosUnidad()
        {
            return this.Query.Select(d => new PedidoAsociarUnidad
            {
                Cliente = d.CD_CLIENTE,
                Empresa = d.CD_EMPRESA,
                Pedido = d.NU_PEDIDO
            }).ToList();
        }
    }
}
