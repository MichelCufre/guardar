using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.ManejoStock.EntradaBlackBox;
using WIS.Exceptions;
using WIS.Filtering;
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
    public class PRD200 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormatProvider _culture;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRD200(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._gridValidationService = gridValidationService;
            this._culture = identity.GetFormatProvider();
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "CD_PRODUTO","NU_IDENTIFICADOR","CD_EMPRESA","CD_ENDERECO"
            };
        }

        public override PageContext PageLoad(PageContext data)
        {
            string nroIngreso = this._session.GetValue<string>("PRD200_INGRESO_PROD");

            if (!string.IsNullOrEmpty(nroIngreso))
            {
                data.AddParameter("Ingreso", nroIngreso);
            }

            this._session.SetValue("PRD200_INGRESO_PROD", null);

            return data;
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
                string ubicacionEntradaBB = this.ObtenerUbicacionEntrada(uow, context.GetParameter("PRD_INGRESO"));
                var dbQuery = new StockEntradaBlackBoxPRD200Query(ubicacionEntradaBB);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_UPDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                this.FormatGrid1(grid, uow);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnDetalleActa":
                    break;

                case "btnDetalleDocAfectado":
                    break;
            }

            context.Redirect("/documento/DOC096", new List<ComponentParameter>() { });

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string ubicacionEntradaBB = this.ObtenerUbicacionEntrada(uow, context.GetParameter("PRD_INGRESO"));

                var dbQuery = new StockEntradaBlackBoxPRD200Query(ubicacionEntradaBB);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_UPDROW", SortDirection.Descending);

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD200GridValidationModule(uow, this._culture), grid, row, context);
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
                            throw new ValidationFailedException("PRD200_Sec0_Error_Error01");

                        uow.CreateTransactionNumber("PRD200");

                        foreach (var row in grid.Rows)
                        {
                            string cdProducto = row.GetCell("CD_PRODUTO").Value;
                            string nroIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
                            int empresa = Convert.ToInt32(row.GetCell("CD_EMPRESA").Value);
                            string ubicacion = row.GetCell("CD_ENDERECO").Value;
                            int faixa = Convert.ToInt32(row.GetCell("CD_FAIXA").Value);

                            decimal? cantidadDisponible = Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value, this._identity.GetFormatProvider());
                            decimal? cantidadMovimientoBB = Convert.ToDecimal(row.GetCell("QT_MOVIMIENTO_BB").Value, this._identity.GetFormatProvider());
                            decimal? cantidadRechazoSano = Convert.ToDecimal(row.GetCell("QT_RECHAZO_SANO").Value, this._identity.GetFormatProvider());
                            decimal? cantidadRechazoAveria = Convert.ToDecimal(row.GetCell("QT_RECHAZO_AVERIA").Value, this._identity.GetFormatProvider());

                            this.UpdateStock(uow, ubicacion, cdProducto, nroIdentificador, empresa, faixa, cantidadDisponible, cantidadMovimientoBB, cantidadRechazoSano, cantidadRechazoAveria, context.GetParameter("PRD_INGRESO"));
                        }

                        context.AddSuccessNotification("PRD200_Sec0_Error_Error02");
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
                    context.AddErrorNotification("PRD200_Sec0_Error_Error03");
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string ubicacionEntradaBB = this.ObtenerUbicacionEntrada(uow, query.GetParameter("PRD_INGRESO"));
            var dbQuery = new StockEntradaBlackBoxPRD200Query(ubicacionEntradaBB);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual void UpdateStock(IUnitOfWork uow, string ubicacion, string producto, string nroIdentificador, int empresa, int faixa, decimal? cantidadDisponible, decimal? cantidadMovimientoBB, decimal? cantidadRechazoSano, decimal? cantidadRechazoAveria, string nroIngreso)
        {
            try
            {
                var entradaBlackBox = new EntradaBlackBox(_uowFactory);
                var nuTransaccion = uow.GetTransactionNumber();

                //Obtener stock
                var stock = uow.StockRepository.GetStock(empresa, producto, faixa, ubicacion, nroIdentificador);

                if (stock == null)
                    throw new ValidationFailedException("General_Sec0_Error_Error72");

                //Rechazar stock
                if (cantidadRechazoSano > 0 || cantidadRechazoAveria > 0)
                {
                    entradaBlackBox.RechazarStockEntradaBlackBox(uow, stock, cantidadRechazoSano, cantidadRechazoAveria, nroIngreso, this._identity.UserId);
                }

                //Mover stock a BlackBox
                if (cantidadMovimientoBB > 0)
                {
                    entradaBlackBox.IngresarStockBlackBox(uow, stock, cantidadMovimientoBB, nroIngreso, this._identity.UserId);
                }

                uow.SaveChanges();

                stock.NumeroTransaccion = nuTransaccion;

                uow.StockRepository.UpdateStock(stock);
                uow.SaveChanges();
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
            }
        }

        public virtual string ObtenerUbicacionEntrada(IUnitOfWork uow, string ingresoProd)
        {
            if (!string.IsNullOrEmpty(ingresoProd))
            {
                var ingreso = uow.ProduccionRepository.GetIngresoBlackBox(ingresoProd);
                return ingreso.Linea.UbicacionEntrada;
            }
            else
            {
                return "";
            }
        }

        public virtual void FormatGrid1(Grid grid, IUnitOfWork uow)
        {
            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>() { "QT_MOVIMIENTO_BB", "QT_RECHAZO_SANO", "QT_RECHAZO_AVERIA" });
            }
        }

        #endregion

    }
}