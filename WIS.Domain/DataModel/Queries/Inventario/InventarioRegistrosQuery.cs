using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;
using WIS.Security;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioRegistrosQuery : QueryObject<V_INV413_REG_DISP, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioRegistrosQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV413_REG_DISP
                .Where(x => x.NU_INVENTARIO == _filtros.NuInventario
                    && x.NU_PREDIO == _filtros.Predio
                    && (_filtros.Empresa.HasValue ? x.CD_EMPRESA == _filtros.Empresa.Value : true)
                    && x.QT_ESTOQUE > 0);
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect, IIdentityService _identity)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_ENDERECO, r.CD_PRODUTO, r.NU_IDENTIFICADOR, r.CD_EMPRESA.ToString(), r.CD_FAIXA.ToString(_identity.GetFormatProvider()) }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude, IIdentityService _identity)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_ENDERECO, r.CD_PRODUTO, r.NU_IDENTIFICADOR, r.CD_EMPRESA.ToString(), r.CD_FAIXA.ToString(_identity.GetFormatProvider()) }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
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
