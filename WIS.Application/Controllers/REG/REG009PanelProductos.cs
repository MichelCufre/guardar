using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
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

namespace WIS.Application.Controllers.REG
{
    public class REG009PanelProductos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG009PanelProductos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA", "CD_PRODUTO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsCommitEnabled = false;
            context.IsAddEnabled = false;
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;

            if (this._security.IsUserAllowed(SecurityResources.REG009Update_Page_Access_Allow))
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY_0", new List<GridButton>
                {
                    new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                }));
            }

            if (this._security.IsUserAllowed(SecurityResources.IMP060_Page_Access_Allow))
            {
                grid.MenuItems = new List<IGridItem>
                {
                    new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir")
                };
            }

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", this.BotonesComprobadosUsuarios()));

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProductosQuery dbQuery;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                dbQuery = new ProductosQuery(idProducto, idEmpresa);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("DS_PRODUTO").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;

                Empresa empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);
                string descProducto = uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto.ToString());

                context.AddParameter("REG009_CD_EMPRESA", idEmpresa.ToString());
                context.AddParameter("REG009_NM_EMPRESA", empresa.Nombre);
                context.AddParameter("REG009_CD_PRODUCTO", idProducto.ToString());
                context.AddParameter("REG009_DS_PRODUCTO", descProducto);
            }
            else if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProductosQuery(idEmpresa);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;

                Empresa empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);

                context.AddParameter("REG009_CD_EMPRESA", idEmpresa.ToString());
                context.AddParameter("REG009_NM_EMPRESA", empresa.Nombre);
            }
            else
            {
                dbQuery = new ProductosQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            grid.SetEditableCells(new List<string>
            {
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProductosQuery dbQuery = new ProductosQuery();

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value, out int idProducto))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProductosQuery(idProducto.ToString(), idEmpresa);
            }
            else if (context.Parameters.Count > 0 && context.Parameters.Any(s => s.Id == "empresa"))
            {
                if (!context.Parameters.Any(s => s.Id == "empresa"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idEmpresa = context.Parameters.Any(s => s.Id == "empresa") ? context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : string.Empty;


                dbQuery = new ProductosQuery(int.Parse(idEmpresa));

                uow.HandleQuery(dbQuery);
            }
            else
            {
                uow.HandleQuery(dbQuery);
            }

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProductosQuery dbQuery = new ProductosQuery();

            if (context.Parameters.Count > 1)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value, out int idProducto))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProductosQuery(idProducto.ToString(), idEmpresa);

            }
            else if (context.Parameters.Count > 0)
            {
                if (!context.Parameters.Any(s => s.Id == "empresa"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idEmpresa = context.Parameters.Any(s => s.Id == "empresa") ? context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : string.Empty;

                if (!string.IsNullOrEmpty(idEmpresa))
                {
                    dbQuery = new ProductosQuery(int.Parse(idEmpresa));
                }
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(grid, row, context);
                case "CD_PRODUTO":
                    return this.SearchProduto(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {

            ComponentParameter componenteProducto = new ComponentParameter("producto", context.Row.GetCell("CD_PRODUTO").Value);
            ComponentParameter componenteEmpresa = new ComponentParameter("empresa", context.Row.GetCell("CD_EMPRESA").Value);

            if (context.ButtonId == "btnCodigoBarras")
            {
                context.Redirect("/registro/REG603", new List<ComponentParameter>
                {
                    componenteEmpresa,
                    componenteProducto
                });
            }
            else if (context.ButtonId == "btnControlesCalidad")
            {
                context.Redirect("/registro/REG602", new List<ComponentParameter>
                {
                    componenteEmpresa,
                    componenteProducto
                });
            }
            else if (context.ButtonId == "btnPallets")
            {
                context.Redirect("/registro/REG605", new List<ComponentParameter>
                {
                    componenteEmpresa,
                    componenteProducto
                });
            }
            else if (context.ButtonId == "btnMovStockProducto")
            {
                context.Redirect("/stock/STO395", new List<ComponentParameter>
                {
                    componenteEmpresa,
                    componenteProducto
                });
            }
            else if (context.ButtonId == "btnAsignacionUbicPacking")
            {
                context.Redirect("/registro/REG050", new List<ComponentParameter>
                {
                    componenteEmpresa,
                    componenteProducto
                });
            }
            else if (context.ButtonId == "btnAdjuntarArchivo")
            {
                //TODO
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string> filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, context);

            if (filasSeleccionadas.Count > 0)
                context.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(filasSeleccionadas));
            else
                throw new MissingParameterException("General_Sec0_Error_impresionSinSeleccion");

            return context;
        }

        #region Metodos Auxiliares

        public virtual List<IGridItem> BotonesComprobadosUsuarios()
        {
            List<IGridItem> listaBotones = new List<IGridItem>();

            if (this._security.IsUserAllowed(SecurityResources.WREG050_Page_Access_AsignacionUbicacionesPick))
                listaBotones.Add(new GridButton("btnAsignacionUbicPacking", "REG009_Sec0_btn_AsigUbicPicking", "fas fa-box-open"));

            if (this._security.IsUserAllowed(SecurityResources.WREG009_grid1_btn_CodigoBarras))
                listaBotones.Add(new GridButton("btnCodigoBarras", "REG009_Sec0_btn_CodigoBarra", "fas fa-barcode"));

            if (this._security.IsUserAllowed(SecurityResources.WREG009_grid1_btn_ControlCalidad))
                listaBotones.Add(new GridButton("btnControlesCalidad", "REG009_Sec0_btn_ControlesCalidad", "fas fa-clipboard-check"));

            if (this._security.IsUserAllowed(SecurityResources.WREG009_grid1_btn_Pallets))
                listaBotones.Add(new GridButton("btnPallets", "REG009_Sec0_btn_CapacidadPallet", "fas fa-pallet"));

            if (this._security.IsUserAllowed(SecurityResources.WREG009_grid1_btn_StockPorProducto))
                listaBotones.Add(new GridButton("btnMovStockProducto", "REG009_Sec0_btn_MovStockProducto", "fas fa-layer-group"));

            if (this._security.IsUserAllowed(SecurityResources.REG009Grupo_Page_Access_Allow))
                listaBotones.Add(new GridButton("btnVerGrupo", "REG009_Sec0_btn_VerGrupo", "fas fa-info"));

            return listaBotones;
        }

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(context.SearchValue, this._identity.UserId);

            foreach (var emp in empresasAsignadasUsuario)
            {
                opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProduto(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Producto> productos = new List<Producto>();
            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(idEmpresa, context.SearchValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value))
                {
                    productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(int.Parse(row.GetCell("CD_EMPRESA").Value), context.SearchValue);
                }
                else
                {
                    row.GetCell("CD_EMPRESA").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));
                }
            }

            foreach (var prod in productos)
            {
                opciones.Add(new SelectOption(prod.Codigo, $"{prod.Codigo} - {prod.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<string> ObtenerKeyLineasSeleccionadas(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            ProductosQuery dbQuery;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                dbQuery = new ProductosQuery(idProducto, idEmpresa);
            }
            else if (context.Parameters.Count > 0 && context.Parameters.Any(s => s.Id == "empresa"))
            {
                int empresaParam = int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value);

                dbQuery = new ProductosQuery(empresaParam);
            }
            else
            {
                dbQuery = new ProductosQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            List<string> resultado = new List<string>();

            int empresa;
            string producto;
            if (context.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new { g.CD_PRODUTO, g.CD_EMPRESA }).ToList();

                foreach (var noSeleccionKeys in context.Selection.Keys)
                {
                    var deseleccion = noSeleccionKeys.Split('$');

                    empresa = int.Parse(deseleccion[0]);
                    producto = deseleccion[1];

                    selectAll.Remove(selectAll.FirstOrDefault(z => z.CD_PRODUTO == producto && z.CD_EMPRESA == empresa));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> { key.CD_EMPRESA.ToString(), key.CD_PRODUTO }));
                }
            }
            else
            {
                foreach (var key in context.Selection.Keys)
                {
                    resultado.Add(key);
                }
            }

            return resultado;
        }

        #endregion
    }
}