using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Domain.General;
using WIS.Components.Common.Select;
using WIS.Components.Common;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Exceptions;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG602ControlesCalidadProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG602ControlesCalidadProducto> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG602ControlesCalidadProducto(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG602ControlesCalidadProducto> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_CONTROL", "CD_EMPRESA", "CD_PRODUTO"
            };
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_CONTROL", SortDirection.Descending),
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_CONTROL", OptionSelectControl()));

            grid.SetInsertableColumns(new List<string>
            {
                "CD_CONTROL",
                "CD_EMPRESA",
                "CD_PRODUTO",
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ControlesDeCalidadEnproductoQuery dbQuery;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                dbQuery = new ControlesDeCalidadEnproductoQuery(idEmpresa, idProducto);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("DS_PRODUTO").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;

                var empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);
                var descProducto = uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto.ToString());

                context.AddParameter("REG602_CD_EMPRESA", idEmpresa.ToString());
                context.AddParameter("REG602_NM_EMPRESA", empresa.Nombre);
                context.AddParameter("REG602_CD_PRODUCTO", idProducto.ToString());
                context.AddParameter("REG602_DS_PRODUCTO", descProducto);
            }
            else
            {
                dbQuery = new ControlesDeCalidadEnproductoQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("DS_PRODUTO").Hidden = false;
                grid.GetColumn("CD_PRODUTO").Hidden = false;
                grid.GetColumn("CD_EMPRESA").Hidden = false;
                grid.GetColumn("NM_EMPRESA").Hidden = false;
            }

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string> { "CD_EMPRESA", "CD_PRODUTO", "CD_CONTROL" });
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDeCalidadEnproductoQuery();

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = context.Parameters.FirstOrDefault(p => p.Id.Equals("producto")).Value;

                dbQuery = new ControlesDeCalidadEnproductoQuery(idEmpresa, idProducto);
            }

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDeCalidadEnproductoQuery();

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = context.Parameters.FirstOrDefault(p => p.Id.Equals("producto")).Value;

                dbQuery = new ControlesDeCalidadEnproductoQuery(idEmpresa, idProducto);
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

            try
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var codigo = row.GetCell("CD_CONTROL").Value;
                        var empresa = row.GetCell("CD_EMPRESA").Value;
                        var producto = row.GetCell("CD_PRODUTO").Value;

                        if (row.IsNew)
                            CrearAsociacionControlDeCalidadProducto(uow, codigo, empresa, producto);
                        else if (row.IsDeleted)
                            EliminarAsociacionControlDeCalidadProducto(uow, row, context);
                        else
                            EditarAsociacionControlDeCalidadProducto(uow, row, context);
                    }

                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "GridCommit");
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG035GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Any(s => s.Id == "producto") && context.Parameters.Any(s => s.Id == "empresa"))
            {
                var idProducto = context.Parameters.FirstOrDefault(p => p.Id.Equals("producto")).Value;
                var idEmpresa = context.Parameters.FirstOrDefault(p => p.Id.Equals("empresa")).Value;
                return this._gridValidationService.Validate(new MantenimientoControlDeCalidadEnProductoClaseGridValidationModule(uow, idEmpresa, idProducto), grid, row, context);
            }

            return this._gridValidationService.Validate(new MantenimientoControlDeCalidadEnProductoClaseGridValidationModule(uow), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_EMPRESA":
                    return SearchEmpresa(grid, row, context);
                case "CD_PRODUTO":
                    return SearchProduto(grid, row, context);
            }

            return new List<SelectOption>();
        }

        #region Metodos auxiliares

        public virtual ControlDeCalidadProducto CrearAsociacionControlDeCalidadProducto(IUnitOfWork uow, string codigo, string empresa, string producto)
        {
            var clase = new ControlDeCalidadProducto()
            {
                Codigo = int.Parse(codigo),
                Empresa = int.Parse(empresa),
                Producto = producto.ToUpper(),
            };

            uow.ControlDeCalidadRepository.AddControlDeCalidadProducto(clase);

            return clase;
        }
        public virtual void EliminarAsociacionControlDeCalidadProducto(IUnitOfWork uow, GridRow row, GridFetchContext context)
        {
            string idControl = row.GetCell("CD_CONTROL").Value;
            string idEmpresa = context.Parameters.Any(s => s.Id == "empresa") ? context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;
            string idProducto = context.Parameters.Any(s => s.Id == "producto") ? context.Parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;

            if (!uow.ControlDeCalidadRepository.AnyControlDeCalidadProducto(int.Parse(idControl), int.Parse(idEmpresa), idProducto))
                throw new EntityNotFoundException("REG602_Frm1_Error_ClaseControlDeCalidadEnProductoNoExiste");

            if (uow.ControlDeCalidadRepository.AnyControlDeCalidadPendienteSinAceptar(int.Parse(idControl), int.Parse(idEmpresa), idProducto))
                throw new ValidationFailedException("REG602_Frm1_Error_ExisteControlDeCalidadPendiente");

            var clase = uow.ControlDeCalidadRepository.GetControlDeCalidadProducto(int.Parse(idControl), int.Parse(idEmpresa), idProducto);
            uow.ControlDeCalidadRepository.RemoveControlDeCalidadProducto(clase);

        }
        public virtual ControlDeCalidadProducto EditarAsociacionControlDeCalidadProducto(IUnitOfWork uow, GridRow row, GridFetchContext context)
        {
            var idControl = row.GetCell("CD_CONTROL").Value;
            var idEmpresa = row.GetCell("CD_EMPRESA").Value;
            var idProducto = row.GetCell("CD_PRODUTO").Value;

            var controlViejo = row.GetCell("CD_CONTROL").Old;
            var prodViejo = row.GetCell("CD_PRODUTO").Old;
            var empresaVieja = row.GetCell("CD_EMPRESA").Old;

            var controlOld = uow.ControlDeCalidadRepository.GetControlDeCalidadProducto(int.Parse(controlViejo), int.Parse(empresaVieja), prodViejo);

            if (controlOld == null)
                throw new ValidationFailedException("REG602_Frm1_Error_ClaseControlDeCalidadEnProductoNoExiste", new String[] { });

            if (uow.ControlDeCalidadRepository.AnyControlDeCalidadPendienteSinAceptar(int.Parse(controlViejo), int.Parse(empresaVieja), prodViejo))
                throw new ValidationFailedException("REG602_Frm1_Error_ExisteControlDeCalidadPendiente");

            var controlNuevo = new ControlDeCalidadProducto()
            {
                Codigo = int.Parse(idControl),
                Empresa = int.Parse(idEmpresa),
                Producto = idProducto.ToUpper(),
                FechaInsercion = controlOld.FechaInsercion,

            };

            uow.ControlDeCalidadRepository.RemoveControlDeCalidadProducto(controlOld);
            uow.ControlDeCalidadRepository.AddControlDeCalidadProducto(controlNuevo);

            return controlNuevo;
        }

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(context.SearchValue, this._identity.UserId);

            foreach (var emp in empresasAsignadasUsuario)
            {
                opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchProduto(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = new List<Producto>();

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(idEmpresa, context.SearchValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value))
                    productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(int.Parse(row.GetCell("CD_EMPRESA").Value), context.SearchValue);
                else
                    row.GetCell("CD_EMPRESA").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));
            }

            foreach (var prod in productos)
            {
                opciones.Add(new SelectOption(prod.Codigo, $"{prod.Codigo} - {prod.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectControl()
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var tiposControlCalidad = uow.ControlDeCalidadRepository.GetTiposControlCalidad();

            foreach (var control in tiposControlCalidad)
            {
                opciones.Add(new SelectOption(control.Id.ToString(), $"{control.Id} - {control.Descripcion}"));
            }

            return opciones;
        }

        #endregion
    }
}