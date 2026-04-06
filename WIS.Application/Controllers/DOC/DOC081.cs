using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Documento;
using WIS.Application.Validation.Modules.GridModules.Documento;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Documento;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.Extension;
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
    public class DOC081 : AppController
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
        protected readonly DocumentoTipoMapper _mapper;

        protected List<string> GridKeys { get; }
        protected List<string> InsertableColumns { get; }

        public DOC081(ISessionAccessor session,
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
            this._mapper = new DocumentoTipoMapper();

            this.GridKeys = new List<string>
            {
                "CD_PRODUTO",
                "CD_EMPRESA",
                "NU_IDENTIFICADOR",
                "CD_FAIXA",
                "NU_DOCUMENTO",
                "TP_DOCUMENTO"
            };

            this.InsertableColumns = new List<string>
            {
                "CD_PRODUTO",
                "NU_IDENTIFICADOR",
                "QT_INGRESADA",
                "VL_MERCADERIA",
                "VL_TRIBUTO"
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            string nroDocumento = context.GetParameter("nuDocumento");
            string tipoDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                IDocumentoIngreso documento = uow.DocumentoRepository.GetIngreso(nroDocumento, tipoDocumento);

                form.GetField("NU_DOCUMENTO").Value = documento.Numero;
                form.GetField("TP_DOCUMENTO").Value = documento.Tipo;
                form.GetField("AUXDS_DOCUMENTO").Value = documento.Descripcion;
                form.GetField("CD_EMPRESA").Value = Convert.ToString(documento.Empresa);
                form.GetField("NM_EMPRESA").Value = uow.EmpresaRepository.GetNombre(documento.Empresa ?? -1);
                form.GetField("TP_DUA").Value = documento.DocumentoAduana.Tipo;
                form.GetField("NU_DUA").Value = documento.DocumentoAduana.Numero;
                form.GetField("NU_EXPORT").Value = documento.NumeroExportacion;
                form.GetField("NU_IMPORT").Value = documento.NumeroImportacion;
                form.GetField("DS_DOCUMENTO").Value = documento.Descripcion;
                form.GetField("NU_FACTURA").Value = documento.Factura;
                form.GetField("NU_CONOCIMIENTO").Value = documento.Conocimiento;
                form.GetField("ID_ESTADO").Value = documento.Estado;
                form.GetField("DT_ADDROW").Value = documento.FechaAlta.ToIsoString();
                form.GetField("DT_UPDROW").Value = documento.FechaModificacion.ToIsoString();
                form.GetField("DT_PROGRAMADO").Value = documento.FechaProgramado.ToIsoString();
                form.GetField("DT_ENVIADO").Value = documento.FechaEnviado.ToIsoString();
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            string nuDocumento = context.GetParameter("nuDocumento");
            string tpDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var documento = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);

                if (documento != null)
                {
                    if (documento.Lineas.Count > 0)
                    {
                        uow.CreateTransactionNumber(this._identity.Application);

                        documento.CalcularValoresCifFob();

                        var nuTransaccion = uow.GetTransactionNumber();

                        uow.DocumentoRepository.UpdateIngresoAndDetails(documento, nuTransaccion);
                        uow.SaveChanges();

                        context.AddSuccessNotification("DOC081_Sec0_Error_Error01");
                        context.ResetForm = true;
                    }
                    else
                    {
                        context.AddErrorNotification("DOC081_Sec0_Error_Error02");
                    }
                }
                else
                {
                    context.AddErrorNotification("DOC081_Sec0_Error_Error03");
                }
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            string nroDocumento = context.GetParameter("nuDocumento");
            string tipoDocumento = context.GetParameter("tpDocumento");

            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new DOC081FormValidationModule(nroDocumento, tipoDocumento, uow, this._identity), form, context);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            string nroDocumento = context.GetParameter("nuDocumento");
            string tipoDocumento = context.GetParameter("tpDocumento");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var tpDoc = uow.DocumentoTipoRepository.GetTipoDocumento(tipoDocumento);
                var documento = uow.DocumentoRepository.GetIngreso(nroDocumento, tipoDocumento);

                context.IsAddEnabled = tpDoc.PermiteAgregarDetalle;
                context.IsCommitEnabled = tpDoc.PermiteAgregarDetalle;
                context.IsRemoveEnabled = tpDoc.PermiteRemoverDetalle;

                if (documento.CanEdit(uow))
                {
                    grid.SetInsertableColumns(this.InsertableColumns);
                    context.IsEditingEnabled = true;
                }
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroDocumento = context.GetParameter("nuDocumento");
                string tipoDocumento = context.GetParameter("tpDocumento");

                var dbQuery = new DocumentoDetalleQuery(nroDocumento, tipoDocumento);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows.AddRange(_gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys));

                IDocumentoIngreso documento = uow.DocumentoRepository.GetIngreso(nroDocumento, tipoDocumento);

                this.SetEditableColumns(uow, grid, documento);
            }

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                DocumentoLinea detalleDocumento = null;
                var nroDocumento = context.GetParameter("nuDocumento");
                var tipoDocumento = context.GetParameter("tpDocumento");
                var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));
                var documento = uow.DocumentoRepository.GetIngreso(nroDocumento, tipoDocumento);
                var culture = this._identity.GetFormatProvider();

                if (!documento.CanEdit(uow))
                    throw new ValidationFailedException("DOC081_Sec0_Error_Error05");

                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("DOC081_Sec0_Error_Error06");

                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var nuTransaccion = uow.GetTransactionNumber();

                    foreach (var row in grid.Rows)
                    {
                        var cdProducto = row.GetCell("CD_PRODUTO").Value;
                        var cdFaixa = 1;
                        var nroIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
                        var nroIdentificadorOld = row.GetCell("NU_IDENTIFICADOR").Old;
                        var quantity = Convert.ToDecimal(row.GetCell("QT_INGRESADA").Value, culture);
                        var value = Convert.ToDecimal(row.GetCell("VL_MERCADERIA").Value, culture);
                        decimal? tributo = null;

                        if (decimal.TryParse(row.GetCell("VL_TRIBUTO").Value, NumberStyles.Number, culture, out _))
                        {
                            tributo = Convert.ToDecimal(row.GetCell("VL_TRIBUTO").Value, culture);
                        }

                        if (row.IsNew)
                        {
                            this.AddDocumentoLinea(uow, documento, cdProducto, nroIdentificador, quantity, value, tributo, null);
                        }
                        else if (row.IsDeleted)
                        {
                            detalleDocumento = uow.DocumentoRepository.GetLineaIngreso(nroDocumento, tipoDocumento, cdProducto, nroIdentificador);
                            decimal? catidadReverva;
                            if (detalleDocumento != null)
                                this.DeleteDocumentoLinea(uow, documento, cdProducto, nroIdentificador, cdFaixa, out catidadReverva);
                        }
                        else
                        {
                            this.UpdateDocumentoLinea(uow, documento, cdProducto, nroIdentificador, nroIdentificadorOld, cdFaixa, quantity, value, tributo);
                        }
                    }

                    if (documento.Validado)
                    {
                        documento.Validado = false;
                        uow.DocumentoRepository.UpdateIngreso(documento, nuTransaccion);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("DOC081_Sec0_Error_Error07");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            string nroDocumento = context.GetParameter("nuDocumento");
            string tipoDocumento = context.GetParameter("tpDocumento");
            string cdEmpresa = context.GetParameter("cdEmpresa");

            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new DOC081GridValidationModule(nroDocumento, tipoDocumento, cdEmpresa, uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nroDocumento = context.GetParameter("nuDocumento");
            string tipoDocumento = context.GetParameter("tpDocumento");

            var dbQuery = new DocumentoDetalleQuery(nroDocumento, tipoDocumento);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroDocumento = context.GetParameter("nuDocumento");
                string tipoDocumento = context.GetParameter("tpDocumento");

                var dbQuery = new DocumentoDetalleQuery(nroDocumento, tipoDocumento);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (context.Payload == null)
                throw new MissingParameterException("Datos nulos");

            using (var excelImporter = new GridExcelImporter(context.Translator, context.FileName, grid.Columns, context.Payload))
            {
                try
                {
                    var rowsExcel = excelImporter.BuildRows();

                    int rowId = 0;

                    foreach (var row in rowsExcel)
                    {
                        foreach (var column in grid.Columns)
                        {
                            if (!row.Cells.Any(c => c.Column.Id == column.Id))
                            {
                                row.AddCell(new GridCell()
                                {
                                    Column = column,
                                });
                            }
                        }

                        rowId--;
                        row.Id = rowId.ToString();

                        var validationContext = new GridValidationContext
                        {
                            Parameters = context.FetchContext.Parameters
                        };

                        grid = this.GridValidateRow(row, grid, validationContext);
                    }

                    if (grid.Rows.Any(r => !r.IsDeleted && !r.IsValid()))
                        throw new ValidationFailedException("General_Sec0_Error_Error07");

                    grid.Rows.AddRange(rowsExcel);

                    grid = this.GridFetchRows(grid, context.FetchContext);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return grid;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO":
                    return this.SearchProducto(grid, row, context);
            }

            return new List<SelectOption>();
        }

        #region Aux

        public virtual void AddDocumentoLinea(IUnitOfWork uow, IDocumentoIngreso documento, string cdProducto, string nroIdentificador, decimal? quantity, decimal? value, decimal? tributo, decimal? CantidadReservada)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var cdEmpresa = documento.Empresa ?? -1;
            var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cdProducto);
            var nroIdentificadorFinal = producto.ParseIdentificador(nroIdentificador);
            var lineaDocumento = new DocumentoLinea
            {
                Producto = cdProducto,
                Empresa = cdEmpresa,
                Faixa = 1,
                Identificador = nroIdentificadorFinal,
                CantidadIngresada = quantity,
                ValorMercaderia = value,
                FechaAlta = DateTime.Now,
                Situacion = documento.Situacion,
                ValorTributo = tributo,
                CantidadReservada = CantidadReservada,
                IdentificadorIngreso = nroIdentificadorFinal

            };

            documento.Lineas.Add(lineaDocumento);
            uow.DocumentoRepository.AddDetail(documento, lineaDocumento, nuTransaccion);
        }

        public virtual void DeleteDocumentoLinea(IUnitOfWork uow, IDocumentoIngreso documento, string cdProducto, string nroIdentificador, decimal faixa, out decimal? cantidadReservada)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var lineaDocumento = documento.GetLinea(cdProducto, nroIdentificador, faixa);

            cantidadReservada = lineaDocumento.CantidadReservada;

            lineaDocumento.FechaModificacion = DateTime.Now;
            lineaDocumento.NumeroTransaccionDelete = nuTransaccion;

            uow.DocumentoRepository.UpdateDetailWithoutDocument(documento.Numero, documento.Tipo, lineaDocumento, nuTransaccion);
            uow.SaveChanges();

            uow.DocumentoRepository.RemoveDetail(documento, lineaDocumento, nuTransaccion);
            documento.Lineas.Remove(lineaDocumento);
        }

        public virtual void UpdateDocumentoLinea(IUnitOfWork uow, IDocumentoIngreso documento, string cdProducto, string nroIdentificador, string nroIdentificadorOld, decimal faixa, decimal? quantity, decimal? value, decimal? tributo)
        {
            decimal? CantidadReservada;
            this.DeleteDocumentoLinea(uow, documento, cdProducto, nroIdentificadorOld, faixa, out CantidadReservada);
            this.AddDocumentoLinea(uow, documento, cdProducto, nroIdentificador, quantity, value, tributo, CantidadReservada);
        }

        public virtual void SetEditableColumns(IUnitOfWork uow, Grid grid, IDocumentoIngreso documento)
        {
            if (!documento.CanEdit(uow))
                return;

            grid.SetEditableCells(uow.DocumentoTipoRepository.GetDetallesEditables(documento.Tipo));

            foreach (var row in grid.Rows)
            {
                var cellIdentificador = row.GetCell("NU_IDENTIFICADOR");
                string cdProducto = row.GetCell("CD_PRODUTO").Value;

                var cellEmpresa = row.GetCell("CD_EMPRESA");

                if (cellEmpresa != null && int.TryParse(cellEmpresa.Value, out int cdEmpresa))
                {
                    Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cdProducto, false);
                    if (producto != null && producto.IsIdentifiedByProducto())
                        cellIdentificador.Editable = false;
                }
            }
        }

        public virtual List<SelectOption> SearchProducto(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            string nroDocumento = context.GetParameter("nuDocumento");
            string tipoDocumento = context.GetParameter("tpDocumento");

            List<SelectOption> options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                IDocumentoIngreso documento = uow.DocumentoRepository.GetIngreso(nroDocumento, tipoDocumento);
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(documento.Empresa ?? -1, context.SearchValue);

                foreach (var producto in productos)
                {
                    options.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
                }
            }

            return options;
        }

        #endregion
    }
}
