using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
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
    public class DOC410 : AppController
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

        protected List<string> GridKeysAjuste { get; }
        protected List<string> GridKeysDocumento { get; }

        public DOC410(ISessionAccessor session,
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

            this.GridKeysAjuste = new List<string>
            {
             "CD_PRODUTO","CD_FAIXA","NU_IDENTIFICADOR","CD_EMPRESA","NU_DOCUMENTO","TP_DOCUMENTO","NU_DOCUMENTO_CAMBIO","TP_DOCUMENTO_CAMBIO"
            };
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;

            grid.MenuItems.Add(new GridButton
            {
                Id = "btnDeshacer",
                Label = "DOC410_Sec0_btn_AgregarUbicacion"
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
            var queryIngreso = new DocumentoAnularCambioDOC410Query();

            uow.HandleQuery(queryIngreso);

            var defaultSortAjuste = new SortCommand("DT_ADDROW", SortDirection.Descending);

            grid.Rows = this._gridService.GetRows(queryIngreso, grid.Columns, context, defaultSortAjuste, this.GridKeysAjuste);


            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return ExporExcel(uow, grid, context);
            }
        }

        public virtual byte[] ExporExcel(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var dbQuery = new DocumentoAjustesDOC400Query();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            context.FileName = "DocumentoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.ButtonId == "btnDeshacer")
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var nuTransaccion = uow.GetTransactionNumber();
                    var query = new DocumentoAnularCambioDOC410Query();

                    uow.HandleQuery(query);

                    var keysRowSelected = query.GetKeysRowsSelected(context.Selection.AllSelected, context.Selection.Keys, _identity.GetFormatProvider());

                    if (keysRowSelected.Count() > 0)
                    {
                        keysRowSelected.ForEach(x =>
                        {
                            var cdproduto = x[0];
                            var cdfaixa = decimal.Parse(x[1], _identity.GetFormatProvider());
                            var nuidentificador = x[2];
                            var cdEmpresa = int.Parse(x[3]);
                            var nuDocOrigen = x[4];
                            var tpDocOrigen = x[5];
                            var nuDocCambio = x[6];
                            var tpDocCambio = x[7];
                            var detalleCambio = uow.DocumentoRepository.GetDetalleDocumento(cdproduto, cdfaixa, nuidentificador, cdEmpresa, nuDocCambio, tpDocCambio);
                            var detalleOrigen = uow.DocumentoRepository.GetDetalleDocumento(cdproduto, cdfaixa, nuidentificador, cdEmpresa, nuDocOrigen, tpDocOrigen);

                            detalleOrigen.CantidadIngresada = detalleOrigen.CantidadIngresada + detalleCambio.CantidadIngresada;

                            detalleCambio.FechaModificacion = DateTime.Now;
                            detalleCambio.NumeroTransaccionDelete = nuTransaccion;

                            uow.DocumentoRepository.UpdateDetailWithoutDocument(nuDocCambio, tpDocCambio, detalleCambio, nuTransaccion);
                            uow.SaveChanges();

                            IDocumentoIngreso documentoCambio = null;
                            documentoCambio = uow.DocumentoRepository.GetIngreso(nuDocCambio, tpDocCambio);
                            uow.DocumentoRepository.RemoveDetail(documentoCambio, detalleCambio, nuTransaccion);

                            IDocumentoIngreso documentoOrigen = null;
                            documentoOrigen = uow.DocumentoRepository.GetIngreso(nuDocOrigen, tpDocOrigen);
                            uow.DocumentoRepository.UpdateDetail(documentoOrigen, detalleOrigen, nuTransaccion);

                            uow.SaveChanges();

                            if (!uow.DocumentoRepository.GetAnyDetalleDocumento(nuDocCambio, tpDocCambio))
                            {
                                documentoCambio.NumeroTransaccionDelete = nuTransaccion;
                                documentoCambio.FechaModificacion = DateTime.Now;

                                uow.DocumentoRepository.UpdateIngreso(documentoCambio, nuTransaccion);
                                uow.SaveChanges();

                                uow.DocumentoRepository.RemoveDocumento(documentoCambio, nuTransaccion);
                            }

                            uow.DocumentoRepository.DeleteCambioDocDet(cdproduto, cdfaixa, nuidentificador, cdEmpresa, nuDocCambio, tpDocCambio, nuDocOrigen, tpDocOrigen, nuDocCambio, tpDocCambio);
                        });

                        uow.SaveChanges();
                        uow.Commit();
                    }
                }
            }
            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var queryIngreso = new DocumentoAnularCambioDOC410Query();

            uow.HandleQuery(queryIngreso);
            queryIngreso.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryIngreso.GetCount()
            };
        }

        #endregion
    }
}
