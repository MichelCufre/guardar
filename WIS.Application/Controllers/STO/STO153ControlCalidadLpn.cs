using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
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

namespace WIS.Application.Controllers.STO
{
    public class STO153ControlCalidadLpn : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<STO153ControlCalidadLpn> _logger;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO153ControlCalidadLpn(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<STO153ControlCalidadLpn> logger,
            ITrafficOfficerService concurrencyControl)
        {
            this.GridKeys = new List<string>
            {
                "NU_LPN", "ID_LPN_DET"
            };

            this.Sorts = new List<SortCommand>
            {
                new SortCommand("NU_LPN", SortDirection.Descending),
                new SortCommand("ID_LPN_DET", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._concurrencyControl = concurrencyControl;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlCalidadEnLpnQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var qtStockLpn = Convert.ToDecimal(row.GetCell("QT_ESTOQUE").Value, _identity.GetFormatProvider());
                var qtReservadaLpn = Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value, _identity.GetFormatProvider());

                var enableSelected = true;
                if (qtReservadaLpn > 0 || qtStockLpn <= 0)
                    enableSelected = false;
                else
                {
                    var nuLpn = long.Parse(row.GetCell("NU_LPN").Value);
                    var idLpnDet = int.Parse(row.GetCell("ID_LPN_DET").Value);
                    var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    var ubicacion = row.GetCell("CD_ENDERECO").Value;
                    var producto = row.GetCell("CD_PRODUTO").Value;
                    var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                    var faixa = Convert.ToDecimal(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());

                    var cantidadDisponibleLpn = uow.ManejoLpnRepository.GetCantidadStockDisponibleDetalleLpn(nuLpn, ubicacion, empresa, producto, identificador, faixa, idLpnDet);
                    cantidadDisponibleLpn = cantidadDisponibleLpn + uow.ManejoLpnRepository.GetCantidNoDisponibleEnLpn(nuLpn, empresa, producto, identificador, faixa, idLpnDet);

                    if (qtStockLpn != cantidadDisponibleLpn)
                        enableSelected = false;
                }

                row.DisabledSelected = !enableSelected;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlCalidadEnLpnQuery();
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

            var dbQuery = new ControlCalidadEnLpnQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            uow.CreateTransactionNumber("STO153 - GridMenuItemAction - Marcar control de calidad de LPN");
            var transactionTO = _concurrencyControl.CreateTransaccion();

            if (context.ButtonId == "BtnConfirmar")
            {
                try
                {
                    var operacionParcial = false;
                    var keysRowSelected = GetSelectedKeys(uow, context);
                    var cdControl = int.Parse(context.GetParameter("CD_CONTROL"));

                    var detallesLpn = uow.ManejoLpnRepository.GetDetallesLpnUbicacion(keysRowSelected);
                    BloquearStock(detallesLpn, transactionTO);
                    bool updateRegistro = false;
                    foreach (var detalle in detallesLpn)
                    {
                        var cantidadDisponibleLpn = uow.ManejoLpnRepository.GetCantidadStockDisponibleDetalleLpn(detalle.NumeroLPN, detalle.Ubicacion, detalle.Empresa, detalle.CodigoProducto, detalle.Lote, detalle.Faixa, detalle.Id);
                        cantidadDisponibleLpn = cantidadDisponibleLpn + uow.ManejoLpnRepository.GetCantidNoDisponibleEnLpn(detalle.NumeroLPN, detalle.Empresa, detalle.CodigoProducto, detalle.Lote, detalle.Faixa, detalle.Id);

                        if (detalle.Cantidad != cantidadDisponibleLpn)
                        {
                            operacionParcial = true;
                            continue;
                        }

                        if (!uow.ControlDeCalidadRepository.AnyControlDeCalidadPendienteDetalleLpn(detalle.NumeroLPN, detalle.Id))
                        {
                            var controlPendiente = new ControlDeCalidadPendiente()
                            {
                                Codigo = cdControl,
                                Ubicacion = detalle.Ubicacion,
                                Empresa = detalle.Empresa,
                                Producto = detalle.CodigoProducto,
                                Faixa = detalle.Faixa,
                                Identificador = detalle.Lote,
                                Predio = detalle.Predio,
                                NroLPN = detalle.NumeroLPN,
                                IdLpnDet = detalle.Id,
                                Aceptado = false,
                                FechaAlta = DateTime.Now,
                                FuncionarioAceptacion = null
                            };

                            uow.ControlDeCalidadRepository.AddControlDeCalidadPendiente(controlPendiente);
                        }

                        detalle.IdCtrlCalidad = EstadoControlCalidad.Pendiente;
                        detalle.NumeroTransaccion = uow.GetTransactionNumber();

                        uow.ManejoLpnRepository.UpdateDetalleLpn(detalle);
                        uow.SaveChanges();
                        updateRegistro = true;
                    }

                    uow.SaveChanges();
                    uow.Commit();

                    if (operacionParcial && updateRegistro)
                        context.AddSuccessNotification("General_msg_Error_OperacionRealizadaParcial");
                    else
                        context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
                catch (ValidationFailedException ex)
                {
                    _logger.LogError($"STO153 -GridMenuItemAction Error: {ex}");
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                    uow.Rollback();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"STO153 -GridMenuItemAction Error: {ex}");
                    context.AddErrorNotification("General_Sec0_Error_ErrorGuardarCambios");
                    uow.Rollback();
                }
                finally
                {
                    _concurrencyControl.DeleteTransaccion(transactionTO);
                    uow.EndTransaction();
                }
            }
            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            InicializarSelects(form, context);
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            context.AddParameter("FORM_CD_CONTROL", form.GetField("CD_CONTROL").Value);
            return form;
        }

        #region Metodos auxiliares
        public virtual void InicializarSelects(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selectControl = form.GetField("CD_CONTROL");
            selectControl.Options = new List<SelectOption>();

            var controles = uow.ControlDeCalidadRepository.GetTiposControlCalidad();

            foreach (var control in controles)
            {
                selectControl.Options.Add(new SelectOption(control.Id.ToString(), $"{control.Id} - {control.Descripcion}")); ;
            }

        }

        public virtual List<LpnDetalle> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new ControlCalidadEnLpnQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keys = dbQuery.GetSelectedKeys(context.Selection.Keys);

            if (context.Selection.AllSelected)
                keys = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return keys.Select(k => new LpnDetalle()
            {
                NumeroLPN = long.Parse(k[0]),
                Id = int.Parse(k[1]),
            }).ToList();
        }

        public virtual void BloquearStock(IEnumerable<LpnDetalle> dets, TrafficOfficerTransaction transactionTO)
        {
            var idsLocks = dets
                .Select(s => $"{s.Ubicacion}#{s.Empresa}#{s.CodigoProducto}#{s.Faixa.ToString("#.##")}#{s.Lote}")
                .Distinct()
                .ToList();

            _concurrencyControl.AddLockList("T_STOCK", idsLocks, transactionTO);
        }
        #endregion
    }
}
