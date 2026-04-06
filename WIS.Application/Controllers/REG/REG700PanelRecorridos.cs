using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.Recorridos;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REG
{
    public class REG700PanelRecorridos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG700PanelRecorridos> _logger;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG700PanelRecorridos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            ILogger<REG700PanelRecorridos> logger,
            ITrafficOfficerService concurrencyControl)
        {
            GridKeys =
            [
                "NU_RECORRIDO"
            ];

            DefaultSort =
            [
                new SortCommand("NU_RECORRIDO", SortDirection.Descending)
            ];

            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _security = security;
            _filterInterpreter = filterInterpreter;
            _logger = logger;
            _concurrencyControl = concurrencyControl;
        }

        public override PageContext PageLoad(PageContext context)
        {
            if (_security.IsUserAllowed(SecurityResources.REG700_Sec0_btn_CrearRecorrido))
                context.AddParameter("REG700_CREAR_RECORRIDO_HABILITADO", "S");

            return context;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsCommitEnabled = true;
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;


            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", GetBotonesArrayGrid()));

            var columnButton = new GridColumnButton("BTN_ARRAY_0",
            [
                new GridButton("btnViewDetails", "REG700_Sec0_btn_ViewDetails", "fas fa-list"),
            ]);

            if (_security.IsUserAllowed(SecurityResources.REG700_Sec0_btn_Editar))
                columnButton.Buttons.Add(new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"));

            columnButton.Buttons.Add(new GridButton("btnUbicacionesSinAsociar", "REG700_Sec0_btn_UbicacionesSinAsociar", "fas fa-exclamation-triangle"));

            grid.AddOrUpdateColumn(columnButton);

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new RecorridosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            DeshabilitarBotones(grid, uow);

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("Borrado recorridos");
            uow.BeginTransaction();

            var transactionTO = this._concurrencyControl.CreateTransaccion();

            try
            {
                if (grid.Rows.Any())
                {
                    var idsRecorridos = new List<int>();

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            idsRecorridos.Add(int.Parse(row.GetCell("NU_RECORRIDO").Value));
                        }
                    }

                    ConsultarBloqueosRecorridos(uow, idsRecorridos, transactionTO);
                    AgregarBloqueosRecorridos(uow, idsRecorridos, transactionTO);

                    DeleteRecorridos(uow, idsRecorridos);
                }
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (EntityLockedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG700GridCommit");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new RecorridosQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{_identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new RecorridosQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var numeroRecorrido = context.Row.GetCell("NU_RECORRIDO").Value;
            var nombreRecorrido = context.Row.GetCell("NM_RECORRIDO").Value;
            var isRecorridoDefault = context.Row.GetCell("FL_DEFAULT").Value;
            var predio = context.Row.GetCell("NU_PREDIO").Value;

            context.AddParameter("REG700_NU_RECORRIDO", numeroRecorrido);
            context.AddParameter("REG700_NM_RECORRIDO", nombreRecorrido);
            context.AddParameter("REG700_FL_DEFAULT", isRecorridoDefault);
            context.AddParameter("REG700_NU_PREDIO", predio);

            if (context.ButtonId == "btnViewDetails")
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var esDefault = context.Row.GetCell("FL_DEFAULT").Value == "S";

                if (Recorrido.PermiteModificarse(predio, esDefault, uow))
                    context.AddParameter("REG700_DETALLE_IMPORT_HABILITADO", "S");
                else
                    context.AddParameter("REG700_DETALLE_IMPORT_HABILITADO", "N");
            }

            return context;
        }

        #region MetodosGenerales

        public virtual List<IGridItem> GetBotonesArrayGrid()
        {
            var listaBotones = new List<IGridItem>
            {
                new GridButton("btnAsociarAplicaciones", "REG700_grid1_btn_btnAsociarAplicaciones", "fas fa-list"),
                new GridButton("btnAsociarAplicacionesUsuario", "REG700_grid1_btn_btnAsociarAplicacionesUsuario", "fas fa-list"),
            };

            listaBotones.AddRange(GetCustomGridActions());

            return listaBotones;
        }

        public virtual List<GridButton> GetCustomGridActions()
        {
            return new List<GridButton>
            {
            };
        }

        public virtual void DeshabilitarBotones(Grid grid, IUnitOfWork uow)
        {
            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.WREG700_grid1_btn_btnAsociarAplicaciones,
                SecurityResources.WREG700_grid1_btn_btnAsociarAplicacionesUsuario
            });

            foreach (var row in grid.Rows)
            {
                if (!result[SecurityResources.WREG700_grid1_btn_btnAsociarAplicaciones])
                    row.DisabledButtons.Add("btnAsociarAplicaciones");

                if (!result[SecurityResources.WREG700_grid1_btn_btnAsociarAplicacionesUsuario])
                    row.DisabledButtons.Add("btnAsociarAplicacionesUsuario");

                var ubicacionesFaltantes = row.GetCell("FL_UBIC_FALTANTE").Value == "S";
                if (!ubicacionesFaltantes) row.DisabledButtons.Add("btnUbicacionesSinAsociar");
            }
        }

        public virtual void ConsultarBloqueosRecorridos(IUnitOfWork uow, List<int> idsRecorridos, TrafficOfficerTransaction transactionTO)
        {
            var detallesRecorridosAConsultar = idsRecorridos
                .Select(r => $"{r}#")
                .ToList();

            var listLock = this._concurrencyControl.GetLockListWithKeyPrefixes("T_RECORRIDO", detallesRecorridosAConsultar, transactionTO);

            if (listLock.Any())
            {
                throw new EntityLockedException("REG700_msg_Error_RecorridosBloqueados");
            }
        }

        public virtual void AgregarBloqueosRecorridos(IUnitOfWork uow, List<int> idsRecorridos, TrafficOfficerTransaction transactionTO)
        {
            var detallesRecorridos = idsRecorridos
                .Select(r => r.ToString())
                .ToList();

            _concurrencyControl.AddLockList("T_RECORRIDO", detallesRecorridos, transactionTO);
        }

        public virtual void DeleteRecorridos(IUnitOfWork uow, List<int> idsRecorridos)
        {
            foreach (var id in idsRecorridos)
            {
                var recorrido = uow.RecorridoRepository.GetRecorridoById(id);

                if (recorrido == null)
                    throw new ValidationFailedException("REG700_Sec0_Error_RecorridoNoExiste", new string[] { id.ToString() });

                if (recorrido.EsDefault)
                    throw new ValidationFailedException("REG700_Sec0_Error_RecorridoPredeterminadoNoPuedeEliminarse");

                if (uow.RecorridoRepository.AnyPredeterminadoRecorridoAplicacion(id))
                    throw new ValidationFailedException("REG700_Sec0_Error_RecorridoAplicacionPredeterminadoNoPuedeEliminarse");

                if (uow.RecorridoRepository.AnyPredeterminadoRecorridoAplicacionUsuario(id))
                    throw new ValidationFailedException("REG700_Sec0_Error_RecorridoAplicacionUsuarioPredeterminadoNoPuedeEliminarse");

                var nuTransaccion = uow.GetTransactionNumber();

                recorrido.Transaccion = nuTransaccion;
                recorrido.TransaccionDelete = nuTransaccion;

                uow.RecorridoRepository.UpdateRecorrido(recorrido);
                uow.SaveChanges();

                uow.RecorridoRepository.DeleteRecorrido(recorrido);
                uow.SaveChanges();
            }
        }

        #endregion

    }
}