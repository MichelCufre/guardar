using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.CodigoMultidato;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Reportes;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.EXP
{
    public class EXP330PedidosMostrador : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IDapper _dapper;
        protected readonly IParameterService _parameterService;
        protected readonly IFactoryService _factoryService;
        protected readonly IReportKeyService _reporteKeyService;
        protected readonly ICodigoMultidatoService _codigoMultidatoService;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeys { get; }
        protected List<string> Grid2Keys { get; }
        protected List<string> Grid3Keys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP330PedidosMostrador(
            ISessionAccessor session,
            IIdentityService identity,
            IGridService gridService,
            ITaskQueueService taskQueue,
            IUnitOfWorkFactory uowFactory,
            IFilterInterpreter filterInterpreter,
            IFormValidationService formValidationService,
            IBarcodeService barcodeService,
            IDapper dapper,
            IParameterService parameterService,
            IFactoryService factoryService,
            IReportKeyService reporteKeyService,
            ICodigoMultidatoService codigoMultidatoService,
            ITrafficOfficerService concurrencyControl)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION","NU_CONTENEDOR","NU_PEDIDO","CD_EMPRESA", "CD_CLIENTE", "NU_CARGA"
            };

            this.Grid2Keys = new List<string>
            {
                "NU_PREPARACION","NU_CONTENEDOR","NU_PEDIDO","CD_CLIENTE", "CD_EMPRESA"
            };

            this.Grid3Keys = new List<string>
            {
                "NU_PREPARACION","NU_CONTENEDOR","CD_PRODUTO","CD_EMPRESA", "NU_IDENTIFICADOR","NU_PEDIDO","CD_CLIENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Descending)
            };

            this._session = session;
            this._identity = identity;
            this._taskQueue = taskQueue;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
            this._barcodeService = barcodeService;
            this._dapper = dapper;
            this._parameterService = parameterService;
            this._factoryService = factoryService;
            this._reporteKeyService = reporteKeyService;
            this._codigoMultidatoService = codigoMultidatoService;
            this._concurrencyControl = concurrencyControl;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            if (context.Parameters.Count > 0)
            {
                var nuPedido = context.GetParameter("NuPedido");

                if (string.IsNullOrEmpty(nuPedido))
                    return grid;

                var cdCliente = context.GetParameter("CdCliente");
                var empresa = int.Parse(context.GetParameter("CdEmpresa"));

                using var uow = this._uowFactory.GetUnitOfWork();

                if (grid.Id == ("EXP330_grid_1"))
                {
                    var dbQuery = new PedidosMostradorQuery(nuPedido, cdCliente, empresa);
                    uow.HandleQuery(dbQuery);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
                }
                else if (grid.Id == "EXP330_grid_2")
                {
                    var dbQuery = new ContenedoresPendientesCarga(nuPedido, cdCliente, empresa);
                    uow.HandleQuery(dbQuery);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.Grid2Keys);
                }
                else
                {
                    var dbQuery = new DetallesPedidosMostrador(nuPedido, cdCliente, empresa);
                    uow.HandleQuery(dbQuery);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.Grid3Keys);

                    grid.Rows.ForEach(w =>
                    {
                        if (w.GetCell("ID_TEMPORAL").Value == "S")
                            w.CssClass = "green";
                    });
                }
            }
            else
                grid.Rows = new List<GridRow>();

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            if (context.Parameters.Count > 0)
            {
                var nuPedido = context.GetParameter("NuPedido");

                if (string.IsNullOrEmpty(nuPedido))
                    return null;

                var cdCliente = context.GetParameter("CdCliente");
                var empresa = int.Parse(context.GetParameter("CdEmpresa"));

                using var uow = this._uowFactory.GetUnitOfWork();

                if (grid.Id == ("EXP330_grid_1"))
                {
                    var dbQuery = new PedidosMostradorQuery(nuPedido, cdCliente, empresa);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else if (grid.Id == "EXP330_grid_2")
                {
                    var dbQuery = new ContenedoresPendientesCarga(nuPedido, cdCliente, empresa);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else
                {
                    var dbQuery = new DetallesPedidosMostrador(nuPedido, cdCliente, empresa);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
            }
            else
                return null;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (_identity.Predio == GeneralDb.PredioSinDefinir)
            {
                form.GetField("NuContenedor").Disabled = true;
                context.AddErrorNotification("EXP330_form1_Error_NoAccesoPantalla");
            }

            form.GetField("NuPedido").Value = context.GetParameter("NuPedido");
            form.GetField("CdCliente").Value = context.GetParameter("CdCliente");
            form.GetField("CdEmpresa").Value = context.GetParameter("CdEmpresa");
            form.GetField("NmEmpresa").Value = context.GetParameter("NmEmpresa");
            form.GetField("DsCliente").Value = context.GetParameter("DsCliente");

            if (!string.IsNullOrEmpty(form.GetField("NuPedido").Value))
            {
                form.GetButton("BtnFacturar").Disabled = (context.GetParameter("IsEnabledBtnFacturar") ?? "False") == "False";
                form.GetButton("BtnExpedir").Disabled = (context.GetParameter("IsEnabledBtnExpedir") ?? "False") == "False";
                form.GetButton("BtnDescartar").Disabled = (form.GetButton("BtnFacturar").Disabled && form.GetButton("BtnExpedir").Disabled);
            }
            else
            {
                form.GetButton("BtnFacturar").Disabled = true;
                form.GetButton("BtnExpedir").Disabled = true;
                form.GetButton("BtnDescartar").Disabled = true;
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var nuContenedor = form.GetField("NuContenedor").Value;
                int? cdEmpresa = null;

                if (!string.IsNullOrEmpty(form.GetField("CdEmpresa").Value))
                {
                    cdEmpresa = int.Parse(form.GetField("CdEmpresa").Value);
                }

                if (cdEmpresa == null && !string.IsNullOrEmpty(context.GetParameter("empresaSubmit")))
                {
                    cdEmpresa = int.Parse(context.GetParameter("empresaSubmit"));
                }

                try
                {
                    var resultadoAIs = _codigoMultidatoService.GetAIs(uow, "EXP330", nuContenedor, new Dictionary<string, string>
                    {
                        ["USERID"] = _identity.UserId.ToString(),
                        ["NU_PREDIO"] = _identity.Predio,
                        ["CD_CAMPO"] = "NuContenedor",
                    }, cdEmpresa, tipoLectura: CodigoMultidatoTipoLectura.LPN).GetAwaiter().GetResult();

                    var ais = resultadoAIs?.AIs;

                    if (cdEmpresa == null && resultadoAIs?.Empresa != null)
                    {
                        cdEmpresa = resultadoAIs.Empresa;
                    }

                    if (ais != null && ais.ContainsKey("NuContenedor"))
                    {
                        nuContenedor = ais["NuContenedor"].ToString();
                    }

                    _barcodeService.ValidarEtiquetaContenedor(nuContenedor, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa, cdEmpresa: cdEmpresa);

                    var idLock = $"{datosContenedor.NuPreparacion}#{datosContenedor.NuContenedor}#{datosContenedor.TipoContenedor}#{datosContenedor.IdExternoContenedor}";
                    if (this._concurrencyControl.IsLocked("T_CONTENEDOR", idLock))
                        throw new ValidationFailedException("General_msg_Error_ContenedorBloqueado", new string[] { datosContenedor.TipoContenedor, datosContenedor.IdExternoContenedor });

                    _concurrencyControl.AddLock("T_CONTENEDOR", idLock);

                    if (cantidadEmpresa > 1)
                    {
                        var empresas = uow.ContenedorRepository.GetEmpresasByCodigoBarrasContenedorLpn(nuContenedor, _identity.UserId, cdEmpresa, false);
                        var jsonEmpresas = JsonConvert.SerializeObject(empresas.Select(e => new { e.Id, Value = e.Nombre }));
                        context.Parameters.Add(new ComponentParameter { Id = "empresas", Value = jsonEmpresas });
                        context.Parameters.Add(new ComponentParameter { Id = "NuContenedor", Value = form.GetField("NuContenedor").Value });
                    }
                    else
                    {
                        var pedidoMostrador = GetDatosPedidoMostrador(uow, datosContenedor);

                        if (!uow.PedidoRepository.ExistePedidoMostrador(pedidoMostrador.NuPreparacion, pedidoMostrador.NuContenedor, pedidoMostrador.NuPedido, pedidoMostrador.CodigoEmpresa, pedidoMostrador.CodigoCliente))
                        {
                            var temp = new TempPedidoMostrador()
                            {
                                CodigoUbicacion = pedidoMostrador.Ubicacion,
                                NumeroPedido = pedidoMostrador.NuPedido,
                                CodigoCliente = pedidoMostrador.CodigoCliente,
                                CodigoEmpresa = pedidoMostrador.CodigoEmpresa,
                                NumeroPreparacion = pedidoMostrador.NuPreparacion,
                                NumeroContenedor = pedidoMostrador.NuContenedor,
                                CodigoCamionFacturado = uow.ContenedorRepository.GetContenedoresEnPreparacion(pedidoMostrador.NuPreparacion, pedidoMostrador.NuContenedor)?.CamionFacturado
                            };

                            var cont = new ContenedorFacturar()
                            {
                                CodigoCliente = pedidoMostrador.CodigoCliente,
                                CodigoEmpresa = pedidoMostrador.CodigoEmpresa,
                                NumeroPedido = pedidoMostrador.NuPedido,
                                NumeroPreparacion = pedidoMostrador.NuPreparacion,
                                NumeroContenedor = pedidoMostrador.NuContenedor,
                            };

                            var nuCargarCont = uow.PreparacionRepository.GetCargaContenedor(cont) ?? 0;
                            var cargaNueva = uow.PreparacionRepository.CopiarCarga(nuCargarCont);

                            uow.SaveChanges();
                            uow.CreateTransactionNumber("Cambiar carga al asignar al camion por retira");

                            var datosPickingContenedor = uow.PreparacionRepository.GetDatosPickingContenedor(cont);
                            foreach (var a in datosPickingContenedor)
                            {
                                a.Carga = cargaNueva;
                                a.Transaccion = uow.GetTransactionNumber();
                                uow.PreparacionRepository.UpdateDetallePreparacion(a);
                            }

                            temp.NumeroCarga = cargaNueva;

                            uow.PedidoRepository.AddPedidoMostrador(temp);
                            uow.SaveChanges();
                        }

                        context.Parameters.Add(new ComponentParameter { Id = "OnLoad", Value = "true" });
                        context.Parameters.Add(new ComponentParameter { Id = "NuPedido", Value = pedidoMostrador.NuPedido });
                        context.Parameters.Add(new ComponentParameter { Id = "CdCliente", Value = pedidoMostrador.CodigoCliente });
                        context.Parameters.Add(new ComponentParameter { Id = "CdEmpresa", Value = pedidoMostrador.CodigoEmpresa.ToString() });
                        context.Parameters.Add(new ComponentParameter { Id = "NmEmpresa", Value = pedidoMostrador.NombreEmpresa });
                        context.Parameters.Add(new ComponentParameter { Id = "DsCliente", Value = pedidoMostrador.DescripcionCliente });
                        context.Parameters.Add(new ComponentParameter { Id = "NuContenedor", Value = datosContenedor.NuContenedor.ToString() });
                        context.Parameters.Add(new ComponentParameter { Id = "NuPreparacion", Value = datosContenedor.NuPreparacion.ToString() });

                        var parameters = context.Parameters;

                        this.HabilitarBotones(uow, ref parameters);
                    }
                }
                catch (TooManyEmpresaCodigoMultidatoException ex)
                {
                    var empresas = uow.ContenedorRepository.GetEmpresasByCodigoBarrasContenedorLpn(ex.Codigo, _identity.UserId, cdEmpresa, true);
                    var jsonEmpresas = JsonConvert.SerializeObject(empresas.Select(e => new { e.Id, Value = e.Nombre }));
                    context.Parameters.Add(new ComponentParameter { Id = "empresas", Value = jsonEmpresas });
                    context.Parameters.Add(new ComponentParameter { Id = "NuContenedor", Value = form.GetField("NuContenedor").Value });
                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EXP330FormValidationModule(uow, this._session, this._identity, this._barcodeService, this._codigoMultidatoService), form, context);
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var keysReportes = new List<string>();
            var keysCamionesExpedicion = new List<string>();
            var keysCamionesFacturacion = new List<string>();

            try
            {
                var isDeleteParameters = false;
                uow.BeginTransaction();

                var pedido = context.GetParameter("NuPedido");

                if (string.IsNullOrEmpty(pedido))
                    return form;

                var cliente = context.GetParameter("CdCliente");
                var empresa = int.Parse(context.GetParameter("CdEmpresa"));

                var nuContenedor = int.Parse(context.GetParameter("NuContenedor"));
                var nuPreparacion = int.Parse(context.GetParameter("NuPreparacion"));
                var contenedor = uow.ContenedorRepository.GetContenedor(nuPreparacion, nuContenedor);
                var idLock = $"{contenedor.NumeroPreparacion}#{contenedor.Numero}#{contenedor.TipoContenedor}#{contenedor.IdExterno}";

                if (context.ButtonId == "BtnDescartar")
                {
                    uow.CreateTransactionNumber("Descartar");

                    var pedidos = uow.PedidoRepository.GetPedidoMostradorSinFac(pedido, cliente, empresa);
                    foreach (var p in pedidos)
                    {
                        uow.PedidoRepository.RemovePedidoMostrador(p);
                    }

                    isDeleteParameters = true;

                    _concurrencyControl.RemoveLockByIdLock("T_CONTENEDOR", idLock, _identity.UserId);
                }
                else if (context.ButtonId == "BtnExpedir")
                {
                    uow.CreateTransactionNumber("Expedir");
                    var exp = new ProcesoExpedicion(_dapper, _parameterService, _identity, _factoryService, _reporteKeyService, _barcodeService, _taskQueue);

                    var pedidosMostradorContenedor = uow.PedidoRepository.GetsPedidosMostradorExpedicion(pedido, cliente, empresa);
                    exp.ExpedirCamion(uow, pedidosMostradorContenedor, out keysCamionesExpedicion, out keysReportes);

                    foreach (var cont in pedidosMostradorContenedor)
                    {
                        var temp = uow.PedidoRepository.GetPedidoMostrador(cont);
                        uow.PedidoRepository.RemovePedidoMostrador(temp);
                    }

                    isDeleteParameters = true;

                    _concurrencyControl.RemoveLockByIdLock("T_CONTENEDOR", idLock, _identity.UserId);
                }
                else if (context.ButtonId == "BtnFacturar")
                {
                    uow.CreateTransactionNumber("Facturar");

                    var pedidosPendientesFacturar = uow.PedidoRepository.GetPedidoMostradorSinFacturar(pedido, cliente, empresa, out List<int> contenedoresConProblemas);

                    if (contenedoresConProblemas.Count > 0)
                    {
                        var args = new List<string>() { string.Join(",", contenedoresConProblemas) };
                        context.AddInfoNotification("EXP330_form1_Error_ContenedoresAsignadosCamionEsgreso", args);
                    }

                    FacturacionLegacy.FacturarPedido(uow, pedidosPendientesFacturar, _identity.Predio, _identity.UserId, out keysCamionesFacturacion, out List<string> errores);

                    if (errores.Count > 0)
                        throw new ValidationFailedException(errores[0]);

                    foreach (var cont in pedidosPendientesFacturar)
                    {
                        var temp = uow.PedidoRepository.GetPedidoMostrador(cont);
                        temp.CodigoCamionFacturado = cont.CodigoCamion;
                        uow.PedidoRepository.UpdatePedidoMostrador(temp);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                var parameters = context.Parameters;

                this.HabilitarBotones(uow, ref parameters, isDeleteParameters);

                context.Parameters = parameters;

                if (_taskQueue.IsEnabled() && keysCamionesExpedicion.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionDePedido, keysCamionesExpedicion);

                if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing() && keysReportes.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.REPORT, keysReportes);

                if (_taskQueue.IsEnabled() && keysCamionesFacturacion.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.Facturacion, keysCamionesFacturacion);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? []));
                uow.Rollback();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
            
            return form;
        }

        public virtual void HabilitarBotones(IUnitOfWork uow, ref List<ComponentParameter> parameters, bool isDeleteParameters = false)
        {
            var cdEmpresa = int.Parse(parameters.FirstOrDefault(w => w.Id == "CdEmpresa")?.Value);

            var isTodoFacturado = uow.PedidoRepository.TodoFacturado(parameters.FirstOrDefault(w => w.Id == "NuPedido")?.Value, parameters.FirstOrDefault(w => w.Id == "CdCliente")?.Value, cdEmpresa);

            if (isDeleteParameters) parameters = new List<ComponentParameter>();

            parameters.Add(new ComponentParameter() { Id = "IsEnabledBtnFacturar", Value = (!isTodoFacturado).ToString() });
            parameters.Add(new ComponentParameter() { Id = "IsEnabledBtnExpedir", Value = (isTodoFacturado).ToString() });
        }

        public virtual PedidoMostrador GetDatosPedidoMostrador(IUnitOfWork uow, AuxContenedor datosContenedor)
        {
            var detPreparacion = uow.PreparacionRepository.GetDetallePreparacion(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

            if (detPreparacion == null)
                throw new ValidationFailedException("General_Sec0_Error_ContenedorNoExiste");

            return new PedidoMostrador()
            {
                NuPreparacion = detPreparacion.NumeroPreparacion,
                NuContenedor = datosContenedor.NuContenedor,
                NuPedido = detPreparacion.Pedido,
                Ubicacion = datosContenedor.Ubicacion,
                CodigoCliente = detPreparacion.Cliente,
                CodigoEmpresa = detPreparacion.Empresa,
                NombreEmpresa = uow.EmpresaRepository.GetNombre(detPreparacion.Empresa),
                DescripcionCliente = uow.AgenteRepository.GetDescripcionAgente(detPreparacion.Empresa, detPreparacion.Cliente),
            };
        }

    }
}
