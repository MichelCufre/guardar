using Irony.Parsing;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
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
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.STO
{
    public class STO152ControlCalidadStock : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogger<STO152ControlCalidadStock> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO152ControlCalidadStock(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            ILogger<STO152ControlCalidadStock> logger)
        {
            this.GridKeys = new List<string>
            {
                "CD_ENDERECO",
                "CD_EMPRESA",
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR"
            };

            this.Sorts = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlCalidadEnStockQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);
            foreach (var row in grid.Rows)
            {
                string endereco = row.GetCell("CD_ENDERECO").Value;
                int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string producto = row.GetCell("CD_PRODUTO").Value;
                decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, this._identity.GetFormatProvider());
                string nuIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
                var stock = Convert.ToDecimal(row.GetCell("QT_ESTOQUE").Value);
                var reserva = Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value);

                decimal cantidadStockLpn = uow.ManejoLpnRepository.GetStockLpnUbicacion(endereco, empresa, producto, nuIdentificador, faixa, out decimal cantidadReservaLpn, out decimal cantidadReservaAtributo);

                if (stock < cantidadStockLpn
                    && (reserva - cantidadReservaLpn - cantidadReservaAtributo) > 0)
                    row.DisabledSelected = true;
            }
            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlCalidadEnStockQuery();
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

            var dbQuery = new ControlCalidadEnStockQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("STO152 Marcar Control calidad stock");
            var transactionTO = _concurrencyControl.CreateTransaccion();

            if (context.ButtonId == "BtnConfirmar")
            {
                try
                {
                    var operacionParcial = false;
                    var keysRowSelected = GetSelectedKeys(uow, context);
                    var cdControl = int.Parse(context.GetParameter("CD_CONTROL"));
                    bool updateRegistro = false;
                    var stocks = uow.StockRepository.GetStocksPredio(keysRowSelected);
                    BloquearStock(stocks, transactionTO);
                    foreach (var sto in stocks)
                    {

                        decimal cantidadStockLpn = uow.ManejoLpnRepository.GetStockLpnUbicacion(sto.Ubicacion, sto.Empresa, sto.Producto, sto.Identificador, sto.Faixa, out decimal cantidadReservaLpn, out decimal cantidadReservaAtributo);

                        if (sto.Cantidad < cantidadStockLpn && (sto.ReservaSalida - cantidadReservaLpn - cantidadReservaAtributo) > 0)
                        {
                            operacionParcial = true;
                            continue;
                        }

                        if (!uow.ControlDeCalidadRepository.AnyControlPendiente(sto))
                        {
                            var controlPendiente = new ControlDeCalidadPendiente()
                            {
                                Codigo = cdControl,
                                Stock = sto,
                                Aceptado = false,
                                Predio = sto.Predio,
                                FechaAlta = DateTime.Now,
                                FuncionarioAceptacion = null,
                            };

                            uow.ControlDeCalidadRepository.AddControlDeCalidadPendiente(controlPendiente);
                        }

                        sto.NumeroTransaccion = uow.GetTransactionNumber();
                        sto.ControlCalidad = EstadoControlCalidad.Pendiente;
                        sto.FechaModificacion = DateTime.Now;

                        uow.StockRepository.UpdateStock(sto);
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
                    _logger.LogError($"STO152 -GridMenuItemAction Error: {ex}");
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                    uow.Rollback();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"STO152 -GridMenuItemAction Error: {ex}");
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

        public virtual List<Stock> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new ControlCalidadEnStockQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keys = dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());

            if (context.Selection.AllSelected)
                keys = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return keys.Select(k => new Stock()
            {
                Ubicacion = k[0],
                Empresa = int.Parse(k[1]),
                Producto = k[2],
                Faixa = decimal.Parse(k[3], _identity.GetFormatProvider()),
                Identificador = k[4]
            }).ToList();
        }
        public virtual void BloquearStock(IEnumerable<Stock> stocks, TrafficOfficerTransaction transactionTO)
        {
            var idsLocks = stocks.Select(s => s.GetLockId(_identity.GetFormatProvider())).ToList();
            _concurrencyControl.AddLockList("T_STOCK", idsLocks, transactionTO);
        }

        #endregion
    }
}
