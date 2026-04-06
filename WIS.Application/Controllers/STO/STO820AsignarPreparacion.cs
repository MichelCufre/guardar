using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Stock;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Documento.TiposDocumento;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Logic;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.StockEntities.Constants;
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
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.STO
{
    public class STO820AsignarPreparacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridExcelService _excelService;
        protected readonly IParameterService _parameterService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IBarcodeService _barcodeService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }
        protected readonly ITrackingService _trackingService;
        protected readonly ITaskQueueService _taskQueue;

        public STO820AsignarPreparacion(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService,
            IGridExcelService excelService,
            IParameterService parameterService,
            ITrafficOfficerService concurrencyControl,
            IBarcodeService barcodeService,
            ITrackingService trackingService,
            ITaskQueueService taskQueueService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;
            this._excelService = excelService;
            this._parameterService = parameterService;
            this._concurrencyControl = concurrencyControl;
            this._barcodeService = barcodeService;
            this._trackingService = trackingService;
            this._taskQueue = taskQueueService;
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO",
                "CD_CLIENTE",
                "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Descending)
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (!long.TryParse(context.GetParameter("idTraspaso"), out long idTraspaso))
                throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

            using var uow = this._uowFactory.GetUnitOfWork();

            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);
            if (!this._security.IsEmpresaAllowed(traspaso.EmpresaOrigen) || !this._security.IsEmpresaAllowed(traspaso.EmpresaDestino))
                throw new ValidationFailedException("STO820_Sec0_Error_EmpresaNoAsociada");

            if (traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen
                || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente)
            {
                context.AddParameter("showGrid", "true");
            }
            else
            {
                context.AddParameter("showGrid", "false");
            }

            if ((traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoSeleccion || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente) && traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
                context.AddParameter("showBtnSeleccion", "true");
            else
                context.AddParameter("showBtnSeleccion", "false");

            var empresa = traspaso.EmpresaOrigen;

            this.InicializarFormSelects(uow, form, traspaso, empresa);

            context.AddParameter("empresaOrigen", empresa.ToString());

            var config = uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(traspaso.EmpresaOrigen);
            if (config == null)
            {
                context.AddParameter("empresaOrigenSinConfig", "true");

                form.GetField("nuPreparacion").Disabled = true;
                form.GetField("nuPreparacion").ReadOnly = false;

                context.AddInfoNotification("STO820_Sec0_Error_EmpresaNoConfigurada");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!long.TryParse(context.GetParameter("idTraspaso"), out long idTraspaso))
                throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

            return this._formValidationService.Validate(new TraspasoEmpresasPreparacionValidationModule(uow, this._identity, this._security, traspaso), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            if (context.ButtonId == "btnAsignarPreparacion")
            {
                if (!long.TryParse(context.GetParameter("idTraspaso"), out long idTraspaso))
                    throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

                var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);
                var empresa = traspaso.EmpresaOrigen;

                var preparacion = context.GetParameter("nuPreparacion");
                InicializarFormSelects(uow, form, traspaso, empresa, preparacion);
            }

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            using var uow = this._uowFactory.GetUnitOfWork();
            if (!int.TryParse(context.Parameters.Find(x => x.Id == "idTraspaso")?.Value, out int idTraspaso))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

            if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
            {
                grid.AddOrUpdateColumn(new GridColumnSelect("TP_EXPEDICION_DESTINO", this.SelectTipoExpedicionDestino(uow)));
            }

            if (traspaso.Estado != TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION && traspaso.Estado != TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO)
            {
                context.IsEditingEnabled = false;
                context.IsCommitEnabled = false;
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "idTraspaso")?.Value, out int idTraspaso))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            int nuPreparacion = -1;
            var strNuPrep = context.Parameters.Find(x => x.Id == "nuPreparacion")?.Value;
            if (!string.IsNullOrEmpty(strNuPrep) && int.TryParse(strNuPrep, out int nuPrep))
                nuPreparacion = nuPrep;

            var dbQuery = new TraspasoEmpresasDetallePedidosQuery(uow, idTraspaso, nuPreparacion);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

            if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
                grid.SetEditableCells(new List<string> { "NU_PEDIDO_DESTINO", "CD_CLIENTE_DESTINO", "TP_PEDIDO_DESTINO", "TP_EXPEDICION_DESTINO" });
            else if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO)
                grid.SetEditableCells(new List<string> { "NU_PEDIDO_DESTINO" });

            var confPedidoDestino = context.Parameters.Find(x => x.Id == "confPedidoDestino")?.Value;

            foreach (var row in grid.Rows)
            {
                var pedido = row.GetCell("NU_PEDIDO");
                pedido.Modified = true;
            }

            if (confPedidoDestino == TraspasoEmpresasDb.MISMO_NUMERO)
            {
                foreach (var row in grid.Rows)
                {
                    row.GetCell("NU_PEDIDO_DESTINO").Value = row.GetCell("NU_PEDIDO").Value;
                    row.GetCell("NU_PEDIDO_DESTINO").Editable = false;
                }
            }
            else if (confPedidoDestino == TraspasoEmpresasDb.MISMO_CRITERIO)
            {
                foreach (var row in grid.Rows)
                {
                    row.GetCell("TP_EXPEDICION_DESTINO").Value = row.GetCell("TP_EXPEDICION").Value;
                    row.GetCell("TP_PEDIDO_DESTINO").Value = row.GetCell("TP_PEDIDO").Value;
                    row.GetCell("TP_EXPEDICION_DESTINO").Editable = false;
                    row.GetCell("TP_PEDIDO_DESTINO").Editable = false;
                }
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            try
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                if (int.TryParse(context.Parameters.Find(x => x.Id == "idTraspaso")?.Value, out int idTraspaso))
                {
                    var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

                    if (traspaso.TipoTraspaso != TipoTraspasoDb.TraspasoPreparacionOrigen
                        && traspaso.TipoTraspaso != TipoTraspasoDb.TraspasoPreparacionPendiente)
                        return grid;
                }

                var confPedidoDestino = context.Parameters.Find(x => x.Id == "confPedidoDestino")?.Value;

                var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
                var configuracion = expedicionService.GetConfiguracionPedido();

                return this._gridValidationService.Validate(new TraspasoEmpresasDetallePedidosValidationModule(uow, configuracion, confPedidoDestino), grid, row, context);
            }
            catch (Exception ex)
            {

            }
            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();
            List<string> keysAjuste = new List<string>();

            try
            {


                if (!int.TryParse(context.Parameters.Find(x => x.Id == "idTraspaso")?.Value, out int idTraspaso))
                    throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");
                else if (!uow.TraspasoEmpresasRepository.AnyTraspaso(idTraspaso))
                    throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

                if (!int.TryParse(context.Parameters.Find(x => x.Id == "nuPreparacion")?.Value, out int nuPreparacion))
                    throw new ValidationFailedException("STO820_Sec0_Error_PreparacionNoValida");

                var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

                var config = uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(traspaso.EmpresaOrigen);
                if (config == null)
                {
                    throw new ValidationFailedException("STO820_Sec0_Error_EmpresaNoConfigurada");
                }

                if (traspaso.Estado != TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION && traspaso.Estado != TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO)
                    throw new ValidationFailedException("STO820_Sec0_Error_EstadoTraspasoNoValido");

                if (!this._security.IsEmpresaAllowed(traspaso.EmpresaOrigen) || !this._security.IsEmpresaAllowed(traspaso.EmpresaDestino))
                    throw new ValidationFailedException("STO820_Sec0_Error_EmpresaNoAsociada");

                if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
                {
                    uow.CreateTransactionNumber("STO820 Asignación de Preparación Traspaso Empresas");
                    uow.BeginTransaction();

                    AsignarPreparacion(grid, context, uow, idTraspaso, nuPreparacion, traspaso);

                }
                else if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO)
                {
                    if (context.Parameters.Find(x => x.Id == "buttonId")?.Value == "btnSubmitGuardar")
                        uow.CreateTransactionNumber("STO820 Actualización Detalle de Pedidos Traspaso Empresas");
                    else
                        uow.CreateTransactionNumber("STO820 Ejecución/Finalización Traspaso Empresas");

                    uow.BeginTransaction();

                    UpdateDatosPedidosTraspaso(grid, uow, traspaso);



                    if (context.Parameters.Find(x => x.Id == "buttonId")?.Value == "btnSubmitEjecutarTraspaso")
                    {
                        if (traspaso.TipoTraspaso != TipoTraspasoDb.TraspasoPda)
                        {
                            var empresaOrigen = traspaso.EmpresaOrigen;
                            var empresaDestino = traspaso.EmpresaDestino;

                            TraspasoEmpresaLogic traspasoLogic = new TraspasoEmpresaLogic(_identity, _concurrencyControl, _barcodeService);

                            var bulkContext = traspasoLogic.ProcesarTraspasoEntreEmpresa(uow, uow.GetTransactionNumber(), traspaso.Id, traspaso.Preparacion ?? -1, traspaso.TipoTraspaso, empresaOrigen, empresaDestino, transactionTO, out List<Agente> agentes, out List<Pedido> pedidosNew);

                            int? preparacion = bulkContext.PreparacionDestino?.Id;
                            DocumentoIngreso documentoIngreso = bulkContext.DocumentoEntrada;
                            DocumentoEgreso DocumentoSalida = bulkContext.DocumentoSalida;
                            keysAjuste = bulkContext.AjustesStock.Select(x => x.NuAjusteStock.ToString()).ToList();

                            traspaso.PreparacionDestino = preparacion;

                            if (documentoIngreso != null)
                            {
                                traspaso.DocumentoIngreso = documentoIngreso.Numero;
                                traspaso.TipoDocumentoIngreso = documentoIngreso.Tipo;
                            }

                            if (DocumentoSalida != null)
                            {
                                traspaso.DocumentoEgreso = DocumentoSalida.Numero;
                                traspaso.TipoDocumentoEgreso = DocumentoSalida.Tipo;
                            }
                            if (agentes != null && agentes.Count() > 0)
                            {
                                foreach (var keyAgente in agentes)
                                {
                                    var agente = uow.AgenteRepository.GetAgente(keyAgente.Empresa, keyAgente.CodigoInterno);
                                    _trackingService.SincronizarAgente(agente, false);
                                    uow.AgenteRepository.UpdateAgente(agente);
                                    uow.SaveChanges();
                                }
                            }
                            if (pedidosNew != null && pedidosNew.Count() > 0)
                            {
                                foreach (var keyPedidosNew in pedidosNew)
                                {
                                    var agente = uow.AgenteRepository.GetAgente(keyPedidosNew.Empresa, keyPedidosNew.Cliente);
                                    var pedido = uow.PedidoRepository.GetPedido(keyPedidosNew.Empresa, keyPedidosNew.Cliente, keyPedidosNew.Id);
                                    _trackingService.SincronizarPedido(uow, pedido, agente, false);
                                    uow.PedidoRepository.UpdatePedido(pedido);
                                    uow.SaveChanges();
                                }
                            }
                        }

                        traspaso.Estado = TraspasoEmpresasDb.ESTADO_TRASPASO_FINALIZADO;
                        traspaso.FechaModificacion = DateTime.Now;

                        uow.TraspasoEmpresasRepository.UpdateTraspaso(traspaso);
                        uow.SaveChanges();

                    }
                }

                uow.SaveChanges();
                uow.Commit();

                if (_taskQueue.IsEnabled() && keysAjuste.Count() > 0)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keysAjuste);

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }

            return grid;
        }

        public virtual void UpdateDatosPedidosTraspaso(Grid grid, IUnitOfWork uow, TraspasoEmpresas traspaso)
        {
            if (traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen
                || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente)
            {
                foreach (var row in grid.Rows)
                {
                    var nuPedido = row.GetCell("NU_PEDIDO").Value;
                    var cdCliente = row.GetCell("CD_CLIENTE").Value;
                    var cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

                    var detalle = uow.TraspasoEmpresasRepository.GetDetallePedido(traspaso.Id, nuPedido, cdCliente, cdEmpresa);
                    var pedidoDestino = string.IsNullOrEmpty(detalle.PedidoDestino) ? string.Empty : detalle.PedidoDestino;
                    if (pedidoDestino != row.GetCell("NU_PEDIDO_DESTINO").Value)
                    {
                        detalle.PedidoDestino = row.GetCell("NU_PEDIDO_DESTINO").Value;
                        uow.TraspasoEmpresasRepository.UpdateDetallePedido(detalle);
                        uow.SaveChanges();
                    }
                }
            }
        }

        public virtual void AsignarPreparacion(Grid grid, GridFetchContext context, IUnitOfWork uow, int idTraspaso, int nuPreparacion, TraspasoEmpresas traspaso)
        {
            var fechaActual = DateTime.Now;

            if (traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen
                || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente)
            {
                traspaso.ConfigPedidoDestino = context.Parameters.Find(x => x.Id == "confPedidoDestino")?.Value;
            }

            _concurrencyControl.AddLock("T_PICKING", nuPreparacion.ToString());

            traspaso.Preparacion = nuPreparacion;
            traspaso.Estado = TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO;
            traspaso.FechaModificacion = fechaActual;

            uow.TraspasoEmpresasRepository.UpdateTraspaso(traspaso);
            uow.SaveChanges();

            if (traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen
                || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente)
            {
                List<TraspasoEmpresasDetallePedido> detPedidos = new List<TraspasoEmpresasDetallePedido>();

                foreach (var row in grid.Rows)
                {
                    var nuPedido = row.GetCell("NU_PEDIDO").Value;
                    var cdCliente = row.GetCell("CD_CLIENTE").Value;
                    var cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    var tpExpedicion = row.GetCell("TP_EXPEDICION").Value;
                    var tpPedido = row.GetCell("TP_PEDIDO").Value;
                    var nuPedidoDestino = row.GetCell("NU_PEDIDO_DESTINO").Value;
                    var cdClienteDestino = row.GetCell("CD_CLIENTE_DESTINO").Value;
                    var cdEmpresaDestino = int.Parse(row.GetCell("CD_EMPRESA_DESTINO").Value);
                    var tpExpedicionDestino = row.GetCell("TP_EXPEDICION_DESTINO").Value;
                    var tpPedidoDestino = row.GetCell("TP_PEDIDO_DESTINO").Value;

                    detPedidos.Add(new TraspasoEmpresasDetallePedido
                    {
                        Traspaso = idTraspaso,
                        PedidoOrigen = nuPedido,
                        ClienteOrigen = cdCliente,
                        EmpresaOrigen = cdEmpresa,
                        PedidoDestino = nuPedidoDestino,
                        ClienteDestino = cdClienteDestino,
                        EmpresaDestino = cdEmpresaDestino,
                        TipoPedidoDestino = tpPedidoDestino,
                        TipoExpedicionDestino = tpExpedicionDestino,
                        FechaAlta = fechaActual,
                    });
                }

                foreach (var det in detPedidos)
                {
                    uow.TraspasoEmpresasRepository.AddDetallePedido(det);
                    uow.SaveChanges();
                }
            }
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_CLIENTE_DESTINO":
                    return this.SearchClienteDestino(grid, row, context);
                case "TP_PEDIDO_DESTINO":
                    return this.SearchTipoPedidoDestino(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "idTraspaso")?.Value, out int idTraspaso))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "nuPreparacion")?.Value, out int nuPreparacion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new TraspasoEmpresasDetallePedidosQuery(uow, idTraspaso, nuPreparacion);

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

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "idTraspaso")?.Value, out int idTraspaso))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "nuPreparacion")?.Value, out int nuPreparacion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new TraspasoEmpresasDetallePedidosQuery(uow, idTraspaso, nuPreparacion);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            context.FileName = $"{this._identity.Application}_{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        #region Metodos Auxiliares 

        public virtual void InicializarFormSelects(IUnitOfWork uow, Form form, TraspasoEmpresas traspaso, int empresa, string preparacion = "")
        {
            var selectPreparacion = form.GetField("nuPreparacion");
            selectPreparacion.Options = new List<SelectOption>();

            var selectConfigPedidoDestino = form.GetField("confPedidoDestino");
            selectConfigPedidoDestino.Options = new List<SelectOption>();

            uow.DominioRepository.GetDominios(CodigoDominioDb.ConfigTraspasoPedidoDestino)?.ForEach(w =>
            {
                selectConfigPedidoDestino.Options.Add(new SelectOption(w.Id, w.Descripcion));
            });
            selectConfigPedidoDestino.Value = TraspasoEmpresasDb.MISMO_NUMERO;

            if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
            {
                if (traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoSeleccion
                || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente)
                {
                    uow.TraspasoEmpresasRepository.GetPreparacionesPendientes(empresa).ForEach(w =>
                    {
                        selectPreparacion.Options.Add(new SelectOption(w.Key.ToString(), $"{w.Key.ToString()} - {w.Value}"));
                    });
                }
                else if (traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPda
                    || traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen)
                {

                    uow.TraspasoEmpresasRepository.GetPreparacionesTodoPreparado(empresa).ForEach(w =>
                    {
                        selectPreparacion.Options.Add(new SelectOption(w.Key.ToString(), $"{w.Key.ToString()} - {w.Value}"));
                    });
                }

                selectPreparacion.Value = preparacion;
            }
            else
            {
                var dsPreparacion = uow.PreparacionRepository.GetPreparacionPorNumero(traspaso.Preparacion.Value).Descripcion;
                selectPreparacion.Options.Add(new SelectOption(traspaso.Preparacion.ToString(), $"{traspaso.Preparacion.ToString()} - {dsPreparacion}"));
                selectPreparacion.Value = traspaso.Preparacion.ToString();

                selectConfigPedidoDestino.Value = traspaso.ConfigPedidoDestino;
                selectPreparacion.ReadOnly = true;
                selectConfigPedidoDestino.ReadOnly = true;
            }
        }

        public virtual List<SelectOption> SearchClienteDestino(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            var empresa = row.GetCell("CD_EMPRESA_DESTINO").Value;

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();

            if (configuracion.PermitePedidosAProveedores)
            {
                var clientes = uow.AgenteRepository.GetByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Tipo} - {cliente.Codigo} - {cliente.Descripcion} "));
                }
            }
            else
            {
                var clientes = uow.AgenteRepository.GetClienteByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Codigo} - {cliente.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SelectTipoExpedicionDestino(IUnitOfWork uow)
        {
            var configuraciones = uow.PedidoRepository.GetConfiguracionesExpedicion();

            var listaConfiguraciones = new List<SelectOption>();

            foreach (var configuracion in configuraciones)
            {
                listaConfiguraciones.Add(new SelectOption(configuracion.Tipo, configuracion.Descripcion));
            }

            return listaConfiguraciones;
        }

        public virtual List<SelectOption> SearchTipoPedidoDestino(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();
            string tpExpedicion = row.GetCell("TP_EXPEDICION_DESTINO").Value;

            if (string.IsNullOrEmpty(tpExpedicion))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();
            var tipos = uow.PedidoRepository.GetTiposPedido(tpExpedicion);

            foreach (var tipo in tipos)
            {
                opciones.Add(new SelectOption(tipo.Key, tipo.Value));
            }

            return opciones;
        }

        #endregion
    }
}
