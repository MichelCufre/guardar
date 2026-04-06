using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Registro;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG605CapacidadDePallet : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG605CapacidadDePallet> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG605CapacidadDePallet(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REG605CapacidadDePallet> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_FAIXA",
                "CD_PALLET",
                "CD_PRODUTO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = true;

            using var uow = this._uowFactory.GetUnitOfWork();

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_PALLET", this.SelectCodigosPallets(uow)));

            var listaInsertables = new List<string>
            {
                "CD_PALLET",
                "NU_PRIORIDAD",
                "QT_UNIDADES",
            };

            if (context.FetchContext.Parameters.Count < 1)
            {
                listaInsertables.Add("CD_EMPRESA");
                listaInsertables.Add("CD_PRODUTO");
            }

            grid.SetInsertableColumns(listaInsertables);

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            CapacidaddDePalletQuery dbQuery;

            if (context.Parameters.Count > 1)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int cdEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(x => x.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var cdProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                dbQuery = new CapacidaddDePalletQuery(cdEmpresa, cdProducto);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("DS_PRODUTO").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;

                var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
                var descProducto = uow.ProductoRepository.GetDescripcion(cdEmpresa, cdProducto);

                context.AddParameter("REG605_CD_EMPRESA", cdEmpresa.ToString());
                context.AddParameter("REG605_NM_EMPRESA", empresa.Nombre);
                context.AddParameter("REG605_CD_PRODUCTO", cdProducto);
                context.AddParameter("REG605_DS_PRODUCTO", descProducto);
            }
            else
            {
                grid.GetColumn("DS_PRODUTO").Hidden = false;
                grid.GetColumn("CD_PRODUTO").Hidden = false;
                grid.GetColumn("CD_EMPRESA").Hidden = false;
                grid.GetColumn("NM_EMPRESA").Hidden = false;

                dbQuery = new CapacidaddDePalletQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            grid.SetEditableCells(new List<string>
            {
                "NU_PRIORIDAD",
                "QT_UNIDADES",
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CapacidaddDePalletQuery dbQuery = new CapacidaddDePalletQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CapacidaddDePalletQuery dbQuery = new CapacidaddDePalletQuery();

            if (context.Parameters.Count > 1)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;

                dbQuery = new CapacidaddDePalletQuery(idEmpresa, idProducto);

            }
            else if (context.Parameters.Count > 0)
            {
                if (!context.Parameters.Any(s => s.Id == "empresa"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idEmpresa = context.Parameters.Any(s => s.Id == "empresa") ? context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : string.Empty;

            }
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new RegistroCapacidadDePalletVlidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
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

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                            CrearProductoPallet(uow, row, context);
                        else
                        {
                            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                            var producto = row.GetCell("CD_PRODUTO").Value;
                            var codigoPallet = short.Parse(row.GetCell("CD_PALLET").Value);
                            var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value);

                            var productoPallet = uow.ProductoRepository.GetProductoPallet(empresa, producto, codigoPallet, faixa);

                            if (productoPallet == null)
                                throw new ValidationFailedException("REG605_Sec0_Error_CapacidadPalletNoExisteEliminar");

                            if (row.IsDeleted)
                                uow.ProductoRepository.RemoveProductoPallet(productoPallet);
                            else
                            {
                                productoPallet.Unidades = decimal.Parse(row.GetCell("QT_UNIDADES").Value, this._identity.GetFormatProvider());
                                productoPallet.Prioridad = short.Parse(row.GetCell("NU_PRIORIDAD").Value);

                                uow.ProductoRepository.UpdateProductoPallet(productoPallet);
                            }
                        }
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, "REG605CapacidadDePallet - GridCommit");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG605CapacidadDePallet - GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
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

        #region Metodos Auxiliares

        public virtual void CrearProductoPallet(IUnitOfWork uow, GridRow row, GridFetchContext context)
        {
            var newPalletProducto = new ProductoPallet
            {
                Empresa = context.Parameters.Any(s => s.Id == "empresa") ? int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value) : int.Parse(row.GetCell("CD_EMPRESA").Value),
                CodigoProducto = context.Parameters.Any(s => s.Id == "producto") ? context.Parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value,
                CodigoPallet = short.Parse(row.GetCell("CD_PALLET").Value),
                Unidades = decimal.Parse(row.GetCell("QT_UNIDADES").Value, this._identity.GetFormatProvider()),
                Prioridad = short.Parse(row.GetCell("NU_PRIORIDAD").Value)
            };

            uow.ProductoRepository.AddProductoPallet(newPalletProducto);
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

        public virtual List<SelectOption> SelectCodigosPallets(IUnitOfWork uow)
        {
            return uow.FacturacionRepository.GetPallets()
                .Select(w => new SelectOption(w.Id.ToString(), $"{w.Id.ToString()} - {w.Descripcion}"))
                .ToList();
        }

        #endregion
    }
}
