using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.Expedicion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG250PanelTiposVehiculo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly List<string> GridKeys;

        public REG250PanelTiposVehiculo(IUnitOfWorkFactory uowFactory, IGridService gridService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_TIPO_VEICULO"
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
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
                new GridButton("btnEditar", "REG250_grid1_btn_Editar", "fas fa-edit")
            }));

            return base.GridInitialize(grid, context);
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<int> listaTiposId = grid.GetDeletedRows().Select(d => int.Parse(d.GetCell("CD_TIPO_VEICULO").Value)).ToList();

            if (uow.VehiculoRepository.AnyVehiculoConTipo(listaTiposId))
                throw new ValidationFailedException("REG250_grid1_error_TipoEnUso");

            List<VehiculoEspecificacion> especificacionesEliminar = uow.TipoVehiculoRepository.GetTipos(listaTiposId);

            foreach (var vehiculo in especificacionesEliminar)
            {
                uow.TipoVehiculoRepository.Delete(vehiculo);
            }

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            return grid;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var query = new TipoVehiculoQuery();

            uow.HandleQuery(query);

            var defaultSort = new SortCommand("CD_TIPO_VEICULO", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TipoVehiculoQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
