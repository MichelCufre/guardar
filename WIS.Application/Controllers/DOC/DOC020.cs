using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class DOC020 : AppController
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

        protected List<string> GridKeys { get; set; }

        public DOC020(ISessionAccessor session,
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

            this.GridKeys = new List<string>
            {
               "TP_DOCUMENTO", "NU_DOCUMENTO", "CD_PRODUTO", "CD_EMPRESA", "CD_FAIXA", "NU_IDENTIFICADOR"
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            decimal? disponible = _session.GetValue<decimal?>("DOC020_QT_DISPONIBLE");
            decimal? reservada = _session.GetValue<decimal?>("DOC020_QT_RESERVADA");
            decimal? mercaderia = _session.GetValue<decimal?>("DOC020_QT_MERCADERIA");

            decimal? ingresado = _session.GetValue<decimal?>("DOC020_QT_INGRESADA");
            decimal? desafectada = _session.GetValue<decimal?>("DOC020_QT_DESAFECTADA");
            decimal? existencia = _session.GetValue<decimal?>("DOC020_QT_EXISTENCIA");

            form.GetField("QT_DISPONIBLE").Value = Math.Round((disponible ?? 0), 2).ToString();
            form.GetField("QT_RESERVADA").Value = Math.Round((reservada ?? 0), 2).ToString(); ;
            form.GetField("QT_MERCADERIA").Value = Math.Round((mercaderia ?? 0), 2).ToString(); ;

            form.GetField("QT_INGRESADA").Value = Math.Round((ingresado ?? 0), 2).ToString(); ;
            form.GetField("QT_DESAFECTADA").Value = Math.Round((desafectada ?? 0), 2).ToString(); ;
            form.GetField("QT_EXISTENCIA").Value = Math.Round((existencia ?? 0), 2).ToString(); ;

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            string documento = context.GetParameter("nuDocumento");
            string tpDocumento = context.GetParameter("tpDocumento");
            string cdEmpresa = context.GetParameter("cdEmpresa");
            int empresa;

            if (!string.IsNullOrEmpty(documento) && !string.IsNullOrEmpty(tpDocumento) && int.TryParse(cdEmpresa, out empresa))
            {
                grid.MenuItems.Add(new GridButton("btnLimpiarFiltro", "DOC020_Sec0_btn_LimpiarFiltro"));
            }
            else
            {
                grid.MenuItems = new List<IGridItem>();
            }

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string documento = context.GetParameter("nuDocumento");
                string tpDocumento = context.GetParameter("tpDocumento");
                string cdEmpresa = context.GetParameter("cdEmpresa");

                var tiposDeIngreso = uow.DocumentoTipoRepository.GetTiposDocumentoIngresoModificacion();

                int? empresa = !string.IsNullOrEmpty(cdEmpresa) ? int.Parse(cdEmpresa) : default(int?);

                var query = new DocumetoSaldoQuery(documento, tpDocumento, empresa, tiposDeIngreso);

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

                ResultadoSaldosDocumento saldos = query.GetSumaSaldos();

                this._session.SetValue("DOC020_QT_DISPONIBLE", saldos.Disponible);
                this._session.SetValue("DOC020_QT_RESERVADA", saldos.Reservado);
                this._session.SetValue("DOC020_QT_MERCADERIA", saldos.Mercaderia);

                this._session.SetValue("DOC020_QT_INGRESADA", saldos.Ingresado);
                this._session.SetValue("DOC020_QT_DESAFECTADA", saldos.Desafectada);
                this._session.SetValue("DOC020_QT_EXISTENCIA", saldos.Existencia);
            }

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.ButtonId == "btnLimpiarFiltro")
            {
                context.Redirect = "/documento/DOC020";
            }
            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string documento = context.GetParameter("nuDocumento");
                string tpDocumento = context.GetParameter("tpDocumento");
                string cdEmpresa = context.GetParameter("cdEmpresa");

                var tiposDeIngreso = uow.DocumentoTipoRepository.GetTiposDocumentoIngresoModificacion();

                int empresa;
                int.TryParse(cdEmpresa, out empresa);

                var dbQuery = new DocumetoSaldoQuery(documento, tpDocumento, empresa, tiposDeIngreso);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = "SaldoDocumentos_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string documento = context.GetParameter("nuDocumento");
            string tpDocumento = context.GetParameter("tpDocumento");
            string cdEmpresa = context.GetParameter("cdEmpresa");

            var tiposDeIngreso = uow.DocumentoTipoRepository.GetTiposDocumentoIngresoModificacion();

            int? empresa = !string.IsNullOrEmpty(cdEmpresa) ? int.Parse(cdEmpresa) : default(int?);

            var query = new DocumetoSaldoQuery(documento, tpDocumento, empresa, tiposDeIngreso);

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }
    }
}
