using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion.Interfaces;
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
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
	public class PRD113StockUbicacionesProduccion : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly ITrafficOfficerService _concurrencyControl;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFormValidationService _formValidationService;
		protected readonly IFilterInterpreter _filterInterpreter;
		protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
		protected readonly IGridValidationService _gridValidationService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

		protected List<string> GridKeys { get; set; }
		protected List<SortCommand> DefaultSort { get; }

		public PRD113StockUbicacionesProduccion(
			IIdentityService identity,
			ITrafficOfficerService concurrencyControl,
			IUnitOfWorkFactory uowFactory,
			IGridService gridService,
			IGridExcelService excelService,
			IFormValidationService formValidationService,
			IFilterInterpreter filterInterpreter,
			ILogicaProduccionFactory logicaProduccionFactory,
			IGridValidationService gridValidationService)
		{
			this._identity = identity;
			this._uowFactory = uowFactory;
			this._gridService = gridService;
			this._excelService = excelService;
			this._formValidationService = formValidationService;
			this._filterInterpreter = filterInterpreter;
			this._logicaProduccionFactory = logicaProduccionFactory;
			this._concurrencyControl = concurrencyControl;
			this._gridValidationService = gridValidationService;

			this.GridKeys = new List<string> { "CD_ENDERECO", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR" };
			this.DefaultSort = new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Ascending) };
		}

		public override Grid GridInitialize(Grid grid, GridInitializeContext context)
		{
			context.IsEditingEnabled = false;
			context.IsAddEnabled = false;
			context.IsCommitEnabled = false;
			context.IsRemoveEnabled = false;


			return GridFetchRows(grid, context.FetchContext);
		}

		public override Grid GridFetchRows(Grid grid, GridFetchContext context)
		{
			using var uow = _uowFactory.GetUnitOfWork();

			var nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nuIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

			var ubicacionProduccion = uow.EspacioProduccionRepository.GetEspacioProduccionByIngreso(nuIngresoProduccion);

            var dbQuery = new StockUbicacionProduccionQuery(ubicacionProduccion.IdUbicacionProduccion, empresa);
			uow.HandleQuery(dbQuery);

			grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

			return grid;
		}

		public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
		{
			using var uow = _uowFactory.GetUnitOfWork();

			var nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nuIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var ubicacionProduccion = uow.EspacioProduccionRepository.GetEspacioProduccionByIngreso(nuIngresoProduccion);

            var dbQuery = new StockUbicacionProduccionQuery(ubicacionProduccion.IdUbicacionProduccion, empresa);

            uow.HandleQuery(dbQuery);

			context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
		{
			using var uow = _uowFactory.GetUnitOfWork();

			var nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nuIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var ubicacionProduccion = uow.EspacioProduccionRepository.GetEspacioProduccionByIngreso(nuIngresoProduccion);

            var dbQuery = new StockUbicacionProduccionQuery(ubicacionProduccion.IdUbicacionProduccion, empresa);

            uow.HandleQuery(dbQuery);

			dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

			return new GridStats
			{
				Count = dbQuery.GetCount()
			};
		}

	}
}
