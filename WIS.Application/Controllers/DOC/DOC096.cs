using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC096 : AppController
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

        public DOC096(ISessionAccessor session,
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
                var nuDocumento = context.GetParameter("nuDocumento");
                var tpDocumento = context.GetParameter("tpDocumento");

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    IDocumento documento = uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);

                    if (documento != null)
                    {
                        context.AddParameter("NU_DOCUMENTO", documento.Numero);
                        context.AddParameter("DS_DOCUMENTO", documento.Descripcion);
                        context.AddParameter("CD_EMPRESA", Convert.ToString(documento.Empresa));
                        context.AddParameter("NM_EMPRESA", uow.EmpresaRepository.GetNombre(documento.Empresa ?? -1));
                        context.AddParameter("ID_ESTADO", documento.Estado);
                        context.AddParameter("NU_AGENDA", Convert.ToString(documento.Agenda));
                        context.AddParameter("NU_FACTURA", Convert.ToString(documento.Factura));
                        context.AddParameter("INFO_DUA", $"{documento.DocumentoAduana.Numero} - {documento.DocumentoAduana.Tipo}");
                        context.AddParameter("INFO_DTI", $"{documento.NumeroDTI}");
                        context.AddParameter("INFO_REFERENCIA_EXTERNA", $"{documento.DocumentoReferenciaExterna.Tipo} - {documento.DocumentoReferenciaExterna.Numero}");

                        context.AddParameter("CD_MONEDA", documento.Moneda + " - " + uow.MonedaRepository.GetDescripcion(documento.Moneda));
                        context.AddParameter("VL_ARBITRAJE", Convert.ToString(documento.ValorArbitraje));
                        context.AddParameter("QT_BULTO", Convert.ToString(documento.Peso));
                        context.AddParameter("VL_SEGURO", Convert.ToString(documento.ValorSeguro));
                        context.AddParameter("VL_FLETE", Convert.ToString(documento.ValorFlete));
                        context.AddParameter("VL_OTROS_GASTOS", Convert.ToString(documento.ValorOtrosGastos));
                        context.AddParameter("CD_UNIDAD_MEDIDA_BULTO", documento.UnidadMedida);
                        context.AddParameter("NU_CONOCIMIENTO", documento.Conocimiento);
                        context.AddParameter("NU_IMPORT", documento.NumeroImportacion);
                        context.AddParameter("NU_EXPORT", documento.NumeroExportacion);

                        context.AddParameter("DT_PROGRAMADO", documento.FechaProgramado.ToIsoString());
                        context.AddParameter("DT_ADDROW", documento.FechaAlta.ToIsoString());
                        context.AddParameter("DT_ENVIADO", documento.FechaEnviado.ToIsoString());
                        context.AddParameter("DT_UPDROW", documento.FechaModificacion.ToIsoString());

                        context.AddParameter("TP_ALMACENAJE", documento.TipoAlmacenajeYSeguro + " - " + uow.TipoAlmacenajeSeguroRepository.GetDescripcion(documento.TipoAlmacenajeYSeguro ?? -1));
                        context.AddParameter("NU_PREDIO", documento.Predio);
                        context.AddParameter("CD_VIA", uow.ViaRepository.GetDescripcion(documento.Via));

                        context.AddParameter("CD_FUNCIONARIO", Convert.ToString(documento.Usuario));
                        context.AddParameter("CD_DESPACHANTE", Convert.ToString(documento.Despachante));

                        var agrupador = uow.DocumentoRepository.GetAgrupador(documento.NumeroAgrupador, documento.TipoAgrupador);

                        if (agrupador != null)
                        {
                            context.AddParameter("NU_DOC_TRANSPORTE", agrupador.Transportadora == null ? "" : agrupador.Transportadora.Descripcion);
                            context.AddParameter("CD_CAMION", agrupador.Placa);
                        }
                    }
                    else
                    {
                        #region parametros vacios
                        context.AddParameter("NU_DOCUMENTO", "");
                        context.AddParameter("DS_DOCUMENTO", "");
                        context.AddParameter("CD_EMPRESA", "");
                        context.AddParameter("NM_EMPRESA", "");
                        context.AddParameter("ID_ESTADO", "");
                        context.AddParameter("NU_AGENDA", "");
                        context.AddParameter("NU_FACTURA", "");
                        context.AddParameter("INFO_DUA", "");

                        context.AddParameter("VL_ARBITRAJE", "");
                        context.AddParameter("CD_MONEDA", "");
                        context.AddParameter("VL_SEGURO", "");
                        context.AddParameter("QT_BULTO", "");
                        context.AddParameter("VL_FLETE", "");
                        context.AddParameter("VL_OTROS_GASTOS", "");
                        context.AddParameter("CD_UNIDAD_MEDIDA_BULTO", "");
                        context.AddParameter("NU_CONOCIMIENTO", "");
                        context.AddParameter("NU_IMPORT", "");
                        context.AddParameter("NU_EXPORT", "");

                        context.AddParameter("DT_PROGRAMADO", "");
                        context.AddParameter("DT_ENVIADO", "");
                        context.AddParameter("DT_ADDROW", "");
                        context.AddParameter("DT_UPDROW", "");

                        context.AddParameter("TP_ALMACENAJE", "");
                        context.AddParameter("NU_PREDIO", "");
                        context.AddParameter("CD_VIA", "");

                        context.AddParameter("CD_FUNCIONARIO", "");
                        context.AddParameter("CD_DESPACHANTE", "");

                        context.AddParameter("NU_DOC_TRANSPORTE", "");
                        context.AddParameter("CD_CAMION", "");
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
            switch (grid.Id)
            {
                case "DOC096_grid_I":
                case "DOC096_grid_E":

                    grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                        new GridButton("btnDetalles", "Detalles", "fas fa-arrow-right")
                    }));
                    break;
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var nuDocumento = context.GetParameter("nuDocumento");
                    var tpDocumento = context.GetParameter("tpDocumento");

                    if (!string.IsNullOrEmpty(nuDocumento) && !string.IsNullOrEmpty(tpDocumento))
                    {
                        switch (grid.Id)
                        {
                            case "DOC096_grid_I":
                                return GridIngresoFetchRow(grid, context, uow, nuDocumento, tpDocumento);

                            case "DOC096_grid_E":
                                return GridEgresoFetchRows(grid, context, uow, nuDocumento, tpDocumento);

                            case "DOC096_grid_L":
                                return GridLogFetchRow(grid, context, uow, nuDocumento, tpDocumento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw new ValidationFailedException("DOC096_Sec0_Error_Error01", new string[] { grid.Id });
            }
            return grid;
        }

        public virtual Grid GridEgresoFetchRows(Grid grid, GridFetchContext context, IUnitOfWork uow, string nuDocumento, string tpDocumento)
        {
            var query = new DetallesEgresoQuery(nuDocumento, tpDocumento);

            uow.HandleQuery(query);

            var defaultSortEgreso = new SortCommand("DT_ADDROW", SortDirection.Descending);

            var GridKeys = new List<string>
            {
                "NU_SECUENCIA", "TP_DOCUMENTO_EGRESO", "NU_DOCUMENTO_EGRESO"
            };

            if (query.Any())
            {
                context.Parameters.Add(new ComponentParameter()
                {
                    Id = "mostrarGridEgreso",
                    Value = "true"
                });

                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSortEgreso, GridKeys);
            }

            return grid;
        }
        public virtual Grid GridIngresoFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, string nuDocumento, string tpDocumento)
        {
            var queryIngreso = new DetallesIngresoQuery(nuDocumento, tpDocumento);

            uow.HandleQuery(queryIngreso);

            var defaultSortIngreso = new SortCommand("DT_ADDROW", SortDirection.Descending);

            List<string> GridKeysIngreso = new List<string>
            {
                "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA", "TP_DOCUMENTO", "NU_DOCUMENTO"
            };

            if (queryIngreso.Any())
            {
                context.Parameters.Add(new ComponentParameter()
                {
                    Id = "mostrarGridIngreso",
                    Value = "true"
                });

                grid.Rows = this._gridService.GetRows(queryIngreso, grid.Columns, context, defaultSortIngreso, GridKeysIngreso);
            }

            return grid;
        }
        public virtual Grid GridLogFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, string nuDocumento, string tpDocumento)
        {
            var queryLog = new DocumentosQuery(nuDocumento, tpDocumento);

            uow.HandleQuery(queryLog);

            var defaultSortIngreso = new SortCommand("NU_LOG_SECUENCIA", SortDirection.Ascending);

            List<string> GridKeysIngreso = new List<string>
            {
                "TP_DOCUMENTO", "NU_DOCUMENTO","NU_LOG_SECUENCIA"
            };

            if (queryLog.Any())
            {
                context.Parameters.Add(new ComponentParameter()
                {
                    Id = "mostrarGridLog",
                    Value = "true"
                });

                grid.Rows = this._gridService.GetRows(queryLog, grid.Columns, context, defaultSortIngreso, GridKeysIngreso);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            var rows = new List<ComponentParameter>();

            foreach (var r in data.Row.Cells)
            {
                rows.Add(new ComponentParameter
                {
                    Id = r.Column.Id,
                    Value = r.Value
                });
            }
            _session.SetValue("DOC082_ROWS", JsonConvert.SerializeObject(rows));

            data.Redirect("/documento/DOC082", new List<ComponentParameter>() { });

            return data;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (grid.Id == "DOC096_grid_I")
                {
                    var dbQuery = new DetallesIngresoQuery(nuDocumento, tpDocumento);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                    context.FileName = "LineaDocIngreso_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }
                else if (grid.Id == "DOC096_grid_E")
                {
                    var dbQuery = new DetallesEgresoQuery(nuDocumento, tpDocumento);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                    context.FileName = "LineaDocEgreso_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }
                else
                {
                    var dbQuery = new DocumentosQuery(nuDocumento, tpDocumento);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_LOG_SECUENCIA", SortDirection.Descending);

                    context.FileName = "LineaDocLog" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(nuDocumento) && !string.IsNullOrEmpty(tpDocumento))
            {
                switch (grid.Id)
                {
                    case "DOC096_grid_I":
                        return GridIngresoFetchStats(grid, context, uow, nuDocumento, tpDocumento);

                    case "DOC096_grid_E":
                        return GridEgresoFetchStats(grid, context, uow, nuDocumento, tpDocumento);

                    case "DOC096_grid_L":
                        return GridLogFetchStats(grid, context, uow, nuDocumento, tpDocumento);
                }
            }

            return null;
        }
        public virtual GridStats GridIngresoFetchStats(Grid grid, GridFetchStatsContext context, IUnitOfWork uow, string nuDocumento, string tpDocumento)
        {
            var queryIngreso = new DetallesIngresoQuery(nuDocumento, tpDocumento);

            uow.HandleQuery(queryIngreso);
            queryIngreso.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryIngreso.GetCount()
            };
        }
        public virtual GridStats GridEgresoFetchStats(Grid grid, GridFetchStatsContext context, IUnitOfWork uow, string nuDocumento, string tpDocumento)
        {
            var query = new DetallesEgresoQuery(nuDocumento, tpDocumento);

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }
        public virtual GridStats GridLogFetchStats(Grid grid, GridFetchStatsContext context, IUnitOfWork uow, string nuDocumento, string tpDocumento)
        {
            var queryLog = new DocumentosQuery(nuDocumento, tpDocumento);

            uow.HandleQuery(queryLog);
            queryLog.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryLog.GetCount()
            };
        }
    }
}