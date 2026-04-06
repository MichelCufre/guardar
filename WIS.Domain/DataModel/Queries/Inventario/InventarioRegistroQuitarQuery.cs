using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioRegistroQuitarQuery : QueryObject<V_INV413_REG_SEL, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioRegistroQuitarQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV413_REG_SEL.Where(w => w.NU_INVENTARIO == _filtros.NuInventario);
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_INVENTARIO.ToString(), r.NU_INVENTARIO_ENDERECO.ToString(), r.NU_INVENTARIO_ENDERECO_DET.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_INVENTARIO.ToString(), r.NU_INVENTARIO_ENDERECO.ToString(), r.NU_INVENTARIO_ENDERECO_DET.ToString() }))
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

        public virtual List<InventarioSelectRegistroLpn> GetRegistros()
        {
            return this.Query.Select(d => new InventarioSelectRegistroLpn
            {
                NuInventario = d.NU_INVENTARIO,
                NuInventarioUbicacion = d.NU_INVENTARIO_ENDERECO,
                NuInventarioUbicacionDetalle = d.NU_INVENTARIO_ENDERECO_DET,
                Empresa = d.CD_EMPRESA.Value,
                Producto = d.CD_PRODUTO,
                Faixa = d.CD_FAIXA.Value,
                Identificador = d.NU_IDENTIFICADOR,
            }).ToList();
        }
    }
}
