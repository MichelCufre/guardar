using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Domain.Automatismo.Logic
{
    public class AutomatismoEntradaRequestStrategy : IAutomatismoRequestStategy
    {
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;

        protected readonly IUnitOfWork _uow;
        protected readonly AutomatismoEjecucion _ejecucion;

        protected readonly List<EntradaStockAutomatismoRequest> _records;

        protected List<string> _gridKeys;
        protected List<string> _detailsGridKeys;
        protected List<SortCommand> _defaultSorting;
        protected List<SortCommand> _detailsDefaultSorting;

        public AutomatismoEntradaRequestStrategy(
            IGridService gridService,
            IIdentityService identity,
            IGridExcelService excelService,
            IUnitOfWork uow,
            AutomatismoEjecucion ejecucion)
        {
            _gridService = gridService;
            _identity = identity;
            _excelService = excelService;
            _uow = uow;
            _ejecucion = ejecucion;

            _gridKeys = new List<string>
            {
                "Ejecucion"
            };

            _detailsGridKeys = new List<string>
            {
                "Producto", "Identificador"
            };

            _defaultSorting = new List<SortCommand>
            {
                new SortCommand("Ejecucion", SortDirection.Descending)
            };

            _detailsDefaultSorting = new List<SortCommand>
            {
                new SortCommand("Producto", SortDirection.Descending),
                new SortCommand("Identificador", SortDirection.Descending)
            };

            _records = new List<EntradaStockAutomatismoRequest>
            {
               GetAutomatismoRequest<EntradaStockAutomatismoRequest>()
            };
        }

        public virtual byte[] CreateExcel(Grid grid, GridExportExcelContext context)
        {
            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
                return GridInMemoryUtils.CreateExcel(_excelService, _uow, grid, context, GetDetalles(), _detailsDefaultSorting, _identity.Application);

            else
                return GridInMemoryUtils.CreateExcel(_excelService, _uow, grid, context, _records, _defaultSorting, _identity.Application);
        }

        public virtual List<GridRow> GenerateRowsAndLoadGrid(Grid grid, GridFetchContext context)
        {
            var isEditEnabled = _uow.SecurityRepository.IsUserAllowed(_identity.UserId, "AUT101EditarEjecuciones_grid1_cell_Editar");
            var editableCells = (!isEditEnabled || _ejecucion.Estado == EstadoEjecucion.PROCESADO_OK) ? new List<string>() : null;

            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
            {
                return GridInMemoryUtils.LoadGrid(_gridService, _uow, grid, context, GetDetalles(), _detailsDefaultSorting, _detailsGridKeys, editableCells);
            }
            else
            {
                context.AddParameter("SHOW_DETAILS_GRID", "S");
                return GridInMemoryUtils.LoadGrid(_gridService, _uow, grid, context, _records, _defaultSorting, _gridKeys, editableCells);
            }
        }

        public virtual T GetAutomatismoRequest<T>()
        {
            try
            {
                string jsonRequest = _ejecucion.AutomatismoData.FirstOrDefault().RequestData;
                return JsonConvert.DeserializeObject<T>(jsonRequest);
            }
            catch (JsonException)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public virtual GridStats FetchStats(GridFetchStatsContext context)
        {
            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
                return GridInMemoryUtils.FetchStats(context, _uow, GetDetalles());
            else
                return GridInMemoryUtils.FetchStats(context, _uow, _records);
        }

        public virtual void UpdateRequest(GridRow row, GridFetchContext context)
        {
            var automatismoData = _ejecucion.AutomatismoData.FirstOrDefault();
            var automatismoRequest = GetAutomatismoRequest<EntradaStockAutomatismoRequest>();

            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
            {
                var producto = row.GetCell("Producto").Value;
                var identificador = row.GetCell("Identificador").Value;
                var detail = automatismoRequest.Detalles.FirstOrDefault(i => i.Producto == producto && i.Identificador == identificador);
                GridInMemoryUtils.GetDataFromRow(row, detail);
            }
            else
                GridInMemoryUtils.GetDataFromRow(row, automatismoRequest);

            automatismoData.RequestData = JsonConvert.SerializeObject(automatismoRequest);

            _uow.AutomatismoDataRepository.Update(automatismoData);
        }

        #region UTILS

        public virtual List<EntradaStockLineaAutomatismoRequest> GetDetalles()
        {
            var records = new List<EntradaStockLineaAutomatismoRequest>();

            _records.ForEach(i => records.AddRange(i.Detalles));

            return records;
        }

        #endregion
    }
}
