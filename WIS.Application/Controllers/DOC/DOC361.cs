using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Exceptions;
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
    public class DOC361 : AppController
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

        protected List<string> GridKeys { get; }

        public DOC361(ISessionAccessor session,
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
               "NU_AJUSTE_STOCK"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = (new List<IGridItem> {
                new GridItemHeader("DOC361_Sec0_lbl_Acciones"),
                new GridButton("btnSeleccionar", "DOC361_grid1_btn_ConfirmarSeleccion", "fas fa-plus")
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var cdEmpresaParam = context.GetParameter("cdEmpresa");
                var esPositivoParam = context.GetParameter("ajPositivo");

                int empresa;
                bool esPositivo;

                if (int.TryParse(cdEmpresaParam, out empresa) && bool.TryParse(esPositivoParam, out esPositivo))
                {
                    var query = new DocumentoAjustesStockDOC361Query(empresa, esPositivo);

                    uow.HandleQuery(query);

                    var defaultSort = new SortCommand("NU_AJUSTE_STOCK", SortDirection.Ascending);
                    grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);
                }
                else
                {
                    context.AddErrorNotification("DOC341_Sec0_Error_ErrorParametros");
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var cdEmpresaParam = context.GetParameter("cdEmpresa");
                var esPositivoParam = context.GetParameter("ajPositivo");

                int empresa;
                bool esPositivo;

                empresa = int.Parse(cdEmpresaParam);
                esPositivo = bool.Parse(esPositivoParam);

                var dbQuery = new DocumentoAjustesStockDOC361Query(empresa, esPositivo);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_AJUSTE_STOCK", SortDirection.Descending);

                context.FileName = "DocumentoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            try
            {
                switch (context.ButtonId)
                {
                    case "btnSeleccionar":
                        this.ProcesarSeleccion(context);
                        break;
                }
            }
            catch (ValidationFailedException ex)
            {
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var cdEmpresaParam = context.GetParameter("cdEmpresa");
            var esPositivoParam = context.GetParameter("ajPositivo");

            int empresa;
            bool esPositivo;

            if (int.TryParse(cdEmpresaParam, out empresa) && bool.TryParse(esPositivoParam, out esPositivo))
            {
                var query = new DocumentoAjustesStockDOC361Query(empresa, esPositivo);

                using var uow = this._uowFactory.GetUnitOfWork();

                uow.HandleQuery(query);
                query.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = query.GetCount()
                };
            }

            return null;
        }

        public virtual GridMenuItemActionContext ProcesarSeleccion(GridMenuItemActionContext context)
        {
            List<int> ajustesSeleccionados = new List<int>();
            bool esPositivo;

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (context.Selection.AllSelected) //Select all
                {
                    ajustesSeleccionados = this.HandleSelectAll(context, uow, out esPositivo);
                }
                else //Select individual
                {
                    ajustesSeleccionados = this.HandleSelect(context, uow, out esPositivo);
                }
            }

            var cdEmpresaParam = context.GetParameter("cdEmpresa");

            _session.SetValue("DOC361_AJUSTES_SELECCIONADOS", JsonConvert.SerializeObject(ajustesSeleccionados));
            _session.SetValue("DOC361_CD_EMPRESA", cdEmpresaParam);

            if (esPositivo)
                context.Redirect = "/documento/DOC362";
            else
                context.Redirect = "/documento/DOC363";

            return context;
        }

        public virtual List<int> HandleSelectAll(GridMenuItemActionContext context, IUnitOfWork uow, out bool esPositivo)
        {
            esPositivo = false;

            List<int> nrosAjuste = new List<int>();

            var cdEmpresaParam = context.GetParameter("cdEmpresa");
            var esPositivoParam = context.GetParameter("ajPositivo");

            int empresa;

            if (int.TryParse(cdEmpresaParam, out empresa) && bool.TryParse(esPositivoParam, out esPositivo))
            {
                var dbQuery = new DocumentoAjustesStockDOC361Query(empresa, esPositivo);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                List<string> keysSelectAll = dbQuery.GetResult().Select(a => a.NU_AJUSTE_STOCK.ToString()).ToList();

                foreach (string key in context.Selection.Keys)
                {
                    var ajusteDesmarcado = keysSelectAll.FirstOrDefault(k => k == key);

                    if (!string.IsNullOrEmpty(ajusteDesmarcado))
                        keysSelectAll.Remove(ajusteDesmarcado);
                }

                if (keysSelectAll.Count == 0)
                {
                    throw new ValidationFailedException("DOC341_Sec0_Error_SeleccionVacio");
                }

                //Controlar cantidades
                nrosAjuste = keysSelectAll.Select(k => int.Parse(k)).ToList();
                var ajustesSeleccionados = uow.AjusteRepository.GetAjustesDocumento(nrosAjuste);

                if (ajustesSeleccionados.Any(a => a.CantidadMovimiento > 0) && ajustesSeleccionados.Any(a => a.CantidadMovimiento < 0))
                {
                    throw new ValidationFailedException("DOC341_Sec0_Error_SeleccionCombinada");
                }
            }
            else
            {
                context.AddErrorNotification("DOC341_Sec0_Error_ErrorParametros");
            }

            return nrosAjuste;
        }

        public virtual List<int> HandleSelect(GridMenuItemActionContext context, IUnitOfWork uow, out bool esPositivo)
        {
            List<string> seleccion = context.Selection.Keys;

            if (seleccion.Count == 0)
            {
                throw new ValidationFailedException("DOC341_Sec0_Error_SeleccionVacio");
            }

            //Controlar cantidades
            List<int> nrosAjuste = seleccion.Select(k => int.Parse(k)).ToList();
            var ajustesSeleccionados = uow.AjusteRepository.GetAjustesDocumento(nrosAjuste);

            if (ajustesSeleccionados.Any(a => a.CantidadMovimiento > 0) && ajustesSeleccionados.Any(a => a.CantidadMovimiento < 0))
            {
                throw new ValidationFailedException("DOC341_Sec0_Error_SeleccionCombinada");
            }

            var esPositivoParam = context.GetParameter("ajPositivo");

            if (string.IsNullOrEmpty(esPositivoParam))
            {
                throw new ValidationFailedException("DOC341_Sec0_Error_ErrorParametros");
            }

            esPositivo = bool.Parse(esPositivoParam);

            return nrosAjuste;
        }
    }
}
