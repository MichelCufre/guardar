using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Produccion.Queries;
using WIS.Domain.StockEntities;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD112PlanificacionInsumos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly Dictionary<string, object> GridPlanificacion = new Dictionary<string, object>();
        protected readonly Dictionary<string, object> GridPlanificacionPedido = new Dictionary<string, object>();

        public PRD112PlanificacionInsumos(
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            ILogicaProduccionFactory logicaProduccion,
            ITrafficOfficerService concurrencyControl,
            IIdentityService identity,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            _uowFactory = uowFactory;
            _gridService = gridService;
            _logicaProduccionFactory = logicaProduccion;
            _concurrencyControl = concurrencyControl;
            _identity = identity;
            _excelService = excelService;
            _gridValidationService = gridValidationService;
            _filterInterpreter = filterInterpreter;

            #region Grids Config

            GridPlanificacion.Add("Id", "PRD112_grid_1");
            GridPlanificacion.Add("GridKeys", new List<string> { "CD_ENDERECO", "CD_PRODUTO", "CD_EMPRESA", "CD_FAIXA", "NU_IDENTIFICADOR" });
            GridPlanificacion.Add("DefaultSort", new List<SortCommand> { new SortCommand("REQUERIDO", SortDirection.Descending) });
            GridPlanificacion.Add("DefaultHiddenColumns", new List<string> { "CD_ENDERECO", "CD_EMPRESA", "CD_FAIXA" });
            GridPlanificacion.Add("EditableCells", new List<string> { "QT_RESERVAR", "QT_PEDIR" });
            GridPlanificacion.Add("InsertableColumns", new List<string> { "CD_PRODUTO", "NU_IDENTIFICADOR", "QT_PEDIR" });


            GridPlanificacionPedido.Add("Id", "PRD112_grid_2");
            GridPlanificacionPedido.Add("GridKeys", new List<string> { "CD_PRODUTO", "CD_EMPRESA", "CD_FAIXA", "NU_IDENTIFICADOR" });
            GridPlanificacionPedido.Add("DefaultSort", new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Descending) });
            GridPlanificacionPedido.Add("DefaultHiddenColumns", new List<string> { "CD_ENDERECO", "CD_EMPRESA", "CD_FAIXA" });
            GridPlanificacionPedido.Add("EditableCells", new List<string> { "QT_PEDIR" });
            GridPlanificacionPedido.Add("InsertableColumns", new List<string> { "CD_PRODUTO", "NU_IDENTIFICADOR", "QT_PEDIR" });

            #endregion
        }

        public override PageContext PageLoad(PageContext data)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var ingreso = uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(data.GetParameter("nuIngresoProduccion"));

            data.AddOrUpdateParameter("tieneEspacioAsignado", ingreso.EspacioProduccion == null ? "N" : "S");
            data.AddOrUpdateParameter("nuIngresoProduccion", data.GetParameter("nuIngresoProduccion"));

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = true;

            if (grid.Id == (string)GridPlanificacion["Id"])
                grid.SetInsertableColumns((List<string>)GridPlanificacion["InsertableColumns"]);
            else
                grid.SetInsertableColumns((List<string>)GridPlanificacionPedido["InsertableColumns"]);

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var idIngreso = context.GetParameter("idIngreso");

            using var uow = _uowFactory.GetUnitOfWork();

            var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, idIngreso);
            var ingreso = logicaProduccion.GetIngresoProduccion();

            if (grid.Id == (string)GridPlanificacion["Id"])
            {
                var dbQuery = new PlanificacionInsumosQuery(idIngreso, ingreso.Empresa.Value, this._identity.GetFormatProvider());

                context.Parameters.Add(new ComponentParameter()
                {
                    Id = "empresa",
                    Value = ingreso.Empresa?.ToString()
                });
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, (List<SortCommand>)GridPlanificacion["DefaultSort"], (List<string>)GridPlanificacion["GridKeys"]);

                grid.Columns.ForEach(column => column.Hidden = ((List<string>)GridPlanificacion["DefaultHiddenColumns"]).Contains(column.Id));

                grid.SetEditableCells((List<string>)GridPlanificacion["EditableCells"]);

                foreach (var row in grid.Rows)
                {
                    var cantidadReserva = row.GetCell("QT_RESERVAR");
                    var cantidadAPedir = row.GetCell("QT_PEDIR");
                    var cantidadDisponible = row.GetCell("QT_DISPONIBLE");
                    var requerido = row.GetCell("REQUERIDO");

                    cantidadAPedir.Value = string.Empty;

                    if (cantidadDisponible.Value == "0" && cantidadReserva.Value == "0" && requerido.Value == "S")
                        cantidadReserva.Editable = false;

                    if (decimal.Parse(cantidadReserva.Value, _identity.GetFormatProvider()) > 0)
                        cantidadReserva.ForceSetOldValue("0");

                    if (requerido.Value == "N")
                        cantidadAPedir.Editable = false;

                    decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_RESERVAR").Value, this._identity.GetFormatProvider(), out decimal qtReserva);
                    decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PENDIENTE").Value, this._identity.GetFormatProvider(), out decimal qtPendiente);

                    decimal cantidad = 0;
                    if (qtPendiente - qtReserva > 0)
                    {
                        cantidad = qtPendiente - qtReserva;
                    }
                    var cantidadPedirSugerida = row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PEDIR_SUGERIDA");
                    cantidadPedirSugerida.Value = cantidad.ToString(this._identity.GetFormatProvider());
                    cantidadPedirSugerida.CssClass = "lightGreen";

                }

            }
            else
            {
                var dbQuery = new PlanificacionPedidoQuery(idIngreso, ingreso.Empresa.Value, this._identity.GetFormatProvider());

                context.Parameters.Add(new ComponentParameter()
                {
                    Id = "empresa",
                    Value = ingreso.Empresa?.ToString()
                });
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, (List<SortCommand>)GridPlanificacionPedido["DefaultSort"], (List<string>)GridPlanificacionPedido["GridKeys"]);

                grid.Columns.ForEach(column => column.Hidden = ((List<string>)GridPlanificacionPedido["DefaultHiddenColumns"]).Contains(column.Id));

                grid.SetEditableCells((List<string>)GridPlanificacionPedido["EditableCells"]);
                foreach (var row in grid.Rows)
                {
                    var cantidadAPedir = row.GetCell("QT_PEDIR");
                    cantidadAPedir.Value = string.Empty;

                    var cantidadPedirSugerida = row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PENDIENTE");
                    cantidadPedirSugerida.CssClass = "lightGreen";

                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var idIngreso = context.GetParameter("idIngreso");
                var empresa = uow.ProduccionRepository.GetIngreso(idIngreso).Empresa.Value;

                if (grid.Id == (string)GridPlanificacion["Id"])
                {
                    var dbQuery = new PlanificacionInsumosQuery(idIngreso, empresa, this._identity.GetFormatProvider());

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }
                else
                {
                    var dbQuery = new PlanificacionPedidoQuery(idIngreso, empresa, this._identity.GetFormatProvider());

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("idIngreso");
            var empresa = uow.ProduccionRepository.GetIngreso(idIngreso).Empresa.Value;

            if (grid.Id == (string)GridPlanificacion["Id"])
            {
                var dbQuery = new PlanificacionInsumosQuery(idIngreso, empresa, this._identity.GetFormatProvider());

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                var dbQuery = new PlanificacionPedidoQuery(idIngreso, empresa, this._identity.GetFormatProvider());

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };

            }

        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("idIngreso");
            var empresa = int.Parse(context.GetParameter("empresa"));
            var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, idIngreso);

            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", idIngreso, true))
            {
                context.AddErrorNotification("General_msg_Error_ProduccionBloqueada");
                return grid;
            }

            var transaction = _concurrencyControl.CreateTransaccion();
            
            try
            {
                var ingreso = logicaProduccion.GetIngresoProduccion();

                if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA || ingreso.Situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL)
                    throw new ValidationFailedException("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                uow.BeginTransaction();
                uow.CreateTransactionNumber("GridCommit Temp Pedido Prod");

                _concurrencyControl.AddLock("T_PRDC_INGRESO", idIngreso, transaction, true);

                if (grid.Id == (string)GridPlanificacion["Id"])
                {
                    GridPlanificacionCommit(grid, context, uow, idIngreso, empresa, logicaProduccion, transaction);
                }
                else
                {
                    GridPlanificacionPedidoCommit(grid, context, uow, idIngreso, empresa, logicaProduccion, transaction);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Db_Success_Update");

                return grid;
            }
            catch (EntityLockedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (ValidationFailedException ex)
            {
                _logger.Debug($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (System.Exception ex)
            {
                _logger.Debug($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message);
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transaction);
            }
            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var empresa = int.Parse(context.GetParameter("empresa"));
            var idIngreso = context.GetParameter("idIngreso");

            var planificacionPedido = (grid.Id == (string)GridPlanificacionPedido["Id"]);

            return this._gridValidationService.Validate(new DetallePlanificacionProduccionGridValidationModule(uow, this._identity.GetFormatProvider(), empresa, idIngreso, planificacionPedido), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));
            switch (context.ColumnId)
            {
                case "CD_PRODUTO":
                    return this.SearchProducto(grid, context, empresa);
            }

            return new List<SelectOption>();
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var idIngreso = context.GetParameter("idIngreso");

            using var uow = _uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(idIngreso))
            {
                var ingreso = uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(idIngreso);
                var descTpProduccion = uow.DominioRepository.GetDominios("TPINGPR").FirstOrDefault(f => f.Id == ingreso.Tipo).Valor;

                form.GetField("idProduccion").Value = ingreso.Id;
                form.GetField("idProduccionExterno").Value = ingreso.IdProduccionExterno;
                form.GetField("tpProduccion").Value = descTpProduccion;

                if (ingreso.EspacioProduccion != null)
                {
                    form.GetField("idEspacio").Value = ingreso.EspacioProduccion.Id;
                    form.GetField("dsEspacio").Value = ingreso.EspacioProduccion.Descripcion;
                    form.GetField("nuProduccionEspacio").Value = ingreso.PosicionEnCola.ToString();
                }

                form.Fields.ForEach(f => f.ReadOnly = true);
            }

            return form;
        }

        #region Metodos Auxiliares

        public virtual void GridPlanificacionCommit(Grid grid, GridFetchContext context, IUnitOfWork uow, string idIngreso, int empresa, ILogicaProduccion logicaProduccion, TrafficOfficerTransaction transaction)
        {
            List<IngresoProduccionDetallePedidoTemporal> detalles = new List<IngresoProduccionDetallePedidoTemporal>();

            var dbQuery = new PlanificacionInsumosQuery(idIngreso, empresa, this._identity.GetFormatProvider());

            uow.HandleQuery(dbQuery);

            detalles = dbQuery.GetResult().Where(x => x.REQUERIDO == "S").Select(x => MapRowToObject(uow, new IngresoProduccionDetallePedidoTemporal
            {
                IdIngreso = x.NU_PRDC_INGRESO,
                Ubicacion = x.CD_ENDERECO,
                Producto = x.CD_PRODUTO,
                Empresa = int.Parse(context.GetParameter("empresa")),
                Faixa = x.CD_FAIXA,
                Lote = x.NU_IDENTIFICADOR,
                CantidadReserva = x.QT_RESERVA,
                CantidadReservar = x.QT_RESERVAR,
                CantidadAPedir = x.QT_PEDIR,
                CantidadPendiente = x.QT_PENDIENTE
            })).ToList();

            grid.Rows.Where(w => w.IsModified && !w.IsNew).ToList().ForEach(row =>
            {
                var detalle = MapRowToObject(uow, row, empresa);
                var det = detalles.FirstOrDefault(x => x.IdIngreso == detalle.IdIngreso && x.Lote == detalle.Lote && x.Producto == detalle.Producto && x.Ubicacion == detalle.Ubicacion);
                if (det != null)
                {
                    detalles.Remove(det);
                }
                detalles.Add(detalle);
            });

            grid.Rows.Where(w => w.IsDeleted).ToList().ForEach(row =>
            {
                var detalle = MapRowToObject(uow, row, empresa);
                var det = detalles.FirstOrDefault(x => x.IdIngreso == detalle.IdIngreso && x.Lote == detalle.Lote && x.Producto == detalle.Producto && x.Ubicacion == detalle.Ubicacion);
                if (det != null)
                {
                    detalles.Remove(det);
                }
            });

            grid.Rows.Where(w => w.IsNew).ToList().ForEach(row =>
            {
                var detalle = MapRowToObject(uow, row, empresa);
                if (detalles.Any(x => x.Producto == detalle.Producto && x.Lote == detalle.Lote))
                {
                    throw new ValidationFailedException("PRD112_Sec0_Error_Er003_DetalleExistente");
                }
                detalle.dtAddrow = DateTime.Now;
                detalles.Add(detalle);
            });

            var detallesAReservar = detalles.Where(x => x.CantidadReservar != null).ToList();
            var detallesAPedir = detalles.Where(x => x.CantidadAPedir > 0).ToList();

            AgregarBloqueos(uow, transaction, detallesAReservar);

            GenerarReservaProduccion(uow, detalles, logicaProduccion, idIngreso);

            GenerarPedido(context, uow, logicaProduccion, detallesAPedir);
        }

        public virtual void GridPlanificacionPedidoCommit(Grid grid, GridFetchContext context, IUnitOfWork uow, string idIngreso, int empresa, ILogicaProduccion logicaProduccion, TrafficOfficerTransaction transaction)
        {
            List<IngresoProduccionDetallePedidoTemporal> detalles = new List<IngresoProduccionDetallePedidoTemporal>();

            var dbQuery = new PlanificacionPedidoQuery(idIngreso, empresa, this._identity.GetFormatProvider());

            uow.HandleQuery(dbQuery);
            detalles = dbQuery.GetResult().Select(x => MapRowToObject(uow, new IngresoProduccionDetallePedidoTemporal
            {
                IdIngreso = x.NU_PRDC_INGRESO,
                Faixa = x.CD_FAIXA ?? 1,
                Producto = x.CD_PRODUTO,
                Empresa = int.Parse(context.GetParameter("empresa")),
                Lote = x.NU_IDENTIFICADOR,
                CantidadAPedir = x.QT_PEDIR,
                CantidadPendiente = x.QT_PENDIENTE
            })).ToList();

            grid.Rows.Where(w => w.IsModified && !w.IsNew).ToList().ForEach(row =>
            {
                var detalle = MapRowToObject(uow, row, empresa);
                var det = detalles.FirstOrDefault(x => x.IdIngreso == detalle.IdIngreso && x.Lote == detalle.Lote && x.Producto == detalle.Producto && x.Ubicacion == detalle.Ubicacion);
                if (det != null)
                {
                    detalles.Remove(det);
                }
                detalles.Add(detalle);
            });

            grid.Rows.Where(w => w.IsDeleted).ToList().ForEach(row =>
            {
                var detalle = MapRowToObject(uow, row, empresa);
                var det = detalles.FirstOrDefault(x => x.IdIngreso == detalle.IdIngreso && x.Lote == detalle.Lote && x.Producto == detalle.Producto && x.Ubicacion == detalle.Ubicacion);
                if (det != null)
                {
                    detalles.Remove(det);
                }
            });

            grid.Rows.Where(w => w.IsNew).ToList().ForEach(row =>
            {
                var detalle = MapRowToObject(uow, row, empresa);
                if (detalles.Any(x => x.Producto == detalle.Producto && x.Lote == detalle.Lote))
                {
                    throw new ValidationFailedException("PRD112_Sec0_Error_Er003_DetalleExistente");
                }
                detalle.dtAddrow = DateTime.Now;
                detalles.Add(detalle);
            });

            GenerarPedido(context, uow, logicaProduccion, detalles);
        }

        public virtual void GenerarPedido(GridFetchContext context, IUnitOfWork uow, ILogicaProduccion logicaProduccion, List<IngresoProduccionDetallePedidoTemporal> detalles)
        {
            if (context.Parameters.FirstOrDefault(x => x.Id == "btnGenerarPedidoLiberar").Value == "S" || context.Parameters.FirstOrDefault(x => x.Id == "btnGenerarPedido").Value == "S")
            {
                if (detalles.Any())
                {
                    ValidateRows(uow, detalles);

                    var pedido = logicaProduccion.GenerarPedido(detalles);

                    uow.SaveChanges();

                    if (context.Parameters.FirstOrDefault(x => x.Id == "btnGenerarPedidoLiberar").Value == "S")
                    {
                        GenerarPreparacion(uow, logicaProduccion, pedido);
                    }
                }
            }
        }

        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext context, int cdEmpresa)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            List<SelectOption> opciones = new List<SelectOption>();

            List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(cdEmpresa, context.SearchValue);

            foreach (Producto producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} - {producto.Descripcion}"));
            }

            return opciones;
        }

        public virtual void AgregarBloqueos(IUnitOfWork uow, TrafficOfficerTransaction transaction, List<IngresoProduccionDetallePedidoTemporal> detalles)
        {
            var keysStock = detalles
               .GroupBy(s => new { s.Ubicacion, s.Empresa, s.Producto, s.Faixa, s.Lote })
               .Select(s => new Stock()
               {
                   Ubicacion = s.Key.Ubicacion,
                   Empresa = s.Key.Empresa,
                   Producto = s.Key.Producto,
                   Faixa = s.Key.Faixa,
                   Identificador = s.Key.Lote,
                   NumeroTransaccion = uow.GetTransactionNumber(),
               })
              .ToList();

            var idsBloqueos = uow.StockRepository.GetStock(keysStock)
               .GroupBy(s => new { s.Ubicacion, s.Empresa, s.Producto, s.Faixa, s.Identificador })
               .Select(s => new Stock()
               {
                   Ubicacion = s.Key.Ubicacion,
                   Empresa = s.Key.Empresa,
                   Producto = s.Key.Producto,
                   Faixa = s.Key.Faixa,
                   Identificador = s.Key.Identificador
               })
               .Select(s => s.GetLockId(_identity.GetFormatProvider()))
               .ToList();

            if (idsBloqueos.Count > 0)
            {
                var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transaction);

                if (listLock.Count > 0)
                {
                    var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD113_msg_Error_StockBloqueada", new string[] { keyBloqueo[2], keyBloqueo[4] });
                }

                this._concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transaction, true);
            }
        }

        public virtual void GenerarReservaProduccion(IUnitOfWork uow, List<IngresoProduccionDetallePedidoTemporal> detalles, ILogicaProduccion logicaProduccion, string idIngreso)
        {
            foreach (var detalle in detalles)
            {
                decimal cantidadReservada = uow.ProduccionRepository.GetCantidadReservaInsumosSumados(idIngreso, detalle.Producto, detalle.Lote, detalle.Empresa, detalle.Faixa, detalle.Ubicacion);
                decimal diferencia = (detalle.CantidadReservar ?? 0) - cantidadReservada;

                if (diferencia > 0)
                {
                    logicaProduccion.AfectarSobrantes(detalle.Producto, detalle.Lote, detalle.Empresa, detalle.Faixa, diferencia, detalle.Ubicacion);
                }
                else if (diferencia < 0)
                {
                    logicaProduccion.DesafectarSobrantes(detalle.Producto, detalle.Lote, detalle.Empresa, detalle.Faixa, diferencia * -1, detalle.Ubicacion);
                }
            }
        }

        public virtual void GenerarPreparacion(IUnitOfWork uow, ILogicaProduccion logicaProduccion, Pedido pedido)
        {
            var codigoContenedorValidado = uow.ParametroRepository.GetParameter("PRDC_PED_CD_CON_VAL");
            var onda = short.Parse(uow.ParametroRepository.GetParameter("PRD112_LIB_ONDA"));
            var agrupacion = uow.ParametroRepository.GetParameter("PRD112_LIB_AGRUPACION");
            var respetarFifoEnLoteAUTO = uow.ParametroRepository.GetParameter("PRD112_LIB_RESP_FIFO_AUTO") == "S" ? true : false;
            var controlaStockDocumental = uow.ParametroRepository.GetParameter("PRD112_LIB_CTRL_STK_DOCUMENTAL") == "S" ? true : false;
            var cursorStock = uow.ParametroRepository.GetParameter("PRD112_LIB_CURSOR_STOCK");
            var cursorPedido = uow.ParametroRepository.GetParameter("PRD112_LIB_CURSOR_PEDIDO");
            var debeLiberarPorCurvas = uow.ParametroRepository.GetParameter("PRD112_LIB_LIBERAR_CURVAS") == "S" ? true : false;
            var debeLiberarPorUnidades = uow.ParametroRepository.GetParameter("PRD112_LIB_LIBERAR_UNIDADES") == "S" ? true : false;
            var repartirEscasez = uow.ParametroRepository.GetParameter("PRD112_LIB_REPARTIR_ESCASEZ");
            var pickingAgrupCamion = uow.ParametroRepository.GetParameter("PRD112_LIB_AGRUP_CAMION") == "S" ? true : false;
            var prepararSoloConCamion = uow.ParametroRepository.GetParameter("PRD112_LIB_PREP_SOLO_CAMION") == "S" ? true : false;
            var modalPalletCompleto = uow.ParametroRepository.GetParameter("PRD112_LIB_MODO_PALLET_COMPLEO");
            var modalPalletIncompleto = uow.ParametroRepository.GetParameter("PRD112_LIB_MODO_PALLET_INCO");
            var priorizarDesborde = uow.ParametroRepository.GetParameter("PRD112_LIB_PRIORIZAR_DESBORDE") == "S" ? true : false;
            var manejaVidaUtil = uow.ParametroRepository.GetParameter("PRD112_LIB_MANEJA_VIDA_UTIL") == "S" ? true : false;
            var requiereUbicacion = uow.ParametroRepository.GetParameter("PRD112_LIB_PICKING_DOS_FACES") == "S" ? true : false;
            var excluirPicking = uow.ParametroRepository.GetParameter("PRD112_LIB_EXCLUIR_PICKING") == "S" ? true : false;

            var ingreso = logicaProduccion.GetIngresoProduccion();

            var preparacion = new Preparacion()
            {
                Descripcion = $"Lib Fabricación: {ingreso.IdProduccionExterno} Ped: {pedido.Id}".Truncate(60),
                Empresa = logicaProduccion.GetEmpresa(),
                Onda = onda,
                Agrupacion = agrupacion,
                RespetarFifoEnLoteAUTO = respetarFifoEnLoteAUTO,
                ControlaStockDocumental = controlaStockDocumental,
                CursorStock = cursorStock,
                DebeLiberarPorCurvas = debeLiberarPorCurvas,
                DebeLiberarPorUnidades = debeLiberarPorUnidades,
                RepartirEscasez = repartirEscasez,
                PickingEsAgrupadoPorCamion = pickingAgrupCamion,
                PrepararSoloConCamion = prepararSoloConCamion,
                ModalPalletCompleto = modalPalletCompleto,
                ModalPalletIncompleto = modalPalletIncompleto,
                CursorPedido = cursorPedido,
                UsarSoloStkPicking = priorizarDesborde,
                ManejaVidaUtil = manejaVidaUtil,
                RequiereUbicacion = requiereUbicacion,
                FechaInicio = DateTime.Now,
                ExcluirUbicacionesPicking = excluirPicking,
                Predio = logicaProduccion.GetPredio(),
                Transaccion = uow.GetTransactionNumber(),
                Tipo = TipoPreparacionDb.Normal,
                Usuario = _identity.UserId,
                Situacion = SituacionDb.PreparacionPendiente,
                AceptaMercaderiaAveriada = false,
                PermitePickVencido = false,
                ValidarProductoProveedor = false,
            };

            int nuPreparacion = uow.PreparacionRepository.AddPreparacion(preparacion);

            pedido.NumeroOrdenLiberacion = 0;
            pedido.PreparacionProgramada = nuPreparacion;

            logicaProduccion.UpdatePedido(pedido);
        }

        public virtual IngresoProduccionDetallePedidoTemporal MapRowToObject(IUnitOfWork uow, GridRow row, int empresa)
        {
            decimal.TryParse(row.GetCell("QT_PEDIR").Value, _identity.GetFormatProvider(), out decimal cantidadPedir);
            decimal.TryParse(row.GetCell("QT_PENDIENTE").Value.ToString(), _identity.GetFormatProvider(), out decimal cantidadPendiente);
            decimal.TryParse(row.GetCell("QT_RESERVAR")?.Value.ToString(), _identity.GetFormatProvider(), out decimal cantidadReservar);
            decimal.TryParse(row.GetCell("QT_RESERVA")?.Value.ToString(), _identity.GetFormatProvider(), out decimal cantidadReserva);

            return new IngresoProduccionDetallePedidoTemporal
            {
                Ubicacion = row.GetCell("CD_ENDERECO")?.Value,
                IdIngreso = row.GetCell("NU_PRDC_INGRESO").Value,
                Producto = row.GetCell("CD_PRODUTO").Value,
                Empresa = empresa,
                Faixa = string.IsNullOrEmpty(row.GetCell("CD_FAIXA")?.Value) ? 1 : decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider()),
                Lote = row.GetCell("NU_IDENTIFICADOR").Value,
                CantidadReserva = cantidadReserva,
                CantidadAPedir = cantidadPedir > 0 ? cantidadPedir : ((cantidadPendiente - cantidadReservar < 0) ? 0 : (cantidadPendiente - cantidadReservar)),
                CantidadReservar = !string.IsNullOrEmpty(row.GetCell("QT_RESERVA")?.Value.ToString()) ? cantidadReservar : null
            };
        }

        public virtual IngresoProduccionDetallePedidoTemporal MapRowToObject(IUnitOfWork uow, IngresoProduccionDetallePedidoTemporal detalleQuery)
        {
            return new IngresoProduccionDetallePedidoTemporal
            {
                Ubicacion = detalleQuery.Ubicacion,
                IdIngreso = detalleQuery.IdIngreso,
                Producto = detalleQuery.Producto,
                Empresa = detalleQuery.Empresa,

                Faixa = detalleQuery.Faixa,
                Lote = detalleQuery.Lote,
                CantidadReserva = detalleQuery.CantidadReserva,
                CantidadAPedir = detalleQuery.CantidadAPedir > 0 ? detalleQuery.CantidadAPedir : ((detalleQuery.CantidadPendiente - (detalleQuery.CantidadReservar ?? 0) < 0) ? 0 : (detalleQuery.CantidadPendiente - (detalleQuery.CantidadReservar ?? 0))),
                CantidadReservar = detalleQuery.CantidadReservar
            };
        }

        public virtual void ValidateRows(IUnitOfWork uow, List<IngresoProduccionDetallePedidoTemporal> detalles)
        {
            var insumos = new List<string>();

            foreach (var detalle in detalles)
            {
                var producto = uow.ProductoRepository.GetProducto(detalle.Empresa, detalle.Producto);

                if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                    detalle.Lote = ManejoIdentificadorDb.IdentificadorProducto;
                else if (string.IsNullOrEmpty(detalle.Lote))
                    detalle.Lote = ManejoIdentificadorDb.IdentificadorAuto;

                var key = $"{detalle.Producto}.{detalle.Lote}";
                var keyAuto = $"{detalle.Producto}.{ManejoIdentificadorDb.IdentificadorAuto}";

                if (insumos.Contains(key))
                    throw new ValidationFailedException("PRD110_grid_error_ProductosRepetidos");
                else if (insumos.Contains(keyAuto))
                    throw new ValidationFailedException("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido");
                else if (detalle.Lote == ManejoIdentificadorDb.IdentificadorAuto && insumos.Any(i => i.Contains(detalle.Producto)))
                    throw new ValidationFailedException("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido");
                else
                    insumos.Add(key);
            }
        }

        #endregion
    }
}
