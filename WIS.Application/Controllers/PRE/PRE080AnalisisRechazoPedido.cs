using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public class PRE080AnalisisRechazoPedido : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFilterInterpreter _filterInterpreter;

		protected List<string> GridKeys { get; }
		protected List<SortCommand> DefaultSort { get; }

		public PRE080AnalisisRechazoPedido(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
		{
			this.GridKeys = new List<string>
			{
				"NU_PEDIDO", "CD_CLIENTE", "NU_PREPARACION", "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR"
			};

			this.DefaultSort = new List<SortCommand>
			{
				new SortCommand("NU_PEDIDO", SortDirection.Descending),
				new SortCommand("NU_PREPARACION", SortDirection.Descending),
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

			string preparacion = context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value;

			AnalisisRechazoPedidoQuery dbQuery = null;

			if (!string.IsNullOrEmpty(preparacion))
			{
				dbQuery = new AnalisisRechazoPedidoQuery(int.Parse(preparacion));
				context.AddParameter("PRE080_NU_PREPARACION", preparacion);
			}
			else
				dbQuery = new AnalisisRechazoPedidoQuery();

			uow.HandleQuery(dbQuery);
			grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

			return grid;
		}
		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			string preparacion = context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value;

			AnalisisRechazoPedidoQuery dbQuery = null;

			if (!string.IsNullOrEmpty(preparacion))
				dbQuery = new AnalisisRechazoPedidoQuery(int.Parse(preparacion));
			else
				dbQuery = new AnalisisRechazoPedidoQuery();

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

			string preparacion = context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value;

			AnalisisRechazoPedidoQuery dbQuery = null;

			if (!string.IsNullOrEmpty(preparacion))
				dbQuery = new AnalisisRechazoPedidoQuery(int.Parse(preparacion));
			else
				dbQuery = new AnalisisRechazoPedidoQuery();

			uow.HandleQuery(dbQuery);

			context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
		}
	}
}
