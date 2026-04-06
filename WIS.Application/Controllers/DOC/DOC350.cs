using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC350 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DOC350(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
        }

        public override PageContext PageLoad(PageContext context)
        {
            try
            {
                var nuAgrupador = context.GetParameter("nuAgrupador");
                var tpAgrupador = context.GetParameter("tpAgrupador");

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    IDocumentoAgrupador agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nuAgrupador, tpAgrupador);
                    var empresa = uow.EmpresaRepository.GetEmpresa(agrupador.Empresa ?? -1);

                    if (agrupador != null)
                    {
                        context.AddParameter("NU_AGRUPADOR", agrupador.Numero);
                        context.AddParameter("CD_EMPRESA", Convert.ToString(agrupador.Empresa));
                        context.AddParameter("NM_EMPRESA", empresa?.Nombre);
                        context.AddParameter("CNPJ_EMPRESA", empresa?.NumeroFiscal);
                        context.AddParameter("DS_ENDERECO_EMPRESA", empresa?.Direccion);
                        context.AddParameter("TP_AGRUPADOR", agrupador.Tipo.TipoAgrupador);
                        context.AddParameter("TP", agrupador.Tipo.TipoAgrupador);
                        context.AddParameter("DT_ADDROW", agrupador.FechaAlta.ToString("dd/MM/yyyy HH:mm"));
                        context.AddParameter("DT_SAIDA", agrupador.FechaSalida.ToString("dd/MM/yyyy HH:mm"));
                        context.AddParameter("DT_LLEGADA", agrupador.FechaLlegada.ToString("dd/MM/yyyy HH:mm"));
                        context.AddParameter("DS_TRANSPORTADORA", agrupador.Transportadora.Descripcion);
                        context.AddParameter("CD_CGC_TRANSP", agrupador.Transportadora.NumeroFiscal?.ToString());
                        context.AddParameter("DS_PLACA", agrupador.Placa);
                        context.AddParameter("TP_VEHICULO", agrupador.TipoVehiculo.Tipo);
                        context.AddParameter("NU_LACRE", agrupador.NumeroLacre);
                        context.AddParameter("DS_MOTORISTA", agrupador.Motorista);
                        context.AddParameter("CPF", agrupador.Anexo1);
                        context.AddParameter("CNH", agrupador.Anexo2);
                        context.AddParameter("ANEXO", agrupador.Anexo3);
                        context.AddParameter("ANEXO2", agrupador.Anexo4);
                        context.AddParameter("ID_ESTADO", agrupador.Estado);
                        context.AddParameter("DS_MOTIVO", agrupador.Motivo);

                        var dbQuery = new DetallesAgrupadorQuery(nuAgrupador, tpAgrupador);
                        uow.HandleQuery(dbQuery);
                        var documentosQuery = dbQuery.GetResult();

                        context.AddParameter("QTD_DOCUMENTOS", documentosQuery.Count().ToString());
                        context.AddParameter("VL_TOTAL_TRANSPORTADO", documentosQuery.Sum(s => s.VL_DOCUMENTO).ToString());
                        context.AddParameter("VL_TOTAL_TRANSPORTADO_CIF", documentosQuery.Sum(s => s.VL_DOCUMENTO_CIF).ToString());
                    }
                    else
                    {
                        #region parametros vacios
                        context.AddParameter("NU_AGRUPADOR", "");
                        context.AddParameter("CD_EMPRESA", "");
                        context.AddParameter("NM_EMPRESA", "");
                        context.AddParameter("CNPJ_EMPRESA", "");
                        context.AddParameter("DS_ENDERECO_EMPRESA", "");
                        context.AddParameter("TP_AGRUPADOR", "");
                        context.AddParameter("DT_ADDROW", "");
                        context.AddParameter("DT_SAIDA", "");
                        context.AddParameter("DT_LLEGADA", "-");
                        context.AddParameter("DS_TRANSPORTADORA", "");
                        context.AddParameter("CD_CGC_TRANSP", "");
                        context.AddParameter("DS_PLACA", "");
                        context.AddParameter("TP_VEHICULO", "");
                        context.AddParameter("NU_LACRE", "");
                        context.AddParameter("DS_MOTORISTA", "");
                        context.AddParameter("CPF", "");
                        context.AddParameter("CNH", "");
                        context.AddParameter("ID_ESTADO", "");
                        context.AddParameter("DS_MOTIVO", "");
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification("General_Sec0_Error_Error45");
            }

            return context;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var nuAgrupador = context.GetParameter("nuAgrupador");
                    var tpAgrupador = context.GetParameter("tpAgrupador");

                    if (!string.IsNullOrEmpty(nuAgrupador) && !string.IsNullOrEmpty(tpAgrupador))
                    {
                        switch (grid.Id)
                        {
                            case "DOC350_grid_1":
                                return GridDetalleFetchRow(grid, context, uow, nuAgrupador, tpAgrupador);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw new ValidationFailedException("DOC350_Sec0_Error_Error01", new string[] { grid.Id });
            }
            return grid;
        }

        public virtual Grid GridDetalleFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, string nuAgrupador, string tpAgrupador)
        {
            var queryDetalles = new DetallesAgrupadorQuery(nuAgrupador, tpAgrupador);

            uow.HandleQuery(queryDetalles);

            var defaultSortIngreso = new SortCommand("DT_ADDROW", SortDirection.Descending);

            List<string> GridKeysIngreso = new List<string>
            {
                "NU_AGRUPADOR", "TP_AGRUPADOR", "NU_DOCUMENTO", "TP_DOCUMENTO"
            };

            if (queryDetalles.Any())
            {
                grid.Rows = this._gridService.GetRows(queryDetalles, grid.Columns, context, defaultSortIngreso, GridKeysIngreso);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DetallesAgrupadorQuery(nuAgrupador, tpAgrupador);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = "LineaDocIngreso_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(nuAgrupador) && !string.IsNullOrEmpty(tpAgrupador))
            {
                switch (grid.Id)
                {
                    case "DOC350_grid_1":
                        return GridDetalleFetchStats(grid, context, uow, nuAgrupador, tpAgrupador);
                }
            }

            return null;
        }

        public virtual GridStats GridDetalleFetchStats(Grid grid, GridFetchStatsContext context, IUnitOfWork uow, string nuAgrupador, string tpAgrupador)
        {
            var queryDetalles = new DetallesAgrupadorQuery(nuAgrupador, tpAgrupador);

            uow.HandleQuery(queryDetalles);
            queryDetalles.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryDetalles.GetCount()
            };
        }

        #region Formulario

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var agrupador = uow.DocumentoRepository.GetAgrupador(nuAgrupador, tpAgrupador);
                if (agrupador.FechaLlegada != null || agrupador.Estado == EstadoDocumentoAgrupador.CANCELADO)
                {
                    form.GetField("fechLlegada").Value = agrupador.FechaLlegada == null ? "" : agrupador.FechaLlegada.ToIsoString();
                    form.GetField("fechLlegada").Disabled = true;
                    form.GetButton("btnSubmit").Disabled = true;

                }
                else
                {
                    IConfirmMessage confirmarAgrupacion = new ConfirmMessage()
                    {
                        AcceptLabel = "DOC350_Sec0_btn_ConfirmarOk",
                        CancelLabel = "DOC350_Sec0_btn_ConfirmarCancel",
                        Message = "DOC350_Sec0_btn_MsgConfirmar"
                    };

                    form.GetButton("btnSubmit").ConfirmMessage = confirmarAgrupacion;
                }

                context.AddParameter("fechaImpreso", agrupador.FechaImpreso == null ? "-" : agrupador.FechaImpreso.ToString("dd/MM/yyyy H:mm"));
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new DOC350FormValidationModule(nuAgrupador, tpAgrupador, uow, this._identity), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                var nuTransaccion = uow.GetTransactionNumber();
                var agrupador = uow.DocumentoRepository.GetAgrupador(nuAgrupador, tpAgrupador);

                DateTime fechaLlegada = DateTime.Parse(form.GetField("fechLlegada").Value, null, System.Globalization.DateTimeStyles.RoundtripKind);
                agrupador.FechaLlegada = fechaLlegada;

                uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);
                uow.SaveChanges();
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                var nuTransaccion = uow.GetTransactionNumber();
                var agrupador = uow.DocumentoRepository.GetAgrupador(nuAgrupador, tpAgrupador);

                if (agrupador != null)
                {
                    agrupador.MarcarImpresion();
                    uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);
                    uow.SaveChanges();
                }

                context.AddParameter("fechaImpreso", agrupador.FechaImpreso == null ? "-" : agrupador.FechaImpreso.ToString("dd/MM/yyyy H:mm"));
            }

            return form;
        }

        #endregion
    }
}



