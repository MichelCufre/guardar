using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Interfaz;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.INT
{
    public class INT103ConsultaInterfazReferenciaProveedorDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        protected List<SortCommand> DefaultSort { get; }

        public INT103ConsultaInterfazReferenciaProveedorDetalle(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_INTERFAZ_EJECUCION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_INTERFAZ_EJECUCION",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "interfaz")?.Value, out int nuInterfaz))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                EstanRefRecepcionDetQuery dbQuery = new EstanRefRecepcionDetQuery(nuInterfaz);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }

            return grid;

        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "interfaz")?.Value, out int nuInterfaz))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                EstanRefRecepcionDetQuery dbQuery = new EstanRefRecepcionDetQuery(nuInterfaz);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };

            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "interfaz")?.Value, out int nuInterfaz))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                EstanRefRecepcionDetQuery dbQuery = new EstanRefRecepcionDetQuery(nuInterfaz);

                uow.HandleQuery(dbQuery);

                query.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }
        }
    }
}
