using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
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

namespace WIS.Application.Controllers.REG
{
    public class REG020PanelFamiliaProductos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG020PanelFamiliaProductos> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REG020PanelFamiliaProductos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG020PanelFamiliaProductos> logger,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_FAMILIA_PRODUTO"
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),

            }));

            return this.GridFetchRows(grid, context.FetchContext);

        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FamiliaProductoQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_FAMILIA_PRODUTO", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FamiliaProductoQuery();

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

            var dbQuery = new FamiliaProductoQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_FAMILIA_PRODUTO", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Rows.Any())
            {

                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                foreach (var row in grid.Rows)
                {
                    var codigoFamilia = row.GetCell("CD_FAMILIA_PRODUTO").Value;

                    if (row.IsNew)
                    {
                    }
                    else if (row.IsDeleted)
                    {
                        this.EliminarLineaProducto(uow, int.Parse(codigoFamilia));
                    }
                    else
                    {
                        // rows editadas

                    }
                }

            }

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "REG020Update_form_1")
            {

                var parametroCodigoFamilia = context.GetParameter("keyCodigoFamilia");
                var campoCodigoProducto = form.GetField("codigoFamilia");

                campoCodigoProducto.ReadOnly = true;

                if (!string.IsNullOrEmpty(parametroCodigoFamilia))
                {

                    ProductoFamilia productoFamilia = uow.ProductoFamiliaRepository.GetFamiliaProducto(int.Parse(parametroCodigoFamilia));

                    if (productoFamilia == null)
                    {
                        throw new ValidationFailedException("REG020_Frm1_Error_FamiliaProductoNoExiste");
                    }

                    campoCodigoProducto.Value = productoFamilia.Id.ToString();

                    form.GetField("descripcionFamilia").Value = productoFamilia.Descripcion;
                }
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoFamilia = form.GetField("codigoFamilia").Value;
            var descripcionFamilia = form.GetField("descripcionFamilia").Value;

            if (form.Id == "REG020_form_1")
            {
                // Crear nueva familia de producto
                var familia = this.CrearFamiliaProducto(uow, int.Parse(codigoFamilia), descripcionFamilia);
                uow.ProductoFamiliaRepository.AddFamiliaProducto(familia);

                uow.SaveChanges();

                context.AddSuccessNotification("REG020_Frm1_Succes_Creacion", new List<string> { familia.Id.ToString() });

            }
            else if (form.Id == "REG020Update_form_1")
            {
                // Edición de familia de producto
                var familia = this.EditarFamiliaProducto(uow, int.Parse(codigoFamilia), descripcionFamilia);
                uow.ProductoFamiliaRepository.UpdateFamiliaProducto(familia);

                uow.SaveChanges();

                context.AddSuccessNotification("REG020_Frm1_Succes_Edicion", new List<string> { familia.Id.ToString() });
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REG020FormValidationModule(uow), form, context);
        }

        public virtual ProductoFamilia CrearFamiliaProducto(IUnitOfWork uow, int codigoFamilia, string descripcionFamilia)
        {
            ProductoFamilia familia = new ProductoFamilia()
            {
                Id = codigoFamilia,
                Descripcion = descripcionFamilia,
            };

            return familia;
        }
        public virtual ProductoFamilia EditarFamiliaProducto(IUnitOfWork uow, int codigoFamilia, string descripcionFamilia)
        {
            var familia = uow.ProductoFamiliaRepository.GetFamiliaProducto(codigoFamilia);

            if (familia == null)
            {
                throw new ValidationFailedException("REG020_Frm1_Error_FamiliaProductoNoExiste");
            }

            familia.Descripcion = descripcionFamilia;

            return familia;
        }
        public virtual void EliminarLineaProducto(IUnitOfWork uow, int codigoFamilia)
        {
            var familia = uow.ProductoFamiliaRepository.GetFamiliaProducto(codigoFamilia);

            if (familia == null)
            {
                throw new ValidationFailedException("REG020_Sec0_Error_FamiliaNoEncontrada", new string[] { familia.Id.ToString() });
            }

            // Control, No debe existir ubicacion con la familia de producto indicada
            if (uow.UbicacionRepository.AnyUbicacionProductoFamilia(familia.Id))
            {
                throw new ValidationFailedException("REG020_Sec0_Error_ExisteUbicacionFamilia");
            }

            // Control, No debe existir productos con la familia asignada
            if (uow.ProductoRepository.ExisteProductoFamilia(familia.Id))
            {
                throw new ValidationFailedException("REG020_Sec0_Error_ExisteProductosFamilia");
            }

            uow.ProductoFamiliaRepository.RemoveFamiliaProducto(familia);

        }

    }
}