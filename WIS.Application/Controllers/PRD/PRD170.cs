using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD170 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public PRD170(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "NU_PRDC_INGRESO"
            };
        }
        
        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {

                new GridItemHeader("PRD170_Sec0_lbl_Acciones"),
                new GridItemDivider(),
                new GridButton("btnDetalles", "PRD170_grid1_btn_Detalles", "fas fa-list"),
                //new GridButton("btnPasadas", "PRD170_grid1_btn_Pasadas", "fas fa-pencil-alt"),
                new GridButton("btnProduccion", "PRD170_grid1_btn_Produccion", "fas fa-paper-plane"),
                new GridButton("btnAjuste", "PRD170_grid1_btn_Ajustes", "fas fa-info"),
                new GridButton("btnCrearPedido", "PRD170_grid1_btn_Pedido", "far fa-file-pdf"),
            }));

            grid.SetInsertableColumns(new List<string>
            {
                "CD_PRDC_LINEA"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new PanelProduccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                this.FormatGrid1(grid, uow);
            }

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new PanelProduccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {

            try
            {
                switch (context.ButtonId)
                {
                    case "btnDetalles":
                        context.Redirect("/produccion/PRD151", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuPrdcIngreso", Value = context.Row.GetCell("NU_PRDC_INGRESO").Value },
                            new ComponentParameter(){ Id = "cdPrdcLinea", Value = context.Row.GetCell("CD_PRDC_LINEA").Value },
                            new ComponentParameter(){ Id = "cdPrdcDefinicion", Value = context.Row.GetCell("CD_PRDC_DEFINICION").Value },
                        });
                        break;
                    case "btnPasadas":
                        context.Redirect("/produccion/PRD180", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuPrdcIngreso", Value = context.Row.GetCell("NU_PRDC_INGRESO").Value },
                            new ComponentParameter(){ Id = "cdPrdcLinea", Value = context.Row.GetCell("CD_PRDC_LINEA").Value },
                            new ComponentParameter(){ Id = "cdPrdcDefinicion", Value = context.Row.GetCell("CD_PRDC_DEFINICION").Value },
                        });
                        break;
                    case "btnAjuste":
                        context.Redirect("/produccion/PRD175", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuPrdcIngreso", Value = context.Row.GetCell("NU_PRDC_INGRESO").Value },
                            new ComponentParameter(){ Id = "ndTipo", Value = context.Row.GetCell("ND_TIPO").Value },
                        });
                        break;
                    case "btnCrearPedido":
                        try
                        {
                            // Creación de pedidos extras para producción
                            string numeroOrdenIngreso = context.Row.GetCell("NU_PRDC_INGRESO").Value;
                            string tipoIngreso = context.Row.GetCell("ND_TIPO").Value;

                            using (var uow = this._uowFactory.GetUnitOfWork())
                            {
                                var situacionIngreso = short.Parse(context.Row.GetCell("CD_SITUACAO").Value);

                                // Se controla que la produccion no este finalizada
                                if (situacionIngreso == SituacionDb.PRODUCCION_FINALIZADA)
                                    throw new ValidationFailedException("PRD170_Sec0_Error_Error01");

                                uow.CreateTransactionNumber("CrearPedido");

                                var ingreso = uow.ProduccionRepository.GetIngreso(numeroOrdenIngreso);
                                var pedido = this.CrearPredidoExtraProduccion(uow, ingreso);

                                uow.PedidoRepository.AddPedidoConDetalle(pedido);
                                uow.SaveChanges();

                                context.AddSuccessNotification("PRD170_Sec0_Succes_SeCreoElIngresoPedido", new List<string> { pedido.Id });
                            }
                        }
                        catch (ValidationFailedException ex)
                        {
                            this._logger.Error(ex, ex.Message);
                            context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }

            return context;

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
                            throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                        foreach (var row in grid.Rows)
                        {

                            var numeroIngreso = row.GetCell("NU_PRDC_INGRESO").Value;
                            var lineaIngresada = row.GetCell("CD_PRDC_LINEA").Value;

                            if (!row.IsNew && !row.IsDeleted)
                            {
                                this.UpdateIngresoProduccionPanel(uow, uow.ProduccionRepository.GetIngresoPanel(numeroIngreso), lineaIngresada);
                            }
                        }
                    }

                    uow.SaveChanges();

                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
                catch (ValidationFailedException ex)
                {
                    this._logger.Error(ex, ex.Message);
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }

            return grid;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRDC_LINEA":
                    return this.SearchLinea(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._gridValidationService.Validate(new PRD170GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PanelProduccionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual void FormatGrid1(Grid grid, IUnitOfWork uow)
        {
            ProduccionMapper mapper = new ProduccionMapper(new LineaMapper(), new FormulaMapper(new FormulaAccionMapper()));

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>() { "CD_PRDC_LINEA" });

                var cellLinea = row.GetCell("CD_PRDC_LINEA");
                var numeroIngreso = row.GetCell("NU_PRDC_INGRESO").Value;
                var situacionIngreso = short.Parse(row.GetCell("CD_SITUACAO").Value);
                TipoProduccionIngreso tipoIngreso = mapper.MapStringToTipoIngreso(row.GetCell("ND_TIPO").Value);

                // Si el ingreso a produccion fue finalizado no se puede cambiar la lineas asociada
                // Si el ingreso a produccion se encuentra iniciado no se puede cambiar la lineas asociada
                // Si el tipo de ingreso a producción es por colector no se puede asigar una linea
                if (situacionIngreso == SituacionDb.PRODUCCION_FINALIZADA || tipoIngreso == TipoProduccionIngreso.Colector)
                {
                    cellLinea.Editable = false;
                }

                // Acciones accesibles
                var situacion = short.Parse(row.GetCell("CD_SITUACAO").Value);
                var linea = row.GetCell("CD_PRDC_LINEA").Value;

                if (situacion == SituacionDb.PRODUCCION_FINALIZADA)
                {
                    row.DisabledButtons.Add("btnProduccion");
                    row.DisabledButtons.Add("btnAjuste");
                    row.DisabledButtons.Add("btnCrearPedido");
                }

                if (string.IsNullOrEmpty(linea))
                {
                    row.DisabledButtons.Add("btnProduccion");
                    row.DisabledButtons.Add("btnAjuste");
                }

                if (tipoIngreso == TipoProduccionIngreso.BlackBox)
                {
                    row.DisabledButtons.Add("btnProduccion");
                    row.DisabledButtons.Add("btnPasadas");
                }
            }
        }

        public virtual List<SelectOption> SearchLinea(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            List<SelectOption> options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                LineaMapper mapperLinea = new LineaMapper();
                ProduccionMapper mapperProduccion = new ProduccionMapper(mapperLinea, new FormulaMapper(new FormulaAccionMapper()));

                TipoProduccionIngreso tipoIngreso = mapperProduccion.MapStringToTipoIngreso(row.GetCell("ND_TIPO").Value);
                TipoProduccionLinea tipoLinea = TipoProduccionLinea.Unknown;

                switch (tipoIngreso)
                {
                    case TipoProduccionIngreso.PanelWeb:
                        tipoLinea = TipoProduccionLinea.WhiteBox;
                        break;
                    case TipoProduccionIngreso.BlackBox:
                        tipoLinea = TipoProduccionLinea.BlackBox;
                        break;
                }

                var dbQuery = new LineasDisponiblesQuery(tipoLinea, mapperLinea);

                uow.HandleQuery(dbQuery);

                List<ILinea> lineas = dbQuery.GetLineasDisponibles();

                foreach (var linea in lineas)
                {
                    options.Add(new SelectOption(linea.Id, linea.Descripcion));
                }
            }

            return options;
        }

        public virtual Pedido CrearPredidoExtraProduccion(IUnitOfWork uow, IIngreso ingreso)
        {
			int empresaInt = (int)ingreso.Empresa;
			var empresa = uow.EmpresaRepository.GetEmpresa(empresaInt);
            int cantidadPedidos = uow.PedidoRepository.GetCantidadPedidosOrden(empresa.Id, empresa.CdClienteArmadoKit, ingreso.Id);
            var cliente = uow.AgenteRepository.GetAgente(empresaInt, empresa.CdClienteArmadoKit);

            Pedido pedido = new Pedido()
            {
                Id = string.Format("{0}/{1}", ingreso.Id, cantidadPedidos),
                Empresa = empresaInt,
                Cliente = empresa.CdClienteArmadoKit,
                Ruta = cliente.Ruta.Id,
                Estado = SituacionDb.PedidoAbierto,
                IsManual = false,
                Agrupacion = "P",
                FechaAlta = DateTime.Now,
                IngresoProduccion = ingreso.Id,
                Memo1 = "Pedido generado para producción en PRD170",
                Origen = "PRD170",
                CondicionLiberacion = "WIS-SC",
                Tipo = TipoPedidoDb.Produccion,
                Memo = "Pedido generado para producción.",
                ConfiguracionExpedicion = new ConfiguracionExpedicionPedido() { Tipo = Domain.DataModel.Mappers.Constants.TipoExpedicion.Produccion },
                Anexo = cantidadPedidos.ToString(), // Se utiliza el anexo 1 para llevar el orden de creación de los pedidos extra, se utiliza en vista pantalla PRD110
                Predio = ingreso.Predio,
                NuCarga = null,
                Transaccion = uow.GetTransactionNumber(),
            };

            return pedido;
        }

        public virtual void UpdateIngresoProduccionPanel(IUnitOfWork uow, IIngresoPanel ingreso, string lineaIngresada)
        {
            if (ingreso != null)
            {
                var linea = uow.LineaRepository.GetLinea(lineaIngresada);
                var lineaAnterior = ingreso.Linea;

                if (linea == null && lineaAnterior == null)
                {
                    throw new ValidationFailedException("PRD170_Sec0_Error_Error02_LineaNoExiste");
                }

                this.ValidarLineaUpdate(uow, ingreso, linea, lineaAnterior);

                if (ingreso.TipoProduccion == TipoProduccionIngreso.PanelWeb || ingreso.TipoProduccion == TipoProduccionIngreso.BlackBox)
                {
                    // Tipo panel web se mantiene la regla de asociación 1 orden activa a una linea sin orden 
                    if (linea.NumeroIngreso != null)
                    {
                        //Verificar si la linea esta en uso
                        if (uow.ProduccionRepository.LineaOcupadaPorIngreso(linea, TipoProduccionIngreso.PanelWeb))
                        {
                            throw new ValidationFailedException("PRD170_Sec0_Error_Error03_LineaEnUso");
                        }
                    }

                    // Actualizo el ingreso con la linea ingresda, si no carga linea la quita del ingreso
                    ingreso.Linea = linea;
                    ingreso.FechaActualizacion = DateTime.Now;

                    switch (ingreso.TipoProduccion)
                    {
                        case TipoProduccionIngreso.PanelWeb:
                            uow.ProduccionRepository.UpdateIngresoWhiteBox((IngresoWhiteBox)ingreso);
                            break;
                        case TipoProduccionIngreso.BlackBox:
                            uow.ProduccionRepository.UpdateIngresoBlackBox((IngresoBlackBox)ingreso);
                            break;
                    }

                    if (linea != null)
                    {
                        // Actualizo la linea ingresada si la misma no es null
                        linea.NumeroIngreso = ingreso.Id;
                        uow.LineaRepository.UpdateLinea(linea);
                    }
                    else
                    {
                        // Quito el numero de ingreso de la linea anteriormente carga para el ingreso
                        lineaAnterior.NumeroIngreso = null;
                        uow.LineaRepository.UpdateLinea(lineaAnterior);
                    }
                }
                else
                {
                    throw new ValidationFailedException("PRD170_Sec0_Error_Error04_TipoIngresoIncorrecto");
                }
            }
        }

        public virtual void ValidarLineaUpdate(IUnitOfWork uow, IIngresoPanel ingreso, ILinea linea, ILinea lineaAnterior)
        {

            // Se controla que la producción no haya sido finalizada
            if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
                throw new ValidationFailedException("PRD170_Sec0_Error_Error01");

            // Se controla que la ubicacones de entrada y salida de la linea anterior esten vacias
            if (lineaAnterior != null && (!uow.StockRepository.AnyStockPositivoUbicacion(lineaAnterior.UbicacionEntrada)
                || !uow.StockRepository.AnyStockPositivoUbicacion(lineaAnterior.UbicacionSalida)))
                throw new ValidationFailedException("PRD170_Sec0_Error_Error05_UbicacionConStock");

            // Desasociar línea
            // Se controla que el ingreso no tenga pasadas para desasociar la linea
            if (linea == null && ingreso.Pasadas.Any())
                throw new ValidationFailedException("PRD170_Sec0_Error_Error06_IngresoConPasadas");

            //Asiganción de linea nueva
            if (linea != null)
            {

                // Se contrala que el  tipo de linea coincida con el tipo de ingreso a producción
                if ((ingreso.TipoProduccion == TipoProduccionIngreso.BlackBox && linea.Tipo != TipoProduccionLinea.BlackBox)
                   || (ingreso.TipoProduccion == TipoProduccionIngreso.PanelWeb && linea.Tipo != TipoProduccionLinea.WhiteBox)
                   || ingreso.TipoProduccion == TipoProduccionIngreso.Colector)
                    throw new ValidationFailedException("PRD170_Sec0_Error_Error04_TipoIngresoIncorrecto");

                // Se verifica que la linea no se encuentre asociada a otro ingreso activo ( WhiteBox y BlackBox)
                if (uow.ProduccionRepository.ExisteIngresoActivoEnLinea(linea.Id))
                    throw new ValidationFailedException("PRD170_Sec0_Error_Error03_LineaEnUso");

                // Se controla que las ubicaciones de la linea que se quiere asignar esten vacias
                if (linea != null && (uow.StockRepository.AnyStockPositivoUbicacion(linea.UbicacionEntrada)
                    || uow.StockRepository.AnyStockPositivoUbicacion(linea.UbicacionSalida)))
                    throw new ValidationFailedException("PRD170_Sec0_Error_Error05_UbicacionConStock");
            }
        }

        #endregion
    }
}
