using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP041PedidosExpedidos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP041PedidosExpedidos(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "CD_CAMION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO",SortDirection.Ascending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosExpedidosQuery dbQuery;

            if (query.Parameters.Count > 2)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idPedido = query.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;

                dbQuery = new PedidosExpedidosQuery(idEmpresa, idPedido, idProducto);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("NU_PEDIDO").Hidden = true;

                string nombreEmp = uow.EmpresaRepository.GetNombre(idEmpresa);
                string descProducto = string.IsNullOrEmpty(uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto)) ? string.Empty : uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto);

                query.AddParameter("EXP041_CD_EMPRESA", idEmpresa.ToString());
                query.AddParameter("EXP041_NM_EMPRESA", nombreEmp);
                query.AddParameter("EXP041_CD_PRODUTO", idProducto.ToString());
                query.AddParameter("EXP041_DS_PRODUTO", descProducto);
                query.AddParameter("EXP041_NU_PEDIDO", idPedido);

                    

            }
            else if (query.Parameters.Count > 0)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PedidosExpedidosQuery(idCamion);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_CAMION").Hidden = true;
                string desCamio = string.IsNullOrEmpty(uow.CamionRepository.GetCamion(idCamion).Descripcion) ? string.Empty : uow.CamionRepository.GetCamion(idCamion).Descripcion;
                query.AddParameter("EXP041_CD_CAMION", idCamion.ToString());
                query.AddParameter("EXP041_DS_CAMION", desCamio);

            }
            else
            {

                dbQuery = new PedidosExpedidosQuery();

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosExpedidosQuery dbQuery;

            if (query.Parameters.Count > 2)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idPedido = query.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;

                dbQuery = new PedidosExpedidosQuery(idEmpresa, idPedido, idProducto);
                uow.HandleQuery(dbQuery);

            }
            else if (query.Parameters.Count > 0)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PedidosExpedidosQuery(idCamion);
                uow.HandleQuery(dbQuery);

            }
            else
            {
                dbQuery = new PedidosExpedidosQuery();
                uow.HandleQuery(dbQuery);
            }

            query.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosExpedidosQuery dbQuery;

            if (query.Parameters.Count > 2)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idPedido = query.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;

                dbQuery = new PedidosExpedidosQuery(idEmpresa, idPedido, idProducto);
                uow.HandleQuery(dbQuery);

            }
            else if (query.Parameters.Count > 0)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PedidosExpedidosQuery(idCamion);
                uow.HandleQuery(dbQuery);

            }
            else
            {
                dbQuery = new PedidosExpedidosQuery();
                uow.HandleQuery(dbQuery);
            }
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
