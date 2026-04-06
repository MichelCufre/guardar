using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Controllers.COF;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.FormModules.Preparacion;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Application.Validation.Modules.GridModules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Logic;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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

namespace WIS.Application.Controllers.PRE
{
    public class PRE810PonderadorFechaEmitido : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly Logger _logger;
        protected readonly IFormValidationService _formValidationService;


        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE810PonderadorFechaEmitido(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService
            , IFormValidationService formValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_INST_PONDERADOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_INST_PONDERADOR",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
            this._formValidationService = formValidationService;

        }



        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_LIST", new List<GridButton>
            {
                new GridButton("btnEditar", "PRE810_grid_btn_Editar", "fas fa-edit")
            }));

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

                ColaDeTrabajoPonderadorQuery query = new ColaDeTrabajoPonderadorQuery(nuColaDeTrabajo, ColasTrabajoDb.CondicionFechaEmitido);

                uow.HandleQuery(query);
                grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

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
            ColaDeTrabajoPonderadorQuery query;
            string strColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");

            if (int.TryParse(strColaDeTrabajo, out int nuColaDeTrabajo))
            {
                query = new ColaDeTrabajoPonderadorQuery(nuColaDeTrabajo, ColasTrabajoDb.CondicionFechaEmitido);

                uow.HandleQuery(query);
                query.ApplyFilter(this._filterInterpreter, context.Filters);
                count.Count = query.GetCount();
            }

            return count;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            ColaDeTrabajoPonderadorQuery query;
            string strColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");
            int.TryParse(strColaDeTrabajo, out int nuColaDeTrabajo);

            query = new ColaDeTrabajoPonderadorQuery(nuColaDeTrabajo, ColasTrabajoDb.CondicionFechaEmitido);

            uow.HandleQuery(query);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, query, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PonderacionGridValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            context.AddParameter("NU_COLA_TRABAJO", context.Row.GetCell("NU_COLA_TRABAJO").Value);
            context.AddParameter("CD_INST_PONDERADOR", context.Row.GetCell("CD_INST_PONDERADOR").Value);
            context.AddParameter("operadorEntrega", context.Row.GetCell("VL_OPERACION").Value);

            return context;
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
                        if (row.IsDeleted)
                            logicCola.DeletePonderadorDetalle(uow, nuColaDeTrabajo, row.GetCell("CD_INST_PONDERADOR").Value, ColasTrabajoDb.CondicionFechaEmitido);
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

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelectOperador(uow, form);

            var fieldDias = form.GetField("dias");
            var fieldHoras = form.GetField("horas");
            var fieldDiasHasta = form.GetField("diasHasta");
            var fieldHorasHasta = form.GetField("horasHasta");
            var fieldOperador = form.GetField("operador");
            var fieldPonderacion = form.GetField("ponderacion");

            if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "NU_COLA_TRABAJO")?.Value, out int nuColaTrabajo))
            {
                var instPonderador = context.Parameters.FirstOrDefault(s => s.Id == "CD_INST_PONDERADOR").Value;

                var detallePonderador = uow.ColaDeTrabajoRepository.GetPonderadorDetalle(nuColaTrabajo, ColasTrabajoDb.CondicionFechaEmitido, instPonderador);

                int dias;
                int horas;
                int horasTotales;
                if (detallePonderador.Operacion == ColasTrabajoDb.OperacionEntre)
                {
                    var ponderadorBruto = detallePonderador.Instancia.Split(' ');
                    horasTotales = int.Parse(ponderadorBruto[0]);
                    var horasTotalesHasta = int.Parse(ponderadorBruto[2]);
                    dias = horasTotales / 24;
                    horas = horasTotales % 24;
                    int diasHasta = horasTotalesHasta / 24;
                    int horasHasta = horasTotalesHasta % 24;
                    fieldDiasHasta.Value = diasHasta.ToString();
                    fieldHorasHasta.Value = horasHasta.ToString();
                    fieldDiasHasta.ReadOnly = true;
                    fieldHorasHasta.ReadOnly = true;
                }
                else
                {
                    horasTotales = int.Parse(detallePonderador.Instancia);
                    dias = horasTotales / 24;
                    horas = horasTotales % 24;
                }

                fieldDias.Value = dias.ToString();
                fieldHoras.Value = horas.ToString();
                fieldOperador.Value = detallePonderador.Operacion;
                fieldPonderacion.Value = detallePonderador.NuPonderacion.ToString();

                fieldDias.ReadOnly = true;
                fieldHoras.ReadOnly = true;
                fieldOperador.ReadOnly = true;
            }
            else
            {
                fieldDias.ReadOnly = false;
                fieldHoras.ReadOnly = false;
                fieldOperador.ReadOnly = false;
                fieldDiasHasta.ReadOnly = false;
                fieldHorasHasta.ReadOnly = false;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRE810PonderadorFechaFormValidationModule(uow), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                string strNuColaDeTrabajo = context.GetParameter("nuColaDeTrabajo");

                if (string.IsNullOrEmpty(strNuColaDeTrabajo))
                    throw new ValidationFailedException("PRE810_Grid_Error_ErrorColaDeTrabajo");

                int.TryParse(strNuColaDeTrabajo, out int nuColaDeTrabajo);

                string srtDias = form.GetField("dias").Value;
                string srtHoras = form.GetField("horas").Value;
                string srtOperador = form.GetField("operador").Value;
                string srtPonderacion = form.GetField("ponderacion").Value;

                string totalHoras;
                int horasTotales = 0;
                int horasTotalesHasta = -1;

                if (!string.IsNullOrEmpty(srtDias))
                {
                    int dias = int.Parse(srtDias);
                    horasTotales = dias * 24;
                }

                int horas = int.Parse(srtHoras);
                horasTotales += horas;

                totalHoras = horasTotales.ToString();

                if (srtOperador == ColasTrabajoDb.OperacionEntre)
                {
                    horasTotalesHasta = 0;
                    string srtDiasHasta = form.GetField("diasHasta").Value;
                    string srtHorasHasta = form.GetField("horasHasta").Value;
                    if (!string.IsNullOrEmpty(srtDiasHasta))
                    {
                        int dias = int.Parse(srtDiasHasta);
                        horasTotalesHasta = dias * 24;
                    }

                    int horasHasta = int.Parse(srtHorasHasta);
                    horasTotalesHasta += horasHasta;

                    totalHoras = horasTotales + ColasTrabajoDb.OperadorAnd + horasTotalesHasta;
                }

                if (horasTotalesHasta >= 0 && horasTotales >= horasTotalesHasta)
                    throw new ValidationFailedException("PRE810_Grid_Error_ErrorHorasHasta");


                LColasDeTrabajo logicCola = new LColasDeTrabajo(uow, _identity, _logger);
                logicCola.EditarPonderadorDetalle(nuColaDeTrabajo, int.Parse(form.GetField("ponderacion").Value), ColasTrabajoDb.CondicionFechaEmitido, srtOperador, totalHoras);

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                context.AddWarningNotification("PRE810_Form_Msg_WarningDosReglas");

                uow.SaveChanges();
                uow.Commit();

            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();

                throw ex;
            }
            return form;

        }

        #region Metodos Auxiliares

        public virtual void InicializarSelectOperador(IUnitOfWork uow, Form form)
        {
            FormField fieldTipoMotivo = form.GetField("operador");
            fieldTipoMotivo.Options = new List<SelectOption>();

            List<string> col = new List<string>()
            {
                ColasTrabajoDb.OperacionIgual,
                ColasTrabajoDb.OperacionMayor,
                ColasTrabajoDb.OperacionMayorIgual ,
                ColasTrabajoDb.OperacionMenor ,
                ColasTrabajoDb.OperacionMenorIgual,
                ColasTrabajoDb.OperacionEntre
            };

            foreach (var item in col)
            {
                fieldTipoMotivo.Options.Add(new SelectOption(item, item));
            }
        }

        #endregion
    }
}