using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento.Integracion.Stock;
using WIS.Domain.ManejoStock;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC362 : AppController
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
        protected readonly IParameterService _parameterService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }
        protected List<string> GridKeysAjuste { get; }

        public DOC362(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            IFactoryService factoryService,
            IParameterService parameterService)
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
            this._parameterService = parameterService;

            this.GridKeys = new List<string>
            {
                "NU_DOCUMENTO", "TP_DOCUMENTO"
            };
            this.GridKeysAjuste = new List<string>
            {
                "NU_AJUSTE_STOCK"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "DOC362_grid_1")
            {
                grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                    new GridItemHeader("DOC362_Sec0_lbl_Acciones"),
                    new GridButton("btnGenDocumento", "DOC362_Sec0_btn_GenerarDocumento", "fas fa-clipboard-list"),

                }));
            }
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                switch (grid.Id)
                {
                    case "DOC362_grid_1":
                        var empresa = _session.GetValue<string>("DOC361_CD_EMPRESA");

                        if (!string.IsNullOrEmpty(empresa))
                            return GridDocumentoFetchRow(grid, context, uow, empresa);
                        else
                            context.AddErrorNotification("DOC362_Sec0_Error_ErrorParametros");

                        return grid;

                    case "DOC362_grid_ajustes":
                        var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
                        return GridAjusteFetchRow(grid, context, uow, nrosAjuste);
                }
            }

            return grid;
        }

        public virtual Grid GridDocumentoFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, string empresa)
        {
            var query = new DocumentoAjusteStockPositivoDOC362Query(int.Parse(empresa));

            uow.HandleQuery(query);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);
            grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public virtual Grid GridAjusteFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, List<int> nrosAjuste)
        {
            var query = new DocumentoAjusteSeleccionadosStockPositivoDOC362Query(nrosAjuste);

            uow.HandleQuery(query);

            var defaultSortAjuste = new SortCommand("NU_AJUSTE_STOCK", SortDirection.Descending);

            grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSortAjuste, this.GridKeysAjuste);

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {

            if (context.ButtonId == "btnGenDocumento")
            {
                this.CrearActaIngresoDocumental(context);
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                switch (grid.Id)
                {
                    case "DOC362_grid_1":
                        var empresa = _session.GetValue<string>("DOC361_CD_EMPRESA");

                        return GridExportExcelGrid1(grid, context, uow);

                    case "DOC362_grid_ajustes":
                        var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
                        return GridExportExcelGridAjustes(grid, context, uow, nrosAjuste);
                }

                return null;
            }
        }

        public virtual byte[] GridExportExcelGrid1(Grid grid, GridExportExcelContext context, IUnitOfWork uow)
        {
            var cdEmpresaParam = _session.GetValue<string>("DOC361_CD_EMPRESA");

            int empresa;
            empresa = int.Parse(cdEmpresaParam);

            var dbQuery = new DocumentoAjusteStockPositivoDOC362Query(empresa);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            context.FileName = "DocumentoIngresoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
        public virtual byte[] GridExportExcelGridAjustes(Grid grid, GridExportExcelContext context, IUnitOfWork uow, List<int> nrosAjuste)
        {
            var dbQuery = new DocumentoAjusteSeleccionadosStockPositivoDOC362Query(nrosAjuste);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            context.FileName = "DocumentoIngresoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            switch (grid.Id)
            {
                case "DOC362_grid_1":
                    var empresa = _session.GetValue<string>("DOC361_CD_EMPRESA");
                    return GridFetchStatsGrid1(grid, context, empresa);

                case "DOC362_grid_ajustes":
                    var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
                    return GridFetchStatsGridAjustes(grid, context, nrosAjuste);
            }

            return null;
        }
        public virtual GridStats GridFetchStatsGrid1(Grid grid, GridFetchStatsContext context, string empresa)
        {
            if (!string.IsNullOrEmpty(empresa))
            {
                var query = new DocumentoAjusteStockPositivoDOC362Query(int.Parse(empresa));

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
        public virtual GridStats GridFetchStatsGridAjustes(Grid grid, GridFetchStatsContext context, List<int> nrosAjuste)
        {
            var query = new DocumentoAjusteSeleccionadosStockPositivoDOC362Query(nrosAjuste);

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }

        public virtual void CrearActaIngresoDocumental(GridButtonActionContext context)
        {
            try
            {
                var ajustesSeleccionados = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));

                if (ajustesSeleccionados != null && ajustesSeleccionados.Count > 0)
                {
                    var nuDocumento = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "NU_DOCUMENTO").Value;
                    var tpDocumento = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "TP_DOCUMENTO").Value;

                    using (var uow = this._uowFactory.GetUnitOfWork())
                    {
                        uow.CreateTransactionNumber("DOC362 CrearActaIngresoDocumental");

                        var documento = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);

                        List<InformacionActaStock> infoLineasActa = new List<InformacionActaStock>();
                        NivelacionAjusteDocumental nivelacion = new NivelacionAjusteDocumental();

                        List<DocumentoAjusteStock> ajustes = uow.AjusteRepository.GetAjustesDocumento(ajustesSeleccionados);

                        int numeroOperacion = int.Parse(uow.AjusteRepository.GetNumeroOperacionAjuste());

                        foreach (var ajuste in ajustes)
                        {
                            // Sumo las cantidades de movimientos en caso que exista mas de un ajuste para el mismo stock
                            InformacionActaStock infoActaStock = infoLineasActa
                                .FirstOrDefault(s => s.Empresa == (int)ajuste.CodigoEmpresa
                                    && s.Producto == ajuste.Producto
                                    && s.Faixa == (decimal)ajuste.Faixa
                                    && s.NumeroIdentificador == ajuste.Identificador);

                            if (infoActaStock == null)
                            {
                                infoActaStock = new InformacionActaStock()
                                {
                                    Empresa = (int)ajuste.CodigoEmpresa,
                                    Producto = ajuste.Producto,
                                    Faixa = (decimal)ajuste.Faixa,
                                    NumeroIdentificador = ajuste.Identificador,
                                    CantidadActa = ajuste.CantidadMovimiento,
                                    CIF = 0,
                                    FOB = 0,
                                    Tributo = 0
                                };
                                infoLineasActa.Add(infoActaStock);
                            }
                            else
                            {
                                infoActaStock.CantidadActa = infoActaStock.CantidadActa + ajuste.CantidadMovimiento;
                            }

                            var lineaIngreso = documento.Lineas.FirstOrDefault(l => l.Producto == ajuste.Producto && l.Identificador == ajuste.Identificador);

                            if (lineaIngreso != null)
                            {
                                infoActaStock.FOB = lineaIngreso.ValorMercaderia;
                                infoActaStock.CIF = infoActaStock.FOB;
                                infoActaStock.Tributo = lineaIngreso.ValorTributo;
                            }

                            DocumentoAjusteStockHistorico historicoPositivo = new DocumentoAjusteStockHistorico(ajuste)
                            {
                                CantidadMovimiento = Math.Abs((decimal)ajuste.CantidadMovimiento),
                                NumeroOperacion = numeroOperacion,
                                TipoOperacion = TipoOperacion.ACTA_STOCK,
                            };

                            nivelacion.ajustesHistoricosAgregar.Add(historicoPositivo);
                            nivelacion.ajustesEliminar.Add(ajuste);
                        }

                        var ajusteDocumental = new Domain.Documento.Integracion.Stock.AjusteStockDocumental(this._factoryService, this._parameterService);
                        var actaStock = ajusteDocumental.CrearActaStock(uow, documento, infoLineasActa, this._identity.UserId, true);

                        foreach (DocumentoAjusteStockHistorico historico in nivelacion.ajustesHistoricosAgregar)
                        {
                            historico.NumeroDocumento = actaStock.Numero;
                            historico.TipoDocumento = actaStock.Tipo;
                            historico.FechaActualizacion = DateTime.Now;
                        }

                        Domain.ManejoStock.AjusteStockDocumental.AjusteStockDocumental ajusteStockDocumental = new Domain.ManejoStock.AjusteStockDocumental.AjusteStockDocumental();
                        ajusteStockDocumental.ImpactarNivelacion(uow, nivelacion);

                        uow.SaveChanges();
                        context.Redirect("/documento/DOC360", new List<ComponentParameter>() { });
                        context.AddSuccessNotification("DOC362_Sec0_Success_ActaCreada");
                    }
                }
                else
                {
                    context.AddErrorNotification("DOC362_Sec0_Error_ErrorParametros");
                }

            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification("DOC362_Sec0_Not_ErrorCrearActaStock", new List<string> { ex.Message });
            }
        }
    }
}
