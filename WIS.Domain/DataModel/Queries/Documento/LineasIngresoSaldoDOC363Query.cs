using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class LineasIngresoSaldoDOC363Query : QueryObject<V_DOC363_SALDO_LINEA_INGRESO, WISDB>
    {
        protected readonly List<string> _filtroProducto;
        protected readonly int _empresa;

        public LineasIngresoSaldoDOC363Query(List<string> filtroProducto, int empresa)
        {
            this._filtroProducto = filtroProducto;
            this._empresa = empresa;
        } 

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC363_SALDO_LINEA_INGRESO
                .Where(l => l.CD_EMPRESA == this._empresa && _filtroProducto.Contains(l.VL_FILTRO));
        }
    }
}
