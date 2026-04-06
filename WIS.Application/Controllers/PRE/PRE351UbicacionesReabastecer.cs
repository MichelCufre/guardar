using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE351UbicacionesReabastecer : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE351UbicacionesReabastecer(IUnitOfWorkFactory uowFactory, IGridService gridService, IFilterInterpreter filterInterpreter, IIdentityService identity)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA", "CD_PRODUTO", "CD_ENDERECO_PICKING", "CD_FAIXA", "CD_ENDERECO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._identity = identity;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PRE351StockPickingReabastQuery dbQuery;
            if (context.Parameters.Count > 3)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int cdEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "faixa")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal cdFaixa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!this.CheckParams(context.Parameters))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string cdProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string cdEnderecoPicking = context.Parameters.FirstOrDefault(s => s.Id == "ubicacionPicking").Value;

                dbQuery = new PRE351StockPickingReabastQuery(cdEmpresa, cdProducto, cdEnderecoPicking, cdFaixa);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("CD_ENDERECO_PICKING").Hidden = true;
                grid.GetColumn("CD_FAIXA").Hidden = true;

                return grid;
            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PRE351StockPickingReabastQuery dbQuery;
            if (query.Parameters.Count > 3)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int cdEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "faixa")?.Value, out int cdFaixa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!this.CheckParams(query.Parameters))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string cdProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string cdEnderecoPicking = query.Parameters.FirstOrDefault(s => s.Id == "ubicacionPicking").Value;

                dbQuery = new PRE351StockPickingReabastQuery(cdEmpresa, cdProducto, cdEnderecoPicking, cdFaixa);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                return null;
            }
        }

        public virtual bool CheckParams(List<ComponentParameter> parameters)
        {
            if (String.IsNullOrEmpty(parameters.FirstOrDefault(s => s.Id == "producto").Value))
                return false;

            if (String.IsNullOrEmpty(parameters.FirstOrDefault(s => s.Id == "ubicacionPicking").Value))
                return false;

            return true;

        }
    }
}
