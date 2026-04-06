using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.Domain.DataModel;
using WIS.GridComponent.Build.Configuration;
using WIS.Sorting;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Exceptions;
using WIS.Filtering;
using System.Globalization;

namespace WIS.Application.Controllers.REC
{
    public class REC180LogEtiquetas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC180LogEtiquetas(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_ETIQUETA_LOTE", "TP_ETIQUETA", "NU_EXTERNO_ETIQUETA", "NU_AGENDA", "NU_LOG_ETIQUETA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AGENDA", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
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

            EtiquetaLogQuery dbQuery;

            if (query.Parameters.Count > 3)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "etiqueta")?.Value, out int etiquetaLote))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "identificador").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string identificador = query.Parameters.FirstOrDefault(s => s.Id == "identificador").Value;
                string numeroExterno = query.Parameters.FirstOrDefault(s => s.Id == "numeroExterno").Value;

                dbQuery = new EtiquetaLogQuery(etiquetaLote, embalaje, idProducto, identificador);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_FAIXA").Hidden = true;
                grid.GetColumn("NU_IDENTIFICADOR").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("NU_ETIQUETA_LOTE").Hidden = true;
                grid.GetColumn("DS_PRODUTO").Hidden = true;
                grid.GetColumn("TP_ETIQUETA").Hidden = true;

                string ds_Prod = string.Empty;
                string tp_Etiqueta = string.Empty;

                if (grid.Rows.Count > 0)
                {
                    ds_Prod = " - " + grid.Rows.FirstOrDefault().GetCell("DS_PRODUTO").Value;
                    tp_Etiqueta = " - " + grid.Rows.FirstOrDefault().GetCell("TP_ETIQUETA").Value;
                }

                query.AddParameter("REC180_NU_IDENTIFICADOR", identificador);
                query.AddParameter("REC180_CD_FAIXA", embalaje.ToString());
                query.AddParameter("REC180_NU_ETIQUETA_LOTE", etiquetaLote.ToString() + tp_Etiqueta);
                query.AddParameter("REC180_CD_PRODUCTO", idProducto + ds_Prod);
                query.AddParameter("REC180_NU_EXTERNO_ETIQUETA", numeroExterno);
            }
            else
            {

                dbQuery = new EtiquetaLogQuery();
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EtiquetaLogQuery dbQuery;

            if (query.Parameters.Count > 3)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "etiqueta")?.Value, out int etiquetaLote))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "identificador").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string identificador = query.Parameters.FirstOrDefault(s => s.Id == "identificador").Value;

                dbQuery = new EtiquetaLogQuery(etiquetaLote, embalaje, idProducto, identificador);
            }
            else
            {

                dbQuery = new EtiquetaLogQuery();
            }

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EtiquetaLogQuery dbQuery;

            if (query.Parameters.Count > 3)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "etiqueta")?.Value, out int etiquetaLote))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "identificador").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string identificador = query.Parameters.FirstOrDefault(s => s.Id == "identificador").Value;

                dbQuery = new EtiquetaLogQuery(etiquetaLote, embalaje, idProducto, identificador);
            }
            else
            {

                dbQuery = new EtiquetaLogQuery();
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