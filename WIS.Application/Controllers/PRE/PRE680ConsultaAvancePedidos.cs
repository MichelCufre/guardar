using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Preparacion;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
using WIS.Extension;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE680ConsultaAvancePedidos : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysAgenda { get; }
        protected List<string> GridKeysCrossDock { get; }

        public PRE680ConsultaAvancePedidos(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IGridExcelService excelService,
               IFilterInterpreter filterInterpreter)
        {
            this.GridKeysAgenda = new List<string>
            {
                "NU_PREDIO_NECESIDAD","CD_PRODUTO","CD_FAIXA","NU_IDENTIFICADOR","CD_EMPRESA"
            };

            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            SortCommand defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            FiltrosReabasteciminento filtros = JsonConvert.DeserializeObject<FiltrosReabasteciminento>(query.GetParameter("FILTROS"));

            var dbQuery = new PedidoReabastecimientoQueryObject(filtros);

            uow.HandleQuery(dbQuery);

            query.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;
            query.IsAddEnabled = false;
            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FiltrosReabasteciminento filtros = JsonConvert.DeserializeObject<FiltrosReabasteciminento>(query.GetParameter("FILTROS"));
            var dbQuery = new PedidoReabastecimientoQueryObject(filtros);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FiltrosReabasteciminento filtros = JsonConvert.DeserializeObject<FiltrosReabasteciminento>(query.GetParameter("FILTROS"));

            var dbQuery = new PedidoReabastecimientoQueryObject(filtros);

            uow.HandleQuery(dbQuery, false, false, false);

            List<SortCommand> defaultSort = new List<SortCommand>() {
                new SortCommand("CD_PRODUTO",SortDirection.Descending)
            };

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeysAgenda);

            grid.SetEditableCells(new List<string> {
                "QT_PEDIDOS_REAB_PRED1",
                "QT_PEDIDOS_REAB_PRED10",
                "QT_PEDIDOS_REAB_PRED2",
                "QT_PEDIDOS_REAB_PRED3",
                "QT_PEDIDOS_REAB_PRED4",
                "QT_PEDIDOS_REAB_PRED5",
                "QT_PEDIDOS_REAB_PRED6",
                "QT_PEDIDOS_REAB_PRED7",
                "QT_PEDIDOS_REAB_PRED8",
                "QT_PEDIDOS_REAB_PRED9"
            });

            if (string.IsNullOrEmpty(filtros.DT_DESDE) && string.IsNullOrEmpty(filtros.DT_HASTA))
                grid.Rows.Clear();

            if (!string.IsNullOrEmpty(filtros.NU_PREDIO_STOCK_DEFAULT))
            {
                grid.Rows.ForEach(w =>
                {

                    string NU_PREDIO_AUX1 = w.GetCell("NU_PREDIO_AUX1").Value;
                    string NU_PREDIO_AUX2 = w.GetCell("NU_PREDIO_AUX2").Value;
                    string NU_PREDIO_AUX3 = w.GetCell("NU_PREDIO_AUX3").Value;
                    string NU_PREDIO_AUX4 = w.GetCell("NU_PREDIO_AUX4").Value;
                    string NU_PREDIO_AUX5 = w.GetCell("NU_PREDIO_AUX5").Value;
                    string NU_PREDIO_AUX6 = w.GetCell("NU_PREDIO_AUX6").Value;
                    string NU_PREDIO_AUX7 = w.GetCell("NU_PREDIO_AUX7").Value;
                    string NU_PREDIO_AUX8 = w.GetCell("NU_PREDIO_AUX8").Value;
                    string NU_PREDIO_AUX9 = w.GetCell("NU_PREDIO_AUX9").Value;
                    string NU_PREDIO_AUX10 = w.GetCell("NU_PREDIO_AUX10").Value;
                    string QT_DISP1 = w.GetCell("QT_DISOPNIBLE_PRED1").Value;
                    string QT_DISP2 = w.GetCell("QT_DISOPNIBLE_PRED2").Value;
                    string QT_DISP3 = w.GetCell("QT_DISOPNIBLE_PRED3").Value;
                    string QT_DISP4 = w.GetCell("QT_DISOPNIBLE_PRED4").Value;
                    string QT_DISP5 = w.GetCell("QT_DISOPNIBLE_PRED5").Value;
                    string QT_DISP6 = w.GetCell("QT_DISOPNIBLE_PRED6").Value;
                    string QT_DISP7 = w.GetCell("QT_DISOPNIBLE_PRED7").Value;
                    string QT_DISP8 = w.GetCell("QT_DISOPNIBLE_PRED8").Value;
                    string QT_DISP9 = w.GetCell("QT_DISOPNIBLE_PRED9").Value;
                    string QT_DISP10 = w.GetCell("QT_DISOPNIBLE_PRED10").Value;

                    string QT_UND_BULTO = w.GetCell("QT_UND_BULTO").Value;
                    string QT_UNIDADES = w.GetCell("QT_UNIDADES").Value;

                    string Aproximar = filtros.cmbo_Redondiar;
                    string QT_NECESIDAD_FINAL = w.GetCell("QT_NECESIDAD_FINAL").Value;

                    if (NU_PREDIO_AUX1 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED1").Value = valorPorDefecto(QT_DISP1, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX2 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED2").Value = valorPorDefecto(QT_DISP2, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX3 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED3").Value = valorPorDefecto(QT_DISP3, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX4 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED4").Value = valorPorDefecto(QT_DISP4, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX5 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED5").Value = valorPorDefecto(QT_DISP5, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX6 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED6").Value = valorPorDefecto(QT_DISP6, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX7 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED7").Value = valorPorDefecto(QT_DISP7, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX8 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED8").Value = valorPorDefecto(QT_DISP8, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX9 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED9").Value = valorPorDefecto(QT_DISP9, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                    else if (NU_PREDIO_AUX10 == filtros.NU_PREDIO_STOCK_DEFAULT)
                    {
                        w.GetCell("QT_PEDIDOS_REAB_PRED10").Value = valorPorDefecto(QT_DISP10, Aproximar, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                    }
                });
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<GridRow> rows = grid.Rows.OrderBy(x => x.GetCell("CD_EMPRESA").Value).ToList();

            bool error = false;

            List<Pedido> pedidos = new List<Pedido>();

            uow.CreateTransactionNumber(this._identity.Application);

            foreach (var row in rows)
            {
                int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string producto = row.GetCell("CD_PRODUTO").Value;
                string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());

                string predioOrigen = row.GetCell("NU_PREDIO_NECESIDAD").Value;
                string QT_REAB1 = row.GetCell("QT_PEDIDOS_REAB_PRED1").Value;
                string QT_REAB2 = row.GetCell("QT_PEDIDOS_REAB_PRED2").Value;
                string QT_REAB3 = row.GetCell("QT_PEDIDOS_REAB_PRED3").Value;
                string QT_REAB4 = row.GetCell("QT_PEDIDOS_REAB_PRED4").Value;
                string QT_REAB5 = row.GetCell("QT_PEDIDOS_REAB_PRED5").Value;
                string QT_REAB6 = row.GetCell("QT_PEDIDOS_REAB_PRED6").Value;
                string QT_REAB7 = row.GetCell("QT_PEDIDOS_REAB_PRED7").Value;
                string QT_REAB8 = row.GetCell("QT_PEDIDOS_REAB_PRED8").Value;
                string QT_REAB9 = row.GetCell("QT_PEDIDOS_REAB_PRED9").Value;
                string QT_REAB10 = row.GetCell("QT_PEDIDOS_REAB_PRED10").Value;

                string QT_DISOPNIBLE_PRED1 = row.GetCell("QT_DISOPNIBLE_PRED1").Value;
                string QT_DISOPNIBLE_PRED2 = row.GetCell("QT_DISOPNIBLE_PRED2").Value;
                string QT_DISOPNIBLE_PRED3 = row.GetCell("QT_DISOPNIBLE_PRED3").Value;
                string QT_DISOPNIBLE_PRED4 = row.GetCell("QT_DISOPNIBLE_PRED4").Value;
                string QT_DISOPNIBLE_PRED5 = row.GetCell("QT_DISOPNIBLE_PRED5").Value;
                string QT_DISOPNIBLE_PRED6 = row.GetCell("QT_DISOPNIBLE_PRED6").Value;
                string QT_DISOPNIBLE_PRED7 = row.GetCell("QT_DISOPNIBLE_PRED7").Value;
                string QT_DISOPNIBLE_PRED8 = row.GetCell("QT_DISOPNIBLE_PRED8").Value;
                string QT_DISOPNIBLE_PRED9 = row.GetCell("QT_DISOPNIBLE_PRED9").Value;
                string QT_DISOPNIBLE_PRED10 = row.GetCell("QT_DISOPNIBLE_PRED10").Value;

                string NU_PREDIO_AUX1 = row.GetCell("NU_PREDIO_AUX1").Value;
                string NU_PREDIO_AUX2 = row.GetCell("NU_PREDIO_AUX2").Value;
                string NU_PREDIO_AUX3 = row.GetCell("NU_PREDIO_AUX3").Value;
                string NU_PREDIO_AUX4 = row.GetCell("NU_PREDIO_AUX4").Value;
                string NU_PREDIO_AUX5 = row.GetCell("NU_PREDIO_AUX5").Value;
                string NU_PREDIO_AUX6 = row.GetCell("NU_PREDIO_AUX6").Value;
                string NU_PREDIO_AUX7 = row.GetCell("NU_PREDIO_AUX7").Value;
                string NU_PREDIO_AUX8 = row.GetCell("NU_PREDIO_AUX8").Value;
                string NU_PREDIO_AUX9 = row.GetCell("NU_PREDIO_AUX9").Value;
                string NU_PREDIO_AUX10 = row.GetCell("NU_PREDIO_AUX10").Value;

                string CELDA = null;

                Agente agente = uow.AgenteRepository.GetAgente(empresa, predioOrigen, "DEP");

                try
                {
                    if (!string.IsNullOrEmpty(QT_REAB1) && decimal.Parse(QT_REAB1, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED1";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX1, empresa, pedidos, decimal.Parse(QT_REAB1, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED1, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB2) && decimal.Parse(QT_REAB2, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED2";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX2, empresa, pedidos, decimal.Parse(QT_REAB2, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED2, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB3) && decimal.Parse(QT_REAB3, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED3";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX3, empresa, pedidos, decimal.Parse(QT_REAB3, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED3, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB4) && decimal.Parse(QT_REAB4, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED4";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX4, empresa, pedidos, decimal.Parse(QT_REAB4, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED4, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB5) && decimal.Parse(QT_REAB5, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED5";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX5, empresa, pedidos, decimal.Parse(QT_REAB5, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED5, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB6) && decimal.Parse(QT_REAB6, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED6";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX6, empresa, pedidos, decimal.Parse(QT_REAB6, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED6, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB7) && decimal.Parse(QT_REAB7, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED7";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX7, empresa, pedidos, decimal.Parse(QT_REAB7, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED7, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB8) && decimal.Parse(QT_REAB8, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED8";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX8, empresa, pedidos, decimal.Parse(QT_REAB8, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED8, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB9) && decimal.Parse(QT_REAB9, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED9";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX9, empresa, pedidos, decimal.Parse(QT_REAB9, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED9, producto, identificador, faixa);
                    }
                    if (!string.IsNullOrEmpty(QT_REAB10) && decimal.Parse(QT_REAB10, _identity.GetFormatProvider()) > 0)
                    {
                        CELDA = "QT_PEDIDOS_REAB_PRED10";
                        Agregar_Pedido(uow, agente, NU_PREDIO_AUX10, empresa, pedidos, decimal.Parse(QT_REAB10, _identity.GetFormatProvider()), QT_DISOPNIBLE_PRED10, producto, identificador, faixa);
                    }
                }
                catch (ValidationFailedException ex)
                {
                    row.GetCell(CELDA).SetError(new ComponentError(ex.Message, null));
                    error = true;
                }
            }

            if (!error)
            {
                foreach (var ped in pedidos)
                {
                    uow.PedidoRepository.AddPedido(ped);

                    foreach (var linea in ped.Lineas)
                    {
                        linea.Transaccion = uow.GetTransactionNumber();
                        uow.PedidoRepository.AddDetallePedido(linea);
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Db_Success_Update");

                query.AddParameter("FILTROS", JsonConvert.SerializeObject(new FiltrosReabasteciminento()));
            }
            else
            {
                throw new ValidationFailedException("General_Db_Error_Update");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            return this._gridValidationService.Validate(new ConsultaAvancePedidosGridValidationModule(this._identity.GetFormatProvider()), grid, row, context);
        }
        public virtual string valorPorDefecto(string QT_DISP, string Aproximar, string QT_NECESIDAD_FINAL, string QT_UND_BULTO, string QT_UNIDADES)
        {
            string valor = "";
            if (!string.IsNullOrEmpty(QT_DISP) && QT_DISP.ToNumber<decimal>() > 0)
            {
                if (string.IsNullOrEmpty(Aproximar))
                {
                    if (QT_NECESIDAD_FINAL.ToNumber<decimal>() > QT_DISP.ToNumber<decimal>())
                    {
                        valor = QT_DISP;
                    }
                    else
                    {
                        valor = QT_NECESIDAD_FINAL;
                    }
                }
                else
                {
                    valor = aproximar(Aproximar, QT_DISP, QT_NECESIDAD_FINAL, QT_UND_BULTO, QT_UNIDADES);
                }
            }
            return valor;
        }
        public virtual string aproximar(string aproximar, string qT_DISP1, string qT_NECESIDAD_FINAL, string QT_UND_BULTO, string QT_UNIDADES)
        {
            string valor = "";
            if (aproximar.Equals("UNIDAD") && !string.IsNullOrEmpty(QT_UND_BULTO))
            {
                if (qT_NECESIDAD_FINAL.ToNumber<decimal>() > qT_DISP1.ToNumber<decimal>())
                {
                    decimal unidad = QT_UND_BULTO.ToNumber<decimal>();
                    decimal disp = qT_DISP1.ToNumber<decimal>();
                    if ((disp % unidad) == 0)
                    {
                        valor = disp.ToString();
                    }
                    else
                    {
                        decimal cantidad = disp / unidad;
                        decimal v = Math.Round(cantidad, 0);
                        valor = (v * unidad).ToString();
                    }

                }
                else
                {
                    decimal unidad = QT_UND_BULTO.ToNumber<decimal>();
                    decimal disp = qT_NECESIDAD_FINAL.ToNumber<decimal>();
                    if ((disp % unidad) == 0)
                    {
                        valor = disp.ToString();
                    }
                    else
                    {
                        decimal cantidad = disp / unidad;
                        decimal v = Math.Round(cantidad, 0);
                        valor = (v * unidad).ToString();
                    }
                }

            }
            else if (aproximar.Equals("PALLET") && !string.IsNullOrEmpty(QT_UNIDADES))
            {
                if (qT_NECESIDAD_FINAL.ToNumber<decimal>() > qT_DISP1.ToNumber<decimal>())
                {
                    decimal unidad = QT_UNIDADES.ToNumber<decimal>();
                    decimal disp = (qT_DISP1).ToNumber<decimal>();
                    if ((disp % unidad) == 0)
                    {
                        valor = disp.ToString();
                    }
                    else
                    {
                        decimal cantidad = disp / unidad;
                        decimal v = Math.Round(cantidad, 0);
                        valor = (v * unidad).ToString();
                    }

                }
                else
                {
                    decimal unidad = (QT_UNIDADES).ToNumber<decimal>();
                    decimal disp = (qT_NECESIDAD_FINAL).ToNumber<decimal>();
                    if ((disp % unidad) == 0)
                    {
                        valor = disp.ToString();
                    }
                    else
                    {
                        decimal cantidad = disp / unidad;
                        decimal v = Math.Round(cantidad, 0);
                        valor = (v * unidad).ToString();
                    }
                }
            }
            else
            {
                if ((qT_NECESIDAD_FINAL).ToNumber<decimal>() > (qT_DISP1).ToNumber<decimal>())
                {
                    valor = qT_DISP1;
                }
                else
                {
                    valor = qT_NECESIDAD_FINAL;
                }
            }
            return valor;


        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {

            var aux = form.GetField("cmbo_Redondiar");
            aux.Disabled = true;

            aux.Options = new List<SelectOption>
            {
                new SelectOption("UNIDAD", "WPRE680_Sec0_lbl_frm_Stock_OP1"),
                new SelectOption("PALLET", "WPRE680_Sec0_lbl_frm_Stock_OP2"),
            };

            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("NU_PREDIO").Options = this.SelectPredio(uow);
            form.GetField("NU_PREDIO_STOCK_DEFAULT").Options = this.SelectPredioDefault(uow);

            query.AddParameter("FILTROS", JsonConvert.SerializeObject(new { DT_DESDE = "", CD_EMPRESA = "", NU_PREDIO = "", DT_HASTA = "" }));

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            FiltrosReabasteciminento filtros = JsonConvert.DeserializeObject<FiltrosReabasteciminento>(query.GetParameter("FILTROS"));

            var dbQuery = new PedidoReabastecimientoQueryObject(filtros);


            //ejecutar pacage para llenar los datos de la tabla de T_RESERVA_STOCK
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.PreparacionRepository.ProcesarReabastecimientoPredio(filtros.DT_DESDE, filtros.DT_HASTA, filtros.CD_EMPRESA, filtros.NU_PREDIO_STOCK_DEFAULT, filtros.NU_PREDIO, "BASICA");
            uow.SaveChanges();
            //uow.HandleQuery(dbQuery, false, false, false);

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return this._formValidationService.Validate(new ConsultaAvancePedidosFormValidationModule(), form, context);
        }
        public virtual List<SelectOption> SearchEmpresa(string searchValue, string userId)
        {

            int user = string.IsNullOrEmpty(userId) ? _identity.UserId : int.Parse(userId);

            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(searchValue, user)
                    .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                    .ToList();
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "CD_EMPRESA": return this.SearchEmpresa(query.SearchValue, null);

                default:
                    return new List<SelectOption>();
            }
        }
        public virtual List<SelectOption> SelectPredio(IUnitOfWork uow)
        {
            List<string> cont = new List<string>() {
                GeneralDb.PredioSinDefinir,"MT"};
            List<Predio> predio = uow.PredioRepository.GetPredios();
            List<SelectOption> list = predio.Where(x => !cont.Contains(x.Numero)).Select(w => new SelectOption(w.Numero, w.Descripcion))
                  .ToList();
            return list;
        }
        public virtual List<SelectOption> SelectPredioDefault(IUnitOfWork uow)
        {
            List<string> cont = new List<string>() {
                GeneralDb.PredioSinDefinir,"MT"};
            List<Predio> predio = uow.PredioRepository.GetPredios();
            List<SelectOption> list = predio.Where(x => !cont.Contains(x.Numero)).Select(w => new SelectOption(w.Numero, w.Descripcion))
                  .ToList();
            return list;
        }


        public virtual void Agregar_Pedido(IUnitOfWork uow, Agente agente, string predio, int empresa, List<Pedido> pedidos, decimal QT_REAB, string QT_DISP, string producto, string identificador, decimal faixa)
        {
            if (string.IsNullOrEmpty(QT_DISP))
                throw new ValidationFailedException("PRE680_Sec0_Error_Er001_SinStock");

            decimal stock = decimal.Parse(QT_DISP, _identity.GetFormatProvider());

            if (QT_REAB > stock)
                throw new ValidationFailedException("PRE680_Sec0_Error_Er002_QTReabTieneMenorigualStock");

            Pedido pedido = pedidos.FirstOrDefault(x => x.Empresa == empresa && x.Predio == predio);

            ConfiguracionExpedicionPedido configuracion = uow.PedidoRepository.GetConfiguracionExpedicion(TipoExpedicion.NoFacturables);

            var nuTransaccion = uow.GetTransactionNumber();

            if (pedido == null)
            {
                Pedido nuevoPedido = new Pedido
                {
                    Empresa = empresa,
                    Cliente = agente.CodigoInterno,
                    //pedido.Agrupacion = "P";
                    //pedido.CodigoTransportadora = 1;
                    FechaAlta = DateTime.Now,
                    Predio = predio,
                    Ruta = agente.Ruta.Id,
                    ConfiguracionExpedicion = configuracion,
                    //pedido.ConfiguracionExpedicion 
                    CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                    IsManual = true,
                    Estado = SituacionDb.PedidoAbierto,
                    Tipo = TipoPedidoDb.Reabastecimiento,
                    NuCarga = null,
                    Transaccion = nuTransaccion,
                };

                pedidos.Add(nuevoPedido);

                DetallePedido detalle = new DetallePedido
                {
                    Identificador = identificador,
                    Faixa = faixa,
                    Agrupacion = Agrupacion.Pedido,
                    Producto = producto,
                    CantidadOriginal = QT_REAB,
                    Cantidad = QT_REAB,
                    CantidadAnulada = 0,
                    CantidadLiberada = 0,
                    EspecificaIdentificador = !identificador.Equals(ManejoIdentificadorDb.IdentificadorAuto)
                };

                nuevoPedido.Lineas.Add(detalle);

                return;
            }

            DetallePedido nuevoDetalle = new DetallePedido
            {
                Identificador = identificador,
                Faixa = faixa,
                Agrupacion = Agrupacion.Pedido,
                Producto = producto,
                CantidadOriginal = QT_REAB,
                Cantidad = QT_REAB,
                CantidadAnulada = 0,
                CantidadLiberada = 0
            };

            if (identificador.Equals(ManejoIdentificadorDb.IdentificadorAuto))
            {
                if (pedido.Lineas.Any(d => d.Producto == producto && d.Identificador == identificador && d.Faixa == faixa))
                {
                    if (!pedidos.Any(x => x.Empresa == empresa && x.Predio == predio && (x.Id != pedido.Id || x.Cliente != pedido.Cliente)))
                    {
                        Pedido nuevoPedido = new Pedido
                        {
                            Empresa = empresa,
                            Cliente = agente.CodigoInterno,
                            //Agrupacion = "P",
                            //CodigoTransportadora = 1,
                            FechaAlta = DateTime.Now,
                            Predio = predio,
                            Ruta = agente.Ruta.Id,
                            ConfiguracionExpedicion = configuracion,
                            CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                            IsManual = true,
                            Estado = SituacionDb.PedidoAbierto,
                            Tipo = TipoPedidoDb.Reabastecimiento,
                            NuCarga = null,
                            Transaccion = nuTransaccion,
                        };

                        pedidos.Add(nuevoPedido);

                        nuevoPedido.Lineas.Add(nuevoDetalle);
                    }
                    else
                    {
                        Pedido pedidoExistente = pedidos.FirstOrDefault(x => x.Empresa == empresa && x.Predio == predio && (x.Id != pedido.Id || x.Cliente != pedido.Cliente));

                        pedidoExistente.Lineas.Add(nuevoDetalle);
                    }
                }

                nuevoDetalle.EspecificaIdentificador = false;
            }
            else
            {
                if (pedido.Lineas.Any(d => d.Producto == producto && d.Identificador == ManejoIdentificadorDb.IdentificadorAuto && d.Faixa == faixa))
                {
                    if (!pedidos.Any(x => x.Empresa == empresa && x.Predio == predio && (x.Id != pedido.Id || x.Cliente != pedido.Cliente)))
                    {
                        Pedido nuevoPedido = new Pedido
                        {
                            Empresa = empresa,
                            Cliente = agente.CodigoInterno,
                            //Agrupacion = "P",
                            //CodigoTransportadora = 1,
                            FechaAlta = DateTime.Now,
                            Predio = predio,
                            Ruta = agente.Ruta.Id,
                            ConfiguracionExpedicion = configuracion,
                            //Tipo = "REA",
                            CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                            IsManual = true,
                            Estado = SituacionDb.PedidoAbierto,
                            Tipo=TipoPedidoDb.Reabastecimiento,
                            NuCarga = null,
                            Transaccion = nuTransaccion,
                        };

                        pedidos.Add(nuevoPedido);

                        nuevoPedido.Lineas.Add(nuevoDetalle);
                    }
                    else
                    {
                        Pedido pedidoExistente = pedidos.FirstOrDefault(x => x.Empresa == empresa && x.Predio == predio && (x.Id != pedido.Id || x.Cliente != pedido.Cliente));

                        pedidoExistente.Lineas.Add(nuevoDetalle);
                    }
                }
                nuevoDetalle.EspecificaIdentificador = true;
            }
        }
    }
}
