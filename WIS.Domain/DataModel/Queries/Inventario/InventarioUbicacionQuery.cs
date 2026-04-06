using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioUbicacionQuery : QueryObject<V_INV411_UBIC_DISP, WISDB>
    {
        protected InventarioFiltros _filtros;

        public InventarioUbicacionQuery(InventarioFiltros filtros)
        {
            _filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            _filtros.Empresa = null; //Se fuerza a que no filtre por empresa

            var query = context.V_INV411_UBIC_DISP
                .Where(x => x.NU_INVENTARIO == _filtros.NuInventario
                    && x.NU_PREDIO == _filtros.Predio
                    && (_filtros.Empresa.HasValue ? x.CD_EMPRESA == _filtros.Empresa.Value : true));

            if (!_filtros.PermiteUbicacionesDeOtrosInventarios)
            {
                query = query.Where(x =>
                    !context.T_INVENTARIO_ENDERECO
                        .Join(context.T_INVENTARIO,
                              ie => ie.NU_INVENTARIO,
                              i => i.NU_INVENTARIO,
                              (ie, i) => new { ie, i })
                        .Any(j =>
                            j.ie.CD_ENDERECO == x.CD_ENDERECO &&
                            j.ie.NU_INVENTARIO != _filtros.NuInventario &&
                            j.i.ND_ESTADO_INVENTARIO == EstadoInventario.EnProceso
                        )
                );
            }

            this.Query = query;
        }

        public virtual List<string> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => r.CD_ENDERECO)
                .Intersect(keysToSelect)
                .Select(w => w)
                .ToList();
        }

        public virtual List<string> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => r.CD_ENDERECO)
                .Except(keysToExclude)
                .Select(w => w)
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
