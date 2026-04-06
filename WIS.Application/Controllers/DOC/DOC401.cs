using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Documento;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Documento.Serializables.Salida;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC401 : AppController
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

        protected List<string> GridKeysAjuste { get; }
        protected List<string> GridKeysDocumento { get; }

        public DOC401(ISessionAccessor session,
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

            this.GridKeysAjuste = new List<string>
            {
               "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "CD_EMPRESA", "NU_DOCUMENTO_INGRESO", "TP_DOCUMENTO_INGRESSO"
            };
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.MenuItems.Add(new GridButton
            {
                Id = "btnAceptar",
                Label = "DOC401_Sec0_btn_Aceptar"
            });

            grid.MenuItems.Add(new GridButton
            {
                Id = "btnCancelar",
                Label = "DOC401_Sec0_btn_Cancelar"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return GridFetchRow(grid, context, uow);
            }
        }

        public virtual Grid GridFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            var queryIngreso = new DocumentoAjustesDOC401Query();

            uow.HandleQuery(queryIngreso);

            var defaultSortAjuste = new SortCommand("DT_ADDROW", SortDirection.Descending);

            grid.Rows = this._gridService.GetRows(queryIngreso, grid.Columns, context, defaultSortAjuste, this.GridKeysAjuste);

            foreach (var row in grid.Rows)
            {
                row.GetCell("QT_MOVIMIENTO").Editable = true;
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new DOC401GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return ExporExcel(uow, grid, context);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var queryIngreso = new DocumentoAjustesDOC401Query();

            uow.HandleQuery(queryIngreso);
            queryIngreso.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryIngreso.GetCount()
            };
        }

        public virtual byte[] ExporExcel(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {

            var dbQuery = new DocumentoAjustesDOC401Query();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            context.FileName = "DocumentoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            var culture = this._identity.GetFormatProvider();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();
                    string fecha = DateTime.Now.ToString();

                    foreach (var row in grid.Rows)
                    {
                        var docNuevo = row.GetCell("NU_DOC").Value;
                        var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                        var lote = row.GetCell("NU_IDENTIFICADOR").Value;
                        var faxia = decimal.Parse(row.GetCell("CD_FAIXA").Value, culture);
                        var NU_DOCUMENTO_INGRESO = row.GetCell("NU_DOCUMENTO_INGRESO").Value;
                        var TP_DOCUMENTO_INGRESSO = row.GetCell("TP_DOCUMENTO_INGRESSO").Value;
                        var tpdocNue = row.GetCell("TP_DOCUMENTO_CAMBIO").Value;
                        var CD_PRODUTO = row.GetCell("CD_PRODUTO").Value;
                        var QT_MOVIMIENTO = decimal.Parse(row.GetCell("QT_MOVIMIENTO").Value, culture);

                        uow.DocumentoRepository.GetCambioDocPre(docNuevo, tpdocNue, empresa, lote, faxia, NU_DOCUMENTO_INGRESO, TP_DOCUMENTO_INGRESSO, CD_PRODUTO, QT_MOVIMIENTO);

                    }
                    uow.SaveChanges();
                    uow.Commit();

                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    uow.Rollback();
                    throw ex;
                }
            }

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.ButtonId == "btnAceptar")
            {
                var egreso = new EgresoDocumental(this._factoryService);

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var keysRowSelected = uow.DocumentoRepository.GetDatosVistaDOC401();

                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var fecha = DateTime.Now.ToString("dd/MM/yyyy");

                    if (keysRowSelected.Count() > 0)
                    {
                        keysRowSelected.ForEach(x =>
                        {
                            var existeDoc = egreso.ValidarCambioIngr(uow, fecha, x.NU_DOC, x.TP_DOCUMENTO_CAMBIO, this._identity.Application, this._identity.UserId, false);
                            var doc = new CambioDocumentoDetIngreso();

                            doc.NU_DOCUMENTO_NUEVO = x.NU_DOC;
                            doc.TP_DOCUMENTO_NUEVO = x.TP_DOCUMENTO_CAMBIO;
                            doc.NU_DOCUMENTO_EGRESO = fecha;
                            doc.TP_DOCUMENTO_EGRESO = x.NU_DOC;
                            doc.CD_EMPRESA = x.CD_EMPRESA;
                            doc.NU_IDENTIFICADOR = x.NU_IDENTIFICADOR;
                            doc.CD_FAIXA = x.CD_FAIXA;
                            doc.NU_DOCUMENTO_INGRESO_ORIGINAL = x.NU_DOCUMENTO_INGRESO;
                            doc.TP_DOCUMENTO_INGRESO_ORIGINAL = x.TP_DOCUMENTO_INGRESSO;
                            doc.NU_DOCUMENTO_INGRESO = x.NU_DOCUMENTO_INGRESO;
                            doc.TP_DOCUMENTO_INGRESO = x.TP_DOCUMENTO_INGRESSO;
                            doc.QT_MOVIMIENTO = x.QT_MOVIMIENTO ?? 0;
                            doc.CD_PRODUTO = x.CD_PRODUTO;
                            doc.existeDoc = existeDoc;
                            doc.userId = this._identity.UserId;
                            doc.page = this._identity.Application;

                            egreso.CambioIngresoDOC400(uow, doc, fecha);

                        });

                        uow.SaveChanges();
                        uow.DocumentoRepository.UpdateProcesarCambioDocPre();
                        uow.SaveChanges();
                    }
                    uow.Commit();
                }
            }
            else
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.DocumentoRepository.CancelarCambioDocPre();
                    uow.SaveChanges();
                }
            }

            return context;
        }

        #endregion
    }
}
