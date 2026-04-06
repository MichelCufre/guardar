using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
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
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Sorting;
using WIS.Validation;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100DetallePedidoLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;
        protected readonly IParameterService _paramService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE100DetallePedidoLpn(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            IParameterService paramService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA", "ID_ESPECIFICA_IDENTIFICADOR", "TP_LPN_TIPO", "ID_LPN_EXTERNO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            _paramService = paramService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var empresaId = int.Parse(context.GetParameter("empresa"));
            var clienteId = context.GetParameter("cliente");
            var pedidoId = context.GetParameter("pedido");

            using var uow = this._uowFactory.GetUnitOfWork();

            var editable = PedidoEditable(uow, empresaId, clienteId, pedidoId);

			var empresa = uow.EmpresaRepository.GetEmpresa(empresaId);
			var cliente = uow.AgenteRepository.GetAgente(empresaId, clienteId);

			context.AddParameter("empresaNombre", empresa.Nombre);
			context.AddParameter("agenteDescripcion", cliente.Descripcion);
			context.AddParameter("agenteCodigo", cliente.Codigo);
			context.AddParameter("agenteTipo", cliente.Tipo);

			context.IsEditingEnabled = editable;
            context.IsAddEnabled = editable;
            context.IsRemoveEnabled = editable;

            if (editable)
            {
                grid.SetInsertableColumns(new List<string>
                {
                    "CD_PRODUTO",
                    "NU_IDENTIFICADOR",
                    "QT_PEDIDO",
                    "TP_LPN_TIPO",
                    "ID_LPN_EXTERNO",
                });

                grid.SetEditableCells(new List<string>
                {
                    "QT_PEDIDO",
                });
            }
            /*
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnDetalleLpnAtributo", "PRE100DetallePedidoLpn_grid1_btn_DetalleAtributo", "fas fa-list"),
            }));
            */
            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            using var uow = this._uowFactory.GetUnitOfWork();

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);
            var dbQuery = new DetallePedidoLpnQuery(pedido);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var qtLiberado = decimal.Parse(row.GetCell("QT_LIBERADO").Value, _identity.GetFormatProvider());
                var qtAnulado = decimal.Parse(row.GetCell("QT_ANULADO").Value, _identity.GetFormatProvider());

                var editable = true;
                if (qtLiberado > 0 || qtAnulado > 0)
                    editable = false;

                row.GetCell("QT_PEDIDO").Editable = editable;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            var dbQuery = new DetallePedidoLpnQuery(pedido);
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

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            var dbQuery = new DetallePedidoLpnQuery(pedido);
            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("Detalle pedido Lpn especifico");
            uow.BeginTransaction();

            try
            {
                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                var empresa = int.Parse(context.GetParameter("empresa"));
                var cliente = context.GetParameter("cliente");
                var nuPedido = context.GetParameter("pedido");

                var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

                if (!pedido.IsManual)
                    throw new ValidationFailedException("WPRE101_Sec0_Error_Er011");

                if (!pedido.PuedeModificarse())
                    throw new ValidationFailedException("General_Sec0_Error_Er092_SituacionNoPermiteEdicion");

                foreach (var row in grid.Rows)
                {
                    var identificador = string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value) ? ManejoIdentificadorDb.IdentificadorAuto : row.GetCell("NU_IDENTIFICADOR").Value;
                    var idEspecificaIdentificador = (!string.IsNullOrEmpty(identificador) && identificador != ManejoIdentificadorDb.IdentificadorAuto) ? "S" : "N";

                    var lpnEspecifico = new DetallePedidoLpnEspecifico()
                    {
                        Pedido = pedido.Id,
                        Cliente = pedido.Cliente,
                        Empresa = pedido.Empresa,
                        Producto = row.GetCell("CD_PRODUTO").Value,
                        Faixa = !string.IsNullOrEmpty(row.GetCell("CD_FAIXA").Value) ? decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider()) : 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = idEspecificaIdentificador,
                        TipoLpn = row.GetCell("TP_LPN_TIPO").Value,
                        IdExternoLpn = row.GetCell("ID_LPN_EXTERNO").Value,
                        Cantidad = decimal.Parse(row.GetCell("QT_PEDIDO").Value, _identity.GetFormatProvider())
                    };

                    if (row.IsNew)
                        AgregarLinea(uow, row, pedido, lpnEspecifico);
                    else if (row.IsDeleted)
                        EliminarLinea(uow, row, pedido, lpnEspecifico);
                    else
                        ModificarLinea(uow, row, pedido, lpnEspecifico);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
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

            return this._gridValidationService.Validate(new DetallePedidoLpnGridValidationModule(uow, this._identity.GetFormatProvider(), empresa, cliente, pedido), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO": return SearchProducto(context);
                case "TP_LPN_TIPO": return SearchTipoLpn(context);
                case "ID_LPN_EXTERNO": return SearchIdExterno(row, context);
            }

            return new List<SelectOption>();
        }

        #region Auxs

        public virtual List<SelectOption> SearchProducto(GridSelectSearchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));

            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

            foreach (var producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} - {producto.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchTipoLpn(GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            var uow = this._uowFactory.GetUnitOfWork();

            var tipos = uow.ManejoLpnRepository.GetAllTipoLPNByDescriptionOrCodePartial(context.SearchValue);

            foreach (var tipo in tipos)
            {
                opciones.Add(new SelectOption(tipo.Tipo, $"{tipo.Tipo} - {tipo.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchIdExterno(GridRow row, GridSelectSearchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var producto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            var tipoLpn = row.GetCell("TP_LPN_TIPO").Value;

            if (string.IsNullOrEmpty(producto) || string.IsNullOrEmpty(tipoLpn))
                return null;

            var lpns = uow.ManejoLpnRepository.GetLpnByIdExternoPartial(tipoLpn, empresa, producto, identificador, context.SearchValue);

            foreach (var lpn in lpns)
            {
                opciones.Add(new SelectOption(lpn.IdExterno, $"{lpn.IdExterno}"));
            }

            return opciones;
        }

        public virtual bool PedidoEditable(IUnitOfWork uow, int empresa, string cliente, string nuPedido)
        {
            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);
            var editAllowed = _security.IsUserAllowed("WPRE101_Page_Access_Puede_Modificar");
            var lpnHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_LPN, empresa) ?? "N") == "S";

            return (!pedido.IsManual || !editAllowed || !lpnHabilitados) ? false : true;
        }

        public virtual void AgregarLinea(IUnitOfWork uow, GridRow row, Pedido pedido, DetallePedidoLpnEspecifico lpnEspecifico)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            var lpn = uow.ManejoLpnRepository.GetLpnByIdExternoTipo(lpnEspecifico.TipoLpn, lpnEspecifico.IdExternoLpn);

            var detallePedidoLpn = uow.ManejoLpnRepository.GetDetallePedidoLpn(pedido.Id, pedido.Cliente, pedido.Empresa, lpnEspecifico.Producto, lpnEspecifico.Faixa,
                lpnEspecifico.Identificador, lpnEspecifico.IdEspecificaIdentificador, lpnEspecifico.TipoLpn, lpnEspecifico.IdExternoLpn);

            if (detallePedidoLpn != null)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_DetallePedidoLpnExistente", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            var detallePedido = pedido.Lineas
                .FirstOrDefault(d => d.Producto == lpnEspecifico.Producto
                    && d.Faixa == lpnEspecifico.Faixa
                    && d.Identificador == lpnEspecifico.Identificador
                    && d.EspecificaIdentificador == (lpnEspecifico.IdEspecificaIdentificador == "S"));

            var cantidad = decimal.Parse(row.GetCell("QT_PEDIDO").Value, _identity.GetFormatProvider());

            if (detallePedido == null)
            {
                var nuevoDetalle = new DetallePedido()
                {
                    Id = pedido.Id,
                    Cliente = pedido.Cliente,
                    Empresa = pedido.Empresa,
                    Producto = lpnEspecifico.Producto,
                    Faixa = lpnEspecifico.Faixa,
                    Identificador = lpnEspecifico.Identificador.Trim(),
                    EspecificaIdentificador = (lpnEspecifico.IdEspecificaIdentificador == "S"),
                    Agrupacion = Agrupacion.Pedido,
                    Cantidad = cantidad,
                    CantidadLiberada = 0,
                    CantidadAnulada = 0,
                    CantidadOriginal = cantidad,
                    FechaAlta = DateTime.Now,
                    Transaccion = nuTransaccion
                };

                pedido.Lineas.Add(nuevoDetalle);
                uow.PedidoRepository.AddDetallePedido(nuevoDetalle);
            }
            else
            {
                detallePedido.Cantidad = (detallePedido.Cantidad ?? 0) + cantidad;
                detallePedido.FechaModificacion = DateTime.Now;
                detallePedido.Transaccion = nuTransaccion;

                uow.PedidoRepository.UpdateDetallePedido(detallePedido);
            }

            uow.PedidoRepository.AddDetallePedidoLpn(new DetallePedidoLpn
            {
                Pedido = pedido.Id,
                Cliente = pedido.Cliente,
                Empresa = pedido.Empresa,
                Producto = lpnEspecifico.Producto,
                Faixa = lpnEspecifico.Faixa,
                Identificador = lpnEspecifico.Identificador.Trim(),
                IdEspecificaIdentificador = lpnEspecifico.IdEspecificaIdentificador,
                CantidadPedida = cantidad,
                CantidadAnulada = 0,
                CantidadLiberada = 0,
                Tipo = lpnEspecifico.TipoLpn,
                IdLpnExterno = lpnEspecifico.IdExternoLpn,
                NumeroLpn = lpn.NumeroLPN,
                FechaAlta = DateTime.Now,
                Transaccion = nuTransaccion
            });

            uow.SaveChanges();
        }

        public virtual void ModificarLinea(IUnitOfWork uow, GridRow row, Pedido pedido, DetallePedidoLpnEspecifico lpnEspecifico)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            var detallePedidoLpn = uow.ManejoLpnRepository.GetDetallePedidoLpn(pedido.Id, pedido.Cliente, pedido.Empresa, lpnEspecifico.Producto, lpnEspecifico.Faixa,
                lpnEspecifico.Identificador, lpnEspecifico.IdEspecificaIdentificador, lpnEspecifico.TipoLpn, lpnEspecifico.IdExternoLpn);

            if (detallePedidoLpn == null)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_DetallePedidoLpnNoExiste", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            var qtNoDisponible = (detallePedidoLpn.CantidadLiberada ?? 0) + (detallePedidoLpn.CantidadAnulada ?? 0);
            if (lpnEspecifico.Cantidad < qtNoDisponible)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_error_DetallePedidoLpnCantidadMenorSaldo", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            var detsPedidoLpnAtributos = uow.ManejoLpnRepository.GetDetallesPedidoLpnAtributo(pedido.Id, pedido.Cliente, pedido.Empresa, lpnEspecifico.Producto, lpnEspecifico.Faixa,
                lpnEspecifico.Identificador, lpnEspecifico.IdEspecificaIdentificador, lpnEspecifico.TipoLpn, lpnEspecifico.IdExternoLpn);

            if (detsPedidoLpnAtributos != null && detsPedidoLpnAtributos.Count > 0)
            {
                var qtUtilizada = detsPedidoLpnAtributos.Sum(d => (d.CantidadPedida));
                if (lpnEspecifico.Cantidad < qtUtilizada)
                {
                    row.GetCell("CD_PRODUTO").SetError("PRE100_msg_error_DetallePedidoLpnCantidadMenorUtilizada", new List<string>() { });
                    throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
                }
            }

            var detallePedido = pedido.Lineas
                .FirstOrDefault(d => d.Producto == lpnEspecifico.Producto
                    && d.Faixa == lpnEspecifico.Faixa
                    && d.Identificador == lpnEspecifico.Identificador
                    && d.EspecificaIdentificador == (lpnEspecifico.IdEspecificaIdentificador == "S"));

            if (detallePedido == null)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_DetallePedidoNoExiste", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            pedido.Lineas.Remove(detallePedido);

            var diferencia = lpnEspecifico.Cantidad - (detallePedidoLpn.CantidadPedida ?? 0);

            detallePedido.Cantidad = (detallePedido.Cantidad ?? 0) + diferencia;
            detallePedido.FechaModificacion = DateTime.Now;
            detallePedido.Transaccion = nuTransaccion;

            pedido.Lineas.Add(detallePedido);

            uow.PedidoRepository.UpdateDetallePedido(detallePedido);

            detallePedidoLpn.CantidadPedida = lpnEspecifico.Cantidad;
            detallePedidoLpn.Transaccion = nuTransaccion;
            detallePedidoLpn.FechaModificacion = DateTime.Now;

            uow.PedidoRepository.UpdateDetallePedidoLpn(detallePedidoLpn);

            uow.SaveChanges();
        }

        public virtual void EliminarLinea(IUnitOfWork uow, GridRow row, Pedido pedido, DetallePedidoLpnEspecifico lpnEspecifico)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            var detallePedidoLpn = uow.ManejoLpnRepository.GetDetallePedidoLpn(pedido.Id, pedido.Cliente, pedido.Empresa, lpnEspecifico.Producto, lpnEspecifico.Faixa,
                lpnEspecifico.Identificador, lpnEspecifico.IdEspecificaIdentificador, lpnEspecifico.TipoLpn, lpnEspecifico.IdExternoLpn);

            if (detallePedidoLpn == null)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_DetallePedidoLpnNoExiste", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }
            else if ((detallePedidoLpn.CantidadLiberada ?? 0) > 0 || (detallePedidoLpn.CantidadAnulada ?? 0) > 0)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_NoSePuedeEliminarCantLiberadaAnulada", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            if (uow.ManejoLpnRepository.AnyProductoDetPedidoLpnAtributo(pedido.Id, pedido.Cliente, pedido.Empresa, lpnEspecifico.Producto, lpnEspecifico.Faixa,
                    lpnEspecifico.Identificador, lpnEspecifico.IdEspecificaIdentificador, lpnEspecifico.TipoLpn, lpnEspecifico.IdExternoLpn))
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_NoSePuedeEliminarExitenAtributos", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            var detallePedido = pedido.Lineas
                .FirstOrDefault(d => d.Producto == lpnEspecifico.Producto
                    && d.Faixa == lpnEspecifico.Faixa
                    && d.Identificador == lpnEspecifico.Identificador
                    && d.EspecificaIdentificador == (lpnEspecifico.IdEspecificaIdentificador == "S"));

            if (detallePedido == null)
            {
                row.GetCell("CD_PRODUTO").SetError("PRE100_msg_Error_DetallePedidoNoExiste", new List<string>() { });
                throw new ValidationFailedException("General_Sec0_Error_ErrorGuardarCambios");
            }

            pedido.Lineas.Remove(detallePedido);

            detallePedido.Cantidad = (detallePedido.Cantidad ?? 0) - (detallePedidoLpn.CantidadPedida ?? 0);
            detallePedido.FechaModificacion = DateTime.Now;
            detallePedido.Transaccion = nuTransaccion;

            pedido.Lineas.Add(detallePedido);

            uow.PedidoRepository.UpdateDetallePedido(detallePedido);

            detallePedidoLpn.FechaModificacion = DateTime.Now;
            detallePedidoLpn.Transaccion = nuTransaccion;
            detallePedidoLpn.TransaccionDelete = nuTransaccion;

            uow.PedidoRepository.UpdateDetallePedidoLpn(detallePedidoLpn);
            uow.SaveChanges();

            uow.PedidoRepository.DeleteDetallePedidoLpn(detallePedidoLpn);
            uow.SaveChanges();
        }

        #endregion
    }
}
