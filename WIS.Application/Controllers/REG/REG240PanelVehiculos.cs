using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.Expedicion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
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

namespace WIS.Application.Controllers.REG
{
    public class REG240PanelVehiculos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG240PanelVehiculos> _logger;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ITrackingService _trackingService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly List<string> GridKeys;

        public REG240PanelVehiculos(IUnitOfWorkFactory uowFactory, IIdentityService identity, ILogger<REG240PanelVehiculos> logger, IGridService gridService, IGridExcelService excelService, ITrackingService trackingService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_VEICULO"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridService = gridService;
            this._excelService = excelService;
            this._trackingService = trackingService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsCommitEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsAddEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_LIST", new List<GridButton>
            {
                new GridButton("btnEditar", "REG240_grid_btn_Editar", "fas fa-edit")
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ENABLE", new List<GridButton> {
                new GridButton("btnHabilitar", "REG240_grid_btn_Habilitar", "fas fa-toggle-off"),
                new GridButton("btnDeshabilitar", "REG240_grid_btn_Deshabilitar", "fas fa-toggle-on")
            }));

            return base.GridInitialize(grid, context);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var query = new VehiculoQuery();

            uow.HandleQuery(query);

            var defaultSort = new SortCommand("CD_VEICULO", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                GridCell disponibilidadDesde = row.GetCell("HR_DISPONIBILIDAD_DESDE");
                GridCell disponibilidadHasta = row.GetCell("HR_DISPONIBILIDAD_HASTA");
                GridCell estadoCell = row.GetCell("ND_ESTADO");

                disponibilidadDesde.Value = TimeSpan.FromMilliseconds(long.Parse(disponibilidadDesde.Value)).ToString(@"hh\:mm", this._identity.GetFormatProvider());
                disponibilidadHasta.Value = TimeSpan.FromMilliseconds(long.Parse(disponibilidadHasta.Value)).ToString(@"hh\:mm", this._identity.GetFormatProvider());

                disponibilidadDesde.ForceSetOldValue(disponibilidadDesde.Value);
                disponibilidadHasta.ForceSetOldValue(disponibilidadHasta.Value);

                if (estadoCell.Value == EstadoVehiculoDb.EnViaje)
                {
                    row.DisabledButtons.Add("btnHabilitar");
                    row.DisabledButtons.Add("btnDeshabilitar");
                }
                else if (estadoCell.Value == EstadoVehiculoDb.NoDisponible)
                    row.DisabledButtons.Add("btnDeshabilitar");
                else if (estadoCell.Value == EstadoVehiculoDb.Disponible)
                    row.DisabledButtons.Add("btnHabilitar");
            }

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<int> listaVehiculosId = grid.GetDeletedRows().Select(d => int.Parse(d.GetCell("CD_VEICULO").Value)).ToList();

            if (uow.CamionRepository.AnyCamionConVehiculo(listaVehiculosId))
                throw new ValidationFailedException("REG240_grid1_error_VehiculosEnUso");

            List<Vehiculo> vehiculosEliminar = uow.VehiculoRepository.GetVehiculos(listaVehiculosId);

            foreach (var vehiculo in vehiculosEliminar)
            {
                if (!vehiculo.CanDelete())
                    throw new ValidationFailedException("REG240_grid1_error_VehiculoEnUso", new string[] { vehiculo.Id.ToString() });

                uow.VehiculoRepository.Delete(vehiculo);
            }

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                int vehiculoId = int.Parse(context.Row.GetCell("CD_VEICULO").Value);
                Vehiculo vehiculo = uow.VehiculoRepository.GetVehiculo(vehiculoId);

                if (context.ButtonId == "btnHabilitar")
                    vehiculo.Estado = EstadoVehiculoDb.Disponible;
                else if (context.ButtonId == "btnDeshabilitar")
                    vehiculo.Estado = EstadoVehiculoDb.NoDisponible;

                uow.VehiculoRepository.Update(vehiculo);
                uow.SaveChanges();

                _trackingService.SincronizarVehiculo(uow,vehiculo, true);
                uow.VehiculoRepository.Update(vehiculo);
                uow.TipoVehiculoRepository.Update(vehiculo.Caracteristicas);
                uow.SaveChanges();

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new VehiculoQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_VEICULO", SortDirection.Descending);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new VehiculoQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

    }
}
