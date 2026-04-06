using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Inventario;
using WIS.Domain.Inventario;
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
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.INV
{
    public class INV411UbicacionesInventario : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogger<INV411UbicacionesInventario> _logger;

        protected List<string> GridKeysDisponibles { get; }
        protected List<string> GridKeysSeleccionados { get; }

        protected List<SortCommand> SortsUbicacionesDisp { get; }
        protected List<SortCommand> SortsUbicacionesSel { get; }


        public INV411UbicacionesInventario(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            ILogger<INV411UbicacionesInventario> logger)
        {
            this.GridKeysDisponibles = new List<string>
            {
                "CD_ENDERECO"
            };

            this.GridKeysSeleccionados = new List<string>
            {
                "NU_INVENTARIO_ENDERECO"
            };

            this.SortsUbicacionesDisp = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
            };

            this.SortsUbicacionesSel = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!decimal.TryParse(context.GetParameter("nuInventario"), _identity.GetFormatProvider(), out decimal nuInventario))
                return grid;

            var inventario = uow.InventarioRepository.GetInventario(nuInventario);
            if (inventario.IsEditable())
            {
                if (grid.Id == "INV411_grid_1")
                {
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnAgregar", "INV410_Sec0_btn_AgregarRegistro"),
                        new GridButton("btnAgregarHabilitar", "INV410_Sec0_btn_AgregarHabilitar")
                    };
                }
                else if (grid.Id == "INV411_grid_2")
                {
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnEliminar", "INV410_Sec0_btn_QuitarRegistro"),
                    };
                }
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!decimal.TryParse(context.GetParameter("nuInventario"), _identity.GetFormatProvider(), out decimal nuInventario))
                throw new MissingParameterException("INV411_Sec0_Info_PerdidaNuInventario");

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);

            if (grid.Id == "INV411_grid_1")
            {
                var dbQuery = new InventarioUbicacionQuery(filtros);
                uow.HandleQuery(dbQuery, filterEmpresa: false);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, SortsUbicacionesDisp, GridKeysDisponibles);
            }
            else
            {
                var dbQuery = new InventarioUbicacionQuitarQuery(filtros);
                uow.HandleQuery(dbQuery, filterEmpresa: false);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, SortsUbicacionesSel, GridKeysSeleccionados);
            }

            var inventario = uow.InventarioRepository.GetInventario(nuInventario);

            if (!inventario.IsEditable())
                grid.Rows.ForEach(i => i.DisabledSelected = true);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);

            if (grid.Id == "INV411_grid_1")
            {
                var dbQuery = new InventarioUbicacionQuery(filtros);
                uow.HandleQuery(dbQuery, filterEmpresa: false);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                var dbQuery = new InventarioUbicacionQuitarQuery(filtros);
                uow.HandleQuery(dbQuery, filterEmpresa: false);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);

            if (grid.Id == "INV411_grid_1")
            {
                var dbQuery = new InventarioUbicacionQuery(filtros);
                uow.HandleQuery(dbQuery, filterEmpresa: false);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, SortsUbicacionesDisp);
            }
            else
            {
                var dbQuery = new InventarioUbicacionQuitarQuery(filtros);
                uow.HandleQuery(dbQuery, filterEmpresa: false);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, SortsUbicacionesSel);
            }
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                uow.CreateTransactionNumber("GridMenuItemAction");

                if (context.ButtonId == "btnEliminar")
                    GridMenuItemQuitar(uow, inventario, context);
                else
                    GridMenuItemAgregar(uow, inventario, logic, context);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemAction");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemAction");
                uow.Rollback();
                throw;
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (!decimal.TryParse(context.GetParameter("nuInventario"), _identity.GetFormatProvider(), out decimal nuInventario))
                return form;

            using var uow = this._uowFactory.GetUnitOfWork();

            var inventario = uow.InventarioRepository.GetInventario(nuInventario);
            var tipoInventario = uow.DominioRepository.GetDominio(CodigoDominioDb.TipoInventario, inventario.TipoInventario);
            var predio = uow.PredioRepository.GetPredio(inventario.Predio);

            form.GetField("nuInventario").Value = context.GetParameter("nuInventario");
            form.GetField("descInventario").Value = inventario.Descripcion;
            form.GetField("tipoInventario").Value = $"{inventario.TipoInventario} - {tipoInventario.Descripcion}";
            form.GetField("predio").Value = $"{inventario.Predio} - {predio.Descripcion}";

            if (inventario.Empresa.HasValue)
            {
                var empresa = uow.EmpresaRepository.GetEmpresa(inventario.Empresa.Value);
                form.GetField("empresa").Value = empresa.Id.ToString();
                form.GetField("descEmpresa").Value = empresa.Nombre;
            }

            return form;
        }

        #region Metodos auxiliares

        public virtual GridMenuItemActionContext GridMenuItemAgregar(IUnitOfWork uow, IInventario inventario, InventarioLogic logic, GridMenuItemActionContext context)
        {
            var cantNotAdd = 0;
            var keysRowSelected = GetSelectedUbicaciones(uow, context);

            keysRowSelected.ForEach(x =>
            {
                if (!logic.AgregarUbicacion(uow, inventario, x))
                    cantNotAdd++;
            });

            uow.SaveChanges();

            Domain.Validation.Error info = null;

            if (context.ButtonId == "btnAgregarHabilitar" && cantNotAdd == 0)
            {
                logic.HabilitarInventario(uow, inventario, out info);
            }

            uow.SaveChanges();
            uow.Commit();

            if (cantNotAdd > 0)
            {
                if (cantNotAdd == keysRowSelected.Count)
                    context.AddErrorNotification("INV410_Sec0_Info_UbicacionesNoAgregados");
                else
                    context.AddWarningNotification("INV410_Sec0_Error_AgregadoParcialdeUbicaciones", new List<string> { cantNotAdd.ToString() });
            }
            else if (context.ButtonId == "btnAgregarHabilitar")
            {
                if (info != null)
                {
                    var arguments = info.GetArgumentos();
                    context.AddInfoNotification(info.Mensaje, arguments);
                }
                else
                    context.AddSuccessNotification("INV410_Sec0_Success_InventarioXHabilitado", new List<string> { inventario.NumeroInventario.ToString() });
            }

            return context;
        }
        public virtual GridMenuItemActionContext GridMenuItemQuitar(IUnitOfWork uow, IInventario inventario, GridMenuItemActionContext context)
        {
            var keysRowSelected = GetSelectedUbicacionesQuitar(uow, context);

            keysRowSelected.ForEach(x =>
            {
                var inventarioUbicacion = uow.InventarioRepository.GetInventarioEndereco(x);

                inventarioUbicacion.NumeroTransaccion = uow.GetTransactionNumber();
                inventarioUbicacion.NumeroTransaccionDelete = inventarioUbicacion.NumeroTransaccion;

                uow.InventarioRepository.UpdateInventarioEndereco(inventarioUbicacion);
                uow.SaveChanges();

                uow.InventarioRepository.DeleteInventarioEndereco(inventarioUbicacion);
                uow.SaveChanges();
            });

            uow.SaveChanges();
            uow.Commit();

            return context;
        }

        public virtual List<string> GetSelectedUbicaciones(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuery(filtros);

            ((UnitOfWork)uow).HandleQuery(dbQuery, filterEmpresa: false);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
        public virtual List<decimal> GetSelectedUbicacionesQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuitarQuery(filtros);

            ((UnitOfWork)uow).HandleQuery(dbQuery, filterEmpresa: false);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keys = new List<decimal>();

            foreach (var key in context.Selection.Keys)
            {
                keys.Add(decimal.Parse(key, _identity.GetFormatProvider()));
            }

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
        }

        #endregion
    }
}
