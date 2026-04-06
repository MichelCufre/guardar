using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
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
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REG
{
    public class REG700AsociarAplicacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IParameterService _parameterService;
        protected readonly IFactoryService _factoryService;
        protected readonly ITrackingService _trackingService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG700AsociarAplicacion(
            IIdentityService identity,
            IFactoryService factoryService,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IParameterService parameterService,
            ITrackingService trackingService)
        {
            this.GridKeys = new List<string>
            {
                "NU_RECORRIDO", "CD_APLICACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_APLICACION",SortDirection.Ascending)
            };

            this._identity = identity;
            this._factoryService = factoryService;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._parameterService = parameterService;
            this._trackingService = trackingService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._security.IsUserAllowed(SecurityResources.WREG700_grid1_btn_btnAsociarAplicaciones))
            {
                if (grid.Id == "AgregarAplicacion_grid_1")
                    grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
                else if (grid.Id == "QuitarAplicacion_grid_2")
                    grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_Quitar"));
            }

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int recorrido = int.Parse(context.GetParameter("recorrido"));


            if (grid.Id == "AgregarAplicacion_grid_1")
            {
                var dbQuery = new AplicacionesDisponibleRecorridoQuery(recorrido);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else if (grid.Id == "QuitarAplicacion_grid_2")
            {
                var dbQuery = new AplicacionesAsociadaRecorridoQuery(recorrido);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    if (row.GetCell("FL_PREDETERMINADO").Value == "S")
                    {
                        row.DisabledSelected = true;
                    }
                }

            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int recorrido = int.Parse(query.GetParameter("recorrido"));


            if (grid.Id == "AgregarAplicacion_grid_1")
            {
                var dbQuery = new AplicacionesDisponibleRecorridoQuery(recorrido);
                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "QuitarAplicacion_grid_2")
            {
                var dbQuery = new AplicacionesAsociadaRecorridoQuery(recorrido);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int recorrido = int.Parse(context.GetParameter("recorrido"));

            if (grid.Id == "AgregarAplicacion_grid_1")
            {
                var dbQuery = new AplicacionesDisponibleRecorridoQuery(recorrido);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "QuitarAplicacion_grid_2")
            {
                var dbQuery = new AplicacionesAsociadaRecorridoQuery(recorrido);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarAplicacion_grid_1" && context.ButtonId == "btnAgregar")
                this.ProcesarAgregar(context);
            else if (context.GridId == "QuitarAplicacion_grid_2" && context.ButtonId == "btnQuitar")
                this.ProcesarQuitar(context);

            return context;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            if (query.ButtonId == "btnCerrar")
                this._concurrencyControl.ClearToken();
            return base.FormButtonAction(form, query);
        }

        public virtual void CheckIfLocked(string recorrido)
        {
            if (this._concurrencyControl.IsLocked("T_RECORRIDO", recorrido))
                throw new EntityLockedException("General_Sec0_Error_LockedEntity");
        }

        public virtual void ProcesarAgregar(GridMenuItemActionContext context)
        {
            int recorrido = int.Parse(context.GetParameter("recorrido"));
            this.CheckIfLocked(context.GetParameter("recorrido"));

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("AplicacionRecorrido: Agregar");
            uow.BeginTransaction();

            try
            {
                var selection = context.Selection.GetSelection(this.GridKeys);
                var aplicaciones = selection.Select(item => new AplicacionRecorrido
                {
                    IdRecorrido = int.Parse(item["NU_RECORRIDO"]),
                    IdAplicacion = item["CD_APLICACION"]
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new AplicacionesDisponibleRecorridoQuery(recorrido);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var contenedoresSeleccion = dbQuery.GetAllAplicaciones();
                    aplicaciones = contenedoresSeleccion.Except(aplicaciones).ToList();
                }

                foreach (var aplicacion in aplicaciones)
                {
                    if (!uow.RecorridoRepository.AnyAplicacionRecorridoAsociado(aplicacion))
                    {
                        aplicacion.NuTransaccion = uow.GetTransactionNumber();
                        aplicacion.FechaAlta = DateTime.Now;
                        aplicacion.EsPredeterminado = false;
                        uow.RecorridoRepository.AddAplicacionesRecorrido(aplicacion);
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));

                uow.Rollback();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);

                uow.Rollback();
            }
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context)
        {
            int recorrido = int.Parse(context.GetParameter("recorrido"));
            this.CheckIfLocked(context.GetParameter("recorrido"));

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("AplicacionRecorrido: Quitar");
            uow.BeginTransaction();

            try
            {
                var selection = context.Selection.GetSelection(this.GridKeys);
                var aplicacionesAsociadas = selection.Select(item => new AplicacionRecorrido
                {
                    IdRecorrido = int.Parse(item["NU_RECORRIDO"]),
                    IdAplicacion = item["CD_APLICACION"],
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new AplicacionesAsociadaRecorridoQuery(recorrido);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var contenedoresSeleccion = dbQuery.GetAllAplicacionesAsociadas();
                    aplicacionesAsociadas = contenedoresSeleccion.Except(aplicacionesAsociadas).ToList();
                }

                foreach (var aplicacion in aplicacionesAsociadas)
                {
                    var aplicacionAsociada = uow.RecorridoRepository.GetAplicacionAsociada(aplicacion.IdRecorrido, aplicacion.IdAplicacion);

                    if (aplicacionAsociada.EsPredeterminado)
                    {
                        throw new ValidationFailedException("REG700_msg_Error_AplicacionPorPredeterminada");
                    }

                    if (aplicacionAsociada != null)
                    {
                        aplicacionAsociada.NuTransaccion = uow.GetTransactionNumber();
                        aplicacionAsociada.NuTransaccionDelete = uow.GetTransactionNumber();
                        uow.RecorridoRepository.UpdateRecorridoAsociado(aplicacionAsociada);
                        uow.SaveChanges();
                        uow.RecorridoRepository.EliminarAplicacionAsociada(aplicacionAsociada);
                        uow.SaveChanges();
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));

                uow.Rollback();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);

                uow.Rollback();
            }
        }
    }
}