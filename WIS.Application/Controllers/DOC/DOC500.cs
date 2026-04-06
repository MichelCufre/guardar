using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Documento;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Documento.Serializables.Salida;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC500 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFactoryService _factoryService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public DOC500(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            IFactoryService factoryService)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._factoryService = factoryService;

            this.GridKeys = new List<string>
            {
             "NU_DOCUMENTO_EGRESO","NU_DOCUMENTO_EGRESO_PRDC","TP_DOCUMENTO_EGRESO_PRDC","TP_DOCUMENTO_INGRESO"
             ,"NU_DOCUMENTO_INGRESO","TP_DOCUMENTO_INGRESO_ORIGINAL","NU_DOCUMENTO_INGRESO_ORIGINAL"
             ,"CD_EMPRESA","CD_PRODUTO","CD_FAIXA","NU_IDENTIFICADOR","CD_PRODUTO_PRODUCIDO","NU_NIVEL"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnSelect("TP_DOC", this.OptionSelectCierreConteo()));

            return GridFetchRows(grid, context.FetchContext);
        }

        protected virtual List<SelectOption> OptionSelectCierreConteo()
        {
            List<SelectOption> opciones = new List<SelectOption>() {
                new SelectOption("DI","DI - Declaración de Importación"),
                new SelectOption("DE","DE - Declaración de Importación "),
            };

            return opciones;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string documento = context.GetParameter("nuDocumento");
                string tpDocumento = context.GetParameter("tpDocumento");
                string empresaString = context.GetParameter("cdEmpresa");

                int? empresa = !string.IsNullOrEmpty(empresaString) ? int.Parse(empresaString) : default(int?);

                var query = new DOC500Query(documento, "EP", empresa);

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    if (string.IsNullOrEmpty(row.GetCell("NU_DOCUMENTO_CAMBIO").Value))
                    {
                        row.GetCell("NU_DOC").Editable = true;
                        row.GetCell("TP_DOC").Editable = true;
                        row.GetCell("NU_DOC_CONF").Editable = true;
                    }
                }
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new DOC500GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string documento = context.GetParameter("nuDocumento");
                string tpDocumento = context.GetParameter("tpDocumento");
                string empresaString = context.GetParameter("cdEmpresa");

                int empresa;
                int.TryParse(empresaString, out empresa);

                var dbQuery = new DOC500Query(documento, tpDocumento, empresa);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = "CambioDocumento_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            var egreso = new EgresoDocumental(this._factoryService);
            var culture = this._identity.GetFormatProvider();

            try
            {
                string fecha = DateTime.Now.ToString("dd/MM/yyyy");

                foreach (var row in grid.Rows)
                {
                    var docNuevo = row.GetCell("NU_DOC").Value;
                    var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    var lote = row.GetCell("NU_IDENTIFICADOR").Value;
                    var faxia = decimal.Parse(row.GetCell("CD_FAIXA").Value, culture);
                    var NU_DOCUMENTO_INGRESO = row.GetCell("NU_DOCUMENTO_INGRESO").Value;
                    var NU_DOCUMENTO_EGRESO = row.GetCell("NU_DOCUMENTO_EGRESO").Value;
                    var NU_DOCUMENTO_EGRESO_PRDC = row.GetCell("NU_DOCUMENTO_EGRESO_PRDC").Value;
                    var TP_DOCUMENTO_EGRESO_PRDC = row.GetCell("TP_DOCUMENTO_EGRESO_PRDC").Value;
                    var TP_DOCUMENTO_INGRESSO = row.GetCell("TP_DOCUMENTO_INGRESO_ORIGINAL").Value;
                    var CD_PRODUTO_PRODUCIDO = row.GetCell("CD_PRODUTO_PRODUCIDO").Value;
                    var CD_PRODUTO = row.GetCell("CD_PRODUTO").Value;
                    var QT_MOVIMIENTO = decimal.Parse(row.GetCell("QT_MOVIMIENTO").Value, culture);
                    var TP_DOC = row.GetCell("TP_DOC").Value;
                    var NU_NIVEL = int.Parse(row.GetCell("NU_NIVEL").Value);
                    var existeDoc = false;

                    using (var uow = this._uowFactory.GetUnitOfWork())
                    {
                        uow.CreateTransactionNumber(this._identity.Application);
                        uow.BeginTransaction();

                        existeDoc = egreso.ValidarCambioIngr(uow, fecha, docNuevo, TP_DOC, this._identity.Application, this._identity.UserId, false);

                        var doc = new CambioDocumentoDetIngreso();
                        doc.NU_DOCUMENTO_NUEVO = docNuevo;
                        doc.TP_DOCUMENTO_NUEVO = TP_DOC;
                        doc.NU_DOCUMENTO_EGRESO = NU_DOCUMENTO_EGRESO;
                        doc.NU_DOCUMENTO_EGRESO_PRDC = NU_DOCUMENTO_EGRESO_PRDC;
                        doc.TP_DOCUMENTO_EGRESO_PRDC = TP_DOCUMENTO_EGRESO_PRDC;
                        doc.CD_EMPRESA = empresa;
                        doc.NU_IDENTIFICADOR = lote;
                        doc.CD_FAIXA = faxia;
                        doc.NU_DOCUMENTO_INGRESO_ORIGINAL = NU_DOCUMENTO_INGRESO;
                        doc.TP_DOCUMENTO_INGRESO_ORIGINAL = TP_DOCUMENTO_INGRESSO;
                        doc.NU_DOCUMENTO_INGRESO = NU_DOCUMENTO_INGRESO;
                        doc.TP_DOCUMENTO_INGRESO = TP_DOCUMENTO_INGRESSO;
                        doc.QT_MOVIMIENTO = QT_MOVIMIENTO;
                        doc.CD_PRODUTO = CD_PRODUTO;
                        doc.CD_PRODUTO_PRODUCIDO = CD_PRODUTO_PRODUCIDO;
                        doc.existeDoc = existeDoc;
                        doc.userId = this._identity.UserId;
                        doc.NU_NIVEL = NU_NIVEL;
                        doc.page = this._identity.Application;

                        egreso.CambioIngreso(doc, uow, false, fecha);

                        uow.SaveChanges();
                        uow.Commit();
                    }
                }

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            string documento = context.GetParameter("nuDocumento");
            string tpDocumento = context.GetParameter("tpDocumento");
            string empresaString = context.GetParameter("cdEmpresa");

            int? empresa = !string.IsNullOrEmpty(empresaString) ? int.Parse(empresaString) : default(int?);

            var query = new DOC500Query(documento, "EP", empresa);

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }
    }
}
