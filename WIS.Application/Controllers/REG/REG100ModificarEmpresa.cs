using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Domain.DataModel.Mappers;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Exceptions;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG100ModificarEmpresa : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG100ModificarEmpresa> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG100ModificarEmpresa(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG100ModificarEmpresa> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_REL_EMPRESA_TIPO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_RECEPCION_REL_EMPRESA_TIPO", SortDirection.Ascending),
                new SortCommand("CD_EMPRESA", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsEditingEnabled = true;

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ReferenciaExternaRecepcionQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value, out int empresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ReferenciaExternaRecepcionQuery(empresa);

            }
            else
            {
                dbQuery = new ReferenciaExternaRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "TP_RECEPCION_EXTERNO", "DS_RECEPCION_EXTERNO", "FL_HABILITADO" });

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        int nuRecepcionRelEmpresaTipo = int.Parse(row.GetCell("NU_RECEPCION_REL_EMPRESA_TIPO").Value);

                        bool habilitado = !string.IsNullOrEmpty(row.GetCell("FL_HABILITADO").Value) && row.GetCell("FL_HABILITADO").Value == "S";
                        string tipoRecepcionExterno = row.GetCell("TP_RECEPCION_EXTERNO").Value;
                        string descTipoRecepcionExterno = row.GetCell("DS_RECEPCION_EXTERNO").Value;

                        var empresa = uow.RecepcionTipoRepository.GetEmpresaRecepcionTipoById(nuRecepcionRelEmpresaTipo);
                        if (empresa == null)
                            throw new ValidationFailedException("REG100_Sec0_Error_EmpresaRecepcionNoExiste");

                        empresa.Habilitado = habilitado;
                        empresa.TipoExterno = tipoRecepcionExterno;
                        empresa.DescripcionExterna = descTipoRecepcionExterno;

                        uow.RecepcionTipoRepository.UpdateEmpresaRecepcionTipo(empresa);
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, ex.Message, ex.StrArguments?.ToList());
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG100UpdateGridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new ReferenciasExternasDeRecepcionValidationModule(uow), grid, row, context);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ReferenciaExternaRecepcionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value, out int empresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ReferenciaExternaRecepcionQuery(empresa);

            }
            else
            {
                dbQuery = new ReferenciaExternaRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ReferenciaExternaRecepcionQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value, out int empresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ReferenciaExternaRecepcionQuery(empresa);

            }
            else
            {
                dbQuery = new ReferenciaExternaRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
