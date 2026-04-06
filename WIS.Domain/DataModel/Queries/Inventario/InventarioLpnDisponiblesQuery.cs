using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioLpnDisponiblesQuery : QueryObject<V_INV416_REG_DISP, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioLpnDisponiblesQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            var query = context.V_INV416_REG_DISP
            .Where(i => i.NU_INVENTARIO == _filtros.NuInventario
                && (_filtros.Empresa.HasValue ? (i.CD_EMPRESA == _filtros.Empresa.Value) : true)
                && (!string.IsNullOrEmpty(_filtros.Predio) ? (i.NU_PREDIO == _filtros.Predio) : true)
                && (_filtros.ExcluirSueltos ? (i.NU_LPN != "-" && !string.IsNullOrEmpty(i.NU_LPN)) : true)
                && (_filtros.ExcluirLpns ? (i.NU_LPN == "-" || string.IsNullOrEmpty(i.NU_LPN)) : true)
                && i.QT_ESTOQUE > 0);

            foreach (var idAtributo in _filtros.AtributosCabezal.Keys)
            {
                var valor = _filtros.AtributosCabezal[idAtributo];

                query = query
                    .Join(context.V_INV410_LPN_ATRIBUTO_CAB,
                        d => new { d.NU_LPN },
                        v => new { NU_LPN = v.NU_LPN.ToString() },
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
                Ubicacion = d.CD_ENDERECO,
                Empresa = d.CD_EMPRESA,
                Producto = d.CD_PRODUTO,
                Faixa = d.CD_FAIXA,
                Identificador = d.NU_IDENTIFICADOR,
                NroLpn = d.NU_LPN,
                Cantidad = d.QT_ESTOQUE.Value,
                Vencimiento = d.DT_FABRICACAO,
            }).ToList();
        }
    }
}
