using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Preparacion;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE812InstanciaModal : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFormatProvider _formatProvider;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }
        protected readonly Logger _logger;

        public PRE812InstanciaModal(
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity,
            IFormValidationService formValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PONDEREDOR"
            };

            this.DefaultSort = new List<SortCommand>() {
                new SortCommand("CD_PONDEREDOR",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;
            this._formValidationService = formValidationService;

            this._formatProvider = CultureInfo.InvariantCulture;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string strEmpresa = context.GetParameter("cdEmpresa");
            string strCliente = context.GetParameter("cdCliente");
            string strPedido = context.GetParameter("nuPedido");

            if (string.IsNullOrEmpty(strPedido) || string.IsNullOrEmpty(strCliente) || !int.TryParse(strEmpresa, out int cdEmpresa))
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }

            var dbQuery = new SeguimientoPonderacionColaDeTrabajoQuery(cdEmpresa, strCliente, strPedido);

            uow.HandleQuery(dbQuery);

            List<SortCommand> defaultSort = new List<SortCommand>() {
                new SortCommand("CD_PONDEREDOR",SortDirection.Descending)
            };

            List<string> listKey = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PONDEREDOR"
            };

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, listKey);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string strEmpresa = context.GetParameter("cdEmpresa");
            string strCliente = context.GetParameter("cdCliente");
            string strPedido = context.GetParameter("nuPedido");
            int cdEmpresa = -1;

            if (!string.IsNullOrEmpty(strPedido) && !string.IsNullOrEmpty(strCliente) && int.TryParse(strEmpresa, out cdEmpresa) && cdEmpresa >= 0)
            {
                var dbQuery = new SeguimientoPonderacionColaDeTrabajoQuery(cdEmpresa, strCliente, strPedido);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                dbQuery.ApplyFilter(this._filterInterpreter, context.ExplicitFilter);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;

        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string strEmpresa = context.GetParameter("cdEmpresa");
            string strCliente = context.GetParameter("cdCliente");
            string strPedido = context.GetParameter("nuPedido");

            if (string.IsNullOrEmpty(strPedido) || string.IsNullOrEmpty(strCliente) || !int.TryParse(strEmpresa, out int cdEmpresa))
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }

            var dbQuery = new SeguimientoPonderacionColaDeTrabajoQuery(cdEmpresa, strCliente, strPedido);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "PRE812Ins_form_1")
            {
                string strCdEmpresa = context.GetParameter("cdEmpresa");
                var cdCliente = context.GetParameter("cdCliente");
                var nuPedido = context.GetParameter("nuPedido");

                if (int.TryParse(strCdEmpresa, out int cdEmpresa))
                {
                    decimal? nuPonderacionPed = uow.PedidoRepository.GetNuPonderacionPedido(cdEmpresa, cdCliente, nuPedido);

                    if (nuPonderacionPed != null)
                        form.GetField("qtPonderacion").Value = nuPonderacionPed.ToString();
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();

            try
            {
                if (form.Id == "PRE812Ins_form_1")
                {
                    uow.CreateTransactionNumber("PRE812 Modificación cantidad ponderación");

                    string strPonderacion = form.GetField("qtPonderacion").Value;

                    if (int.TryParse(strPonderacion, out int qtPonderacion))
                    {
                        int cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));
                        var cdCliente = context.GetParameter("cdCliente");
                        var nuPedido = context.GetParameter("nuPedido");

                        uow.PedidoRepository.UpdateNuPonderacionPedido(cdEmpresa, cdCliente, nuPedido, qtPonderacion);

                        uow.SaveChanges();
                        uow.Commit();
                        context.AddSuccessNotification("PRE812_Sec0_msg_CantPonderacionModConExito");
                    }
                    else
                        context.AddErrorNotification("PRE812_Sec0_Error_ValorEntero");
                }

            }
            catch (ExpectedException ex)
            {
                _logger.Error(ex, "FormSubmit");
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "FormSubmit");
                context.AddErrorNotification("PRE812_Sec0_Msg_ErrorAlAsociar");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new UpdatePonderacionPedidoValidationModule(uow, this._identity.UserId, this._identity.Predio, this._formatProvider), form, context);
        }
    }
}
