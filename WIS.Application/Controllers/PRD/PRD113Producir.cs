using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD113Producir : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; set; }
        protected List<SortCommand> DefaultSort { get; }

        public PRD113Producir(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogicaProduccionFactory logicaProduccionFactory,
            IGridValidationService gridValidationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logicaProduccionFactory = logicaProduccionFactory;
            this._concurrencyControl = concurrencyControl;
            this._gridValidationService = gridValidationService;

            this.GridKeys = new List<string> { "NU_PRDC_DET_TEORICO" };
            this.DefaultSort = new List<SortCommand> { new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Ascending) };
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {

            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;


            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("nuIngresoProduccion");
            var loteUtilizar = context.GetParameter("loteUtilizar")?.Trim()?.ToUpper();

            var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var modalidadLote = context.GetParameter("modalidadLote");

            var dbQuery = new DetallesProducirQuery(idIngreso, ingreso.Empresa.Value);

            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "QT_PRODUCIR", "ND_MOTIVO" });

            grid.AddOrUpdateColumn(new GridColumnSelect("ND_MOTIVO", this.SelectMotivos()));

            var motivoParam = uow.ParametroRepository.GetParameter(ParamManager.PRODUCCION_MOT_PROD_DEFAULT);
            var descripcionMotivoParam = uow.DominioRepository.GetDominio(motivoParam).Descripcion;

            foreach (var row in grid.Rows)
            {
                row.GetCell("ND_MOTIVO").Value = motivoParam;
                row.GetCell("DS_MOTIVO").Value = descripcionMotivoParam;

                if (!string.IsNullOrEmpty(modalidadLote))
                {
                    if (row.GetCell("NU_IDENTIFICADOR").Value != ManejoIdentificadorDb.IdentificadorProducto)
                    {
                        row.GetCell("NU_IDENTIFICADOR").Value = loteUtilizar;
                    }
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("nuIngresoProduccion");
            var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var dbQuery = new DetallesProducirQuery(idIngreso, ingreso.Empresa.Value);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns.Where(x => x.Id != "QT_PRODUCIR").ToList(), context, this.DefaultSort);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD113ProducirGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("nuIngresoProduccion");
            var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var dbQuery = new DetallesProducirQuery(idIngreso, ingreso.Empresa.Value);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SelectMotivos()
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var motivos = uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_PRODUCCION);

            List<SelectOption> opciones = new List<SelectOption>();

            foreach (var motivo in motivos)
            {
                opciones.Add(new SelectOption(motivo.Id, motivo.Id + " - " + motivo.Descripcion));
            }

            return opciones;
        }

        public virtual string GetLoteByModalidad(string idModalidad, IngresoProduccion ingreso)
        {
            var lote = string.Empty;

            switch (idModalidad)
            {
                case CTipoIngresoModalidadLote.ID_INTERNO:
                    lote = ingreso.Id;
                    break;

                case CTipoIngresoModalidadLote.ID_EXTERNO:
                    lote = ingreso.IdProduccionExterno;
                    break;

                case CTipoIngresoModalidadLote.FECHA_DIA:
                    lote = DateTime.Now.ToString("d", this._identity.GetFormatProvider());
                    break;

                case CTipoIngresoModalidadLote.MES_PROD:
                    var mesProduccion = ingreso.FechaAlta.Value.Month;
                    lote = mesProduccion.ToString();
                    break;

                default:
                    break;
            }

            return lote;
        }

        #endregion
    }
}
