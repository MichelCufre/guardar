using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
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

namespace WIS.Application.Controllers.STO
{
    public class STO720ConsultaDeContenidoDeLPN : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ITrafficOfficerService _concurrencyControl;


        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO720ConsultaDeContenidoDeLPN(
            ITrafficOfficerService concurrencyControl,
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_LPN", "ID_LPN_DET"
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_LPN", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._identity = identity;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;
            this._concurrencyControl = concurrencyControl;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridButton("btnAtributos", "STO700_grd1_btn_Atributos", "fas fa-list"),
                new GridButton("btnHistorial", "STO700_grd1_btn_Historial", "fas fa-clock"),
            }));

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_MOTIVO_AVERIA", this.InitializeSelectMotivoAveria()));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            long? numeroLpn = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnsActivos);

            if (long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "numeroLPN")?.Value, out long parsedValue))
            {
                var lpn = uow.ManejoLpnRepository.GetLpn(parsedValue);

                context.AddParameter("STO720_NU_LPN", lpn.NumeroLPN.ToString());
                context.AddParameter("STO720_TP_LPN_TIPO", lpn.Tipo);
                context.AddParameter("STO720_ID_LPN_EXTERNO", lpn.IdExterno);

                numeroLpn = parsedValue;
            }

            var dbQuery = new ConsultaDeContenidoLPNQuery(numeroLpn, lpnsActivos);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            var result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.STO720_Page_Access_PermiteEditarFecha,
                SecurityResources.STO720_Page_Access_PermiteMarcarAveria,
            });

            foreach (var row in grid.Rows)
            {
                long numLpn = long.Parse(row.GetCell("NU_LPN").Value);
                int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string ubicacion = row.GetCell("CD_ENDERECO").Value;
                string producto = row.GetCell("CD_PRODUTO").Value;
                string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                decimal faixa = Convert.ToDecimal(row.GetCell("CD_FAIXA").Value);
                int idLpnDet = int.Parse(row.GetCell("ID_LPN_DET").Value);
                string idAveria = row.GetCell("ID_AVERIA").Value;

                Producto prod = uow.ProductoRepository.GetProducto(empresa, producto);

                if (idAveria == "S")
                {
                    row.GetCell("ID_AVERIA").Editable = true;
                    row.GetCell("CD_MOTIVO_AVERIA").Editable = true;
                }
                else if (result[SecurityResources.STO720_Page_Access_PermiteMarcarAveria]
                    && Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value, this._identity.GetFormatProvider()) <= 0
                    && Convert.ToDecimal(row.GetCell("QT_ESTOQUE").Value, this._identity.GetFormatProvider()) > 0)
                {
                    decimal cantidadDisponibleLpn = uow.ManejoLpnRepository.GetCantidadStockDisponibleDetalleLpn(numLpn, ubicacion, empresa, producto, identificador, faixa, idLpnDet);
                    cantidadDisponibleLpn = cantidadDisponibleLpn + uow.ManejoLpnRepository.GetCantidNoDisponibleEnLpn(numLpn, empresa, producto, identificador, faixa, idLpnDet);

                    if (Convert.ToDecimal(row.GetCell("QT_ESTOQUE").Value, this._identity.GetFormatProvider()) == cantidadDisponibleLpn)
                    {
                        row.GetCell("ID_AVERIA").Editable = true;

                        if (prod.ManejaFechaVencimiento())
                            row.GetCell("DT_FABRICACAO").Editable = true;
                    }
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            long? numeroLpn = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnsActivos);

            if (long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "numeroLPN")?.Value, out long parsedValue))
                numeroLpn = parsedValue;

            var dbQuery = new ConsultaDeContenidoLPNQuery(numeroLpn, lpnsActivos);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            long? numeroLpn = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnsActivos);

            if (long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "numeroLPN")?.Value, out long parsedValue))
                numeroLpn = parsedValue;

            var dbQuery = new ConsultaDeContenidoLPNQuery(numeroLpn, lpnsActivos);
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

            try
            {
                switch (context.ButtonId)
                {
                    case "btnAtributos":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "detalle", Value = "true" },
                            new ComponentParameter(){ Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                        });
                        break;

                    case "btnHistorial":
                        context.Redirect("/stock/STO722", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "tipo", Value = context.Row.GetCell("DS_LPN_TIPO").Value },
                            new ComponentParameter(){ Id = "tipoLpn", Value = context.Row.GetCell("TP_LPN_TIPO").Value },
                            new ComponentParameter(){ Id = "numeroLpn", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "codigoEmpresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                            new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("NM_EMPRESA").Value },
                            new ComponentParameter(){ Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                        });
                        break;
                }
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction(uow.GetSnapshotIsolationLevel());
            var transactionTO = _concurrencyControl.CreateTransaccion();
            bool isOperacionParcial = false;

            try
            {
                var idsLocks = grid.Rows
                                .Select(s => $"{s.GetCell("CD_ENDERECO").Value}#{s.GetCell("CD_EMPRESA").Value}#{s.GetCell("CD_PRODUTO").Value}#{s.GetCell("CD_FAIXA").Value}#{s.GetCell("NU_IDENTIFICADOR").Value}")
                                .Distinct().ToList();

                _concurrencyControl.AddLockList("T_STOCK", idsLocks, transactionTO);

                foreach (var row in grid.Rows)
                {
                    long nuLpn = long.Parse(row.GetCell("NU_LPN").Value);
                    int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    string ubicacion = row.GetCell("CD_ENDERECO").Value;
                    string producto = row.GetCell("CD_PRODUTO").Value;
                    string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                    decimal faixa = Convert.ToDecimal(row.GetCell("CD_FAIXA").Value);
                    int idLpnDet = int.Parse(row.GetCell("ID_LPN_DET").Value);
                    decimal qtStock = decimal.Parse(row.GetCell("QT_ESTOQUE").Value, _identity.GetFormatProvider());
                    string idAveria = row.GetCell("ID_AVERIA").Value;
                    string idAveriaOld = row.GetCell("ID_AVERIA").Old;
                    string motivoAveria = row.GetCell("CD_MOTIVO_AVERIA").Value;

                    DateTime? vencimiento = null;
                    if (DateTime.TryParse(row.GetCell("DT_FABRICACAO").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fecha))
                    {
                        vencimiento = fecha.Date;
                    }

                    DateTime? vencimientoOld = null;
                    if (DateTime.TryParse(row.GetCell("DT_FABRICACAO").Old, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fecha1))
                    {
                        vencimientoOld = fecha1.Date;
                    }

                    bool isExpirable = uow.ProductoRepository.GetProducto(empresa, producto).IsFefo();

                    if ((idAveriaOld != idAveria && idAveria == "S") || (isExpirable && vencimiento < DateTime.Now && vencimientoOld > DateTime.Now))
                    {
                        decimal cantidadDisponibleLpn = uow.ManejoLpnRepository.GetCantidadStockDisponibleDetalleLpn(nuLpn, ubicacion, empresa, producto, identificador, faixa, idLpnDet);
                        cantidadDisponibleLpn = cantidadDisponibleLpn + uow.ManejoLpnRepository.GetCantidNoDisponibleEnLpn(nuLpn, empresa, producto, identificador, faixa, idLpnDet);
                        
                        if (qtStock != cantidadDisponibleLpn)
                        {
                            isOperacionParcial = true;
                            continue;
                        }
                    }

                    LpnDetalle detalle = uow.ManejoLpnRepository.GetDetalleEtiquetaLpn(nuLpn, idLpnDet, producto, empresa, identificador, faixa);
                    detalle.IdAveria = idAveria;
                    detalle.NumeroTransaccion = uow.GetTransactionNumber();
                    detalle.Vencimiento = vencimiento;

                    if (idAveria == "S")
                        detalle.MotivoAveria = motivoAveria;
                    else
                        detalle.MotivoAveria = null;

                    uow.ManejoLpnRepository.UpdateDetalleLpn(detalle);
                    uow.SaveChanges();

                }

                uow.Commit();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null
                  && uow.IsSnapshotException(ex.InnerException))
                {
                    return this.GridCommit(grid, context);
                }
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
            }
            finally
            {

                if (!isOperacionParcial)
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                else
                    context.AddInfoNotification("General_msg_Error_OperacionRealizadaParcial");

                _concurrencyControl.DeleteTransaccion(transactionTO);
                uow.EndTransaction();
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new STO720GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        #region Aux

        public virtual List<SelectOption> InitializeSelectMotivoAveria()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(IntegracionServicioDb.MOTIVO_AVERIA);

            foreach (var dominio in dominios)
                opciones.Add(new SelectOption(dominio.Id, dominio.Descripcion));

            return opciones;
        }

        #endregion
    }
}
