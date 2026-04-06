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
	public class PRE155AtributosDeDetalleDePedido : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IGridService _gridService;
		protected readonly IGridExcelService _excelService;
		protected readonly IFilterInterpreter _filterInterpreter;

		protected List<string> GridKeys { get; }
		protected List<SortCommand> DefaultSort { get; }

		public PRE155AtributosDeDetalleDePedido(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
		{
			this.GridKeys = new List<string>
			{
				"NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "NU_DET_PED_SAI_ATRIB"
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
				new GridButton("btnDetalles", "PRE155_grd1_btn_Detalles", "fas fa-list"),
				}));

			return this.GridFetchRows(grid, context.FetchContext);
		}

		public override Grid GridFetchRows(Grid grid, GridFetchContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			AtributosDeDetalleDePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "codEmp").Value, out int cdEmpresa))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string codCliente = context.Parameters.FirstOrDefault(s => s.Id == "codCli").Value;
				string codProducto = context.Parameters.FirstOrDefault(s => s.Id == "codPro").Value;
				decimal codFaixa = decimal.Parse(context.Parameters.FirstOrDefault(s => s.Id == "codFai").Value, this._identity.GetFormatProvider());
				string numIdentificador = context.Parameters.FirstOrDefault(s => s.Id == "numIde").Value;
				string idEspecificaIdentificador = context.Parameters.FirstOrDefault(s => s.Id == "idEspIde").Value;


				dbQuery = new AtributosDeDetalleDePedidoQuery(numPedido, codCliente, cdEmpresa, codProducto, codFaixa, numIdentificador, idEspecificaIdentificador);
				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

				Empresa empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
				Agente agente = uow.AgenteRepository.GetAgente(cdEmpresa, codCliente);
				Producto producto = uow.ProductoRepository.GetProducto(cdEmpresa, codProducto);

				context.AddParameter("PRE155_CD_CLIENTE", agente.Codigo);
				context.AddParameter("PRE155_DS_CLIENTE", agente.Descripcion);
				context.AddParameter("PRE155_DS_TIPO_CLIENTE", agente.Tipo.ToString());
				context.AddParameter("PRE155_CD_EMPRESA", cdEmpresa.ToString());
				context.AddParameter("PRE155_NM_EMPRESA", empresa.Nombre);
				context.AddParameter("PRE155_NU_PEDIDO", numPedido);
				context.AddParameter("PRE155_CD_PRODUTO", codProducto);
				context.AddParameter("PRE155_DS_PRODUTO", producto.Descripcion);
				context.AddParameter("PRE155_CD_FAIXA", codFaixa.ToString());
				context.AddParameter("PRE155_NU_IDENTIFICADOR", numIdentificador);
				context.AddParameter("PRE155_ID_ESPECIFICA_INDENTIFICADOR", idEspecificaIdentificador);
			}
			else
			{
				dbQuery = new AtributosDeDetalleDePedidoQuery();

				uow.HandleQuery(dbQuery);
				grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
			}

			this.VisibilidadBotones(grid);

			return grid;
		}

		public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			AtributosDeDetalleDePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "codEmp").Value, out int cdEmpresa))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string codCliente = context.Parameters.FirstOrDefault(s => s.Id == "codCli").Value;
				string codProducto = context.Parameters.FirstOrDefault(s => s.Id == "codPro").Value;
				decimal codFaixa = decimal.Parse(context.Parameters.FirstOrDefault(s => s.Id == "codFai").Value, this._identity.GetFormatProvider());
				string numIdentificador = context.Parameters.FirstOrDefault(s => s.Id == "numIde").Value;
				string idEspecificaIdentificador = context.Parameters.FirstOrDefault(s => s.Id == "idEspIde").Value;

				dbQuery = new AtributosDeDetalleDePedidoQuery(numPedido, codCliente, cdEmpresa, codProducto, codFaixa, numIdentificador, idEspecificaIdentificador);
			}
			else
				dbQuery = new AtributosDeDetalleDePedidoQuery();

			uow.HandleQuery(dbQuery);
			context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
			return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
		}

		public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
		{
			using var uow = this._uowFactory.GetUnitOfWork();

			AtributosDeDetalleDePedidoQuery dbQuery;

			if (context.Parameters.Count > 0)
			{
				if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "codEmp").Value, out int cdEmpresa))
					throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

				string numPedido = context.Parameters.FirstOrDefault(s => s.Id == "numPed").Value;
				string codCliente = context.Parameters.FirstOrDefault(s => s.Id == "codCli").Value;
				string codProducto = context.Parameters.FirstOrDefault(s => s.Id == "codPro").Value;
				decimal codFaixa = decimal.Parse(context.Parameters.FirstOrDefault(s => s.Id == "codFai").Value, this._identity.GetFormatProvider());
				string numIdentificador = context.Parameters.FirstOrDefault(s => s.Id == "numIde").Value;
				string idEspecificaIdentificador = context.Parameters.FirstOrDefault(s => s.Id == "idEspIde").Value;

				dbQuery = new AtributosDeDetalleDePedidoQuery(numPedido, codCliente, cdEmpresa, codProducto, codFaixa, numIdentificador, idEspecificaIdentificador);
			}
			else
				dbQuery = new AtributosDeDetalleDePedidoQuery();

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
						context.Redirect("/preparacion/PRE156", true, new List<ComponentParameter>()
						{
							new ComponentParameter(){ Id = "numPed", Value = context.Row.GetCell("NU_PEDIDO").Value },
							new ComponentParameter(){ Id = "numDet", Value = context.Row.GetCell("NU_DET_PED_SAI_ATRIB").Value },
							new ComponentParameter(){ Id = "codEmp", Value = context.Row.GetCell("CD_EMPRESA").Value },
							new ComponentParameter(){ Id = "codPro", Value = context.Row.GetCell("CD_PRODUTO").Value },
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

				if (!uow.PedidoRepository.AnyDetalleAtributo(nuDetalle))
					botonesDeshabiltiados.Add("btnDetalles");
				else
					row.DisabledButtons.Remove("btnDetalles");

				row.DisabledButtons = botonesDeshabiltiados;
			}
		}
	}
}
