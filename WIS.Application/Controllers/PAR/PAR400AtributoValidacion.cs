using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Parametrizacion;
using WIS.Domain.Parametrizacion;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PAR
{
    public class PAR400AtributoValidacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PAR400Atributos> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys1 { get; }
        protected List<SortCommand> DefaultSort1 { get; }
        protected List<string> GridKeys2 { get; }
        protected List<SortCommand> DefaultSort2 { get; }
        public PAR400AtributoValidacion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<PAR400Atributos> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys1 = new List<string>
            {
                "ID_VALIDACION",
            };

            this.DefaultSort1 = new List<SortCommand>
            {
                new SortCommand("ID_VALIDACION", SortDirection.Ascending),
            };

            this.GridKeys2 = new List<string>
            {
                "ID_ATRIBUTO","ID_VALIDACION"
            };

            this.DefaultSort2 = new List<SortCommand>
            {
                new SortCommand("ID_ATRIBUTO", SortDirection.Ascending),
                new SortCommand("ID_VALIDACION", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;

        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var idAtributo = context.GetParameter("codigoAtributo");
            var nmAtributo = context.GetParameter("nmAtributo");

            form.GetField("ID_ATRIBUTO").Value = idAtributo;
            form.GetField("NM_ATRIBUTO").Value = nmAtributo;

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = false;
            query.IsRemoveEnabled = false;
            query.IsCommitEnabled = false;

            using var uow = this._uowFactory.GetUnitOfWork();

            int idAtributo = int.Parse(query.GetParameter("codigoAtributo"));

            if (uow.AtributoRepository.AnyLpnTipoAsociadaAtributo(idAtributo))
            {
                query.Parameters.Add(new ComponentParameter() { Id = "BtnDisable", Value = "F" });
            }
            else
            {
                query.Parameters.Add(new ComponentParameter() { Id = "BtnDisable", Value = "T" });

                if (grid.Id == "PAR400ValidacionesDisponible_grid_1")
                {
                    grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_AgregarSeleccion"));
                }
                else if (grid.Id == "PAR400ValidacionesAsociadas_grid_2")
                {
                    grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_QuitarSeleccion"));
                }
            }

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PAR400GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int idAtributo = int.Parse(query.GetParameter("codigoAtributo"));

            if (grid.Id == "PAR400ValidacionesDisp_grid_1")
            {
                var tpAtributo = query.GetParameter("tipoAtributo");
                var listIdValidacion = uow.AtributoRepository.GetIdValidacionesAsociada(idAtributo);
                var dbQuery = new AtributosValidacionDisponibleQuery(listIdValidacion, tpAtributo);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort1, this.GridKeys1);
            }
            else
            {
                var dbQuery = new AtributosValidacionAsociadosQuery(idAtributo);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort2, this.GridKeys2);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int idAtributo = int.Parse(query.GetParameter("codigoAtributo"));

            if (grid.Id == "PAR400ValidacionesDisp_grid_1")
            {
                var tpAtributo = query.GetParameter("tipoAtributo");
                var listIdValidacion = uow.AtributoRepository.GetIdValidacionesAsociada(idAtributo);
                var dbQuery = new AtributosValidacionDisponibleQuery(listIdValidacion, tpAtributo);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                var dbQuery = new AtributosValidacionAsociadosQuery(idAtributo);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int idAtributo = int.Parse(query.GetParameter("codigoAtributo"));

            if (grid.Id == "PAR400ValidacionesDisp_grid_1")
            {
                var tpAtributo = query.GetParameter("tipoAtributo");
                var listIdValidacion = uow.AtributoRepository.GetIdValidacionesAsociada(idAtributo);
                var dbQuery = new AtributosValidacionDisponibleQuery(listIdValidacion, tpAtributo);

                uow.HandleQuery(dbQuery);
                query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort1);
            }
            else
            {
                var dbQuery = new AtributosValidacionAsociadosQuery(idAtributo);
                uow.HandleQuery(dbQuery);
                query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort2);
            }
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            if (context.GridId == "PAR400ValidacionesDisp_grid_1" && context.ButtonId == "btnAgregar")
            {
                var selection = context.Selection.GetSelection(this.GridKeys1);
                var listAtributovalidacion = selection.Select(item => new AtributoValidacion
                {
                    Id = short.Parse(item["ID_VALIDACION"]),
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var idAtributo = int.Parse(context.GetParameter("codigoAtributo"));
                    var tpAtributo = context.GetParameter("tipoAtributo");
                    var listIdValidacion = uow.AtributoRepository.GetIdValidacionesAsociada(idAtributo);
                    var dbQuery = new AtributosValidacionDisponibleQuery(listIdValidacion, tpAtributo);

                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var AtributovalidacionSeleccion = dbQuery.GetAtributoValidacion();
                    listAtributovalidacion = AtributovalidacionSeleccion.Except(listAtributovalidacion).ToList();
                }

                var listEspecificacionValorDeValidacion = this.ProcesarAgregar(context, listAtributovalidacion);

                if (listEspecificacionValorDeValidacion.Count > 0)
                {
                    context.Parameters.Add(new ComponentParameter() { Id = "ListValidacion", Value = JsonConvert.SerializeObject(listEspecificacionValorDeValidacion) });
                }
                else
                {
                    context.Parameters.Add(new ComponentParameter() { Id = "ListValidacion", Value = "" });
                }

            }
            else if (context.GridId == "PAR400ValidacionesAso_grid_2" && context.ButtonId == "btnQuitar")
            {
                var selection = context.Selection.GetSelection(this.GridKeys2);
                var listAtributovalidacionAsociada = selection.Select(item => new AtributoValidacionAsociada
                {
                    IdValidacion = short.Parse(item["ID_VALIDACION"]),
                    IdAtributo = int.Parse(item["ID_ATRIBUTO"]),

                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var idAtributo = int.Parse(context.GetParameter("codigoAtributo"));
                    var tpAtributo = context.GetParameter("tipoAtributo");
                    var listIdValidacion = uow.AtributoRepository.GetIdValidacionesAsociada(idAtributo);
                    var dbQuery = new AtributosValidacionAsociadosQuery(idAtributo);

                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var AtributovalidacionSeleccion = dbQuery.GetAtributoValidacionAsociada();
                    listAtributovalidacionAsociada = AtributovalidacionSeleccion.Except(listAtributovalidacionAsociada).ToList();
                }

                this.ProcesarQuitar(context, listAtributovalidacionAsociada);
            }

            return context;
        }

        public virtual List<AtributoValidacion> ProcesarAgregar(GridMenuItemActionContext context, List<AtributoValidacion> listAtributovalidacion)
        {
            var listEspecificacionValorDeValidacion = new List<AtributoValidacion>();

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("ArmarEgresoCarga: Quitar");
            uow.BeginTransaction();

            try
            {
                foreach (var atributoValidacion in listAtributovalidacion)
                {
                    var validacion = uow.AtributoRepository.GetAtributoValidacion(atributoValidacion.Id);

                    if (string.IsNullOrEmpty(validacion.TipoArgumento))
                    {
                        int idAtributo = int.Parse(context.GetParameter("codigoAtributo"));
                        var new_Asociada = new AtributoValidacionAsociada();
                        new_Asociada.IdValidacion = atributoValidacion.Id;
                        new_Asociada.IdAtributo = idAtributo;
                        uow.AtributoRepository.AddAtributoValidacionAsociada(new_Asociada);
                    }
                    else
                    {
                        listEspecificacionValorDeValidacion.Add(validacion);
                    }
                }
                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception)
            {
                uow.Rollback();
                throw;
            }

            return listEspecificacionValorDeValidacion;
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context, List<AtributoValidacionAsociada> ListAtributovalidacion)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("ArmarEgresoCarga: Quitar");
            uow.BeginTransaction();

            try
            {
                foreach (var atributoValidacionAsociada in ListAtributovalidacion)
                {
                    var deleteAsociada = uow.AtributoRepository.GetAtributoValidacionAsociada(atributoValidacionAsociada.IdValidacion, atributoValidacionAsociada.IdAtributo);
                    uow.AtributoRepository.DeleteAtributoValidacionAsociada(deleteAsociada);
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception)
            {
                uow.Rollback();
                throw;
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }
    }
}
