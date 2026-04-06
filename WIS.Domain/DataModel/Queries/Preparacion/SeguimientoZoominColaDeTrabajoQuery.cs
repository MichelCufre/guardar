using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Filtering;
using WIS.GridComponent.Execution.Configuration;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class SeguimientoZoominColaDeTrabajoQuery : QueryObject<V_ZOOMIN_PRE812, WISDB>
    {
        public int _cdEmpresa;
        public string _nuPedido;
        public string _cdCliente;


        public SeguimientoZoominColaDeTrabajoQuery(int cdEmpresa, string cdCliente, string nuPedido)
        {
            this._cdCliente = cdCliente;
            this._cdEmpresa = cdEmpresa;
            this._nuPedido = nuPedido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ZOOMIN_PRE812.Where(w => w.CD_CLIENTE == this._cdCliente && w.CD_EMPRESA == this._cdEmpresa && w.NU_PEDIDO == this._nuPedido);
        }

        public virtual List<string[]> GetKeysRowsSelected(IUnitOfWork uow, GridMenuItemActionContext selection, IFilterInterpreter filterInterpreter)
        {
            uow.HandleQuery(this);
            this.ApplyFilter(filterInterpreter, selection.Filters);

            if (selection.Selection.AllSelected)
            {
                return this.GetResult().Select(r => string.Join("$", new string[] {
                    r.CD_PRODUTO,
                    r.CD_EMPRESA.ToString(),
                    r.NU_IDENTIFICADOR,
                    r.CD_FAIXA.ToString(),
                    r.NU_PREPARACION.ToString(),
                    r.NU_PEDIDO,
                    r.CD_CLIENTE,
                    r.CD_ENDERECO,
                    r.NU_SEQ_PREPARACION.ToString()}))
                        .Except(selection.Selection.Keys).Select(w => w.Split('$')).ToList();
            }
            else
            {
                return this.GetResult().Select(r => string.Join("$", new string[] {
                    r.CD_PRODUTO,
                    r.CD_EMPRESA.ToString(),
                    r.NU_IDENTIFICADOR,
                    r.CD_FAIXA.ToString(),
                    r.NU_PREPARACION.ToString(),
                    r.NU_PEDIDO,
                    r.CD_CLIENTE,
                    r.CD_ENDERECO,
                    r.NU_SEQ_PREPARACION.ToString()}))
                        .Intersect(selection.Selection.Keys).Select(w => w.Split('$')).ToList();
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
