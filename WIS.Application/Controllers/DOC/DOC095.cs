using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.DataModel.Queries.DocumentoVistaQuery;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Exceptions;
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
    public class DOC095 : AppController
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

        public DOC095(ISessionAccessor session,
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
               "TP_DOCUMENTO", "NU_DOCUMENTO"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridItemHeader("DOC080_Sec0_lbl_Acciones"),
                new GridButton("btnDetalles", "General_Sec0_btn_Detalles", "fas fa-list"),
                new GridButton("btnDocumentos", "General_Sec0_btn_Documentos", "fas fa-paperclip"),
                new GridButton("btnSimularCuentaCorriente", "DOC080_Sec0_btn_CuentaCorriente", "fas fa-clipboard-list"),
                new GridItemDivider(),
                new GridButton("btnEditEgreso", "General_Sec0_btn_Editar", "fas fa-pencil-alt"),
            }));

            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;
            context.IsRemoveEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                DocumentoMapper mapper = new DocumentoMapper();
                var query = new DocumentoVistaQuery();

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);
                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

                var docsEditables = uow.DocumentoTipoRepository.GetDocumentosHabilitadosParaEdicion();
                var docsSimularCC = uow.DocumentoTipoRepository.GetDocumentosHabilitadosParaSimularCC();

                foreach (var r in grid.Rows)
                {
                    var tipoDoc = r.GetCell("TP_DOCUMENTO").Value;
                    var estado = r.GetCell("ID_ESTADO").Value;

                    if (!docsEditables.ContainsKey(tipoDoc) || !docsEditables[tipoDoc].Contains(estado))
                        r.DisabledButtons.Add("btnEditEgreso");

                    if (!docsSimularCC.ContainsKey(tipoDoc) || !docsSimularCC[tipoDoc].Contains(estado))
                        r.DisabledButtons.Add("btnSimularCuentaCorriente");

                    if (uow.DocumentoTipoRepository.GetTipoDocumento(tipoDoc).TipoOperacion == TipoDocumentoOperacion.INGRESO)
                    {
                        r.SetEditableCells(new List<string> { "DT_PROGRAMADO" });
                    }
                }
            }
            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentoVistaQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = "Documentos_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnDetalles":
                    context.Redirect("/documento/DOC096", new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "nuDocumento", Value = context.Row.GetCell("NU_DOCUMENTO").Value },
                        new ComponentParameter(){ Id = "tpDocumento", Value = context.Row.GetCell("TP_DOCUMENTO").Value },
                    });
                    break;
                case "btnDocumentos":
                    context.Parameters.RemoveAll(p => p.Id == "codigoEntidad");

                    var tpDocumento = context.Row.GetCell("TP_DOCUMENTO").Value;
                    var nuDocumento = context.Row.GetCell("NU_DOCUMENTO").Value;

                    context.AddParameter("codigoEntidad", $"{tpDocumento}${nuDocumento}");
                    break;
                case "btnEditEgreso":
                    context.Redirect("/documento/DOC340", new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "nuDocumento", Value = context.Row.GetCell("NU_DOCUMENTO").Value },
                        new ComponentParameter(){ Id = "tpDocumento", Value = context.Row.GetCell("TP_DOCUMENTO").Value },
                    });
                    break;
                case "btnSimularCuentaCorriente":
                    using (var uow = this._uowFactory.GetUnitOfWork())
                    {
                        uow.BeginTransaction();

                        context.Redirect("/documento/DOC500", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuDocumento", Value = context.Row.GetCell("NU_DOCUMENTO").Value },
                            new ComponentParameter(){ Id = "tpDocumento", Value = context.Row.GetCell("TP_DOCUMENTO").Value },
                            new ComponentParameter(){ Id = "cdEmpresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                        });

                        string nu_egreso = "";
                        IDocumento doc = uow.DocumentoRepository.GetDocumento(context.Row.GetCell("NU_DOCUMENTO").Value, context.Row.GetCell("TP_DOCUMENTO").Value);

                        foreach (var det in doc.Lineas)
                        {
                            CuentaCorrienteCambioDoc new_insum = new CuentaCorrienteCambioDoc();

                            new_insum.NU_DOCUMENTO_EGRESO_PRDC = context.Row.GetCell("NU_DOCUMENTO").Value;
                            new_insum.TP_DOCUMENTO_EGRESO_PRDC = "EP";
                            new_insum.TP_DOCUMENTO_INGRESO = "IP";
                            new_insum.NU_DOCUMENTO_INGRESO = context.Row.GetCell("NU_DOCUMENTO").Value;
                            new_insum.TP_DOCUMENTO_INGRESO_ORIGINAL = "IP";
                            new_insum.NU_DOCUMENTO_INGRESO_ORIGINAL = context.Row.GetCell("NU_DOCUMENTO").Value;
                            new_insum.QT_DECLARADA_ORIGINAL = det.CantidadIngresada;
                            new_insum.CD_EMPRESA = det.Empresa;
                            new_insum.CD_PRODUTO = det.Producto;
                            new_insum.CD_FAIXA = det.Faixa;
                            new_insum.NU_IDENTIFICADOR = det.Identificador;
                            new_insum.QT_MOVIMIENTO = det.GetCantidadCambio();
                            new_insum.CD_PRODUTO_PRODUCIDO = det.Producto;
                            new_insum.NU_NIVEL = 1;

                            CuentaCorrienteCambioDoc new_insu = uow.DocumentoRepository.GetCuentaCorrienteSinNumEgreso(new_insum);

                            if (new_insu != null)
                            {
                                new_insu.QT_MOVIMIENTO = det.GetCantidadCambio();
                                uow.DocumentoRepository.UpdateCuentaCorrienteDocumento(new_insu);
                                nu_egreso = new_insu.NU_DOCUMENTO_EGRESO.ToString();
                                uow.DocumentoRepository.DeleteCuentaCorrienteDocumento(new_insu.NU_DOCUMENTO_EGRESO);
                            }
                            else
                            {
                                new_insum.NU_DOCUMENTO_EGRESO = uow.DocumentoRepository.GetNumeroSecuenciaCambioCuenta().ToString();
                                nu_egreso = new_insum.NU_DOCUMENTO_EGRESO.ToString();
                                uow.DocumentoRepository.AddCuentaCorrienteInsumo(new_insum);
                            }
                        }

                        uow.SaveChanges();

                        int x = 1;
                        while (true) // Itero en los detalles de documento para buscar lineas que desafecten de IP
                        {
                            List<CuentaCorrienteCambioDoc> lista = uow.DocumentoRepository.GetCuentaDocumentoIPNivel(nu_egreso, x);

                            if (lista.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                foreach (var curDetEgreso in lista)
                                {
                                    int v_cd_empresa_EG = curDetEgreso.CD_EMPRESA;
                                    string v_cd_producto_EG = curDetEgreso.CD_PRODUTO;
                                    decimal v_cd_faixa_EG = curDetEgreso.CD_FAIXA;
                                    string v_nu_identificador_EG = curDetEgreso.NU_IDENTIFICADOR;
                                    decimal v_qt_egresado = curDetEgreso.QT_MOVIMIENTO ?? 0;
                                    curDocProduccion curDocProduccion = uow.DocumentoRepository.GetPrdcIngresoSaldo(curDetEgreso.NU_DOCUMENTO_INGRESO, curDetEgreso.TP_DOCUMENTO_INGRESO);
                                    string v_nu_prdc_ingreso = curDocProduccion.NU_PRDC_INGRESO;
                                    string v_nu_documento_EP = curDocProduccion.NU_DOCUMENTO_EGR;
                                    string v_tp_documento_EP = curDocProduccion.TP_DOCUMENTO_EGR;
                                    decimal v_qt_ingresado_producido = curDocProduccion.QT_INGRESADO ?? 0;
                                    List<DocumentoLineaEgreso> ListacurPRDCEntrada = uow.DocumentoRepository.GetEgresoDocumento(v_tp_documento_EP, curDetEgreso.NU_DOCUMENTO_INGRESO);

                                    foreach (var curPRDCEntrada in ListacurPRDCEntrada)
                                    {
                                        decimal v_qt_insumo = (curDetEgreso.QT_MOVIMIENTO ?? 0) * (curPRDCEntrada.CantidadDesafectada ?? 0) / (curDetEgreso.QT_DECLARADA_ORIGINAL ?? 1);
                                        int v_cd_empresa_EP = curPRDCEntrada.Empresa;
                                        string v_cd_producto_EP = curPRDCEntrada.Producto;
                                        decimal v_cd_faixa_EP = curPRDCEntrada.Faixa;
                                        List<PrdcSaldo> listssaldo = uow.DocumentoRepository.GetPrdcSaldo(v_nu_documento_EP, v_tp_documento_EP, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP);

                                        foreach (var curDetEP in listssaldo)
                                        {
                                            decimal v_qt_movimiento;
                                            if (v_qt_insumo > 0)
                                            {
                                                if (curDetEP.QT_SALDO <= v_qt_insumo)
                                                {
                                                    v_qt_movimiento = curDetEP.QT_SALDO ?? 0;
                                                }
                                                else
                                                {
                                                    v_qt_movimiento = v_qt_insumo;
                                                }
                                                v_qt_insumo = v_qt_insumo - v_qt_movimiento;

                                                string v_tp_documento_ingreso_orig = curDetEP.TP_DOCUMENTO_INGRESO;
                                                string v_nu_documento_ingreso_orig = curDetEP.NU_DOCUMENTO_INGRESO;
                                                string v_nu_identificador_EP = curDetEP.NU_IDENTIFICADOR;
                                                decimal v_qt_ingresada_doc_Origen = uow.DocumentoRepository.GetQtIngresadaDocOrigen(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP, v_nu_identificador_EP);
                                                decimal v_qt_ingresada_doc_Origen_acta = 0;

                                                if (curDetEP.TP_DOCUMENTO_INGRESO == "A")
                                                {
                                                    uow.DocumentoRepository.GetDocumentoActa(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO, out v_tp_documento_ingreso_orig, out v_nu_documento_ingreso_orig);
                                                    v_qt_ingresada_doc_Origen_acta = uow.DocumentoRepository.GetCantidadActa(v_nu_documento_ingreso_orig, v_tp_documento_ingreso_orig, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP, v_nu_identificador_EP);
                                                }
                                                else
                                                {
                                                    v_qt_ingresada_doc_Origen_acta = uow.DocumentoRepository.GetDocOriQTIngreso(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO, v_cd_empresa_EP, v_cd_producto_EP, v_cd_faixa_EP, v_nu_identificador_EP);
                                                }

                                                if (curDetEP.TP_DOCUMENTO_INGRESO == "IP")
                                                {
                                                    v_qt_ingresada_doc_Origen = uow.DocumentoRepository.GetQtIngresadaDocOrigen(curDetEP.NU_DOCUMENTO_INGRESO, curDetEP.TP_DOCUMENTO_INGRESO);
                                                }

                                                decimal v_qt_declarada_original = v_qt_ingresada_doc_Origen + (v_qt_ingresada_doc_Origen_acta);

                                                CuentaCorrienteCambioDoc new_insumo = new CuentaCorrienteCambioDoc();
                                                new_insumo.NU_DOCUMENTO_EGRESO = nu_egreso;
                                                new_insumo.NU_DOCUMENTO_EGRESO_PRDC = v_nu_documento_EP;
                                                new_insumo.TP_DOCUMENTO_EGRESO_PRDC = v_tp_documento_EP;
                                                new_insumo.TP_DOCUMENTO_INGRESO = curDetEP.TP_DOCUMENTO_INGRESO;
                                                new_insumo.NU_DOCUMENTO_INGRESO = curDetEP.NU_DOCUMENTO_INGRESO;
                                                new_insumo.TP_DOCUMENTO_INGRESO_ORIGINAL = v_tp_documento_ingreso_orig;
                                                new_insumo.NU_DOCUMENTO_INGRESO_ORIGINAL = v_nu_documento_ingreso_orig;
                                                new_insumo.QT_DECLARADA_ORIGINAL = v_qt_declarada_original;
                                                new_insumo.CD_EMPRESA = v_cd_empresa_EP;
                                                new_insumo.CD_PRODUTO = v_cd_producto_EP;
                                                new_insumo.CD_FAIXA = v_cd_faixa_EP;
                                                new_insumo.NU_IDENTIFICADOR = v_nu_identificador_EP;
                                                new_insumo.QT_MOVIMIENTO = v_qt_movimiento;
                                                new_insumo.CD_PRODUTO_PRODUCIDO = v_cd_producto_EG;
                                                new_insumo.NU_NIVEL = x + 1;

                                                CuentaCorrienteCambioDoc new_ins = uow.DocumentoRepository.GetCuentaCorriente(new_insumo);

                                                if (new_ins != null)
                                                {
                                                    new_ins.QT_MOVIMIENTO = new_ins.QT_MOVIMIENTO + v_qt_movimiento;
                                                    uow.DocumentoRepository.UpdateCuentaCorrienteDocumento(new_ins);
                                                }
                                                else
                                                {
                                                    uow.DocumentoRepository.AddCuentaCorrienteInsumo(new_insumo);

                                                }
                                            }
                                        }
                                    }
                                    x = x + 1;
                                }
                            }
                        }
                        uow.SaveChanges();
                        uow.Commit();
                    }
                    break;
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("DOC081_Sec0_Error_Error06");

                    uow.CreateTransactionNumber(this._identity.Application);

                    var nuTransaccion = uow.GetTransactionNumber();

                    foreach (var row in grid.Rows)
                    {
                        if (!row.IsDeleted && !row.IsNew)
                        {
                            string nuDocumento = row.GetCell("NU_DOCUMENTO").Value;
                            string tpDocumento = row.GetCell("TP_DOCUMENTO").Value;

                            var documento = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);

                            if (DateTime.TryParse(row.GetCell("DT_PROGRAMADO").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime programado))
                                documento.FechaProgramado = programado;

                            uow.DocumentoRepository.UpdateIngreso(documento, nuTransaccion);
                        }
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("DOC081_Sec0_Error_Error07");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new DOC095GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var query = new DocumentoVistaQuery();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }
    }
}