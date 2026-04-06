using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{

    public class REG104PanelZonas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG104PanelZonas(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_ZONA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DS_ZONA", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsCommitEnabled = true;
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            if (this._security.IsUserAllowed(SecurityResources.REG104Update_Page_Access_Allow))
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
                {
                    new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                }));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ZonaQuery dbQuery;
            dbQuery = new ZonaQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ZonaQuery dbQuery;
            dbQuery = new ZonaQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ZonaQuery dbQuery = new ZonaQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            throw new OperationNotAllowedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                        else if (row.IsDeleted)
                        {
                            this.DeleteZona(uow, row, context);
                        }
                        else
                        {
                            throw new OperationNotAllowedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                    }

                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public virtual void DeleteZona(IUnitOfWork uow, GridRow row, GridFetchContext context)
        {
            var cdZona = row.GetCell("CD_ZONA").Value;

            if (uow.ZonaRepository.AnyZona(cdZona))
            {
                if (!uow.ZonaRepository.AnyPedidoAsociado(cdZona))
                    uow.ZonaRepository.DeleteZona(cdZona);
                else
                    throw new ValidationFailedException("REG104_Sec0_Error_ZonaConPedidosAsociados");
            }
            else
            {
                throw new ValidationFailedException("REG104_Sec0_Error_ZonaNoExisteEliminar");
            }
        }
    }
}
