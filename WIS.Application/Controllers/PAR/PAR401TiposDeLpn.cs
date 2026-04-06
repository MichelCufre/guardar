using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Parametrizacion;
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

namespace WIS.Application.Controllers.PAR
{
    public class PAR401TiposDeLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly ISecurityService _security;
        protected readonly ILogger<PAR401TiposDeLpn> _logger;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PAR401TiposDeLpn(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            ISecurityService security,
            ILogger<PAR401TiposDeLpn> logger,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "TP_LPN_TIPO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("TP_LPN_TIPO", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;
            this._gridValidationService = gridValidationService;

        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = true;

            using var uow = this._uowFactory.GetUnitOfWork();

            grid.AddOrUpdateColumn(new GridColumnSelect("NU_TEMPLATE_ETIQUETA", this.SelectEtiquetaEstilos(uow)));
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                 new GridButton("btnAtributos", "PAR401_Sec0_btn_Atributos", "fas fa-bezier-curve")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TiposDeLpnQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "NM_LPN_TIPO",
                "DS_LPN_TIPO",
                "NU_TEMPLATE_ETIQUETA",
                "NU_COMPONENTE",
                "FL_PERMITE_GENERAR",
                "FL_PERMITE_CONSOLIDAR",
                "FL_PERMITE_AGREGAR_LINEAS",
                "FL_MULTIPRODUCTO",
                "FL_MULTI_LOTE",
                "FL_PERMITE_DESTRUIR_ALM",
                "VL_PREFIJO",
                "FL_CONTENEDOR_LPN",
            });

            grid.Rows.ForEach(row =>
            {
                var lpnEnUso = uow.ManejoLpnRepository.AnyTipoLpnEnUso(row.GetCell("TP_LPN_TIPO").Value);

                if (row.GetCell("FL_PERMITE_GENERAR").Value == "N")
                    row.GetCell("NU_SEQ_LPN").Editable = false;

                var cell = row.GetCell("NU_COMPONENTE");
                cell.CssClass = "inputCellMayus";

                if (lpnEnUso)
                {
                    row.GetCell("FL_PERMITE_CONSOLIDAR").Editable = false;
                    row.GetCell("FL_PERMITE_AGREGAR_LINEAS").Editable = false;
                    row.GetCell("FL_MULTIPRODUCTO").Editable = false;
                    row.GetCell("FL_MULTI_LOTE").Editable = false;
                    row.GetCell("FL_PERMITE_GENERAR").Editable = false;
                    row.GetCell("NU_SEQ_LPN").Editable = false;
                    row.GetCell("FL_INGRESO_RECEPCION_ATRIBUTO").Editable = false;
                    row.GetCell("FL_PERMITE_DESTRUIR_ALM").Editable = false;
                    row.GetCell("VL_PREFIJO").Editable = false;
                }
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TiposDeLpnQuery();
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

            var dbQuery = new TiposDeLpnQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var operacionRealizadaParcial = false;
                var eliminoAlMenosUnRegistro = false;

                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var tipoLpn = row.GetCell("TP_LPN_TIPO").Value;

                        if (row.IsDeleted)
                        {
                            if (uow.ManejoLpnRepository.AnyTipoLpnEnUso(tipoLpn))
                                operacionRealizadaParcial = true;
                            else
                            {
                                var tpContenedor = uow.ContenedorRepository.GetTipoContenedor(tipoLpn);
                                uow.ContenedorRepository.DeleteTipoContenedor(tpContenedor);

                                uow.ManejoLpnRepository.EliminarAtributoAsociados(tipoLpn);

                                var tpLpn = uow.ManejoLpnRepository.GetTipoLpn(tipoLpn);
                                uow.ManejoLpnRepository.DeleteTipoLpn(tpLpn);

                                eliminoAlMenosUnRegistro = true;
                            }
                        }
                        else
                        {
                            var tipo = uow.ManejoLpnRepository.GetTipoLpn(tipoLpn);

                            tipo.Tipo = tipoLpn;
                            tipo.Nombre = row.GetCell("NM_LPN_TIPO").Value;
                            tipo.Descripcion = row.GetCell("DS_LPN_TIPO").Value;
                            tipo.PermiteConsolidar = row.GetCell("FL_PERMITE_CONSOLIDAR").Value;
                            tipo.PermiteAgregarLineas = row.GetCell("FL_PERMITE_AGREGAR_LINEAS").Value;
                            tipo.MultiProducto = row.GetCell("FL_MULTIPRODUCTO").Value;
                            tipo.MultiLote = row.GetCell("FL_MULTI_LOTE").Value;
                            tipo.NumeroTemplate = row.GetCell("NU_TEMPLATE_ETIQUETA").Value;
                            tipo.NumeroComponente = row.GetCell("NU_COMPONENTE").Value.ToUpper();
                            tipo.ContenedorLPN = row.GetCell("FL_CONTENEDOR_LPN").Value;
                            tipo.PermiteGenerar = row.GetCell("FL_PERMITE_GENERAR").Value;
                            tipo.PermiteDestruirAlmacenaje = row.GetCell("FL_PERMITE_DESTRUIR_ALM").Value;
                            tipo.Prefijo = row.GetCell("VL_PREFIJO").Value.ToUpper();

                            if (!string.IsNullOrEmpty(row.GetCell("NU_SEQ_LPN").Value))
                                tipo.NumeroSecuencia = long.Parse(row.GetCell("NU_SEQ_LPN").Value);
                            else
                                tipo.NumeroSecuencia = null;

                            uow.ManejoLpnRepository.UpdateTipoLpn(tipo);
                        }
                    }
                }

                uow.SaveChanges();

                if (operacionRealizadaParcial && !eliminoAlMenosUnRegistro)
                    context.AddErrorNotification("PAR401_Sec0_Error_ErrorOperacionNoRealizada");
                else if (operacionRealizadaParcial && eliminoAlMenosUnRegistro)
                    context.AddErrorNotification("PAR401_Sec0_Error_ErrorOperacionParcial");
                else
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PAR110GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new TipoLpnGridValidationModule(uow, this._identity, this._security), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnAtributos":
                    if (this._concurrencyControl.IsLocked("T_LPN_TIPO", context.Parameters.FirstOrDefault(s => s.Id == "TP_LPN_TIPO").Value))
                    {
                        context.Parameters.Add(new ComponentParameter("Lockeada", "true"));
                        throw new EntityLockedException("PAR401_Frm1_Error_EdicionLockeada");
                    }
                    else
                    {
                        context.Redirect("/parametrizacion/PAR401AtributosTipo", new List<ComponentParameter>
                        {
                            new ComponentParameter("LpnTipo", context.Row.GetCell("TP_LPN_TIPO").Value),
                            new ComponentParameter("Nombre", context.Row.GetCell("NM_LPN_TIPO").Value)
                        });
                    }
                    break;
            }

            return context;
        }

        public virtual List<SelectOption> SelectEtiquetaEstilos(IUnitOfWork uow)
        {
            return uow.EstiloEtiquetaRepository.GetEtiquetaEstilos(EstiloEtiquetaDb.EtiquetaLpn)
                .Select(w => new SelectOption(w.Id.ToString(), $"{w.Id} - {w.Descripcion}")).ToList();
        }
    }
}
