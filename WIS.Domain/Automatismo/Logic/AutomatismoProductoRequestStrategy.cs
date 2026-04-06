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
    public class AutomatismoProductoRequestStrategy : IAutomatismoRequestStategy
    {
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;

        protected readonly IUnitOfWork _uow;
        protected readonly AutomatismoEjecucion _ejecucion;

        protected readonly List<ProductosAutomatismoRequest> _records;

        protected List<string> _gridKeys;
        protected List<SortCommand> _defaultSorting;
        protected List<string> _productoKeys;
        protected List<SortCommand> _productoDefaultSorting;

        public AutomatismoProductoRequestStrategy(
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

            _productoKeys = new List<string>
            {
                "Codigo"
            };

            _defaultSorting = new List<SortCommand>
            {
                new SortCommand("Empresa", SortDirection.Descending)
            };

            _productoDefaultSorting = new List<SortCommand>
            {
                new SortCommand("Codigo", SortDirection.Ascending),
            };

            _records = new List<ProductosAutomatismoRequest>
            {
                GetAutomatismoRequest<ProductosAutomatismoRequest>()
            };
        }

        public virtual List<Producto> GetProductos()
        {
            var request = _records.FirstOrDefault();
            var productos = request.Productos
                .Select(p => new Producto
                {
                    CodigoEmpresa = request.Empresa,
                    Codigo = p.Codigo,
                });

            return _uow.ProductoRepository.GetProductos(productos).ToList();
        }

        public virtual byte[] CreateExcel(Grid grid, GridExportExcelContext context)
        {
            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
                return GridInMemoryUtils.CreateExcel(_excelService, _uow, grid, context, GetProductos(), _productoDefaultSorting, _identity.Application);

            return GridInMemoryUtils.CreateExcel(_excelService, _uow, grid, context, _records, _defaultSorting, _identity.Application);
        }

        public virtual List<GridRow> GenerateRowsAndLoadGrid(Grid grid, GridFetchContext context)
        {
            var isEditEnabled = _uow.SecurityRepository.IsUserAllowed(_identity.UserId, "AUT101EditarEjecuciones_grid1_cell_Editar");
            var editableCells = (!isEditEnabled || _ejecucion.Estado == EstadoEjecucion.PROCESADO_OK) ? new List<string>() : null;

            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
            {
                return GridInMemoryUtils.LoadGrid(_gridService, _uow, grid, context, GetProductos(), _productoDefaultSorting, _productoKeys, editableCells);
            }
            else
            {
                context.AddParameter("SHOW_DETAILS_GRID", "S");
                return GridInMemoryUtils.LoadGrid(_gridService, _uow, grid, context, _records, _defaultSorting, _gridKeys, editableCells);
            }
        }

        public virtual GridStats FetchStats(GridFetchStatsContext context)
        {
            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
                return GridInMemoryUtils.FetchStats(context, _uow, GetProductos());

            return GridInMemoryUtils.FetchStats(context, _uow, _records);
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
            var automatismoRequest = GetAutomatismoRequest<ProductosAutomatismoRequest>();

            if (context.Parameters.Any(i => i.Id == "SHOW_DETAILS_GRID"))
            {
                var codigo = row.GetCell("Codigo").Value;
                var producto = automatismoRequest.Productos.FirstOrDefault(p => p.Codigo == codigo);
                GridInMemoryUtils.GetDataFromRow(row, producto);
            }
            else
                GridInMemoryUtils.GetDataFromRow(row, automatismoRequest);

            automatismoData.RequestData = JsonConvert.SerializeObject(automatismoRequest);

            _uow.AutomatismoDataRepository.Update(automatismoData);
        }
    }
}
