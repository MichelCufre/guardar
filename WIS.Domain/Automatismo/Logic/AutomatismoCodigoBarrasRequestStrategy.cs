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
    public class AutomatismoCodigoBarrasRequestStrategy : IAutomatismoRequestStategy
    {
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;

        protected readonly IUnitOfWork _uow;
        protected readonly AutomatismoEjecucion _ejecucion;

        protected readonly List<CodigosBarrasAutomatismoRequest> _records;

        protected List<string> _gridKeys;
        protected List<SortCommand> _defaultSorting;
        protected List<string> _codigoBarrasKeys;
        protected List<SortCommand> _codigoBarrasSorting;

        public AutomatismoCodigoBarrasRequestStrategy(
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
                "Empresa"
            };

            _codigoBarrasKeys = new List<string>
            {
                "Codigo"
            };

            _defaultSorting = new List<SortCommand>
            {
                new SortCommand("Empresa", SortDirection.Descending)
            };

            _codigoBarrasSorting = new List<SortCommand>
            {
                new SortCommand("Codigo", SortDirection.Descending)
            };

            _records = new List<CodigosBarrasAutomatismoRequest>{
                GetAutomatismoRequest<CodigosBarrasAutomatismoRequest>()
            };
        }

        public virtual List<CodigoBarras> GetCodigosBarras()
        {
            var request = _records.FirstOrDefault();
            var codigos = request.CodigosDeBarras
                .Select(c => new CodigoBarras
                {
                    Codigo = c.Codigo,
                    Empresa = request.Empresa,
                });

            return _uow.CodigoBarrasRepository.GetCodigosBarras(codigos).ToList();
        }

        public virtual byte[] CreateExcel(Grid grid, GridExportExcelContext context)
        {
            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
                return GridInMemoryUtils.CreateExcel(_excelService, _uow, grid, context, GetCodigosBarras(), _codigoBarrasSorting, _identity.Application);

            return GridInMemoryUtils.CreateExcel(_excelService, _uow, grid, context, _records, _defaultSorting, _identity.Application);
        }

        public virtual GridStats FetchStats(GridFetchStatsContext context)
        {
            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
                return GridInMemoryUtils.FetchStats(context, _uow, GetCodigosBarras());

            return GridInMemoryUtils.FetchStats(context, _uow, _records);
        }

        public virtual List<GridRow> GenerateRowsAndLoadGrid(Grid grid, GridFetchContext context)
        {
            var isEditEnabled = _uow.SecurityRepository.IsUserAllowed(_identity.UserId, "AUT101EditarEjecuciones_grid1_cell_Editar");
            var editableCells = (!isEditEnabled || _ejecucion.Estado == EstadoEjecucion.PROCESADO_OK) ? new List<string>() : null;

            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
            {
                return GridInMemoryUtils.LoadGrid(_gridService, _uow, grid, context, GetCodigosBarras(), _codigoBarrasSorting, _codigoBarrasKeys, editableCells);
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

        public virtual void UpdateRequest(GridRow row, GridFetchContext context)
        {
            var automatismoData = _ejecucion.AutomatismoData.FirstOrDefault();
            var automatismoRequest = GetAutomatismoRequest<CodigosBarrasAutomatismoRequest>();

            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
            {
                var codigo = row.GetCell("Codigo").Value;
                var codigoBarras = automatismoRequest.CodigosDeBarras.FirstOrDefault(c => c.Codigo == codigo);
                GridInMemoryUtils.GetDataFromRow(row, codigoBarras);
            }
            else
                GridInMemoryUtils.GetDataFromRow(row, automatismoRequest);

            automatismoData.RequestData = JsonConvert.SerializeObject(automatismoRequest);

            _uow.AutomatismoDataRepository.Update(automatismoData);
        }
    }
}
