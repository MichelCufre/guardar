using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT040InstanciasEvento : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT040InstanciasEvento> _logger;

        protected List<string> GridKeys { get; }

        protected List<SortCommand> DefaultSort { get; }

        public EVT040InstanciasEvento(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT040InstanciasEvento> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_EVENTO_INSTANCIA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EVENTO_INSTANCIA", SortDirection.Descending),
            };

            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnDestinatarios", "General_Sec0_btn_Destinatarios", "fa-solid fa-users-line")
             }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InstanciasEventosQuery dbQuery = new InstanciasEventosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT040 Eliminar Instancia");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            var numeroInstancia = int.Parse(row.GetCell("NU_EVENTO_INSTANCIA").Value);

                            if (uow.EventoRepository.AnyInstanciaNotificacion(numeroInstancia))
                                throw new ValidationFailedException("EVT040_Sec0_Error_InstanciaNotificacion");

                            var parametrosInstancias = uow.EventoRepository.GetParametrosInstancia(numeroInstancia);

                            foreach (var parametro in parametrosInstancias)
                            {
                                parametro.FechaModificacion = DateTime.Now;
                                parametro.NumeroTransaccion = nuTransaccion;
                                parametro.NumeroTransaccionDelete = nuTransaccion;

                                uow.EventoRepository.UpdateParametroInstancia(parametro);
                                uow.SaveChanges();

                                uow.EventoRepository.RemoveParametroInstancia(parametro);
                                uow.SaveChanges();
                            }

                            var instanciaGrupo = uow.DestinatarioRepository.GetGruposInstancia(numeroInstancia);

                            foreach (var grupo in instanciaGrupo)
                            {
                                grupo.FechaModificacion = DateTime.Now;
                                grupo.NumeroTransaccion = nuTransaccion;
                                grupo.NumeroTransaccionDelete = nuTransaccion;

                                uow.DestinatarioRepository.UpdateGrupoInstancia(grupo);
                                uow.SaveChanges();

                                uow.DestinatarioRepository.RemoveGrupoOfInstancia(grupo);
                                uow.SaveChanges();
                            }

                            var instancia = uow.EventoRepository.GetInstancia(numeroInstancia);

                            instancia.FechaModificacion = DateTime.Now;
                            instancia.NumeroTransaccion = nuTransaccion;
                            instancia.NumeroTransaccionDelete = nuTransaccion;

                            uow.EventoRepository.UpdateInstancia(instancia);
                            uow.SaveChanges();

                            uow.EventoRepository.RemoveInstancia(instancia);

                            uow.SaveChanges();
                            uow.Commit();
                        }
                    }
                }

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex.Message, "EVT040GridCommit");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "EVT040GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InstanciasEventosQuery dbQuery = new InstanciasEventosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InstanciasEventosQuery dbQuery = null;

            dbQuery = new InstanciasEventosQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
