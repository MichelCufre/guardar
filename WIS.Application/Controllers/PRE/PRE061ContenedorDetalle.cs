using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
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
    public class PRE061ContenedorDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE061ContenedorDetalle(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA", "NU_PREPARACION", "NU_PEDIDO", "CD_CLIENTE", "CD_ENDERECO", "NU_SEQ_PREPARACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Descending),
                new SortCommand("CD_FAIXA", SortDirection.Descending),
                new SortCommand("NU_PREPARACION", SortDirection.Descending),
                new SortCommand("NU_PEDIDO", SortDirection.Descending),
                new SortCommand("CD_CLIENTE", SortDirection.Descending),
                new SortCommand("CD_ENDERECO", SortDirection.Descending),
                new SortCommand("NU_SEQ_PREPARACION", SortDirection.Descending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_ENDERECO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_ENDERECO", "ubicacion") });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count() > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(d => d.Id == "preparacion").Value, out int codigoPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(d => d.Id == "contenedor").Value, out int codigoContenedor))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new DetallesDeContenedoresQuery(codigoPreparacion, codigoContenedor);

                Contenedor contenedor = uow.ContenedorRepository.GetContenedor(codigoPreparacion, codigoContenedor);
                string situacionContenedor = contenedor.Estado.ToString(); //+ " - " + uow.SituacionRepository.GetSituacionDescripcion(contenedor.CodigoSituacion);                
                string subClase = uow.ClaseRepository.GetDescripcionSuperClase(contenedor.CodigoSubClase);

                context.AddParameter("PRE061_PREPARACION", codigoPreparacion.ToString());
                context.AddParameter("PRE061_CONTENEDOR", codigoContenedor.ToString());
                context.AddParameter("PRE061_UBICACION", contenedor.Ubicacion);
                context.AddParameter("PRE061_SITUACION", situacionContenedor);
                context.AddParameter("PRE061_SUB_CLASE", subClase);
                context.AddParameter("PRE061_CD_CAMION", contenedor.CodigoCamion.ToString());
                context.AddParameter("PRE061_ID_EXTERNO_CONTENEDOR", contenedor.IdExterno.ToString());

                if (!string.IsNullOrEmpty(contenedor.CodigoCamion.ToString()) && string.IsNullOrEmpty(contenedor.CodigoPuerta.ToString()))
                {
                    var camion = uow.CamionRepository.GetCamion(contenedor.CodigoCamion ?? 0);
                    context.AddParameter("PRE061_PUERTA", camion.Puerta.ToString());
                }
                else
                    context.AddParameter("PRE061_PUERTA", contenedor.CodigoPuerta.ToString());


                uow.HandleQuery(dbQuery);

                grid.GetColumn("NU_PREPARACION").Hidden = true;
                grid.GetColumn("NU_CONTENEDOR").Hidden = true;
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count() > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(d => d.Id == "preparacion").Value, out int codigoPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(d => d.Id == "contenedor").Value, out int codigoContenedor))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new DetallesDeContenedoresQuery(codigoPreparacion, codigoContenedor);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (query.Parameters.Count() > 0)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(d => d.Id == "preparacion").Value, out int codigoPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.FirstOrDefault(d => d.Id == "contenedor").Value, out int codigoContenedor))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new DetallesDeContenedoresQuery(codigoPreparacion, codigoContenedor);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };

            }
            else
            {
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");
            }
        }
    }
}
