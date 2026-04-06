using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Configuracion;
using WIS.Domain.General;
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

namespace WIS.Application.Controllers.COF
{
    public class CON010ParametrosConfiguracion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<CON010ParametrosConfiguracion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public CON010ParametrosConfiguracion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<CON010ParametrosConfiguracion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PARAMETRO_CONFIGURACION", "CD_PARAMETRO", "DO_ENTIDAD_PARAMETRIZABLE", "ND_ENTIDAD"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_PARAMETRO", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "ND_ENTIDAD",
                "VL_PARAMETRO"
            });

            //Valores por Parametro
            var cdParametro = query.Parameters.FirstOrDefault(s => s.Id == "cdParametro").Value;
            var doEntidad = query.Parameters.FirstOrDefault(s => s.Id == "doEntidad").Value;
            var dsDominio = query.Parameters.FirstOrDefault(s => s.Id == "dsDominio").Value;

            //Cargo valores para columnas
            var columnCdParametro = grid.Columns.FirstOrDefault(w => w.Id == $"CD_PARAMETRO");
            var columnEntidadParametrizable = grid.Columns.FirstOrDefault(w => w.Id == $"DO_ENTIDAD_PARAMETRIZABLE");
            var columnDsDominio = grid.Columns.FirstOrDefault(w => w.Id == $"DS_DOMINIO");
            columnCdParametro.DefaultValue = cdParametro;
            columnEntidadParametrizable.DefaultValue = doEntidad;
            columnDsDominio.DefaultValue = dsDominio;

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdParametro = query.Parameters.FirstOrDefault(s => s.Id == "cdParametro").Value;
            var doEntidad = query.Parameters.FirstOrDefault(s => s.Id == "doEntidad").Value;

            var dbQuery = new ParametrosConfiguracionQuery(cdParametro, doEntidad);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "VL_PARAMETRO"
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdParametro = query.Parameters.FirstOrDefault(s => s.Id == "cdParametro").Value;
            var doEntidad = query.Parameters.FirstOrDefault(s => s.Id == "doEntidad").Value;

            var dbQuery = new ParametrosConfiguracionQuery(cdParametro, doEntidad);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdParametro = query.Parameters.FirstOrDefault(s => s.Id == "cdParametro").Value;
            var doEntidad = query.Parameters.FirstOrDefault(s => s.Id == "doEntidad").Value;

            var dbQuery = new ParametrosConfiguracionQuery(cdParametro, doEntidad);
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var cdParametro = row.GetCell("CD_PARAMETRO").Value;
                        var entidad = row.GetCell("ND_ENTIDAD").Value;
                        var entidadParametrizable = row.GetCell("DO_ENTIDAD_PARAMETRIZABLE").Value;
                        var valorParametro = row.GetCell("VL_PARAMETRO").Value;

                        if (row.IsNew)
                        {
                            if (entidadParametrizable == ParamManager.PARAM_GRAL && uow.ParametroRepository.ExisteParametroConfiguracion(cdParametro))
                                throw new ValidationFailedException("CON010ParametrosConfig_Sec0_error_ParametroIngresado");

                            var parametroConfiguracion = CrearParametroConfiguracion(cdParametro, entidad, entidadParametrizable, valorParametro);
                            uow.ParametroRepository.AddParametroConfiguracion(parametroConfiguracion);
                        }
                        else if (row.IsDeleted)
                        {
                            var parametroConfiguracion = uow.ParametroRepository.GetParametroConfiguracion(cdParametro, entidad);

                            if (parametroConfiguracion == null)
                                throw new ValidationFailedException("CON010ParametrosConfig_Sec0_error_ParametroNoEncontrado");

                            if (entidadParametrizable == ParamManager.PARAM_GRAL)
                                throw new ValidationFailedException("CON010ParametrosConfig_Sec0_error_ParametroGeneralNoEliminable");

                            // rows delete
                            DeleteParametroConfiguracion(uow, cdParametro, entidad);
                        }
                        else
                        {
                            var parametroConfiguracion = UpdateParametroConfiguracion(uow, cdParametro, entidad, valorParametro);
                            uow.ParametroRepository.UpdateParametroConfiguracion(parametroConfiguracion);
                        }
                    }

                }

                uow.SaveChanges();
                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "GridCommit");
                query.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF020GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoValorParametrosSistemaGridValidationModule(uow), grid, row, context); ;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (context.ColumnId)
            {
                case "ND_ENTIDAD":
                    var cdParametro = context.Parameters.FirstOrDefault(s => s.Id == "cdParametro").Value;
                    var doEntidad = context.Parameters.FirstOrDefault(s => s.Id == "doEntidad").Value;

                    return this.SearchValorEntidad(cdParametro, doEntidad, context.SearchValue);
            }

            return new List<SelectOption>();
        }

        public virtual ParametroConfiguracion CrearParametroConfiguracion(string cdParametro, string entidad, string entidadParametrizable, string valorParametro)
        {
            return new ParametroConfiguracion()
            {
                CodigoParametro = cdParametro,
                TipoParametro = entidadParametrizable,
                Clave = entidad,
                Valor = valorParametro,
            };
        }

        public virtual ParametroConfiguracion UpdateParametroConfiguracion(IUnitOfWork uow, string cdParametro, string entidad, string valorParametro)
        {
            var parametroConfiguracion = uow.ParametroRepository.GetParametroConfiguracion(cdParametro, entidad);

            parametroConfiguracion.Valor = valorParametro;

            return parametroConfiguracion;
        }

        public virtual void DeleteParametroConfiguracion(IUnitOfWork uow, string cdParametro, string ndEntidad)
        {
            if (uow.ParametroRepository.ExisteParametroConfiguracion(cdParametro, ndEntidad))
                uow.ParametroRepository.DeleteParametroConfiguracion(cdParametro, ndEntidad);
            else
                throw new EntityNotFoundException("CON010ParametrosConfig_Sec0_Error_Er001_ParametroConfiguracionNoExisteEliminar");
        }

        public virtual List<SelectOption> SearchValorEntidad(string cdParametro, string doEntidad, string valor)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var parametrosConfiguracion = uow.ParametroRepository.GetParams(cdParametro);

            valor = valor?.Trim()?.ToLower();

            return uow.DominioRepository.GetDominios(doEntidad)
                .Where(w => !parametrosConfiguracion.Any(a => a.Clave == w.Id)
                    && (string.IsNullOrEmpty(valor) || w.Valor.Trim().ToLower().Contains(valor) || w.Descripcion.Trim().ToLower().Contains(valor)))
                .Select(w => new SelectOption(w.Id, w.Id + " - " + w.Valor + " - " + w.Descripcion))
                .ToList();
        }
    }
}
