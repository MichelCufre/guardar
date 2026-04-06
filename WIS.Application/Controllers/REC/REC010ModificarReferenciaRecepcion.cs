using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Domain.General;
using WIS.Components.Common.Select;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.General.Enums;
using WIS.GridComponent.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General.Auxiliares;
using Newtonsoft.Json;
using WIS.Application.Security;
using WIS.Extension;
using Newtonsoft.Json.Serialization;
using WIS.Application.Validation;
using WIS.Filtering;
using System.Globalization;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Application.Controllers.REC
{
    public class REC010ModificarReferenciaRecepcion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysDetalle { get; }

        public REC010ModificarReferenciaRecepcion(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IGridService gridService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysDetalle = new List<string>
            {
                "NU_RECEPCION_REFERENCIA_DET"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._security = security;
            _filterInterpreter = filterInterpreter;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion)
                throw new ValidationFailedException("REC010_frm1_error_DigitacionLineasNoPermitido");

            if (this._security.IsUserAllowed(SecurityResources.WREC011_Page_Access_Puede_Modificar))
            {
                query.AddParameter("PermiteIngresarLineas", "true");
            }

            this.InicializarSelects(form);

            var idReferencia = query.GetParameter("keyReferencia");

            if (!string.IsNullOrEmpty(idReferencia))
            {

                ReferenciaRecepcion referencia = uow.ReferenciaRecepcionRepository.GetReferencia(int.Parse(idReferencia));

                if (referencia == null)
                    throw new ValidationFailedException("REC010_Frm1_Error_ReferenciaNoExiste");

                this.InicializarCamposUpdate(uow, form, referencia);

            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, query);
                case "codigoInternoAgente": return this.SearchAgente(form, query);

                default: return new List<SelectOption>();
            }
        }

        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion)
                throw new ValidationFailedException("REC010_frm1_error_DigitacionLineasNoPermitido");

            var idReferencia = query.GetParameter("keyReferencia");

            var referencia = uow.ReferenciaRecepcionRepository.GetReferenciaConDetalle(int.Parse(idReferencia));

            if (referencia == null)
                throw new ValidationFailedException("REC010_Frm1_Error_ReferenciaNoExiste");

            if (referencia.Estado != EstadoReferenciaRecepcionDb.Abierta)
                throw new ValidationFailedException("REC010_Frm1_Error_EstadoReferenciaNoEditable");

            List<GridRow> rowsDetalles = new List<GridRow>();
            if (query.Parameters.Any(s => s.Id == "rowsDetalle"))
            {
                if (this._security.IsUserAllowed(SecurityResources.WREC011_Page_Access_Puede_Modificar))
                {
                    rowsDetalles = JsonConvert.DeserializeObject<List<GridRow>>(query.GetParameter("rowsDetalle"));
                }
            }

            uow.CreateTransactionNumber($"{this._identity.Application} - Modificar Referencia de Recepcion");
            uow.BeginTransaction();

            // Validar lineas de detalle
            this.ValidarDetalles(form, uow, rowsDetalles, referencia, query);

            referencia.CodigoCliente = form.GetField("codigoInternoAgente").Value;
            referencia.IdPredio = form.GetField("numeroPredio").Value;

            referencia.FechaEmitida = DateTime.Parse(form.GetField("fechaEmitida").Value, _identity.GetFormatProvider());
            referencia.Memo = form.GetField("memo").Value;
            referencia.Anexo1 = form.GetField("anexo1").Value;
            referencia.Anexo2 = form.GetField("anexo2").Value;
            referencia.Anexo3 = form.GetField("anexo3").Value;
            referencia.Moneda = form.GetField("moneda").Value;
            referencia.NumeroTransaccion = uow.GetTransactionNumber();

            if (DateTime.TryParse(form.GetField("fechaVencimiento").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime vencimiento))
            {
                referencia.FechaVencimientoOrden = vencimiento;
            }

            if (DateTime.TryParse(form.GetField("fechaEntrega").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime entrega))
            {
                referencia.FechaEntrega = entrega;
            }

            EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalles = this.MapReferenciaDetalle(referencia, rowsDetalles);

            this.CheckForDuplicatesOnReferencia(referencia.Detalles);

            UpdateReferencia(uow, referencia, cambiosDetalles);

            uow.SaveChanges();
            uow.Commit();

            query.AddSuccessNotification("REC010_Frm1_Succes_Edicion", new List<string> { referencia.Numero.ToString() });

            query.Parameters?.Clear();

            return form;
        }


        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoReferenciaRecepcionFormValidationModule(uow, this._identity.UserId), form, context);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idReferencia = query.GetParameter("keyReferencia");
            var referenciaEnUso = uow.ReferenciaRecepcionRepository.ReferenciaEnUso(int.Parse(idReferencia));

            if (referenciaEnUso)
            {
                query.IsEditingEnabled = false;
                query.IsAddEnabled = false;
                query.IsCommitEnabled = false;
                query.IsRemoveEnabled = false;
            }
            else
            {
                query.IsEditingEnabled = true;
                query.IsAddEnabled = true;
                query.IsCommitEnabled = true;
                query.IsRemoveEnabled = true;
                query.IsCommitButtonUnavailable = true;

                grid.SetInsertableColumns(new List<string>
                {
                    "CD_PRODUTO",
                    "NU_IDENTIFICADOR",
                    "QT_REFERENCIA",
                    "DT_VENCIMIENTO",
                    "ID_LINEA_SISTEMA_EXTERNO",
                    "DS_ANEXO1",
                    "IM_UNITARIO"
                });
            }

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string idReferencia = query.GetParameter("keyReferencia");

            var dbQuery = new ReferenciaRecepcionDetalleQuery(int.Parse(idReferencia));


            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DS_PRODUTO", SortDirection.Descending);

            grid.Rows.AddRange(_gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeysDetalle));

            foreach (var row in grid.Rows)
            {
                List<string> camposEditables = new List<string>() {
                    "CD_PRODUTO",
                    "QT_REFERENCIA",
                    "ID_LINEA_SISTEMA_EXTERNO",
                    "DS_ANEXO1",
                    "IM_UNITARIO"
                };

                bool importExcel = !string.IsNullOrEmpty(query.GetParameter("importExcel"));

                if (!importExcel)
                {
                    var manejofecha = uow.ProductoRepository.GetProductoManejoFecha(int.Parse(row.GetCell("CD_EMPRESA").Value), row.GetCell("CD_PRODUTO").Value);
                    var manejo = uow.ProductoRepository.GetProductoManejoIdentificador(int.Parse(row.GetCell("CD_EMPRESA").Value), row.GetCell("CD_PRODUTO").Value);

                    if (manejofecha == ManejoFechaProducto.Expirable)
                        camposEditables.Add("DT_VENCIMIENTO");

                    if (manejo != ManejoIdentificador.Producto)
                        camposEditables.Add("NU_IDENTIFICADOR");
                }
                row.SetEditableCells(camposEditables);
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string idReferencia = query.GetParameter("keyReferencia");

            var dbQuery = new ReferenciaRecepcionDetalleQuery(int.Parse(idReferencia));

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext query)
        {
            switch (query.ColumnId)
            {
                case "CD_PRODUTO": return this.SearchProducto(grid, query);
            }

            return new List<SelectOption>();
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
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoDetalleReferenciaRecepcionGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form)
        {
            //Inicializar selects
            FormField selectTipoReferencia = form.GetField("tipoReferencia");
            FormField selectPredio = form.GetField("numeroPredio");
            FormField selectMoneda = form.GetField("moneda");

            selectTipoReferencia.Options = new List<SelectOption>();
            selectPredio.Options = new List<SelectOption>();
            selectMoneda.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Tipos de recepción
            List<ReferenciaRecepcionTipo> tiposAlmacenajes = uow.ReferenciaRecepcionRepository.GetReferenciaRecepcionTipos();

            foreach (var tipo in tiposAlmacenajes)
            {
                selectTipoReferencia.Options.Add(new SelectOption(tipo.Tipo.ToString(), $"{tipo.Tipo} - {tipo.Descripcion}"));
            }

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            // Moneda
            List<Moneda> monedas = uow.MonedaRepository.GetMonedas();

            foreach (var moneda in monedas)
            {
                selectMoneda.Options.Add(new SelectOption(moneda.Codigo, $"{moneda.Codigo} - {moneda.Descripcion}"));
            }
        }
        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, ReferenciaRecepcion referencia)
        {
            // Marcar campos solo de lectura
            form.GetField("codigo").ReadOnly = true;
            form.GetField("tipoReferencia").ReadOnly = true;
            form.GetField("idEmpresa").ReadOnly = true;
            form.GetField("codigoInternoAgente").ReadOnly = true;

            var referenciaEnUso = uow.ReferenciaRecepcionRepository.ReferenciaEnUso(referencia.Id);
            if (referenciaEnUso)
                form.GetField("numeroPredio").ReadOnly = true;

            // Cargar valores iniciales
            form.GetField("codigo").Value = referencia.Numero;
            form.GetField("tipoReferencia").Value = referencia.TipoReferencia;

            // Carga de search Empresa
            var fieldEmpresa = form.GetField("idEmpresa");
            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = referencia.IdEmpresa.ToString()
            });

            fieldEmpresa.Value = referencia.IdEmpresa.ToString();

            // Carga de search Empresa
            var fieldAgente = form.GetField("codigoInternoAgente");
            fieldAgente.Options = SearchAgente(form, new FormSelectSearchContext()
            {
                SearchValue = referencia.CodigoCliente.ToString()
            });

            fieldAgente.Value = referencia.CodigoCliente.ToString();

            form.GetField("numeroPredio").Value = referencia.IdPredio;
            form.GetField("moneda").Value = referencia.Moneda;

            if (referencia.FechaVencimientoOrden != null)
                form.GetField("fechaVencimiento").Value = referencia.FechaVencimientoOrden.ToIsoString();

            form.GetField("fechaEmitida").Value = referencia.FechaEmitida.ToIsoString();
            form.GetField("fechaEntrega").Value = referencia.FechaEntrega.ToIsoString();

            form.GetField("memo").Value = referencia.Memo;
            form.GetField("anexo1").Value = referencia.Anexo1;
            form.GetField("anexo2").Value = referencia.Anexo2;
            form.GetField("anexo3").Value = referencia.Anexo3;

        }

        public virtual void UpdateReferencia(IUnitOfWork uow, ReferenciaRecepcion referencia, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalles)
        {
            cambiosDetalles.AddedRecords.ForEach(d => d.NumeroTransaccion = referencia.NumeroTransaccion);
            cambiosDetalles.UpdatedRecords.ForEach(d => d.NumeroTransaccion = referencia.NumeroTransaccion);

            foreach (var dr in cambiosDetalles.DeletedRecords)
            {
                dr.NumeroTransaccion = referencia.NumeroTransaccion;
                dr.NumeroTransaccionDelete = referencia.NumeroTransaccion;
                dr.FechaModificacion = DateTime.Now;

                uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(dr);
            }

            uow.SaveChanges();
            uow.ReferenciaRecepcionRepository.UpdateReferencia(referencia, cambiosDetalles);
        }
        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext FormQuery)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(FormQuery.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchAgente(Form form, FormSelectSearchContext FormQuery)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {

                var dbQuery = new GetAgentesQuery(FormQuery.SearchValue, idEmpresa);
                uow.HandleQuery(dbQuery);

                List<AgenteAuxiliar> agentes = dbQuery.GetByDescripcionOrAgentePartial(FormQuery.SearchValue, idEmpresa);

                foreach (var agente in agentes)
                {
                    opciones.Add(new SelectOption(agente.CodigoInterno, $"{agente.Tipo} - {agente.Codigo} - {agente.Descripcion}"));
                }

            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext GridQuery)
        {
            string paramEmpresa = GridQuery.GetParameter("empresa");

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(paramEmpresa))
                return opciones;

            int empresa = int.Parse(paramEmpresa);

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, GridQuery.SearchValue);

            foreach (Producto producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
            }

            return opciones;
        }

        public virtual void ValidarDetalles(Form form, IUnitOfWork uow, List<GridRow> rowsDetalles, ReferenciaRecepcion referencia, FormSubmitContext query)
        {
            var parametros = new List<ComponentParameter>
            {
                new ComponentParameter("empresa", referencia.IdEmpresa.ToString()),
            };

            this.ValidateRowsDetalles(uow, rowsDetalles, parametros);

            if (rowsDetalles.Any(d => !d.IsValid()))
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var json = JsonConvert.SerializeObject(rowsDetalles, serializerSettings);

                query.Parameters.Add(new ComponentParameter() { Id = "rowValidated", Value = json });

                throw new ValidationFailedException("REC010_frm1_error_ErroresEnLineas", true);
            }
        }

        public virtual void ValidateRowsDetalles(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            Grid grid = new Grid() { Rows = rows };

            // Verifica que no existan lineas duplicadas
            if (grid.HasNewDuplicates(new List<string>() { "CD_PRODUTO", "NU_IDENTIFICADOR", "ID_LINEA_SISTEMA_EXTERNO" }))
                throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

            var entradaValidationModule = new MantenimientoDetalleReferenciaRecepcionGridValidationModule(uow, this._identity.GetFormatProvider());
            var columnasAValidar = entradaValidationModule.Schema.Select(s => s.Key).ToList();

            foreach (var row in rows)
            {
                entradaValidationModule.Validator = new GridValidator(parametros);
                if (row.IsDeleted)
                    continue;

                row.Cells.Where(s => columnasAValidar.Contains(s.Column.Id)).ToList()
                    .ForEach(c => c.Modified = true);
                entradaValidationModule.Validate(row);
            }
        }

        public virtual EntityChanges<ReferenciaRecepcionDetalle> MapReferenciaDetalle(ReferenciaRecepcion referencia, List<GridRow> rowsDetalle)
        {
            var cambios = new EntityChanges<ReferenciaRecepcionDetalle>();

            foreach (var row in rowsDetalle)
            {

                if (row.IsNew)
                {
                    var entrada = new ReferenciaRecepcionDetalle
                    {
                        IdReferencia = referencia.Id,
                        IdEmpresa = referencia.IdEmpresa,
                        CodigoProducto = row.GetCell("CD_PRODUTO").Value,
                        Faixa = 1,
                        Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                        CantidadReferencia = decimal.Parse(row.GetCell("QT_REFERENCIA").Value, this._identity.GetFormatProvider()),
                        IdLineaSistemaExterno = row.GetCell("ID_LINEA_SISTEMA_EXTERNO").Value,
                        Anexo1 = row.GetCell("DS_ANEXO1").Value,
                        CantidadAgendada = 0,
                        CantidadRecibida = 0,
                        CantidadAnulada = 0,
                    };

                    if (decimal.TryParse(row.GetCell("IM_UNITARIO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal value))
                        entrada.ImporteUnitario = value;

                    if (DateTime.TryParse(row.GetCell("DT_VENCIMIENTO").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime vencimiento))
                    {
                        entrada.FechaVencimiento = vencimiento;
                    }

                    referencia.Detalles.Add(entrada);

                    cambios.AddedRecords.Add(entrada);
                }
                else if (row.IsDeleted)
                {
                    int idDetalle = int.Parse(row.GetCell("NU_RECEPCION_REFERENCIA_DET").Value);

                    var detalle = referencia.Detalles.Where(d => d.Id == idDetalle).FirstOrDefault();

                    if (detalle != null)
                        cambios.DeletedRecords.Add(detalle);
                }
                else
                {
                    int idDetalle = int.Parse(row.GetCell("NU_RECEPCION_REFERENCIA_DET").Value);

                    var detalle = referencia.Detalles.Where(d => d.Id == idDetalle).FirstOrDefault();

                    if (detalle == null)
                        throw new EntityNotFoundException("REC010_Frm1_Error_ReferenciaNoExiste");

                    detalle.CodigoProducto = row.GetCell("CD_PRODUTO").Value;
                    detalle.Identificador = row.GetCell("NU_IDENTIFICADOR").Value;

                    detalle.CantidadReferencia = decimal.Parse(row.GetCell("QT_REFERENCIA").Value, this._identity.GetFormatProvider());
                    detalle.IdLineaSistemaExterno = row.GetCell("ID_LINEA_SISTEMA_EXTERNO").Value;
                    detalle.Anexo1 = row.GetCell("DS_ANEXO1").Value;

                    if (decimal.TryParse(row.GetCell("IM_UNITARIO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal value))
                        detalle.ImporteUnitario = value;

                    if (DateTime.TryParse(row.GetCell("DT_VENCIMIENTO").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime vencimiento))
                    {
                        detalle.FechaVencimiento = vencimiento;
                    }

                    cambios.UpdatedRecords.Add(detalle);
                }
            }

            return cambios;
        }
        public virtual void CheckForDuplicatesOnReferencia(List<ReferenciaRecepcionDetalle> cambiosDetalles)
        {
            if (cambiosDetalles.GroupBy(d => new { d.IdEmpresa, d.IdLineaSistemaExterno, d.CodigoProducto, d.Identificador }).Any(d => d.Count() > 1))
                throw new ValidationFailedException("REC010_Sec0_error_LineasDetallesDuplicadas");

        }

        #endregion

    }
}
