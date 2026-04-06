using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Items;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Domain.General;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Enums;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.Registro;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG080PanelPuertasEmbarque : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG080PanelPuertasEmbarque> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly PuertaEmbarqueMapper _mapper;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG080PanelPuertasEmbarque(
            IIdentityService _identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG080PanelPuertasEmbarque> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this._mapper = new PuertaEmbarqueMapper();

            this.GridKeys = new List<string>
            {
                "CD_PORTA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_PORTA", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._identity = _identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;
            query.IsAddEnabled = false;
            query.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit") //fa fa-file-pdf-o
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PuertaEmbarqueQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string>
            {
                "CD_SITUACAO"
            });


            foreach (var row in grid.Rows)
            {
                if (uow.PuertaEmbarqueRepository.GetPuertaEmbarque(short.Parse(row.GetCell("CD_PORTA").Value)).Estado == SituacionDb.Activo)
                {
                    row.GetCell("CD_SITUACAO").Value = "S";
                }
                else
                {
                    row.GetCell("CD_SITUACAO").Value = "N";
                }

                row.GetCell("CD_SITUACAO").ForceSetOldValue(int.Parse(row.GetCell("CD_SITUACAO").Old ?? "0") == SituacionDb.Activo ? "S" : "N");
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PuertaEmbarqueQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PuertaEmbarqueQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

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
                        var shortEstadoPuerta = row.GetCell("CD_SITUACAO").Value == "S" ? SituacionDb.Activo : SituacionDb.Inactivo;

                        if (shortEstadoPuerta.ToString() != row.GetCell("CD_SITUACAO").Old)
                        {
                            var estadoPorta = row.GetCell("CD_SITUACAO").Value;

                            var puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(short.Parse(row.GetCell("CD_PORTA").Value));

                            if (estadoPorta == "S")
                                puerta.Estado = SituacionDb.Activo;
                            else
                                puerta.Estado = SituacionDb.Inactivo;


                            uow.PuertaEmbarqueRepository.UpdatePuertaEmbarque(puerta);
                            uow.SaveChanges();
                        }
                    }
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG080GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoPuertaEmbarqueValidationModule(uow), grid, row, context);
        }
    }
}
