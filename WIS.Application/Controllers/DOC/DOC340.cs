using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Documento;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class DOC340 : AppController
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

        public DOC340(ISessionAccessor session,
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
                "NU_IDENTIFICADOR", "CD_PRODUTO", "CD_EMPRESA", "NU_DOCUMENTO", "TP_DOCUMENTO", "DT_ADDROW", "ID_ESTADO", "CD_FAIXA"
            };
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {

                var documento = uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);

                if (documento != null)
                {
                    string estado = string.Empty;
                    if (documento is IDocumentoEgreso)
                        estado = ((IDocumentoEgreso)documento).Estado.ToString();
                    else if (documento is IDocumentoActa)
                        estado = ((IDocumentoActa)documento).Estado.ToString();
                    else if (documento is IDocumentoIngreso)
                        estado = ((IDocumentoIngreso)documento).Estado.ToString();

                    var empresa = uow.EmpresaRepository.GetEmpresa(documento.Empresa != null ? (int)documento.Empresa : -1);

                    form.GetField("NU_DOCUMENTO").Value = documento.Numero;
                    form.GetField("DS_DOCUMENTO").Value = uow.DocumentoTipoRepository.GetTipoDocumento(tpDocumento).DescripcionTipoDocumento;
                    form.GetField("ESTADO").Value = estado;
                    form.GetField("EMPRESA").Value = empresa == null ? "-" : empresa.Nombre;

                }
            }

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var documento = uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);

                if (documento.CanEdit(uow))
                    context.IsEditingEnabled = true;
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var nuDocumento = context.GetParameter("nuDocumento");
                var tpDocumento = context.GetParameter("tpDocumento");

                var query = new DocumentosDetalleQueryDOC340(nuDocumento, tpDocumento);

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, GridKeys);

                var documento = uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);

                if (documento is IDocumentoEgreso)
                {
                    // Es un documento de egreso
                    this.SetEditableColumnsEgreso(uow, grid, ((IDocumentoEgreso)documento));
                    grid.Columns.FirstOrDefault(s => s.Id == "VL_MERCADERIA").Hidden = true;
                    grid.Columns.FirstOrDefault(s => s.Id == "NU_PROCESO").Hidden = false;
                }
                else if (documento is IDocumentoActa)
                {
                    this.SetEditableColumnsActa(uow, grid, ((IDocumentoActa)documento));

                    var acta = (IDocumentoActa)documento;
                    if (acta.Lineas.Count > 0)
                    {
                        //Acta positiva
                        grid.Columns.FirstOrDefault(s => s.Id == "NU_PROCESO").Hidden = true;
                        grid.Columns.FirstOrDefault(s => s.Id == "VL_FOB_INGRESO").Hidden = true;
                    }
                    else
                    {
                        //Acta negativa
                        grid.Columns.FirstOrDefault(s => s.Id == "NU_PROCESO").Hidden = false;
                        grid.Columns.FirstOrDefault(s => s.Id == "VL_MERCADERIA").Hidden = true;
                    }
                }
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            var flagDocumento = string.Empty;
            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");
            var culture = this._identity.GetFormatProvider();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                var nuTransaccion = uow.GetTransactionNumber();
                var documento = uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);

                if (documento is IDocumentoEgreso)
                {
                    if (!documento.CanEdit(uow))
                        throw new ValidationFailedException("DOC340_Sec0_Error_ErrorEstado");
                    flagDocumento = "E";
                }
                else if (documento is IDocumentoActa)
                {
                    if (!documento.CanEdit(uow))
                        throw new ValidationFailedException("DOC340_Sec0_Error_ErrorEstado");

                    var acta = (IDocumentoActa)documento;
                    if (acta.Lineas.Count > 0)
                    {
                        flagDocumento = "AP";
                    }
                    else
                    {
                        flagDocumento = "AN";
                    }
                }

                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("DOC340_Sec0_Error_ErrorDuplicado");

                    foreach (var row in grid.Rows)
                    {
                        string cdProducto = row.GetCell("CD_PRODUTO").Value;
                        decimal cdFaixa = 1;
                        string nroIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
                        decimal? tributo = Convert.ToDecimal(row.GetCell("VL_TRIBUTO").Value, culture);

                        if (flagDocumento == "E")
                        {
                            string numeroProceso = row.GetCell("NU_PROCESO").Value;

                            this.UpdateDocumentoLinea(uow, ((IDocumentoEgreso)documento), cdProducto, nroIdentificador, cdFaixa, numeroProceso, tributo);
                        }
                        else if (flagDocumento == "AP")
                        {
                            decimal? mercaderia = Convert.ToDecimal(row.GetCell("VL_MERCADERIA").Value, culture);
                            decimal? cif = Convert.ToDecimal(row.GetCell("VL_CIF_INGRESO").Value, culture);

                            this.UpdateDocumentoActaLineaPositiva(uow, ((IDocumentoActa)documento), cdProducto, nroIdentificador, cdFaixa, mercaderia, cif, tributo);
                        }
                        else if (flagDocumento == "AN")
                        {
                            decimal? fob = Convert.ToDecimal(row.GetCell("VL_FOB_INGRESO").Value, culture);
                            decimal? cif = Convert.ToDecimal(row.GetCell("VL_CIF_INGRESO").Value, culture);
                            string numeroProceso = row.GetCell("NU_PROCESO").Value;

                            this.UpdateDocumentoActaLineaNegativa(uow, ((IDocumentoActa)documento), cdProducto, nroIdentificador, cdFaixa, fob, cif, tributo, numeroProceso);
                        }
                    }
                }

                uow.SaveChanges();

                // Valido documento en caso que se acta para habilitar cambio de estado
                if (flagDocumento == "AP" || flagDocumento == "AN")
                {
                    var acta = (IDocumentoActa)documento;
                    acta.ValidarActa();

                    uow.DocumentoRepository.UpdateActaSinDetalle(acta, nuTransaccion);

                    uow.SaveChanges();
                }
            }

            context.AddSuccessNotification("DOC340_Sec0_Error_Succes");

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            return this._gridValidationService.Validate(new DOC340GridValidationModule(tpDocumento, nuDocumento, uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentosDetalleQueryDOC340(nuDocumento, tpDocumento);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

                context.FileName = "LineasDoc_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var nuDocumento = context.GetParameter("nuDocumento");
            var tpDocumento = context.GetParameter("tpDocumento");

            var query = new DocumentosDetalleQueryDOC340(nuDocumento, tpDocumento);

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }

        #region Aux
        public virtual void SetEditableColumnsEgreso(IUnitOfWork uow, Grid grid, IDocumentoEgreso documento)
        {
            if (!documento.CanEdit(uow))
                return;

            grid.SetEditableCells(new List<string> { "VL_TRIBUTO", "NU_PROCESO" });
        }

        public virtual void SetEditableColumnsActa(IUnitOfWork uow, Grid grid, IDocumentoActa documento)
        {
            if (!documento.CanEdit(uow))
                return;

            var acta = (IDocumentoActa)documento;
            if (acta.Lineas.Count > 0)
            {
                // Actas positivas
                grid.SetEditableCells(new List<string> { "VL_TRIBUTO", "VL_MERCADERIA", "VL_CIF_INGRESO" });
            }
            else
            {
                //Actas negativas
                grid.SetEditableCells(new List<string> { "VL_TRIBUTO", "VL_FOB_INGRESO", "VL_CIF_INGRESO", "NU_PROCESO" });
            }
        }

        public virtual void UpdateDocumentoLinea(IUnitOfWork uow, IDocumentoEgreso documento, string cdProducto, string nroIdentificador, decimal faixa, string numeroProceso, decimal? tributo)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var linea = uow.DocumentoRepository.GetLineaEgreso(documento, cdProducto, nroIdentificador);

            linea.Tributo = tributo;
            linea.NumeroProceso = numeroProceso;

            uow.DocumentoRepository.UpdateLineaEgreso(linea, documento.Numero, documento.Tipo, nuTransaccion);
            uow.SaveChanges();
        }

        public virtual void UpdateDocumentoActaLineaPositiva(IUnitOfWork uow, IDocumentoActa documento, string cdProducto, string nroIdentificador, decimal faixa, decimal? valorMercaderia, decimal? cif, decimal? tributo)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var linea = uow.DocumentoRepository.GetLineaIngreso(documento.Numero, documento.Tipo, cdProducto, nroIdentificador);

            linea.ValorMercaderia = valorMercaderia;
            linea.CIF = cif;
            linea.ValorTributo = tributo;

            uow.DocumentoRepository.UpdateLineaActa(linea, documento.Numero, documento.Tipo, nuTransaccion);
            uow.SaveChanges();
        }

        public virtual void UpdateDocumentoActaLineaNegativa(IUnitOfWork uow, IDocumentoActa documento, string cdProducto, string nroIdentificador, decimal faixa, decimal? fob, decimal? cif, decimal? tributo, string numeroProceso)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var linea = uow.DocumentoRepository.GetLineaEgreso(documento.Numero, documento.Tipo, cdProducto, nroIdentificador);

            linea.FOB = fob;
            linea.CIF = cif;
            linea.Tributo = tributo;
            linea.NumeroProceso = numeroProceso;

            uow.DocumentoRepository.UpdateLineaEgresoActa(linea, documento.Numero, documento.Tipo, nuTransaccion);
            uow.SaveChanges();
        }
        #endregion GRID
    }
}