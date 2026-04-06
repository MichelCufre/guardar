using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Items;
using WIS.Components.Common;

namespace WIS.Application.Controllers.PRE
{
	public class PRE156DetalleDeAtributosDeDetallePedido : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFilterInterpreter _filterInterpreter;

		protected List<string> GridKeys { get; }
		protected List<SortCommand> DefaultSort { get; }

		public PRE156DetalleDeAtributosDeDetallePedido(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
		{
			this.GridKeys = new List<string>
			{
				"NU_DET_PED_SAI_ATRIB", "ID_ATRIBUTO", "FL_CABEZAL"
			};

			this.DefaultSort = new List<SortCommand>
			{
				new SortCommand("ID_ATRIBUTO", SortDirection.Ascending)
			};

			this._identity = identity;
			this._uowFactory = uowFactory;
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

			DetalleDeAtributosDeDetallePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "numDet").Value, out long numeroDetalle))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string codProducto = context.Parameters.FirstOrDefault(s => s.Id == "codPro").Value;
				int cdEmpresa = int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "codEmp").Value);


				dbQuery = new DetalleDeAtributosDeDetallePedidoQuery(numeroDetalle);
				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

				Empresa empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
				Producto producto = uow.ProductoRepository.GetProducto(cdEmpresa, codProducto);

				context.AddParameter("PRE156_CD_EMPRESA", cdEmpresa.ToString());
				context.AddParameter("PRE156_NM_EMPRESA", empresa.Nombre);
				context.AddParameter("PRE156_NU_PEDIDO", numPedido);
				context.AddParameter("PRE156_CD_PRODUTO", codProducto);
				context.AddParameter("PRE156_DS_PRODUTO", producto.Descripcion);
				context.AddParameter("PRE156_NU_DET_PED_SAI_ATRIB", numeroDetalle.ToString());
			}
			else
			{
				dbQuery = new DetalleDeAtributosDeDetallePedidoQuery();

				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
			}

			return grid;
		}

		public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			DetalleDeAtributosDeDetallePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "numDet").Value, out long numeroDetalle))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				dbQuery = new DetalleDeAtributosDeDetallePedidoQuery(numeroDetalle);
			}
			else
				dbQuery = new DetalleDeAtributosDeDetallePedidoQuery();

			uow.HandleQuery(dbQuery);
			context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			DetalleDeAtributosDeDetallePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "numDet").Value, out long numeroDetalle))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				dbQuery = new DetalleDeAtributosDeDetallePedidoQuery(numeroDetalle);
			}
			else
				dbQuery = new DetalleDeAtributosDeDetallePedidoQuery();

			uow.HandleQuery(dbQuery);
			dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

			return new GridStats
			{
				Count = dbQuery.GetCount()
			};
		}
	}
}
