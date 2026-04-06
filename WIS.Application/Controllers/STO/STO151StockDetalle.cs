using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO151StockDetalle : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly List<string> GridKeysPicking;
        protected readonly List<string> GridKeysContenedor;
        protected readonly List<string> GridKeysPalletTransferencia;
        protected readonly List<string> GridKeysEtiquetaRecepcion;
        protected readonly List<string> GridKeysLpn;

        protected readonly List<SortCommand> SortsContenedor;
        protected readonly List<SortCommand> SortsPalletTransferencia;
        protected readonly List<SortCommand> SortsEtiquetaRecepcion;
        protected readonly List<SortCommand> SortsLpn;

        public STO151StockDetalle(
            ISessionAccessor session,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysPicking = new List<string>
            {
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "CD_EMPRESA",
                "CD_ENDERECO",
                "NU_PEDIDO",
                "CD_CLIENTE",
                "NU_SEQ_PREPARACION"
            };

            this.GridKeysContenedor = new List<string>
            {
                "NU_CONTENEDOR",
                "NU_PREPARACION",
                "NU_PEDIDO"
            };

            this.GridKeysPalletTransferencia = new List<string>
            {
                "CD_PRODUTO",
                "NU_ETIQUETA",
                "NU_SEC_ETIQUETA",
                "CD_EMPRESA"
            };

            this.GridKeysEtiquetaRecepcion = new List<string>
            {
                "NU_ETIQUETA_LOTE",
                "NU_AGENDA",
                "NU_EXTERNO_ETIQUETA",
                "TP_ETIQUETA",
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "CD_EMPRESA"
            };

            this.GridKeysLpn = new List<string>
            {
                "NU_LPN",
                "CD_EMPRESA",
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR"
            };

            this.SortsContenedor = new List<SortCommand>
            {
                new SortCommand("NU_CONTENEDOR", SortDirection.Ascending)
            };

            this.SortsPalletTransferencia = new List<SortCommand> {
                new SortCommand("DT_ADDROW", SortDirection.Descending),
                new SortCommand("NU_SEC_ETIQUETA", SortDirection.Descending)
            };

            this.SortsEtiquetaRecepcion = new List<SortCommand> {
                new SortCommand("NU_AGENDA", SortDirection.Ascending),
                new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Ascending)
            };

            this.SortsLpn = new List<SortCommand> {
                new SortCommand("NU_LPN", SortDirection.Ascending)
            };

            this._session = session;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override PageContext PageLoad(PageContext context)
        {
            if (!this._session.ContainsKey("STO151_CD_EMPRESA"))
                context.Redirect = "/stock/STO150";

            return context;
        }

        public override PageContext PageUnload(PageContext context)
        {
            this._session.SetValue("STO151_CD_AREA_ARMAZ", null);
            this._session.SetValue("STO151_CD_EMPRESA", null);
            this._session.SetValue("STO151_CD_ENDERECO", null);
            this._session.SetValue("STO151_CD_PRODUTO", null);
            this._session.SetValue("STO151_CD_FAIXA", null);
            this._session.SetValue("STO151_NU_IDENTIFICADOR", null);

            return context;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsRollbackEnabled = false;
            context.IsCommitEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            switch (grid.Id)
            {
                case "STO151_grid_1": return this.FetchGridPicking(grid, context);
                case "STO151_grid_2": return this.FetchGridContenedor(grid, context);
                case "STO151_grid_3": return this.FetchGridPalletTransferencia(grid, context);
                case "STO151_grid_4": return this.FetchGridEtiquetaRecepcion(grid, context);
                case "STO151_grid_5": return this.FetchGridLpns(grid, context);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "STO151_grid_1")
            {
                if (context.Parameters.Count == 0)
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "area")?.Value, out int codigoArea))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                //TODO: Pasar a parametros
                List<int> areasLimitadas = new List<int> { 20, 21, 60, 90 };

                if (areasLimitadas.Contains(codigoArea))
                    return null;

                StockPickingQuery dbQuery = this.GetQueryPicking(uow, context.Parameters);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };

            }
            else if (grid.Id == "STO151_grid_2")
            {
                StockContenedorProductoQuery dbQuery = this.GetQueryContenedor(uow, context.Parameters);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "STO151_grid_3")
            {

                StockPalletTransferenciaQuery dbQuery = this.GetQueryPalletTransferencia(uow, context.Parameters);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "STO151_grid_4")
            {
                StockEtiquetaRecepcionQuery dbQuery = this.GetQueryEtiquetaRecepcion(uow, context.Parameters);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                StockLpnQuery dbQuery = this.GetQueryLpn(uow, context.Parameters);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            switch (grid.Id)
            {
                case "STO151_grid_1": return this.ExportExcelPicking(grid, context);
                case "STO151_grid_2": return this.ExportExcelContenedor(grid, context);
                case "STO151_grid_3": return this.ExportExcelPalletTransferencia(grid, context);
                case "STO151_grid_4": return this.ExportExcelEtiquetaRecepcion(grid, context);
                case "STO151_grid_5": return this.ExportExcelLpn(grid, context);
            }

            return default;
        }

        #region Auxs

        public virtual Grid FetchGridPicking(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "area")?.Value, out int codigoArea))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            //TODO: Pasar a parametros
            List<int> areasLimitadas = new List<int> { 20, 21, 60, 90 };

            if (areasLimitadas.Contains(codigoArea))
                return grid;

            StockPickingQuery dbQuery = this.GetQueryPicking(uow, context.Parameters);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsContenedor, this.GridKeysPicking);

            return grid;
        }
        public virtual Grid FetchGridContenedor(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockContenedorProductoQuery dbQuery = this.GetQueryContenedor(uow, context.Parameters);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsContenedor, this.GridKeysContenedor);

            return grid;
        }
        public virtual Grid FetchGridPalletTransferencia(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockPalletTransferenciaQuery dbQuery = this.GetQueryPalletTransferencia(uow, context.Parameters);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsPalletTransferencia, this.GridKeysPalletTransferencia);

            return grid;
        }
        public virtual Grid FetchGridEtiquetaRecepcion(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockEtiquetaRecepcionQuery dbQuery = this.GetQueryEtiquetaRecepcion(uow, context.Parameters);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsEtiquetaRecepcion, this.GridKeysEtiquetaRecepcion);

            return grid;
        }
        public virtual Grid FetchGridLpns(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockLpnQuery dbQuery = this.GetQueryLpn(uow, context.Parameters);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsLpn, this.GridKeysLpn);

            return grid;
        }

        public virtual byte[] ExportExcelPicking(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "area")?.Value, out int codigoArea))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            //TODO: Pasar a parametros
            List<int> areasLimitadas = new List<int> { 20, 21, 60, 90 };

            if (areasLimitadas.Contains(codigoArea))
                throw new OperationNotAllowedException("El código de area no permite realizar la exportación");

            StockPickingQuery dbQuery = this.GetQueryPicking(uow, context.Parameters);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsContenedor);
        }
        public virtual byte[] ExportExcelContenedor(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockContenedorProductoQuery dbQuery = this.GetQueryContenedor(uow, context.Parameters);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsContenedor);
        }
        public virtual byte[] ExportExcelPalletTransferencia(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockPalletTransferenciaQuery dbQuery = this.GetQueryPalletTransferencia(uow, context.Parameters);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsPalletTransferencia);
        }
        public virtual byte[] ExportExcelEtiquetaRecepcion(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockEtiquetaRecepcionQuery dbQuery = this.GetQueryEtiquetaRecepcion(uow, context.Parameters);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsEtiquetaRecepcion);
        }
        public virtual byte[] ExportExcelLpn(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockLpnQuery dbQuery = this.GetQueryLpn(uow, context.Parameters);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsLpn);
        }

        public virtual StockPickingQuery GetQueryPicking(IUnitOfWork uow, List<ComponentParameter> parameters = null)
        {
            if (parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string ubicacion = parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

            string producto = parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

            if (!decimal.TryParse(parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string identificador = parameters.FirstOrDefault(s => s.Id == "identificador")?.Value;

            var dbQuery = new StockPickingQuery(empresa, ubicacion, producto, embalaje, identificador);

            uow.HandleQuery(dbQuery);

            return dbQuery;
        }
        public virtual StockContenedorProductoQuery GetQueryContenedor(IUnitOfWork uow, List<ComponentParameter> parameters = null)
        {
            if (parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string ubicacion = parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

            string producto = parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

            if (!decimal.TryParse(parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string identificador = parameters.FirstOrDefault(s => s.Id == "identificador")?.Value;

            var dbQuery = new StockContenedorProductoQuery(empresa, ubicacion, producto, embalaje, identificador);

            uow.HandleQuery(dbQuery);

            return dbQuery;
        }
        public virtual StockPalletTransferenciaQuery GetQueryPalletTransferencia(IUnitOfWork uow, List<ComponentParameter> parameters = null)
        {

            if (parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string ubicacion = parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

            string producto = parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

            if (!decimal.TryParse(parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string identificador = parameters.FirstOrDefault(s => s.Id == "identificador")?.Value;

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "area")?.Value, out int codigoArea))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            //TODO: Pasar a parametros
            List<int> areasLimitadas = new List<int> { 20, 21, 60, 90 };

            bool isAreaLimitada = areasLimitadas.Contains(codigoArea);

            var stockQuery = new StockUbicacionQuery(empresa, ubicacion, producto, embalaje, identificador);

            uow.HandleQuery(stockQuery);

            decimal qtEntrada = stockQuery.GetAmountTransitoEntrada();

            var dbQuery = new StockPalletTransferenciaQuery(empresa, ubicacion, producto, embalaje, identificador, qtEntrada, isAreaLimitada);

            uow.HandleQuery(dbQuery);

            return dbQuery;
        }
        public virtual StockEtiquetaRecepcionQuery GetQueryEtiquetaRecepcion(IUnitOfWork uow, List<ComponentParameter> parameters = null)
        {

            if (parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string ubicacion = parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

            string producto = parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

            if (!decimal.TryParse(parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string identificador = parameters.FirstOrDefault(s => s.Id == "identificador")?.Value;

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "area")?.Value, out int codigoArea))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var stockQuery = new StockUbicacionQuery(empresa, ubicacion, producto, embalaje, identificador);

            uow.HandleQuery(stockQuery);

            decimal qtEntrada = stockQuery.GetAmountTransitoEntrada();

            var dbQuery = new StockEtiquetaRecepcionQuery(empresa, ubicacion, producto, embalaje, identificador, qtEntrada);

            uow.HandleQuery(dbQuery);

            return dbQuery;
        }
        public virtual StockLpnQuery GetQueryLpn(IUnitOfWork uow, List<ComponentParameter> parameters = null)
        {
            if (parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string ubicacion = parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

            string producto = parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

            if (!decimal.TryParse(parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string identificador = parameters.FirstOrDefault(s => s.Id == "identificador")?.Value;

            var dbQuery = new StockLpnQuery(empresa, ubicacion, producto, embalaje, identificador);
            uow.HandleQuery(dbQuery);

            return dbQuery;
        }

        #endregion
    }
}
