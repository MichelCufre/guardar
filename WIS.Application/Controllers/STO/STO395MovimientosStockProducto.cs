using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Exceptions;
using WIS.Extension;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO395MovimientosStockProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected List<string> GridKeys { get; }

        public STO395MovimientosStockProducto(
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
                "DT_ADDROW",
                "NU_DOCTO",
                "NU_DOCTO_EXT",
                "CD_PRODUTO",
                "CD_EMPRESA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._security = security;
            _filterInterpreter = filterInterpreter;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            var fieldProducto = form.GetField("CD_PRODUTO");
            fieldProducto.ReadOnly = false;

            var fieldEmpresa = form.GetField("CD_EMPRESA");
            fieldEmpresa.ReadOnly = false;

            var cdProducto = query.GetParameter("CD_PRODUTO");
            var cdEmpresa = query.GetParameter("CD_EMPRESA")?.ToNumber<int?>();

            //Parametros por URL
            if (int.TryParse(query.Parameters.FirstOrDefault(z => z.Id == "empresa")?.Value, out int idEmpresa))
                cdEmpresa = idEmpresa;

            if (!string.IsNullOrEmpty(query.Parameters.FirstOrDefault(z => z.Id == "producto")?.Value))
                cdProducto = query.Parameters.FirstOrDefault(z => z.Id == "producto")?.Value;


            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(cdProducto) && cdEmpresa != null)
            {
                fieldProducto.ReadOnly = true;
                fieldProducto.Value = cdProducto;
                fieldProducto.Options = new List<SelectOption> { new SelectOption(fieldProducto.Value, uow.ProductoRepository.GetDescripcion((int)cdEmpresa, cdProducto)) };

                fieldEmpresa.ReadOnly = true;
                fieldEmpresa.Value = cdEmpresa.ToString();
                fieldEmpresa.Options = new List<SelectOption> { new SelectOption(fieldEmpresa.Value, uow.EmpresaRepository.GetNombre((int)cdEmpresa)) };

            }
            else
            {
                var empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

                if (empresa != null)
                {
                    fieldEmpresa.ReadOnly = true;
                    fieldEmpresa.Value = empresa.Id.ToString();
                    fieldEmpresa.Options = new List<SelectOption> { new SelectOption(fieldEmpresa.Value, empresa.Nombre) };
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            context.AddParameter("CD_EMPRESA", form.GetField("CD_EMPRESA").Value);
            context.AddParameter("CD_PRODUTO", form.GetField("CD_PRODUTO").Value);
            context.AddParameter("DT_INICIO", form.GetField("DT_INICIO").Value);
            context.AddParameter("DT_FIN", form.GetField("DT_FIN").Value);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new STO395FormValidationModule(uow), form, context);
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


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsRollbackEnabled = false;
            context.IsCommitEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>() {
                new GridButton("btnDetalle", "STO395_grid1_btn_Detalle", "fas fa-list"),
            }));

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            if (!context.HasParameter("CD_PRODUTO") || !context.HasParameter("CD_EMPRESA"))
                return grid;

            var producto = context.GetParameter("CD_PRODUTO");
            var empresa = int.Parse(context.GetParameter("CD_EMPRESA"));

            DateTime? fechaInicio = DateTimeExtension.ParseFromIso(context.GetParameter("DT_INICIO"));
            DateTime? fechaFin = DateTimeExtension.ParseFromIso(context.GetParameter("DT_FIN"))?.Date.AddDays(1).AddSeconds(-1);

            if (!this._security.IsEmpresaAllowed(empresa))
                throw new OperationNotAllowedException("General_Sec0_Error_UsuarioNoManejaEmpresa");

            context.Parameters.Clear();

            var dbQuery = new MovimientosStockQuery(empresa, producto, fechaInicio, fechaFin, this._identity.Predio);

            uow.HandleQuery(dbQuery);

            context.AddParameter("QT_STOCK_GENERAL", Convert.ToString(uow.StockRepository.GetTotalStockGeneralByProductoEmpresaPredio(empresa, producto, this._identity.Predio)));
            context.AddParameter("QT_STOCK_INICIO_PERIODO", Convert.ToString(dbQuery.GetCantidadStockInicioPeriodo()));
            context.AddParameter("QT_STOCK_FIN_PERIODO", Convert.ToString(dbQuery.GetCantidadStockFinalPeriodo()));

            dbQuery.FilterByDate();

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoMovimiento = short.Parse(context.Row.GetCell("CD_MOVIMIENTO").Value);

            var tpMovProducto = uow.StockRepository.GetTipoMovimientoProducto(codigoMovimiento);

            switch (tpMovProducto?.Categoria)
            {
                case TipoMovimientoProducto.Ingreso:

                    context.Redirect("/recepcion/REC171", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "producto", Value = context.Row.GetCell("CD_PRODUTO").Value},
                        new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value},
                        new ComponentParameter(){ Id = "agenda", Value = context.Row.GetCell("NU_DOCTO").Value},
                    });

                    break;
                case TipoMovimientoProducto.Salida:

                    context.Redirect("/expedicion/EXP041", true, new List<ComponentParameter>()
                    {
                      new ComponentParameter(){ Id = "producto", Value = context.Row.GetCell("CD_PRODUTO").Value},
                      new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value},
                      new ComponentParameter(){ Id = "pedido", Value = context.Row.GetCell("NU_DOCTO").Value}
                    });

                    break;
                case TipoMovimientoProducto.Stock:

                    context.Redirect("/inventario/INV030", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "producto", Value = context.Row.GetCell("CD_PRODUTO").Value},
                        new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value},
                        new ComponentParameter(){ Id = "documento", Value = context.Row.GetCell("NU_DOCTO").Value},
                        new ComponentParameter(){ Id = "fechaRealizado", Value = context.Row.GetCell("DT_ADDROW").Value},
                    });

                    break;
                case TipoMovimientoProducto.Produccion:

                    context.Redirect("/produccion/PRD113", true, new List<ComponentParameter>
                    {
                        new ComponentParameter("nuIngresoProduccion", context.Row.GetCell("NU_DOCTO").Value)
                    });

                    break;
                default:
                    break;
            }

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "CD_PRODUTO")?.Value) || string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "CD_EMPRESA")?.Value))
                return null;

            string producto = context.GetParameter("CD_PRODUTO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));

            DateTime? fechaInicio = DateTimeExtension.ParseFromIso(context.GetParameter("DT_INICIO"));
            DateTime? fechaFin = DateTimeExtension.ParseFromIso(context.GetParameter("DT_FIN"))?.Date.AddDays(1).AddSeconds(-1);

            if (!this._security.IsEmpresaAllowed(empresa))
                throw new OperationNotAllowedException("General_Sec0_Error_UsuarioNoManejaEmpresa");

            context.Parameters.Clear();

            var dbQuery = new MovimientosStockQuery(empresa, producto, fechaInicio, fechaFin, this._identity.Predio);

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

            if (!context.HasParameter("CD_PRODUTO") || !context.HasParameter("CD_EMPRESA"))
                return null;

            if (string.IsNullOrEmpty(context.GetParameter("CD_PRODUTO")) && string.IsNullOrEmpty(context.GetParameter("CD_EMPRESA")))
                throw new OperationNotAllowedException("STO395_Sec0_Error_ExportExcel");


            string producto = context.GetParameter("CD_PRODUTO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));

            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;
            var fechaInicioAux = context.GetParameter("DT_INICIO");
            var fechaFinAux = context.GetParameter("DT_FIN");

            if (!string.IsNullOrEmpty(fechaInicioAux))
                fechaInicio = DateTimeExtension.ParseFromIso(fechaInicioAux);
            if (!string.IsNullOrEmpty(fechaFinAux))
                fechaFin = DateTimeExtension.ParseFromIso(fechaFinAux)?.Date.AddDays(1).AddSeconds(-1);

            var dbQuery = new MovimientosStockQuery(empresa, producto, fechaInicio, fechaFin, this._identity.Predio);

            uow.HandleQuery(dbQuery);
            dbQuery.FilterByDate();

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), empresa.Id + " - " + empresa.Nombre));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProducto(Form form, FormSelectSearchContext context)
        {
            FormField fieldEmpresa = form.GetField("CD_EMPRESA");

            List<SelectOption> options = new List<SelectOption>();

            if (string.IsNullOrEmpty(fieldEmpresa.Value))
                return options;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(fieldEmpresa.Value, out int cdEmpresa))
            {
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(cdEmpresa, context.SearchValue, permiteProductosInactivos: true);

                foreach (var producto in productos)
                {
                    options.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
                }
            }

            return options;
        }

        #endregion
    }
}
