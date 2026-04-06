using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
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
    public class PRE130Preparaciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE130Preparaciones(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION", "NU_SEQ_PREPARACION", "CD_ENDERECO", "CD_CLIENTE", "NU_PEDIDO", "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION",SortDirection.Ascending),
                new SortCommand("NU_SEQ_PREPARACION",SortDirection.Ascending),
                new SortCommand("CD_ENDERECO",SortDirection.Ascending),
                new SortCommand("CD_CLIENTE",SortDirection.Ascending),
                new SortCommand("NU_PEDIDO",SortDirection.Ascending),
                new SortCommand("CD_PRODUTO",SortDirection.Ascending),
                new SortCommand("CD_EMPRESA",SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR",SortDirection.Ascending),
                new SortCommand("CD_FAIXA",SortDirection.Ascending)


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

            ConsultaDePreparaciones dbQuery = null;

            if (context.Parameters.Count == 0)
                dbQuery = new ConsultaDePreparaciones();
            else if (context.Parameters.Any(s => s.Id == "FROM_PRE052") || context.Parameters.Any(s => s.Id == "FROM_PRE220"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int emp))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaDePreparaciones(prep, emp);
            }
            else
            {
                DetallePreparacion det = new DetallePreparacion();

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int emp))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "carga")?.Value, out long carga))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "faixa")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal faixa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "identificador")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                det.NumeroPreparacion = prep;
                det.Empresa = emp;
                det.Carga = carga;
                det.Pedido = context.Parameters.FirstOrDefault(s => s.Id == "pedido").Value; ;
                det.Cliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value; ;
                det.Producto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value; ;
                det.Lote = context.Parameters.FirstOrDefault(s => s.Id == "identificador").Value; ;
                det.Faixa = faixa;

                dbQuery = new ConsultaDePreparaciones(det);
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaDePreparaciones dbQuery = null;

            if (context.Parameters.Any(s => s.Id == "FROM_PRE052"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int emp))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaDePreparaciones(prep, emp);
            }
            else
                dbQuery = new ConsultaDePreparaciones();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaDePreparaciones dbQuery = null;

            if (context.Parameters.Any(s => s.Id == "FROM_PRE052"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int emp))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaDePreparaciones(prep, emp);
            }
            else
                dbQuery = new ConsultaDePreparaciones();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
