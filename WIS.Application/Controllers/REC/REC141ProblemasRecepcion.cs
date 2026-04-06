using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Exceptions;
using WIS.Filtering;
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

namespace WIS.Application.Controllers.REC
{
    public class REC141ProblemasRecepcion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ILogger<REC141ProblemasRecepcion> _logger;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC141ProblemasRecepcion(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            ILogger<REC141ProblemasRecepcion> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_AGENDA_PROBLEMA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_RECEPCION_AGENDA_PROBLEMA", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._security.IsUserAllowed(SecurityResources.REC141_btn_Access_AceptarProblema))
                grid.MenuItems.Add(new GridButton("btnAceptarProblema", "REC141_Sec0_btn_AceptarProblema", string.Empty, new ConfirmMessage("REC141_Sec0_msg_AceptarProblema")));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProblemasDeRecepcionQuery dbQuery;

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal idEmbalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;
                string identificador = context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value;

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda, idEmbalaje, idProducto, identificador);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);


                context.AddParameter("REC141_NU_AGENDA", idAgenda.ToString());
                context.AddParameter("REC141_NU_IDENTIFICADOR", identificador);
                context.AddParameter("REC141_CD_FAIXA", idEmbalaje.ToString());

                grid.GetColumn("CD_FAIXA").Hidden = true;
                grid.GetColumn("NU_IDENTIFICADOR").Hidden = true;
                grid.GetColumn("NU_AGENDA").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;

                string ds_Prod = " - ";

                if (grid.Rows.Count > 0)
                {
                    ds_Prod = ds_Prod + grid.Rows.FirstOrDefault().GetCell("DS_PRODUTO").Value;
                    grid.GetColumn("DS_PRODUTO").Hidden = true;

                }

                context.AddParameter("REC141_CD_PRODUCTO", idProducto + ds_Prod);

            }
            else if (context.Parameters.Count > 0 && context.Parameters.Any(x => x.Id == "agenda"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);


                context.AddParameter("REC141_NU_AGENDA", idAgenda.ToString());
                grid.GetColumn("NU_AGENDA").Hidden = true;

            }
            else
            {
                dbQuery = new ProblemasDeRecepcionQuery();
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }


            foreach (GridRow row in grid.Rows)
            {
                if (!row.GetCell("FL_ACEPTADO").Value.Equals("S"))
                {
                    row.CssClass = row.CssClass + "noAceptado";
                    row.GetCell("FL_ACEPTADO").Editable = true;
                }
                else
                    row.DisabledSelected = true;
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProblemasDeRecepcionQuery dbQuery;

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                if (!decimal.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal idEmbalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;
                string identificador = context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value;

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda, idEmbalaje, idProducto, identificador);

            }
            else if (context.Parameters.Count > 0 && context.Parameters.Any(x => x.Id == "agenda"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda);

            }

            else
            {
                dbQuery = new ProblemasDeRecepcionQuery();

            }

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProblemasDeRecepcionQuery dbQuery;

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("");
                if (!decimal.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal idEmbalaje))
                    throw new MissingParameterException("");
                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "producto").Value))
                    throw new MissingParameterException("");
                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value))
                    throw new MissingParameterException("");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;
                string identificador = context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value;

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda, idEmbalaje, idProducto, identificador);

            }
            else if (context.Parameters.Count > 0 && context.Parameters.Any(x => x.Id == "agenda"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda);

            }
            else
                dbQuery = new ProblemasDeRecepcionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.CreateTransactionNumber($"{this._identity.Application} - Problemas de Recepcion - GridCommit");
                uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    var listaProblemas = new Dictionary<int, int>();

                    foreach (var row in grid.Rows)
                    {

                        // rows editadas
                        GridCell flag = row.GetCell("FL_ACEPTADO");
                        if (flag.Value != flag.Old)
                        {

                            listaProblemas.Add(int.Parse(row.GetCell("NU_RECEPCION_AGENDA_PROBLEMA").Value), int.Parse(row.GetCell("NU_AGENDA").Value));

                        }
                    }

                    AceptarProblemasRecepcion problemas = new AceptarProblemasRecepcion(uow, _concurrencyControl, _identity.UserId, _identity.Application);
                    problemas.AceptarProblemas(listaProblemas);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "REC141GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
            }
            finally
            {
                uow.EndTransaction();
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoProblemasDeRecepcionValidationModule(uow), grid, row, context);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.ButtonId == "btnAceptarProblema")
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                try
                {
                    uow.CreateTransactionNumber($"{this._identity.Application} - Problemas de Recepcion - GridMenuItemAction");

                    uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

                    List<int> listaProblemas = new List<int>();

                    if (context.Selection.AllSelected)
                        listaProblemas = this.GetProblemasAllSelected(uow, context);
                    else
                        listaProblemas = this.GetProblemasSelection(context);

                    AceptarProblemasRecepcion problemas = new AceptarProblemasRecepcion(uow, _concurrencyControl, _identity.UserId, _identity.Application);
                    problemas.AceptarProblemas(listaProblemas);

                    uow.SaveChanges();
                    uow.Commit();

                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
                catch (ExpectedException ex)
                {
                    uow.Rollback();
                    context.AddErrorNotification(ex.Message);
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    this._logger.LogError(ex, "REC141GridMenuItemAction");
                    context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
                }
                finally
                {
                    uow.EndTransaction();
                }
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual List<int> GetProblemasAllSelected(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            ProblemasDeRecepcionQuery dbQuery;

            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal idEmbalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "producto").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;
                string identificador = context.Parameters.FirstOrDefault(x => x.Id == "identificador").Value;

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda, idEmbalaje, idProducto, identificador);
            }
            else if (context.Parameters.Count > 0 && context.Parameters.Any(x => x.Id == "agenda"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ProblemasDeRecepcionQuery(idAgenda);
            }
            else
            {
                dbQuery = new ProblemasDeRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
            return dbQuery.GetProblemasNoAceptados();
        }

        public virtual List<int> GetProblemasSelection(GridMenuItemActionContext context)
        {
            List<int> problemas = new List<int>();

            foreach (var id in context.Selection.Keys)
            {
                problemas.Add(int.Parse(id));
            }
            return problemas;
        }
        #endregion
    }
}