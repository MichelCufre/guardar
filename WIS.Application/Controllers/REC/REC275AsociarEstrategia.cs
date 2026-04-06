using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC275AsociarEstrategia : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC275AsociarEstrategia> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC275AsociarEstrategia(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC275AsociarEstrategia> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREDIO", "TP_ENTIDAD", "CD_ENTIDAD", "CD_EMPRESA", "TP_ALM_OPERATIVA_ASOCIABLE", "TP_RECEPCION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ALM_ASOCIACION", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;
            query.IsCommitEnabled = true;

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                if (context.ButtonId == "btnSubmitConfirmarAsociaciones")
                {
                    context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroEstrategia = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("numeroEstrategia")))
            {
                numeroEstrategia = int.Parse(query.GetParameter("numeroEstrategia"));
            }

            if (numeroEstrategia != -1)
            {
                AsociacionDeEstrategiasQuery dbQuery = new AsociacionDeEstrategiasQuery(numeroEstrategia);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdEstrategia = context.Parameters.FirstOrDefault(s => s.Id == "numeroEstrategia")?.Value;
            form.GetField("codigoEstrategia").Value = cdEstrategia;

            var estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(cdEstrategia);
            if (estrategia != null)
                form.GetField("descripcionEstrategia").Value = estrategia.Descripcion;

            return form;
        }


        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroEstrategia = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("numeroEstrategia")))
            {
                numeroEstrategia = int.Parse(query.GetParameter("numeroEstrategia"));
            }

            AsociacionDeEstrategiasQuery dbQuery = new AsociacionDeEstrategiasQuery();

            if (numeroEstrategia != -1)
            {
                dbQuery = new AsociacionDeEstrategiasQuery(numeroEstrategia);
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    MantenimientoPanelDeEstrategias mantenimientoEstrategias = new MantenimientoPanelDeEstrategias(uow, this._identity.UserId, this._identity.Application);

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            string codigoAsociacion = row.GetCell("NU_ALM_ASOCIACION").Value;
                            var codigoEstrategia = int.Parse(row.GetCell("NU_ALM_ESTRATEGIA").Value);
                            var predio = row.GetCell("NU_PREDIO").Value;
                            var tipoOperativa = row.GetCell("TP_ALM_OPERATIVA_ASOCIABLE").Value;
                            var codigoOperativa = row.GetCell("TP_RECEPCION").Value;
                            var tipoEntidad = row.GetCell("TP_ENTIDAD").Value;
                            var codigoEntidad = row.GetCell("CD_ENTIDAD").Value;
                            var empresa = row.GetCell("CD_EMPRESA").Value;

                            var sugerenciasExistentes = uow.EstrategiaRepository.AnySugerenciaEstrategiaPendiente(codigoEstrategia, predio, tipoOperativa, codigoOperativa, tipoEntidad, codigoEntidad, empresa);
                            if (sugerenciasExistentes)
                                throw new ValidationFailedException("REC275_Sec0_Error_SugerenciasPendientes");

                            AsociacionEstrategia asociacionEstrategia = uow.EstrategiaRepository.GetAsociacionEstrategiaByCod(short.Parse(codigoAsociacion));
                            mantenimientoEstrategias.BorrarAsociacionEstrategia(asociacionEstrategia);
                        }
                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, ex.Message);
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REC275GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            int numeroEstrategia = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("numeroEstrategia")))
            {
                numeroEstrategia = int.Parse(query.GetParameter("numeroEstrategia"));
            }

            if (numeroEstrategia != -1)
            {
                AsociacionDeEstrategiasQuery dbQuery = new AsociacionDeEstrategiasQuery(numeroEstrategia);
                uow.HandleQuery(dbQuery);
                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
            else
            {
                AsociacionDeEstrategiasQuery dbQuery = new AsociacionDeEstrategiasQuery();
                uow.HandleQuery(dbQuery);
                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
        }

    }
}
