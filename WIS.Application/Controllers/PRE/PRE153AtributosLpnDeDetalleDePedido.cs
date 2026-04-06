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
	public class PRE153AtributosLpnDeDetalleDePedido : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFilterInterpreter _filterInterpreter;

		protected List<string> GridKeys { get; }
		protected List<SortCommand> DefaultSort { get; }

		public PRE153AtributosLpnDeDetalleDePedido(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
		{
			this.GridKeys = new List<string>
			{
				"NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "ID_LPN_EXTERNO", "TP_LPN_TIPO", "NU_DET_PED_SAI_ATRIB"
			};

			this.DefaultSort = new List<SortCommand>
			{
				new SortCommand("NU_PEDIDO", SortDirection.Ascending)
			};

			this._identity = identity;
			this._uowFactory = uowFactory;
			this._gridService = gridService;
			this._excelService = excelService;
			_filterInterpreter = filterInterpreter;
		}

		public override Grid GridInitialize(Grid grid, GridInitializeContext context)
		{

			grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
				new GridButton("btnDetalles", "PRE153_grd1_btn_Detalles", "fas fa-list"),
				}));

			return this.GridFetchRows(grid, context.FetchContext);
		}

		public override Grid GridFetchRows(Grid grid, GridFetchContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			AtributosLpnDeDetalleDePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "codEmp").Value, out int cdEmpresa))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string idLpnExterno = context.Parameters.FirstOrDefault(s => s.Id == "idLpnEx").Value;
				string tipoLpn = context.Parameters.FirstOrDefault(s => s.Id == "tipoLpn").Value;
				string numeroLpn = context.Parameters.FirstOrDefault(s => s.Id == "numLpn").Value;
				string codProducto = context.Parameters.FirstOrDefault(s => s.Id == "codPro").Value;

				dbQuery = new AtributosLpnDeDetalleDePedidoQuery(numPedido, idLpnExterno, tipoLpn);
				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

				Empresa empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
				Producto producto = uow.ProductoRepository.GetProducto(cdEmpresa, codProducto);

				context.AddParameter("PRE153_NU_LPN", numeroLpn);
				context.AddParameter("PRE153_ID_LPN_EXTERNO", idLpnExterno);
				context.AddParameter("PRE153_TP_LPN_TIPO", tipoLpn);
				context.AddParameter("PRE153_NU_PEDIDO", numPedido);
				context.AddParameter("PRE153_CD_EMPRESA", cdEmpresa.ToString());
				context.AddParameter("PRE153_NM_EMPRESA", empresa.Nombre);
				context.AddParameter("PRE153_CD_PRODUTO", codProducto);
				context.AddParameter("PRE153_DS_PRODUTO", producto.Descripcion);
			}
			else
			{
				dbQuery = new AtributosLpnDeDetalleDePedidoQuery();

				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
			}

			this.VisibilidadBotones(grid);

			return grid;
		}

		public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			AtributosLpnDeDetalleDePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idLpnEx").Value))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string idLpnExterno = context.Parameters.FirstOrDefault(s => s.Id == "idLpnEx").Value;
				string tipoLpn = context.Parameters.FirstOrDefault(s => s.Id == "tipoLpn").Value;

				dbQuery = new AtributosLpnDeDetalleDePedidoQuery(numPedido, idLpnExterno, tipoLpn);
			}
			else
				dbQuery = new AtributosLpnDeDetalleDePedidoQuery();

			uow.HandleQuery(dbQuery);
			context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			AtributosLpnDeDetalleDePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idLpnEx").Value))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string idLpnExterno = context.Parameters.FirstOrDefault(s => s.Id == "idLpnEx").Value;
				string tipoLpn = context.Parameters.FirstOrDefault(s => s.Id == "tipoLpn").Value;

				dbQuery = new AtributosLpnDeDetalleDePedidoQuery(numPedido, idLpnExterno, tipoLpn);
			}
			else
				dbQuery = new AtributosLpnDeDetalleDePedidoQuery();

			uow.HandleQuery(dbQuery);
			dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

			return new GridStats
			{
				Count = dbQuery.GetCount()
			};
		}

		public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			try
			{
				switch (context.ButtonId)
				{

					case "btnDetalles":
						context.Redirect("/preparacion/PRE154", true, new List<ComponentParameter>()
							{
								new ComponentParameter(){ Id = "numDet", Value = context.Row.GetCell("NU_DET_PED_SAI_ATRIB").Value },
								new ComponentParameter(){ Id = "idLpnEx", Value = context.Row.GetCell("ID_LPN_EXTERNO").Value },
								new ComponentParameter(){ Id = "tipoLpn", Value = context.Row.GetCell("TP_LPN_TIPO").Value },
							});
						break;
				}
			}
			catch (ValidationFailedException ex)
			{
				if (!string.IsNullOrEmpty(ex.Message))
					context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return context;
		}

		public virtual void VisibilidadBotones(Grid grid)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			foreach (GridRow row in grid.Rows)
			{
				List<string> botonesDeshabiltiados = new List<string>();

				long nuDetalle = long.Parse(row.GetCell("NU_DET_PED_SAI_ATRIB").Value);

				if (!uow.PedidoRepository.AnyDetalleAtributoDeDetallePedido(nuDetalle))
					botonesDeshabiltiados.Add("btnDetalles");
				else
					row.DisabledButtons.Remove("btnDetalles");

				row.DisabledButtons = botonesDeshabiltiados;
			}
		}
	}
}
