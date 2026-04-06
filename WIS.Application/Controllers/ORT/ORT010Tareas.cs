using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.OrdenTarea;
using WIS.Domain.OrdenTarea;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.ORT
{
    public class ORT010Tareas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<ORT010Tareas> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT010Tareas(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<ORT010Tareas> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_TAREA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_TAREA", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_TAREA",
                "DS_TAREA",
                "NU_COMPONENTE",
                "FL_REGISTRO_HORAS",
                "FL_REGISTRO_HORAS_EQUIPO",
                "FL_REGISTRO_MANIPULEO",
                "FL_REGISTRO_INSUMOS"
            });

            using var uow = this._uowFactory.GetUnitOfWork();

            //Cargo select
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_SITUACAO", this.SelectSituacion(uow)));

            //Cargo default values
            var defaultColumns = new Dictionary<string, string>();
            defaultColumns.Add("CD_SITUACAO", SituacionDb.Activo.ToString());
            defaultColumns.Add("TP_TAREA", "MANUAL");

            grid.SetColumnDefaultValues(defaultColumns);

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TareasQuery dbQuery = new TareasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                if (row.GetCell("TP_TAREA").Value == "MANUAL")
                {
                    row.SetEditableCells(new List<string> {
                        "DS_TAREA",
                        "CD_SITUACAO",
                        "FL_REGISTRO_HORAS",
                        "FL_REGISTRO_HORAS_EQUIPO",
                        "FL_REGISTRO_MANIPULEO",
                        "FL_REGISTRO_INSUMOS",
                        "NU_COMPONENTE",
                    });
                }
                else
                {
                    row.SetEditableCells(new List<string> {
                        "NU_COMPONENTE",
                    });
                }
            }

            return grid;
        }
        
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionTarea registroModificacionT = new RegistroModificacionTarea(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            Tarea tarea = this.CrearTarea(uow, row, query);
                            registroModificacionT.RegistrarTarea(tarea);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteTareaManual(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            Tarea tarea = this.UpdateTarea(uow, row, query);
                            registroModificacionT.ModificarTarea(tarea);
                        }
                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch(ValidationFailedException ex)
            {
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF010GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoTareasGridValidationModule(uow), grid, row, context);
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TareasQuery dbQuery = new TareasQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TareasQuery dbQuery = new TareasQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual Tarea CrearTarea(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            Tarea nuevaTarea = new Tarea();

            var registroHoras = row.GetCell("FL_REGISTRO_HORAS").Value;
            var registroHorasEquipo = row.GetCell("FL_REGISTRO_HORAS_EQUIPO").Value;
            var registroManipuleos = row.GetCell("FL_REGISTRO_MANIPULEO").Value;
            var registroInsumos = row.GetCell("FL_REGISTRO_INSUMOS").Value;

            nuevaTarea.Id = row.GetCell("CD_TAREA").Value;
            nuevaTarea.Descripcion = row.GetCell("DS_TAREA").Value;
            nuevaTarea.CodigoSituacion = short.Parse(row.GetCell("CD_SITUACAO").Value);
            nuevaTarea.RegistroHoras = !string.IsNullOrEmpty(registroHoras) ? registroHoras : "N";
            nuevaTarea.RegistroHorasEquipo = !string.IsNullOrEmpty(registroHorasEquipo) ? registroHorasEquipo : "N";
            nuevaTarea.RegistroManipuleo = !string.IsNullOrEmpty(registroManipuleos) ? registroManipuleos : "N";
            nuevaTarea.RegistroInsumos = !string.IsNullOrEmpty(registroInsumos) ? registroInsumos : "N";
            nuevaTarea.TipoTarea = row.GetCell("TP_TAREA").Value;
            nuevaTarea.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;

            return nuevaTarea;
        }

        public virtual Tarea UpdateTarea(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string cdTarea = row.GetCell("CD_TAREA").Value;

            Tarea tarea = new Tarea();
            tarea = uow.TareaRepository.GetTarea(cdTarea);

            tarea.Descripcion = row.GetCell("DS_TAREA").Value;
            tarea.CodigoSituacion = short.Parse(row.GetCell("CD_SITUACAO").Value);
            tarea.RegistroHoras = row.GetCell("FL_REGISTRO_HORAS").Value;
            tarea.RegistroHorasEquipo = row.GetCell("FL_REGISTRO_HORAS_EQUIPO").Value;
            tarea.RegistroManipuleo = row.GetCell("FL_REGISTRO_MANIPULEO").Value;
            tarea.RegistroInsumos = row.GetCell("FL_REGISTRO_INSUMOS").Value;
            tarea.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;

            return tarea;
        }

        public virtual void DeleteTareaManual(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string cdTarea = row.GetCell("CD_TAREA").Value;

            if (uow.TareaRepository.AnyTarea(cdTarea))
            {
                if (!uow.TareaRepository.AnyOrdenTarea(cdTarea))
                {
                    string tipoTarea = uow.TareaRepository.GetTipoTarea(cdTarea);

                    if (tipoTarea == "MANUAL")
                    {
                        uow.TareaRepository.DeleteTarea(cdTarea);
                    }
                    else
                    {
                        throw new ValidationFailedException("ORT010_Sec0_Error_TipoTareaNoPermiteEliminar");
                    }
                }
                else
                {
                    throw new ValidationFailedException("ORT010_Sec0_Error_TareaContieneInstancias");
                }
            }
            else
            {
                throw new ValidationFailedException("ORT010_Sec0_Error_TareaNoExisteEliminar");
            }
        }

        public virtual List<SelectOption> SelectSituacion(IUnitOfWork uow)
        {
            List<int> situaciones = new List<int> { SituacionDb.Activo, SituacionDb.Inactivo};

            return uow.SituacionRepository.GetSituaciones(situaciones).Select(w => new SelectOption(w.Id.ToString(), w.Id + " - " + w.Descripcion)).ToList();
        }

        #endregion
    }
}
