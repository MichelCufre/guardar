using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Components.Common;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Documento.Integracion.Produccion;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.ManejoStock.SalidaBlackBox;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Interfaces.Entrada;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD260 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRD260(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFactoryService factoryService,
            IParameterService parameterService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "NU_INTERFAZ_EJECUCION", "NU_REGISTRO", "TIPO"
            };
        }

        
        public override PageContext PageLoad(PageContext data)
        {
            try
            {
                string ejecucionStr = data.GetParameter("nuEjecucion");
                long nuEjecucion = long.Parse(ejecucionStr);

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var ejecucion = uow.EjecucionRepository.GetEjecucion(nuEjecucion).GetAwaiter().GetResult();
                    var mapper = new InterfazMapper();

                    if (ejecucion != null)
                    {
                        data.AddParameter("NU_EJECUCION", ejecucion.Id.ToString());
                        data.AddParameter("CD_SITUACION", ejecucion.Situacion.ToString());
                        data.AddParameter("DS_REFERENCIA", ejecucion.Referencia);
                        data.AddParameter("NM_EMPRESA", uow.EmpresaRepository.GetNombre(ejecucion.Empresa ?? -1));
                        data.AddParameter("DT_COMIENZO", ejecucion.Comienzo.ToIsoString());
                    }
                    else
                    {
                        data.AddParameter("NU_EJECUCION", "");
                        data.AddParameter("CD_SITUACION", "");
                        data.AddParameter("DS_REFERENCIA", "");
                        data.AddParameter("NM_EMPRESA", "");
                        data.AddParameter("DT_COMIENZO", "");
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                data.AddErrorNotification("General_Sec0_Error_Error45");
            }

            return data;
        }
        

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string ejecucionStr = context.GetParameter("nuEjecucion");
                long ejecucion = -1;
                long.TryParse(ejecucionStr, out ejecucion);

                var dbQuery = new DetalleInterfacesSalidaBBPRD260Query(ejecucion);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_REGISTRO", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalles")
            {
                context.Redirect("/produccion/PRD260", new List<ComponentParameter>()
                {
                    new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_INTERFAZ_EJECUCION").Value },
                });
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string ejecucionStr = context.GetParameter("nuEjecucion");
                long ejecucion = -1;
                long.TryParse(ejecucionStr, out ejecucion);

                var dbQuery = new DetalleInterfacesSalidaBBPRD260Query(ejecucion);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_REGISTRO", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string ejecucionStr = query.GetParameter("nuEjecucion");
            long ejecucion = -1;
            long.TryParse(ejecucionStr, out ejecucion);

            var dbQuery = new DetalleInterfacesSalidaBBPRD260Query(ejecucion);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "btnAprobar":
                    this.AprobarInterfazSalidaBB(form, context);
                    break;
                case "btnRechazar":
                    this.RechazarInterfazSalidaBB(form, context);
                    break;
            }

            return form;
        }


        #region Metodos Auxiliares

        public virtual void RechazarInterfazSalidaBB(Form form, FormSubmitContext context)
        {
            string motivoRechazo = form.GetField("dsMotivo").Value;

            try
            {
                if (string.IsNullOrEmpty(motivoRechazo))
                {
                    throw new ValidationFailedException("PRD260_form1_error_MotivoRechazoObligatorio");
                }

                if (motivoRechazo.Length > 400)
                {
                    throw new ValidationFailedException("PRD260_form1_error_LargoMaximoMotivoRechazo", new string[] { "400" });
                }

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    string ejecucionStr = context.GetParameter("nuEjecucion");

                    long nroEjecucion = -1;
                    long.TryParse(ejecucionStr, out nroEjecucion);

                    InterfazProductoProducidoBB interfaz = new InterfazProductoProducidoBB(uow);

                    interfaz.RechazarInterfazProductoProducido(nroEjecucion, motivoRechazo);

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD260_form1_success_InterfazRechazada");
                    context.Redirect("/produccion/PRD250", new List<ComponentParameter>() { });
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual void AprobarInterfazSalidaBB(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber("PRD260 Aprobar Salida BB");
                uow.BeginTransaction();

                try
                {
                    bool consumioInsumos = false;
                    bool generoProducto = false;

                    string ejecucionStr = context.GetParameter("nuEjecucion");
                    long nroEjecucion = long.Parse(ejecucionStr);

                    //Obtener cabezal de interfaz
                    InterfazEntradaProduccion interfaz = uow.ProduccionRepository.GetInterfazEntradaProduccionPorEjecucion(nroEjecucion);

                    if (interfaz == null)
                    {
                        throw new Exception("PRD260_form1_error_InterfazNoEncontrada");
                    }

                    IngresoBlackBox ingreso = uow.ProduccionRepository.GetIngresoBlackBox(interfaz.NumeroIngreso);

                    List<InterfazEntradaProduccionInsumo> insumosConsumidos = uow.ProduccionRepository.GetInsumoConsumidoInterfaceEntradaProduccionPorEjecucion(nroEjecucion, TiposMovimiento.CONSUMO);
                    var consumoAgrupado = insumosConsumidos
                        .Where(i => i.Semiacabado == "N" && i.Consumible == "N")
                        .GroupBy(i => new { i.CodigoProducto, i.CodigoFaixa, i.Identificador, i.Semiacabado })
                        .Select(d => new { d.Key.CodigoProducto, d.Key.Identificador, Cantidad = d.Sum(e => e.CantidadSalida) }).ToList();

                    var lista = uow.ProduccionRepository.GetCantidadDetallePedidosSumados(interfaz.NumeroIngreso);

                    foreach (var d in lista)
                    {
                        var encontro = consumoAgrupado.FirstOrDefault(x => x.CodigoProducto == d.Producto && x.Identificador == d.Identificador);
                        if (encontro != null)
                        {
                            if (encontro.Cantidad != d.cantidad)
                            {
                                throw new Exception("PRD260_Sec0_btn_CantidadesNoCoinciden");
                            }
                        }
                        else
                        {
                            var insumos = insumosConsumidos
                                .Where(x => x.CodigoProducto == d.Producto && x.Identificador == d.Identificador)
                                .GroupBy(i => new { i.CodigoProducto, i.CodigoFaixa, i.Identificador, i.Semiacabado, i.Consumible })
                                .Select(i => new { i.Key.CodigoProducto, i.Key.Identificador, Cantidad = i.Sum(e => e.CantidadSalida), i.Key.Semiacabado, i.Key.Consumible })
                                .ToList();

                            if (insumos.Count() > 1)
                            {
                                throw new Exception("PRD260_Sec0_btn_ExistenProductos");
                            }
                            else if (insumos.Count() == 1)
                            {
                                var insumos1 = insumos.FirstOrDefault();

                                if (string.IsNullOrEmpty(insumos1.Semiacabado) ||
                                    (!string.IsNullOrEmpty(insumos1.Semiacabado) && (insumos1.Semiacabado == "N") && insumos1.Consumible == "N"))
                                {
                                    throw new Exception("PRD260_Sec0_btn_ExistenProductos");
                                }
                            }
                        }
                    }

                    this.ConsumirStockBB(uow, ingreso, nroEjecucion, out consumioInsumos);
                    uow.SaveChanges();

                    //Generar productos producidos
                    this.ProducirProductoBB(uow, ingreso, nroEjecucion, out generoProducto, out List<Stock> liststo);

                    //Sacar insumos y productos producidos de linea BlackBox a ubicacion de salida BB
                    this.RealizarSalidaProductoProducidoBB(uow, ingreso, nroEjecucion, liststo);
                    this.RealizarSalidaInsumoBB(uow, ingreso, nroEjecucion);

                    if (consumioInsumos && generoProducto)
                    {
                        this.GenerarDocumentosProduccion(uow, ingreso.Id);
                    }

                    this.CerrarProduccion(uow, ingreso.Id);

                    InterfazProductoProducidoBB interfazSalida = new InterfazProductoProducidoBB(uow);
                    interfazSalida.AprobarInterfazProductoProducido(nroEjecucion);

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD260_form1_success_InterfazAprobada");
                    context.Redirect("/produccion/PRD250", new List<ComponentParameter>() { });

                    uow.Commit();
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }
        }

        public virtual void ConsumirStockBB(IUnitOfWork uow, IngresoBlackBox ingreso, long nroEjecucion, out bool consumioInsumo)
        {
            //Obtener detalles de interfaz para consumo
            var insumosConsumidos = uow.ProduccionRepository.GetInsumoConsumidoInterfaceEntradaProduccionPorEjecucion(nroEjecucion, TiposMovimiento.CONSUMO);
            var consumoAgrupado = insumosConsumidos.GroupBy(i => new { i.CodigoProducto, i.CodigoFaixa, i.Identificador, i.Semiacabado, i.Consumible }).ToList();
            consumioInsumo = insumosConsumidos.Any();

            //Consumir stock
            var nuTransaccion = uow.GetTransactionNumber();
            var ajuste = new AjusteConsumidoBlackBox(uow, ingreso, this._identity.UserId);

            foreach (var consumo in consumoAgrupado)
            {
                var producto = consumo.FirstOrDefault().CodigoProducto;
                var identificador = consumo.FirstOrDefault().Identificador;
                var faixa = consumo.FirstOrDefault().CodigoFaixa;
                var empresa = ingreso.Empresa;
                var cantidad = consumo.Sum(c => c.CantidadSalida);

                int empresaInt = (int)empresa;
                //ajuste.AjustarProducto(empresaInt, producto, identificador, faixa, cantidad, consumo.Key.Semiacabado, consumo.Key.Consumible);
            }

            uow.SaveChanges();
        }

        public virtual void ProducirProductoBB(IUnitOfWork uow, IngresoBlackBox ingreso, long nroEjecucion, out bool generoProducto, out List<Stock> Lisststock)
        {
            Lisststock = new List<Stock>();

            //Obtener detalles de interfaz para producir
            List<InterfazEntradaProduccionProducido> productosProducidos = uow.ProduccionRepository.GetProducidoInterfaceEntradaProduccionPorEjecucion(nroEjecucion);

            var producidoAgrupado = productosProducidos.GroupBy(i => new { i.CodigoProducto, i.CodigoFaixa, i.Identificador, i.Semiacabado }).ToList();
            generoProducto = productosProducidos.Any();

            var ajuste = new AjusteProducidoBlackBox(uow, ingreso, this._identity.UserId);

            foreach (var producido in producidoAgrupado)
            {
                Stock stock;
                var producto = producido.FirstOrDefault().CodigoProducto;
                var identificador = producido.FirstOrDefault().Identificador;

                producido.FirstOrDefault().FechaVencimiento.TryParseFromIso(out DateTime? vencimiento);

                var faixa = producido.FirstOrDefault().CodigoFaixa;
                var empresa = ingreso.Empresa;
                int empresaInt = (int)empresa;
                var cantidad = producido.Sum(c => c.CantidadProducido ?? 0);
                var prod = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresaInt, producto);

                ajuste.AddLineaProducto(producto, prod.ParseIdentificador(identificador), faixa, vencimiento, cantidad, empresaInt, out stock, false, producido.Key.Semiacabado);

                if (Lisststock.Any(x => x.Identificador == prod.ParseIdentificador(identificador) && x.Producto == producto && x.Faixa == faixa))
                {
                    Stock sto = Lisststock.FirstOrDefault(x => x.Identificador == prod.ParseIdentificador(identificador) && x.Producto == producto && x.Faixa == faixa);
                    Lisststock.Remove(sto);
                    sto.Cantidad = (sto.Cantidad ?? 0) + cantidad;
                    Lisststock.Add(sto);
                }
                else
                {
                    Lisststock.Add(stock);
                }

                uow.SaveChanges();
            }

            uow.SaveChanges();
        }

        public virtual void RealizarSalidaProductoProducidoBB(IUnitOfWork uow, IngresoBlackBox ingreso, long nroEjecucion, List<Stock> liststo)
        {
            //Obtener detalles de interfaz producto producido
            var productosProducidos = uow.ProduccionRepository.GetProducidoInterfaceEntradaProduccionPorEjecucion(nroEjecucion);
            var producidoAgrupado = productosProducidos.GroupBy(i => new { i.CodigoProducto, i.CodigoFaixa, i.Identificador, i.AccionMovimiento, i.Semiacabado }).ToList();
            var nroTransaccion = uow.GetTransactionNumber();
            var salidaBlackBox = new SalidaBlackBox(this._uowFactory);

            foreach (var producido in producidoAgrupado)
            {
                var producto = producido.FirstOrDefault().CodigoProducto;
                var identificador = producido.FirstOrDefault().Identificador;
                var faixa = producido.FirstOrDefault().CodigoFaixa;
                var empresa = ingreso.Empresa;
                int empresaInt = (int)empresa;
                var cantidad = producido.Sum(c => c.CantidadProducido ?? 0);
                var accionMovimiento = producido.FirstOrDefault().AccionMovimiento;

                //Obtener Producto
                Producto prod = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresaInt, producto);

                //Obtener stock
                var stock = liststo.FirstOrDefault(x => x.Ubicacion == ((LineaBlackBox)ingreso.Linea).UbicacionBlackBox && x.Empresa == empresa && x.Producto == producto && x.Identificador == prod.ParseIdentificador(identificador) && x.Faixa == faixa && (x.Cantidad ?? 0) > 0);

                if (accionMovimiento == TiposMovimiento.SALIDA_PRODUCTO)
                {
                    salidaBlackBox.SalidaStockBlackBox(uow, stock, cantidad, 0, TipoStockOutBB.PRODUCTO, ingreso.Id, this._identity.UserId, producido.Key.Semiacabado);
                }
                else if (accionMovimiento == TiposMovimiento.SALIDA_PRODUCTO_AVERIADO)
                {
                    salidaBlackBox.SalidaStockBlackBox(uow, stock, 0, cantidad, TipoStockOutBB.PRODUCTO, ingreso.Id, this._identity.UserId, producido.Key.Semiacabado);
                }
            }

            foreach (var sto in liststo)
            {
                Producto prod = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(sto.Empresa, sto.Producto);
                var existe = uow.StockRepository.GetStock(sto.Empresa, sto.Producto, sto.Faixa, ((LineaBlackBox)ingreso.Linea).UbicacionBlackBox, prod.ParseIdentificador(sto.Identificador));

                sto.NumeroTransaccion = nroTransaccion;

                if (existe != null)
                {
                    uow.StockRepository.UpdateStock(sto);
                }
                else
                {
                    uow.StockRepository.AddStock(sto);
                }

                uow.SaveChanges();
            }
        }

        public virtual void RealizarSalidaInsumoBB(IUnitOfWork uow, IngresoBlackBox ingreso, long nroEjecucion)
        {
            try
            {
                //Obtener detalles de interfaz salida insumos sanos y averiados
                var insumos = uow.ProduccionRepository.GetInsumoConsumidoInterfaceEntradaProduccionPorEjecucion(nroEjecucion, TiposMovimiento.SALIDA_INSUMO);
                var insumosAveriados = uow.ProduccionRepository.GetInsumoConsumidoInterfaceEntradaProduccionPorEjecucion(nroEjecucion, TiposMovimiento.SALIDA_INSUMO_AVERIADO);

                insumos.AddRange(insumosAveriados);

                var insumosAgrupado = insumos.GroupBy(i => new { i.CodigoProducto, i.CodigoFaixa, i.Identificador, i.AccionMovimiento }).ToList();
                var nuTransaccion = uow.GetTransactionNumber();
                var salidaBlackBox = new SalidaBlackBox(this._uowFactory);

                foreach (var insumo in insumosAgrupado)
                {
                    var producto = insumo.FirstOrDefault().CodigoProducto;
                    var identificador = insumo.FirstOrDefault().Identificador;
                    var faixa = insumo.FirstOrDefault().CodigoFaixa;
                    var empresa = ingreso.Empresa;
                    int empresaInt = (int)ingreso.Empresa;
                    var cantidad = insumo.Sum(c => c.CantidadSalida);
                    var accionMovimiento = insumo.FirstOrDefault().AccionMovimiento;

                    //Obtener stock
                    var stock = uow.StockRepository.GetStock(empresaInt, producto, faixa, ((LineaBlackBox)ingreso.Linea).UbicacionBlackBox, identificador);

                    if (accionMovimiento == TiposMovimiento.SALIDA_INSUMO)
                    {
                        salidaBlackBox.SalidaStockBlackBox(uow, stock, cantidad, 0, TipoStockOutBB.INSUMO, ingreso.Id, this._identity.UserId);
                    }
                    else if (accionMovimiento == TiposMovimiento.SALIDA_INSUMO_AVERIADO)
                    {
                        salidaBlackBox.SalidaStockBlackBox(uow, stock, 0, cantidad, TipoStockOutBB.INSUMO, ingreso.Id, this._identity.UserId);
                    }

                    stock.NumeroTransaccion = nuTransaccion;

                    uow.StockRepository.UpdateStock(stock);
                }

                uow.SaveChanges();
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual void CerrarProduccion(IUnitOfWork uow, string nroIngreso)
        {
            //Obtener ingreso produccion
            var ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);

            //Cerrar produccion
            ingreso.CerrarProduccion();
            uow.ProduccionRepository.UpdateIngresoBlackBox(ingreso);
            uow.SaveChanges();

            var ajusteConsumo = new AjusteConsumidoBlackBox(uow, ingreso, this._identity.UserId);
            //ajusteConsumo.CrearHistoricoDeConsumo(ingreso.Consumidos);
            uow.SaveChanges();

            var ajusteProducido = new AjusteProducidoBlackBox(uow, ingreso, this._identity.UserId);
            //ajusteProducido.CrearHistoricoProducido(ingreso.Producidos);
            uow.SaveChanges();
        }

        public virtual void GenerarDocumentosProduccion(IUnitOfWork uow, string nroIngreso)
        {
            var ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);
            var lineasIngreso = new List<LineaIngresoDocumentalRequest>();

            foreach (var lineaProducida in ingreso.Producidos)
            {
                int empresaInt = (int)ingreso.Empresa;
                //lineasIngreso.Add(new LineaIngresoDocumentalRequest() { CantidadAfectada = lineaProducida.Cantidad, Empresa = empresaInt, Identificador = lineaProducida.Identificador, Producto = lineaProducida.Producto, Faixa = lineaProducida.Faixa, Semiacabado = lineaProducida.Semiacabado });
            }

            var lineasReserva = new List<LineaReservaDocumentalRequest>();
            //var listaConsumido = ingreso.Consumidos
            //            .GroupBy(c => new { c.Empresa, c.Producto, c.Faixa, c.Identificador, c.Semiacabado, c.Consumible })
            //            .Select(s => new { s.Key.Empresa, s.Key.Producto, s.Key.Faixa, s.Key.Identificador, Cantidad = s.Sum(e => e.Cantidad), s.Key.Semiacabado, s.Key.Consumible })
            //            .ToList();

            //foreach (var reserva in listaConsumido)
            //{
            //    lineasReserva.Add(new LineaReservaDocumentalRequest()
            //    {
            //        Identificador = reserva.Identificador,
            //        CantidadAfectada = reserva.Cantidad,
            //        Empresa = reserva.Empresa,
            //        Faixa = reserva.Faixa,
            //        Producto = reserva.Producto,
            //        Semiacabado = reserva.Semiacabado,
            //        Consumible = reserva.Consumible
            //    });
            //}

            int empresaRequestInt = (int)ingreso.Empresa;

            var request = new ProduccionDocumentalBlackBoxRequest()
            {

                Aplicacion = this._identity.Application,
                Empresa = empresaRequestInt,
                NroProduccion = ingreso.Id,
                Usuario = this._identity.UserId,
                LineasIngreso = lineasIngreso,
                Reservas = lineasReserva
            };

            var produccion = new ProduccionDocumental(this._uowFactory, this._factoryService, this._parameterService, this._identity);
            var response = produccion.DocumentarProduccionBlackBox(request, uow);
        }

        #endregion
    }
}
