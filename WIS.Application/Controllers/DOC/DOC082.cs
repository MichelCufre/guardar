using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC082 : AppController
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

        protected List<string> GridDocumentoAsociadosKeys { get; }
        protected List<string> GridLogsLineaIngresoKeys { get; }
        protected List<string> GridLogsLineaEgresoKeys { get; }

        public DOC082(ISessionAccessor session,
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

            this.GridDocumentoAsociadosKeys = new List<string>
            {
               "NU_DOCUMENTO", "TP_DOCUMENTO", "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR", "TP_DOCUMENTO_ASOCIADO", "NU_DOCUMENTO_ASOCIADO"
            };

            this.GridLogsLineaIngresoKeys = new List<string>
            {
               "NU_LOG_SECUENCIA", "NU_DOCUMENTO"
            };

            this.GridLogsLineaEgresoKeys = new List<string>
            {
                "NU_SECUENCIA", "NU_DOCUMENTO", "TP_DOCUMENTO"
            };
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var rowsJson = _session.GetValue<string>("DOC082_ROWS");

            if (!string.IsNullOrEmpty(rowsJson))
            {
                List<ComponentParameter> rows;
                string producto, lote, nuDocumento, tpDocumento;
                decimal faixa;
                int empresa;

                GetFilterValues(rowsJson, out rows, out producto, out lote, out faixa, out empresa, out nuDocumento, out tpDocumento);

                context.Parameters.AddRange(rows);

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    if (grid.Id == "DOC082_grid_1")
                    {
                        var query = new DocumentoLineaDetQuery(producto, lote, faixa, empresa, nuDocumento, tpDocumento);

                        uow.HandleQuery(query);

                        var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                        grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridDocumentoAsociadosKeys);
                    }
                    else if (grid.Id == "DOC082_grid_2")
                    {
                        var query = new DocumentoDetalleQueryLT(producto, lote, faixa, empresa, nuDocumento, tpDocumento);

                        uow.HandleQuery(query);

                        if (query.Any())
                        {
                            context.Parameters.Add(new ComponentParameter()
                            {
                                Id = "mostrarGrid2",
                                Value = "true"
                            });

                            var defaultSort = new SortCommand("DT_LOG_ADD_ROW", SortDirection.Ascending);

                            grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridLogsLineaIngresoKeys);
                        }
                    }
                    else
                    {
                        DocumentoDetEgresoQuery query = this.GetQueryGrid3(producto, lote, nuDocumento, tpDocumento, faixa, empresa, uow);

                        uow.HandleQuery(query);

                        if (query.Any())
                        {
                            context.Parameters.Add(new ComponentParameter()
                            {
                                Id = "mostrarGrid3",
                                Value = "true"
                            });

                            var defaultSort = new SortCommand("DT_LOG_ADD_ROW", SortDirection.Ascending);

                            grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridLogsLineaEgresoKeys);
                        }
                    }
                }
            }

            return grid;
        }

        public virtual DocumentoDetEgresoQuery GetQueryGrid3(string producto, string lote, string nuDocumento, string tpDocumento, decimal faixa, int empresa, IUnitOfWork uow)
        {
            return new DocumentoDetEgresoQuery(producto, lote, faixa, empresa, nuDocumento, tpDocumento);
        }

        public virtual void GetFilterValues(string rowsJson, out List<ComponentParameter> rows, out string producto, out string lote, out decimal faixa, out int empresa, out string nuDocumento, out string tpDocumento)
        {
            rows = JsonConvert.DeserializeObject<List<ComponentParameter>>(rowsJson);

            producto = rows.FirstOrDefault(r => r.Id == "CD_PRODUTO").Value;
            lote = rows.FirstOrDefault(r => r.Id == "NU_IDENTIFICADOR").Value;
            var rFaixa = rows.FirstOrDefault(r => r.Id == "CD_FAIXA");
            var rEmpresa = rows.FirstOrDefault(r => r.Id == "CD_EMPRESA");
            faixa = -1;
            empresa = -1;
            if (rFaixa != null)
                decimal.TryParse(rFaixa.Value, out faixa);

            if (rEmpresa != null)
                int.TryParse(rEmpresa.Value, out empresa);

            if (rows.Any(r => r.Id == "NU_DOCUMENTO_EGRESO"))
            {
                nuDocumento = rows.FirstOrDefault(r => r.Id == "NU_DOCUMENTO_EGRESO").Value;
                tpDocumento = rows.FirstOrDefault(r => r.Id == "TP_DOCUMENTO_EGRESO").Value;
            }
            else
            {
                nuDocumento = rows.FirstOrDefault(r => r.Id == "NU_DOCUMENTO").Value;
                tpDocumento = rows.FirstOrDefault(r => r.Id == "TP_DOCUMENTO").Value;
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var rowsJson = _session.GetValue<string>("DOC082_ROWS");

            if (!string.IsNullOrEmpty(rowsJson))
            {
                List<ComponentParameter> rows;
                string producto, lote, nuDocumento, tpDocumento;
                decimal faixa;
                int empresa;

                GetFilterValues(rowsJson, out rows, out producto, out lote, out faixa, out empresa, out nuDocumento, out tpDocumento);

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    if (grid.Id == "DOC082_grid_1")
                    {
                        var dbQuery = new DocumentoLineaDetQuery(producto, lote, faixa, empresa, nuDocumento, tpDocumento);

                        uow.HandleQuery(dbQuery);

                        var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                        context.FileName = "DetalleLineaDoc_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                        return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                    }
                    else if (grid.Id == "DOC082_grid_2")
                    {
                        var dbQuery = new DocumentoDetalleQueryLT(producto, lote, faixa, empresa, nuDocumento, tpDocumento);

                        uow.HandleQuery(dbQuery);

                        var defaultSort = new SortCommand("DT_LOG_ADD_ROW", SortDirection.Ascending);

                        context.FileName = "DetalleLineaDocLogDoc_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                        return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                    }
                    else
                    {
                        DocumentoDetEgresoQuery dbQuery = GetQueryGrid3(producto, lote, nuDocumento, tpDocumento, faixa, empresa, uow);

                        uow.HandleQuery(dbQuery);

                        var defaultSort = new SortCommand("DT_LOG_ADD_ROW", SortDirection.Ascending);

                        context.FileName = "DetalleLineaDocLogDocEgreso_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                        return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var rowsJson = _session.GetValue<string>("DOC082_ROWS");

            if (!string.IsNullOrEmpty(rowsJson))
            {
                List<ComponentParameter> rows;
                string producto, lote, nuDocumento, tpDocumento;
                decimal faixa;
                int empresa;

                GetFilterValues(rowsJson, out rows, out producto, out lote, out faixa, out empresa, out nuDocumento, out tpDocumento);

                context.Parameters.AddRange(rows);

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    if (grid.Id == "DOC082_grid_1")
                    {
                        var query = new DocumentoLineaDetQuery(producto, lote, faixa, empresa, nuDocumento, tpDocumento);

                        uow.HandleQuery(query);
                        query.ApplyFilter(this._filterInterpreter, context.Filters);

                        return new GridStats
                        {
                            Count = query.GetCount()
                        };
                    }
                    else if (grid.Id == "DOC082_grid_2")
                    {
                        var query = new DocumentoDetalleQueryLT(producto, lote, faixa, empresa, nuDocumento, tpDocumento);

                        uow.HandleQuery(query);
                        query.ApplyFilter(this._filterInterpreter, context.Filters);

                        return new GridStats
                        {
                            Count = query.GetCount()
                        };
                    }
                    else
                    {
                        DocumentoDetEgresoQuery query = this.GetQueryGrid3(producto, lote, nuDocumento, tpDocumento, faixa, empresa, uow);

                        uow.HandleQuery(query);
                        query.ApplyFilter(this._filterInterpreter, context.Filters);

                        return new GridStats
                        {
                            Count = query.GetCount()
                        };
                    }
                }
            }

            return null;
        }

    }
}