using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
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
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC500FacturaDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC500FacturaDetalle> _logger;

        protected List<string> GridKeysDetalle { get; }

        public REC500FacturaDetalle(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ITrafficOfficerService concurrencyControl,
            IGridValidationService gridValidationService,
            IGridExcelService gridExcelService,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC500FacturaDetalle> logger)
        {
            this.GridKeysDetalle = new List<string>
            {
                "NU_RECEPCION_FACTURA",
                "CD_EMPRESA",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "CD_PRODUTO"
            };

            this._uowFactory = uowFactory;
            this._concurrencyControl = concurrencyControl;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._excelService = gridExcelService;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string idFactura = context.GetParameter("keyFactura");

            Factura factura = uow.FacturaRepository.GetFacturaCabezal(int.Parse(idFactura));

            if (!factura.Agenda.HasValue || (factura.Agenda.HasValue && !uow.AgendaRepository.IsAgendaFacturaValida(factura.Agenda.Value)))
            {
                context.IsEditingEnabled = true;
                context.IsAddEnabled = true;
                context.IsCommitEnabled = true;
                context.IsRemoveEnabled = true;
                context.IsCommitButtonUnavailable = true;

                grid.SetInsertableColumns(new List<string>
                {
                    "CD_PRODUTO",
                    "NU_IDENTIFICADOR",
                    "QT_FACTURADA",
                    "IM_UNITARIO_DIGITADO",
                });
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string idFactura = context.GetParameter("keyFactura");
            Factura factura = uow.FacturaRepository.GetFacturaCabezal(int.Parse(idFactura));

            var dbQuery = new FacturaDetalleQuery(int.Parse(idFactura), factura.IdEmpresa);
            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeysDetalle);

            if (factura.Agenda == null)
            {
                foreach (var row in grid.Rows)
                {
                    row.SetEditableCells(new List<string> {
                        "CD_PRODUTO",
                        "NU_IDENTIFICADOR",
                        "QT_FACTURADA",
                        "IM_UNITARIO_DIGITADO",
                    });
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string idFactura = context.GetParameter("keyFactura");

            Factura factura = uow.FacturaRepository.GetFacturaCabezal(int.Parse(idFactura));

            var dbQuery = new FacturaDetalleQuery(int.Parse(idFactura), factura.IdEmpresa);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyFactura")?.Value, out int idFactura))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            Factura factura = uow.FacturaRepository.GetFacturaCabezal(idFactura);

            var dbQuery = new FacturaDetalleQuery(idFactura, factura.IdEmpresa);
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (context.Payload == null)
                throw new MissingParameterException("Datos nulos");

            grid = this.GridFetchRows(grid, context.FetchContext);

            using (var excelImporter = new GridExcelImporter(
                context.Translator,
                context.FileName,
                grid.Columns,
                context.Payload))
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
                                row.AddCell(new GridCell { Column = column });
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
                }
                catch (ValidationFailedException ex)
                {
                    var payload = Convert.ToBase64String(excelImporter.GetAsByteArray());
                    throw new GridExcelImporterException(ex.Message, payload, ex.StrArguments);
                }
                catch (Exception ex)
                {
                    context.AddErrorNotification(ex.Message);
                    throw;
                }
            }

            return grid;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO": return this.SearchProducto(grid, context);
            }
            return new List<SelectOption>();
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!context.Parameters.Any(s => s.Id == "keyFactura"))
                throw new MissingParameterException("General_Sec0_Error_Error80");

            var factura = uow.FacturaRepository.GetFacturaCabezal(int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "keyFactura").Value));

            context.Parameters.Add(new ComponentParameter("factura", factura.Id.ToString()));
            context.Parameters.Add(new ComponentParameter("empresa", factura.IdEmpresa.ToString()));
            context.Parameters.Add(new ComponentParameter("faixa", "1"));

            return this._gridValidationService.Validate(new MantenimientoDetalleFacturaGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idFactura = context.GetParameter("keyFactura");

            if (!string.IsNullOrEmpty(idFactura))
            {
                Factura factura = uow.FacturaRepository.GetFacturaCabezal(int.Parse(idFactura));

                if (factura == null)
                    throw new ValidationFailedException("REC500_Frm1_Error_FacturaNoExiste", new string[] { idFactura });

                string empresa = uow.EmpresaRepository.GetNombre(factura.IdEmpresa);

                context.AddParameter("infoFactura", $"{factura.Id}");
                context.AddParameter("cdCliente", factura.CodigoInternoCliente);
                context.AddParameter("idEmpresa", factura.IdEmpresa.ToString());

                if (factura.Agenda != null && uow.AgendaRepository.IsAgendaFacturaValida(factura.Agenda.Value))
                {
                    context.AddParameter("NotAsignada", "false");
                    form.GetButton("btnSubmitGuardar").Disabled = true;
                    form.GetButton("btnSubmitConfirmar").Disabled = true;
                    form.GetButton("btnSubmitGuardar").Hidden = true;
                    form.GetButton("btnSubmitConfirmar").Hidden = true;
                }

            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("Crear detalle factura");

            try
            {
                var idFactura = context.GetParameter("keyFactura");
                var factura = uow.FacturaRepository.GetFactura(int.Parse(idFactura));

                if (factura == null)
                    throw new ValidationFailedException("REC500_Sec0_Error_FacturaNoEncontrada", new string[] { idFactura });

                if (factura.Agenda.HasValue && uow.AgendaRepository.IsAgendaFacturaValida(factura.Agenda.Value))
                    throw new ValidationFailedException("REC500_Sec0_Error_FacturaValildada", new string[] { idFactura });

                List<GridRow> rowsDetalles = new List<GridRow>();

                if (context.Parameters.Any(s => s.Id == "rowsDetalle"))
                {
                    rowsDetalles = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsDetalle"));
                }

                if (context.ButtonId == "btnConfirmarReferencia")
                {
                    if (!int.TryParse(form.GetField("idReferencias").Value, out int idReferencia))
                        throw new ValidationFailedException("REC500_Sec0_Error_ReferenciaRequerida");

                    var referencia = uow.ReferenciaRecepcionRepository.GetReferenciaConDetalle(idReferencia);

                    if (referencia == null)
                        throw new ValidationFailedException("REC500_Sec0_Error_ReferenciaNoExiste");

                    AsignarDetalleFacturaPorReferencia(referencia, factura, uow);
                }
                else
                {
                    ValidarDetalles(form, uow, rowsDetalles, factura, context);

                    foreach (var row in rowsDetalles)
                    {
                        if (!int.TryParse(row.GetCell("NU_RECEPCION_FACTURA_DET").Value, out int nuRecepcionFactura))
                            throw new ValidationFailedException("REC500_Sec0_Error_ErrorObtenerFactura");

                        if (row.IsDeleted)
                        {
                            var detalle = factura.Detalles.First(s => s.Id == nuRecepcionFactura);
                            detalle.FechaModificacion = DateTime.Now;
                            detalle.NumeroTransaccionDelete = uow.GetTransactionNumber();

                            uow.FacturaRepository.UpdateFacturaDetalle(detalle);

                            uow.FacturaRepository.DeleteFacturaDetalle(detalle);
                        }
                        else if (row.IsNew)
                        {
                            var detalle = ProcesarAddRow(uow, row, factura);
                            uow.FacturaRepository.AddFacturaDetalle(detalle);
                        }
                        else
                        {
                            var detalle = factura.Detalles.First(s => s.Id == nuRecepcionFactura);

                            if (decimal.TryParse(row.GetCell("IM_UNITARIO_DIGITADO")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal unitarioDigitado))
                                detalle.ImporteUnitario = unitarioDigitado;

                            if (decimal.TryParse(row.GetCell("QT_FACTURADA")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal facturada))
                                detalle.CantidadFacturada = facturada;

                            detalle.FechaModificacion = DateTime.Now;
                            detalle.NumeroTransaccion = uow.GetTransactionNumber();
                            uow.FacturaRepository.UpdateFacturaDetalle(detalle);
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("REC500_Frm1_Succes_Edicion", new List<string> { factura.Id.ToString() });

                context.Parameters?.Clear();
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FormSubmit Error: {ex.Message}");
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "idReferencias": return this.SearchReferencia(form, context);

                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void ValidarDetalles(Form form, IUnitOfWork uow, List<GridRow> rowsDetalles, Factura factura, FormSubmitContext context)
        {
            var parametros = new List<ComponentParameter>
                {
                    new ComponentParameter("empresa", factura.IdEmpresa.ToString()),
                    new ComponentParameter("factura", factura.Id.ToString()),
                    new ComponentParameter("faixa", "1"),
                };

            this.ValidateRowsDetalles(uow, rowsDetalles, parametros);

            if (rowsDetalles.Any(d => !d.IsValid()))
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var json = JsonConvert.SerializeObject(rowsDetalles, serializerSettings);

                context.Parameters.Add(new ComponentParameter() { Id = "rowValidated", Value = json });

                throw new ValidationFailedException("REC500_frm1_error_ErroresEnLineas", true);
            }
        }

        public virtual void ValidateRowsDetalles(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            Grid grid = new Grid() { Rows = rows };

            if (grid.HasNewDuplicates(new List<string>() { "CD_PRODUTO", "NU_IDENTIFICADOR" }))
                throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

            var entradaValidationModule = new MantenimientoDetalleFacturaGridValidationModule(uow, this._identity.GetFormatProvider());
            var columnasAValidar = entradaValidationModule.Schema.Select(s => s.Key).ToList();

            foreach (var row in rows)
            {
                entradaValidationModule.Validator = new GridValidator(parametros);

                if (row.IsDeleted)
                {
                    var detalle = uow.FacturaRepository.GetFacturaDetalle(int.Parse(row.GetCell("NU_RECEPCION_FACTURA").Value), int.Parse(row.GetCell("CD_EMPRESA").Value), row.GetCell("CD_PRODUTO").Value, 1, row.GetCell("NU_IDENTIFICADOR").Value);

                    if (detalle == null)
                        continue;
                }
                row.Cells.Where(s => columnasAValidar.Contains(s.Column.Id)).ToList()
                    .ForEach(c => c.Modified = true);
                entradaValidationModule.Validate(row);
            }
        }

        public virtual void AsignarDetalleFacturaPorReferencia(ReferenciaRecepcion referencia, Factura factura, IUnitOfWork uow)
        {
            var detFacturas = referencia.Detalles
                .Where(p => p.CantidadReferencia - p.CantidadRecibida - p.CantidadAnulada > 0)
                .GroupBy(d => new { d.CodigoProducto, d.IdEmpresa, d.Identificador })
                .Select(d => new FacturaDetalle
                {
                    IdFactura = factura.Id,
                    IdEmpresa = factura.IdEmpresa,
                    Producto = d.Key.CodigoProducto,
                    Identificador = d.Key.Identificador,
                    CantidadFacturada = d.Sum(p => p.CantidadReferencia - p.CantidadRecibida - p.CantidadAnulada),
                    CantidadValidada = 0,
                    CantidadRecibida = 0,
                    Faixa = 1,
                    NumeroTransaccion = uow.GetTransactionNumber(),
                    FechaVencimiento = d.Min(d => d.FechaVencimiento) ?? referencia.FechaVencimientoOrden,
                    ImporteUnitario = null,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now
                }).ToList();


            foreach (var det in detFacturas)
            {
                var detFacturaExistente = factura.Detalles
                    .FirstOrDefault(d => d.Producto == det.Producto
                        && d.Identificador == det.Identificador);

                if (detFacturaExistente != null)
                {
                    detFacturaExistente.CantidadFacturada += det.CantidadFacturada;
                    detFacturaExistente.FechaModificacion = DateTime.Now;
                    detFacturaExistente.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.FacturaRepository.UpdateFacturaDetalle(detFacturaExistente);
                }
                else
                {
                    factura.Detalles.Add(det);
                    uow.FacturaRepository.AddFacturaDetalle(det);
                }
            }
        }

        public virtual void CheckForDuplicatesOnFactura(List<FacturaDetalle> cambiosDetalles)
        {
            if (cambiosDetalles.GroupBy(d => new { d.IdEmpresa, d.Producto, d.Identificador }).Any(d => d.Count() > 1))
                throw new ValidationFailedException("REC500_Sec0_error_LineasDetallesDuplicadas");
        }

        public virtual FacturaDetalle ProcesarAddRow(IUnitOfWork uow, GridRow row, Factura factura)
        {
            var detalle = new FacturaDetalle
            {
                IdFactura = factura.Id,
                IdEmpresa = factura.IdEmpresa,
                Producto = row.GetCell("CD_PRODUTO").Value,
                Faixa = 1,
                Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                CantidadValidada = 0,
                CantidadRecibida = 0,
                NumeroTransaccion = uow.GetTransactionNumber(),
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
            };

            if (decimal.TryParse(row.GetCell("IM_UNITARIO_DIGITADO")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal unitarioDigitado))
                detalle.ImporteUnitario = unitarioDigitado;

            if (decimal.TryParse(row.GetCell("QT_FACTURADA")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal facturada))
                detalle.CantidadFacturada = facturada;

            factura.Detalles.Add(detalle);

            return detalle;
        }

        public virtual FacturaDetalle ProcesarModifiedRow(IUnitOfWork uow, GridRow row, Factura factura, EntityChanges<FacturaDetalle> cambios)
        {
            var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value);

            var detalle = factura.Detalles.Where(d => d.Id == factura.Id
                                                  && d.IdEmpresa == factura.IdEmpresa
                                                  && d.Producto == row.GetCell("CD_PRODUTO").Old
                                                  && d.Faixa == faixa
                                                  && d.Identificador == row.GetCell("NU_IDENTIFICADOR").Old)
                                         .FirstOrDefault();

            if (detalle == null)
                throw new EntityNotFoundException("REC500_Frm1_Error_DetalleAgendaNoExiste");

            detalle.Producto = row.GetCell("CD_PRODUTO").Value;
            detalle.Identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            //detalle.CantidadAgendada = decimal.Parse(row.GetCell("QT_AGENDADO").Value, this._identity.GetFormatProvider());
            //detalle.CantidadAgendadaOriginal = decimal.Parse(row.GetCell("QT_AGENDADO").Value, this._identity.GetFormatProvider());

            if (row.GetCell("CD_PRODUTO").Old != row.GetCell("CD_PRODUTO").Value)
            {
                var producto = uow.ProductoRepository.GetProducto(factura.IdEmpresa, row.GetCell("CD_PRODUTO").Value);
                //detalle.Vencimiento = producto.GetFechaVencimiento();
            }

            cambios.UpdatedRecords.Add(detalle);

            return detalle;
        }

        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            string idFactura = context.GetParameter("keyFactura");

            if (string.IsNullOrEmpty(idFactura))
                return opciones;

            Factura factura = uow.FacturaRepository.GetFacturaCabezal(int.Parse(idFactura));

            opciones.AddRange(uow.ProductoRepository.GetByDescriptionOrCodePartial(factura.CodigoInternoCliente, factura.IdEmpresa, context.SearchValue));

            return opciones;
        }

        public virtual List<SelectOption> SearchReferencia(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();
            using var uow = this._uowFactory.GetUnitOfWork();

            //var fieldCliente = form.GetField("cdCliente");
            //var cdCliente = fieldCliente?.Value;

            var idFactura = context.GetParameter("keyFactura");
            Factura factura = uow.FacturaRepository.GetFactura(int.Parse(idFactura));

            if (!string.IsNullOrWhiteSpace(factura.CodigoInternoCliente))
            {
                var dbQuery = new BuscarReferenciasPorClienteQuery(context.SearchValue, factura.CodigoInternoCliente, factura.IdEmpresa);
                uow.HandleQuery(dbQuery);

                var referencias = dbQuery.GetResultados();
                foreach (var referencia in referencias)
                {
                    opciones.Add(new SelectOption(
                        referencia.NU_RECEPCION_REFERENCIA.ToString(),
                        $"Referencia: {referencia.NU_REFERENCIA} - Tp: {referencia.TP_REFERENCIA} - Vencimiento: {referencia.DT_VENCIMIENTO_ORDEN} - Entrega: {referencia.DT_ENTREGA} - Anexo 1:{referencia.DS_ANEXO1}"
                    ));
                }
            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }
            return opciones;
        }

        #endregion

    }
}
