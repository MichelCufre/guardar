using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Filtering;
using WIS.GridComponent.Execution.Configuration;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
	public class PedidosTempQuery : QueryObject<T_CROSS_DOCK_TEMP, WISDB>
    {
        protected int? empresa;
        protected int? agenda;

        public PedidosTempQuery(int? empresa, int? agenda)
        {
            this.empresa = empresa;
            this.agenda = agenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_CROSS_DOCK_TEMP.AsNoTracking();

            this.Query = this.Query.Where(w => w.CD_EMPRESA == empresa && w.NU_AGENDA == agenda);
        }
        public virtual List<string[]> GetKeysRowsSelected(IUnitOfWork uow, GridMenuItemActionContext selection, IFilterInterpreter filterInterpreter)
        {

            uow.HandleQuery(this);
            this.ApplyFilter(filterInterpreter, selection.Filters);
            if (selection.Selection.AllSelected)
            {
                return this.GetResult()
                    .Select(r => string.Join("$", new string[] { r.NU_AGENDA.ToString(), r.CD_CLIENTE, r.NU_PEDIDO, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR }))
                    .Except(selection.Selection.Keys)
                    .Select(w => w.Split('$'))
                    .ToList();
            }
            else
            {
                return this.GetResult()
                    .Select(r => string.Join("$", new string[] { r.NU_AGENDA.ToString(), r.CD_CLIENTE, r.NU_PEDIDO, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR }))
                    .Intersect(selection.Selection.Keys)
                    .Select(w => w.Split('$'))
                    .ToList();
            }
        }
    }
}
