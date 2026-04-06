using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Build.Configuration;
using WIS.Sorting;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.Domain.General;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Exceptions;
using WIS.Domain.Recepcion;
using WIS.Domain.DataModel;
using WIS.Filtering;

namespace WIS.Application.Controllers.REC
{
    public class REC011ReferenciaRecepcionDetalle : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REC011ReferenciaRecepcionDetalle(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_REFERENCIA_DET"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            
            query.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            DetalleDeReferenciaDeRecepcion dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "referencia").Value, out int idRecepcionReferencia))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new DetalleDeReferenciaDeRecepcion(idRecepcionReferencia);

                ReferenciaRecepcion refRecepcion = uow.ReferenciaRecepcionRepository.GetReferenciaConDetalle(idRecepcionReferencia);

                string codigoCliente = refRecepcion.CodigoCliente;
                int codigoEmpresa = refRecepcion.IdEmpresa;

                Empresa empresa = uow.EmpresaRepository.GetEmpresa(codigoEmpresa);
                Agente agente = uow.AgenteRepository.GetAgente(codigoEmpresa, codigoCliente);
                string descEstadoReferencia = uow.ReferenciaRecepcionRepository.GetDescripcionTipoReferencia(refRecepcion.TipoReferencia);

                query.AddParameter("REC011_CD_AGENTE", agente.Codigo);
                query.AddParameter("REC011_DS_AGENTE", agente.Descripcion);
                query.AddParameter("REC011_DS_TIPO_AGENTE", agente.Tipo.ToString());
                query.AddParameter("REC011_CD_EMPRESA", codigoEmpresa.ToString());
                query.AddParameter("REC011_NM_EMPRESA", empresa.Nombre);
                query.AddParameter("REC011_TP_REFERENCIA", refRecepcion.TipoReferencia);
                query.AddParameter("REC011_DS_TIPO_REFERENCIA", descEstadoReferencia);
                query.AddParameter("REC011_NU_REFERENCIA", refRecepcion.Numero);
                query.AddParameter("REC011_DS_ESTADO_REFERENCIA", refRecepcion.Estado.ToString());

                grid.GetColumn("NU_RECEPCION_REFERENCIA").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
                grid.GetColumn("CD_AGENTE").Hidden = true;
                grid.GetColumn("DS_TIPO_AGENTE").Hidden = true;
                grid.GetColumn("TP_REFERENCIA").Hidden = true;
                grid.GetColumn("DS_REFERENCIA").Hidden = true;
                grid.GetColumn("NU_REFERENCIA").Hidden = true;
            }
            else
            {
                dbQuery = new DetalleDeReferenciaDeRecepcion();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_REFERENCIA_DET", SortDirection.Ascending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            DetalleDeReferenciaDeRecepcion dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "referencia").Value, out int idReferencia))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new DetalleDeReferenciaDeRecepcion(idReferencia);

            }
            else
            {
                dbQuery = new DetalleDeReferenciaDeRecepcion();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_REFERENCIA_DET", SortDirection.Ascending);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            DetalleDeReferenciaDeRecepcion dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "referencia").Value, out int idRecepcionReferencia))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new DetalleDeReferenciaDeRecepcion(idRecepcionReferencia);

            }
            else
            {
                dbQuery = new DetalleDeReferenciaDeRecepcion();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}