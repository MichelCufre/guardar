using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PreparacionesPorEmpresaNumPrep : QueryObject<V_PRE162_PICKING_PENDIENTE, WISDB>
    {
        protected readonly int _empresa;
        protected readonly int _preparacion;

        public PreparacionesPorEmpresaNumPrep()
        {

        }

        public PreparacionesPorEmpresaNumPrep(int empresa, int preparacion)
        {
            this._empresa = empresa;
            this._preparacion = preparacion;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE162_PICKING_PENDIENTE.AsNoTracking();

            if ((this._empresa != 0) && (this._preparacion != 0))
            {
                this.Query = this.Query.Where(s => s.CD_EMPRESA == this._empresa && s.NU_PREPARACION == this._preparacion);
            }

        }

        public virtual string Retorno_ds_preparacion(int nu_preparacion)
        {
            return this.Query.FirstOrDefault(x => x.NU_PREPARACION == nu_preparacion)?.DS_PREPARACION;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
