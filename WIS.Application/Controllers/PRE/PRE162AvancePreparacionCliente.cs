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

namespace WIS.Application.Controllers.PRE
{
    public class PRE162AvancePreparacionCliente : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE162AvancePreparacionCliente(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA", "NU_PREPARACION", "CD_CLIENTE", "NU_PEDIDO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                new SortCommand("NU_PREPARACION", SortDirection.Ascending),
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("NU_PEDIDO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            return data;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PreparacionesPorEmpresaNumPrep dbQuery;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int cd_empresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion").Value, out int nu_preparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                Empresa empre = uow.EmpresaRepository.GetEmpresa(cd_empresa);

                dbQuery = new PreparacionesPorEmpresaNumPrep(cd_empresa, nu_preparacion);

                uow.HandleQuery(dbQuery);

                context.AddParameter("PRE162_CD_EMPRESA", empre.Id.ToString());
                context.AddParameter("PRE162_NM_EMPRESA", empre.Nombre);
                context.AddParameter("PRE162_DS_PREPARACION", dbQuery.Retorno_ds_preparacion(nu_preparacion)??"");
                context.AddParameter("PRE162_NU_PREPARACION", nu_preparacion.ToString());
            }
            else
            {
                dbQuery = new PreparacionesPorEmpresaNumPrep();
                uow.HandleQuery(dbQuery);
            }

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            this.CamposCalculadosVista(uow, grid);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PreparacionesPorEmpresaNumPrep dbQuery;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int cd_empresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion").Value, out int nu_preparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PreparacionesPorEmpresaNumPrep(cd_empresa, nu_preparacion);
            }
            else
            {
                dbQuery = new PreparacionesPorEmpresaNumPrep();
            }

            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PreparacionesPorEmpresaNumPrep();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);
            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        
        public virtual void CamposCalculadosVista(IUnitOfWork uow, Grid grid)
        {
            //TODO: Refactorizar, copiado del 2.0
            foreach (var row in grid.Rows)
            {

                var cantPreparaciones = uow.PreparacionRepository.GetCantidadesPreparacion(int.Parse(row.GetCell("NU_PREPARACION").Value), int.Parse(row.GetCell("CD_EMPRESA").Value), row.GetCell("CD_CLIENTE").Value, row.GetCell("NU_PEDIDO").Value);

                decimal QT_PRODUCTOS_TOTALES = cantPreparaciones.FirstOrDefault(x => x.Key == "producto_total").Value;
                decimal QT_PICKEOS_TOTALES = cantPreparaciones.FirstOrDefault(x => x.Key == "pickeo_total").Value;


                GridCell cAUXUNIDADES_PREPARADAS = row.GetCell("AUXUNIDADES_PREPARADAS");
                GridCell cAUXPICKEOS_PREPARADOS = row.GetCell("AUXPICKEOS_PREPARADOS");

                int.TryParse(row.GetCell("QT_PRODUCTOS").Value, out int QT_PRODUCTOS);
                int.TryParse(row.GetCell("QT_PICKEOS").Value, out int QT_PICKEOS);

                cAUXUNIDADES_PREPARADAS.Value = "" + (QT_PRODUCTOS_TOTALES - QT_PRODUCTOS).ToString();
                cAUXPICKEOS_PREPARADOS.Value = "" + (QT_PICKEOS_TOTALES - QT_PICKEOS).ToString();

                GridCell cAUXPORC_UNIDADES = row.GetCell("AUXPORC_UNIDADES");
                GridCell cAUXPORC_PICKEOS = row.GetCell("AUXPORC_PICKEOS");

                cAUXPORC_UNIDADES.Value = "" + decimal.Truncate(decimal.Parse(cAUXUNIDADES_PREPARADAS.Value, _identity.GetFormatProvider()) * 100 / QT_PRODUCTOS_TOTALES).ToString();
                cAUXPORC_PICKEOS.Value = "" + decimal.Truncate(decimal.Parse(cAUXPICKEOS_PREPARADOS.Value, _identity.GetFormatProvider()) * 100 / QT_PICKEOS_TOTALES).ToString();
            }
        }
    }
}
