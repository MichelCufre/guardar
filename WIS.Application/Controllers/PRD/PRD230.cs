using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.ManejoStock.SalidaBlackBox;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD230 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFormatProvider _culture;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRD230(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._culture = identity.GetFormatProvider();
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "CD_PRODUTO","NU_IDENTIFICADOR","CD_EMPRESA","CD_ENDERECO"
            };
        }

        public override PageContext PageLoad(PageContext data)
        {
            string nroIngreso = this._session.GetValue<string>("PRD230_INGRESO_PROD");

            if (!string.IsNullOrEmpty(nroIngreso))
            {
                data.AddParameter("Ingreso", nroIngreso);
            }

            this._session.SetValue("PRD230_INGRESO_PROD", null);

            return data;
        }
        
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            FormField selectTpSlida = form.GetField("tpSalida");
            selectTpSlida.Options = new List<SelectOption>();

            selectTpSlida.Options.Add(new SelectOption(TipoStockOutBB.INSUMO.ToString(), "PRD230_frm1_lbl_TP_INSUMO"));
            selectTpSlida.Options.Add(new SelectOption(TipoStockOutBB.PRODUCTO.ToString(), "PRD230_frm1_lbl_TP_PRODUCTO"));

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return this._formValidationService.Validate(new PRD230FormValidationModule(), form, context);
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsEditingEnabled = true;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string ubicacionBB = this.ObtenerUbicacionBlackBox(uow, context.GetParameter("PRD_INGRESO"));

                var dbQuery = new StockSalidaBlackBoxPRD230Query(ObtenerTipoStockSalida(context.Parameters), ubicacionBB);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_UPDROW", Sorting.SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                this.FormatGrid1(grid, uow);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string ubicacionBB = this.ObtenerUbicacionBlackBox(uow, context.GetParameter("PRD_INGRESO"));

                var dbQuery = new StockSalidaBlackBoxPRD230Query(ObtenerTipoStockSalida(context.Parameters), ubicacionBB);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_UPDROW", Sorting.SortDirection.Descending);

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD230GridValidationModule(uow, this._culture), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    if (grid.Rows.Any())
                    {
                        if (grid.HasNewDuplicates(this.GridKeys))
                            throw new ValidationFailedException("PRD230_Sec0_Error_Error01");

                        uow.CreateTransactionNumber(this._identity.Application);

                        foreach (var row in grid.Rows)
                        {
                            var cdProducto = row.GetCell("CD_PRODUTO").Value;
                            var nroIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
                            var empresa = Convert.ToInt32(row.GetCell("CD_EMPRESA").Value);
                            var ubicacion = row.GetCell("CD_ENDERECO").Value;
                            var faixa = Convert.ToInt32(row.GetCell("CD_FAIXA").Value);
                            var cantidadDisponible = Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value, this._identity.GetFormatProvider());
                            var cantidadMovimientoBB = Convert.ToDecimal(row.GetCell("QT_MOVIMIENTO_BB").Value, this._identity.GetFormatProvider());
                            var cantidadRechazoAveria = Convert.ToDecimal(row.GetCell("QT_RECHAZO_AVERIA").Value, this._identity.GetFormatProvider());

                            this.UpdateStock(uow, ubicacion, cdProducto, nroIdentificador, empresa, faixa, cantidadDisponible, cantidadMovimientoBB, cantidadRechazoAveria, context.Parameters, context.GetParameter("PRD_INGRESO"));
                        }

                        context.AddSuccessNotification("PRD230_Sec0_Error_Error02");
                    }

                    uow.SaveChanges();
                }
                catch (ValidationFailedException ex)
                {
                    this._logger.Error(ex, ex.Message);
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    context.AddErrorNotification("PRD230_Sec0_Error_Error03");
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string ubicacionBB = this.ObtenerUbicacionBlackBox(uow, query.GetParameter("PRD_INGRESO"));

            var dbQuery = new StockSalidaBlackBoxPRD230Query(ObtenerTipoStockSalida(query.Parameters), ubicacionBB);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        
        #region Metodos Auxiliares

        public virtual void UpdateStock(IUnitOfWork uow, string ubicacion, string producto, string nroIdentificador, int empresa, int faixa, decimal? cantidadDisponible, decimal? cantidadMovimientoBB, decimal? cantidadRechazoAveria, List<ComponentParameter> parametros, string nroIngreso)
        {
            var salidaBlackBox = new SalidaBlackBox(this._uowFactory);
            var nuTransaccion = uow.GetTransactionNumber();

            //Obtener stock
            var stock = uow.StockRepository.GetStock(empresa, producto, faixa, ubicacion, nroIdentificador);

            if (stock == null)
                throw new ValidationFailedException("General_Sec0_Error_Error72");

            //Mover stock a salida BlackBox
            if (cantidadMovimientoBB > 0 || cantidadRechazoAveria > 0)
            {
                salidaBlackBox.SalidaStockBlackBox(uow, stock, cantidadMovimientoBB, cantidadRechazoAveria, ObtenerTipoStockSalida(parametros), nroIngreso, this._identity.UserId);
            }

            stock.NumeroTransaccion = nuTransaccion;

            uow.StockRepository.UpdateStock(stock);
            uow.SaveChanges();
        }

        public virtual void FormatGrid1(Grid grid, IUnitOfWork uow)
        {
            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>() { "QT_MOVIMIENTO_BB", "QT_RECHAZO_AVERIA" });
            }
        }

        public static TipoStockOutBB ObtenerTipoStockSalida(List<ComponentParameter> parametros)
        {
            if (parametros != null && parametros.Any(p => p.Id == "TP_STOCK"))
            {
                var tipoStock = parametros.FirstOrDefault(p => p.Id == "TP_STOCK").Value; ;

                if (Enum.IsDefined(typeof(TipoStockOutBB), tipoStock))
                    return (TipoStockOutBB)Enum.Parse(typeof(TipoStockOutBB), tipoStock);
            }

            return TipoStockOutBB.Unknown;
        }

        public virtual string ObtenerUbicacionBlackBox(IUnitOfWork uow, string ingresoProd)
        {
            if (!string.IsNullOrEmpty(ingresoProd))
            {
                var ingreso = uow.ProduccionRepository.GetIngresoBlackBox(ingresoProd);
                return ((LineaBlackBox)ingreso.Linea).UbicacionBlackBox;
            }
            else
            {
                return "";
            }
        }

        #endregion
    }
}
