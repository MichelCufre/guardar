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
    public class EXP400ContenedoresProblemaDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP400ContenedoresProblemaDetalle(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION", "NU_SEQ_PREPARACION", "CD_ENDERECO", "CD_CLIENTE", "NU_PEDIDO", "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Descending)
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

            DetallesContenedoresCompartidos dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int camion))
                throw new MissingParameterException("EXP400_Sec0_error_necesitaParam");

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "contenedor")?.Value, out int contenedor))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value, out int preparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new DetallesContenedoresCompartidos(contenedor, preparacion);
            }
            else
                dbQuery = new DetallesContenedoresCompartidos();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.Rows.ForEach(w =>
            {
                if (w.GetCell("COMPARTIDO").Value == "S" && (w.GetCell("CD_CAMION_1").Value != camion.ToString() || w.GetCell("CD_CAMION_2").Value != camion.ToString()))
                    w.CssClass = "red";
                else if (string.IsNullOrEmpty(w.GetCell("CD_CAMION_1").Value))
                    w.CssClass = "yellow";

            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            DetallesContenedoresCompartidos dbQuery;

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int camion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "contenedor")?.Value, out int contenedor))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value, out int preparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new DetallesContenedoresCompartidos(contenedor, preparacion);
            }
            else
                dbQuery = new DetallesContenedoresCompartidos();

            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            DetallesContenedoresCompartidos dbQuery;

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int camion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "contenedor")?.Value, out int contenedor))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value, out int preparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new DetallesContenedoresCompartidos(contenedor, preparacion);
            }
            else
                dbQuery = new DetallesContenedoresCompartidos();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
