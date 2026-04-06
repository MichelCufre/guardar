using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioUbicacionQuitarQuery : QueryObject<V_INV411_UBIC_SEL, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioUbicacionQuitarQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV411_UBIC_SEL.Where(w => w.NU_INVENTARIO == _filtros.NuInventario).AsQueryable();
        }

        public virtual List<decimal> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => r.NU_INVENTARIO_ENDERECO.ToString(formatProvider))
                .Intersect(keysToSelect)
                .Select(w => decimal.Parse(w, formatProvider))
                .ToList();
        }

        public virtual List<decimal> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => r.NU_INVENTARIO_ENDERECO.ToString(formatProvider))
                .Except(keysToExclude)
                .Select(w => decimal.Parse(w, formatProvider))
                .ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}