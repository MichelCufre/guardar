using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
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
using WIS.TrafficOfficer;


namespace WIS.Application.Controllers.REC
{
    public class REC500Factura : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ILogger<REC500Factura> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly AutomatismoMapper _automatismoMapper;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IAutomatismoFactory _automatismoFactory;

        protected List<string> GridKeys { get; }

        public REC500Factura(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            ILogger<REC500Factura> logger,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            IAutomatismoFactory automatismoFactory)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._logger = logger;
            this._automatismoMapper = new AutomatismoMapper(automatismoFactory);
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;

            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_FACTURA"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", this.GetBotonesArrayGrid()));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnLineas", "General_Sec0_btn_EditarDetalle", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaQuery(todos: true);

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda").Value, out int numeroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new FacturaQuery(nuAgenda: numeroAgenda);
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_FACTURA", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            #region >> btn

            foreach (var row in grid.Rows)
            {
                if (row.GetCell("CD_SITUACAO").Value == SituacionDb.Activo.ToString() && int.TryParse(row.GetCell("NU_AGENDA").Value, out int nuAgenda))
                {
                    var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);

                    if (!agenda.EnEstadoCerrada())
                        row.DisabledButtons.Add("btnDesvincularDeAgenda");
                }
                else
                {
                    row.DisabledButtons.Add("btnDesvincularDeAgenda");
                }

                if (row.GetCell("CD_SITUACAO").Value == SituacionDb.Inactivo.ToString() || !string.IsNullOrEmpty(row.GetCell("NU_AGENDA").Value))
                    row.DisabledButtons.Add("btnCancelarFactura");

            }
            #endregion

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaQuery(todos: true);

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda").Value, out int numeroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new FacturaQuery(nuAgenda: numeroAgenda);
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_FACTURA", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaQuery(todos: true);

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda").Value, out int numeroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new FacturaQuery(nuAgenda: numeroAgenda);
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            switch (context.ButtonId)
            {
                case "btnLineas":
                    if (this._concurrencyControl.IsLocked("T_RECEPCION_FACTURA", context.Parameters.FirstOrDefault(s => s.Id == "idFactura").Value))
                    {
                        context.Parameters.Add(new ComponentParameter("Lockeada", "true"));
                        throw new EntityLockedException("REC500_Frm1_Error_EdicionLockeada", new string[] { });
                    }
                    break;
                case "btnCancelarFactura":
                    this.CancelarFactura(context);
                    break;
                case "btnDesvincularDeAgenda":  
                    this.DesvincularDeAgenda(context);
                    break;
            }

            uow.Commit();

            return context;
        }

        #region Metodos Auxiliares

        public virtual List<IGridItem> GetBotonesArrayGrid()
        {
            List<IGridItem> listaBotones = new List<IGridItem>();

            listaBotones.Add(new GridButton("btnCancelarFactura", "REC500_grid1_btn_btnCancelarFactura", "fas fa-box"));
            listaBotones.Add(new GridButton("btnDesvincularDeAgenda", "REC500_grid1_btn_btnDesvincularDeAgenda", "fas fa-copy"));

            return listaBotones;
        }

        public virtual void CancelarFactura(GridButtonActionContext context)
        {
            Factura factura = new Factura();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (factura.CancelarFactura(uow, _identity.Application, _identity.UserId, _concurrencyControl, int.Parse(context.Row.GetCell("NU_RECEPCION_FACTURA").Value)))
            {
                context.AddSuccessNotification("REC500_Sec0_Succes_CancelarFactura", new List<string> { context.Row.GetCell("NU_RECEPCION_FACTURA").Value });
            }
            else
            {
                context.AddErrorNotification("REC500_Sec0_Error_FacturaNoCancelable");
            }
        }

        public virtual void DesvincularDeAgenda(GridButtonActionContext context)
        {
            Factura factura = new Factura();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (factura.DesvincularDeAgenda(uow, _identity.Application, _identity.UserId, _concurrencyControl, int.Parse(context.Row.GetCell("NU_RECEPCION_FACTURA").Value)))
            {
                context.AddSuccessNotification("REC500_Sec0_Succes_FacturaDesvinculada");
            }
            else
            {
                context.AddErrorNotification("REC500_Sec0_Error_NoEsPosibleDesvincular");
            }
        }

        #endregion
    }
}
