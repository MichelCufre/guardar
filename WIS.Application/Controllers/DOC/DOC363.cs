using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Stock;
using WIS.Domain.ManejoStock;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class DOC363 : AppController
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

        protected List<string> GridKeysAjuste { get; }
        protected List<string> GridKeysDocumento { get; }

        public DOC363(ISessionAccessor session,
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

            this.GridKeysAjuste = new List<string>
            {
                "NU_AJUSTE_STOCK"
            };

            this.GridKeysDocumento = new List<string>
            {
                "NU_DOCUMENTO","TP_DOCUMENTO","CD_EMPRESA"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "DOC363_grid_documentos")
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                    new GridButton("btnSeleccionar", "Seleccionar", "fas fa-hand-pointer")
                }));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
                var empresa = int.Parse(_session.GetValue<string>("DOC361_CD_EMPRESA"));

                switch (grid.Id)
                {
                    case "DOC363_grid_ajustes":
                        return GridAjusteFetchRow(grid, context, uow, nrosAjuste);
                    case "DOC363_grid_documentos":
                        return GridDocumentoFetchRow(grid, context, uow, nrosAjuste, empresa);
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                switch (grid.Id)
                {
                    case "DOC363_grid_ajustes":
                        return ExporExcelAjustes(uow, grid, context);

                    case "DOC363_grid_documentos":
                        return ExporExcelDocumentos(uow, grid, context);
                    default:
                        return null;
                }

            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
            var empresa = int.Parse(_session.GetValue<string>("DOC361_CD_EMPRESA"));

            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "DOC363_grid_ajustes":
                    return GridAjusteFetchStats(grid, context, uow, nrosAjuste);
                case "DOC363_grid_documentos":
                    return GridDocumentoFetchStats(grid, context, uow, nrosAjuste, empresa);
            }

            return null;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnSeleccionar")
            {
                //Obtener documento
                var nuDocumento = context.Row.GetCell("NU_DOCUMENTO").Value;
                var tpDocumento = context.Row.GetCell("TP_DOCUMENTO").Value;

                var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
                var empresa = int.Parse(_session.GetValue<string>("DOC361_CD_EMPRESA"));

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    //Obtener filtro documento
                    var queryLineas = new LineasIngresoPorDocumentoDOC363Query(nuDocumento, tpDocumento);

                    uow.HandleQuery(queryLineas);

                    var filtroDoc = queryLineas.GetResult().Select(l => l.VL_FILTRO).ToList();

                    //Obtener ajustes
                    var ajusteQuery = new DocumentoAjusteStockFiltroDOC363(filtroDoc, nrosAjuste);

                    uow.HandleQuery(ajusteQuery);

                    var ajustes = ajusteQuery.GetResult().Select(f => f.NU_AJUSTE_STOCK).ToList();

                    context.AddParameter("FILTRO_AJUSTES", JsonConvert.SerializeObject(ajustes));
                    context.AddParameter("FILTRO_DOCUMENTO", nuDocumento);
                    context.AddParameter("FILTRO_TIPO_DOC", tpDocumento);
                }
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            FormField nroDoc = form.GetField("nroDoc");
            FormField tpDocumento = form.GetField("tpDocumento");
            FormField estado = form.GetField("estado");
            FormField empresa = form.GetField("empresa");
            FormField fechaCreacion = form.GetField("fechaCreacion");
            FormField cantLineas = form.GetField("cantLineas");

            if (context.Parameters != null && context.Parameters.Any(p => p.Id == "FILTRO_DOCUMENTO") && context.Parameters.Any(p => p.Id == "FILTRO_TIPO_DOC"))
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    string NU_DOCUMENTO = context.Parameters.FirstOrDefault(p => p.Id == "FILTRO_DOCUMENTO").Value;
                    string TP_DOCUMENTO = context.Parameters.FirstOrDefault(p => p.Id == "FILTRO_TIPO_DOC").Value;

                    var queryDoc = new DocumentoInfoDOC096Query(NU_DOCUMENTO, TP_DOCUMENTO);

                    uow.HandleQuery(queryDoc);

                    var infoDoc = queryDoc.GetResult().FirstOrDefault();

                    if (infoDoc != null)
                    {
                        nroDoc.Value = infoDoc.NU_DOCUMENTO;
                        tpDocumento.Value = infoDoc.TP_DOCUMENTO;
                        estado.Value = infoDoc.ID_ESTADO;
                        empresa.Value = infoDoc.NM_EMPRESA;
                        fechaCreacion.Value = infoDoc.DT_ADDROW.ToIsoString();
                        cantLineas.Value = infoDoc.QT_LINEAS == null ? "0" : infoDoc.QT_LINEAS.ToString();
                    }
                }
            }
            else
            {
                nroDoc.Value = "";
                tpDocumento.Value = "";
                estado.Value = "";
                empresa.Value = "";
                fechaCreacion.Value = "";
                cantLineas.Value = "";
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("DOC363 CrearActaDocumental");
            uow.BeginTransaction();

            try
            {
                if (context.Parameters == null ||
                    string.IsNullOrEmpty(context.GetParameter("FILTRO_DOCUMENTO")) ||
                    string.IsNullOrEmpty(context.GetParameter("FILTRO_TIPO_DOC")) ||
                    string.IsNullOrEmpty(context.GetParameter("AJUSTES_ACTA")))
                {
                    throw new ValidationFailedException("DOC363_Sec0_Error_ErrorParametros");
                }

                CrearActaDocumental(uow, context);

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());

            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.Error(ex, "DOC362_Sec0_Not_ErrorCrearActaStock");
                context.AddErrorNotification("DOC362_Sec0_Not_ErrorCrearActaStock");
            }

            return form;
        }

        #region Metodos Auxiliares

        public virtual Grid GridAjusteFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, List<int> nrosAjuste)
        {
            var queryIngreso = new DocumentoAjustesStockDOC363Query(nrosAjuste);

            context.Filters = new List<FilterCommand>(); //Piso los filtros para solo aplciar los saldos por numero de ajiste ASC
            context.Sorts = new List<SortCommand>(); // IDEM filtros

            uow.HandleQuery(queryIngreso);

            var defaultSortAjuste = new SortCommand("NU_AJUSTE_STOCK", SortDirection.Descending);

            grid.Rows = this._gridService.GetRows(queryIngreso, grid.Columns, context, defaultSortAjuste, this.GridKeysAjuste, enableSkipAndTakeRecords: false);

            this.FiltrarAjustesConsumoActa(grid, context, uow);

            grid.Rows = grid.Rows.Skip(context.RowsToSkip).Take(context.RowsToFetch).ToList();

            return grid;
        }

        public virtual Grid GridDocumentoFetchRow(Grid grid, GridFetchContext context, IUnitOfWork uow, List<int> nrosAjuste, int cdEmpresa)
        {
            var filtroDocumento = this.ObtenerFiltroDocumentos(uow, nrosAjuste, cdEmpresa);

            var queryDocumentos = new DocumentoIngresoAjusteDOC363Query(filtroDocumento);

            uow.HandleQuery(queryDocumentos);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            grid.Rows = this._gridService.GetRows(queryDocumentos, grid.Columns, context, defaultSort, this.GridKeysDocumento);

            return grid;
        }

        public virtual List<string> ObtenerFiltroDocumentos(IUnitOfWork uow, List<int> nrosAjustes, int cdmpresa)
        {
            List<string> filtro = new List<string>();

            //Obtener ajustes
            var ajusteQuery = new DocumentoAjustesStockDOC363Query(nrosAjustes);

            uow.HandleQuery(ajusteQuery);

            var ajustes = ajusteQuery.GetResult().Select(a => a.VL_FILTRO).ToList();

            var queryLineas = new LineasIngresoSaldoDOC363Query(ajustes, cdmpresa);

            uow.HandleQuery(queryLineas);

            var lineasIngreso = queryLineas.GetResult();

            //Procesar filtro documento

            filtro = lineasIngreso.Select(l => new { l.NU_DOCUMENTO, l.TP_DOCUMENTO }).Distinct().Select(l => l.NU_DOCUMENTO + l.TP_DOCUMENTO).ToList();

            return filtro;
        }

        public virtual void FiltrarAjustesConsumoActa(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            if (!string.IsNullOrEmpty(context.GetParameter("FILTRO_AJUSTES")) && !string.IsNullOrEmpty(context.GetParameter("FILTRO_DOCUMENTO")) && !string.IsNullOrEmpty(context.GetParameter("FILTRO_TIPO_DOC")))
            {
                var ajustesAfectados = JsonConvert.DeserializeObject<List<int>>(context.GetParameter("FILTRO_AJUSTES"));
                var queryLineasIngreso = new LineasIngresoPorDocumentoDOC363Query(context.GetParameter("FILTRO_DOCUMENTO"), context.GetParameter("FILTRO_TIPO_DOC"));
                var culture = this._identity.GetFormatProvider();

                uow.HandleQuery(queryLineasIngreso);

                var infoLineas = queryLineasIngreso
                    .GetResult()
                    .Select(l => new { l.CD_PRODUTO, l.NU_IDENTIFICADOR, l.SALDO })
                    .GroupBy(l => new { l.CD_PRODUTO, l.NU_IDENTIFICADOR })
                    .Select(l => new SaldoAjusteNegativoActa() { CD_PRODUCTO = l.Key.CD_PRODUTO, NU_IDENTIFICADOR = l.Key.NU_IDENTIFICADOR, SALDO = l.Sum(s => s.SALDO ?? 0) })
                    .ToList();

                List<AjusteNegativoActa> ajustesActa = new List<AjusteNegativoActa>();

                bool afectado = false;
                grid.Rows.ForEach(row =>
                {
                    afectado = false;

                    var nuAjuste = int.Parse(row.GetCell("NU_AJUSTE_STOCK").Value);
                    var qtAjuste = decimal.Parse(row.GetCell("QT_MOVIMIENTO").Value, culture);
                    var producto = row.GetCell("CD_PRODUTO").Value;
                    var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

                    //Procesar saldos
                    var saldoDisponible = infoLineas.FirstOrDefault(l => l.CD_PRODUCTO == producto && l.NU_IDENTIFICADOR == identificador)?.SALDO ?? 0;

                    if (saldoDisponible > 0)
                    {
                        afectado = true;
                        decimal saldoAjsutado;

                        //Ver si saldo cubre todo el ajuste, o lo cubre de manera parcial
                        if (Math.Abs(qtAjuste) >= saldoDisponible)
                        {
                            saldoAjsutado = saldoDisponible;
                            infoLineas.FirstOrDefault(l => l.CD_PRODUCTO == producto && l.NU_IDENTIFICADOR == identificador).SALDO = 0;
                        }
                        else
                        {
                            saldoAjsutado = Math.Abs(qtAjuste);
                            infoLineas.FirstOrDefault(l => l.CD_PRODUCTO == producto && l.NU_IDENTIFICADOR == identificador).SALDO = saldoDisponible + qtAjuste;
                        }

                        row.GetCell("QT_ACTA").Value = saldoAjsutado.ToString();

                        ajustesActa.Add(new AjusteNegativoActa()
                        {
                            CD_PRODUCTO = producto,
                            NU_AJUSTE_STOCK = nuAjuste.ToString(),
                            NU_IDENTIFICADOR = identificador,
                            QT_MOVIMIENTO = qtAjuste,
                            QT_DESAFECTAR_ACTA = -saldoAjsutado
                        });
                    }

                    if (ajustesAfectados.Contains(nuAjuste) && afectado)
                    {
                        row.Cells.ForEach(cell => { cell.CssClass = "affected"; });
                    }
                    else
                    {
                        row.Cells.ForEach(cell => { cell.CssClass = "noAffected"; });
                    }
                });

                context.AddParameter("AJUSTES_ACTA", JsonConvert.SerializeObject(ajustesActa));
            }
        }

        public virtual GridStats GridAjusteFetchStats(Grid grid, GridFetchStatsContext context, IUnitOfWork uow, List<int> nrosAjuste)
        {
            var queryIngreso = new DocumentoAjustesStockDOC363Query(nrosAjuste);

            uow.HandleQuery(queryIngreso);
            queryIngreso.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryIngreso.GetCount()
            };
        }

        public virtual GridStats GridDocumentoFetchStats(Grid grid, GridFetchStatsContext context, IUnitOfWork uow, List<int> nrosAjuste, int cdEmpresa)
        {
            var filtroDocumento = this.ObtenerFiltroDocumentos(uow, nrosAjuste, cdEmpresa);

            var queryDocumentos = new DocumentoIngresoAjusteDOC363Query(filtroDocumento);

            uow.HandleQuery(queryDocumentos);
            queryDocumentos.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = queryDocumentos.GetCount()
            };
        }

        public virtual byte[] ExporExcelAjustes(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));

            var dbQuery = new DocumentoAjustesStockDOC363Query(nrosAjuste);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_AJUSTE_STOCK", SortDirection.Descending);

            context.FileName = "DocumentoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public virtual byte[] ExporExcelDocumentos(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var nrosAjuste = JsonConvert.DeserializeObject<List<int>>(_session.GetValue<string>("DOC361_AJUSTES_SELECCIONADOS"));
            var empresa = int.Parse(_session.GetValue<string>("DOC361_CD_EMPRESA"));

            var filtroDocumento = this.ObtenerFiltroDocumentos(uow, nrosAjuste, empresa);

            var dbQuery = new DocumentoIngresoAjusteDOC363Query(filtroDocumento);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);
            context.FileName = "DocumentoAjusteStockDoc_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public virtual void CrearActaDocumental(IUnitOfWork uow, FormSubmitContext context)
        {
            var ajustesSeleccionados = JsonConvert.DeserializeObject<List<AjusteNegativoActa>>(context.GetParameter("AJUSTES_ACTA"));
            var nuDocumento = context.GetParameter("FILTRO_DOCUMENTO");
            var tpDocumento = context.GetParameter("FILTRO_TIPO_DOC");

            var nuTransaccion = uow.GetTransactionNumber();

            //Obtener documento de ingreso con lineas
            var documentoIngreso = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);

            var infoLineasActa = new List<InformacionActaStock>();
            var nivelacion = new NivelacionAjusteDocumental();

            //Obtener ajustes de stock nivelados por el acta
            var ajustes = uow.AjusteRepository.GetAjustesDocumento(ajustesSeleccionados.Select(a => int.Parse(a.NU_AJUSTE_STOCK)).ToList());
            var lineasIngresoAfectadas = new List<DocumentoLinea>();

            //Obtener numero de operacion para agrupar historicos de ajuste
            var numeroOperacion = int.Parse(uow.AjusteRepository.GetNumeroOperacionAjuste());

            decimal qtActa = 0;
            AjusteNegativoActa seleccion;

            if (ajustes != null && ajustes.Count > 0)
            {
                foreach (var ajuste in ajustes)
                {
                    seleccion = ajustesSeleccionados.FirstOrDefault(a => a.NU_AJUSTE_STOCK == ajuste.NumeroAjuste.ToString());
                    qtActa = seleccion.QT_DESAFECTAR_ACTA;

                    //Generar linea de acta
                    var lineaActaExistente = infoLineasActa.FirstOrDefault(l => l.Producto == ajuste.Producto && l.NumeroIdentificador == ajuste.Identificador);

                    InformacionActaStock lineaActaAfectada = null;

                    if (lineaActaExistente != null)
                    {
                        lineaActaAfectada = infoLineasActa.FirstOrDefault(l => l.Producto == ajuste.Producto && l.NumeroIdentificador == ajuste.Identificador);
                        lineaActaAfectada.CantidadActa = lineaActaExistente.CantidadActa + qtActa;
                    }
                    else
                    {
                        lineaActaAfectada = new InformacionActaStock()
                        {
                            NumeroIdentificador = ajuste.Identificador,
                            Producto = ajuste.Producto,
                            CantidadActa = qtActa,
                            CIF = 0,
                            FOB = 0,
                            Empresa = (int)ajuste.CodigoEmpresa,
                            Faixa = (decimal)ajuste.Faixa
                        };
                        infoLineasActa.Add(lineaActaAfectada);
                    }

                    //Desafectar linea de ingreso
                    var lineaIngreso = documentoIngreso.Lineas.FirstOrDefault(l => l.Producto == ajuste.Producto && l.Identificador == ajuste.Identificador);

                    if (lineasIngresoAfectadas.Contains(lineaIngreso))
                    {
                        var lineaExistente = lineasIngresoAfectadas.FirstOrDefault(l => l.Producto == ajuste.Producto && l.Identificador == ajuste.Identificador);

                        lineasIngresoAfectadas.FirstOrDefault(l => l.Producto == ajuste.Producto && l.Identificador == ajuste.Identificador).CantidadDesafectada = lineaExistente.CantidadDesafectada + Math.Abs(qtActa);
                    }
                    else
                    {
                        lineaIngreso.CantidadDesafectada = (lineaIngreso.CantidadDesafectada ?? 0) + Math.Abs(qtActa);
                        lineasIngresoAfectadas.Add(lineaIngreso);
                    }

                    //Actualizo CIF FOB Tributo
                    lineaActaAfectada.CIF = ((lineaIngreso.CIF ?? 0) / lineaIngreso.CantidadIngresada) * Math.Abs((lineaActaAfectada.CantidadActa ?? 0));
                    lineaActaAfectada.FOB = ((lineaIngreso.ValorMercaderia ?? 0) / lineaIngreso.CantidadIngresada) * Math.Abs((lineaActaAfectada.CantidadActa ?? 0));
                    lineaActaAfectada.Tributo = ((lineaIngreso.ValorTributo ?? 0) / lineaIngreso.CantidadIngresada) * Math.Abs((lineaActaAfectada.CantidadActa ?? 0));

                    //Gnerar historico de ajuste de stock
                    DocumentoAjusteStockHistorico historicoNegativo = new DocumentoAjusteStockHistorico(ajuste)
                    {
                        CantidadMovimiento = (decimal)qtActa,
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.ACTA_STOCK
                    };

                    //Modificar o Eliminar ajuste original dependiendo del saldo a desafectar
                    if (seleccion.QT_MOVIMIENTO < seleccion.QT_DESAFECTAR_ACTA)
                    {
                        ajuste.CantidadMovimiento = ajuste.CantidadMovimiento + Math.Abs(qtActa);
                        nivelacion.ajustesModificar.Add(ajuste);
                    }
                    else
                    {
                        nivelacion.ajustesEliminar.Add(ajuste);
                    }

                    nivelacion.ajustesHistoricosAgregar.Add(historicoNegativo);
                }

                //Crear documento Acta de stock
                var ajusteDocumental = new Domain.Documento.Integracion.Stock.AjusteStockDocumental(this._factoryService, this._parameterService);
                IDocumento actaStock = ajusteDocumental.CrearActaStock(uow, documentoIngreso, infoLineasActa, this._identity.UserId);

                foreach (DocumentoAjusteStockHistorico historico in nivelacion.ajustesHistoricosAgregar)
                {
                    historico.NumeroDocumento = actaStock.Numero;
                    historico.TipoDocumento = actaStock.Tipo;
                    historico.FechaActualizacion = DateTime.Now;
                }

                //Impactar nivelacion de ajustes de stock
                Domain.ManejoStock.AjusteStockDocumental.AjusteStockDocumental ajusteStockDocumental = new Domain.ManejoStock.AjusteStockDocumental.AjusteStockDocumental();
                ajusteStockDocumental.ImpactarNivelacion(uow, nivelacion);

                //Impactar modificacion en lineas de ingreso
                foreach (DocumentoLinea lineaModificada in lineasIngresoAfectadas)
                {
                    uow.DocumentoRepository.UpdateDetail(documentoIngreso, lineaModificada, nuTransaccion);
                    uow.SaveChanges();
                }

                uow.SaveChanges();

                context.AddSuccessNotification("DOC363_msg_Succes_ActaStockGenerada", new List<string> { actaStock.Numero });
                context.Redirect("/documento/DOC360", new List<ComponentParameter>() { });
            }
            else
                throw new ValidationFailedException("DOC363_msg_Error_SinAjustes");

        }

        #endregion
    }
}
