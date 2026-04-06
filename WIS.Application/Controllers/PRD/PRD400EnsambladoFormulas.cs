using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Common.Extensions;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Produccion;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD400EnsambladoFormulas : AppController
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IBarcodeService _barcodeService;

        public PRD400EnsambladoFormulas(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, IGridService gridService, IFilterInterpreter filterInterpreter, IBarcodeService barcodeService)
        {
            _formValidationService = formValidationService;
            _identity = identity;
            _uowFactory = uowFactory;
            _gridService = gridService;
            _filterInterpreter = filterInterpreter;
            _barcodeService = barcodeService;
        }

        #region Keys

        protected List<string> GridProductosEntradaKeys => new List<string> { "CD_PRDC_DEFINICION", "NU_PREPARACION", "NU_CONTENEDOR", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA" };
        protected List<SortCommand> GridProductosEntradaSort => new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Descending) };
        protected List<string> GridProductosSobrantesKeys => new List<string> { "CD_PRDC_DEFINICION", "NU_PREPARACION", "NU_CONTENEDOR", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA" };
        protected List<SortCommand> GridProductosSobrantesSort => new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Ascending) };
        protected List<string> GridProductosSalidaKeys => new List<string> { "CD_PRDC_DEFINICION", "NU_PREPARACION", "NU_CONTENEDOR", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA" };
        protected List<SortCommand> GridProductosSalidaSort => new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Ascending) };
        #endregion

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "PRD400_grid_ProductosEntrada": return this.GridProductosEntradaInitialize(grid, context);
                case "PRD400_grid_ProductosSobrantes": return this.GridProductosSobrantesInitialize(grid, context);
                case "PRD400_grid_ProductosSalida": return this.GridProductosSalidaInitialize(grid, context);
            }

            return grid;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            switch (grid.Id)
            {
                case "PRD400_grid_ProductosEntrada": return this.GridProductosEntradaFetchRows(grid, context);
                case "PRD400_grid_ProductosSobrantes": return this.GridProductosSobrantesFetchRows(grid, context);
                case "PRD400_grid_ProductosSalida": return this.GridProductosSalidaFetchRows(grid, context);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            return new byte[0];
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            switch (grid.Id)
            {
                case "PRD400_grid_ProductosEntrada": return this.GridProductosEntradaFetchStats(grid, query);
                case "PRD400_grid_ProductosSobrantes": return this.GridProductosSobrantesFetchStats(grid, query);
                case "PRD400_grid_ProductosSalida": return this.GridProductosSalidaFetchStats(grid, query);
            }

            return null;
        }


        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            try
            {
                query.ResetForm = true;
                using var uow = this._uowFactory.GetUnitOfWork();

                var fieldContenedor = form.GetField("NuContenedor");

                _barcodeService.ValidarEtiquetaContenedor(fieldContenedor.Value, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);
                fieldContenedor.Value = string.Empty;

                query.AddParameter("NuContenedor", datosContenedor.NuContenedor.ToString());
                query.AddParameter("NuPreparacion", datosContenedor.NuPreparacion.ToString());
                query.AddParameter("summary", uow.PedidoRepository.GetInformacionPedidoEnsambladoPorContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor).ToList()?.Serialize());

            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(query.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == 0 || filtros.NU_CONTENEDOR == 0)
            {
                return form;
            }

            using var uow = this._uowFactory.GetUnitOfWork();
            try
            {
                uow.CreateTransactionNumber("Producir ensamblados de formula");
                uow.BeginTransaction();

                var pedidoEnsamblado = uow.PedidoRepository.GetPedidoEnsambladoPorContenedor((int)filtros.NU_PREPARACION, (int)filtros.NU_CONTENEDOR);
                var pedidoOriginal = uow.PedidoRepository.GetPedidoOriginalEnsambladoPorPedido(pedidoEnsamblado);
                var contenedor = uow.ContenedorRepository.GetContenedor((int)filtros.NU_PREPARACION, (int)filtros.NU_CONTENEDOR);

                EnsambladoFormulaContenedor ensamblarFormula = new EnsambladoFormulaContenedor(uow, pedidoEnsamblado, pedidoOriginal, contenedor, _identity.UserId, _identity.Predio, _barcodeService);

                var colInsumos = uow.ProduccionRepository.GetInsumosPorContenedor(contenedor.NumeroPreparacion, contenedor.Numero);

                if ((colInsumos?.Count ?? 0) == 0)
                {
                    throw new Exception("PRD400_Op_Error_NoHayInsumosEnElContenedor");
                }

                var colProductosFinales = uow.ProduccionRepository.GetProductosFinalesPorContenedor(contenedor.NumeroPreparacion, contenedor.Numero, colInsumos.FirstOrDefault().CD_PRDC_DEFINICION);


                if ((colProductosFinales?.Count ?? 0) == 0)
                {
                    throw new Exception("PRD400_Op_Error_NoSePuedenProducirProductosConLosInsumosDelContenedor");
                }

                long? cargaOriginal = uow.PreparacionRepository.GetCarga(contenedor.NumeroPreparacion, contenedor.Numero);

                if (cargaOriginal == null)
                {
                    throw new Exception("PRD400_Op_Error_NoSePudoEncontrarLaCargaInicial");
                }

                ensamblarFormula.ProcesarInsumos(colInsumos);
                ensamblarFormula.ProcesarProductosFinales(colProductosFinales, (int)cargaOriginal);

                bool seFinalizoLaProduccion = false;

                try
                {
                    ensamblarFormula.FinalizarProduccionEnsamblado();
                    seFinalizoLaProduccion = true;
                }
                catch (Exception ex)
                {
                    query.AddInfoNotification(ex.Message);
                }

                uow.SaveChanges();
                uow.Commit();

                query.AddSuccessNotification("PRD400_Op_Ok_LaProduccionFueRealizada");

                if (seFinalizoLaProduccion)
                    query.AddSuccessNotification("PRD400_Op_Ok_LaProduccionFueFinalizada");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.Error(ex, ex.Message);
                throw ex;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CodigoBarrasContenedorEnsambladoFormulaFormValidationModule(uow, this._identity, _barcodeService), form, context);
        }

        #region Metodos Auxiliares

        public virtual Grid GridProductosEntradaInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridProductosEntradaFetchRows(grid, context.FetchContext);
        }
        public virtual Grid GridProductosSobrantesInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridProductosSobrantesFetchRows(grid, context.FetchContext);
        }
        public virtual Grid GridProductosSalidaInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridProductosSalidaFetchRows(grid, context.FetchContext);
        }

        public virtual Grid GridProductosSalidaFetchRows(Grid grid, GridFetchContext context)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(context.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == 0 || filtros.NU_CONTENEDOR == 0)
            {
                grid.Rows.Clear();
                return grid;
            }

            if (filtros.NU_PREPARACION != null && filtros.NU_CONTENEDOR != null)
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                ProductosSalidaContenedorQuery dbQuery = null;

                dbQuery = new ProductosSalidaContenedorQuery((int)filtros.NU_PREPARACION, (int)filtros.NU_CONTENEDOR);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridProductosEntradaSort, this.GridProductosEntradaKeys);
            }

            return grid;
        }
        public virtual Grid GridProductosEntradaFetchRows(Grid grid, GridFetchContext context)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(context.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == null || filtros.NU_CONTENEDOR == null)
            {
                grid.Rows.Clear();
                return grid;
            }

            using var uow = this._uowFactory.GetUnitOfWork();
            ProductosEntradaContenedorQuery dbQuery = null;

            dbQuery = new ProductosEntradaContenedorQuery(filtros?.NU_PREPARACION, filtros?.NU_CONTENEDOR);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridProductosEntradaSort, this.GridProductosEntradaKeys);

            return grid;
        }
        public virtual Grid GridProductosSobrantesFetchRows(Grid grid, GridFetchContext context)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(context.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == 0 || filtros.NU_CONTENEDOR == 0)
            {
                grid.Rows.Clear();
                return grid;
            }

            if (filtros.NU_PREPARACION != null && filtros.NU_CONTENEDOR != null)
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                ProductosSobranteContenedorQuery dbQuery = null;

                dbQuery = new ProductosSobranteContenedorQuery((int)filtros.NU_PREPARACION, (int)filtros.NU_CONTENEDOR);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridProductosEntradaSort, this.GridProductosEntradaKeys);
            }

            return grid;
        }

        public virtual GridStats GridProductosEntradaFetchStats(Grid grid, GridFetchStatsContext query)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(query.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == null || filtros.NU_CONTENEDOR == null)
            {
                grid.Rows.Clear();
                return null;
            }

            using var uow = this._uowFactory.GetUnitOfWork();
            ProductosEntradaContenedorQuery dbQuery = null;

            dbQuery = new ProductosEntradaContenedorQuery(filtros?.NU_PREPARACION, filtros?.NU_CONTENEDOR);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats GridProductosSobrantesFetchStats(Grid grid, GridFetchStatsContext query)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(query.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == 0 || filtros.NU_CONTENEDOR == 0)
            {
                grid.Rows.Clear();
                return null;
            }

            if (filtros.NU_PREPARACION != null && filtros.NU_CONTENEDOR != null)
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                ProductosSobranteContenedorQuery dbQuery = null;

                dbQuery = new ProductosSobranteContenedorQuery((int)filtros.NU_PREPARACION, (int)filtros.NU_CONTENEDOR);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;


        }
        public virtual GridStats GridProductosSalidaFetchStats(Grid grid, GridFetchStatsContext query)
        {
            FiltroContenedor filtros = JsonConvert.DeserializeObject<FiltroContenedor>(query.GetParameter("FILTROS") ?? "");

            if (filtros == null || filtros.NU_PREPARACION == 0 || filtros.NU_CONTENEDOR == 0)
            {
                grid.Rows.Clear();
                return null;
            }

            if (filtros.NU_PREPARACION != null && filtros.NU_CONTENEDOR != null)
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                ProductosSalidaContenedorQuery dbQuery = null;

                dbQuery = new ProductosSalidaContenedorQuery((int)filtros.NU_PREPARACION, (int)filtros.NU_CONTENEDOR);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        #endregion
    }

    public class FiltroContenedor
    {
        public int? NU_PREPARACION;
        public int? NU_CONTENEDOR;
    }
}
