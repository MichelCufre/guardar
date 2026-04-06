using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
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

namespace WIS.Application.Controllers.PRD
{
	public class PRD113PanelFabricacionProductosFinalesEsperados : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFormValidationService _formValidationService;
		protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRD113PanelFabricacionProductosFinalesEsperados(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFormValidationService formValidationService, IFilterInterpreter filterInterpreter)
		{
			this._identity = identity;
			this._uowFactory = uowFactory;
			this._gridService = gridService;
			this._excelService = excelService;
			this._formValidationService = formValidationService;
			this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "NU_PRDC_DET_TEORICO","CD_PRODUTO","CD_EMPRESA","CD_FAIXA","NU_IDENTIFICADOR",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending),
            };
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
			var nroIngresoProduccion = context.Parameters.Find(x => x.Id == "nuIngresoProduccion")?.Value;
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

			if (string.IsNullOrEmpty(nroIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
				throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

			using var uow = _uowFactory.GetUnitOfWork();

			var dbQuery = new ProductosFinalesEsperadosProduccionQuery(nroIngresoProduccion, empresa);

			uow.HandleQuery(dbQuery);

			grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

			return grid;
		}

		public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
		{
			var nroIngresoProduccion = context.Parameters.Find(x => x.Id == "nuIngresoProduccion")?.Value;
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nroIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new ProductosFinalesEsperadosProduccionQuery(nroIngresoProduccion, empresa);
            uow.HandleQuery(dbQuery);

			context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
		{
			var nroIngresoProduccion = context.Parameters.Find(x => x.Id == "nuIngresoProduccion")?.Value;
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nroIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new ProductosFinalesEsperadosProduccionQuery(nroIngresoProduccion, empresa);

            uow.HandleQuery(dbQuery);

			dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

			return new GridStats
			{
				Count = dbQuery.GetCount()
			};
		}

		public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
		{
			return context;
		}

	}
}
