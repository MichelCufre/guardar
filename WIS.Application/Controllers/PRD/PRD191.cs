using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Extension;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD191 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormatProvider _culture;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public PRD191(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._culture = identity.GetFormatProvider();
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "CD_ENDERECO", "CD_PRODUTO", "CD_EMPRESA", "CD_FAIXA", "NU_ORDEN"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = true;

            var ubicacion = context.FetchContext.GetParameter("ubicacion");
            var insertableColums = new List<string>
            {
                "CD_PRODUTO", "CD_EMPRESA", "NU_ORDEN", "NU_IDENTIFICADOR", "DT_VENCIMIENTO", "QT_STOCK"
            };

            if (string.IsNullOrEmpty(ubicacion))
                insertableColums.Add("CD_ENDERECO");

            grid.SetInsertableColumns(insertableColums);

            grid = this.GridFetchRows(grid, context.FetchContext);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                ILinea linea = uow.LineaRepository.GetLineaByUbicacionSalida(ubicacion);

                if (linea != null && !string.IsNullOrEmpty(linea.NumeroIngreso))
                {
                    IIngreso ingreso = uow.ProduccionRepository.GetIngreso(linea.NumeroIngreso);

                    foreach (var salida in ingreso.Formula.Salida)
                    {
                        if (!grid.Rows.Any(d => d.GetCell("CD_PRODUTO").Value == salida.Producto && d.GetCell("CD_EMPRESA").Value == Convert.ToString(salida.Empresa)))
                        {
                            Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(salida.Empresa, salida.Producto);
                            GridRow row = grid.AddEditableRow(true);
                            GridCell cellIdentificador = row.GetCell("NU_IDENTIFICADOR");

                            row.GetCell("CD_PRODUTO").Value = producto.Codigo;
                            row.GetCell("DS_PRODUTO").Value = producto.Descripcion;
                            row.GetCell("CD_EMPRESA").Value = Convert.ToString(salida.Empresa);
                            cellIdentificador.Value = producto.GetDefaultIdentificador();

                            if (producto.IsIdentifiedByProducto())
                                cellIdentificador.Editable = false;

                            row.IsNew = true;
                        }
                    }
                }
            }

            return grid;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string ubicacion = context.GetParameter("ubicacion");
                var dbQuery = new ProduccionIdentificadorConsumirQuery(ubicacion);

                uow.HandleQuery(dbQuery);

                var sorts = new List<SortCommand> {
                    new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                    new SortCommand("NU_ORDEN", SortDirection.Ascending)
                };

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, sorts, this.GridKeys);

                var editableColumns = new List<string> { "NU_ORDEN", "NU_IDENTIFICADOR", "DT_VENCIMIENTO", "QT_STOCK" };

                if (string.IsNullOrEmpty(ubicacion))
                    editableColumns.Add("CD_ENDERECO");

                grid.SetEditableCells(editableColumns);
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                foreach (var row in grid.Rows)
                {
                    if (row.IsDeleted)
                        this.HandleDelete(uow, row, grid, context);
                    else if (row.IsNew)
                        this.HandleCreate(uow, row, grid, context);
                    else
                        this.HandleUpdate(uow, row, grid, context);
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string ubicacion = query.GetParameter("ubicacion");
            var dbQuery = new ProduccionIdentificadorConsumirQuery(ubicacion);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._gridValidationService.Validate(new IdentificadorProducirValidationModule(uow, this._culture, false), grid, row, context);
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

        #region Metodos Auxiliares

        public virtual void HandleDelete(IUnitOfWork uow, GridRow row, Grid grid, GridFetchContext context)
        {
            string ubicacion = context.GetParameter("ubicacion");
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            string producto = row.GetCell("CD_PRODUTO").Value;
            int orden = int.Parse(row.GetCell("NU_ORDEN").Value);

            if (string.IsNullOrEmpty(ubicacion))
                ubicacion = row.GetCell("CD_ENDERECO").Value;

            IdentificadorProducir identificador = uow.IdentificadorProducirRepository.GetIdentificador(ubicacion, empresa, producto, orden);

            uow.IdentificadorProducirRepository.DeleteIdentificador(identificador);
        }

        public virtual void HandleCreate(IUnitOfWork uow, GridRow row, Grid grid, GridFetchContext context)
        {
            string ubicacion = context.GetParameter("ubicacion");
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            string codigoProducto = row.GetCell("CD_PRODUTO").Value;
            Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoProducto);
            DateTime? vencimiento = producto.ManejaFechaVencimiento() ? DateTimeExtension.ParseFromIso(row.GetCell("DT_VENCIMIENTO").Value) : null;

            if (string.IsNullOrEmpty(ubicacion))
                ubicacion = row.GetCell("CD_ENDERECO").Value;

            var identificador = new IdentificadorProducir
            {
                Empresa = empresa,
                Faixa = 1,
                Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                Orden = int.Parse(row.GetCell("NU_ORDEN").Value),
                Producto = codigoProducto,
                Stock = int.Parse(row.GetCell("QT_STOCK").Value),
                Ubicacion = ubicacion,
                Vencimiento = vencimiento
            };

            uow.IdentificadorProducirRepository.AddIdentificador(identificador);
        }

        public virtual void HandleUpdate(IUnitOfWork uow, GridRow row, Grid grid, GridFetchContext context)
        {
            string ubicacion = context.GetParameter("ubicacion");
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            string codigoProducto = row.GetCell("CD_PRODUTO").Value;
            int orden = int.Parse(row.GetCell("NU_ORDEN").Old);
            int nuevoOrden = int.Parse(row.GetCell("NU_ORDEN").Value);

            if (string.IsNullOrEmpty(ubicacion))
                ubicacion = row.GetCell("CD_ENDERECO").Value;

            IdentificadorProducir identificador = uow.IdentificadorProducirRepository.GetIdentificador(ubicacion, empresa, codigoProducto, orden);

            if (nuevoOrden != orden)
            {
                uow.IdentificadorProducirRepository.DeleteIdentificador(identificador);
                this.HandleCreate(uow, row, grid, context);
            }
            else
            {
                Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoProducto);

                DateTime? vencimiento = producto.ManejaFechaVencimiento() ? DateTimeExtension.ParseFromIso(row.GetCell("DT_VENCIMIENTO").Value) : null;

                identificador.Identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                identificador.Stock = int.Parse(row.GetCell("QT_STOCK").Value);
                identificador.Vencimiento = vencimiento;

                uow.IdentificadorProducirRepository.UpdateIdentificador(identificador);
            }
        }

        public virtual List<SelectOption> SelectProducto(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            string empresaString = row.GetCell("CD_EMPRESA").Value;

            if (string.IsNullOrEmpty(empresaString) || !int.TryParse(empresaString, out int empresa))
                return opciones;

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

                foreach (Producto producto in productos)
                {
                    opciones.Add(new SelectOption(producto.Codigo, producto.Descripcion));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SelectEmpresa(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

                foreach (Empresa empresa in empresas)
                {
                    opciones.Add(new SelectOption(Convert.ToString(empresa.Id), empresa.Nombre));
                }
            }

            return opciones;
        }

        #endregion
    }
}
