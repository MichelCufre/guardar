using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Ptl;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
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
using WIS.Session;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PTL
{
    public class PTL010PickToLight : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISessionAccessor _session;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ISecurityService _security;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IAutomatismoPtlClientService _automatismoPtlClientService;
        protected readonly IBarcodeService _barcoService;
        protected readonly PtlLogic _logicPtl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public PTL010PickToLight(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ISessionAccessor session,
            ITrafficOfficerService concurrencyControl,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            IAutomatismoPtlClientService automatismoPtlClientService,
            IBarcodeService barcoService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION", "CD_CLIENTE", "CD_EMPRESA","NU_AUTOMATISMO"
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_PREPARACION", SortDirection.Descending),
                new SortCommand("CD_CLIENTE", SortDirection.Descending)
            };

            _uowFactory = uowFactory;
            _identity = identity;
            _session = session;
            _concurrencyControl = concurrencyControl;
            _gridValidationService = gridValidationService;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
            _security = security;
            _automatismoPtlClientService = automatismoPtlClientService;
            _logicPtl = new PtlLogic(barcoService,identity);
            _barcoService = barcoService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnNotificarPTL", "PTL010_grid1_btn_NotificarPTL", "far fa-share-square"),
                new GridButton("btnLiberarLineas", "PTL010_grid1_btn_LiberarLineasPTL", "fas fa-minus"),
                new GridButton("btnTomarLineas", "PTL010_grid1_btn_TomarLineasPTL", "fas fa-plus")
            }));


            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PickToLightQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            HabilitarBotones(uow, grid.Rows);

            return grid;
        }

        public virtual void HabilitarBotones(IUnitOfWork uow, List<GridRow> rows)
        {
            ValidationsResult result = null;
            bool ptlTieneReferenciaActiva = false;
            foreach (var row in rows)
            {
                string referencia = _logicPtl.CreatePtlReferencia(row.GetCell("NU_PREPARACION").Value.ToNumber<int>(), row.GetCell("CD_CLIENTE").Value, row.GetCell("CD_EMPRESA").Value.ToNumber<int>());

                (result, ptlTieneReferenciaActiva) = _automatismoPtlClientService.ValidatePtlReferencia(row.GetCell("CD_ZONA_UBICACION").Value, referencia);

                if (!ptlTieneReferenciaActiva || !_logicPtl.TieneUbicacionesAutomatismoPendLiberar(uow, row.GetCell("NU_PREPARACION").Value.ToNumber<int>(), row.GetCell("CD_EMPRESA").Value.ToNumber<int>(), row.GetCell("CD_CLIENTE").Value))
                    row.DisabledButtons.Add("btnLiberarLineas");

                if (!ptlTieneReferenciaActiva || !_logicPtl.TienePickingPendienteEnUbicacionesAutomatismo(uow, row.GetCell("NU_PREPARACION").Value.ToNumber<int>(), row.GetCell("CD_EMPRESA").Value.ToNumber<int>(), row.GetCell("CD_CLIENTE").Value))
                    row.DisabledButtons.Add("btnTomarLineas");
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PickToLightQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PickToLightQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            switch (data.ButtonId)
            {
                case "btnLiberarLineas": return LiberarLineasAgrupacion(data);
                case "btnTomarLineas": return TomarLineasAgrupacion(data);
            }

            return data;
        }

        public virtual GridButtonActionContext LiberarLineasAgrupacion(GridButtonActionContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("Boton LiberarLineasAgrupacion");

            var preparacion = data.Row.GetCell("NU_PREPARACION").Value.ToNumber<int>();
            var cliente = data.Row.GetCell("CD_CLIENTE").Value;
            var empresa = data.Row.GetCell("CD_EMPRESA").Value.ToNumber<int>();

            _logicPtl.LiberarLineasAgrupacion(uow, preparacion, empresa, cliente, EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE, EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO);

            return data;
        }

        public virtual GridButtonActionContext TomarLineasAgrupacion(GridButtonActionContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("Boton TomarLineasAgrupacion");

            var preparacion = data.Row.GetCell("NU_PREPARACION").Value.ToNumber<int>();
            var cliente = data.Row.GetCell("CD_CLIENTE").Value;
            var empresa = data.Row.GetCell("CD_EMPRESA").Value.ToNumber<int>();

            _logicPtl.TomarLineasAgrupacion(uow, preparacion, empresa, cliente, EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO, EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE);

            return data;
        }
    }
}
