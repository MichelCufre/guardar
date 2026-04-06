using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Controllers.COF;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Application.Validation.Modules.GridModules.Preparacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Logic;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE810PonderadorCliente : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly Logger _logger;


        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE810PonderadorCliente(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_CLIENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }



        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            grid.SetEditableCells(new List<string> {
                   "NU_PONDERACION"
                });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            try
            {
                string ponderador = context.GetParameter("cdPonderador");
                string strColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");

                if (!int.TryParse(strColaDeTrabajo, out int nuColaDeTrabajo))
                    throw new ValidationFailedException("PRE810_Grid_Error_ErrorColaDeTrabajo");

                using var uow = this._uowFactory.GetUnitOfWork();

                ColaDeTrabajoPonderadorClienteQuery query = new ColaDeTrabajoPonderadorClienteQuery(nuColaDeTrabajo);

                uow.HandleQuery(query);
                grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);


                List<int> empUser = uow.EmpresaRepository.GetCdEmpresasForUsuario(_identity.UserId);

                foreach (var row in grid.Rows)
                {
                    if (empUser.Contains(int.Parse(row.GetCell("CD_EMPRESA").Value)))
                    {
                        row.SetEditableCells(new List<string>
                        {
                            "NU_PONDERACION",
                        });
                    }

                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GridFetchRows");
                throw ex;
            }
            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var count = new GridStats();
            using var uow = this._uowFactory.GetUnitOfWork();
            ColaDeTrabajoPonderadorClienteQuery query;
            string strColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");

            if (int.TryParse(strColaDeTrabajo, out int nuColaDeTrabajo))
            {
                query = new ColaDeTrabajoPonderadorClienteQuery(nuColaDeTrabajo);

                uow.HandleQuery(query);
                query.ApplyFilter(this._filterInterpreter, context.Filters);
                count.Count = query.GetCount();
            }

            return count;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            ColaDeTrabajoPonderadorClienteQuery query;
            string strColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");
            int.TryParse(strColaDeTrabajo, out int nuColaDeTrabajo);

            query = new ColaDeTrabajoPonderadorClienteQuery(nuColaDeTrabajo);

            uow.HandleQuery(query);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, query, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PonderacionGridValidationModule(uow), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            string strNuColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");

            if (string.IsNullOrEmpty(strNuColaDeTrabajo))
                throw new ValidationFailedException("PRE810_Grid_Error_ErrorColaDeTrabajo");

            int.TryParse(strNuColaDeTrabajo, out int nuColaDeTrabajo);

            try
            {
                if (grid.Rows.Any())
                {
                    LColasDeTrabajo logicCola = new LColasDeTrabajo(uow, _identity, _logger);

                    foreach (var row in grid.Rows)
                    {
                        logicCola.EditarPonderadorDetalle(nuColaDeTrabajo, int.Parse(row.GetCell("NU_PONDERACION").Value), ColasTrabajoDb.Cliente, ColasTrabajoDb.OperacionIgual, $"{row.GetCell("CD_EMPRESA").Value}_{row.GetCell("CD_CLIENTE").Value}");
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }
            return grid;
        }

    }
}