using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Exceptions;
using WIS.Filtering;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP045EgresoDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        public EXP045EgresoDetalle(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "EXP045_grid_2")
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
                {
                    new GridButton("btnViewError", "EXP045_grid1_btn_Error", "fas fa-exclamation-triangle")
                }));

                context.AddLink("CD_ENDERECO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_ENDERECO", "ubicacion") });
            }

            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int idCamion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (uow.CamionRepository.AnyCamion(idCamion))
            {
                var camionDescripcion = uow.CamionRepository.GetCamionDescripcion(idCamion);

                context.AddParameter("EXP045_RESPETA_ORDEN", "N");                
                context.AddParameter("EXP045_CD_CAMION", idCamion.ToString());
                context.AddParameter("EXP045_PLACA", camionDescripcion.Matricula);
                context.AddParameter("EXP045_SITUACION", $"{(short)camionDescripcion.Estado} - {camionDescripcion.DescSituacion}");
                context.AddParameter("EXP045_DT_INGRESO", camionDescripcion.FechaCreacion?.ToString(CDateFormats.DATE_ONLY));

                context.AddParameter("EXP045_RUTA", camionDescripcion.Ruta == null ? string.Empty : $"{camionDescripcion.Ruta} - {camionDescripcion.DescRuta}");
                context.AddParameter("EXP045_PUERTA", camionDescripcion.Puerta == null ? string.Empty : $"{camionDescripcion.Puerta} - {camionDescripcion.DescPuerta}");
                context.AddParameter("EXP045_EMPRESA", camionDescripcion.Empresa == null ? string.Empty : $"{camionDescripcion.Empresa} - {camionDescripcion.DescEmpresa}");
            }
            else
                throw new MissingParameterException("EXP045_Sec0_error_NoExisteCamion", new string[] { idCamion.ToString() });

            switch (grid.Id)
            {
                case "EXP045_grid_1": return this.GridFetchRowsContenedoresEmbarcados(grid, uow, context);

                case "EXP045_grid_2": return this.GridFetchRowsContenedoresSinEmbarcar(grid, uow, context);

                case "EXP045_grid_3": return this.GridFetchRowsProductosSinPreparacion(grid, uow, context);

                case "EXP045_grid_4": return this.GridFetchRowsProductosDetalleCamion(grid, uow, context);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "EXP045_grid_1")
            {
                var defaultSort = new List<SortCommand>()
                {
                    new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                    new SortCommand("NU_CONTENEDOR", SortDirection.Ascending),
                };

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ContenedoresEmbarcadosQuery(idCamion);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
            else if (grid.Id == "EXP045_grid_2")
            {
                var defaultSort = new List<SortCommand>()
                {
                    new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                    new SortCommand("NU_CONTENEDOR", SortDirection.Ascending),
                };

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ContenedoresSinEmbarcarQuery(idCamion);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
            else if (grid.Id == "EXP045_grid_3")
            {
                var defaultSort = new List<SortCommand>()
                {
                    new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                    new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                    new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending)
                };

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ProductosSinPreparacionQuery(idCamion);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
            else
            {
                var defaultSort = new List<SortCommand>()
                {
                    new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                    new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                    new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending)
                };

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ProductosDetalleCamionQuery(idCamion);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "EXP045_grid_1")
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ContenedoresEmbarcadosQuery(idCamion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };

            }
            else if (grid.Id == "EXP045_grid_2")
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ContenedoresSinEmbarcarQuery(idCamion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "EXP045_grid_3")
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ProductosSinPreparacionQuery(idCamion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else //(grid.Id == "EXP045_grid_4")
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var dbQuery = new ProductosDetalleCamionQuery(idCamion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnViewError")
            {
                context.Redirect("/expedicion/EXP400", new List<ComponentParameter>
                {
                    new ComponentParameter("contenedor",context.Row.GetCell("NU_CONTENEDOR").Value),
                    new ComponentParameter("preparacion",context.Row.GetCell("NU_PREPARACION").Value),
                    new ComponentParameter("camion",context.Row.GetCell("CD_CAMION").Value)
                });
            }
            return context;
        }

        #region Metodos Auxiliares

        public virtual Grid GridFetchRowsContenedoresEmbarcados(Grid grid, IUnitOfWork uow, GridFetchContext context)
        {
            var gridKeys = new List<string>
            {
                "NU_CONTENEDOR"
            };

            var defaultSort = new List<SortCommand>()
            {
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("NU_CONTENEDOR", SortDirection.Ascending),
            };

            ContenedoresEmbarcadosQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ContenedoresEmbarcadosQuery(idCamion);
            }
            else
            {
                dbQuery = new ContenedoresEmbarcadosQuery();
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, gridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsContenedoresSinEmbarcar(Grid grid, IUnitOfWork uow, GridFetchContext context)
        {
            var gridKeys = new List<string>
            {
                "NU_CONTENEDOR", "CD_CLIENTE", "CD_EMPRESA"
            };

            var defaultSort = new List<SortCommand>()
            {
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("NU_CONTENEDOR", SortDirection.Ascending),
            };

            ContenedoresSinEmbarcarQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ContenedoresSinEmbarcarQuery(idCamion);
            }
            else
            {
                dbQuery = new ContenedoresSinEmbarcarQuery();
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, gridKeys);


            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int camion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var contenedoresConProblemas = uow.CamionRepository.GetContenedoresConProblemas(camion);

            foreach (var row in grid.Rows)
            {
                var btnErrorCell = row.GetCell("btnViewError");
                row.DisabledButtons = new List<string> { "btnViewError" };

                var contenedor = int.Parse(row.GetCell("NU_CONTENEDOR").Value);
                var preparacion = int.Parse(row.GetCell("NU_PREPARACION").Value);

                if (contenedoresConProblemas.Any(w => w[0] == preparacion && w[1] == contenedor))
                {
                    row.DisabledButtons.Remove("btnViewError");
                    row.CssClass = row.CssClass + " error";
                }
            }

            return grid;
        }

        public virtual Grid GridFetchRowsProductosSinPreparacion(Grid grid, IUnitOfWork uow, GridFetchContext context)
        {
            var gridKeys = new List<string>
            {
                "CD_CAMION", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA", "CD_EMPRESA", "CD_CLIENTE", "NU_PREPARACION"
            };

            var defaultSort = new List<SortCommand>()
            {
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending)
            };

            ProductosSinPreparacionQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProductosSinPreparacionQuery(idCamion);
            }
            else
                dbQuery = new ProductosSinPreparacionQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, gridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsProductosDetalleCamion(Grid grid, IUnitOfWork uow, GridFetchContext context)
        {
            var gridKeys = new List<string>
            {
                 "DT_PICKEO", "CD_CAMION", "NU_CARGA", "NU_PEDIDO", "CD_CLIENTE", "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "NU_PREPARACION", "CD_FAIXA"
            };

            var defaultSort = new List<SortCommand>()
            {
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending)
            };

            ProductosDetalleCamionQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProductosDetalleCamionQuery(idCamion);
            }
            else
                dbQuery = new ProductosDetalleCamionQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, gridKeys);

            return grid;
        }

        #endregion
    }
}
