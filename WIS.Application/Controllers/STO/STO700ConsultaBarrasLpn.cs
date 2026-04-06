using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Impresiones;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentFormat.OpenXml.InkML;

namespace WIS.Application.Controllers.STO
{
	public class STO700ConsultaBarrasLpn : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IIdentityService _identity;
		protected readonly IFilterInterpreter _filterInterpreter;

		protected List<string> GridKeys { get; }
		protected List<SortCommand> Sorts { get; }

		public STO700ConsultaBarrasLpn(IGridExcelService excelService, IUnitOfWorkFactory uowFactory, IGridService gridService, IIdentityService identity, IFilterInterpreter filterInterpreter)
		{
			this.GridKeys = new List<string>
			{
				"NU_LPN", "ID_LPN_BARRAS"
			};

			this.Sorts = new List<SortCommand> {
				new SortCommand("ID_LPN_BARRAS", SortDirection.Descending),
			};

			this._uowFactory = uowFactory;
			this._gridService = gridService;
			_excelService = excelService;
			_identity = identity;
			_filterInterpreter = filterInterpreter;
		}

		public override Form FormInitialize(Form form, FormInitializeContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			form.GetField("numeroLpn").ReadOnly = true;
			form.GetField("tipoLpn").ReadOnly = true;

			if (!string.IsNullOrEmpty(context.GetParameter("numeroLpn")))
			{
				var numeroLpn = context.GetParameter("numeroLpn");
				var lpn = uow.ManejoLpnRepository.GetLpn(long.Parse(numeroLpn));

				form.GetField("numeroLpn").Value = numeroLpn;
				form.GetField("tipoLpn").Value = lpn.Tipo;
			}

			return form;
		}

		public override Grid GridInitialize(Grid grid, GridInitializeContext context)
		{
			return this.GridFetchRows(grid, context.FetchContext);
		}

		public override Grid GridFetchRows(Grid grid, GridFetchContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			long numeroLpn = -1; 

			if (!string.IsNullOrEmpty(context.GetParameter("numeroLpn")))
				numeroLpn = long.Parse(context.GetParameter("numeroLpn"));

			if (numeroLpn != -1)
			{
				ConsultaCodigosBarraLpnQuery dbQuery = new ConsultaCodigosBarraLpnQuery(numeroLpn);
				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);
			}

			return grid;
		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
		{
			using var uow = this._uowFactory.GetUnitOfWork();
			long numeroLpn = -1;
			ConsultaCodigosBarraLpnQuery dbQuery = new ConsultaCodigosBarraLpnQuery();

			if (!string.IsNullOrEmpty(query.GetParameter("numeroLpn")))
				numeroLpn = long.Parse(query.GetParameter("numeroLpn"));

			if (numeroLpn != -1)
				dbQuery = new ConsultaCodigosBarraLpnQuery(numeroLpn);

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
			long numeroLpn = -1;
			ConsultaCodigosBarraLpnQuery dbQuery = new ConsultaCodigosBarraLpnQuery();

			if (!string.IsNullOrEmpty(context.GetParameter("numeroLpn")))
				numeroLpn = long.Parse(context.GetParameter("numeroLpn"));

			if (numeroLpn != -1)
				dbQuery = new ConsultaCodigosBarraLpnQuery(numeroLpn);

			uow.HandleQuery(dbQuery);

			context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
		}
	}
}
