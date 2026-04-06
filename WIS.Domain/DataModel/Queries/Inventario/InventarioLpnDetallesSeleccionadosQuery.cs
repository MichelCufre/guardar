using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioLpnDetallesSeleccionadosQuery : QueryObject<V_INV417_REG_SEL, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioLpnDetallesSeleccionadosQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            var query = context.V_INV417_REG_SEL
                .Where(i => i.NU_INVENTARIO == _filtros.NuInventario
                    && (_filtros.ExcluirSueltos ? i.NU_LPN.HasValue : true)
                    && (_filtros.ExcluirLpns ? !i.NU_LPN.HasValue : true));

            foreach (var idAtributo in _filtros.AtributosCabezal.Keys)
            {
                var valor = _filtros.AtributosCabezal[idAtributo];
                query = query
                    .Join(context.V_INV410_LPN_DET_ATRIBUTO_CAB,
                        d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR },
                        v => new
                        {
                            ID_LPN_DET = (int?)v.ID_LPN_DET,
                            NU_LPN = (long?)v.NU_LPN,
                            v.CD_PRODUTO,
                            CD_FAIXA = (decimal?)v.ID_LPN_DET,
                            CD_EMPRESA = (int?)v.ID_LPN_DET,
                            v.NU_IDENTIFICADOR
                        },
                        (d, v) => new { Registros = d, Atributo = v })
                    .Where(da => da.Atributo.ID_ATRIBUTO == idAtributo
                        && EF.Functions.Like(da.Atributo.VL_ATRIBUTO, valor))
                    .Select(da => da.Registros);
            }

            foreach (var idAtributo in _filtros.AtributosDetalle.Keys)
            {
                var valor = _filtros.AtributosDetalle[idAtributo];
                query = query
                    .Join(context.V_INV410_LPN_DET_ATRIBUTO_DET,
                        d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR },
                        v => new
                        {
                            ID_LPN_DET = (int?)v.ID_LPN_DET,
                            NU_LPN = (long?)v.NU_LPN,
                            v.CD_PRODUTO,
                            CD_FAIXA = (decimal?)v.ID_LPN_DET,
                            CD_EMPRESA = (int?)v.ID_LPN_DET,
                            v.NU_IDENTIFICADOR
                        },
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
                NroLpnReal = d.NU_LPN,
                IdDetalleLpn = d.ID_LPN_DET.ToString(),
                IdDetalleLpnReal = d.ID_LPN_DET,
            }).ToList();
        }
    }
}
