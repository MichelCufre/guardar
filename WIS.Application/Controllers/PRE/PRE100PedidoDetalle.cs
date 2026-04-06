using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100PedidoDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }
        protected Dictionary<string, Func<GridRow, Grid, GridSelectSearchContext, List<SelectOption>>> SelectSearchList { get; }

        public PRE100PedidoDetalle(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            ISecurityService security,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA", "ID_ESPECIFICA_IDENTIFICADOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this.SelectSearchList = new Dictionary<string, Func<GridRow, Grid, GridSelectSearchContext, List<SelectOption>>>
            {
                ["CD_PRODUTO"] = this.SearchProducto
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._security = security;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var codigoEmpresa = int.Parse(context.GetParameter("empresa"));
            var codigoCliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            using var uow = this._uowFactory.GetUnitOfWork();

            var pedido = uow.PedidoRepository.GetPedido(codigoEmpresa, codigoCliente, nuPedido);
			var empresa = uow.EmpresaRepository.GetEmpresa(codigoEmpresa);
			var cliente = uow.AgenteRepository.GetAgente(codigoEmpresa, codigoCliente);

			context.AddParameter("empresaNombre", empresa.Nombre);

			context.AddParameter("agenteDescripcion", cliente.Descripcion);
			context.AddParameter("agenteCodigo", cliente.Codigo);
			context.AddParameter("agenteTipo", cliente.Tipo);

			context.IsAddEnabled = pedido.IsManual;
            context.IsRemoveEnabled = pedido.IsManual;
            context.IsCommitEnabled = pedido.IsManual;
            context.IsEditingEnabled = pedido.IsManual;

            if (pedido.IsManual)
            {
                grid.SetInsertableColumns(new List<string>
                {
                    "CD_PRODUTO",
                    "NU_IDENTIFICADOR",
                    "QT_PEDIDO"
                });

                grid.SetEditableCells(new List<string>
                {
                    "QT_PEDIDO",
                    "NU_IDENTIFICADOR"
                });
            }
            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var pedido = context.GetParameter("pedido");

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new DetallePedidoPanelQuery(empresa, cliente, pedido);

            uow.HandleQuery(dbQuery);

            grid.Rows.AddRange(_gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys));

            var editAllowed = this._security.IsUserAllowed("WPRE101_Page_Access_Puede_Modificar");

            foreach (var row in grid.Rows)
            {
                if (!AnyDuplicado(uow, row, empresa, cliente, pedido))
                {
                    var qtLpn = !string.IsNullOrEmpty(row.GetCell("QT_UTILIZADO_LPN").Value) ? decimal.Parse(row.GetCell("QT_UTILIZADO_LPN").Value, _identity.GetFormatProvider()) : 0;

                    if (row.GetCell("ID_MANEJO_IDENTIFICADOR").Value != "P" && qtLpn <= 0)
                        row.GetCell("NU_IDENTIFICADOR").Editable = true;
                    else
                        row.GetCell("NU_IDENTIFICADOR").Editable = false;

                    if (editAllowed)
                        row.GetCell("QT_PEDIDO").Editable = true;
                }
                else
                {
                    row.GetCell("NU_IDENTIFICADOR").Editable = false;
                    row.GetCell("QT_PEDIDO").Editable = false;
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(query.GetParameter("empresa"));
            var cliente = query.GetParameter("cliente");
            var pedido = query.GetParameter("pedido");

            var dbQuery = new DetallePedidoPanelQuery(empresa, cliente, pedido);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);
            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var pedido = context.GetParameter("pedido");

            var dbQuery = new DetallePedidoPanelQuery(empresa, cliente, pedido);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
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

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("Modificacion lineas pedido");
            uow.BeginTransaction();

            try
            {
                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                var empresa = int.Parse(context.GetParameter("empresa"));
                var cliente = context.GetParameter("cliente");
                var nuPedido = context.GetParameter("pedido");

                var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

                this._concurrencyControl.AddLock("T_PEDIDO_SAIDA", pedido.GetLockId());

                if (!pedido.IsManual)
                    throw new ValidationFailedException("WPRE101_Sec0_Error_Er011");

                if (!pedido.PuedeModificarse())
                    throw new ValidationFailedException("General_Sec0_Error_Er092_SituacionNoPermiteEdicion");

                foreach (var row in grid.Rows)
                {
                    if (row.IsDeleted)
                        EliminarLinea(uow, pedido, row);
                    else if (row.IsNew)
                        AgregarLinea(uow, pedido, row);
                    else
                        ModificarLinea(uow, pedido, row);
                }

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var pedido = context.GetParameter("pedido");

            return this._gridValidationService.Validate(new DetallePedidoGridValidationModule(uow, this._identity.GetFormatProvider(), empresa, cliente, pedido), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            return this.SelectSearchList[context.ColumnId](row, grid, context);
        }

        #region Auxs

        public virtual void EliminarLinea(IUnitOfWork uow, Pedido pedido, GridRow row)
        {
            var producto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

            var detalle = pedido.Lineas.Where(d => d.Producto == producto && d.Identificador == identificador).FirstOrDefault();

            if (detalle == null)
                throw new ValidationFailedException("WPRE101_Sec0_Error_Er005_DetalleNoEncontrado");

            if (detalle.TieneCantidadLiberadaOAnulada())
                throw new ValidationFailedException("PRE100_msg_Error_NoSePuedeEliminarDetallePedido");

            var qtLpn = !string.IsNullOrEmpty(row.GetCell("QT_UTILIZADO_LPN").Value) ? decimal.Parse(row.GetCell("QT_UTILIZADO_LPN").Value, _identity.GetFormatProvider()) : 0;

            if (qtLpn > 0)
                throw new ValidationFailedException("PRE101_Sec0_Error_NoSePuedeEliminarQtUtilizadaLpn");

            var nuTransaccion = uow.GetTransactionNumber();

            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = nuTransaccion;
            detalle.TransaccionDelete = nuTransaccion;

            uow.PedidoRepository.UpdateDetallePedido(detalle);
            uow.SaveChanges();

            uow.PedidoRepository.DeleteDetallePedido(detalle);

            pedido.Lineas.Remove(detalle);
        }

        public virtual void AgregarLinea(IUnitOfWork uow, Pedido pedido, GridRow row)
        {
            var productoId = row.GetCell("CD_PRODUTO").Value;
            var identificador = string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value) ? ManejoIdentificadorDb.IdentificadorAuto : row.GetCell("NU_IDENTIFICADOR").Value;

            if (pedido.Lineas.Any(d => d.Producto == productoId && d.Identificador == identificador))
                throw new ValidationFailedException("WPRE101_Sec0_Error_Er003_DetalleExistente");

            var cantidad = decimal.Parse(row.GetCell("QT_PEDIDO").Value, _identity.GetFormatProvider());
            decimal.TryParse(row.GetCell("QT_ANULADO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal cantidadAnulada);
            decimal.TryParse(row.GetCell("QT_LIBERADO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal cantidadLiberada);

            var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(pedido.Empresa, productoId);

            var detalle = new DetallePedido
            {
                Id = pedido.Id,
                Cliente = pedido.Cliente,
                Empresa = pedido.Empresa,
                Producto = productoId,
                Cantidad = cantidad,
                CantidadOriginal = cantidad,
                Identificador = identificador.Trim(),
                Agrupacion = Agrupacion.Pedido,
                CantidadAnulada = cantidadAnulada,
                CantidadLiberada = cantidadLiberada,
                EspecificaIdentificador = Producto.EspecificaIdentificador(identificador),
                FechaAlta = DateTime.Now,
                Faixa = 1,
                Transaccion = uow.GetTransactionNumber()
            };

            uow.PedidoRepository.AddDetallePedido(detalle);

            pedido.Lineas.Add(detalle);
        }

        public virtual void ModificarLinea(IUnitOfWork uow, Pedido pedido, GridRow row)
        {
            var producto = row.GetCell("CD_PRODUTO").Value;
            var identificadorActual = row.GetCell("NU_IDENTIFICADOR").Old;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

            var detalle = pedido.Lineas.Where(d => d.Producto == producto && d.Identificador == identificadorActual).FirstOrDefault();

            if (detalle.TieneCantidadLiberadaOAnulada())
                throw new ValidationFailedException("PRE100_msg_Error_NoSePuedeModificarDetallePedido");

            var cantidad = decimal.Parse(row.GetCell("QT_PEDIDO").Value, _identity.GetFormatProvider());
            var qtLpn = !string.IsNullOrEmpty(row.GetCell("QT_UTILIZADO_LPN").Value) ? decimal.Parse(row.GetCell("QT_UTILIZADO_LPN").Value, _identity.GetFormatProvider()) : 0;

            if (cantidad < qtLpn)
                throw new ValidationFailedException("PRE101_Sec0_Error_NoSePuedeModificarQtUtilizadaLpn", new string[] { qtLpn.ToString() });

            var nuTransaccion = uow.GetTransactionNumber();

            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = nuTransaccion;
            detalle.TransaccionDelete = nuTransaccion;

            uow.PedidoRepository.UpdateDetallePedido(detalle);
            uow.SaveChanges();

            uow.PedidoRepository.DeleteDetallePedido(detalle);

            detalle.Identificador = identificador;
            detalle.EspecificaIdentificador = Producto.EspecificaIdentificador(identificador);
            detalle.Cantidad = decimal.Parse(row.GetCell("QT_PEDIDO").Value, _identity.GetFormatProvider());
            detalle.TransaccionDelete = null;

            uow.PedidoRepository.AddDetallePedido(detalle);
        }

        public virtual List<SelectOption> SearchProducto(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));

            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

            foreach (Producto producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
            }

            return opciones;
        }

        public virtual bool AnyDuplicado(IUnitOfWork uow, GridRow row, int empresa, string cliente, string pedido)
        {
            var producto = row.GetCell("CD_PRODUTO").Value;
            var lote = row.GetCell("NU_IDENTIFICADOR").Value;
            var espIdentificador = row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value;

            return uow.PedidoRepository.AnyDuplicado(pedido, empresa, cliente, producto, lote, espIdentificador);
        }

        #endregion
    }
}
