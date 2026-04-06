using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE450PreparacionesNoAnularJobNocturno : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IIdentityService _identity;

        public PRE450PreparacionesNoAnularJobNocturno(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IFilterInterpreter filterInterpreter)
        {
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            List<IGridItem> items = new List<IGridItem>();
            query.IsRemoveEnabled = false;
            query.IsAddEnabled = false;
            if (grid.Id == "PRE450_grid_1")
            {
                grid.MenuItems = new List<IGridItem> {
                        new GridButton("btnAgregar", "General_Sec0_btn_Agregar"),
                    };
                query.IsEditingEnabled = false;
                query.IsCommitEnabled = false;
                query.IsRollbackEnabled = false;
            }
            else
            {
                grid.MenuItems = new List<IGridItem> {
                        new GridButton("btnQuitar", "General_Sec0_btn_Quitar"),
                    };
                query.IsEditingEnabled = true;
                query.IsCommitEnabled = true;
                query.IsRollbackEnabled = true;
            }
            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {

            if (grid.Id == "PRE450_grid_1")
            {
                SortCommand defaultSort = new SortCommand("NU_PEDIDO", SortDirection.Descending);

                using var uow = this._uowFactory.GetUnitOfWork();

                PreparacionesNoAnularQuery dbQuery = new PreparacionesNoAnularQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, new List<string> { "CD_CLIENTE", "CD_EMPRESA", "NU_PEDIDO", "NU_PREPARACION" });
            }
            else
            {
                SortCommand defaultSort = new SortCommand("NU_PEDIDO", SortDirection.Descending);

                using var uow = this._uowFactory.GetUnitOfWork();

                PreparacionesNoAnularSelectedQuery dbQuery = new PreparacionesNoAnularSelectedQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, new List<string> { "CD_CLIENTE", "CD_EMPRESA", "NU_PEDIDO", "NU_PREPARACION" });
                grid.SetEditableCells(new List<string> { "DT_FIN" });
            }
            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "PRE450_grid_1")
            {
                PreparacionesNoAnularQuery dbQuery = new PreparacionesNoAnularQuery();

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                PreparacionesNoAnularSelectedQuery dbQuery = new PreparacionesNoAnularSelectedQuery();

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.GridId == "PRE450_grid_1")
            {
                DateTime? dtFin = null;

                if (DateTime.TryParse(context.Parameters.Find(s => s.Id == "dtFin").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime auxDtFin))
                    dtFin = auxDtFin;

                var lstKeys = this.GetSelectedPreparacionesAnular(uow, context);

                foreach (var key in lstKeys)
                {
                    string cdCliente = key[0];
                    int cdEmpresa = int.Parse(key[1]);
                    string nuPedido = key[2];
                    int nuPreparacion = int.Parse(key[3]);

                    uow.PreparacionRepository.AddPrepNoAnular(new PrepNoAnular()
                    {
                        dtFin = dtFin ?? DateTime.Today,
                        cdCliente = cdCliente,
                        cdEmpresa = cdEmpresa,
                        nuPedido = nuPedido,
                        nuPreparacion = nuPreparacion
                    });
                }
            }
            else
            {
                var lstKeys = this.GetSelectedPreparacionesAnularQuitar(uow, context);

                foreach (var key in lstKeys)
                {
                    string cdCliente = key[0];
                    int cdEmpresa = int.Parse(key[1]);
                    string nuPedido = key[2];
                    int nuPreparacion = int.Parse(key[3]);
                    uow.PreparacionRepository.RemovePrepNoAnular(cdCliente, cdEmpresa, nuPedido, nuPreparacion);
                }
            }
            uow.SaveChanges();

            return context;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            grid.Rows.ForEach(row =>
            {
                string cdCliente = row.GetCell("CD_CLIENTE").Value;
                int cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string nuPedido = row.GetCell("NU_PEDIDO").Value;
                int nuPreparacion = int.Parse(row.GetCell("NU_PREPARACION").Value);
                DateTime? dtFin = null;
                if (DateTime.TryParse(row.GetCell("DT_FIN").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime auxDtFin))
                    dtFin = auxDtFin;
                uow.PreparacionRepository.UpdatePrepNoAnular(new PrepNoAnular()
                {
                    dtFin = dtFin ?? DateTime.Today,
                    cdCliente = cdCliente,
                    cdEmpresa = cdEmpresa,
                    nuPedido = nuPedido,
                    nuPreparacion = nuPreparacion
                });
            });
            uow.SaveChanges();

            return grid;
        }

        public virtual List<string[]> GetSelectedPreparacionesAnular(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new PreparacionesNoAnularQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
        public virtual List<string[]> GetSelectedPreparacionesAnularQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new PreparacionesNoAnularSelectedQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            form.GetField("dtFin").Value = DateTime.Today.ToString("o");
            return form;
        }
    }
}
