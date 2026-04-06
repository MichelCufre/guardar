using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
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
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC170AgendaDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysDetalle { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC170AgendaDetalle(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ITrafficOfficerService concurrencyControl,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysDetalle = new List<string>
            {
                "NU_AGENDA",
                "CD_EMPRESA",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "CD_PRODUTO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DS_PRODUTO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._concurrencyControl = concurrencyControl;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var nuAgenda = int.Parse(context.GetParameter("keyAgenda"));

            var agenda = uow.AgendaRepository.GetAgendaSinDetalles(nuAgenda);

            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

            if (tipoRecepcion.PermiteDigitacion)
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
                    "QT_AGENDADO",

                });
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAgenda = int.Parse(context.GetParameter("keyAgenda"));
            var agenda = uow.AgendaRepository.GetAgendaSinDetalles(nuAgenda);

            var dbQuery = new AgendaDetalleQuery(agenda.Id, agenda.IdEmpresa);
            uow.HandleQuery(dbQuery);
            grid.Rows.AddRange(_gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysDetalle));

            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);
            if (tipoRecepcion.PermiteDigitacion)
            {
                foreach (var row in grid.Rows)
                {
                    row.SetEditableCells(new List<string>
                    {
                        "CD_PRODUTO",
                        "QT_AGENDADO",
                    });
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAgenda = int.Parse(context.GetParameter("keyAgenda"));

            var agenda = uow.AgendaRepository.GetAgendaSinDetalles(nuAgenda);

            var dbQuery = new AgendaDetalleQuery(agenda.Id, agenda.IdEmpresa);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (context.Payload == null)
                throw new MissingParameterException("Datos nulos");

            using (var excelImporter = new GridExcelImporter(context.Translator, context.FileName, grid.Columns, context.Payload))
            {
                try
                {
                    var rowId = 0;
                    var rowsExcel = excelImporter.BuildRows();

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

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!context.Parameters.Any(s => s.Id == "keyAgenda"))
                throw new MissingParameterException("General_Sec0_Error_Error80");

            var agenda = uow.AgendaRepository.GetAgendaSinDetalles(int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda").Value));

            context.Parameters.Add(new ComponentParameter("agenda", agenda.Id.ToString()));
            context.Parameters.Add(new ComponentParameter("empresa", agenda.IdEmpresa.ToString()));
            context.Parameters.Add(new ComponentParameter("faixa", "1"));

            return this._gridValidationService.Validate(new MantenimientoDetalleAgendaGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO": return this.SearchProducto(grid, context);
            }

            return new List<SelectOption>();
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idAgenda = context.GetParameter("keyAgenda");

            if (!string.IsNullOrEmpty(idAgenda))
            {
                var agenda = uow.AgendaRepository.GetAgendaSinDetalles(int.Parse(idAgenda));
                if (agenda == null)
                    throw new ValidationFailedException("REC170_Frm1_Error_AgendaNoExiste", new string[] { idAgenda });

                context.AddParameter("infoAgenda", $"{agenda.Id}");

                var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

                if (!tipoRecepcion.PermiteDigitacion)
                {
                    form.GetButton("btnSubmitGuardar").Disabled = true;
                    form.GetButton("btnSubmitConfirmar").Disabled = true;
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idAgenda = context.GetParameter("keyAgenda");

            var agenda = uow.AgendaRepository.GetAgenda(int.Parse(idAgenda));

            if (agenda == null)
                throw new ValidationFailedException("General_Sec0_Error_AgendaNoEncontrada", new string[] { idAgenda });

            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

            if (!tipoRecepcion.PermiteDigitacion)
                throw new ValidationFailedException("General_Sec0_Error_NoDigitaDetalle");

            if (!agenda.EnEstadoAbierta())
                throw new ValidationFailedException("REC170_Frm1_Error_EstadoAgendaNoEditable");

            var rowsDetalles = new List<GridRow>();
            if (context.Parameters.Any(s => s.Id == "rowsDetalle"))
                rowsDetalles = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsDetalle"));


            // Lockear agenda y referencias y detalles

            this._concurrencyControl.AddLock("T_AGENDA", agenda.Id.ToString());

            var referencias = uow.ReferenciaRecepcionRepository.GetReferenciasAgenda(agenda.Id);
            foreach (var item in referencias)
            {
                this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA", item.Id.ToString());
            }

            // Validar lineas de detalle
            this.ValidarDetalles(form, uow, rowsDetalles, agenda, context);

            this.MapAgendaDetalle(agenda, referencias, rowsDetalles, uow, out EntityChanges<AgendaDetalle> cambiosDetalles, out EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, out EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones);

            this.CheckForDuplicatesOnAgenda(agenda.Detalles);

            uow.CreateTransactionNumber($"{this._identity.Application} - Agenda Detalle");

            uow.BeginTransaction();

            try
            {
                // Persisto detalles de agenda
                UpdateAgendaDetalles(uow, cambiosDetalles);

                // Persisto modificaciones de las referencias
                UpdateReferenciaDetalles(uow, cambiosDetalleReferencia);

                // Persisto las asociaciones de las referencias y los detalles de agenda
                UpdateDetalleAgendaReferenciaAsociada(uow, cambiosAsociaciones);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("REC170_Frm1_Succes_Edicion", new List<string> { agenda.Id.ToString() });

                context.Parameters?.Clear();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
        }

        #region Auxs
        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var idAgenda = context.GetParameter("keyAgenda");

            if (string.IsNullOrEmpty(idAgenda))
                return opciones;

            var agenda = uow.AgendaRepository.GetAgendaSinDetalles(int.Parse(idAgenda));

            var productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(agenda.IdEmpresa, context.SearchValue);

            foreach (var producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
            }

            return opciones;
        }

        public virtual void ValidarDetalles(Form form, IUnitOfWork uow, List<GridRow> rowsDetalles, Agenda agenda, FormSubmitContext context)
        {
            var parametros = new List<ComponentParameter>
            {
                new ComponentParameter("empresa", agenda.IdEmpresa.ToString()),
                new ComponentParameter("agenda", agenda.Id.ToString()),
                new ComponentParameter("faixa", "1"),
            };

            this.ValidateRowsDetalles(uow, rowsDetalles, parametros);

            if (rowsDetalles.Any(d => !d.IsValid()))
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var json = JsonConvert.SerializeObject(rowsDetalles, serializerSettings);

                context.Parameters.Add(new ComponentParameter() { Id = "rowValidated", Value = json });

                throw new ValidationFailedException("REC170_frm1_error_ErroresEnLineas", true);
            }
        }

        public virtual void ValidateRowsDetalles(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            var grid = new Grid() { Rows = rows };

            // Verifica que no existan lineas duplicadas
            if (grid.HasNewDuplicates(new List<string>() { "CD_PRODUTO", "NU_IDENTIFICADOR" }))
                throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

            var entradaValidationModule = new MantenimientoDetalleAgendaGridValidationModule(uow, this._identity.GetFormatProvider());
            var columnasAValidar = entradaValidationModule.Schema.Select(s => s.Key).ToList();

            foreach (var row in rows)
            {
                entradaValidationModule.Validator = new GridValidator(parametros);

                if (row.IsDeleted)
                {
                    var detalle = uow.AgendaRepository.GetAgendaDetalle(int.Parse(row.GetCell("NU_AGENDA").Value), int.Parse(row.GetCell("CD_EMPRESA").Value), row.GetCell("CD_PRODUTO").Value, 1, row.GetCell("NU_IDENTIFICADOR").Value);

                    if (detalle == null)
                        continue;

                    if (detalle.CantidadRecibida > 0)
                    {
                        row.GetCell("CD_PRODUTO").SetError(new ComponentError("WREC171_Sec0_Error_TieneCantidadRecibida", new List<string>() { }));
                        //throw new ValidationFailedException("WREC171_Sec0_Error_TieneCantidadRecibida");
                    }
                }

                row.Cells
                    .Where(s => columnasAValidar.Contains(s.Column.Id))
                    .ToList()
                    .ForEach(c => c.Modified = true);

                entradaValidationModule.Validate(row);
            }
        }

        public virtual void MapAgendaDetalle(Agenda agenda, List<ReferenciaRecepcion> referencias, List<GridRow> rowsDetalle, IUnitOfWork uow, out EntityChanges<AgendaDetalle> cambios, out EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, out EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones)
        {
            // TODO Refactorizar toda la solución. El actual dominio no es el mejor y se hace un parche por falta de tiempo en la 1er entrega.
            // Gonzalo 08/03/2021

            // Busco detalles de las referencias asociadas y si tiene manejo de referencia            
            var manejaReferencia = this.ManejaReferencias(uow, agenda, referencias, out List<ReferenciaRecepcionDetalle> detallesReferenciaDisponibles);

            cambios = new EntityChanges<AgendaDetalle>();
            cambiosDetalleReferencia = new EntityChanges<ReferenciaRecepcionDetalle>();
            cambiosAsociaciones = new EntityChanges<DetalleAgendaReferenciaAsociada>();


            foreach (var row in rowsDetalle.Where(r => r.IsDeleted))
            {
                // Comienzo por devolver los saldos de los detalles eliminados 

                // Elimino detalle de agenda
                var detalle = this.ProcesarDeleteRow(uow, row, agenda, cambios);

                if (manejaReferencia && detalle != null)
                {
                    // Ajusto los saldos de las referencias y elimino las asociaciones 
                    this.ProcesarDeleteRowReferencia(uow, detalle, detallesReferenciaDisponibles, cambiosAsociaciones, cambiosDetalleReferencia);
                }
            }

            foreach (var row in rowsDetalle.Where(r => !r.IsNew && !r.IsDeleted))
            {
                // Se ajustan los saldos de las celdas editadas antes de agregar

                var detalle = this.ProcesarDeleteRow(uow, row, agenda, cambios);

                if (manejaReferencia && detalle != null)
                {
                    // Ajusto los saldos de las referencias y elimino las asociaciones 
                    this.ProcesarDeleteRowReferencia(uow, detalle, detallesReferenciaDisponibles, cambiosAsociaciones, cambiosDetalleReferencia);
                }

                detalle = this.ProcesarAddRow(uow, row, agenda, cambios);

                if (manejaReferencia && detalle != null)
                {
                    // Ajusto los saldos de las referencias y elimino las asociaciones 
                    this.ProcesarAddRowReferencia(uow, detalle, detallesReferenciaDisponibles, cambiosAsociaciones, cambiosDetalleReferencia);
                }
            }

            foreach (var row in rowsDetalle.Where(r => r.IsNew))
            {
                // Al final recorro las filas a agregar con todo el saldo posible disponible
                var detalle = this.ProcesarAddRow(uow, row, agenda, cambios);

                if (manejaReferencia && detalle != null)
                {
                    // Ajusto los saldos de las referencias y elimino las asociaciones 
                    this.ProcesarAddRowReferencia(uow, detalle, detallesReferenciaDisponibles, cambiosAsociaciones, cambiosDetalleReferencia);
                }
            }
        }
        public virtual bool ManejaReferencias(IUnitOfWork uow, Agenda agenda, List<ReferenciaRecepcion> referencias, out List<ReferenciaRecepcionDetalle> detallesReferenciaDisponibles)
        {
            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

            var manejaReferencia = false;
            detallesReferenciaDisponibles = new List<ReferenciaRecepcionDetalle>();

            if (tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion
                || tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion)
            {
                manejaReferencia = true;

                foreach (var referencia in referencias)
                {
                    detallesReferenciaDisponibles.AddRange(referencia.Detalles);
                }
            }

            return manejaReferencia;
        }


        public virtual void UpdateReferenciaDetalles(IUnitOfWork uow, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            cambiosDetalleReferencia.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleReferencia.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosDetalleReferencia.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;
                dr.FechaModificacion = DateTime.Now;

                uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(dr);
            }

            uow.SaveChanges();
            uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalles(cambiosDetalleReferencia);
        }

        public virtual void UpdateAgendaDetalles(IUnitOfWork uow, EntityChanges<AgendaDetalle> cambiosDetalles)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            cambiosDetalles.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalles.DeletedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalles.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            uow.AgendaRepository.UpdateAgendaDetalles(cambiosDetalles);
        }

        public virtual void UpdateDetalleAgendaReferenciaAsociada(IUnitOfWork uow, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            cambiosAsociaciones.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosAsociaciones.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosAsociaciones.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;

                uow.ReferenciaRecepcionRepository.UpdateDetalleAgendaReferenciaAsociada(dr);
            }

            uow.SaveChanges();
            uow.ReferenciaRecepcionRepository.UpdateDetalleAgendaReferenciaAsociada(cambiosAsociaciones);
        }

        public virtual AgendaDetalle ProcesarDeleteRow(IUnitOfWork uow, GridRow row, Agenda agenda, EntityChanges<AgendaDetalle> cambios)
        {
            var detalle = agenda.Detalles.Where(d => d.IdAgenda == agenda.Id
                                                        && d.IdEmpresa == agenda.IdEmpresa
                                                        && d.CodigoProducto == row.GetCell("CD_PRODUTO").Value
                                                        && d.Faixa == decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider())
                                                        && d.Identificador == row.GetCell("NU_IDENTIFICADOR").Old)
                                               .FirstOrDefault();

            if (detalle != null)
            {
                cambios.DeletedRecords.Add(detalle);
                agenda.Detalles.Remove(detalle);
            }

            return detalle;
        }

        public virtual AgendaDetalle ProcesarAddRow(IUnitOfWork uow, GridRow row, Agenda agenda, EntityChanges<AgendaDetalle> cambios)
        {
            var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(agenda.IdEmpresa, row.GetCell("CD_PRODUTO").Value);

            var detalle = new AgendaDetalle
            {
                IdAgenda = agenda.Id,
                CodigoProducto = row.GetCell("CD_PRODUTO").Value,
                Faixa = 1,
                Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                IdEmpresa = agenda.IdEmpresa,
                CantidadAgendada = decimal.Parse(row.GetCell("QT_AGENDADO").Value, this._identity.GetFormatProvider()),
                CantidadAgendadaOriginal = decimal.Parse(row.GetCell("QT_AGENDADO").Value, this._identity.GetFormatProvider()),
                CantidadAceptada = 0,
                CantidadCrossDocking = 0,
                CantidadRecibida = 0,
                CantidadRecibidaFicticia = 0,
                Estado = EstadoAgendaDetalle.Abierta,
                Vencimiento = producto.GetFechaVencimiento(),
            };

            agenda.Detalles.Add(detalle);

            cambios.AddedRecords.Add(detalle);

            return detalle;
        }

        public virtual AgendaDetalle ProcesarModifiedRow(IUnitOfWork uow, GridRow row, Agenda agenda, EntityChanges<AgendaDetalle> cambios)
        {
            var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());

            var detalle = agenda.Detalles.Where(d => d.IdAgenda == agenda.Id
                                                  && d.IdEmpresa == agenda.IdEmpresa
                                                  && d.CodigoProducto == row.GetCell("CD_PRODUTO").Old
                                                  && d.Faixa == faixa
                                                  && d.Identificador == row.GetCell("NU_IDENTIFICADOR").Old)
                                         .FirstOrDefault();

            if (detalle == null)
                throw new EntityNotFoundException("REC170_Frm1_Error_DetalleAgendaNoExiste");

            detalle.CodigoProducto = row.GetCell("CD_PRODUTO").Value;
            detalle.Identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            detalle.CantidadAgendada = decimal.Parse(row.GetCell("QT_AGENDADO").Value, this._identity.GetFormatProvider());
            detalle.CantidadAgendadaOriginal = decimal.Parse(row.GetCell("QT_AGENDADO").Value, this._identity.GetFormatProvider());

            if (row.GetCell("CD_PRODUTO").Old != row.GetCell("CD_PRODUTO").Value)
            {
                var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(agenda.IdEmpresa, row.GetCell("CD_PRODUTO").Value);
                detalle.Vencimiento = producto.GetFechaVencimiento();
            }

            cambios.UpdatedRecords.Add(detalle);

            return detalle;
        }

        public virtual void ProcesarDeleteRowReferencia(IUnitOfWork uow, AgendaDetalle detalle, List<ReferenciaRecepcionDetalle> detallesReferenciaDisponibles, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            // Traer detalles de referencia afectados y retorna saldo 

            var detallesReferenciaAsociados = uow.ReferenciaRecepcionRepository.GetDetalleAgendaReferenciaAsociada(detalle);

            foreach (var asociacion in detallesReferenciaAsociados)
            {
                //Locker detalles
                this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.Id.ToString());

                var detalleReferencia = detallesReferenciaDisponibles.FirstOrDefault(d => d.Id == asociacion.DetalleReferencia.Id);

                // actualizo cantidad de la referencia
                detalleReferencia.CantidadAgendada -= asociacion.CantidadAgendada;

                // Remover asociación 
                cambiosAsociaciones.DeletedRecords.Add(asociacion);

                // actualizar detalle referencia
                this.AgregarRecordDetalleReferencia(cambiosDetalleReferencia, detalleReferencia);
            }
        }

        public virtual void ProcesarAddRowReferencia(IUnitOfWork uow, AgendaDetalle detalle, List<ReferenciaRecepcionDetalle> detallesReferenciaDisponibles, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            decimal saldoDetalleAgenda = detalle.CantidadAgendada;

            var detallesReferencia = detallesReferenciaDisponibles
                .Where(d => d.IdEmpresa == detalle.IdEmpresa
                    && d.CodigoProducto == detalle.CodigoProducto
                    && d.Faixa == detalle.Faixa
                    && (d.Identificador == detalle.Identificador || d.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                .ToList();

            // Itero en los detalles de referencia con saldo disponible, Lote auto al final
            foreach (var detalleReferencia in detallesReferencia.Where(s => s.GetSaldo() > 0).OrderBy(s => s.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
            {
                // Lockeo detalle
                this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.Id.ToString());

                if (saldoDetalleAgenda == 0)
                    break;

                // Actualizo cantidades y creo asociacion entre los detalles de agenda y referencia
                if (detalleReferencia.GetSaldo() >= saldoDetalleAgenda)
                {
                    detalleReferencia.CantidadAgendada += saldoDetalleAgenda;

                    this.AgregarRecordAsociacion(uow, cambiosAsociaciones, detalle, detalleReferencia, saldoDetalleAgenda);

                    saldoDetalleAgenda = 0;
                }
                else
                {
                    var saldo = detalleReferencia.GetSaldo();

                    saldoDetalleAgenda -= saldo;
                    detalleReferencia.CantidadAgendada += saldo;

                    this.AgregarRecordAsociacion(uow, cambiosAsociaciones, detalle, detalleReferencia, saldo);

                }

                this.AgregarRecordDetalleReferencia(cambiosDetalleReferencia, detalleReferencia);
            }

            if (saldoDetalleAgenda > 0)
                throw new ValidationFailedException("REC170_Frm1_Error_EdicionSinSaldo", new string[] { detalle.CodigoProducto, detalle.Identificador, saldoDetalleAgenda.ToString() });

        }

        public virtual void ProcesarModifiedRowReferencia(IUnitOfWork uow, Agenda agenda, AgendaDetalle detalle, GridRow row, List<ReferenciaRecepcionDetalle> detallesReferenciaDisponibles, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {

            // Verificar si cambia producto o identificador

            if (row.GetCell("CD_PRODUTO").Value != row.GetCell("CD_PRODUTO").Old || row.GetCell("NU_IDENTIFICADOR").Value != row.GetCell("NU_IDENTIFICADOR").Old)
            {
                // El producto o el identificador es diferente

                // Retornar saldos viejos y elimiar asociaciones viejas

                var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
                var detalleOld = agenda.Detalles.Where(d => d.IdAgenda == agenda.Id
                                                 && d.IdEmpresa == agenda.IdEmpresa
                                                 && d.CodigoProducto == row.GetCell("CD_PRODUTO").Old
                                                 && d.Faixa == faixa
                                                 && d.Identificador == row.GetCell("NU_IDENTIFICADOR").Old)
                                        .FirstOrDefault();

                // retorno saldo de asociaciones a detalles y elimino las asociaciones
                this.ProcesarDeleteRowReferencia(uow, detalleOld, detallesReferenciaDisponibles, cambiosAsociaciones, cambiosDetalleReferencia);

                // Crear nuevas asociaciones y consumir saldos referencias
                this.ProcesarAddRowReferencia(uow, detalle, detallesReferenciaDisponibles, cambiosAsociaciones, cambiosDetalleReferencia);

            }
            else
            {
                // Es el mismo producto/idetificador, cambia la cantidad agendada

                // Actualizo asociaciones y detalles de referencias

                decimal diferenciaAgendado = detalle.CantidadAgendada - decimal.Parse(row.GetCell("QT_AGENDADO").Old, this._identity.GetFormatProvider());

                if (diferenciaAgendado > 0)
                {
                    // Diferencia es mayor, consultar saldo y reservar

                    var detallesReferencia = detallesReferenciaDisponibles.Where(d => d.IdEmpresa == detalle.IdEmpresa
                                                                   && d.CodigoProducto == detalle.CodigoProducto
                                                                   && d.Faixa == detalle.Faixa
                                                                   && (d.Identificador == detalle.Identificador || d.Identificador == ManejoIdentificadorDb.IdentificadorAuto)).ToList();

                    foreach (var detalleReferencia in detallesReferencia.Where(s => s.GetSaldo() > 0).OrderBy(s => s.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                    {
                        // Lockeo detalle
                        this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.Id.ToString());

                        if (diferenciaAgendado == 0)
                            break;

                        // Actualizo detalles y asociaciones

                        if (detalleReferencia.GetSaldo() >= diferenciaAgendado)
                        {
                            detalleReferencia.CantidadAgendada += diferenciaAgendado;

                            this.AgregarRecordAsociacion(uow, cambiosAsociaciones, detalle, detalleReferencia, diferenciaAgendado);

                            diferenciaAgendado = 0;
                        }
                        else
                        {
                            var saldo = detalleReferencia.GetSaldo();

                            diferenciaAgendado -= saldo;
                            detalleReferencia.CantidadAgendada += saldo;

                            this.AgregarRecordAsociacion(uow, cambiosAsociaciones, detalle, detalleReferencia, saldo);

                        }

                        this.AgregarRecordDetalleReferencia(cambiosDetalleReferencia, detalleReferencia);

                    }

                    if (diferenciaAgendado > 0)
                        throw new ValidationFailedException("REC170_Frm1_Error_EdicionSinSaldo", new string[] { detalle.CodigoProducto, detalle.Identificador, diferenciaAgendado.ToString() });

                }
                else if (diferenciaAgendado < 0)
                {
                    // si la diferencia es menor retorno saldo 

                    // Obtengo detalles de referencia afectados y retorna saldo 

                    var detallesReferenciaAsociados = uow.ReferenciaRecepcionRepository.GetDetalleAgendaReferenciaAsociada(detalle);

                    foreach (var asociacion in detallesReferenciaAsociados.OrderByDescending(s => s.FechaInsercion))
                    {
                        if (diferenciaAgendado == 0)
                            break;

                        //Locker detalles
                        this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.Id.ToString());

                        var detalleReferencia = detallesReferenciaDisponibles.FirstOrDefault(d => d.Id == asociacion.DetalleReferencia.Id);

                        if ((asociacion.CantidadAgendada + diferenciaAgendado) <= 0)
                        {
                            // La diferenciaAgendado es negativa, si lo resto a la cantidad agendada de la asociacion  
                            // y el resultado es menor o igual a 0 la elimino y retorno el saldo


                            detalleReferencia.CantidadAgendada -= asociacion.CantidadAgendada;
                            diferenciaAgendado += asociacion.CantidadAgendada;

                            // Remover asociacion 
                            cambiosAsociaciones.DeletedRecords.Add(asociacion);

                            // actualizar detalle referencia
                            this.AgregarRecordDetalleReferencia(cambiosDetalleReferencia, detalleReferencia);
                        }
                        else
                        {
                            // Si la cantidad de la asociación sumada la diferencia negativa es mayor a 0 retorno el saldo y actualizo la asociación

                            detalleReferencia.CantidadAgendada += diferenciaAgendado;
                            asociacion.CantidadAgendada += diferenciaAgendado;

                            // actualizo asociación 
                            this.AgregarRecordAsociacion(uow, cambiosAsociaciones, detalle, detalleReferencia, diferenciaAgendado);

                            // actualizar detalle referencia
                            this.AgregarRecordDetalleReferencia(cambiosDetalleReferencia, detalleReferencia);

                            diferenciaAgendado = 0;
                        }

                    }
                }
            }
        }

        public virtual void AgregarRecordDetalleReferencia(EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, ReferenciaRecepcionDetalle detalleReferencia)
        {

            var record = cambiosDetalleReferencia.UpdatedRecords.FirstOrDefault(d => d.Id == detalleReferencia.Id);

            if (record != null)
                cambiosDetalleReferencia.UpdatedRecords.Remove(record);

            cambiosDetalleReferencia.UpdatedRecords.Add(detalleReferencia);

        }

        public virtual void AgregarRecordAsociacion(IUnitOfWork uow, EntityChanges<DetalleAgendaReferenciaAsociada> cambioAsociaciones, AgendaDetalle detalleAgenda, ReferenciaRecepcionDetalle detalleReferencia, decimal cantidad)
        {
            // Agregar nueva o modificacion de existente
            // Verifico que no exista en base si existe la elimino y creo la nueva

            var asociacionExistente = uow.ReferenciaRecepcionRepository.GetDetalleAgendaReferenciaAsociada(detalleAgenda, detalleReferencia);

            if (asociacionExistente != null)
                cambioAsociaciones.DeletedRecords.Add(asociacionExistente);

            cambioAsociaciones.AddedRecords.Add(new DetalleAgendaReferenciaAsociada()
            {
                DetalleAgenda = detalleAgenda,
                DetalleReferencia = detalleReferencia,
                CantidadAgendada = cantidad,
                CantidadRecibida = 0
            });
        }

        public virtual void CheckForDuplicatesOnAgenda(List<AgendaDetalle> cambiosDetalles)
        {
            if (cambiosDetalles.GroupBy(d => new { d.IdEmpresa, d.CodigoProducto, d.Identificador }).Any(d => d.Count() > 1))
                throw new ValidationFailedException("REC170_Sec0_error_LineasDetallesDuplicadas");
        }

        #endregion
    }
}
