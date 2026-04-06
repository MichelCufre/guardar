using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.OrdenTarea;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
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

namespace WIS.Application.Controllers.ORT
{
    public class ORT020Insumos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<ORT020Insumos> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ISecurityService _security;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT020Insumos(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<ORT020Insumos> logger,
            IGridValidationService gridValidationService,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "CD_INSUMO_MANIPULEO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_INSUMO_MANIPULEO", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_INSUMO_MANIPULEO",
                "DS_INSUMO_MANIPULEO",
                "NU_COMPONENTE",
                "TP_INSUMO_MANIPULEO",
                "CD_PRODUTO",
                "CD_EMPRESA",
                "FL_TODA_EMPRESA"
            });

            using var uow = this._uowFactory.GetUnitOfWork();

            if (this._security.IsUserAllowed(SecurityResources.ORT020AsignarEmpresas_Page_Access_Allow))
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
                {
                    new GridButton("btnAsignarEmpresas", "General_Sec0_btn_AsignarEmpresas", "fas fa-list-ul")
                }));
            }

            //Cargo select
            grid.AddOrUpdateColumn(new GridColumnSelect("TP_INSUMO_MANIPULEO", this.SelectInsumoManipuleo(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InsumosQuery dbQuery = new InsumosQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                if (row.GetCell("TP_INSUMO_MANIPULEO").Value == OrdenTareaDb.TipoInsumo)
                {
                    row.SetEditableCells(new List<string> {
                        "DS_INSUMO_MANIPULEO",
                        "NU_COMPONENTE",
                        "CD_PRODUTO",
                        "CD_EMPRESA",
                        "FL_TODA_EMPRESA"
                    });
                }
                else
                {
                    row.SetEditableCells(new List<string> {
                        "DS_INSUMO_MANIPULEO",
                        "NU_COMPONENTE",
                        "FL_TODA_EMPRESA"
                    });
                }

                if (row.GetCell("FL_TODA_EMPRESA").Value == "S")
                {
                    row.DisabledButtons.Add("btnAsignarEmpresas");
                }
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionInsumosManipuleos registroModificacionIM = new RegistroModificacionInsumosManipuleos(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            InsumoManipuleo insumoManipuleo = this.CrearInsumoManipuleo(uow, row, query);
                            registroModificacionIM.RegistrarInsumoManipuleo(insumoManipuleo);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteInsumoManipuleo(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            InsumoManipuleo insumoManipuleo = this.UpdateInsumoManipuleo(uow, row, query);
                            registroModificacionIM.ModificarInsumoManipuleo(insumoManipuleo);
                        }

                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogDebug($"Error {ex.Message} - {ex}");
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF010GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoInsumoGridValidationModule(uow, _security), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO": return this.SelectProducto(row, grid, context);
                case "CD_EMPRESA": return this.SelectEmpresa(row, grid, context);
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InsumosQuery dbQuery = new InsumosQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InsumosQuery dbQuery = new InsumosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SelectInsumoManipuleo(IUnitOfWork uow)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            var dominios = uow.DominioRepository.GetDominios("TPINSMAN");

            foreach (var dominio in dominios)
            {
                opciones.Add(new SelectOption(dominio.Valor, dominio.Valor + " - " + dominio.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> SelectProducto(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            string empresaString = row.GetCell("CD_EMPRESA").Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(empresaString) || !int.TryParse(empresaString, out int empresa))
                return opciones;

            List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

            foreach (Producto producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, producto.Descripcion));
            }


            return opciones;
        }

        public virtual List<SelectOption> SelectEmpresa(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (Empresa empresa in empresas)
            {
                opciones.Add(new SelectOption(Convert.ToString(empresa.Id), empresa.Nombre));
            }


            return opciones;
        }

        public virtual InsumoManipuleo CrearInsumoManipuleo(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string empresaString = row.GetCell("CD_EMPRESA").Value;

            InsumoManipuleo nuevoInsumoManipuleo = new InsumoManipuleo();

            nuevoInsumoManipuleo.Id = row.GetCell("CD_INSUMO_MANIPULEO").Value;
            nuevoInsumoManipuleo.Descripcion = row.GetCell("DS_INSUMO_MANIPULEO").Value;
            nuevoInsumoManipuleo.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            nuevoInsumoManipuleo.FlTodaEmpresa = row.GetCell("FL_TODA_EMPRESA").Value;
            nuevoInsumoManipuleo.Tipo = row.GetCell("TP_INSUMO_MANIPULEO").Value;

            if (!string.IsNullOrEmpty(empresaString))
            {
                nuevoInsumoManipuleo.Producto = row.GetCell("CD_PRODUTO").Value;
                nuevoInsumoManipuleo.Empresa = int.Parse(empresaString);
            }

            return nuevoInsumoManipuleo;
        }

        public virtual InsumoManipuleo UpdateInsumoManipuleo(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string cdInsumoManipuleo = row.GetCell("CD_INSUMO_MANIPULEO").Value;
            string empresaString = row.GetCell("CD_EMPRESA").Value;

            InsumoManipuleo insumoManipuleo = uow.InsumoManipuleoRepository.GetInsumoManipuleo(cdInsumoManipuleo);

            insumoManipuleo.Descripcion = row.GetCell("DS_INSUMO_MANIPULEO").Value;
            insumoManipuleo.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            insumoManipuleo.FlTodaEmpresa = row.GetCell("FL_TODA_EMPRESA").Value;

            if (insumoManipuleo.FlTodaEmpresa == "S")
            {
                uow.InsumoManipuleoRepository.RemoverTodasEmpresas(cdInsumoManipuleo);
            }

            if (!string.IsNullOrEmpty(empresaString))
            {
                insumoManipuleo.Producto = row.GetCell("CD_PRODUTO").Value;
                insumoManipuleo.Empresa = int.Parse(empresaString);
            }
            else
            {
                insumoManipuleo.Empresa = null;
                insumoManipuleo.Producto = null;
            }

            return insumoManipuleo;
        }

        public virtual void DeleteInsumoManipuleo(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string cdInsumoManipuleo = row.GetCell("CD_INSUMO_MANIPULEO").Value;

            if (!uow.TareaRepository.AnyTareaConInsumoManipuleo(cdInsumoManipuleo))
            {
                uow.InsumoManipuleoRepository.DeleteInsumoManipuleo(cdInsumoManipuleo);
            }
            else
            {
                throw new ValidationFailedException("ORT020_Sec0_Error_InsumoManipuleoUtilizado");
            }
        }

        #endregion
    }
}
