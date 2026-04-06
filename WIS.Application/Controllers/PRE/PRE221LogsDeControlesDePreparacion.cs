using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
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

namespace WIS.Application.Controllers.PRE
{
	public class PRE221LogsDeControlesDePreparacion : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFilterInterpreter _filterInterpreter;

		protected List<string> GridKeys { get; }

		public PRE221LogsDeControlesDePreparacion(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
		{
			this.GridKeys = new List<string>
			{
				"NU_LOG_CONT_PICKEO"
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
			SortCommand defaultSort = new SortCommand("NU_LOG_CONT_PICKEO", SortDirection.Descending);

			using var uow = this._uowFactory.GetUnitOfWork();

			LogsDeControlesDePreparacionQuery dbQuery = null;

			if (context.Parameters.Count == 0)
				dbQuery = new LogsDeControlesDePreparacionQuery();
			else if (context.Parameters.Any(s => s.Id == "FROM_PRE220"))
			{
				if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int emp))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				dbQuery = new LogsDeControlesDePreparacionQuery(prep, emp);
			}

			uow.HandleQuery(dbQuery);

			grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

			return grid;
		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			var dbQuery = new LogsDeControlesDePreparacionQuery();

			uow.HandleQuery(dbQuery);
			dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

			return new GridStats
			{
				Count = dbQuery.GetCount()
			};
		}

		public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
		{
			SortCommand defaultSort = new SortCommand("NU_LOG_CONT_PICKEO", SortDirection.Descending);

			using var uow = this._uowFactory.GetUnitOfWork();

			LogsDeControlesDePreparacionQuery dbQuery = null;

			if (context.Parameters.Any(s => s.Id == "FROM_PRE220"))
			{
				if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int emp))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				dbQuery = new LogsDeControlesDePreparacionQuery(prep, emp);
			}
			else
				dbQuery = new LogsDeControlesDePreparacionQuery();

			uow.HandleQuery(dbQuery);

			context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
		}
	}
}
