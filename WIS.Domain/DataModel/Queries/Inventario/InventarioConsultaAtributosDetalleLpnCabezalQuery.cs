using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioConsultaAtributosDetalleLpnCabezalQuery : QueryObject<V_INV410_ATRIBUTO, WISDB>
    {
        protected Dictionary<int, string> _atributos;

        public InventarioConsultaAtributosDetalleLpnCabezalQuery()
        {
            _atributos = new Dictionary<int, string>();
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV410_LPN_DET_ATRIBUTO_CAB
                .Where(d => d.ID_ATRIBUTO != -1)
                .GroupBy(v => new V_INV410_ATRIBUTO
                {
                    ID_ATRIBUTO = v.ID_ATRIBUTO,
                    NM_ATRIBUTO = v.NM_ATRIBUTO,
                })
                .Select(g => g.Key);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}