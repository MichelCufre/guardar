using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Interfaz;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Sorting;

namespace WIS.Application.Controllers.INT
{
    public class INT050AdministrarBloqueos : AppController
    {
        protected readonly IGridService _gridService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INT050AdministrarBloqueos(IGridService gridService, IUnitOfWorkFactory uowFactory, IFilterInterpreter filterInterpreter, ITaskQueueService taskQueueService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending)
            };

            _gridService = gridService;
            _uowFactory = uowFactory;
            _filterInterpreter = filterInterpreter;
            _taskQueue = taskQueueService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> { new GridButton("btnDesbloquear", "INT050_Sec0_btn_Desbloquear") };
            return this.GridFetchRows(grid, context.FetchContext);

        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            EmpresasBloqueadasQuery dbQuery = new EmpresasBloqueadasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            try
            {
                foreach (var codigoEmpresa in GetSelectedKeyEmpresa(uow, selection))
                {
                    Empresa empresa = uow.EmpresaRepository.GetEmpresa(int.Parse(codigoEmpresa));
                    if (empresa == null)
                        throw new Exception("REG100_Frm1_Error_EmpresaNoExiste");

                    empresa.IsLocked = false;
                    uow.EmpresaRepository.UpdateEmpresa(empresa);
                    uow.SaveChanges();
                }

                uow.Commit();
                _taskQueue.Restart();
                selection.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                selection.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return selection;
        }

        public virtual List<string> GetSelectedKeyEmpresa(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new EmpresasBloqueadasQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EmpresasBloqueadasQuery dbQuery = new EmpresasBloqueadasQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
