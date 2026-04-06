using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Security;
using WIS.CheckboxListComponent;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Documento;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
using CondicionLiberacion = WIS.Domain.Liberacion.CondicionLiberacion;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052SeleccionPedidosLiberacionOnda : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PRE052SeleccionPedidosLiberacionOnda> _logger;

        protected List<string> GridKeys { get; }

        protected List<SortCommand> DefaultSort { get; }

        public PRE052SeleccionPedidosLiberacionOnda(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            ILogger<PRE052SeleccionPedidosLiberacionOnda> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_EMPRESA", "CD_CLIENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Ascending),
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._security.IsUserAllowed(SecurityResources.WPRE051_grid1_btn_GuardarLibOnda))
            {
                grid.MenuItems = new List<IGridItem>
                {
                    new GridButton("btnLiberar", "PRE052_grid1_btn_liberar")
                };
            }

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("QT_PRODUCTOS_SIN_PESO_BRUTO", "PRE052_grid1_btn_PesoSinPesoBruto", "fa fa-exclamation-triangle fa-lg")
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY2", new List<GridButton>
            {
                new GridButton("QT_PRODUCTOS_SIN_VOLUMEN", "PRE052_grid1_btn_PesoSinVol", "fa fa-times-circle fa-lg")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = ParametrosPreparacion(context.Parameters, uow);

            var dbQuery = new PendienteLiberacionQuery(datos.Empresa, datos.Onda, datos.Predio);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            SetConfigurationRows(grid, uow);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = this.ParametrosPreparacion(context.Parameters, uow);

            var dbQuery = new PendienteLiberacionQuery(datos.Empresa, datos.Onda, datos.Predio);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = this.ParametrosPreparacion(context.Parameters, uow);

            var dbQuery = new PendienteLiberacionQuery(datos.Empresa, datos.Onda, datos.Predio);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();

            try
            {
                if (context.ButtonId == "btnLiberar")
                {
                    LiberarOnda(context, uow);
                    uow.SaveChanges();
                }

                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "PRE052SeleccionPedidosLiberacionOnda - GridMenuItemAction");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                _logger.LogError(ex, "PRE052SeleccionPedidosLiberacionOnda - GridMenuItemAction");
                uow.Rollback();
            }

            return context;
        }

        public override GridNotifySelectionContext GridNotifySelection(GridNotifySelectionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            GridNotifySelectionCalculos(uow, context.Parameters, context.Selection, context.Filters);
            return context;
        }

        public override GridNotifyInvertSelectionContext GridNotifyInvertSelection(GridNotifyInvertSelectionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            GridNotifySelectionCalculos(uow, context.Parameters, context.Selection, context.Filters);
            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }

        #region Metodos Auxiliares

        public virtual Preparacion ParametrosPreparacion(List<ComponentParameter> parameters, IUnitOfWork uow)
        {
            var tiposDocumento = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.Find(x => x.Id == "tiposDocumento").Value);
            var documentos = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.Find(x => x.Id == "documentos").Value);
            var empresa = int.Parse(parameters.Find(x => x.Id == "idEmpresa").Value);
            var onda = short.Parse(parameters.Find(x => x.Id == "onda").Value);
            var descripcion = parameters.Find(x => x.Id == "descripcion")?.Value;

            var tiposDocumentosLiberables = new HashSet<string>();
            var documentosLiberables = new List<DocumentoLiberable>();

            foreach (var t in tiposDocumento)
            {
                if (t.Selected && !tiposDocumentosLiberables.Contains(t.Id))
                {
                    tiposDocumentosLiberables.Add(t.Id);
                }
            }

            if (tiposDocumentosLiberables.Count > 0)
            {
                documentosLiberables.AddRange(uow.DocumentoRepository.GetDocumentosLiberables(empresa, tiposDocumentosLiberables));
            }

            foreach (var d in documentos)
            {
                if (d.Selected)
                {
                    var id = d.Id.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
                    var tipoDocumento = id[0];

                    if (!tiposDocumentosLiberables.Contains(tipoDocumento))
                    {
                        var nroDocumento = id[1];

                        documentosLiberables.Add(new DocumentoLiberable()
                        {
                            Tipo = tipoDocumento,
                            Numero = nroDocumento
                        });
                    }
                }
            }

            var dtoPreparacion = new Preparacion()
            {
                Empresa = empresa,
                Onda = onda,
                Agrupacion = parameters.Find(x => x.Id == "agrupacion").Value,
                RespetarFifoEnLoteAUTO = parameters.Find(x => x.Id == "respetaFifo")?.Value == "S" ? true : false,
                ControlaStockDocumental = parameters.Find(x => x.Id == "stockDtmi").Value == "S" ? true : false,
                CursorStock = parameters.Find(x => x.Id == "stock").Value,
                DebeLiberarPorCurvas = parameters.Find(x => x.Id == "liberarPorCurvas")?.Value == "S" ? true : false,
                DebeLiberarPorUnidades = parameters.Find(x => x.Id == "liberarPorUnidades")?.Value == "S" ? true : false,
                RepartirEscasez = parameters.Find(x => x.Id == "repartirEscasez").Value,
                PickingEsAgrupadoPorCamion = parameters.Find(x => x.Id == "agrupPorCamion")?.Value == "S" ? true : false,
                PrepararSoloConCamion = parameters.Find(x => x.Id == "prepSoloCamion")?.Value == "S" ? true : false,
                ModalPalletCompleto = parameters.Find(x => x.Id == "ubicacionCompleta").Value,
                ModalPalletIncompleto = parameters.Find(x => x.Id == "ubicacionIncompleta").Value,
                CursorPedido = parameters.Find(x => x.Id == "pedidos").Value,
                Predio = parameters.Find(x => x.Id == "predio").Value,
                PriozarDesborde = parameters.Find(x => x.Id == "priorizarDesborde")?.Value == "S" ? true : false,
                UsarSoloStkPicking = parameters.Find(x => x.Id == "usarSoloStkPicking")?.Value == "true" ? true : false,
                Documentos = documentosLiberables,

                ManejaVidaUtil = parameters.Find(x => x.Id == "manejaVidaUtil")?.Value == "S" ? true : false,
                RequiereUbicacion = parameters.Find(x => x.Id == "ubicacionPicking2Fases")?.Value == "S" ? true : false,
                Descripcion = string.IsNullOrEmpty(descripcion) ? "Liberación de Onda Manual" : descripcion,
                ExcluirUbicacionesPicking = parameters.Find(x => x.Id == "excluirUbicPicking")?.Value == "true" ? true : false,
            };

            dtoPreparacion.UsarSoloStkPicking = dtoPreparacion.UsarSoloStkPicking && !dtoPreparacion.ExcluirUbicacionesPicking;

            return dtoPreparacion;
        }

        public virtual void SetConfigurationRows(Grid grid, IUnitOfWork uow)
        {
            foreach (var row in grid.Rows)
            {
                var botonesDeshabiltiados = new List<string>();

                if (decimal.Parse(row.GetCell("QT_LIBERADO").Value, _identity.GetFormatProvider()) > 0)
                {
                    row.CssClass = row.CssClass + " liberado";
                }
                else if (string.IsNullOrEmpty(row.GetCell("NU_ULT_PREPARACION").Value))
                {
                    row.CssClass = row.CssClass + " ultPrepVacia";
                }
                else
                {
                    row.CssClass = row.CssClass + " ultPrep";
                }

                if (!string.IsNullOrEmpty(row.GetCell("QT_PRODUCTOS_SIN_PESO_BRUTO").Value))
                {
                    var qtProdSinPeso = decimal.Parse(row.GetCell("QT_PRODUCTOS_SIN_PESO_BRUTO").Value, _identity.GetFormatProvider());
                    if (qtProdSinPeso <= 0)
                        botonesDeshabiltiados.Add("QT_PRODUCTOS_SIN_PESO_BRUTO");
                }

                if (!string.IsNullOrEmpty(row.GetCell("QT_PRODUCTOS_SIN_VOLUMEN").Value))
                {
                    var qtProdSinVolumen = decimal.Parse(row.GetCell("QT_PRODUCTOS_SIN_VOLUMEN").Value, _identity.GetFormatProvider());
                    if (qtProdSinVolumen <= 0)
                        botonesDeshabiltiados.Add("QT_PRODUCTOS_SIN_VOLUMEN");
                }


                if (row.GetCell("AUX_VL_VOLUMEN") != null && row.GetCell("VL_VOLUMEN_TOTAL") != null && !string.IsNullOrEmpty(row.GetCell("VL_VOLUMEN_TOTAL").Value) &&
                    decimal.TryParse(row.GetCell("VL_VOLUMEN_TOTAL").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal totalVol) && totalVol > 0)
                {
                    var value = (totalVol / 1000000).ToString(_identity.GetFormatProvider());
                    row.GetCell("AUX_VL_VOLUMEN").Value = value;
                    row.GetCell("AUX_VL_VOLUMEN").ForceSetOldValue(value);
                }

                row.DisabledButtons = botonesDeshabiltiados;
            }
        }

        public virtual void GridNotifySelectionCalculos(IUnitOfWork uow, List<ComponentParameter> parameters, GridSelection selection, List<FilterCommand> filters)
        {
            var empresa = int.Parse(parameters.FirstOrDefault(x => x.Id == "idEmpresa").Value);
            var onda = short.Parse(parameters.FirstOrDefault(x => x.Id == "onda").Value);
            var predio = parameters.FirstOrDefault(x => x.Id == "predio")?.Value;

            var dbQuery = new PendienteLiberacionQuery(empresa, onda, predio);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, filters);

            var keys = dbQuery.GetSelectedKeys(selection.Keys);

            if (selection.AllSelected)
                keys = dbQuery.GetSelectedKeysAndExclude(selection.Keys);

            var valoresCalculados = uow.PreparacionRepository.GetCalculosLiberacion(keys);

            parameters.Add(new ComponentParameter("unidadesCalculadas", valoresCalculados.Unidades.ToString(_identity.GetFormatProvider())));
            parameters.Add(new ComponentParameter("lineasCalculadas", valoresCalculados.CantidadLineas.ToString(_identity.GetFormatProvider())));
            parameters.Add(new ComponentParameter("pesoCalculado", valoresCalculados.Peso.ToString(_identity.GetFormatProvider())));
            parameters.Add(new ComponentParameter("volumenCalculado", valoresCalculados.VolumenFinal.ToString(_identity.GetFormatProvider())));
            parameters.Add(new ComponentParameter("filasSeleccionadas", valoresCalculados.FilasSeleccionadas.ToString(_identity.GetFormatProvider())));
        }

        public virtual GridMenuItemActionContext LiberarOnda(GridMenuItemActionContext context, IUnitOfWork uow)
        {
            var hayAlgunPedidoValido = false;
            var pedidosLiberados = new List<Pedido>();
            var pedidosNoLiberados = new List<Pedido>();

            var dtoPrep = ParametrosPreparacion(context.Parameters, uow);

            if (dtoPrep == null)
                throw new ValidationFailedException("PRE052SelecLibOnda_Sec0_Error_Er006_ImposibleRealizarLaOperacion");

            var keysSelected = GetSelectedKeys(uow, context, dtoPrep, out decimal porcentajeVidaUtil, out bool diferentesPorcentajesVidaUtil);

            if (keysSelected.Count() == 0)
                throw new ValidationFailedException("PRE052SelecLibOnda_Sec0_Error_Er002_SeleccionarPedido");

            if (dtoPrep.ManejaVidaUtil && diferentesPorcentajesVidaUtil)
                throw new ValidationFailedException("PRE052SelecLibOnda_Sec0_Error_PedidosSelecNoManejanMismoPorcen");

            var cursoresInvalidos = new string[] { "1", "2", "3" };

            if (cursoresInvalidos.Contains(dtoPrep.CursorPedido))
                throw new ValidationFailedException("PRE052SelecLibOnda_Sec0_Error_Er001_OperacionDeshabilitada");

            uow.CreateTransactionNumber("Liberación de Onda Manual");

            var transaccion = uow.GetTransactionNumber();

            dtoPrep.Transaccion = transaccion;
            dtoPrep.Tipo = TipoPreparacionDb.Normal;
            dtoPrep.Usuario = this._identity.UserId;
            dtoPrep.Situacion = SituacionDb.PreparacionPendiente;
            dtoPrep.FechaInicio = DateTime.Now;
            dtoPrep.AceptaMercaderiaAveriada = false;
            dtoPrep.PermitePickVencido = false;
            dtoPrep.ValidarProductoProveedor = false;

            short.TryParse(porcentajeVidaUtil.ToString(), out short parsedValue);
            dtoPrep.ValorVidaUtil = parsedValue;

            uow.PreparacionRepository.AddPreparacion(dtoPrep);

            SetCondicionesLiberacion(context.Parameters, uow, dtoPrep);

            foreach (var key in keysSelected)
            {
                var pedidoPendienteLiberar = uow.PedidoRepository.GetPedido(key.Empresa, key.Cliente, key.Pedido);

                if (pedidoPendienteLiberar.PreparacionProgramada == null)
                {
                    hayAlgunPedidoValido = true;
                    pedidoPendienteLiberar.NumeroOrdenLiberacion = 0;
                    pedidoPendienteLiberar.PreparacionProgramada = dtoPrep.Id;
                    pedidoPendienteLiberar.Transaccion = transaccion;
                    pedidoPendienteLiberar.FechaModificacion = DateTime.Now;

                    uow.PedidoRepository.UpdatePedido(pedidoPendienteLiberar);
                    uow.SaveChanges();

                    pedidosLiberados.Add(pedidoPendienteLiberar);
                }
                else
                    pedidosNoLiberados.Add(pedidoPendienteLiberar);
            }

            if (hayAlgunPedidoValido)
            {
                SeleccionPedidoCompatible(uow, dtoPrep.Onda.Value, dtoPrep.Empresa.Value, out bool valido, out string msg, out List<string> args);

                if (!valido)
                {
                    foreach (var pedido in pedidosLiberados)
                    {
                        pedido.Transaccion = transaccion;
                        pedido.PreparacionProgramada = null;
                        pedido.NumeroOrdenLiberacion = null;
                        pedido.FechaModificacion = DateTime.Now;

                        uow.PedidoRepository.UpdatePedido(pedido);
                        uow.SaveChanges();
                    }

                    var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(dtoPrep.Id);

                    preparacion.Transaccion = transaccion;
                    preparacion.TransaccionDelete = transaccion;

                    uow.PreparacionRepository.UpdatePreparacion(preparacion);
                    uow.SaveChanges();

                    uow.PreparacionRepository.RemovePreparacion(dtoPrep.Id);
                    uow.SaveChanges();

                    context.AddErrorNotification(msg, args);
                }
                else
                {
                    context.AddSuccessNotification("PRE052SelecLibOnda_Sec0_Error_Er005_PreparacionXIngresada", [dtoPrep.Id.ToString()]);
                }
            }
            else if (pedidosNoLiberados.Count > 0)
            {
                if (pedidosNoLiberados.Count == 1)
                {
                    var pedido = pedidosNoLiberados.FirstOrDefault();
                    context.AddErrorNotification("PRE052SelecLibOnda_msg_Error_ImposibleIngresarPedido", [pedido.Id, pedido.Cliente]);
                }
                else
                {
                    var pedidos = pedidosNoLiberados.Select(p => $"{p.Id} - {p.Cliente}");
                    var args = string.Join(";", pedidos);
                    context.AddErrorNotification("PRE052SelecLibOnda_msg_Error_ImposibleIngresarPedidos", [args]);
                }
            }

            return context;
        }

        public virtual IEnumerable<LiberacionOndaPedido> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context, Preparacion preparacion, out decimal porcentajeVidaUtil, out bool diferentesPorcentajesVidaUtil)
        {
            var keysSelected = context.Selection.GetSelection(this.GridKeys)
                .Select(k => new LiberacionOndaPedido
                {
                    Pedido = k["NU_PEDIDO"],
                    Empresa = int.Parse(k["CD_EMPRESA"]),
                    Cliente = k["CD_CLIENTE"],
                }).ToList();

            var dbQuery = new PendienteLiberacionQuery(preparacion.Empresa, preparacion.Onda, preparacion.Predio);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistros();

            IEnumerable<LiberacionOndaPedido> keys = null;

            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.Pedido, ks.Empresa, ks.Cliente },
                    s => new { s.Pedido, s.Empresa, s.Cliente },
                    (ks, s) => ks).ToList();

                keys = seleccion.Except(registros).ToList();
            }
            else
            {
                keys = seleccion.Join(keysSelected,
                    ks => new { ks.Pedido, ks.Empresa, ks.Cliente },
                    s => new { s.Pedido, s.Empresa, s.Cliente },
                    (ks, s) => ks).ToList();
            }

            diferentesPorcentajesVidaUtil = false;
            porcentajeVidaUtil = 0;

            if (keys.Count() > 0)
            {
                if (keys.Min(k => k.PorcentajeVidaUtil) != keys.Max(k => k.PorcentajeVidaUtil))
                    diferentesPorcentajesVidaUtil = true;

                porcentajeVidaUtil = Math.Min(keys.Min(k => k.PorcentajeVidaUtil), 100);
            }

            return keys;
        }

        public virtual void SetCondicionesLiberacion(List<ComponentParameter> parameters, IUnitOfWork uow, Preparacion dtoPrep)
        {
            var paramCondiciones = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.Find(x => x.Id == "condicionesLiberacion").Value);
            var colCondiciones = new List<CondicionLiberacion>();
            foreach (var condicion in paramCondiciones)
            {
                if (condicion.Selected)
                    colCondiciones.Add(uow.LiberacionRepository.GetCondicion(condicion.Id));
            }

            uow.LiberacionRepository.AsignarCondicionesLiberacion(dtoPrep.Onda ?? 0, dtoPrep.Empresa ?? 0, colCondiciones);
        }

        public virtual void SeleccionPedidoCompatible(IUnitOfWork uow, short cdOnda, int idEmpresa, out bool valido, out string msg, out List<string> msgArgs)
        {
            msg = string.Empty;

            var dbQuery = new PedidosCompatiblesLiberacionQuery(idEmpresa, cdOnda);

            uow.HandleQuery(dbQuery);

            msgArgs = dbQuery.SeleccionPedidoCompatible(out valido);

            if (!valido)
                msg = "PRE052SelecLibOnda_Sec0_Error_Er007_ImposibleLiberarLoteAuto";

        }

        #endregion
    }
}
