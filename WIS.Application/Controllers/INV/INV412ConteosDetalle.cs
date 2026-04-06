using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Inventario;
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

namespace WIS.Application.Controllers.INV
{
    public class INV412ConteosDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<INV412ConteosDetalle> _logger;

        protected List<string> GridKeys { get; }
        protected List<string> EditableCells { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INV412ConteosDetalle(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<INV412ConteosDetalle> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_INVENTARIO_ENDERECO_DET"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_INVENTARIO_ENDERECO_DET", SortDirection.Descending)
            };

            this.EditableCells = new List<string>
            {
                "CD_MOTIVO_AJUSTE"
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsCommitEnabled = true;
            context.IsRollbackEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            var nuInventario = context.GetParameter("inventario");
            if (!string.IsNullOrEmpty(nuInventario) && !decimal.TryParse(nuInventario, _identity.GetFormatProvider(), out decimal i))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var showOnlyPending = context.GetParameter("showOnlyPending");
            if (!string.IsNullOrEmpty(showOnlyPending) && !bool.TryParse(showOnlyPending, out bool b))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnAtributosCabezal", "INV410_Sec0_btn_AtributosCabezal", "fas fa-list"),
                new GridButton("btnAtributosDetalle", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
                new GridButton("btnAtributosDetalleTemporal", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
            }));

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_MOTIVO_AJUSTE", OptionSelectMotivo()));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var showOnlyPending = bool.TryParse(context.GetParameter("showOnlyPending"), out bool b) ? b : false;
            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV412GridQuery(nuInventario, showOnlyPending);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            grid.SetEditableCells(this.EditableCells);

            grid.Rows.ForEach(row =>
            {
                if (string.IsNullOrEmpty(row.GetCell("NU_LPN").Value))
                {
                    row.DisabledButtons.Add("btnAtributosCabezal");
                    row.DisabledButtons.Add("btnAtributosDetalle");
                    row.DisabledButtons.Add("btnAtributosDetalleTemporal");
                }
                else
                {
                    if (string.IsNullOrEmpty(row.GetCell("ID_LPN_DET").Value))
                        row.DisabledButtons.Add("btnAtributosDetalle");
                    else
                        row.DisabledButtons.Add("btnAtributosDetalleTemporal");
                }

                var estadoDetalle = row.GetCell("ND_ESTADO_INV_ENDERECO_DET").Value;
                if (estadoDetalle != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF && estadoDetalle != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC)
                    row.SetEditableCells(new List<string>());
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var showOnlyPending = bool.TryParse(context.GetParameter("showOnlyPending"), out bool b) ? b : false;
            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), _identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV412GridQuery(nuInventario, showOnlyPending);
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

            var showOnlyPending = bool.TryParse(context.GetParameter("showOnlyPending"), out bool b) ? b : false;
            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), _identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV412GridQuery(nuInventario, showOnlyPending);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                uow.CreateTransactionNumber("GridCommit");

                foreach (var row in grid.Rows)
                {
                    var nuDetalleInventario = decimal.Parse(row.GetCell("NU_INVENTARIO_ENDERECO_DET").Value, _identity.GetFormatProvider());

                    var detalleInventario = uow.InventarioRepository.GetInventarioEnderecoDetalle(nuDetalleInventario);

                    if (detalleInventario == null)
                    {
                        throw new ValidationFailedException("INV412_Sec0_Error_ConteoXNoSeEncontro", new string[] { nuDetalleInventario.ToString() });
                    }
                    else if (detalleInventario.Estado != (EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF) && detalleInventario.Estado != (EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC))
                    {
                        throw new ValidationFailedException("INV412_Sec0_Error_ImposibleEditarConteo");
                    }

                    detalleInventario.MotivoAjuste = row.GetCell("CD_MOTIVO_AJUSTE").Value;
                    detalleInventario.NumeroTransaccion = uow.GetTransactionNumber();
                    detalleInventario.UserId = _identity.UserId;

                    uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalleInventario);
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridCommit");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridCommit");
                uow.Rollback();
                throw;
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new INV412GridValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnAtributosCabezal":
                    context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter() { Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter() { Id = "detalle", Value = "false" },
                        });
                    break;

                case "btnAtributosDetalle":
                    context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter() { Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter() { Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                            new ComponentParameter() { Id = "detalle", Value = "true" },
                        });
                    break;
            }

            return context;
        }

        #region Metodos auxiliares

        public virtual List<SelectOption> OptionSelectMotivo()
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var motivos = uow.AjusteRepository.GetsMotivosAjuste();

            foreach (var motivo in motivos)
            {
                opciones.Add(new SelectOption(motivo.Codigo, $"{motivo.Codigo} - {motivo.Descripcion}"));
            }

            return opciones;
        }

        #endregion
    }
}
