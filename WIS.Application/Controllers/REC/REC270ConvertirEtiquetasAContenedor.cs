using DocumentFormat.OpenXml.InkML;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC270ConvertirEtiquetasAContenedor : AppController
    {
        #region PROPS
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger;
        protected readonly string _entityToLock;

        protected List<string> Grid1Keys { get; }
        protected List<string> Grid2Keys { get; }
        protected List<string> Grid3Keys { get; }

        protected List<SortCommand> GridSorts = new List<SortCommand>
        {
            new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Ascending),
        };
        #endregion PROPS

        public REC270ConvertirEtiquetasAContenedor(IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IIdentityService identity, IFormValidationService formValidationService, ITrafficOfficerService concurrencyControl, IFilterInterpreter filterInterpreter)
        {
            _uowFactory = uowFactory;
            _gridService = gridService;
            _excelService = excelService;
            _identity = identity;
            _formValidationService = formValidationService;
            _concurrencyControl = concurrencyControl;
            this._filterInterpreter = filterInterpreter;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _entityToLock = ($"T_AGENDA").Trim();

            this.Grid1Keys = new List<string>
            {
                "NU_ETIQUETA_LOTE"
            };

            this.Grid2Keys = new List<string>
            {
                "NU_ETIQUETA_LOTE"
            };

            this.Grid3Keys = new List<string>
            {
                "NU_ETIQUETA_LOTE"
            };
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            switch (grid.Id)
            {
                case "REC200_grid_1": return this.InitializeGrid1(grid, query);
                case "REC200_grid_2": return this.InitializeGrid2(grid, query);
                case "REC200_grid_3": return this.InitializeGrid3(grid, query);

            }

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public virtual Grid InitializeGrid1(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }
        public virtual Grid InitializeGrid2(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }
        public virtual Grid InitializeGrid3(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }


        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {

            switch (grid.Id)
            {
                case "REC270_grid_1": return this.FetchRowsGrid1(grid, query);
                case "REC270_grid_2": return this.FetchRowsGrid2(grid, query);
                case "REC270_grid_3": return this.FetchRowsGrid3(grid, query);

            }

            return grid;
        }
        public virtual Grid FetchRowsGrid1(Grid grid, GridFetchContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroAgenda = -1;
                if (!string.IsNullOrEmpty(query.GetParameter("Agenda")))
                {
                    nroAgenda = int.Parse(query.GetParameter("Agenda"));
                }

                var dbQuery = new REC270Grid1Query(nroAgenda);

                uow.HandleQuery(dbQuery);

                SortCommand defaultSort = new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Descending);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.GridSorts, this.Grid1Keys);
            }

            return grid;
        }
        public virtual Grid FetchRowsGrid2(Grid grid, GridFetchContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroAgenda = -1;
                if (!string.IsNullOrEmpty(query.GetParameter("Agenda")))
                {
                    nroAgenda = int.Parse(query.GetParameter("Agenda"));
                }
                var dbQuery = new REC270Grid2Query(nroAgenda);

                uow.HandleQuery(dbQuery);

                SortCommand defaultSort = new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Descending);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.GridSorts, this.Grid2Keys);

            }

            return grid;
        }
        public virtual Grid FetchRowsGrid3(Grid grid, GridFetchContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroAgenda = -1;
                if (!string.IsNullOrEmpty(query.GetParameter("Agenda")))
                {
                    nroAgenda = int.Parse(query.GetParameter("Agenda"));
                }


                var dbQuery = new REC270Grid3Query(nroAgenda);

                uow.HandleQuery(dbQuery);

                SortCommand defaultSort = new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Descending);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.GridSorts, this.Grid3Keys);

            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "REC270_grid_1":
                    var dbQuery = new REC270Grid1Query();
                    uow.HandleQuery(dbQuery);

                    query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

                    return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.GridSorts);

                case "REC270_grid_2":
                    var dbQuery1 = new REC270Grid2Query();
                    uow.HandleQuery(dbQuery1);

                    query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

                    return this._excelService.GetExcel(query.FileName, dbQuery1, grid.Columns, query, this.GridSorts);

                case "REC270_grid_3":
                    var dbQuery2 = new REC270Grid3Query();
                    uow.HandleQuery(dbQuery2);

                    query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

                    return this._excelService.GetExcel(query.FileName, dbQuery2, grid.Columns, query, this.GridSorts);


            }

            return null;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            switch (grid.Id)
            {
                case "REC270_grid_1": return this.FetchStatsGrid1(grid, query);
                case "REC270_grid_2": return this.FetchStatsGrid2(grid, query);
                case "REC270_grid_3": return this.FetchStatsGrid3(grid, query);
            }

            return null;
        }
        public virtual GridStats FetchStatsGrid1(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nroAgenda = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("Agenda")))
            {
                nroAgenda = int.Parse(query.GetParameter("Agenda"));
            }

            var dbQuery = new REC270Grid1Query(nroAgenda);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsGrid2(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nroAgenda = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("Agenda")))
            {
                nroAgenda = int.Parse(query.GetParameter("Agenda"));
            }
            var dbQuery = new REC270Grid2Query(nroAgenda);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsGrid3(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nroAgenda = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("Agenda")))
            {
                nroAgenda = int.Parse(query.GetParameter("Agenda"));
            }

            var dbQuery = new REC270Grid3Query(nroAgenda);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        #endregion GRID

        #region FORM
        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC220FormValidationModule(uow), form, context);
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            FormField fieldNU_AGENDA = form.GetField("NU_AGENDA");
            if (int.TryParse(fieldNU_AGENDA.Value, out int nroAgenda))
            {

                using var uow = this._uowFactory.GetUnitOfWork();

                uow.CreateTransactionNumber($"Convertir etiquetas a contenedor Agenda: {nroAgenda}");
                uow.BeginTransaction();

                var transaccion = _concurrencyControl.CreateTransaccion();
                try
                {
                    var etiquetas = uow.CrossDockingRepository.GetListaDeEtiquetasAConvertir(nroAgenda);
                    if (etiquetas == null || etiquetas.Count() == 0)
                    {
                        query.AddErrorNotification("No hay etiquetas para transferir");
                        _concurrencyControl.DeleteTransaccion(transaccion);
                        return form;
                    }

                    var entityToLock = _entityToLock;

                    CheckIfLocked(nroAgenda.ToString(), entityToLock);

                    _concurrencyControl.AddLock(entityToLock, nroAgenda.ToString(), transaccion, true);

                    var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);
                    var trasferirEtiquetasAgenda = new TransferirEtiquetasAgenda(_logger, _identity);

                    trasferirEtiquetasAgenda.ProcesarTransferenciaEtiquetas(uow, etiquetas, agenda);
                    uow.Commit();

                    this._concurrencyControl.RemoveLockByIdLock(entityToLock, nroAgenda.ToString(), _identity.UserId);

                    if (query.Notifications.Count() == 0)
                        query.AddSuccessNotification("General_Db_Success_Update");
                }
                catch (EntityLockedException ex)
                {
                    uow.Rollback();
                    query.AddErrorNotification(ex.Message, ex.StrArguments.ToList());
                }
                catch (Exception ex)
                {
                    string message = $"**Exception Message Nro Agenda: {nroAgenda}** : ";
                    _logger.Error(string.Format("{0}{1}", message, ex.Message));

                    string innerException = $"**InnerException Nro Agenda: {nroAgenda}** : ";
                    innerException = string.Format("{0}{1}", innerException, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    query.AddErrorNotification("General_Sec0_Error_Operacion");
                }
                finally
                {
                    _concurrencyControl.DeleteTransaccion(transaccion);
                    _logger.Trace($"Post DeleteTransaccion");
                }
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "NU_AGENDA": return this.SearchAgenda(query.SearchValue, null);

                default:
                    return new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchAgenda(string searchValue, string userId)
        {
            int user = string.IsNullOrEmpty(userId) ? _identity.UserId : int.Parse(userId);

            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.CrossDockingRepository.GetAgendaCrossDockingREC270(searchValue)
                   .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                   .ToList();
        }

        #endregion FORM

        #region AUX FUNCTIONS

        protected virtual void CheckIfLocked(string agenda, string entityToLock)
        {
            if (this._concurrencyControl.IsLocked(entityToLock, agenda, true))
                throw new EntityLockedException("REC270_Sec0_Error_AgendaBloqueada", new string[] { agenda });
        }

        public virtual void Corregir_Cross_Dock(IUnitOfWork uow, string P_Pedido, string P_Cliente, long P_Carga, string P_Produto, decimal P_Faixa, int P_empresa, string P_Identif, string P_Espec_Identif, int P_Prep, int P_Agenda, decimal P_QT_pendiente, decimal P_QT_preparado)
        {
            List<LineaCrossDocking> Listdet = uow.CrossDockingRepository.GetDetalleCrossDock(P_Produto, P_Faixa, P_Agenda, P_Prep, P_Carga, P_Pedido, P_Cliente, P_Identif, P_Espec_Identif);
            foreach (var det in Listdet)
            {
                det.Cantidad = det.Cantidad - P_QT_preparado;
                uow.CrossDockingRepository.UpdateDetalleCrossDocking(det);
            }
        }

        #endregion AUX FUNCTIONS
    }
}
