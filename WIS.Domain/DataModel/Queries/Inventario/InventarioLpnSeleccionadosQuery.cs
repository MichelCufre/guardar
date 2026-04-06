using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioLpnSeleccionadosQuery : QueryObject<V_INV416_REG_SEL, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioLpnSeleccionadosQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            var query = context.V_INV416_REG_SEL
                .Where(i => i.NU_INVENTARIO == _filtros.NuInventario
                    && (_filtros.ExcluirSueltos ? i.NU_LPN.HasValue : true)
                    && (_filtros.ExcluirLpns ? !i.NU_LPN.HasValue : true));

            foreach (var idAtributo in _filtros.AtributosCabezal.Keys)
            {
                var valor = _filtros.AtributosCabezal[idAtributo];

                query = query
                    .Join(context.V_INV410_LPN_ATRIBUTO_CAB,
                        d => new { d.NU_LPN },
                        v => new { NU_LPN = (long?)v.NU_LPN },
                        (d, v) => new { Registros = d, Atributo = v })
                    .Where(da => da.Atributo.ID_ATRIBUTO == idAtributo
                        && EF.Functions.Like(da.Atributo.VL_ATRIBUTO, valor))
                    .Select(da => da.Registros);
            }

            this.Query = query;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<InventarioSelectRegistroLpn> GetRegistrosLpn()
        {
            return this.Query.Select(d => new InventarioSelectRegistroLpn
            {
                NuInventario = d.NU_INVENTARIO,
                NuInventarioUbicacion = d.NU_INVENTARIO_ENDERECO,
                NuInventarioUbicacionDetalle = d.NU_INVENTARIO_ENDERECO_DET,
                NroLpn = d.NU_LPN.ToString(),
                NroLpnReal  = d.NU_LPN,
                Empresa  = d.CD_EMPRESA.Value,
                Producto  = d.CD_PRODUTO,
                Faixa  = d.CD_FAIXA.Value,
                Identificador  = d.NU_IDENTIFICADOR,
            }).ToList();
        }
    }
}
