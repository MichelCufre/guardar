using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO150PanelStock : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO150PanelStock(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "CD_ENDERECO",
                "CD_EMPRESA",
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR"
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnDetalle", "STO150_grid1_btn_Detalle", "fas fa-list")
            }));

            grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnTotalizar", "STO150_grid1_btn_Totalizar")
                };
            grid.AddOrUpdateColumn(new GridColumnSelect("NU_DOMINIO", this.InitializeSelectMotivoAveria()));

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockUbicacionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa)
                    || string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string nmEmpresa = uow.EmpresaRepository.GetNombre(idEmpresa);
                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string descProducto = uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto);

                context.AddParameter("STO150_EMPRESA", idEmpresa.ToString() + " - " + nmEmpresa);
                context.AddParameter("STO150_PRODUCTO", idProducto + " - " + descProducto);
                if (context.Parameters.Any(s => s.Id == "excluirAreaEquipoTransferencia"))
                {

                    if (!bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "excluirAreaEquipoTransferencia")?.Value, out bool excluir))
                    {
                        throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                    }
                    else
                    {
                        context.AddParameter("STO150_EXCLUIR", context.Parameters.FirstOrDefault(s => s.Id == "excluirAreaEquipoTransferencia")?.Value.ToLower());

                        dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value, excluir);
                    }
                }
                else
                {
                    dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value);
                }
            }
            else
            {
                dbQuery = new StockUbicacionQuery();
            }

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            var areas = uow.UbicacionAreaRepository.GetAreasPermitidasParaCambiarVencimiento();
            string[] manejoFechas = { ManejoFechaProductoDb.Fifo, ManejoFechaProductoDb.Expirable };

            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.STO150_Page_Access_PermiteEditarFecha,
                SecurityResources.STO150_Page_Access_PermiteMarcarAveria,
            });

            foreach (var row in grid.Rows)
            {
                if (Convert.ToDecimal(row.GetCell("QT_TRANSITO_ENTRADA").Value) <= 0
                    && Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value) <= 0
                    && Convert.ToDecimal(row.GetCell("QT_LPN").Value) <= 0)
                    row.DisabledButtons.Add("btnDetalle");

                string endereco = row.GetCell("CD_ENDERECO").Value;
                int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string producto = row.GetCell("CD_PRODUTO").Value;
                decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, this._identity.GetFormatProvider());
                string nuIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;

                decimal cantidadStockLpn = uow.ManejoLpnRepository.GetStockLpnUbicacion(endereco, empresa, producto, nuIdentificador, faixa, out decimal cantidadReservaLpn, out decimal cantidadReservaAtributo);
                var stock = Convert.ToDecimal(row.GetCell("QT_ESTOQUE").Value);
                var reserva = Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value);

                if (result[SecurityResources.STO150_Page_Access_PermiteEditarFecha]
                    && areas.Contains(short.Parse(row.GetCell("CD_AREA_ARMAZ").Value))
                    && manejoFechas.Contains(row.GetCell("TP_MANEJO_FECHA").Value)
                    && stock > cantidadStockLpn
                    && (reserva - cantidadReservaLpn - cantidadReservaAtributo) <= 0)
                    row.GetCell("DT_VENCIMIENTO").Editable = true;



                if (result[SecurityResources.STO150_Page_Access_PermiteMarcarAveria]
                    && areas.Contains(short.Parse(row.GetCell("CD_AREA_ARMAZ").Value))
                    && stock > cantidadStockLpn
                    && (reserva - cantidadReservaLpn - cantidadReservaAtributo) <= 0)
                    row.GetCell("ID_AVERIA").Editable = true;

            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockUbicacionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa)
                    || string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (context.Parameters.Any(s => s.Id == "excluirAreaEquipoTransferencia"))
                {

                    if (!bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "excluirAreaEquipoTransferencia").Value, out bool excluir))
                    {
                        throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                    }
                    else
                    {
                        dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value, excluir);
                    }
                }
                else
                {
                    dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value);
                }
            }
            else
            {
                dbQuery = new StockUbicacionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

            try
            {
                foreach (var row in grid.Rows)
                {
                    int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    string producto = row.GetCell("CD_PRODUTO").Value;
                    string ubicacion = row.GetCell("CD_ENDERECO").Value;
                    decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
                    string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                    DateTime? vencimiento = null;
                    if (DateTime.TryParse(row.GetCell("DT_VENCIMIENTO").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fecha))
                    {
                        vencimiento = fecha.Date;
                    }

                    Stock stock = uow.StockRepository.GetStock(empresa, producto, faixa, ubicacion, identificador);
                    List<short> areas = uow.UbicacionAreaRepository.GetAreasPermitidasParaCambiarVencimiento();
                    string[] manejoFechas = { ManejoFechaProductoDb.Fifo, ManejoFechaProductoDb.Expirable }; //TODO: Ver que hacer con esto

                    short? area = uow.UbicacionAreaRepository.GetAreaByUbicacion(stock.Ubicacion);

                    Producto productoStock = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(stock.Empresa, stock.Producto);

                    if (vencimiento != stock.Vencimiento)
                    {
                        if (!areas.Contains(area ?? -1))
                            throw new ValidationFailedException("STO150_grid1_Error_StockCantBeEditedArea");

                        if (!manejoFechas.Contains(productoStock.TipoManejoFecha))
                            throw new ValidationFailedException("STO150_grid1_Error_StockCantBeEditedManejoFecha");

                        stock.Vencimiento = vencimiento;
                    }

                    stock.Averia = row.GetCell("ID_AVERIA").Value;
                    stock.MotivoAveria = row.GetCell("NU_DOMINIO").Value;
                    stock.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.StockRepository.UpdateStock(stock);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null
                  && uow.IsSnapshotException(ex.InnerException))
                {
                    return this.GridCommit(grid, context);
                }
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
            }
            finally
            {
                uow.EndTransaction();
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new STO150GridValidationModule(uow), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockUbicacionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa)
                    || string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (context.Parameters.Any(s => s.Id == "excluirAreaEquipoTransferencia"))
                {

                    if (!bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "excluirAreaEquipoTransferencia").Value, out bool excluir))
                    {
                        throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                    }
                    else
                    {
                        dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value, excluir);
                    }
                }
                else
                {
                    dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value);
                }
            }
            else
            {
                dbQuery = new StockUbicacionQuery();
            }


            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalle")
            {
                context.Redirect("/stock/STO151", newTab: true, new List<ComponentParameter>() {
                new ComponentParameter("ubicacion",context.Row.GetCell("CD_ENDERECO").Value),
                new ComponentParameter("producto",context.Row.GetCell("CD_PRODUTO").Value),
                new ComponentParameter("empresa",context.Row.GetCell("CD_EMPRESA").Value),
                new ComponentParameter("embalaje",context.Row.GetCell("CD_FAIXA").Value),
                new ComponentParameter("identificador", context.Row.GetCell("NU_IDENTIFICADOR").Value),
                new ComponentParameter("area",context.Row.GetCell("CD_AREA_ARMAZ").Value),

                });
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockUbicacionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa)
                    || string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string nmEmpresa = uow.EmpresaRepository.GetNombre(idEmpresa);
                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string descProducto = uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto);

                context.AddParameter("STO150_EMPRESA", idEmpresa.ToString() + " - " + nmEmpresa);
                context.AddParameter("STO150_PRODUCTO", idProducto + " - " + descProducto);
                if (context.Parameters.Any(s => s.Id == "excluirAreaEquipoTransferencia"))
                {

                    if (!bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "excluirAreaEquipoTransferencia")?.Value, out bool excluir))
                    {
                        throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                    }
                    else
                    {
                        context.AddParameter("STO150_EXCLUIR", context.Parameters.FirstOrDefault(s => s.Id == "excluirAreaEquipoTransferencia")?.Value.ToLower());

                        dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value, excluir);
                    }
                }
                else
                {
                    dbQuery = new StockUbicacionQuery(idEmpresa, context.Parameters.FirstOrDefault(s => s.Id == "producto").Value);
                }
            }
            else
            {
                dbQuery = new StockUbicacionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            Stock totalizado = dbQuery.GetTotalizado();

            context.AddParameter("QT_ESTOQUE", totalizado.Cantidad.ToString());
            context.AddParameter("QT_RESERVA_SAIDA", totalizado.ReservaSalida.ToString());
            context.AddParameter("QT_TRANSITO_ENTRADA", totalizado.CantidadTransitoEntrada.ToString());

            return context;
        }

        #region Metodos Auxiliares
        public virtual List<SelectOption> InitializeSelectMotivoAveria()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(IntegracionServicioDb.MOTIVO_AVERIA);

            foreach (var dominio in dominios)
                opciones.Add(new SelectOption(dominio.Id, dominio.Descripcion));

            return opciones;
        }
        #endregion
    }
}
