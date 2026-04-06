using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO005ConsultaStockGeneral : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected List<string> GridKeys { get; }

        public STO005ConsultaStockGeneral(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_ENDERECO",
                "CD_EMPRESA",
                "CD_PRODUTO",
                "NU_IDENTIFICADOR",
                "CD_FAIXA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._security = security;
            _filterInterpreter = filterInterpreter;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            context.AddParameter("CD_EMPRESA", form.GetField("CD_EMPRESA").Value);
            context.AddParameter("CD_PRODUTO", form.GetField("CD_PRODUTO").Value);

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new STO005FormValidationModule(uow), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "CD_EMPRESA": return this.SearchEmpresa(form, context);
                case "CD_PRODUTO": return this.SearchProducto(form, context);
            }

            return new List<SelectOption>();
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext query)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(query.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), empresa.Id + " - " + empresa.Nombre));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchProducto(Form form, FormSelectSearchContext query)
        {
            FormField fieldEmpresa = form.GetField("CD_EMPRESA");

            List<SelectOption> options = new List<SelectOption>();

            if (string.IsNullOrEmpty(fieldEmpresa.Value))
                return options;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(fieldEmpresa.Value, out int cdEmpresa))
            {
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(cdEmpresa, query.SearchValue, permiteProductosInactivos: true);

                foreach (var producto in productos)
                {
                    options.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
                }
            }

            return options;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("CD_ENDERECO", SortDirection.Ascending);

            using var uow = this._uowFactory.GetUnitOfWork();

            if (!context.HasParameter("CD_PRODUTO") || !context.HasParameter("CD_EMPRESA"))
                return grid;

            string codigoProducto = context.GetParameter("CD_PRODUTO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));

            if (!this._security.IsEmpresaAllowed(empresa))
                throw new OperationNotAllowedException("General_Sec0_Error_UsuarioNoManejaEmpresa");

            context.Parameters.Clear();

            var dbQuery = new StockGeneralQuery(empresa, codigoProducto);

            uow.HandleQuery(dbQuery);

            StockGeneral stock = dbQuery.GetTotalStockGeneral();

            Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoProducto);

            stock.SetDatosProducto(producto);

            context.Parameters.Add(new ComponentParameter("datosStockGeneral", JsonConvert.SerializeObject(stock)));

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!query.HasParameter("CD_PRODUTO") || !query.HasParameter("CD_EMPRESA"))
                return null;

            string codigoProducto = query.GetParameter("CD_PRODUTO");
            int empresa = int.Parse(query.GetParameter("CD_EMPRESA"));

            if (!this._security.IsEmpresaAllowed(empresa))
                throw new OperationNotAllowedException("General_Sec0_Error_UsuarioNoManejaEmpresa");

            query.Parameters.Clear();

            var dbQuery = new StockGeneralQuery(empresa, codigoProducto);

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

            if (!context.HasParameter("CD_PRODUTO") || !context.HasParameter("CD_EMPRESA"))
                return null;

            string producto = context.GetParameter("CD_PRODUTO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));

            var dbQuery = new StockGeneralQuery(empresa, producto);

            uow.HandleQuery(dbQuery);

            SortCommand defaultSort = new SortCommand("CD_ENDERECO", SortDirection.Ascending);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
    }
}
