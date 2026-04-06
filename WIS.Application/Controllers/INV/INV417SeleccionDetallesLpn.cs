using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
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
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.INV
{
    public class INV417SeleccionDetallesLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogger<INV417SeleccionDetallesLpn> _logger;

        protected List<string> GridKeysAtributo { get; }
        protected List<SortCommand> SortsAtributo { get; }
        protected List<string> GridKeysRegistrosDisp { get; }
        protected List<SortCommand> SortsRegistrosDisp { get; }
        protected List<string> GridKeysRegistrosSel { get; }
        protected List<SortCommand> SortsRegistrosSel { get; }

        public INV417SeleccionDetallesLpn(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            ILogger<INV417SeleccionDetallesLpn> logger)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
            _concurrencyControl = concurrencyControl;
            _logger = logger;

            GridKeysRegistrosDisp = new List<string>
            {
                "CD_ENDERECO",
                "CD_PRODUTO",
                "CD_EMPRESA",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "NU_LPN",
                "ID_LPN_DET"
            };

            GridKeysRegistrosSel = new List<string>
            {
                "NU_INVENTARIO",
                "NU_INVENTARIO_ENDERECO",
                "NU_INVENTARIO_ENDERECO_DET"
            };

            GridKeysAtributo = new List<string>
            {
                "ID_ATRIBUTO",
            };

            SortsRegistrosDisp = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
            };

            SortsRegistrosSel = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
            };

            SortsAtributo = new List<SortCommand> {
                new SortCommand("NM_ATRIBUTO", SortDirection.Ascending),
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "INV417_grid_DetalleLpnAtrCab" || grid.Id == "INV417_grid_DetalleLpnAtrDet")
            {
                context.IsEditingEnabled = true;
                context.IsAddEnabled = false;
                context.IsCommitEnabled = false;
                context.IsRemoveEnabled = false;
                context.IsRollbackEnabled = false;
            }
            else
            {
                if (!decimal.TryParse(context.GetParameter("nuInventario"), _identity.GetFormatProvider(), out decimal nuInventario))
                    return grid;

                var inventario = uow.InventarioRepository.GetInventario(nuInventario);
                if (inventario.IsEditable())
                {
                    if (grid.Id == "INV417_grid_1")
                    {
                        grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
                        {
                            new GridButton("btnAtributosCabezal", "INV410_Sec0_btn_AtributosCabezal", "fas fa-list"),
                            new GridButton("btnAtributosDetalle", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
                        }));

                        grid.MenuItems = new List<IGridItem>
                        {
                            new GridButton("btnAgregar", "INV410_Sec0_btn_AgregarRegistro"),
                            new GridButton("btnAgregarHabilitar", "INV410_Sec0_btn_AgregarHabilitar")
                        };
                    }
                    else if (grid.Id == "INV417_grid_2")
                    {
                        grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
                        {
                            new GridButton("btnAtributosCabezal", "INV410_Sec0_btn_AtributosCabezal", "fas fa-list"),
                            new GridButton("btnAtributosDetalle", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
                        }));

                        grid.MenuItems = new List<IGridItem>
                        {
                            new GridButton("btnEliminar", "INV410_Sec0_btn_QuitarRegistro"),
                        };
                    }
                }
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "INV417_grid_DetalleLpnAtrCab")
            {
                var dbQuery = new InventarioConsultaAtributosDetalleLpnCabezalQuery();
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, SortsAtributo, GridKeysAtributo);
                grid.SetEditableCells(new List<string> { "VL_ATRIBUTO" });
            }
            else if (grid.Id == "INV417_grid_DetalleLpnAtrDet")
            {
                var dbQuery = new InventarioConsultaAtributosDetalleLpnDetQuery();
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, SortsAtributo, GridKeysAtributo);
                grid.SetEditableCells(new List<string> { "VL_ATRIBUTO" });
            }
            else
            {
                if (!decimal.TryParse(context.GetParameter("nuInventario"), _identity.GetFormatProvider(), out decimal nuInventario))
                    return grid;

                var filtros = InventarioLogic.GetFiltros(uow, context, _identity);

                if (grid.Id == "INV417_grid_1")
                {
                    var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
                    uow.HandleQuery(dbQuery);
                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, SortsRegistrosDisp, GridKeysRegistrosDisp);
                }
                else
                {
                    var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
                    uow.HandleQuery(dbQuery);
                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, SortsRegistrosSel, GridKeysRegistrosSel);
                }

                var inventario = uow.InventarioRepository.GetInventario(nuInventario);

                grid.Rows.ForEach(row =>
                {
                    row.DisabledButtons = new List<string>()
                    {
                        "btnAtributosCabezal",
                        "btnAtributosDetalle",
                    };

                    if (!string.IsNullOrEmpty(row.GetCell("NU_LPN").Value) && row.GetCell("NU_LPN").Value != "-" &&
                        !string.IsNullOrEmpty(row.GetCell("ID_LPN_DET").Value) && row.GetCell("ID_LPN_DET").Value != "-")
                    {
                        row.DisabledButtons = new List<string>();
                    }

                    if (!inventario.IsEditable())
                        row.DisabledSelected = true;
                });
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "INV417_grid_DetalleLpnAtrCab")
            {
                var dbQuery = new InventarioConsultaAtributosDetalleLpnCabezalQuery();
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "INV417_grid_DetalleLpnAtrDet")
            {
                var dbQuery = new InventarioConsultaAtributosDetalleLpnDetQuery();
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                    return null;

                var filtros = InventarioLogic.GetFiltros(uow, context, _identity);

                if (grid.Id == "INV417_grid_1")
                {
                    var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else
                {
                    var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "INV417_grid_DetalleLpnAtrCab")
            {
                var dbQuery = new InventarioConsultaAtributosDetalleLpnCabezalQuery();
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, SortsAtributo);
            }
            else if (grid.Id == "INV417_grid_DetalleLpnAtrDet")
            {
                var dbQuery = new InventarioConsultaAtributosDetalleLpnDetQuery();
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, SortsAtributo);
            }
            else
            {
                if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                    return null;

                var filtros = InventarioLogic.GetFiltros(uow, context, _identity);

                if (grid.Id == "INV417_grid_1")
                {
                    var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
                    uow.HandleQuery(dbQuery);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, SortsRegistrosDisp);
                }
                else
                {
                    var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
                    uow.HandleQuery(dbQuery);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, SortsRegistrosSel);
                }
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

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.GridId == "INV417_grid_1" || context.GridId == "INV417_grid_2")
            {
                switch (context.ButtonId)
                {
                    case "btnAtributosCabezal":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter() { Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter() { Id = "detalle", Value = "false" },
                        });
                        break;

                    case "btnAtributosDetalle":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter() { Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter() { Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                            new ComponentParameter() { Id = "detalle", Value = "true" },
                        });
                        break;
                }
            }

            return context;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            return form;
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
            var cantInOtherInventario = 0;
            var cantNotAdd = 0;

            var registros = GetSelectedRegistrosDetalleLpn(uow, context)
                .OrderBy(r => r.Ubicacion)
                .ThenBy(r => r.Producto)
                .ThenBy(r => r.Identificador)
                .ToList();

            foreach (var registro in registros)
            {
                var result = logic.AgregarRegistros(uow, inventario, registro, out bool enOtroIventario);

                if (!result)
                {
                    cantNotAdd++;
                }
                else if (enOtroIventario)
                {
                    cantInOtherInventario++;
                }
            }

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
                if (cantNotAdd == registros.Count)
                    context.AddErrorNotification("INV410_Sec0_Info_RegistrosNoAgregados");
                else
                    context.AddWarningNotification("INV410_Sec0_Error_AgregadoParcialdeRegistros", new List<string> { cantInOtherInventario.ToString() });
            }
            else
            {
                if (cantInOtherInventario > 0)
                    context.AddInfoNotification("INV413_Sec0_Info_CantRegistroEnOtroInventario", new List<string> { cantInOtherInventario.ToString() });

                if (context.ButtonId == "btnAgregarHabilitar")
                {
                    if (info != null)
                    {
                        var arguments = info.GetArgumentos();
                        context.AddInfoNotification(info.Mensaje, arguments);
                    }
                    else
                        context.AddSuccessNotification("INV410_Sec0_Success_InventarioXHabilitado", new List<string> { inventario.NumeroInventario.ToString() });
                }
            }

            return context;
        }
        public virtual GridMenuItemActionContext GridMenuItemQuitar(IUnitOfWork uow, IInventario inventario, GridMenuItemActionContext context)
        {
            var keysRowSelected = GetSelectedRegistrosDetalleLpnQuitar(uow, context);

            var detallesToDelete = new Dictionary<decimal, List<InventarioSelectRegistroLpn>>();

            foreach (var key in keysRowSelected)
            {
                if (!detallesToDelete.ContainsKey(key.NuInventarioUbicacion))
                    detallesToDelete[key.NuInventarioUbicacion] = new List<InventarioSelectRegistroLpn>();

                detallesToDelete[key.NuInventarioUbicacion].Add(key);
            }

            var nuTransaccion = uow.GetTransactionNumber();

            foreach (var ubicacion in detallesToDelete.Keys)
            {
                var amountDeleted = 0;
                var inventarioUbicacion = uow.InventarioRepository.GetInventarioEndereco(ubicacion);

                foreach (var key in detallesToDelete[ubicacion])
                {
                    var detalle = inventarioUbicacion.Detalles.FirstOrDefault(d => d.Id == key.NuInventarioUbicacionDetalle);

                    detalle.NumeroTransaccion = nuTransaccion;
                    detalle.NumeroTransaccionDelete = nuTransaccion;
                    detalle.UserId = _identity.UserId;

                    uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalle);
                    uow.SaveChanges();

                    uow.InventarioRepository.DeleteInventarioEnderecoDetalle(detalle);
                    uow.SaveChanges();

                    amountDeleted++;
                }

                if (inventarioUbicacion.Detalles.Count == amountDeleted)
                {
                    inventarioUbicacion.NumeroTransaccion = nuTransaccion;
                    inventarioUbicacion.NumeroTransaccionDelete = nuTransaccion;

                    uow.InventarioRepository.UpdateInventarioEndereco(inventarioUbicacion);
                    uow.SaveChanges();

                    uow.InventarioRepository.DeleteInventarioEndereco(inventarioUbicacion);
                    uow.SaveChanges();
                }
            }

            uow.SaveChanges();
            uow.Commit();

            return context;
        }

        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosDetalleLpn(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(GridKeysRegistrosDisp)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    Ubicacion = item["CD_ENDERECO"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Producto = item["CD_PRODUTO"],
                    Faixa = decimal.Parse(item["CD_FAIXA"], _identity.GetFormatProvider()),
                    Identificador = item["NU_IDENTIFICADOR"],
                    NroLpn = item["NU_LPN"],
                    IdDetalleLpn = item["ID_LPN_DET"],
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistrosLpn();

            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.Ubicacion, ks.Producto, ks.Empresa, ks.Identificador, ks.Faixa, ks.NroLpn, ks.IdDetalleLpn },
                    s => new { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa, s.NroLpn, s.IdDetalleLpn },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.Ubicacion, ks.Producto, ks.Empresa, ks.Identificador, ks.Faixa, ks.NroLpn, ks.IdDetalleLpn },
                    s => new { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa, s.NroLpn, s.IdDetalleLpn },
                    (ks, s) => ks).ToList();
            }
        }
        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosDetalleLpnQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(GridKeysRegistrosSel)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    NuInventario = decimal.Parse(item["NU_INVENTARIO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacion = decimal.Parse(item["NU_INVENTARIO_ENDERECO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacionDetalle = decimal.Parse(item["NU_INVENTARIO_ENDERECO_DET"], _identity.GetFormatProvider()),
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistrosLpn();
            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();
            }
        }

        #endregion

    }
}
